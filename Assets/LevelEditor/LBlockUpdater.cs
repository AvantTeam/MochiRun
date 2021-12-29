using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkLoader;

public class LBlockUpdater : MonoBehaviour
{
    public Block type;
    public byte ctype;
    public BlockSave save;

    /*
    void Update()
    {
        if(LChunkLoader.main.ShouldRemove(this)) Destroy(gameObject);
    }
    */

    public void SetBlock(Block b, byte ctype) {
        type = b;
        this.ctype = ctype;
        if(b.hasObject) {
            GameObject pref = b.prefab;
            transform.localScale = pref.transform.localScale;
            GetComponent<MeshFilter>().mesh = pref.GetComponent<MeshFilter>().sharedMesh;
            GetComponent<MeshRenderer>().materials = pref.GetComponent<MeshRenderer>().sharedMaterials;
        }
    }
}
