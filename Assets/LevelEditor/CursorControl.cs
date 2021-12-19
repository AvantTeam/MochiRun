using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkLoader;

public class CursorControl : MonoBehaviour
{
    public const float SNAP = 0.5f, SCROLL_SCALE = 0.5f;

    public bool focused = true;

    public Block block;
    public byte ctype;
    public GameObject cursorPrefab, lblock;
    public Material cursorMaterial;
    public Color placeColor, invalidColor, removeColor;

    public enum STATE {
        NONE = -1,
        PLACE = 0,
        REMOVE,
        SELECT,
        NUM
    };

    public STATE state = STATE.PLACE;

    private float rotateScrollDelta;
    private Collider2D[] colresult = new Collider2D[4];
    private ContactFilter2D triggerContactFilter;
    GameObject cam;
    Camera camc;
    MeshRenderer mrender;
    Collider2D collider2d;

    void Start()
    {
        cam = GameObject.Find("Main Camera");
        camc = cam.GetComponent<Camera>();
        mrender = GetComponent<MeshRenderer>();
        collider2d = GetComponent<Collider2D>();
        triggerContactFilter = new ContactFilter2D();
        triggerContactFilter.useTriggers = true;

        focused = true;
        SetBlock(Blocks.spike, 0);
        SetColor(placeColor);
    }

    void Update()
    {
        if(focused){
            moveToMouse();
            playerInputs();
        }
    }

    public void SetBlock(Block b, byte ctype) {
        block = b;
        if(b == null) {
            this.ctype = 0;
            transform.localScale = cursorPrefab.transform.localScale;
            GetComponent<MeshFilter>().mesh = cursorPrefab.GetComponent<MeshFilter>().sharedMesh;
            mrender.materials = cursorPrefab.GetComponent<MeshRenderer>().sharedMaterials;
        }
        else {
            this.ctype = ctype;
            if(b.hasObject) {
                GameObject pref = b.prefab;
                transform.localScale = pref.transform.localScale;
                GetComponent<MeshFilter>().mesh = pref.GetComponent<MeshFilter>().sharedMesh;

                int matn = pref.GetComponent<MeshRenderer>().sharedMaterials.Length;
                Material[] newmat = new Material[matn];
                for(int i = 0; i < matn; i++) newmat[i] = cursorMaterial;
                mrender.materials = newmat;
            }
        }
    }

    private void moveToMouse() {
        Vector3 mpos = camc.ScreenToWorldPoint(Input.mousePosition);
        mpos.z = (block == null ? 0f : block.zLayer);
        mpos.x = Snap(mpos.x);
        mpos.y = Snap(mpos.y); //todo onFloor, and make spike's onFloor false

        transform.position = mpos;
        if(block != null) {
            transform.rotation = block.rotate ? Block.rotation[ctype % 4] : Quaternion.identity;
        }
    }

    public void SetColor(Color color) {
        cursorMaterial.color = color;
    }

    public void PlaceBlock() {
        if(block.hasObject) {
            Vector3 pos = transform.position;
            GameObject newo = Instantiate(lblock, new Vector3(pos.x, pos.y, block.zLayer), block.rotate ? Block.rotation[ctype % 4] : Quaternion.identity);//note: the lblocks should reposition itself if onFloor is true, no need to consider it here

            LBlockUpdater bu = newo.GetComponent<LBlockUpdater>();
            bu.SetBlock(block, ctype);
            bu.save = new BlockSave(block, pos.x, pos.y, ctype);
        }
    }

    private void playerInputs() {
        if(block == null) {

        }
        else {
            switch(state) {
                case STATE.PLACE:
                    if(block.rotate) {
                        float scroll = Input.mouseScrollDelta.y * SCROLL_SCALE;
                        rotateScrollDelta += scroll;
                        ctype = (byte)((Mathf.RoundToInt(rotateScrollDelta) % 4 + 8) % 4);
                    }

                    bool obstructed = collider2d.OverlapCollider(triggerContactFilter, colresult) > 0 || transform.position.y <= LChunkLoader.main.GetFloorY(transform.position.x) + FLOOR_HEIGHT / 2 + SNAP * 0.5f;
                    SetColor(obstructed ? invalidColor : placeColor);

                    if(!obstructed && Input.GetMouseButton(0)) PlaceBlock();
                    break;
            }
            


        }
    }

    private void snapRotate() {
        rotateScrollDelta = Mathf.Repeat(Mathf.Round(rotateScrollDelta), 4f);
    }

    public float Snap(float a) {
        return Mathf.Round(a / SNAP) * SNAP;
    }
}
