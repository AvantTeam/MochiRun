using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSlotButton : MonoBehaviour {
    public string path = "";
    public GameObject dialog;

    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    public void Set(string path, GameObject dialog) {
        this.path = path;
        this.dialog = dialog;
    }

    void Clicked() {
        if(path.Equals("") || dialog == null) return;
        LChunkLoader.main.LoadLevel(LevelIO.read(path));
        dialog.SetActive(false);
    }
}
