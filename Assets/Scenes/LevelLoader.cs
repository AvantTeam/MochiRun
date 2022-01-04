using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader main;

    public Level loading = null;
    public string prevScene;
    
    private void Awake() {
        Debug.Log("New LevelLoader awake!");
        if(main != null) Destroy(main.gameObject);
        main = this;
        DontDestroyOnLoad(this);
    }

    public static void LoadRun(Level level) { 
        new GameObject("LevelLoader", typeof(LevelLoader));
        Debug.Log("Set Level!");
        main.loading = level;
        main.prevScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("RunScene");
        Debug.Log("Loaded Scene!");
    }

    //called after the death/
    public static void End() {
        SceneManager.LoadScene(main.prevScene);
        Destroy(main.gameObject);
    }

    public static bool IsEditor() {
        return main.prevScene != null && main.prevScene.Equals("LevelEditScene");
    }
}