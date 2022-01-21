using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkLoader;

public class Vars : MonoBehaviour
{
    public static Vars main;
    public VarsPrefab prefabs;
    public ContentList content;

#if UNITY_ANDROID
    public const bool mobile = true;
#else
    public const bool mobile = false;
#endif

    public string firstScene;
    public Level tempBlockSaves;

    protected Blocks blocks;
    void Awake() {
        if(main != null && main != this)
            Destroy(gameObject);
        else
            main = this;

        DontDestroyOnLoad(this);
    }

    public void Load() {
        main.blocks = new Blocks();
        main.blocks.Load();

        KeyBinds.Load();
    }
}

[System.Serializable]
public class VarsPrefab {
    public GameObject announcement, curtain, ynpopup, stringpopup;
}