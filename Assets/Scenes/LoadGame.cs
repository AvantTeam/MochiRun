using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public string firstScene;
    void Start()
    {
        Vars.main.Load();

        SceneManager.LoadScene(firstScene);
    }
}
