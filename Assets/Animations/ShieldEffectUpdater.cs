using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffectUpdater : MonoBehaviour {
    private bool init = false;
    private float delta = 0;
    public Material material;

    public GameObject shieldFx;

    void Awake() {
        init = false;
        gameObject.SetActive(false);
    }

    void Update() {
        if(init) {
            delta += Time.deltaTime;
            float f = delta / 0.5f;

            if(f > 1) {
                init = false;
                gameObject.SetActive(false);
            }
            else {
                float r = PlayerAnimator.SHIELD_RAD + f;
                transform.localScale = new Vector3(r, r, r);
                material.SetFloat("_Alpha", 1 - f);
            }
        }
    }

    public void Play(PlayerControl pcon) {
        if(!init || delta > 0) {
            //todo sound
            pcon.Fx(shieldFx);
        }
        init = true;
        delta = 0;
        float r = PlayerAnimator.SHIELD_RAD;
        transform.localScale = new Vector3(r, r, r);
        material.SetFloat("_Alpha", 1);
        gameObject.SetActive(true);
    }
}
