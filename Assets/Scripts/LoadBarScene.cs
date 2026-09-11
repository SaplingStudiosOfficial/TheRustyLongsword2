using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBarScene : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] public PlayerController Player;

    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoadBar()
    {
        SceneManager.LoadScene("Crybt");
    }
}
