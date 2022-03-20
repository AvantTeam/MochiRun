using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputHandler : InputHandler {
    public const float shieldBound = 0.4f;

    public MobileInputHandler(Camera cam) : base(cam) {
    }

    public override bool JumpDown() {
        return (Input.GetMouseButtonDown(0) && MouseBoundX(false, shieldBound) && !EventSystem.current.IsPointerOverGameObject()) || base.JumpDown();
    }

    public override bool Jump() {
        return (Input.GetMouseButton(0) && MouseBoundX(false, shieldBound) && !EventSystem.current.IsPointerOverGameObject()) || base.Jump();
    }

    public override bool ShieldDown() {
        return (Input.GetMouseButtonDown(0) && MouseBoundX(true, 0.3f) && !EventSystem.current.IsPointerOverGameObject()) || base.ShieldDown();
    }

    public override bool Shield() {
        return (Input.GetMouseButton(0) && MouseBoundX(true, 0.3f) && !EventSystem.current.IsPointerOverGameObject()) || base.Shield();
    }

    public bool MouseBoundX(bool left, float p) {
        return left == Input.mousePosition.x < p * cam.pixelWidth;
    }
}
