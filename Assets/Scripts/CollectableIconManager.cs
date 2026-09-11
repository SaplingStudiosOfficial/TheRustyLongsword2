using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectableIconManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Collected;
    public PlayerController player;
    [SerializeField] private int village;
    int collectedNumber;

    // Update is called once per frame
    void Update()
    {
        if (village == 1)
        {
            collectedNumber = 0;
            for(int i = 0; i < player.VillageOneCollectableInfo.Length; i++)
            {
                if (player.VillageOneCollectableInfo[i] == 1)
                {
                    collectedNumber++;
                }
            }
            Collected.text = collectedNumber.ToString() + "/46";
        }else if(village == 2)
        {
            collectedNumber = 0;
            for (int i = 0; i < player.VillageTwoCollectableInfo.Length; i++)
            {
                if (player.VillageTwoCollectableInfo[i] == 1)
                {
                    collectedNumber++;
                }
            }
            Collected.text = collectedNumber.ToString() + "/48";
        }
        else if (village == 3)
        {
            collectedNumber = 0;
            for (int i = 0; i < player.VillageThreeCollectableInfo.Length; i++)
            {
                if (player.VillageThreeCollectableInfo[i] == 1)
                {
                    collectedNumber++;
                }
            }
            Collected.text = collectedNumber.ToString() + "/48";
        }
    }
}
