using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundUpdater : MonoBehaviour {
    private const float WIDTH = 20.48f / 5f, ZBACK = 30f;

    public float z = 1f, z2 = 1.5f;
    public GameObject cam, bg1, bg2, fog;

    void Start() {
        if(LevelLoader.main != null) {
            bg1.GetComponent<SpriteRenderer>().sprite = LevelLoader.main.loading.GetTheme().background1;
            bg2.GetComponent<SpriteRenderer>().sprite = LevelLoader.main.loading.theme.background2;
            fog.GetComponent<SpriteRenderer>().color = LevelLoader.main.loading.theme.fog;
            GetComponent<Camera>().backgroundColor = LevelLoader.main.loading.theme.sky;
        }
    }

    void Update() {
        bool scrolly = LevelLoader.main == null || LevelLoader.main.loading.theme.backgroundScrollY;
        bg1.transform.position = new Vector3(-clampX(cam.transform.position.x * 0.05f / z), scrolly ? Mathf.Min(0f, -cam.transform.position.y * 0.01f / z) : 0f, z + ZBACK);
        bg2.transform.position = new Vector3(-clampX(cam.transform.position.x * 0.05f / z2), scrolly ? Mathf.Min(0f, -cam.transform.position.y * 0.01f / z2) : 0f, z2 + ZBACK);
    }

    private float clampX(float x) {
        return x % WIDTH - WIDTH / 2f;
    }
}
