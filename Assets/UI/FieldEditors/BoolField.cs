using System;
using UnityEngine;
using UnityEngine.UI;

public class BoolField : MonoBehaviour {
    public bool init = false;
    public Action toggle;
    public Action<bool> setAs;
    public Func<bool> get = () => false;

    public Button tb;
    public GameObject check;
    void Start() {
        tb.onClick.AddListener(Toggled);
    }

    void Update() {
        if(!init) return;
        if(check.activeInHierarchy != get()) check.SetActive(get());
    }

    public void Set(Func<bool> get, Action toggle, Action<bool> setAs) {
        init = true;
        this.get = get;
        this.toggle = toggle;
        this.setAs = setAs;
    }

    private void Toggled() {
        toggle();
    }
}
