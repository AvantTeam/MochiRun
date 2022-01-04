using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static ChunkLoader;

public class CursorControl : MonoBehaviour
{
    public const float SNAP = 0.5f, SCROLL_SCALE = 0.5f, CURSOR_RADIUS = 0.4f;

    public bool focused = true;

    public Block block;
    public byte ctype;
    public GameObject cursorPrefab, lblock, ltag;
    public Material cursorMaterial;
    public Color placeColor, invalidColor, removeColor, defaultColor, selectColor;
    public Material additiveSprite, defaultSprite;

    public enum STATE {
        NONE = -1,
        PLACE = 0,
        REMOVE,
        SELECT, //drag, change ctype and rotate selected block(s).
        SELECTING, //dragging across the screen to select multiple blocks. becomes SELECT when the selection(s) is decided.
        PICK_BLOCK,
        CAMERA, //3d camera view, comes with a screenshot option and stuff
        NUM
    };

    public STATE state = STATE.PLACE;
    public GameObject selection = null;
    private bool selectedViaRclick = false;
    private bool rclickErase = false;

    private Block lastBlock; //last placed block
    private byte lastCtype;
    private float rotateScrollDelta;

    public bool dragging = false;
    private bool dragSnapOffsetX, dragSnapOffsetY; //whether drag snap center is set to 0.5, 1.5, 2.5...
    private Collider2D[] colresult = new Collider2D[7];
    private ContactFilter2D triggerContactFilter;

    SpriteRenderer tagRenderer;
    GameObject cam;
    Camera camc;
    EditorCameraControl camController;
    MeshRenderer mrender;
    Collider2D collider2d;
    Projector highlighter;

    void Start()
    {
        tagRenderer = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        cam = GameObject.Find("Main Camera");
        camc = cam.GetComponent<Camera>();
        camController = cam.GetComponent<EditorCameraControl>();
        mrender = GetComponent<MeshRenderer>();
        collider2d = GetComponent<Collider2D>();
        highlighter = GetComponent<Projector>();
        highlighter.enabled = false;
        triggerContactFilter = new ContactFilter2D();
        triggerContactFilter.useTriggers = true;

        Deselect();
        focused = true;
        SetBlock(Blocks.spike);
        SetState(STATE.NONE);
        SetColor(placeColor);
    }

    void LateUpdate()
    {
        focused = !EventSystem.current.IsPointerOverGameObject();
        Show(state == STATE.SELECT ? 
            selection != null && !tagRenderer.gameObject.activeInHierarchy : 
            !(camController.panning || state == STATE.CAMERA || (state == STATE.PLACE && tagRenderer.gameObject.activeInHierarchy)));
        if(focused) {
            playerInputs();
        }
        if(state != STATE.SELECT || selection == null) {
            if(state != STATE.CAMERA) moveToMouse();
        }
        else {
            Color c = selectColor;
            c.a = absin(Time.realtimeSinceStartup, 0.15f, 1f);
            SetColor(c);
        }
    }

    private void moveToMouse() {
        Vector3 mpos = camc.ScreenToWorldPoint(Input.mousePosition);
        mpos.z = (block == null ? 0f : block.zLayer);
        mpos.x = DragSnap(mpos.x, dragSnapOffsetX);
        mpos.y = DragSnap(mpos.y, dragSnapOffsetY); //todo onFloor, and make spike's onFloor false

        transform.position = mpos;
        if(block != null) {
            transform.rotation = block.rotate ? Block.rotation[ctype % 4] : Quaternion.identity;
        }
    }

