using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainShieldUpdater : ObstacleUpdater {
    public override void OnHit() {
        pcon.AddItem(new ShieldItem());
    }
}
