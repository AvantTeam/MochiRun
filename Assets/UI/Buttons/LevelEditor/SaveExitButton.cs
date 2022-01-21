using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveExitButton : MonoBehaviour {
    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    void Clicked() {
        bool res = SaveLevelButton.SaveLevel();
        res = false; //todo remove
        if(res) SceneManager.LoadScene("TitleScene");
        else {
            UI.ShowPopup("<color=#ff7777>Failed to save the level due to an error.</color>\nPlease send a bug report if this keeps happening.\nExit anyways?", 
                () => SceneManager.LoadScene("TitleScene"), () => { }, 2, true);
        }
    }
}
