using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public const float FLOOR_WIDTH = 4f, FLOOR_HEIGHT = 6f;
    public const float CAM_CLIP = 7.5f;

    public static float lastX = -20f;
    public static float floorY = 0f;
    public static bool isPit = false;
    private byte lastFloorType = 0;

    public GameObject floorPrefab;
    public GameObject[] blockPrefabs;
    public static GameObject cam;
    public static PlayerControl pcon;
    //Block floorBlock, pitStart, pitEnd, spike, potion, jumpOrb, balloon, balloonLow, balloonHigh;
    private List<BlockSave> loadList; //must be sorted by x!
    private int listIndex, listSize;

    public struct IslandBackground {
        public bool islands;
        public float islandChance;
        public float islandZ, islandZRand;

        public static IslandBackground none = new IslandBackground(false, 0, 0, 0), islandMany = new IslandBackground(true, 0.02f, 18f, 5f);

        public IslandBackground(bool islands, float islandChance, float islandZ, float islandZRand) {
            this.islands = islands;
            this.islandChance = islandChance;
            this.islandZ = islandZ;
            this.islandZRand = islandZRand;
        }
    }

    //todo should be set by LevelLoader
    public IslandBackground islandData;
    public GameObject islandPrefab;
    private float lastIslandX;
    private bool prePlaced = false;

    public struct BlockSave {
        public Block type;
        public byte ctype;
        public float x;
        public float y;

        public BlockSave(Block b, float x, float y, byte ctype) {
            type = b;
            this.x = x;
            this.y = y;
            this.ctype = ctype;
        }
    }
    
    void Start() {
        //loadBlocks();
        Debug.Log("Starting ChunkLoader!");
        cam = GameObject.Find("Main Camera");
        pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        reset();

        if(LevelLoader.main == null || LevelLoader.main.loading == null) {
            if(LevelLoader.main == null) Debug.LogError("ChunkLoader: LevelLoader missing!");
            else Debug.LogError("ChunkLoader: Loading level data missing!");
            loadTestLevel();
            pcon.Apply(new Level());
        }
        else {
            Level level = LevelLoader.main.loading;
            islandData = level.islands;
            loadList = level.blocks;
            pcon.Apply(level);
        }
    }

    void Update() {
        placeFloors();
        placeBlocks();
        //placeBackground();
    }

    private void reset() {
        lastIslandX = 0f;
        lastX = -20f;
        floorY = 0f;
        isPit = prePlaced = false;
        resetLoadList();
    }

    private void placeFloors() {
        if(isPit) return;
        float xBound = cam.transform.position.x + CAM_CLIP / cam.GetComponent<CameraController>().zoom + FLOOR_WIDTH;
        while(lastX < xBound) {
            Blocks.floorBlock.init(lastX, floorY, (byte)(++lastFloorType % 2));
            lastX += FLOOR_WIDTH;
        }
    }

    private void placeBlocks() {
        if(listSize == -1) listSize = loadList.Count;
        float xBound = cam.transform.position.x + CAM_CLIP / cam.GetComponent<CameraController>().zoom;

        while(listIndex < listSize && loadList[listIndex].x < xBound + loadList[listIndex].type.clipsize) {
            placeBlock(loadList[listIndex]);
            listIndex++;
        }
    }

    private void placeBlock(BlockSave bs) {
        bs.type.init(bs.x, bs.type.onFloor ? floorY + FLOOR_HEIGHT / 2 + bs.type.height / 2 : bs.y, bs.ctype);
    }

    private void placeBackground() {
        if(islandData.islands) {
            if(!prePlaced) prePlaceIslands();
            if(cam.transform.position.x > 0.5f + lastIslandX && Random.Range(0f, 1f) <= islandData.islandChance * Time.deltaTime * 60f) {
                float z = islandData.islandZ + Random.Range(-islandData.islandZRand, islandData.islandZRand);
                float xBound = cam.transform.position.x + DistancedCamClip(z) + 2f;
                lastIslandX = cam.transform.position.x;
                Instantiate(islandPrefab, new Vector3(xBound, Random.Range(-2f, -1f), z), Quaternion.identity);
            }
        }
    }

    //only call after IslandBackground is properly set
    private void prePlaceIslands() {
        float xLeft = cam.transform.position.x - DistancedCamClip(islandData.islandZ) - 2f;
        float xRight = cam.transform.position.x + DistancedCamClip(islandData.islandZ) + 2f;
        lastIslandX = cam.transform.position.x;
        int n = (int)(islandData.islandChance * 320);

        for(int i = 0; i < n; i++) {
            float z = islandData.islandZ + Random.Range(-islandData.islandZRand, islandData.islandZRand);
            Instantiate(islandPrefab, new Vector3(Random.Range(xLeft, xRight), Random.Range(-2f, -1f), z), Quaternion.identity);
        }
        prePlaced = true;
    }

    /*private void loadBlocks() {
        floorBlock = new Floor(floorPrefab);
        pitStart = new PitStarter();
        pitEnd = new PitEnder();
        spike = new Block(find("Spike")) {
            onFloor = true
        };
        potion = new Block(find("Potion"));
        jumpOrb = new Balloon(find("JumpOrb")) {
            zLayer = 0.1f
        };
        balloon = new Balloon(find("JumpBalloon"));
        balloonLow = new Balloon(find("JumpBalloonLow"));
        balloonHigh = new Balloon(find("JumpBalloonHigh"));
    }*/

    private void resetLoadList() {
        if(loadList != null) loadList.Clear();
        listIndex = 0;
        listSize = -1;
    }

    private GameObject find(string prefab) {
        foreach(GameObject o in blockPrefabs) {
            if(o.name == prefab) return o;
        }
        return null;
    }

    public static float DistancedCamClip(float z) {
        return CAM_CLIP / (cam.GetComponent<CameraController>().zoom * (CameraController.CAMSCALE / (CameraController.CAMSCALE + z)));
    }

    private void loadTestLevel() {
        if(loadList == null) loadList = new List<BlockSave>();
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
        islandData = IslandBackground.islandMany;
    }
}
