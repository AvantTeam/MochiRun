using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public const float FLOOR_WIDTH = 4f, FLOOR_HEIGHT = 6f;
    public const float CAM_CLIP = 10.5f;

    public static float lastX = -20f;
    public static float floorY = 0f;
    public static bool isPit = false;
    private byte lastFloorType = 0;

    public GameObject floorPrefab;
    GameObject cam;
    Block floorBlock, pitStart, pitEnd;
    List<BlockSave> loadList = new List<BlockSave>(); //must be sorted by x!
    private int listIndex, listSize;

    private struct BlockSave {
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
    
    // Start is called before the first frame update
    void Start() {
        loadBlocks();
        cam = GameObject.Find("Main Camera");
        isPit = false;
        resetLoadList();

        //TODO remove
        loadList.Add(new BlockSave(pitStart, 60f, 0f, 0));
        loadList.Add(new BlockSave(pitEnd, 61f, 0f, 0));
    }

    // Update is called once per frame
    void Update() {
        placeFloors();
        placeBlocks();
    }

    private void placeFloors() {
        if(isPit) return;
        float xBound = cam.transform.position.x + CAM_CLIP / cam.GetComponent<CameraController>().zoom + FLOOR_WIDTH;
        while(lastX < xBound) {
            floorBlock.init(lastX, floorY, (byte)(++lastFloorType % 2));
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
        Debug.Log(bs.x);
        bs.type.init(bs.x, bs.type.onFloor ? floorY + FLOOR_HEIGHT / 2 + bs.type.height / 2 : bs.y, bs.ctype);
    }

    private void loadBlocks() {
        floorBlock = new Floor(floorPrefab);
        pitStart = new PitStarter();
        pitEnd = new PitEnder();
    }

    private void resetLoadList() {
        loadList.Clear();
        listIndex = 0;
        listSize = -1;
    }
}
