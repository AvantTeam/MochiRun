using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategorySelectButton : MonoBehaviour
{
    public Category category;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    void Clicked() {
        if(category == null) return;
        LChunkLoader.main.frag.SetCategory(category);
    }

    public void SetCategory(Category cat) {
        category = cat;
        GameObject icon = transform.GetChild(0).gameObject;
        icon.GetComponent<RawImage>().texture = cat.icon;
        GetComponent<Image>().color = cat.color;

        if(!cat.blackIcon && GetComponent<Outline>() == null) {
            float gray = 0.75f;
            Color outc = new Color(cat.color.r * gray, cat.color.g * gray, cat.color.b * gray, 1f);
            Outline o1 = icon.AddComponent<Outline>();
            Outline o2 = icon.AddComponent<Outline>();
            o1.effectColor = outc;
            o2.effectColor = outc;
            o1.effectDistance = new Vector2(0f, 3f);
            o2.effectDistance = new Vector2(3f, 0f);
        }
    }
}
