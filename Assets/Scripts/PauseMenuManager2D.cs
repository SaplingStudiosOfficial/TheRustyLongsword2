using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager2D : MonoBehaviour
{

    [SerializeField] GameObject PauseMenu;
    private bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        PauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            isPaused = true;
            PauseMenu.SetActive(true);
            
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            isPaused = false;
            PauseMenu.SetActive(false);
            
        }
    }
}
