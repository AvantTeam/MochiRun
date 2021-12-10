using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour{
    public const float camScale = 5f;
    public const float defFloorY = 2.5f;
    private const float DIM_TIME = 1.5f;

    public float zoom = 1f;

    public Vector2 playerOffset = new Vector2(4f, 1.2f);
    public float targetYMode = 0f; //0 for inBounds cam, 1 for mochi-centered cam
    public float targetZoom = 1f;
    public float targetFloorY = 2.5f;
    public float curFloorY = 2.5f; //current Y position of mochi if its on the Floor Block
    public float boundsUpY = 3.5f, boundsDownY = 1.5f; //if mochi is under this amount away from the curFloorY it is "acceptable" and the camera snaps to the floor

    GameObject player;
    GameObject clight;
    PlayerControl pcon;
    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        clight = GameObject.Find("Camera Light");
        pcon = player.GetComponent<PlayerControl>();
        curFloorY = targetFloorY = defFloorY;
        clight.GetComponent<Light>().intensity = 0.5f;
    }

    // Update is called once per frame
    void Update() {
        if(pcon.state == PlayerControl.STATE.CUTSCENE) {
            //todo
        }
        else if(pcon.state != PlayerControl.STATE.STOP) {
            SetCamera();
        }
        else {
            //slowly dim the lights
            float f = Mathf.Clamp01(pcon.stateTime / DIM_TIME);
            clight.GetComponent<Light>().intensity = Mathf.Lerp(clight.GetComponent<Light>().intensity, 0f, f);;
        }
    }

    void SetCamera() {
        Vector3 ppos = player.transform.position;

        curFloorY = lerpDelta(curFloorY, targetFloorY, 0.1f);
        bool inBounds = (ppos.y > curFloorY && ppos.y < curFloorY + boundsUpY) || (ppos.y < curFloorY && ppos.y > curFloorY - boundsDownY);
        targetYMode = lerpDelta(targetYMode, inBounds ? 0f : 1f, 0.08f);
        float targetY = Mathf.Lerp(curFloorY + playerOffset.y / zoom, ppos.y, targetYMode);
        //further clean the targetY to make it not show below the floor
        if(inBounds) targetY = Mathf.Max(targetY, curFloorY + playerOffset.y / zoom);

        if(Mathf.Abs(zoom - targetZoom) < 0.01f) zoom = targetZoom;
        else zoom = lerpDelta(zoom, targetZoom, 0.05f);

        transform.position = new Vector3(ppos.x + playerOffset.x / zoom, targetY, ppos.z + zoomZ());
        clight.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    float zoomZ() {
        return -camScale / zoom;
    }

    float lerpDelta(float fromValue, float toValue, float progress) {
        return fromValue + (toValue - fromValue) * Mathf.Clamp01(progress * Time.deltaTime * 60f);
    }
}
