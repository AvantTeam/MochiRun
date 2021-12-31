using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PencilEraser : MonoBehaviour
{
    public bool mobileOnly;
    void Start()
    {
        if(mobileOnly) gameObject.SetActive(Vars.main.mobile);
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    void Clicked() {
        if(LChunkLoader.main.cursor.state == CursorControl.STATE.PLACE) LChunkLoader.main.cursor.SetState(CursorControl.STATE.REMOVE);
        else LChunkLoader.main.cursor.SetState(CursorControl.STATE.PLACE);
    }
}
