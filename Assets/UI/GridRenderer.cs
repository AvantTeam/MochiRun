using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    public Material normal, xray;
    public MODE currentMode;

    public enum MODE {
        NONE,
        NORMAL,
        XRAY
    }
    public int modes;

    GameObject cam;
    Camera camc;
    private float lastW, lastH;
    void Start()
    {
        cam = GameObject.Find("Main Camera");
        camc = cam.GetComponent<Camera>();
        modes = Enum.GetNames(typeof(MODE)).Length;
        Resize(5f, 5f);
        SetMode(MODE.NORMAL);
    }

    void Update()
    {
        Resize(2.1f * camc.orthographicSize * Screen.width / Screen.height, 2.1f * camc.orthographicSize);
        transform.position = (Vector2)cam.transform.position;
    }

    private void Resize(float w, float h) {
        if(w != lastW || h != lastH) {
            lastW = w; lastH = h;
            transform.localScale = new Vector3(w / 10f, 1, h / 10f);
        }
    }

    public void SetMode(MODE mode) {
        currentMode = mode;
        if(mode == MODE.NONE) {
            gameObject.SetActive(false);
        }
        else {
            gameObject.SetActive(true);
            GetComponent<MeshRenderer>().material = (mode == MODE.XRAY ? xray : normal);
        }
    }

    public void ToggleMode() {
        int current = (int)currentMode;
        if(modes == 0) modes = Enum.GetNames(typeof(MODE)).Length;
        SetMode((MODE) ((current + 1) % modes));
    }
}
