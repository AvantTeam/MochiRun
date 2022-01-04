using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataStructures.Queue;
using static ChunkLoader;

public class LChunkLoader : MonoBehaviour
{
    private List<BlockSave> loadList = new List<BlockSave>(); //must be sorted by x. Will not update even if blocks are placed in the editor!
    private PriorityQueue<float, BlockSave> lUnload = new PriorityQueue<float, BlockSave>();
    private PriorityQueue<float, BlockSave> rUnload = new PriorityQueue<float, BlockSave>();
    private Level level;

    //public const float FLOOR_WIDTH = 4f, FLOOR_HEIGHT = 6f;

    private int listSize;
    private float lastL, lastR;

    public GameObject lblock, lfloor, ltag;
    public CursorControl cursor;
    public LevelEditorFragment frag;
    public static LChunkLoader main;

    GameObject cam;
    Camera camc;

    private void Awake() {
        main = this;
    }
    void Start()
    {
        cam = GameObject.Find("Main Camera");
        camc = cam.GetComponent<Camera>();
        resetLoadList();
        loadTestLevel();

        AfterLoad();
        PlaceAllBlocks();
        frag.Load();
    }

    // Update is called once per frame
    void Update()
    {
        placeFloors();
        //placeBlocks();
    }

    public void PlaceAllBlocks() {
        foreach(BlockSave bs in loadList) {
            placeBlock(bs);
        }
    }

    private void placeFloors() {
        float bound = GetBound();
        float xBoundR = cam.transform.position.x + bound + FLOOR_WIDTH;
        float xBoundL = cam.transform.position.x - bound - FLOOR_WIDTH;
        while(lastR < xBoundR) {
            Instantiate(lfloor, new Vector3(lastR, GetFloorY(lastR), 0f), Quaternion.identity);
            lastR += FLOOR_WIDTH;
        }
        while(lastL > xBoundL) {
            Instantiate(lfloor, new Vector3(lastL, GetFloorY(lastL), 0f), Quaternion.identity);
            lastL -= FLOOR_WIDTH;
        }
    }

    private void placeBlocks() {
        float bound = GetBound();
        float xBoundR = cam.transform.position.x + bound + 3f;
        float xBoundL = cam.transform.position.x - bound - 3f;
        while(rUnload.Count > 0 && rUnload.Top().x < xBoundR) {
            placeBlock(rUnload.Dequeue());
        }
        while(lUnload.Count > 0 && -lUnload.Top().x > xBoundL) {
            placeBlock(lUnload.Dequeue());
        }
    }

    public void AfterLoad() {
        listSize = loadList.Count;
        //my god this is O(n^2)
        foreach(BlockSave bs in loadList) {
            rUnload.Enqueue(bs.x, bs);
        }
        lastR = -20f;
        lastL = -21f;
    }

    public bool ShouldRemove(LBlockUpdater block) {
        float bound = GetBound();
        float xBoundR = cam.transform.position.x + bound + 3f;
        float xBoundL = cam.transform.position.x - bound - 3f;
        Vector3 pos = block.gameObject.transform.position;

        if(pos.x >= xBoundR) {
            rUnload.Enqueue(block.save.x, block.save);
            return true;
        }
        else if(pos.x <= xBoundL) {
            lUnload.Enqueue(-block.save.x, block.save);
            return true;
        }
        return false;
    }

    public bool ShouldRemoveFloor(GameObject block) {
        float bound = GetBound();
        float xBoundR = cam.transform.position.x + bound + FLOOR_WIDTH + 0.5f;
        float xBoundL = cam.transform.position.x - bound - FLOOR_WIDTH - 0.5f;
        Vector3 pos = block.transform.position;

        if(pos.x >= xBoundR) {
            lastR = Mathf.Min(pos.x, lastR);
            return true;
        }
        else if(pos.x <= xBoundL) {
            lastL = Mathf.Max(pos.x, lastL);
            return true;
        }
        return false;
    }

    //half of what the camera sees vertically
    public float GetBound() {
        return camc.orthographicSize * Screen.width / Screen.height;
    }

