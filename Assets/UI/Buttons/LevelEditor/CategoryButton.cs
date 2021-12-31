using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{
    public Category category;
    public Texture2D openTexture;

    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
        category = Vars.main.content.defaultCategory;
    }

    void Clicked() {
        if(category == null) return;
        StopAllCoroutines();
        if(LChunkLoader.main.frag.OpenCategories()) {
            RawImage im = transform.GetChild(0).gameObject.GetComponent<RawImage>();
            im.texture = openTexture;
            im.color = category.blackIcon ? Color.black : Color.white;
            StartCoroutine(FoldIn(im.gameObject.GetComponent<RectTransform>()));
        }
        else {
            SetCategory(category);
        }
    }

    public void SetCategory(Category cat) {
        category = cat;
        RawImage im = transform.GetChild(0).gameObject.GetComponent<RawImage>();
        im.texture = cat.icon;
        im.color = Color.white;
        RectTransform rect = im.gameObject.GetComponent<RectTransform>();
        rect.localScale = new Vector3(rect.localScale.y, rect.localScale.y, rect.localScale.z);
    }

    IEnumerator FoldIn(RectTransform rect) {
        rect.localScale = new Vector3(0f, rect.localScale.y, rect.localScale.z);

        for(int i = 0; i <= 15; i++) {
            if(!rect.gameObject.activeInHierarchy) yield break;
            rect.localScale = new Vector3(Mathf.Lerp(0f, rect.localScale.y, i / 15f), rect.localScale.y, rect.localScale.z);
            yield return null;
        }

        rect.localScale = new Vector3(rect.localScale.y, rect.localScale.y, rect.localScale.z);
    }
}
