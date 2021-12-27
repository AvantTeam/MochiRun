using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    public void Clicked() {
        //todo
    }
}
