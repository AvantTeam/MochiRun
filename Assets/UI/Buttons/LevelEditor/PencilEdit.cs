using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PencilEdit : MonoBehaviour {
    public bool desktopOnly = true;
    void Start() {
        if(desktopOnly) gameObject.SetActive(!Vars.main.mobile);
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    public static void Clicked() {
        if(LChunkLoader.main.cursor.state == CursorControl.STATE.PLACE) LChunkLoader.main.cursor.SetState(CursorControl.STATE.NONE);
        else LChunkLoader.main.cursor.SetState(CursorControl.STATE.PLACE);
    }
}
