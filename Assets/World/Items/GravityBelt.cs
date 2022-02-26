using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerControl;

public class GravityBelt : Item {
    public const float GRAVITY = 1.3f;// gravity multiplier
    public const float MIN_FALL = 0.4f;

    public bool active;
    public float time = 0f;

    public override void Reset() {
        time = 0f;
        active = false;
    }

    public override bool Update(PlayerControl pcon) {
        if(pcon.landed) {
            if(active && time >= MIN_FALL) {
                //ground pound; todo
            }
            active = false;
            return false;
        }

        //in air
        if(KeyBinds.Shield() && !(pcon.state == STATE.FLOAT && pcon.gliding && pcon.courage > 0f)){
            pcon.SetVelocityY(Physics2D.gravity.y * GRAVITY);
            if(!active){
                time = 0f;
                active = true;
            }
            pcon.updateFxFlag = 1;
        }

        return true;
    }

    public override void BuildTable(GameObject table) {
        ItemDisplay o = Object.Instantiate(Vars.main.prefabs.itemDisplay).GetComponent<ItemDisplay>();
        o.image.texture = o.gravityBelt;
        o.transform.SetParent(table.transform, false);
    }

    public override TYPE Type() {
        return TYPE.AIR;
    }
}
