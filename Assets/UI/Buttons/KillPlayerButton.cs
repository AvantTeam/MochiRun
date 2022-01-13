using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillPlayerButton : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(LevelLoader.isCampaign());
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    private void Clicked() {
        ChunkLoader.pcon.Kill(false);
        transform.parent.parent.gameObject.SetActive(false);//hide dialog
    }
}
