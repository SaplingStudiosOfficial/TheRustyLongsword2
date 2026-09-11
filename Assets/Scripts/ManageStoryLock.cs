using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageStoryLock : MonoBehaviour
{
    [SerializeField] private GameObject unlocked;
    [SerializeField] private PlayerController player;
    [SerializeField] private int storyPoint; 
    void Start()
    {
        if(player.night >= storyPoint)
        {
            unlocked.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
    void Update()
    {
        if (player.night >= storyPoint)
        {
            unlocked.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}
