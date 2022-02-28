using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagblockUpdater : MonoBehaviour {
    public MeshRenderer mrenderer;
    public MeshFilter mfilter;

    private Vector3 vel;
    private Quaternion rvel1, rvel2;

    void FixedUpdate() {
        transform.position = transform.position + vel * Time.fixedDeltaTime;
        vel = vel * 0.995f;
        vel.y += -9f * Time.fixedDeltaTime;

        transform.rotation = (transform.rotation * rvel1) * rvel2;

        if(transform.position.y < 0) Destroy(gameObject);
    }

    public RagblockUpdater Impulse(float x, float y, float z){
        vel = new Vector3(x, y, z);
        return this;
    }

    public RagblockUpdater Spin(float r1, float r2) {
        rvel1 = Quaternion.Euler(r1, 0, 0);
        rvel2 = Quaternion.Euler(0, r2, 0);
        return this;
    }

    public static RagblockUpdater New(Vector3 pos, Quaternion rot) {
        return Instantiate(Vars.main.prefabs.ragblock, pos, rot).GetComponent<RagblockUpdater>();
    }

    public static RagblockUpdater At(GameObject pref) {
        RagblockUpdater o = New(pref.transform.position, pref.transform.rotation);
        o.transform.localScale = pref.transform.localScale;
        o.mfilter.mesh = pref.GetComponent<MeshFilter>().sharedMesh;
        o.mrenderer.materials = pref.GetComponent<MeshRenderer>().sharedMaterials;
        o.mrenderer.enabled = true;

        return o;
    }

    public static RagblockUpdater Yeet(GameObject source, float speed) {
        return At(source).Impulse(speed * Random.Range(0.8f, 2.2f) * PlayerControl.SPEED_MAX, Random.Range(3f, 8f) * speed, Random.Range(-1.5f, -1f)).Spin(Random.Range(-7f, 7f) * speed, Random.Range(-15f, 15f));
    }
}
