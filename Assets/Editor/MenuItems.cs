using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class MenuItems {
    public const string initScene = "LoadScene";
    public static string defaltFirstScene = "RunScene";
    public static string lastEditScene = "RunScene";
    public static bool loadSceneMode = false;

    [MenuItem("Tools/Play Scene", false, 1)]
    public static void PlayCurrentScene() {
        PrePlayScene();
        EditorApplication.EnterPlaymode();
    }

    [MenuItem("Tools/Play Scene", true)]
    public static bool ValidatePS() {
        return !SceneManager.GetActiveScene().name.Equals(initScene);
    }

    public static string PrePlayScene() {
        string current = SceneManager.GetActiveScene().name;
        if(!current.Equals(initScene)) {
            loadSceneMode = true;
            lastEditScene = current;
            Debug.Log("Playing: " + current);
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            //EditorSceneManager.OpenScene("Assets/Scenes/" + initScene + ".unity");
            Vars varsPrefab = VarsPrefab();
            defaltFirstScene = varsPrefab.firstScene;
            varsPrefab.firstScene = current;
            return defaltFirstScene;
        }
        else {
            loadSceneMode = false;
            defaltFirstScene = VarsPrefab().firstScene;
            return defaltFirstScene;
        }
    }

    public static Vars VarsPrefab() {
        return ((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Vars.prefab", typeof(GameObject))).GetComponent<Vars>();
    }

    [MenuItem("Tools/Pre Build", false, 20)]
    public static void PreBuild() {
        Debug.LogWarning("----Starting Prebuild----");
        SetContentList();
        ListBlocks();
        AssetPreviewPack();
        Debug.LogWarning("----Prebuild complete!----");
    }

    [MenuItem("Tools/Init ContentList", false, 21)]
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

        ContentList c = (ContentList)AssetDatabase.LoadAssetAtPath("Assets/ContentList.asset", typeof(ContentList));
        c.blocks = blocks;
        EditorUtility.SetDirty(c);
    }

    [MenuItem("Tools/List Block Prefabs", false, 22)]
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

    [MenuItem("Tools/Pack Block Previews", false, 23)]
    private static void AssetPreviewPack() {
        Debug.Log("PACKING BLOCK PREVIEWS");
        AssetPreview.SetPreviewTextureCacheSize(100);
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
            if(spriteName.StartsWith("Trigger")) continue;
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

            Sprite assetTex = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/MatSprites/Gen/" + spriteName + ".png", typeof(Sprite));
            BlockPrefabCollector.icons[i] = assetTex.texture;

            //now, if a Block exists, set its sprite
            Object asset = AssetDatabase.LoadAssetAtPath("Assets/World/Blocks/" + spriteName + ".asset", typeof(Block));
            if(asset != null) {
                Block block = (Block)asset;
                block.sprite = assetTex;
                EditorUtility.SetDirty(block);
            }
        }
    }

    [MenuItem("Tools/Personal Sh1p Chan", false, 100)]
    private static void ShipChan() {
        Debug.Log(EditorSceneManager.playModeStartScene);
    }
}