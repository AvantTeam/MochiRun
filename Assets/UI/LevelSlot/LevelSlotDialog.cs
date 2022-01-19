using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

//Displays all levels in the base directory.
public class LevelSlotDialog : MonoBehaviour {
    //elements
    public GameObject cont;

    //prefabs
    public GameObject horizontal, levelPrefab, emptyPrefab;

    public static string SavePath(string name) {
        return Application.persistentDataPath + @"\levels\" + name + ".mlvl";
    }

    public static string SaveDirectory() {
        return Application.persistentDataPath + @"\levels";
    }

    public void Build() {
        string rootPath = SaveDirectory();
        Debug.Log("Level folder: " + rootPath);
        clearChildren(cont);

        try {
            if(!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
            var levels = Directory.EnumerateFiles(rootPath, "*.mlvl");

            int n = 0;
            GameObject currentv = null;
            foreach(string p in levels) {
                Debug.Log(p);
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
        }
        catch(Exception e){
            Debug.LogError(e);
        }
    }

    private GameObject button(string path) {
        GameObject button = pref(levelPrefab);
        FileInfo file = new FileInfo(path);
        string name = file.Name;
        name = name.Substring(0, name.Length - 5);
        button.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = name;
        button.transform.Find("Date").gameObject.GetComponent<TextMeshProUGUI>().text = file.LastWriteTime.ToString();
        //todo rule icons
        button.GetComponent<LevelSlotButton>().Set(path, gameObject);

        return button;
    }

    private GameObject pref(GameObject o) {
        return Instantiate(o, Vector3.zero, Quaternion.identity);
    }

    private void clearChildren(GameObject o) {
        int n = o.transform.childCount;
        if(n <= 0) return;
        for(int i = n - 1; i >= 0; i--) {
            Destroy(o.transform.GetChild(i).gameObject);
        }
    }
}
