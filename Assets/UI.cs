using System;
using UnityEngine;

public static class UI {
    public static char[] blackchars = { '<', '>', ':', '\"', '/', '\\', '|', '?', '*', '\n', '\r' }; //these cannot be used in file names
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
}
