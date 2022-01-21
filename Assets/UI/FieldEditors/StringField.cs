using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StringField : MonoBehaviour {
    public bool init = false;
    public Action<string> setAs;
    public Func<string> get = () => "";

    public TMP_InputField value;
    void Start() {
        value.onValueChanged.AddListener(s => Edited(s));
    }

    public void Set(Func<string> get, Action<string> setAs) {
        init = true;
        this.get = get;
        this.setAs = setAs;
        value.text = get();
    }

    private void Edited(string s) {
        setAs(s);
        value.text = get();
    }
}
