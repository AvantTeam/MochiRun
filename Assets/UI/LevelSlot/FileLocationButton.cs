using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileLocationButton : MonoBehaviour {
    void Awake() {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        gameObject.SetActive(true);
        GetComponent<Button>().onClick.AddListener(Clicked);
#else
        gameObject.SetActive(false);
#endif
    }

    void Clicked() {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        string itemPath = LevelSlotDialog.SaveDirectory();
        itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
        //System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
        System.Diagnostics.Process.Start("explorer.exe", itemPath);
#endif
    }
}
