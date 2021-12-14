using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//something in the world is a Block.
public class Block {
    public static List<Block> blocks = new List<Block>();

    public bool hasObject = false; //false is for "special" Blocks like pits.
    public bool hasUpdate = false; //whether the prefab stores its Block instance.
    public GameObject prefab;
    public float zLayer = 0f;
    public float width = 1f;
    public float height = 1f;
    public float clipsize = 4f;
    public bool onFloor = false; //whether to provide the floor + height / 2 as the y position on init().

    public int id;
    public Block() {
        id = blocks.Count;
        blocks.Add(this);
    }

    public Block(GameObject o) {
        hasObject = true;
        prefab = o;
        hasUpdate = o.GetComponent<BlockUpdater>() != null;
        width = o.transform.localScale.x;
        height = o.transform.localScale.y;

        id = blocks.Count;
        blocks.Add(this);
    }

    public Block(string o) : this() { }

    private static GameObject findPrefab(string prefabName) {
        return null;
    }

    //called when a block is being spawned (typically out of screen). ctype is only saved for blocks with hasUpdate set to true.
    public virtual void init(float x, float y, byte ctype) {
        if(hasObject){
            GameObject newo = Object.Instantiate(prefab, new Vector3(x, y, zLayer), Quaternion.identity);
            if(hasUpdate){
                BlockUpdater bu = newo.GetComponent<BlockUpdater>();
                bu.type = this;
                bu.ctype = ctype;
            }
        }
    }
}

public class Floor : Block {
    public float zRand = 0.7f;
    protected bool yBoop = false; //prevent z-fighting

    public Floor(GameObject o) : base(o) {
        width = ChunkLoader.FLOOR_WIDTH;
        height = ChunkLoader.FLOOR_HEIGHT;
    }
    public override void init(float x, float y, byte ctype) {
        if(hasObject) {
            GameObject newo = Object.Instantiate(prefab, new Vector3(x, y + (yBoop ? 0.01f : 0f), zLayer + Random.Range(-zRand, zRand)), Quaternion.identity);
            if(yBoop){
                BoxCollider2D colli = newo.GetComponent<BoxCollider2D>();
                colli.offset = new Vector2(colli.offset.x, colli.offset.y - 0.01f / height);
            }
            yBoop = !yBoop;
            if(hasUpdate) {
                BlockUpdater bu = newo.GetComponent<BlockUpdater>();
                bu.type = this;
                bu.ctype = ctype;
            }
        }
    }
}

public class PitStarter : Block {
    public PitStarter() : base() {
        Debug.Log(id);
        clipsize = 4f;
        onFloor = true;
    }

    public override void init(float x, float y, byte ctype) {
        Vector2 s = new Vector2(x, y);
        Vector2 e = s + Vector2.down * 1f;
        Collider2D collider = Physics2D.Linecast(s, e).collider;
        if(collider != null && collider.gameObject.CompareTag("Floor")) Object.Destroy(collider.gameObject); //destroy the floor under this
        ChunkLoader.isPit = true;
        Debug.Log("Start Pit!");
    }
}

public class PitEnder : Block {
    public PitEnder() : base() {
        Debug.Log(id);
        clipsize = 4f;
        onFloor = true;
    }

    public override void init(float x, float y, byte ctype) {
        ChunkLoader.lastX = x - width / 2f + ChunkLoader.FLOOR_WIDTH / 2f;
        ChunkLoader.isPit = false;
    }
}