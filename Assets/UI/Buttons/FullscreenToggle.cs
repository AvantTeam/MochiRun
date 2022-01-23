using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour {
    void Start() {
        if(Vars.mobile) gameObject.SetActive(false);
        else GetComponent<Button>().onClick.AddListener(Clicked);
    }

    void Clicked() {
        //Screen.fullScreen = !Screen.fullScreen;
        if(Screen.fullScreenMode == FullScreenMode.FullScreenWindow) Screen.fullScreenMode = FullScreenMode.Windowed;
        else Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }
}
