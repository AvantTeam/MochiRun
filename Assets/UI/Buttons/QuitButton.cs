using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour {
    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    void Clicked() {
        UI.ShowPopup("Are you sure you want to exit?", () => Application.Quit(), () => { });
    }
}
