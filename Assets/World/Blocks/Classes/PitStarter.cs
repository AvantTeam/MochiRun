using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PitStarter", menuName = "Blocks/Pit Starter", order = 101)]
public class PitStarter : Block {
    protected override void OnValidate() {
        base.OnValidate();
        clipsize = 4f;
        onFloor = true;
    }

    public override void init(float x, float y, byte ctype) {
        Vector2 s = new Vector2(x, y);
        Vector2 e = s + Vector2.down * 1f;
        Collider2D collider = Physics2D.Linecast(s, e).collider;
        if(collider != null && collider.gameObject.CompareTag("Floor")) Object.Destroy(collider.gameObject); //destroy the floor under this
        ChunkLoader.isPit = true;
    }
}