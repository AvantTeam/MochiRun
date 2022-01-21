using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneButton : MonoBehaviour {
    public string scene = "";

    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    void Clicked() {
        if(scene == "") return;
        SceneManager.LoadScene(scene);
    }
}
