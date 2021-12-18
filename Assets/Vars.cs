using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vars : MonoBehaviour
{
    public static Vars main;
    public Blocks blocks;
    public int test = 0;

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
    }
}
