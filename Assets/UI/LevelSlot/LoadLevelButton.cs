using UnityEngine;
using UnityEngine.UI;

public class LoadLevelButton : MonoBehaviour {
    public LevelSlotDialog dialog;

    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
        dialog.gameObject.SetActive(false);
    }

    public void Clicked() {
        dialog.gameObject.SetActive(true);
        dialog.Build();
    }
}
