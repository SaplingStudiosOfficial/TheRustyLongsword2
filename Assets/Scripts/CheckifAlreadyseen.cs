using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckifAlreadyseen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SaveSystem.LoadData().hasSeenCutscene == true)
        {
            StartCoroutine(WaitThenLeave());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator WaitThenLeave()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("3DEnviroment");
    }
}
