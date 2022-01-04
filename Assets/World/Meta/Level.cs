using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkLoader;

public class Level {
    public List<BlockSave> blocks;
    public bool campaign = false; //if campaign is true, the max health, courage etc. are overriden by story mode's progression.

    public Vector2 gravity = new Vector2(0f, -18f);
    public float maxHealth = 100f;
    public float healthLoss = 4f;
    public float maxSpeed = 4f;
    public float jumpHeight = 2f; //in blocks
    public int courages = 0;
    public float zoom = 1f;
    public IslandBackground islands = IslandBackground.islandMany;

    public Level() {
        blocks = new List<BlockSave>();
    }

    public Level(List<BlockSave> blocks) {
        this.blocks = blocks;
    }
}