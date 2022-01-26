using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFloor", menuName = "Blocks/Floor", order = 100)]
public class Floor : Block {
    protected bool yBoop = false; //prevent z-fighting

    protected override void OnValidate() {
        base.OnValidate();
        width = ChunkLoader.FLOOR_WIDTH;
        height = ChunkLoader.FLOOR_HEIGHT;
        hidden = true;
    }

    public override void init(float x, float y, byte ctype) {
        if(hasObject) {
            GameObject newo = Object.Instantiate(prefab, new Vector3(x, y + (yBoop ? 0.01f : 0f), zLayer), Quaternion.identity);
            if(yBoop){
                BoxCollider2D colli = newo.GetComponent<BoxCollider2D>();
                colli.offset = new Vector2(colli.offset.x, colli.offset.y - 0.01f / height);
            }
            yBoop = !yBoop;
            if(hasUpdate) {
                BlockUpdater bu = newo.GetComponent<BlockUpdater>();
                bu.type = this;
                bu.ctype = ctype;
            }
        }
    }
}