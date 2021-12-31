using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDeath : TriggerUpdater {
    public override void Triggered() {
        pcon.Kill(data > 0);
    }
}
