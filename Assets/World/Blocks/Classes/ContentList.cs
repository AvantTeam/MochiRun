using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ContentList", menuName = "ContentList", order = 2)]
public class ContentList : ScriptableObject
{
    public Block[] blocks;
    public Category defaultCategory;
    public Theme[] themes;

    public Block block(string name) {
        foreach(Block b in blocks) {
            if(b.name == name || (b.hasObject && b.prefab.name == name)) return b;
        }
        return null;
    }

    public Theme theme(string name) {
        foreach(Theme b in themes) {
            if(b.name == name) return b;
        }
        return null;
    }
}
