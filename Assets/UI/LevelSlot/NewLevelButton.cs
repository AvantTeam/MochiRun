using UnityEngine;
using UnityEngine.UI;

public class NewLevelButton : MonoBehaviour {
    public GameObject dialog;

    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    public void Clicked() {
        UI.StringPopup("Enter level name:", s => {
            Level level = new Level();
            level.name = s;
            LChunkLoader.main.LoadLevel(level);

            dialog.SetActive(false);
        });
    }
}
