using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using static MenuItems;

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
}
