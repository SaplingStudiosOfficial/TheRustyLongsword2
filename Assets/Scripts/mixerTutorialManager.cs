using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mixerTutorialManager : MonoBehaviour
{
    [SerializeField] GameObject tutorialScreen;
    public bool show = false;

    // Start is called before the first frame update
    void Start()
    {
        if(SaveSystem.LoadData() != null)
        {
            show = SaveSystem.LoadData().mixerTutorialDone;
        }
        if (!show)
        {
            tutorialScreen.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
