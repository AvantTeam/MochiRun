using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveLevelButton : MonoBehaviour {
    private const string defName = "Untitled";

    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    void Clicked() {
        SaveLevel();
    }

    public static bool SaveLevel() {
        LChunkLoader lc = LChunkLoader.main;
        lc.RecacheBlocks();
        Level level = lc.GetLevel();

        string rootPath = LevelSlotDialog.SaveDirectory();
        try {
            if(!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);

            if(level.name.Equals(defName)) {
                //name the level "Untitled0, 1, ..." to prevent overriding
                int i = 0;
                while(i <= 99) {
                    if(!File.Exists(LevelSlotDialog.SavePath(defName + i))) break;
                    i++;
                }
                level.name = defName + i;
            }

            string path = LevelSlotDialog.SavePath(level.name);
            Debug.Log("Saving level to " + path);
            LevelIO.write(level, path);

            UI.Announce("Saved \'" + level.name + ".mlvl\'", 3f);
            return true;
        }
        catch(System.Exception e) {
            Debug.LogError(e);
            UI.Announce("<color=#ff7777>Failed to save \'" + level.name + ".mlvl\', please retry!</color>", 3f);
        }
        return false;
    }
}
