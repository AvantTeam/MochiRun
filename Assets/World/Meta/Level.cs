using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkLoader;

[Serializable]
public class Level {
    [NonSerialized] public List<BlockSave> blocks;
    [NonSerialized] public Theme theme;

    public string name = "Untitled"; //must equal the file name
    public bool campaign = false; //if campaign is true, the max health, courage etc. are overriden by story mode's progression.

    public Vector2 gravity = new Vector2(0f, -18f);
    public float maxHealth = 100f;
    public float healthLoss = 4f;
    public float maxSpeed = 4f;
    public float jumpHeight = 2f; //in blocks
    //public int courages = 0;
    public float courage = 0f;
    public float zoom = 1f;
    //todo remove? enumify?
    public IslandBackground islands = IslandBackground.islandMany;

    //io fields
    public string[] palette;
    public byte version = 0;
    public string themeName = "Plains";

    public Level() {
        blocks = new List<BlockSave>();
        //theme = Vars.main.content.theme("Plains");
    }

    public Level(List<BlockSave> blocks) {
        this.blocks = blocks;
        //theme = Vars.main.content.theme("Plains");
    }

    public Theme GetTheme() {
        if(theme == null) theme = Vars.main.content.theme("Plains");
        return theme;
    }
}