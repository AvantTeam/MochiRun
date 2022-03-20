using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler {
    public Camera cam;

    public InputHandler(Camera cam) {
        this.cam = cam;
    }

    public virtual bool JumpDown() {
        return KeyBinds.JumpDown();
    }

    public virtual bool Jump() {
        return KeyBinds.Jump();
    }

    public virtual bool ShieldDown() {
        return KeyBinds.ShieldDown();
    }

    public virtual bool Shield() {
        return KeyBinds.Shield();
    }
}
