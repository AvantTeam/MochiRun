using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextDisable : MonoBehaviour
{
    public Color disabledColor = Color.black;
    public float mix = 0.25f;
    private Color onColor, offColor;
    Button button; Text text;

    void Start()
    {
        button = GetComponent<Button>();
        text = transform.GetChild(0).gameObject.GetComponent<Text>();
        onColor = text.color;
        offColor = Color.Lerp(onColor, disabledColor, mix);
    }

    void Update()
    {
        if(button.interactable) {
            text.color = onColor;
        }
        else {
            text.color = offColor;
        }
    }
}
