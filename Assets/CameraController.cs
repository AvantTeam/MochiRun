using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour{
    public const float CAMSCALE = 5f;
    public const float defFloorY = 2.5f;
    private const float DIM_TIME = 1f;
    //private const float DEATH_SPOT_INT = 2.5f;
    //private const float DEATH_SPOT_H = 2f;

    public float zoom = 1f;

    public Vector2 playerOffset = new Vector2(4f, 1.2f);
    public float targetYMode = 0f; //0 for inBounds cam, 1 for mochi-centered cam
    public float targetZoom = 1f;
    public float targetFloorY = 2.5f;
    public float curFloorY = 2.5f; //current Y position of mochi if its on the Floor Block
    public float boundsUpY = 3.5f, boundsDownY = 1.5f; //if mochi is under this amount away from the curFloorY it is "acceptable" and the camera snaps to the floor

    public struct LightSetting {
        public bool directional;
        public Quaternion angle; //directional only
        public Color color;
        public float intensity;
        public float radius; //!directional only
        public Vector3 offset; //!directional only, offset from camera. z is the offset from the player, so the zoom will not affect it.

        public static LightSetting sun = new LightSetting(Color.white, 1f, Quaternion.Euler(135f, 110f, 0f));
        public LightSetting(Color c) {
            directional = false;
            angle = Quaternion.Euler(135f, 110f, 0f);
            color = c;
            intensity = 2f;
            radius = 150f;
            offset = new Vector3(15f, 15f, -5f);
        }

        public LightSetting(Color c, float intensity, float radius, Vector3 offset) {
            directional = false;
            angle = Quaternion.identity;
            color = c;
            this.intensity = intensity;
            this.radius = radius;
            this.offset = offset;
        }

        public LightSetting(Color c, float intensity, Quaternion angle) {
            directional = true;
            this.angle = angle;
            color = c;
            this.intensity = intensity;
            radius = 30f;
            offset = new Vector3(0, 0, -5);
        }
    }

    GameObject player;
    public GameObject clight, dlight;
    PlayerControl pcon;
    LightSetting lightCurrent;
    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        pcon = player.GetComponent<PlayerControl>();
        curFloorY = targetFloorY = defFloorY;

        //todo levelloader will call this
        //deathSpotlight.SetActive(false);
        setLight(LightSetting.sun);
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
            float f = Mathf.Clamp01(pcon.stateTime / DIM_TIME);
            if(lightCurrent.directional) {
                //slowly dim *THE SUN*
                dlight.GetComponent<Light>().intensity = Mathf.Lerp(lightCurrent.intensity, 0.1f, f);
            }
            else {
                //slowly dim the camera light
                clight.GetComponent<Light>().intensity = Mathf.Lerp(lightCurrent.intensity, 0.1f, f);
            }
            /*
            Vector3 ppos = player.transform.position;
            if(!deathSpotlight.activeInHierarchy) deathSpotlight.SetActive(true);
            deathSpotlight.GetComponent<Light>().intensity = Mathf.Lerp(0f, DEATH_SPOT_INT, f);
            deathSpotlight.transform.position = new Vector3(ppos.x, Mathf.Max(ppos.y, curFloorY) + DEATH_SPOT_H, ppos.z);
            */
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

        if(targetZoom <= 0.1f) targetZoom = 0.1f;
        if(Mathf.Abs(zoom - targetZoom) < 0.001f) zoom = targetZoom;
        else zoom = approachDelta(zoom, targetZoom, 0.25f);

        transform.position = new Vector3(ppos.x + playerOffset.x / zoom, targetY, ppos.z + zoomZ());
        if(!lightCurrent.directional){
            clight.transform.position = new Vector3(transform.position.x + lightCurrent.offset.x / zoom, transform.position.y + lightCurrent.offset.y / zoom, ppos.z + lightCurrent.offset.z);
            if(zoom != targetZoom) clight.GetComponent<Light>().intensity = lightCurrent.intensity / zoom;
        }
    }

    void setLight(LightSetting lset) {
        lightCurrent = lset;
        dlight.SetActive(lset.directional);
        clight.SetActive(!lset.directional);
        Light l = lset.directional ? dlight.GetComponent<Light>() : clight.GetComponent<Light>();
        l.color = lset.color;
        l.intensity = lset.intensity;
        if(lset.directional) l.transform.rotation = lset.angle;
        else l.range = lset.radius;
    }

    float zoomZ() {
        return -CAMSCALE / zoom;
    }

    float lerpDelta(float fromValue, float toValue, float progress) {
        return fromValue + (toValue - fromValue) * Mathf.Clamp01(progress * Time.deltaTime * 60f);
    }
    float approach(float from, float to, float speed) {
        return from + Mathf.Clamp(to - from, -speed, speed);
    }

    float approachDelta(float from, float to, float speed) {
        return approach(from, to, speed * Time.deltaTime);
    }
}