    private void placeBlock(BlockSave bs) {
        //bs.type.init(bs.x, bs.type.onFloor ? floorY + FLOOR_HEIGHT / 2 + bs.type.height / 2 : bs.y, bs.ctype);
        if(bs.type.hasObject) {
            GameObject newo = Instantiate(lblock, new Vector3(bs.x, bs.type.onFloor ? (GetFloorY(bs.x) + FLOOR_HEIGHT / 2 + bs.type.height / 2) : bs.y, bs.type.zLayer), bs.type.rotate ? Block.rotation[bs.ctype % 4] : Quaternion.identity);
            
            LBlockUpdater bu = newo.GetComponent<LBlockUpdater>();
            bu.SetBlock(bs.type, bs.ctype);
            bu.save = bs;
        }
        else if(bs.type.sprite != null) {
            GameObject newo = Instantiate(ltag, new Vector3(bs.x, bs.type.onFloor ? (GetFloorY(bs.x) + FLOOR_HEIGHT / 2 + bs.type.height / 2) : bs.y, -5), bs.type.rotate ? Block.rotation[bs.ctype % 4] : Quaternion.identity);

            LBlockUpdater bu = newo.GetComponent<LBlockUpdater>();
            bu.SetBlock(bs.type, bs.ctype);
            bu.save = bs;
        }
    }

    //todo
    public float GetFloorY(float x) {
        return 0f;
    }

    public bool GetPit(float x) {
        return false;
    }

    public void ResetFloorFlags() {
        
    }

    public void RecacheBlocks() {
        GameObject[] all = GameObject.FindGameObjectsWithTag("LevelBlock");
        GameObject[] tags = GameObject.FindGameObjectsWithTag("LevelTag");
        loadList.Clear();
        foreach(GameObject o in all) {
            loadList.Add(o.GetComponent<LBlockUpdater>().save);
        }
        foreach(GameObject o in tags) {
            loadList.Add(o.GetComponent<LTagUpdater>().save);
        }

        loadList.Sort((x, y) => x.x.CompareTo(y.x));
    }

    public Level GetLevel() {
        if(level == null) level = new Level(loadList);
        return level;
    }

    public void PlayLevel() {
        RecacheBlocks();
        Vars.main.tempBlockSaves = GetLevel();
        LevelLoader.LoadRun(level);
    }

    private void loadTestLevel() {
        //todo remove
        if(loadList.Count > 0) return;
        loadList.Add(new BlockSave(Blocks.jumpOrb, 22f, 5.2f, 0));
        loadList.Add(new BlockSave(Blocks.spike, 30f, 0f, 0));
        loadList.Add(new BlockSave(Blocks.spike, 32f, 0f, 0));
        loadList.Add(new BlockSave(Blocks.pitStart, 60f, 0f, 0));
        loadList.Add(new BlockSave(Blocks.pitEnd, 61f, 0f, 0));
        loadList.Add(new BlockSave(Blocks.spike, 80f, 0f, 0));
        loadList.Add(new BlockSave(Blocks.spike, 81f, 0f, 0));
        loadList.Add(new BlockSave(Blocks.potion, 81f, 5f, 0));
        loadList.Add(new BlockSave(Blocks.spike, 82f, 0f, 0));
        loadList.Add(new BlockSave(Blocks.pitStart, 90f, 0f, 0));
        loadList.Add(new BlockSave(Blocks.balloon, 95f, 2.5f, 0));
        loadList.Add(new BlockSave(Blocks.balloon, 100f, 2.5f, 0));
        loadList.Add(new BlockSave(Blocks.balloonHigh, 105f, 2.5f, 2));
        loadList.Add(new BlockSave(Blocks.pitEnd, 110f, 0f, 0));
    }
    private void resetLoadList() {
        Debug.Log("loadList Count:" + loadList.Count);
        if(Vars.main.tempBlockSaves != null) {
            loadList = Vars.main.tempBlockSaves.blocks;
            level = Vars.main.tempBlockSaves;
            Vars.main.tempBlockSaves = null;
        }
        else loadList.Clear();
        listSize = -1;
        while(lUnload.Count > 0) lUnload.Dequeue();
        while(rUnload.Count > 0) rUnload.Dequeue();
    }
}
