using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Displays all levels in the base directory.
public class LevelSlotDialog : MonoBehaviour {
    //elements
    public GameObject cont;
    //prefabs
    public GameObject horizontal, levelPrefab, emptyPrefab;

    [Serializable]
    public struct RuleTexture {
        public Texture2D texture;
        public int threshold;
    }

    //rule icons
    public RuleTexture[] healthIcons;
    public RuleTexture[] courageIcons;
    public Texture2D noHealthLoss;
    public Texture2D campaignIcon;

    private List<FileInfo> tmpf = new List<FileInfo>();

    public static string SavePath(string name) {
        return Application.persistentDataPath + @"\levels\" + name + ".mlvl";
    }

    public static string SaveDirectory() {
        return Application.persistentDataPath + @"\levels";
    }

    public void Build() {
        string rootPath = SaveDirectory();
        Debug.Log("Level folder: " + rootPath);
        UI.ClearChildren(cont);

        try {
            if(!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
            var levels = Directory.EnumerateFiles(rootPath, "*.mlvl");

            //sort levels by date
            tmpf.Clear();
            foreach(string p in levels) {
                tmpf.Add(new FileInfo(p));
            }
            tmpf.Sort((f1, f2) => f2.LastWriteTime.CompareTo(f1.LastWriteTime));

            int n = 0;
            GameObject currentv = null;
            foreach(FileInfo p in tmpf) {
                //Debug.Log(p.FullName);
                if(n % 2 == 0) {
                    //create horizontal
                    currentv = pref(horizontal);
                    currentv.transform.SetParent(cont.transform);
                }
                button(p).transform.SetParent(currentv.transform);

                n++;
            }

            if(n % 2 == 1) {
                GameObject empty = pref(emptyPrefab);
                empty.transform.SetParent(currentv.transform);
            }

            tmpf.Clear();
        }
        catch(Exception e){
            Debug.LogError(e);
        }
    }

    private GameObject button(FileInfo file) {
        GameObject button = pref(levelPrefab);
        //FileInfo file = new FileInfo(path);
        string name = file.Name;
        if(name.EndsWith(".mlvl")) name = name.Substring(0, name.Length - 5);
        button.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = name;
        button.transform.Find("Date").gameObject.GetComponent<TextMeshProUGUI>().text = file.LastWriteTime.ToString("yyyy/mm/dd HH:mm");
        //rule icons
        setIcons(button, file.FullName);
        button.GetComponent<LevelSlotButton>().Set(file.FullName, gameObject);

        return button;
    }

    private GameObject pref(GameObject o) {
        return Instantiate(o, Vector3.zero, Quaternion.identity);
    }

    private GameObject pref(GameObject o, Transform parent) {
        GameObject p = pref(o);
        p.transform.SetParent(parent);
        return p;
    }

    private void setIcons(GameObject button, string path) {
        GameObject ic = button.transform.Find("RuleIcons").GetChild(0).gameObject;
        Level l = LevelIO.fetchMeta(path);

        if(l.campaign) {
            pref(ic, ic.transform.parent).GetComponent<RawImage>().texture = campaignIcon;
        }

        //health
        foreach(RuleTexture rt in healthIcons) {
            if(l.maxHealth >= rt.threshold) ic.GetComponent<RawImage>().texture = rt.texture;
        }

        //hp loss
        if(l.healthLoss <= 0.1f) {
            pref(ic, ic.transform.parent).GetComponent<RawImage>().texture = noHealthLoss;
        }

        //courage
        if(l.courages >= 1) {
            GameObject icc = pref(ic, ic.transform.parent);
            foreach(RuleTexture rt in courageIcons) {
                if(l.courages >= rt.threshold) icc.GetComponent<RawImage>().texture = rt.texture;
            }
        }
    }
}
