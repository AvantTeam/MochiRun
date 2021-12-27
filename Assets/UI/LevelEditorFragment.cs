using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorFragment : MonoBehaviour
{
    private const float BP_CELL = 90f;
    //elements
    public GameObject blockSelectPane, categoryButton, categoryPane, categoryPaneMobile, categoryViewMobile, menuButton;
    public LayoutElement blockScrollView;
    public GridRenderer grid;
    public Image[] categoryThemed;
    public Category[] categories;
    //prefabs
    public GameObject blockSelectButton, categorySelectButton;

    public Category category;
    private GameObject currentCategoryPane, currentCategoryView; //view is a parent of/equal to pane. View should be (de)activated, pane should hold the buttons.
    private bool catPaneOpen;
    //settings region; todo save to settings
    public BlockPaneConfig BPConfig = BlockPaneConfig.desktop;
    public HashSet<Block> favorites = new HashSet<Block>();

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
        if(category == null) category = Vars.main.content.defaultCategory;
        clearChildren(blockSelectPane);
        clearChildren(categoryPane);
        clearChildren(categoryPaneMobile);
        categoryPane.SetActive(false);
        categoryViewMobile.SetActive(false);
        catPaneOpen = false;
        setBSPaneSize(BPConfig.rows, BPConfig.columns);

        foreach(Category anuke in categories) {
            GameObject bb = Instantiate(categorySelectButton, Vector3.zero, Quaternion.identity);
            bb.transform.SetParent(currentCategoryPane.transform);
            bb.GetComponent<CategorySelectButton>().SetCategory(anuke);
        }

        SetCategory(category);
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
            currentCategoryPane = categoryPaneMobile;
            currentCategoryView = categoryViewMobile;
        }
        else {
            sr.horizontal = false;
            sr.vertical = true;
            gl.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gl.constraintCount = w;
            GridLayoutGroup gl2 = categoryPane.GetComponent<GridLayoutGroup>();
            gl2.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gl2.constraintCount = w;
            currentCategoryPane = currentCategoryView = categoryPane;
        }
    }

    private void rebuildBlocks() {
        clearChildren(blockSelectPane);
        foreach(Block block in Vars.main.content.blocks) {
            if(block.sprite == null || block.hidden || !isCategory(block)) continue;
            GameObject bb = Instantiate(blockSelectButton, Vector3.zero, Quaternion.identity);
            bb.transform.SetParent(blockSelectPane.transform);
            bb.GetComponent<BlockSelectButton>().SetBlock(block);
        }
    }

    Texture2D duplicateTexture(Texture2D source) {
        byte[] pix = source.GetRawTextureData();
        Texture2D readableText = new Texture2D(source.width, source.height, source.format, false);
        readableText.LoadRawTextureData(pix);
        readableText.Apply();
        return readableText;
    }

    public void SetCategory(Category cat) {
        //hide category select bit
        //currentCategoryView.SetActive(false);
        SetActiveFold(currentCategoryView, false);
        catPaneOpen = false;
        category = cat;
        categoryButton.GetComponent<CategoryButton>().SetCategory(cat);

        foreach(Image im in categoryThemed) {
            im.color = cat.color;
        }
        rebuildBlocks();
    }

    public bool OpenCategories() {
        catPaneOpen = !catPaneOpen;
        //currentCategoryView.SetActive(catPaneOpen);
        SetActiveFold(currentCategoryView, catPaneOpen);
        return catPaneOpen;
    }

    private bool isCategory(Block b) {
        if(category.special){
            if(category.blackIcon) return true; //all
            return favorites.Contains(b); //favorites
        }
        return b.category == category;
    }

    void SetActiveFold(GameObject view, bool active) {
        StopAllCoroutines();
        if(active) {
            StartCoroutine(FoldInRect(view.GetComponent<RectTransform>()));
        }
        else {
            StartCoroutine(FoldOutRect(view.GetComponent<RectTransform>()));
        }
    }

    IEnumerator FoldInRect(RectTransform rect) {
        if(!rect.gameObject.activeInHierarchy) rect.localScale = new Vector3(0f, rect.localScale.y, rect.localScale.z);
        rect.gameObject.SetActive(true);

        float current = rect.localScale.x;
        for(int i = 0; i <= 15; i++) {
            if(!rect.gameObject.activeInHierarchy) yield break;
            rect.localScale = new Vector3(Mathf.Lerp(current, rect.localScale.y, i / 15f), rect.localScale.y, rect.localScale.z);
            yield return null;
        }

        rect.localScale = new Vector3(rect.localScale.y, rect.localScale.y, rect.localScale.z);
    }

    IEnumerator FoldOutRect(RectTransform rect) {
        float current = rect.localScale.x;
        for(int i = 0; i <= 15; i++) {
            if(!rect.gameObject.activeInHierarchy) yield break;
            rect.localScale = new Vector3(Mathf.Lerp(current, 0f, i / 15f), rect.localScale.y, rect.localScale.z);
            yield return null;
        }

        rect.localScale = new Vector3(0f, rect.localScale.y, rect.localScale.z);
        rect.gameObject.SetActive(false);
    }
}
