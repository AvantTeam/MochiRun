using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    void Start()
    {
        Vars.main.Load();

        SceneManager.LoadScene(Vars.main.firstScene);
    }
}
