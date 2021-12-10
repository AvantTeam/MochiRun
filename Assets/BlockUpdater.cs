using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUpdater : MonoBehaviour
{
    public Block type;
    public byte ctype = 0;

    GameObject cam;
    // Start is called before the first frame update
    void Start() {
        cam = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update() {
        float xBound = cam.transform.position.x - ChunkLoader.CAM_CLIP / cam.GetComponent<CameraController>().zoom - type.clipsize;
        if(transform.position.x < xBound){
            Destroy(gameObject);
        }
    }
}
