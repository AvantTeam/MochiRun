using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListField : MonoBehaviour {
    public bool init = false;
    public object[] list;
    public Action<object> setAs;
    public Func<object> get = () => default;

    public GameObject buttonPrefab, pane;
    public Button dropb;
    public TextMeshProUGUI value;
    void Start() {
        dropb.onClick.AddListener(DropClicked);
        pane.SetActive(false);
    }

    void Update() {
        if(!init) return;
        value.text = get() is ScriptableObject sc ? sc.name : get().ToString();
    }

    public void Set<T>(Func<T> get, Action<T> setAs, T[] tlist) {
        init = true;
        this.get = () => get();
        this.setAs = o => setAs((T)o);
        list = new object[tlist.Length];
        for(int i = 0; i < tlist.Length; i++) {
            list[i] = tlist[i];
        }
    }

    private void DropClicked() {
        if(!init || pane.activeInHierarchy || list.Length <= 0) {
            pane.SetActive(false);
        }
        else {
            UI.ClearChildren(pane);
            foreach(object t in list) button(t);
            pane.SetActive(true);
        }
    }

    private void button(object item) {
        GameObject b = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
        b.GetComponent<Button>().onClick.AddListener(() => {
            setAs(item);
            pane.SetActive(false);
        });

        b.GetComponent<TextMeshProUGUI>().text = (item is ScriptableObject sc ? sc.name : item.ToString());

        b.transform.SetParent(pane.transform, false);
    }
}
