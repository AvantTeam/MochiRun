using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ContentList", menuName = "ContentList", order = 2)]
public class ContentList : ScriptableObject
{
    public Block[] blocks;

    public Block block(string name) {
        foreach(Block b in blocks) {
            if(b.name == name || (b.hasObject && b.prefab.name == name)) return b;
        }
        return null;
    }
}
