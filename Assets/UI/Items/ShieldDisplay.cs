using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldDisplay : MonoBehaviour {
    public Texture2D[] sprites;
    public Texture2D broken;

    public RawImage image;
    public ShieldItem shield;

    void Update() {
        if(shield == null) return;
        if(shield.health <= 0) image.texture = broken;
        else {
            float f = shield.health / shield.maxHealth;

            image.texture = sprites[Mathf.Clamp((int)((1 - f) * sprites.Length), 0, sprites.Length - 1)];
        }
    }
}
