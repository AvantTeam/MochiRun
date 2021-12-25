using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PitEnder", menuName = "Blocks/Pit Ender", order = 102)]
public class PitEnder : Block {
    protected override void OnValidate() {
        base.OnValidate();
        clipsize = 4f;
        onFloor = true;
    }

    public override void init(float x, float y, byte ctype) {
        ChunkLoader.lastX = x - width / 2f + ChunkLoader.FLOOR_WIDTH / 2f;
        ChunkLoader.isPit = false;
    }
}