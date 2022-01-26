using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LFloorUpdater : MonoBehaviour
{
    void Start()
    {
        SetBlock(Blocks.floorBlock);
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Blocks.floorBlock.zLayer);
    }

    void Update()
    {
        if(LChunkLoader.main.ShouldRemoveFloor(gameObject)) Destroy(gameObject);
    }

    public void SetBlock(Block b) {
        if(b.hasObject) {
            GameObject pref = b.prefab;
            transform.localScale = pref.transform.localScale;
            GetComponent<MeshFilter>().mesh = pref.GetComponent<MeshFilter>().sharedMesh;
            GetComponent<MeshRenderer>().materials = pref.GetComponent<MeshRenderer>().sharedMaterials;
        }
    }
}
