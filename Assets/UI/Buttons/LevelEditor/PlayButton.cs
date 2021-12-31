using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    public void Clicked() {
        Debug.Log("Play!");
        LChunkLoader.main.PlayLevel();
    }
}
