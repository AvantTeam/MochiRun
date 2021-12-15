using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this object has a Trigger Collider
public class ObstacleUpdater : MonoBehaviour
{
    public float damage = 15f;
    public bool destroyOnHit = false;
    public Vector3 angleOffset;
    public float rotateSpeed = 0f;

    public GameObject hitFx = null;

    GameObject cam;
    PlayerControl pcon;
    void Start() {
        cam = ChunkLoader.cam;
        pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        Vector3 rot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(angleOffset + rot);
    }

    void Update() {
        float xBound = cam.transform.position.x - ChunkLoader.CAM_CLIP / cam.GetComponent<CameraController>().zoom - 2f;
        if(transform.position.x < xBound) {
            Destroy(gameObject);
        }
        if(rotateSpeed > 0.1f || rotateSpeed < -0.1f) {
            Vector3 rot = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(rot.x, rot.y + rotateSpeed * Time.deltaTime, rot.z);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            pcon.Damage(damage, gameObject);
            if(hitFx != null) pcon.Fx(hitFx, transform.position, Quaternion.identity);
            if(destroyOnHit) Destroy(gameObject);
        }
    }
}
