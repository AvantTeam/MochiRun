using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadItemDisplay : MonoBehaviour {
    public Sprite bubbleGun, fireBoost;

    public RawImage image;
    public Image reload;
    public Func<float> frac = () => 0;

    void Update() {
        reload.fillAmount = Mathf.Clamp01(1 - frac());
    }

    public void Set(Sprite sprite) {
        image.texture = sprite.texture;
        reload.sprite = sprite;
    }
}
