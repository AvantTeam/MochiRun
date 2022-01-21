using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YNPopupUpdater : MonoBehaviour {
    public Color color = Color.black;

    private bool useEscape = false, init = false;
    private Action yesClicked, noClicked;

    GameObject canvas;
    RectTransform rect;
    RawImage background;
    Button yes, no;
    TextMeshProUGUI textbox;

    void Awake() {
        //find canvas and become adopted
        canvas = FindObjectOfType<Canvas>().gameObject;
        transform.SetParent(canvas.transform, false);
        background = GetComponent<RawImage>();
        rect = GetComponent<RectTransform>();
        background.color = Color.clear;

        textbox = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        yes = transform.GetChild(1).GetComponent<Button>();
        no = transform.GetChild(2).GetComponent<Button>();
        yes.onClick.AddListener(OnYes);
        no.onClick.AddListener(OnNo);

        textbox.text = "";
        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);
    }

    void Update() {
        if(init && useEscape && KeyBinds.EscapeDown()) {
            OnNo();
        }
    }

    public void Set(string text, Action yesa, Action noa, int highlight, bool escape) {
        textbox.text = text;
        yesClicked = yesa;
        noClicked = noa;
        
        useEscape = escape;
        background.color = color;

        rect.localScale = new Vector3(1f, 0f, 1f);
        yes.gameObject.SetActive(true);
        no.gameObject.SetActive(true);

        //0: none 1:highlight Y 2:highlight N
        if(highlight == 1) {
            yes.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontStyle |= FontStyles.Underline;
        }
        else if(highlight == 2) {
            no.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontStyle |= FontStyles.Underline;
        }

        StartCoroutine(IShow());
    }

    void OnYes() {
        if(!init) return;
        StartCoroutine(IHide());
        yesClicked();
    }

    void OnNo() {
        if(!init) return;
        StartCoroutine(IHide());
        noClicked();
    }

    IEnumerator IShow() {
        init = false;
        rect.localScale = new Vector3(1f, 0f, 1f);

        for(int i = 0; i < 6; i++) {
            rect.localScale = new Vector3(1f, i / 6f, 1f);
            yield return null;
        }

        rect.localScale = new Vector3(1f, 1f, 1f);
        init = true;
    }

    IEnumerator IHide() {
        init = false;
        rect.localScale = new Vector3(1f, 1f, 1f);

        for(int i = 0; i < 6; i++) {
            rect.localScale = new Vector3(1f, 1f - (i / 6f), 1f);
            yield return null;
        }

        rect.localScale = new Vector3(1f, 0f, 1f);
        Destroy(gameObject);
    }
}
