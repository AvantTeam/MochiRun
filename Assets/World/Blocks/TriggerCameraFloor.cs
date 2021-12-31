using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCameraFloor : TriggerUpdater {
    public override void Triggered() {
        cam.GetComponent<CameraController>().targetFloorY = transform.position.y;
    }
}
