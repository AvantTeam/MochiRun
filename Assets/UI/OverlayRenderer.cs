using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OverlayRenderer : MonoBehaviour
{
    private const float HPBAR_WIDTH = 600f, HPBAR_HEIGHT = 30f, OUTLINE = 7f;
    private const float CBAR_WIDTH = 106f, CBAR_HEIGHT = 46f;
    private const float HP_DELTA = 0.08f, COURAGE_DELTA = 0.16f, FLASH = 0.3f; //dleta: per frame, flash: per second

    public float deltaHp, deltaCourage, cFlash;
    public int deltaCSlots;
    public Vector2 cBarOffset; //offset from the right tip of the hp bar.
    private bool init = false;
    private int lastMaxCourage = 0;
    private bool courageShown = false, cFlashSlot = false;
    public Color courageColor, courageSlotColor, courageLightColor;

    public RectTransform hpBar, hpBarSub, hpBarBack, hpIcon;
    public RectTransform courageBar, courageBarSub, courageBarBack, courageBarSlots, courageIcon;
    public Image courageBarImage, courageBarSImage;
    PlayerControl pcon;

    void Start()
    {
        pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        init = false;
        setCActive(false);
        AssetPreview.SetPreviewTextureCacheSize(32);
    }

    void Update()
    {
        float hp = pcon.health;
        if(hp > PlayerControl.MAX_HP) hp = PlayerControl.MAX_HP;
        float courage = pcon.courage;
        int cslots = pcon.CourageSlots();
        if(!init) {
            setHPBarPos(hpIcon.anchoredPosition.x, hpIcon.anchoredPosition.y);
            setCBarMax(PlayerControl.COURAGES);
            deltaHp = hp;
            deltaCourage = courage;
            deltaCSlots = cslots;
            lastMaxCourage = PlayerControl.COURAGES;
            cFlash = 0f;
            init = true;
        }

        if(lastMaxCourage != PlayerControl.COURAGES) {
            setCBarMax(PlayerControl.COURAGES);
            lastMaxCourage = PlayerControl.COURAGES;
        }

        if(Mathf.Abs(deltaHp - hp) < 0.01f) deltaHp = hp;
        else deltaHp = lerpDelta(deltaHp, hp, hp >= deltaHp ? HP_DELTA * 2f : HP_DELTA);
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

        if(lastMaxCourage > 0) {
            if(deltaCSlots != cslots) {
                cFlashSlot = deltaCSlots < cslots;
                deltaCSlots = cslots;
                cFlash = 1f;
                courageBarImage.color = courageColor;
                courageBarSImage.color = courageSlotColor;
            }
            if(deltaCourage < courage){
                deltaCourage = courage;
            }
            else deltaCourage = lerpDelta(deltaCourage, courage, COURAGE_DELTA);
            if(cFlash > 0){
                cFlash -= Time.deltaTime / FLASH;
                if(cFlashSlot) courageBarSImage.color = Color.Lerp(courageSlotColor, Color.white, Mathf.Clamp01(cFlash));
                else courageBarImage.color = Color.Lerp(courageColor, courageLightColor, Mathf.Clamp01(cFlash));
            }
            setCSub(deltaCourage);
            setC(courage);
            setCSlots(PlayerControl.COURAGE_SLOT * cslots);
        }
    }

    private void setHP(float hp) {
        hpBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, HPBAR_WIDTH * Mathf.Clamp01(hp / PlayerControl.MAX_HP));
    }

    private void setHPSub(float hp) {
        hpBarSub.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, HPBAR_WIDTH * Mathf.Clamp01(hp / PlayerControl.MAX_HP));
    }

    private void setC(float c) {
        courageBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CBAR_WIDTH * lastMaxCourage * Mathf.Clamp01(c / (PlayerControl.COURAGE_SLOT * lastMaxCourage)));
    }

    private void setCSub(float c) {
        courageBarSub.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CBAR_WIDTH * lastMaxCourage * Mathf.Clamp01(c / (PlayerControl.COURAGE_SLOT * lastMaxCourage)));
    }

    private void setCSlots(float c) {
        courageBarSlots.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Min(c, 1f) + CBAR_WIDTH * lastMaxCourage * Mathf.Clamp01(c / (PlayerControl.COURAGE_SLOT * lastMaxCourage)));
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

    private void setCBarMax(int slots) {
        if(slots > 0) {
            if(!courageShown) {
                setCActive(true);
            }
            Vector2 pos = hpBar.anchoredPosition;
            float x = pos.x + HPBAR_WIDTH - slots * CBAR_WIDTH + cBarOffset.x;
            setCBarPos(x, pos.y + cBarOffset.y, slots);
        }
        else if(courageShown){
            setCActive(false);
        }
    }

    private void setCActive(bool a) {
        courageBarBack.gameObject.SetActive(a);
        courageBarSub.gameObject.SetActive(a);
        courageBar.gameObject.SetActive(a);
        courageBarSlots.gameObject.SetActive(a);
        courageIcon.gameObject.SetActive(a);
        courageShown = a;
    }

    private void setCBarPos(float x, float y, int slots) {
        courageBarBack.anchoredPosition = new Vector3(x - OUTLINE, y, 0f);
        resize(courageBarBack, CBAR_WIDTH * slots + OUTLINE * 2f, CBAR_HEIGHT + OUTLINE * 2f);

        courageBarSub.anchoredPosition = new Vector3(x, y, 0f);
        resize(courageBarSub, CBAR_WIDTH * slots, CBAR_HEIGHT);

        courageBar.anchoredPosition = new Vector3(x, y, 0f);
        resize(courageBar, CBAR_WIDTH * slots, CBAR_HEIGHT);

        courageBarSlots.anchoredPosition = new Vector3(x, y, 0f);
        resize(courageBarSlots, CBAR_WIDTH * slots, CBAR_HEIGHT);

        courageIcon.anchoredPosition = new Vector3(x, y, 0f);
    }

    private void resize(RectTransform r, float w, float h) {
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
    }

    float lerpDelta(float fromValue, float toValue, float progress) {
        return fromValue + (toValue - fromValue) * Mathf.Clamp01(progress * Time.deltaTime * 60f);
    }
}
