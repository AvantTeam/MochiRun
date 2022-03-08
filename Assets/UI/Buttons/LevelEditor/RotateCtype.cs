using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static LChunkLoader;

public class RotateCtype : MonoBehaviour {
    public bool mobileOnly;
    public GameObject button;

    void Start() {
        if (mobileOnly) {
            button.SetActive(Vars.mobile);
            if(!Vars.mobile) Destroy(gameObject.GetComponent<RotateCtype>());
        }
        if(!mobileOnly || Vars.mobile) button.GetComponent<Button>().onClick.AddListener(Clicked);
    }

    void Update() {
        if (mobileOnly && !Vars.mobile) return;
        button.SetActive(main.cursor.state == CursorControl.STATE.PLACE && main.cursor.block != null && main.cursor.block.rotate);
    }

    void Clicked() {
        if (main.cursor.state == CursorControl.STATE.PLACE) {
            main.cursor.Rotate(1);
        }
    }
}
