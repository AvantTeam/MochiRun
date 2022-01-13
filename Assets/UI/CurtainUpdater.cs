using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurtainUpdater : MonoBehaviour
{
    public bool blackout;
    private float lastWidth, lastHeight;

    RectTransform rect;
    GameObject canvas;
    Image image;

    void Awake(){
        GetComponent<Image>().enabled = false;
        rect = GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>().gameObject;
        image = GetComponent<Image>();
    }

    void Update() {
        if(image.enabled && (lastWidth != Screen.width || lastHeight != Screen.height)) fillScreen();
    }

    private void fillScreen() {
        lastWidth = Screen.width;
        lastHeight = Screen.height;
        resize(rect, lastWidth, lastHeight + 64);
    }

    public void Show(float duration) {
        StopAllCoroutines();
        StartCoroutine(FadeIn(duration, blackout));
    }

    //instantly sets the curtain; useful between screen transitions
    public void Set(bool show) {
        transform.SetParent(canvas.transform);
        if(show) {
            rect.anchoredPosition = new Vector2(0f, 0f);
            fillScreen();
            image.color = blackout ? Color.black : Color.white;
            image.enabled = true;
        }
        else {
            rect.anchoredPosition = new Vector2(0f, Screen.height + 64f);
            fillScreen();
            image.enabled = false;
        }
    }

    public void Hide(float duration) {
        StopAllCoroutines();
        if(!image.enabled) Destroy(gameObject);
        else StartCoroutine(FadeOut(duration, blackout));
    }

    IEnumerator FadeIn(float duration, bool black) {
        if(image.enabled) yield break;
        float h = image.enabled ? rect.anchoredPosition.y : Screen.height + 64f;
        image.color = Color.white;
        
        transform.SetParent(canvas.transform);
        fillScreen();
        
        rect.anchoredPosition = new Vector2(0f, h);
        image.enabled = true;

        float f = 0f;
        while(f < 1f) {
            f += Time.deltaTime / duration;
            if(f > 1f) f = 1f;
            rect.anchoredPosition = new Vector2(0f, h * (1 - f) * (1 - f) * (1 - f));
            if(black) image.color = Color.Lerp(Color.white, Color.black, Mathf.Clamp01(f * 2f - 1));
            yield return null;
        }

        rect.anchoredPosition = new Vector2(0f, 0f);
        if(black) image.color = Color.black;
    }

    IEnumerator FadeOut(float duration, bool black) {
        transform.SetParent(canvas.transform);
        fillScreen();
        if(!image.enabled) yield break;
        float h = rect.anchoredPosition.y;

        float f = 0f;
        while(f < 1f) {
            f += Time.deltaTime / duration;
            if(f > 1f) f = 1f;
            rect.anchoredPosition = new Vector2(0f, f * (Screen.height + 64f) + (1 - f) * h);
            if(black) image.color = Color.Lerp(Color.black, Color.white, Mathf.Clamp01(f * 2f));
            yield return null;
        }

        rect.anchoredPosition = new Vector2(0f, Screen.height + 64f);
        image.enabled = false;
        Destroy(gameObject);
    }

    private void resize(RectTransform r, float w, float h) {
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
    }
}
