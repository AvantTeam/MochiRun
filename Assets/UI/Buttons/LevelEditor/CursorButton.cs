using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CursorControl;

public class CursorButton : MonoBehaviour
{
    [Serializable]
    public struct StateSprite {
        public STATE state;
        public Texture2D image;
    }

    public StateSprite[] sprites;
    public STATE[] desktopStates;
    public STATE[] mobileStates;
    public RawImage icon;

    private STATE currentState;

    void Start()
    {
        currentState = STATE.NONE;
        SetSprite(STATE.NONE);
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    void Update()
    {
        if(currentState != LChunkLoader.main.cursor.state){
            currentState = LChunkLoader.main.cursor.state;
            SetSprite(currentState);
        }
    }

    private void SetSprite(STATE s) {
        int i = id(s);
        if(i != -1) icon.texture = sprites[i].image;
    }

    private int id(STATE s) {
        int n = sprites.Length;
        for(int i = 0; i < n; i++) {
            if(sprites[i].state == s) return i;
        }
        return -1;
    }

    private void Clicked() {
        //todo
    }
}