    private void playerInputs() {
        /*if(!Vars.main.mobile) {
            if(state == STATE.PLACE && Input.GetMouseButton(1)) SetState(STATE.REMOVE);
            else if(state == STATE.REMOVE && !Input.GetMouseButton(1)) SetState(STATE.PLACE);
        }*/

        switch(state) {
            case STATE.NONE:
                SetColor(defaultColor);

                if(Input.GetMouseButtonDown(1)) {
                    trySelect(true);
                }
                break;
            case STATE.PLACE:
                if(block == null) {
                    dragging = false;
                    SetState(STATE.NONE); //what went wrong?
                }
                else {
                    if(block.rotate) {
                        float scroll = Input.mouseScrollDelta.y;
                        if(Mathf.Abs(scroll) > SCROLL_SCALE) {
                            rotateScrollDelta += Mathf.Sign(scroll);
                        }

                        ctype = (byte)((Mathf.RoundToInt(rotateScrollDelta) % 4 + 8) % 4);
                    }

                    if(Input.GetMouseButton(1) && !Input.GetMouseButton(0)) {
                        if(!rclickErase) {
                            rclickErase = true;
                            resetModel();
                        }
                        SetColor(removeColor);
                        tryErase();
                    }
                    else {
                        if(rclickErase) {
                            rclickErase = false;
                            setBlockModel();
                        }
                        bool obstructed = collider2d.OverlapCollider(triggerContactFilter, colresult) > 0 || transform.position.y <= LChunkLoader.main.GetFloorY(transform.position.x) + FLOOR_HEIGHT / 2 + SNAP * 0.5f || transform.position.x < -SNAP * 0.5f;
                        SetColor(obstructed ? invalidColor : placeColor);

                        if(Input.GetMouseButton(0)) {
                            if(!dragging) {
                                dragging = true;
                                dragSnapOffsetX = Mathf.RoundToInt(transform.position.x / SNAP) % 2 == 1;
                                dragSnapOffsetY = Mathf.RoundToInt(transform.position.y / SNAP) % 2 == 1;
                            }
                            if(!obstructed) PlaceBlock();
                        }
                        else {
                            if(dragging) dragging = false;
                        }
                    }
                }
                break;
            case STATE.REMOVE:
                //erase mode, available mainly to mobile users.
                if(Input.GetMouseButton(0)) {
                    tryErase();
                }
                    
                SetColor(removeColor);
                break;
            case STATE.SELECT:
                if(selectedViaRclick) {
                    //a right click in NONE state prompted the state change. Return to NONE state asap.
                    if(Input.GetMouseButtonDown(1)) {
                        trySelect(true);
                    }
                    else if(Input.GetMouseButtonDown(0) || selection == null) {
                        SetState(STATE.NONE);
                    }
                }
                else {
                    if(Input.GetMouseButtonDown(0)) {
                        trySelect(false);
                    }
                }
                break;
        }
    }

    private void tryErase() {
        int n = collider2d.OverlapCollider(triggerContactFilter, colresult);
        for(int i = 0; i < n; i++) {
            Destroy(colresult[i].gameObject);
        }
    }

    public void SetBlock(Block b, byte ctype) {
        block = b;
        if(b == null) {
            this.ctype = 0;
            resetModel();
        }
        else {
            this.ctype = ctype;
            setBlockModel();
        }
    }

    private void resetModel() {
        transform.localScale = cursorPrefab.transform.localScale;
        GetComponent<MeshFilter>().mesh = cursorPrefab.GetComponent<MeshFilter>().sharedMesh;
        mrender.materials = cursorPrefab.GetComponent<MeshRenderer>().sharedMaterials;
        tagRenderer.gameObject.SetActive(false);
    }

    private void setBlockModel() {
        if(block == null) return;
        if(block.hasObject) {
            tagRenderer.gameObject.SetActive(false);
            GameObject pref = block.prefab;
            transform.localScale = pref.transform.localScale;
            GetComponent<MeshFilter>().mesh = pref.GetComponent<MeshFilter>().sharedMesh;

            int matn = pref.GetComponent<MeshRenderer>().sharedMaterials.Length;
            Material[] newmat = new Material[matn];
            for(int i = 0; i < matn; i++) newmat[i] = cursorMaterial;
            mrender.materials = newmat;
        }
        else if(block.sprite != null) {
            //the block is a floor tag
            transform.localScale = cursorPrefab.transform.localScale;
            tagRenderer.gameObject.SetActive(true);
            tagRenderer.sprite = block.sprite;
            tagRenderer.material = defaultSprite;
        }
    }

    //called by ui buttons
    public void SetBlock(Block b) {
        if(state == STATE.PLACE) {
            SetBlock(b, ctype);
        }
        else {
            SetState(STATE.PLACE);
            SetBlock(b, ctype);
        }
    }

    public void SetColor(Color color) {
        cursorMaterial.color = color;
        if(tagRenderer.gameObject.activeInHierarchy) tagRenderer.color = color;
    }

    public void PlaceBlock() {
        if(block.hasObject) {
            Vector3 pos = transform.position;
            GameObject newo = Instantiate(lblock, new Vector3(pos.x, pos.y, block.zLayer), block.rotate ? Block.rotation[ctype % 4] : Quaternion.identity);//note: the lblocks should reposition itself if onFloor is true, no need to consider it here

            LBlockUpdater bu = newo.GetComponent<LBlockUpdater>();
            bu.SetBlock(block, ctype);
            bu.save = new BlockSave(block, pos.x, pos.y, ctype);
        }
        else if(block.sprite != null) {
            //a tag or trigger
            byte prefctype = ctype;
            if(block is Trigger trig) {
                prefctype = trig.defaultCType();
            }

            Vector3 pos = transform.position;
            GameObject newo = Instantiate(ltag, new Vector3(pos.x, pos.y, -5), block.rotate ? Block.rotation[ctype % 4] : Quaternion.identity);

            LBlockUpdater bu = newo.GetComponent<LBlockUpdater>();
            bu.SetBlock(block, prefctype);
            bu.save = new BlockSave(block, pos.x, pos.y, prefctype);
        }
    }

