using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPotionUpdater : ObstacleUpdater {
    public float duration = 3;
    public float boost = 0.6f;
    public byte fxFlag = 2;

    public override void OnHit() {
        FireBoost fb = new FireBoost(duration);
        fb.speed = boost;
        fb.fxFlag = fxFlag;
        pcon.AddItem(fb);
    }
}

