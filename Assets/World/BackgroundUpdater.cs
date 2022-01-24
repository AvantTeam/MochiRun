using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundUpdater : MonoBehaviour {
    private const float WIDTH = 20.48f / 5f, ZBACK = 30f;

    public float z = 1f;
    public GameObject cam;

    void Update() {
        transform.position = new Vector3(-clampX(cam.transform.position.x * 0.05f / z), Mathf.Min(0f, -cam.transform.position.y * 0.01f / z), z + ZBACK);
    }

    private float clampX(float x) {
        return x % WIDTH - WIDTH / 2f;
    }
}
