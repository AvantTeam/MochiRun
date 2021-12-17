using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUpdater : MonoBehaviour
{
    public Block type;
    public byte ctype = 0;
    public bool updatesTile = false;

    protected GameObject cam;
    protected PlayerControl pcon;
    protected virtual void Start() {
        cam = ChunkLoader.cam;
        pcon = ChunkLoader.pcon;
    }

    void Update() {
        float xBound = cam.transform.position.x - ChunkLoader.CAM_CLIP / cam.GetComponent<CameraController>().zoom - type.clipsize;
        if(transform.position.x < xBound){
            Destroy(gameObject);
        }
        else if(updatesTile){
            UpdateTile();
        }
    }


    public virtual void UpdateTile() {

    }

    public virtual void Couraged(PlayerControl pcon) {

    }
}
