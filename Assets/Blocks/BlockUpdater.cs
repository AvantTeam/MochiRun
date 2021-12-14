using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUpdater : MonoBehaviour
{
    public Block type;
    public byte ctype = 0;

    GameObject cam;
    void Start() {
        cam = ChunkLoader.cam;
    }

    void Update() {
        float xBound = cam.transform.position.x - ChunkLoader.CAM_CLIP / cam.GetComponent<CameraController>().zoom - type.clipsize;
        if(transform.position.x < xBound){
            Destroy(gameObject);
        }
    }
}
