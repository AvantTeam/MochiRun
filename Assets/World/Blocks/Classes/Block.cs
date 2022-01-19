using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//something in the world is a Block.
[CreateAssetMenu(fileName = "NewBlock", menuName = "Blocks/Block", order = 1)]
public class Block : ScriptableObject {
    //public static List<Block> blocks = new List<Block>();
    public static Quaternion[] rotation = new Quaternion[]{Quaternion.identity, Quaternion.Euler(0, 0, 90), Quaternion.Euler(0, 0, 180), Quaternion.Euler(0, 0, 270)};

    [ReadOnly] public bool hasObject = false; //false is for "special" Blocks like pits.
    [ReadOnly] public bool hasUpdate = false; //whether the prefab stores its Block instance.
    public bool rotate = false; //wheter ctype is rotation (ccw, 0~3)
    public bool hidden = false;
    public GameObject prefab;
    public Sprite sprite = null;
    public float zLayer = 0f;
    [ReadOnly] public float width = 1f;
    [ReadOnly] public float height = 1f;
    public float clipsize = 4f;
    public bool onFloor = false; //whether to provide the floor + height / 2 as the y position on init().
    public Category category;

    //public int id;

    protected virtual void OnValidate() {
        hasObject = prefab != null && !prefab.name.StartsWith("Trigger");
        if(hasObject) {
            hasUpdate = prefab.GetComponent<BlockUpdater>() != null;
            width = prefab.transform.localScale.x;
            height = prefab.transform.localScale.y;
            clipsize = width;
        }
        else if(prefab != null) {
            hasUpdate = prefab.GetComponent<BlockUpdater>() != null;
            width = height = clipsize = 1f;
        }
        else {
            hasUpdate = false;
        }
    }

    public Block() {
        //id = blocks.Count;
        //blocks.Add(this);
    }

    
    public Block(GameObject o) {
        if(o != null && !o.name.StartsWith("Trigger")) {
            hasObject = true;
            prefab = o;
            hasUpdate = o.GetComponent<BlockUpdater>() != null;
            width = o.transform.localScale.x;
            height = o.transform.localScale.y;

            //todo remove
            //Debug.Log(o.ToString() + "'s BlockUpdater:" + (o.GetComponent<BlockUpdater>() == null ? "NULL" : o.GetComponent<BlockUpdater>().ToString()));
        }

        //id = blocks.Count;
        //blocks.Add(this);
    }

    //called when a block is being spawned (typically out of screen). ctype is only saved for blocks with hasUpdate set to true.
    public virtual void init(float x, float y, byte ctype) {
        if(prefab != null){
            GameObject newo = Object.Instantiate(prefab, new Vector3(x, y, zLayer), rotate ? rotation[ctype % 4] : Quaternion.identity);
            if(hasUpdate){
                BlockUpdater bu = newo.GetComponent<BlockUpdater>();
                bu.type = this;
                bu.ctype = ctype;
            }
        }
    }
}