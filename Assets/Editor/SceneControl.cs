using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using static MenuItems;
using UnityEngine.SceneManagement;

[InitializeOnLoadAttribute]
public static class SceneControl
{
    static SceneControl() {
        EditorApplication.playModeStateChanged += PlayModeState;
        EditorSceneManager.playModeStartScene = (SceneAsset)AssetDatabase.LoadAssetAtPath("Assets/Scenes/" + initScene + ".unity", typeof(SceneAsset));
    }

    private static void PlayModeState(PlayModeStateChange state) {
        //Debug.Log(state);

        switch(state) {
            case PlayModeStateChange.EnteredEditMode:
                if(loadSceneMode) {
                    Debug.Log("Stopped Scene Play Mode: " + lastEditScene);
                    Vars vp = VarsPrefab();
                    //revert the tampered vars
                    vp.firstScene = defaltFirstScene;
                }
                loadSceneMode = false;

                
                break;
        }
    }

    /*
    public static string editorScene() {
        return VarsEditor.main == null ? "" : VarsEditor.main.editorScene;
    }

    [MenuItem("Tools/Play", false, 2)]
    public static void PlayStart() {
        string current = SceneManager.GetActiveScene().name;
        Debug.Log("Set current to:" + current);
        

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/LoadScene.unity");
        VarsEditorPrefab().editorScene = current;
        EditorApplication.EnterPlaymode();    }

    public static VarsEditor VarsEditorPrefab() {
        GameObject o = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Editor/VarsEditor.prefab", typeof(GameObject));
        GameObject p = Object.Instantiate(o);
        return p.GetComponent<VarsEditor>();
    }*/
}
