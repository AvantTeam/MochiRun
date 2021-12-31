using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//attach to a child. Closes the parent on click.
public class ParentBackButton : MonoBehaviour
{
    public bool listenEscape = true;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    public void Clicked() {
        transform.parent.gameObject.SetActive(false);
    }

    void Update()
    {
        if(listenEscape && KeyBinds.EscapeDown()) Clicked();
    }
}
