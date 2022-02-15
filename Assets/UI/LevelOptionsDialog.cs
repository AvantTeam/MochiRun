using UnityEngine;

using static UI;

public class LevelOptionsDialog : MonoBehaviour {
    //elements
    public GameObject frame;
    //prefabs
    //public GameObject floatField, intField, listField, stringField, boolField;

    void Start() {
        Rebuild(LChunkLoader.main.GetLevel());
    }

    public void OnEnable() {
        Rebuild(LChunkLoader.main.GetLevel());
    }

    protected void Rebuild(Level level) {
        ClearChildren(frame);
        FrameFields(frame);
        fieldString("Name", () => level.name, v => level.name = v, true, 25);
        fieldList<Theme>("Theme", level.GetTheme, v => {
            level.theme = v;
            LChunkLoader.main.ClearFloor();
            }, Vars.main.content.themes);
        fieldf("Health", () => level.maxHealth, v => level.maxHealth = v , 0f, 300f, 25f);
        fieldf("HP Loss/s", () => level.healthLoss, v => level.healthLoss = v, 0f, 12f, 2f);
        fieldf("Speed", () => level.maxSpeed, v => level.maxSpeed = v, 1f, 10f, 1f);
        fieldf("Jump", () => level.jumpHeight, v => level.jumpHeight = v, 0f, 5f, 0.5f);
        fieldf("Courage", () => level.courage, v => level.courage = v, 0, 5f, 1f);
        fieldf("Gravity", () => -level.gravity.y, v => {
            level.gravity = new Vector2(0f, -v);
        }, 9, 36, 9);

        fieldb("Campaign", () => level.campaign, v => level.campaign = v);
        fieldf("Zoom", () => level.zoom, v => level.zoom = v, 0.1f, 1.5f, 0.1f);

        //reverse order of children
        int n = frame.transform.childCount;
        for(int i = 0; i < n - 1; i++) {
            frame.transform.GetChild(0).SetSiblingIndex(n - 1 - i);
        }
    }
}
