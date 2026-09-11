using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfTutorialHasBeenDone : MonoBehaviour
{
    public PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.tutorialDone == true)
        {
            gameObject.SetActive(false);
        }
    }
}
