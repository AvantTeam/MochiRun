using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//opens a dialog.
public class MenuButton : MonoBehaviour
{
    public GameObject dialog;

    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
        dialog.SetActive(false);
    }

    public void Clicked() {
        dialog.SetActive(true);
    }
}
