using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCameraControl : MonoBehaviour
{
    private const float MIN_ZOOM = 2f, MAX_ZOOM = 10f, Z = -10f;
    
    private float targetZoom;
    public float zoom;
    public bool panning;

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
        if(!cursor.ScrollFocused()) {//todo: add various "focuses" of elements such as the canvas here!
            float scroll = Input.mouseScrollDelta.y;
            if(Mathf.Abs(scroll) > 0.1f) {
                targetZoom = Mathf.Clamp(targetZoom - Mathf.Sign(scroll), MIN_ZOOM, MAX_ZOOM);
            }
        }

        if(cursor.state == CursorControl.STATE.NONE) {
            updateDragPan(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2), Input.GetMouseButton(0) || Input.GetMouseButton(2));
        }
        else if(!Vars.main.mobile) {
            updateDragPan(Input.GetMouseButtonDown(2), Input.GetMouseButton(2));
        }
        else {
            updateDragPan(false, false);
        }

        if(Mathf.Abs(zoom - targetZoom) < 0.01f) zoom = targetZoom;
        else zoom = lerpDelta(zoom, targetZoom, 0.05f);
        cam.orthographicSize = zoom;

        float currentMinY = transform.position.y - cam.orthographicSize;
        if(currentMinY < 0f) transform.position = new Vector3(transform.position.x, cam.orthographicSize, Z);
    }

    private void updateDragPan(bool down, bool pressing) {
        panning = pressing;
        if(down) {
            dragCursorOrigin = Input.mousePosition;
            dragCamOrigin = transform.position;
        }
        else if(pressing) {
            Vector2 dest = cam.ScreenToViewportPoint(-Input.mousePosition + dragCursorOrigin);
            float speed = cam.orthographicSize * 2f;
            transform.position = new Vector3(dragCamOrigin.x + dest.x * speed * Screen.width / Screen.height, dragCamOrigin.y + dest.y * speed, Z);
        }
    }

    float lerpDelta(float fromValue, float toValue, float progress) {
        return fromValue + (toValue - fromValue) * Mathf.Clamp01(progress * Time.deltaTime * 60f);
    }
}
