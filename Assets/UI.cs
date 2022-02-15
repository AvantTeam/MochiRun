using System;
using UnityEngine;
using UnityEngine.UI;

public static class UI {
    public static char[] blackchars = { '<', '>', ':', '\"', '/', '\\', '|', '?', '*', '\n', '\r' }; //these cannot be used in file names
    private static GameObject frame;

    public static void Announce(string text) {
        Announce(text, 1f);
    }

    public static void Announce(string text, float duration) {
        AnnouncementUpdater au = UnityEngine.Object.Instantiate(Vars.main.prefabs.announcement, Vector3.zero, Quaternion.identity).GetComponent<AnnouncementUpdater>();
        au.Set(text, duration);
    }

    public static void ShowPopup(string text, Action confirm, Action deny) {
        ShowPopup(text, confirm, deny, 0, true);
    }

    public static void ShowPopup(string text, Action confirm, Action deny, int highlight, bool useEscape) {
        YNPopupUpdater au = UnityEngine.Object.Instantiate(Vars.main.prefabs.ynpopup, Vector3.zero, Quaternion.identity).GetComponent<YNPopupUpdater>();
        au.Set(text, confirm, deny, highlight, useEscape);
    }

    public static void StringPopup(string title, Action<string> confirm) {
        StringPopup(title, confirm, () => { }, "", 25, true);
    }

    public static void StringPopup(string title, Action<string> confirm, Action deny, string defText, int maxChars, bool filterFile) {
        SPopupUpdater au = UnityEngine.Object.Instantiate(Vars.main.prefabs.stringpopup, Vector3.zero, Quaternion.identity).GetComponent<SPopupUpdater>();
        au.Set(title, defText, confirm, deny, maxChars, filterFile);
    }

    public static void ClearChildren(GameObject o) {
        int n = o.transform.childCount;
        if(n <= 0) return;
        for(int i = n - 1; i >= 0; i--) {
            UnityEngine.Object.Destroy(o.transform.GetChild(i).gameObject);
        }
    }

    public static Color ColorOf(string hex) {
        if(!hex.StartsWith("#")) hex = "#" + hex;
        Color c = new Color();
        if(ColorUtility.TryParseHtmlString(hex, out c)) return c;
        return new Color();
    }

    //field editor methods
    public static void FrameFields(GameObject f) {
        frame = f;
    }

    public static void fieldf(string name, Func<float> get, Action<float> set, float min, float max, float step) {
        GameObject o = GameObject.Instantiate(Vars.main.prefabs.floatField, Vector3.zero, Quaternion.identity);
        setChildTitle(o, name);
        o.GetComponent<FloatField>().Set(get, () => set(Mathf.Max(min, get() - step)), () => set(Mathf.Min(max, get() + step)), set);
        o.transform.SetParent(frame.transform);
    }

    public static void fieldi(string name, Func<int> get, Action<int> set, int min, int max, int step) {
        GameObject o = GameObject.Instantiate(Vars.main.prefabs.intField, Vector3.zero, Quaternion.identity);
        setChildTitle(o, name);
        o.GetComponent<IntField>().Set(get, () => set(Mathf.Max(min, get() - step)), () => set(Mathf.Min(max, get() + step)), set);
        o.transform.SetParent(frame.transform);
    }

    public static void fieldb(string name, Func<bool> get, Action<bool> set) {
        GameObject o = GameObject.Instantiate(Vars.main.prefabs.boolField, Vector3.zero, Quaternion.identity);
        setChildTitle(o, name);
        o.GetComponent<BoolField>().Set(get, () => set(!get()), set);
        o.transform.SetParent(frame.transform);
    }

    public static void fieldString(string name, Func<string> get, Action<string> set, bool filter, int maxChars) {
        GameObject o = GameObject.Instantiate(Vars.main.prefabs.stringField, Vector3.zero, Quaternion.identity);
        setChildTitle(o, name);
        if(maxChars < 0) maxChars = 500;
        if(filter) o.GetComponent<StringField>().Set(get, s => set(Limit(Blackchar(s.Trim()), maxChars)));
        else o.GetComponent<StringField>().Set(get, s => set(Limit(s.Trim().Replace("\n", null).Replace("\r", null), maxChars)));
        o.transform.SetParent(frame.transform);
    }

    public static void fieldList<T>(string name, Func<T> get, Action<T> set, T[] list) {
        GameObject o = GameObject.Instantiate(Vars.main.prefabs.listField, Vector3.zero, Quaternion.identity);
        setChildTitle(o, name);
        o.GetComponent<ListField>().Set<T>(get, set, list);
        o.transform.SetParent(frame.transform);
    }


    private static void setChildTitle(GameObject o, string s) {
        o.transform.GetChild(0).gameObject.GetComponent<Text>().text = s + " ";
    }

    private static string Limit(string s, int maxn) {
        return s.Substring(0, Mathf.Min(s.Length, maxn));
    }

    public static string Blackchar(string s) {
        foreach(char c in blackchars) {
            s = s.Replace(c, '*');
        }
        return s.Replace("*", null);
    }
}
