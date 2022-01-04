using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelOptionsDialog : MonoBehaviour
{
    //elements
    public GameObject frame;
    //prefabs
    public GameObject floatField, intField, listField;

    void Start()
    {
        Rebuild(LChunkLoader.main.GetLevel());
    }

    protected void Rebuild(Level level) {
        clearChildren(frame);
        fieldf("Health", () => level.maxHealth, v => level.maxHealth = v , 0f, 300f, 25f);
        fieldf("HP Loss/s", () => level.healthLoss, v => level.healthLoss = v, 0f, 12f, 2f);
        fieldf("Speed", () => level.maxSpeed, v => level.maxSpeed = v, 1f, 10f, 1f);
        fieldf("Jump", () => level.jumpHeight, v => level.jumpHeight = v, 0f, 5f, 0.5f);
        fieldi("Courage", () => level.courages, v => level.courages = v, 0, 5, 1);
        fieldf("Zoom", () => level.zoom, v => level.zoom = v, 0.1f, 1.5f, 0.1f);
    }

    private void fieldf(string name, Func<float> get, Action<float> set, float min, float max, float step) {
        GameObject o = Instantiate(floatField, Vector3.zero, Quaternion.identity);
        setChildTitle(o, name);
        o.GetComponent<FloatField>().Set(get, () => set(Mathf.Max(min, get() - step)), () => set(Mathf.Min(max, get() + step)), set);
        o.transform.SetParent(frame.transform);
    }

    private void fieldi(string name, Func<int> get, Action<int> set, int min, int max, int step) {
        GameObject o = Instantiate(intField, Vector3.zero, Quaternion.identity);
        setChildTitle(o, name);
        o.GetComponent<IntField>().Set(get, () => set(Mathf.Max(min, get() - step)), () => set(Mathf.Min(max, get() + step)), set);
        o.transform.SetParent(frame.transform);
    }


    private void setChildTitle(GameObject o, string s) {
        o.transform.GetChild(0).gameObject.GetComponent<Text>().text = s + ": ";
    }

    private void clearChildren(GameObject o) {
        int n = o.transform.childCount;
        if(n <= 0) return;
        for(int i = n - 1; i >= 0; i--) {
            Destroy(o.transform.GetChild(i).gameObject);
        }
    }
}
