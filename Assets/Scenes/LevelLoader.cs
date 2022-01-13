using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public const string campaignScene = "MenuMapScene", restartScene = "RestartScene";
    public static LevelLoader main;

    public Level loading = null;
    public string prevScene;
    public bool wasCurtainScene = false;
    
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
        main.wasCurtainScene = main.prevScene.Equals(restartScene);
        if(main.wasCurtainScene) main.prevScene = campaignScene; //if the previous scene is the quick restart scene, return to the map on exit

        SceneManager.LoadScene("RunScene");
        Debug.Log("Loaded Scene!");
    }

    public static bool isCampaign() {
        return main != null && main.prevScene.Equals(campaignScene);
    }

    //called after the death of the player
    public static void End() {
        SceneManager.LoadScene(isCampaign() ? restartScene : main.prevScene);
        Destroy(main.gameObject);
    }

    //called when the "Main menu" option is selected; only makes a difference in campaign mode
    public static void Quit() {
        SceneManager.LoadScene(main.prevScene);
        Destroy(main.gameObject);
    }

    public static bool IsEditor() {
        return main.prevScene != null && main.prevScene.Equals("LevelEditScene");
    }
}