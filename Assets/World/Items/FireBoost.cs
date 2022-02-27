using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBoost : Item {
    public float time, maxTime;
    public float speed = 0.5f;
    public byte fxFlag = 2;

    public FireBoost(float time) {
        this.time = maxTime = time;
    }

    public override void Reset() {
        time = maxTime;
    }

    public override bool UpdateAlways(PlayerControl pcon) {
        if(time <= 0f || pcon.state == PlayerControl.STATE.STUNNED){
            pcon.invincibility = 0.6f;
            return true;
        }
        pcon.Impulse(PlayerControl.SPEED_MAX * speed, 0f);
        time -= Time.deltaTime;
        pcon.updateFxFlag = fxFlag;
        pcon.invincibility = 0f;
        return false;
    }

    public override bool HandleDamage(PlayerControl pcon, float damage, GameObject source) {
        if(damage > 1f && source != null) {
            ObstacleUpdater obs = source.GetComponent<ObstacleUpdater>();
            if(obs != null && !obs.shieldable) {
                //todo devastate contacting obstacles
                RagblockUpdater.Yeet(source, PlayerControl.SPEED_MAX + speed);
                Object.Destroy(obs.gameObject);
            }
        }
        return false;
    }

    public override void BuildTable(GameObject table) {
        ReloadItemDisplay o = Object.Instantiate(Vars.main.prefabs.reloadDisplay).GetComponent<ReloadItemDisplay>();
        o.Set(o.fireBoost);
        o.frac = frac;
        o.transform.SetParent(table.transform, false);
    }

    public float frac() {
        return time / maxTime;
    }

    public override TYPE Type() {
        return TYPE.BOOST;
    }
}
