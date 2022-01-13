using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndButton : MonoBehaviour
{
    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    private void Clicked() {
        ChunkLoader.pcon.Quit();
    }
}
