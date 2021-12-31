using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TriggerUpdater;

public class LTagUpdater : LBlockUpdater {
    public Sprite rangeSprite, crossX, crossY;
    public override void SetBlock(Block b, byte ctype) {
        type = b;
        this.ctype = ctype;
        if(b.sprite != null) {
            //GetComponent<SpriteRenderer>().sprite = b.sprite;
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = b.sprite;
            if(b is Trigger trig) {
                SpriteRenderer rangeRender = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
                rangeRender.transform.localPosition = new Vector3(0, 0, 0.01f);
                TagRange range = CtypeRange(ctype);
                if(range == TagRange.Infinite) {
                    rangeRender.gameObject.SetActive(false);
                }
                else{
                    rangeRender.gameObject.SetActive(true);
                    rangeRender.color = trig.uiColor;
                    if(range <= TagRange.Nine) {
                        rangeRender.drawMode = SpriteDrawMode.Sliced;
                        rangeRender.sprite = rangeSprite;
                        int r = (int)range;
                        int w = r * 2 + 1;
                        int h = w;
                        if(trig.defaultYCross) {
                            rangeRender.transform.localPosition = new Vector3(0, r / 2f, 0.01f);
                            h = r + 1;
                        }
                        rangeRender.size = new Vector2(w, h);
                        rangeRender.gameObject.transform.localScale = new Vector3(1, 1, 1);
                    }
                    else {
                        rangeRender.drawMode = SpriteDrawMode.Simple;
                        rangeRender.sprite = range == TagRange.CrossX ? crossX : crossY;
                        rangeRender.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                    }
                }
            }
        }
    }
}
