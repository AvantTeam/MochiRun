using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this object has a Kinematic RigidBody & Trigger Collider
public class ObstacleUpdater : MonoBehaviour
{
    public float damage = 15f;
    GameObject cam;
    PlayerControl pcon;
    void Start() {
        cam = ChunkLoader.cam;
        pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
    }

    void Update() {
        float xBound = cam.transform.position.x - ChunkLoader.CAM_CLIP / cam.GetComponent<CameraController>().zoom - 2f;
        if(transform.position.x < xBound) {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            pcon.Damage(damage, gameObject);
        }
    }
}
