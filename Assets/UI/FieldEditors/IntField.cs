using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntField : MonoBehaviour {
    public bool init = false;
    public Action left, right;
    public Action<int> setAs;
    public Func<int> get = () => 0;
    public Func<bool> leftValid = () => true, rightValid = () => true;

    public Button lb, rb;
    public Text value;
    void Start() {
        lb.onClick.AddListener(LeftClicked);
        rb.onClick.AddListener(RightClicked);
    }

    void Update() {
        if(!init) return;
        value.text = get().ToString();
        lb.interactable = leftValid();
        rb.interactable = rightValid();
    }

    public void Set(Func<int> get, Action left, Action right, Action<int> setAs) {
        //Debug.Log("SET FLOAT FIELD");
        init = true;
        this.get = get;
        this.left = left;
        this.right = right;
        this.setAs = setAs;
    }

    private void LeftClicked() {
        left();
    }

    private void RightClicked() {
        right();
    }
}