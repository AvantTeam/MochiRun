using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //the only trigger is the player!
    private void OnTriggerEnter2D(Collider2D collision) {
        pcon.Damage(damage, gameObject);
    }
}
