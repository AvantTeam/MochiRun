using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCameraZoom : TriggerUpdater {
    public override void Triggered() {
        cam.GetComponent<CameraController>().targetZoom = trigger.dataToModifier(ctype >> 3);
    }
}