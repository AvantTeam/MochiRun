using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelOptionsDialog : MonoBehaviour {
    //elements
    public GameObject frame;
    //prefabs
    public GameObject floatField, intField, listField, stringField, boolField;

    void Start() {
        Rebuild(LChunkLoader.main.GetLevel());
    }

    public void OnEnable() {
        Rebuild(LChunkLoader.main.GetLevel());
    }

    protected void Rebuild(Level level) {
        UI.ClearChildren(frame);
        fieldString("Name", () => level.name, v => level.name = v, true, 25);
        fieldf("Health", () => level.maxHealth, v => level.maxHealth = v , 0f, 300f, 25f);
        fieldf("HP Loss/s", () => level.healthLoss, v => level.healthLoss = v, 0f, 12f, 2f);
        fieldf("Speed", () => level.maxSpeed, v => level.maxSpeed = v, 1f, 10f, 1f);
        fieldf("Jump", () => level.jumpHeight, v => level.jumpHeight = v, 0f, 5f, 0.5f);
        fieldf("Courage", () => level.courage, v => level.courage = v, 0, 5f, 1f);
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

    private void fieldString(string name, Func<string> get, Action<string> set, bool filter, int maxChars) {
        GameObject o = Instantiate(stringField, Vector3.zero, Quaternion.identity);
        setChildTitle(o, name);
        if(maxChars < 0) maxChars = 500;
        if(filter) o.GetComponent<StringField>().Set(get, s => set(Limit(Blackchar(s.Trim()), maxChars)));
        else o.GetComponent<StringField>().Set(get, s => set(Limit(s.Trim().Replace("\n", null).Replace("\r", null), maxChars)));
        o.transform.SetParent(frame.transform);
    }


    private void setChildTitle(GameObject o, string s) {
        o.transform.GetChild(0).gameObject.GetComponent<Text>().text = s + " ";
    }

    private string Limit(string s, int maxn) {
        return s.Substring(0, Mathf.Min(s.Length, maxn));
    }

    private string Blackchar(string s) {
        foreach(char c in UI.blackchars) {
            s = s.Replace(c, '*');
        }
        return s.Replace("*", null);
    }
}
