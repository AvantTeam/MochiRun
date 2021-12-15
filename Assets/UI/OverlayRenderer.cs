using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayRenderer : MonoBehaviour
{
    private const float HPBAR_WIDTH = 600f, HPBAR_HEIGHT = 30f, OUTLINE = 7f;
    private const float HP_DELTA = 0.08f, COURAGE_DELTA = 0.1f; //per frame

    public float deltaHp, deltaCourage;
    private bool init = false;

    public RectTransform hpBar, hpBarSub, hpBarBack, hpIcon;
    public RectTransform courageBar, courageBarSub, courageBarBack, courageIcon;
    PlayerControl pcon;

    void Start()
    {
        pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        init = false;
    }

    void Update()
    {
        float hp = pcon.health;
        if(hp > PlayerControl.MAX_HP) hp = PlayerControl.MAX_HP;
        float courage = pcon.courage;
        if(!init) {
            setHPBarPos(hpIcon.anchoredPosition.x, hpIcon.anchoredPosition.y);
            deltaHp = hp;
            deltaCourage = courage;
            init = true;
        }

        if(Mathf.Abs(deltaHp - hp) < 0.01f) deltaHp = hp;
        else deltaHp = lerpDelta(deltaHp, hp, hp >= deltaHp ? HP_DELTA * 2f : HP_DELTA);

        if(deltaCourage < courage) deltaCourage = courage;
        else deltaCourage = lerpDelta(deltaCourage, courage, COURAGE_DELTA);

        if(deltaHp <= hp) {
            //healing
            setHPSub(hp);
            setHP(deltaHp);
        }
        else {
            //taken damage
            setHPSub(deltaHp);
            setHP(hp);
        }

        //todo courage bar
    }

    private void setHP(float hp) {
        hpBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, HPBAR_WIDTH * Mathf.Clamp01(hp / PlayerControl.MAX_HP));
    }

    private void setHPSub(float hp) {
        hpBarSub.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, HPBAR_WIDTH * Mathf.Clamp01(hp / PlayerControl.MAX_HP));
    }

    private void setHPBarPos(float x, float y) {
        hpBarBack.anchoredPosition = new Vector3(x - OUTLINE, y, 0f);
        resize(hpBarBack, HPBAR_WIDTH + OUTLINE * 2f, HPBAR_HEIGHT + OUTLINE * 2f);

        hpBarSub.anchoredPosition = new Vector3(x, y, 0f);
        resize(hpBarSub, HPBAR_WIDTH, HPBAR_HEIGHT);

        hpBar.anchoredPosition = new Vector3(x, y, 0f);
        resize(hpBar, HPBAR_WIDTH, HPBAR_HEIGHT);

        hpIcon.anchoredPosition = new Vector3(x, y, 0f);
    }

    private void resize(RectTransform r, float w, float h) {
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
    }

    float lerpDelta(float fromValue, float toValue, float progress) {
        return fromValue + (toValue - fromValue) * Mathf.Clamp01(progress * Time.deltaTime * 60f);
    }
}
