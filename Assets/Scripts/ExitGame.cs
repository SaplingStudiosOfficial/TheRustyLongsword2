using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGame : MonoBehaviour
{
    public bool autoExit;

    // Start is called before the first frame update
    void Start()
    {
        if (autoExit) {
            StartCoroutine(WaitThenRecap());
        }
    }
    public void exitGame()
    {
         Application.Quit();
    }
    public void load3DWorld()
    {
        SceneManager.LoadScene("3DEnviroment");
    }
    public void loadRecap()
    {
        SceneManager.LoadScene("NightRecap");
    }

    public IEnumerator WaitThenRecap()
    {

        yield return new WaitForSeconds(2f);
        loadRecap();
    }
}
