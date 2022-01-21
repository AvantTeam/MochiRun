using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CursorControl;

public class CursorButton : MonoBehaviour
{
    [Serializable]
    public struct StateSprite {
        public STATE state;
        public Texture2D image;
        public string name;
    }

    public StateSprite[] sprites;
    public STATE[] desktopStates;
    public STATE[] mobileStates;
    public RawImage icon;

    //elements & prefabs
    public GameObject pane, buttonPrefab;

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
        if(pane.activeInHierarchy) pane.SetActive(false);
        else {
            buildPane();
            pane.SetActive(true);
        }
    }

    private void buildPane() {
        UI.ClearChildren(pane);

        if(Vars.mobile) {
            foreach(STATE s in mobileStates) button(s);
        }
        else {
            foreach(STATE s in desktopStates) button(s);
        }
    }

    private void button(STATE s) {
        GameObject b = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
        b.GetComponent<Button>().onClick.AddListener(() => {
            LChunkLoader.main.cursor.SetState(s);
            pane.SetActive(false);
        });

        int i = id(s);
        if(i != -1) b.transform.GetChild(0).GetComponent<RawImage>().texture = sprites[i].image;
        b.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = sprites[i].name;

        b.transform.SetParent(pane.transform, false);
    }
}
