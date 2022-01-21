using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SPopupUpdater : MonoBehaviour {
    public Color color = Color.black;

    private bool init = false, filterFile = false;
    private Action<string> yesClicked;
    private Action noClicked;

    GameObject canvas;
    RectTransform rect;
    RawImage background;
    Button yes, no;
    TextMeshProUGUI title;
    TMP_InputField textbox;

    void Awake() {
        //find canvas and become adopted
        canvas = FindObjectOfType<Canvas>().gameObject;
        transform.SetParent(canvas.transform, false);
        background = GetComponent<RawImage>();
        rect = GetComponent<RectTransform>();
        background.color = Color.clear;

        title = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        yes = transform.GetChild(1).GetComponent<Button>();
        no = transform.GetChild(2).GetComponent<Button>();
        textbox = transform.GetChild(3).GetComponent<TMP_InputField>();
        yes.onClick.AddListener(OnYes);
        no.onClick.AddListener(OnNo);

        title.text = "";
        textbox.text = "";
        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);
    }

    public void Set(string titleText, string defText, Action<string> yesa, Action noa, int maxChars, bool filterFile) {
        title.text = titleText;
        yesClicked = yesa;
        noClicked = noa;
        this.filterFile = filterFile;
        textbox.text = defText;
        textbox.characterLimit = maxChars;
        
        background.color = color;

        rect.localScale = new Vector3(1f, 0f, 1f);
        yes.gameObject.SetActive(true);
        no.gameObject.SetActive(true);

        StartCoroutine(IShow());
    }

    private string Blackchar(string s) {
        foreach(char c in UI.blackchars) {
            s = s.Replace(c, '*');
        }
        return s.Replace("*", null);
    }

    void OnYes() {
        if(!init) return;
        StartCoroutine(IHide());
        yesClicked(filterFile ? Blackchar(textbox.text.Trim()) : textbox.text.Trim());
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
