using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnIfT1H : MonoBehaviour
{
    public PlayerController player;
    public GameObject flySpawner;
    // Start is called before the first frame update

    
    void Update()
    {
        if (player.T1HDone)
        {
            flySpawner.SetActive(true);
        }
    }
}
