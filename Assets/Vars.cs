using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkLoader;

public class Vars : MonoBehaviour
{
    public static Vars main;
    public ContentList content;
    
    public string firstScene;
    public bool mobile = false; //todo

    protected Blocks blocks;
    public Level tempBlockSaves;

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
