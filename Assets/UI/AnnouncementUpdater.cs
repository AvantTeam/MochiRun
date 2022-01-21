using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnouncementUpdater : MonoBehaviour {
    private float duration = -1f;
    GameObject canvas;
    TextMeshProUGUI textbox;

    void Awake() {
        //find canvas and become adopted
        canvas = FindObjectOfType<Canvas>().gameObject;
        transform.SetParent(canvas.transform, false);
        textbox = GetComponent<TextMeshProUGUI>();
        textbox.text = "";
    }

    void Update() {
        if(duration <= -1f) return;

        duration -= Time.unscaledDeltaTime;
        if(duration <= 0f) Destroy(gameObject);
        else {
            Color c = Color.white;
            c.a = Mathf.Clamp01(duration * 2f);
            textbox.color = c;
        }
    }

    public void Set(string text, float duration) {
        this.duration = duration;
        textbox.text = text;
    }
}
