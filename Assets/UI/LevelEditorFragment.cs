using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorFragment : MonoBehaviour
{
    private const float BP_CELL = 90f;
    //elementss
    public GameObject blockSelectPane, categoryButton;
    public LayoutElement blockScrollView;
    //prefabs
    public GameObject blockSelectButton;

    public BlockPaneConfig BPConfig = BlockPaneConfig.mindustry;

    public struct BlockPaneConfig {
        public static BlockPaneConfig mobile = new BlockPaneConfig("Mobile", 8, 1), desktop = new BlockPaneConfig("Desktop", 5, 2), mindustry = new BlockPaneConfig("Cat", 4, 5);
        public string name;
        public int rows;
        public int columns;

        public BlockPaneConfig(string name, int rows, int columns) {
            this.name = name;
            this.rows = rows;
            this.columns = columns;
        }
    }

    public void Load() {
        loadBlockSelect();

        //Texture2D yes = (Texture2D)categoryButton.GetComponent<Image>().mainTexture;
        //byte[] bytes = duplicateTexture(yes).EncodeToPNG();
        //System.IO.File.WriteAllBytes("Assets/defaultIFB.png", bytes);
    }

    private void loadBlockSelect() {
        clearChildren(blockSelectPane);
        setBSPaneSize(BPConfig.rows, BPConfig.columns);

        foreach(Block block in Block.blocks) {
            if(!block.hasObject || block.hidden) continue; //todo blocks with "sprites"
            GameObject bb = Instantiate(blockSelectButton, Vector3.zero, Quaternion.identity);
            bb.transform.SetParent(blockSelectPane.transform);
            bb.GetComponent<BlockSelectButton>().SetBlock(block);
        }

        //todo remove
        foreach(Block block in Block.blocks) {
            if(!block.hasObject || block.hidden) continue; //todo blocks with "sprites"
            GameObject bb = Instantiate(blockSelectButton, Vector3.zero, Quaternion.identity);
            bb.transform.SetParent(blockSelectPane.transform);
            bb.GetComponent<BlockSelectButton>().SetBlock(block);
        }
        foreach(Block block in Block.blocks) {
            if(!block.hasObject || block.hidden) continue; //todo blocks with "sprites"
            GameObject bb = Instantiate(blockSelectButton, Vector3.zero, Quaternion.identity);
            bb.transform.SetParent(blockSelectPane.transform);
            bb.GetComponent<BlockSelectButton>().SetBlock(block);
        }
        foreach(Block block in Block.blocks) {
            if(!block.hasObject || block.hidden) continue; //todo blocks with "sprites"
            GameObject bb = Instantiate(blockSelectButton, Vector3.zero, Quaternion.identity);
            bb.transform.SetParent(blockSelectPane.transform);
            bb.GetComponent<BlockSelectButton>().SetBlock(block);
        }
    }

    private void clearChildren(GameObject o) {
        int n = o.transform.childCount;
        if(n <= 0) return;
        for(int i = n - 1; i >= 0; i--) {
            Destroy(o.transform.GetChild(i).gameObject);
        }
    }

    private void setBSPaneSize(int w, int h) {
        blockScrollView.preferredWidth = w * BP_CELL;
        blockScrollView.preferredHeight = h * BP_CELL;
        ScrollRect sr = blockScrollView.gameObject.GetComponent<ScrollRect>();
        GridLayoutGroup gl = blockSelectPane.GetComponent<GridLayoutGroup>();
        
        if(h == 1) {
            //scroll horiz
            sr.horizontal = true;
            sr.vertical = false;
            gl.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            gl.constraintCount = 1;
        }
        else {
            sr.horizontal = false;
            sr.vertical = true;
            gl.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gl.constraintCount = w;
        }
    }

    Texture2D duplicateTexture(Texture2D source) {
        byte[] pix = source.GetRawTextureData();
        Texture2D readableText = new Texture2D(source.width, source.height, source.format, false);
        readableText.LoadRawTextureData(pix);
        readableText.Apply();
        return readableText;
    }
}
