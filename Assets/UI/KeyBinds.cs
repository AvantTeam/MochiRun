using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyBinds {
    //these special keycodes are called every frame, and in bulk.
    public static KeyCode jump = KeyCode.Space, jump2 = KeyCode.None, shield = KeyCode.LeftShift, confirm = KeyCode.Return, cancel = KeyCode.Backspace, escape = KeyCode.Escape; 

    public static Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    public static void Load() {
        if(keys.Count <= 0) {
            Reset();
        }
    }

    public static void Reset() {
        keys.Add("Left", KeyCode.LeftArrow);
        keys.Add("Right", KeyCode.RightArrow);
        keys.Add("Up", KeyCode.UpArrow);
        keys.Add("Down", KeyCode.DownArrow);
        keys.Add("Favorite", KeyCode.LeftAlt);
        keys.Add("Speed Dialogue", KeyCode.X);
        keys.Add("Toggle Grid", KeyCode.G);
        jump = KeyCode.Space;
        jump2 = KeyCode.None;
        shield = KeyCode.LeftShift;
        confirm = KeyCode.Return;
        cancel = KeyCode.Backspace;
        escape = KeyCode.Escape;
        if(Vars.main.mobile) jump2 = KeyCode.Mouse0;
    }

    public static bool JumpDown() {
        return Input.GetKeyDown(jump) || Input.GetKeyDown(jump2);
    }
    public static bool Jump() {
        return Input.GetKey(jump) || Input.GetKey(jump2);
    }

    public static bool ShieldDown() {
        return Input.GetKeyDown(shield);
    }
    public static bool Shield() {
        return Input.GetKey(shield);
    }
    public static bool ConfirmDown() {
        return Input.GetKeyDown(confirm);
    }

    public static bool CancelDown() {
        return Input.GetKeyDown(cancel);
    }

    public static bool EscapeDown() {
        return Input.GetKeyDown(escape);
    }

    public static bool GetDown(string key) {
        return Input.GetKeyDown(keys[key]);
    }
    public static bool Get(string key) {
        return Input.GetKey(keys[key]);
    }
    public static bool GetUp(string key) {
        return Input.GetKeyUp(keys[key]);
    }
}
