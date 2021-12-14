using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackBlockUpdate : MonoBehaviour
{
    GameObject cam;
    // Start is called before the first frame update
    void Start() {
        cam = ChunkLoader.cam;
    }

    // Update is called once per frame
    void Update() {
        float xBound = cam.transform.position.x - ChunkLoader.DistancedCamClip(transform.position.z) * 0.9f;
        if(transform.position.x < xBound) {
            Destroy(gameObject);
        }
    }
}
