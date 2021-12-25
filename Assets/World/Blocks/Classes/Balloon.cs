using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBalloon", menuName = "Blocks/Balloon", order = 2)]
public class Balloon : Block {
    public override void init(float x, float y, byte ctype) {
        if(hasObject) {
            GameObject newo = Object.Instantiate(prefab, new Vector3(x, y, zLayer), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            if(hasUpdate) {
                BlockUpdater bu = newo.GetComponent<BlockUpdater>();
                bu.type = this;
                bu.ctype = ctype;
            }
        }
    }
}