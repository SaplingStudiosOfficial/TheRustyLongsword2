using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TutorialTextManager : MonoBehaviour
{
    [SerializeField] private GameObject PopUp;
    public bool canShow = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider collider)
    {
        
        if (SaveSystem.LoadData() != null)
        {
            canShow = SaveSystem.LoadData().tutorialDone;
        }

        if (collider.tag == "Player" && !canShow)
        {
            PopUp.SetActive(true);
        }
    }
}
