using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorCameraControl : MonoBehaviour
{
    private const float MIN_ZOOM = 2f, MAX_ZOOM = 15f, Z = -10f;
    private const float MIN_X = 0f, MIN_Y = -1.5f;
    
    private float targetZoom;
    public float zoom;
    public bool panning;
    public bool view3D = false;
    private bool lastClickStartFocused = false, middlePan = false;

    private Vector3 dragCursorOrigin, dragCamOrigin;

    public CursorControl cursor;
    Camera cam;
    void Start()
    {
        zoom = targetZoom = 3;
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if(!cursor.ScrollFocused() && !EventSystem.current.IsPointerOverGameObject()) {
            float scroll = Input.mouseScrollDelta.y;
            if(Mathf.Abs(scroll) > 0.1f) {
                targetZoom = Mathf.Clamp(targetZoom - Mathf.Sign(scroll), MIN_ZOOM, MAX_ZOOM);
            }
        }

        //keyboard shortcuts
        if(Input.anyKey && !KeyBinds.hasDialog()) {
            if(KeyBinds.GetDown("Toggle Grid")) LChunkLoader.main.frag.grid.ToggleMode();
            if(Input.GetKeyDown(KeyCode.Tab)) PencilEdit.Clicked();
            if(KeyBinds.EscapeDown()) LChunkLoader.main.frag.menuButton.GetComponent<MenuButton>().Clicked();
        }

        if(cursor.state == CursorControl.STATE.NONE) {
            if(Input.GetMouseButtonDown(0)) lastClickStartFocused = !EventSystem.current.IsPointerOverGameObject();

            updateDragPan((Input.GetMouseButtonDown(0) && lastClickStartFocused) || Input.GetMouseButtonDown(2), (Input.GetMouseButton(0) && lastClickStartFocused) || Input.GetMouseButton(2));
        }
        else if(!Vars.mobile) {
            lastClickStartFocused = false;
            updateDragPan(Input.GetMouseButtonDown(2), Input.GetMouseButton(2));
        }
        else {
            lastClickStartFocused = false;
            updateDragPan(false, false);
        }

        if(Mathf.Abs(zoom - targetZoom) < 0.01f) zoom = targetZoom;
        else zoom = lerpDelta(zoom, targetZoom, 0.05f);
        cam.orthographicSize = zoom;

        float currentMin = transform.position.y - cam.orthographicSize;//min y
        if(currentMin < MIN_Y) transform.position = new Vector3(transform.position.x, cam.orthographicSize + MIN_Y, Z);
        currentMin = transform.position.x - cam.orthographicSize * Screen.width / Screen.height;//min x
        if(currentMin < MIN_X) transform.position = new Vector3(cam.orthographicSize * Screen.width / Screen.height + MIN_X, transform.position.y, Z);
    }

    private void updateDragPan(bool down, bool pressing) {
        bool prevpan = panning;
        panning = pressing;
        if(down) {
            dragCursorOrigin = Input.mousePosition;
            dragCamOrigin = transform.position;
            middlePan = Input.GetMouseButton(2);
        }
        else if(pressing) {
            Vector2 dest = cam.ScreenToViewportPoint(-Input.mousePosition + dragCursorOrigin);
            float speed = cam.orthographicSize * 2f;
            transform.position = new Vector3(dragCamOrigin.x + dest.x * speed * Screen.width / Screen.height, dragCamOrigin.y + dest.y * speed, Z);
        }
        else if(prevpan && middlePan) {
            //stopped panning
            if(Mathf.Abs(transform.position.x - dragCamOrigin.x) + Mathf.Abs(transform.position.y - dragCamOrigin.y) <= 0.2f) {
                //pick block
                GameObject o = cursor.ClosestBlock();
                if(o != null) {
                    Block block = o.GetComponent<LBlockUpdater>().type;
                    if(!block.hidden) cursor.SetBlock(block);
                }
            }
        }
    }

    float lerpDelta(float fromValue, float toValue, float progress) {
        return fromValue + (toValue - fromValue) * Mathf.Clamp01(progress * Time.deltaTime * 60f);
    }
}