    private void trySelect(bool rclick) {
        GameObject selected = ClosestBlockTag();
        //Debug.Log(selected + " at " + selected.transform.position.ToString());
        if(selected != null) {
            selectedViaRclick = rclick;
            SelectBlock(selected);
            LBlockUpdater lb = selected.GetComponent<LBlockUpdater>();
            Block type = lb.type;
            //show popup window
            if(type is Trigger trig) {
                LChunkLoader.main.frag.rightClick.GetComponent<RightClickPopup>().SetBlock(trig, lb);
                //LChunkLoader.main.frag.rightClick.SetActive(true);
            }
        }
        else if(selection != null){
            Deselect();
            if(rclick) SetState(STATE.NONE);
        }
    }

    public GameObject ClosestBlock() {
        Vector2 mpos = camc.ScreenToWorldPoint(Input.mousePosition);
        float mdist = 99f;
        GameObject selected = null;

        //int n = collider2d.OverlapCollider(triggerContactFilter, colresult);
        int n = Physics2D.OverlapCircle(mpos, 0.5f, triggerContactFilter, colresult);
        for(int i = 0; i < n; i++) {
            if(!colresult[i].isTrigger) continue;
            Vector2 bpos = colresult[i].gameObject.transform.position;
            float dist = Vector2.Distance(mpos, bpos);
            if(dist < mdist) {
                selected = colresult[i].gameObject;
                mdist = dist;
            }
        }

        return selected;
    }

    public GameObject ClosestBlockTag() {
        Vector2 mpos = camc.ScreenToWorldPoint(Input.mousePosition);
        float mdist = 99f;
        GameObject selected = null;

        int n = Physics2D.OverlapCircle(mpos, 0.5f, triggerContactFilter, colresult);
        for(int i = 0; i < n; i++) {
            if(!colresult[i].isTrigger || !colresult[i].gameObject.CompareTag("LevelTag")) continue;
            Vector2 bpos = colresult[i].gameObject.transform.position;
            float dist = Vector2.Distance(mpos, bpos);
            if(dist < mdist) {
                selected = colresult[i].gameObject;
                mdist = dist;
            }
        }

        return selected;
    }

    public void SelectBlock(GameObject b) {
        LBlockUpdater lb = b.GetComponent<LBlockUpdater>();
        if(lb == null) return;
        SetState(STATE.SELECT);
        selection = b;
        SetBlock(lb.type, lb.ctype);
        if(lb.type.hasObject) transform.position = b.transform.position;
        else{
            transform.position = new Vector3(b.transform.position.x, b.transform.position.y, 0f);
            tagRenderer.material = additiveSprite;
        }
        transform.rotation = b.transform.rotation;
        //change mesh(or enable special effects)
    }

    public void Deselect() {
        if(selection == null) return;
        selection = null;
        //hide popup window
        LChunkLoader.main.frag.rightClick.SetActive(false);
    }

    private void snapRotate() {
        rotateScrollDelta = Mathf.Repeat(Mathf.Round(rotateScrollDelta), 4f);
    }

    public bool ScrollFocused() {
        return focused && (state == STATE.PLACE && block != null && block.rotate);
    }

    public float Snap(float a) {
        return Mathf.Round(a / SNAP) * SNAP;
    }

    public float DragSnap(float a, bool offset) {
        if(dragging) {
            if(offset) return Mathf.Round(a + SNAP) - SNAP;
            return Mathf.Round(a);
        }
        return Snap(a);
    }

    public void SetState(STATE newState) {
        if(state == newState) return;
        
        dragging = false;
        if(newState == STATE.PLACE) {
            SetBlock(lastBlock, lastCtype);
        }
        else if(state == STATE.PLACE){
            lastBlock = block == null ? Blocks.spike : block;
            lastCtype = ctype;
            SetBlock(null, 0);
        }
        if(state == STATE.SELECT) {
            Deselect();
            SetBlock(null, 0);
        }

        state = newState;

        highlighter.enabled = false;
        switch(state) {
            case STATE.NONE:
                SetColor(defaultColor);
                break;
            case STATE.PLACE:
                SetColor(placeColor);
                break;
            case STATE.REMOVE:
                SetColor(removeColor);
                highlighter.enabled = true;
                break;
            default:
                SetColor(selectColor);
                break;
        }
    }

    public void Show(bool show) {
        mrender.enabled = show;
    }

    float absin(float inn, float scl, float mag) {
        return (sin(inn, scl * 2f, mag) + mag) / 2f;
    }

    float sin(float radians, float scl, float mag) {
        return Mathf.Sin(radians / scl) * mag;
    }
}
