using System;
using UnityEngine;
using UnityEngine.UI;

public class FloatField : MonoBehaviour
{
    public bool init = false;
    public Action left, right;
    public Action<float> setAs;
    public Func<float> get = () => 0f;
    public Func<bool> leftValid = () => true, rightValid = () => true;

    public Button lb, rb;
    public Text value;
    void Start()
    {
        lb.onClick.AddListener(LeftClicked);
        rb.onClick.AddListener(RightClicked);
    }

    void Update()
    {
        if(!init) return;
        value.text = (Mathf.Round(get()*10f)/10f).ToString();
        lb.interactable = leftValid();
        rb.interactable = rightValid();
    }

    public void Set(Func<float> get, Action left, Action right, Action<float> setAs) {
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
