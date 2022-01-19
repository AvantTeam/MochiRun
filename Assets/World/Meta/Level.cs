using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkLoader;

[Serializable]
public class Level {
    [NonSerialized] public List<BlockSave> blocks;
    public string name = "Untitled"; //must equal the file name
    public bool campaign = false; //if campaign is true, the max health, courage etc. are overriden by story mode's progression.

    public Vector2 gravity = new Vector2(0f, -18f);
    public float maxHealth = 100f;
    public float healthLoss = 4f;
    public float maxSpeed = 4f;
    public float jumpHeight = 2f; //in blocks
    public int courages = 0;
    public float zoom = 1f;
    //todo remove? enumify?
    public IslandBackground islands = IslandBackground.islandMany;

    //io fields
    public string[] palette;
    //public string blockStream;
    public byte version = 0;

    public Level() {
        blocks = new List<BlockSave>();
    }

    public Level(List<BlockSave> blocks) {
        this.blocks = blocks;
    }
}