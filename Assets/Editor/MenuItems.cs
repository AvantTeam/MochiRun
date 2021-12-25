using UnityEngine;
using UnityEditor;

public class MenuItems {
    [MenuItem("Tools/Pre Build")]
    public static void PreBuild() {
        Debug.LogWarning("----Starting Prebuild----");
        SetContentList();
        ListBlocks();
        AssetPreviewPack();
        Debug.LogWarning("----Prebuild complete!----");
    }

    [MenuItem("Tools/Init ContentList")]
    private static void SetContentList() {
        initBlocks();
    }

    private static void initBlocks() {
        Debug.Log("INITIALIZING BLOCK LIST");
        string[] guids = AssetDatabase.FindAssets("t:Block", new[] { "Assets/World/Blocks" });
        Block[] blocks = new Block[guids.Length];
        for(int i = 0; i < guids.Length; i++) {
            blocks[i] = (Block)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), typeof(Block));
            blocks[i].id = i;
            Debug.Log(blocks[i].name);
        }

        ((ContentList)AssetDatabase.LoadAssetAtPath("Assets/ContentList.asset", typeof(ContentList))).blocks = blocks;
    }

    [MenuItem("Tools/List Block Prefabs")]
    private static void ListBlocks() {
        Debug.Log("LISTING BLOCK PREFABS");
        string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] {"Assets/World/Blocks"});
        GameObject[] blocks = new GameObject[guids.Length];
        for(int i = 0; i < guids.Length; i++) {
            Debug.Log(AssetDatabase.GUIDToAssetPath(guids[i]));
            blocks[i] = (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), typeof(GameObject));
        }

        BlockPrefabCollector.list = blocks;
        BlockPrefabCollector.icons = new Texture2D[guids.Length];
    }

    [MenuItem("Tools/Pack Block Previews")]
    private static void AssetPreviewPack() {
        Debug.Log("PACKING BLOCK PREVIEWS");
        if(BlockPrefabCollector.list == null) {
            Debug.LogError("Error: List block prefabs first!");
            return;
        }
        if(AssetPreview.IsLoadingAssetPreviews()) {
            Debug.LogError("Error: Previews have not yet been loaded. Please try again later.");
            return;
        }

        for(int i = 0; i < BlockPrefabCollector.icons.Length; i++) {
            string spriteName = BlockPrefabCollector.list[i].name;
            string path = "Assets/MatSprites/Gen/" + spriteName + ".png";
            Texture2D tex = AssetPreview.GetAssetPreview(BlockPrefabCollector.list[i]);
            //todo remove background and things
            byte[] bytes = tex.EncodeToPNG();
            // For testing purposes, also write to a file in the project folder
            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.mipmapEnabled = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.filterMode = FilterMode.Point;
            AssetDatabase.WriteImportSettingsIfDirty(path);

            Texture2D assetTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MatSprites/Gen/" + spriteName + ".png", typeof(Texture2D));
            BlockPrefabCollector.icons[i] = assetTex;

            //now, if a Block exists, set its sprite
            Object asset = AssetDatabase.LoadAssetAtPath("Assets/World/Blocks/" + spriteName + ".asset", typeof(Block));
            if(asset != null) {
                Block block = (Block)asset;
                block.sprite = assetTex;
            }
        }
    }

    [MenuItem("Tools/Personal Sh1p Chan")]
    private static void ShipChan() {
    }
}