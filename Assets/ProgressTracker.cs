using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressTracker : MonoBehaviour { 

    [SerializeField] private Slider progressBar;
    [SerializeField] private PlayerController Player;
    private int[] HatfieldCoinsInfo;
    private int[] EastwoodCoinsInfo;
    private int[] BoenahaCoinsInfo;
    private int[] UniqueCollectableInfo;
    private int PlayerNight;
    private int QuestInfo;
    public bool isWorldBar;
    public float progress = 0.0f;
    [SerializeField] private Text LogPercentage;
    public int logPercentage = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBar();
    }

    void UpdateBar()
    {
        if (isWorldBar)
        {
            progress = setProgressValueWorld();
        }
        else if (Player.worldMusic == 0) 
        {
            progress = setProgressValueHatfield();
        }
        else if(Player.worldMusic == 1)
        {
            progress = setProgressValueEastwood();
        }
        else if(Player.worldMusic == 2)
        {
            progress = setProgressValueBoenaha();
        }
        else
        {
            progress = 0.0f;
        }
        progressBar.value = progress;
        logPercentage = (int)(progress * 100);
        if (isWorldBar)
        {
            LogPercentage.text = "Game:" + logPercentage.ToString() + '%';
        }
        else
        {
            LogPercentage.text = "Area:" + logPercentage.ToString() + '%';
        }
    }

    float setProgressValueWorld()
    {
        // Get progress values for each area
        float hatfieldProgress = setProgressValueHatfield();
        float eastwoodProgress = setProgressValueEastwood();
        float boenahaProgress = setProgressValueBoenaha();

        // Calculate the overall progress as an average of the three areas
        float overallProgress = (hatfieldProgress + eastwoodProgress + boenahaProgress) / 3;

        return overallProgress;
    }
    float setProgressValueHatfield()
    {
        //TODO: Import Hatfield progess info then return calculated percentage of completion.
        float collected = 0.0f;
        getPlayerProgressInfo();
        float total = 50f;
        for(var x = 0; x < HatfieldCoinsInfo.Length; x++)
        {
            if (HatfieldCoinsInfo[x] == 1)
            {
                collected += 1.0f;
            }
        }
        for(int x = 0; x < 4; x++)
        {
            if(Player.UniqueCoins[x] == 1)
            {
                collected += 1.0f;
            }
        }
        return collected/total;
    }
    float setProgressValueEastwood() 
    {
        //TODO: Import Eastwood night, coin and collectable info then return calculated percentage of completion.
        float collected = 0.0f;
        getPlayerProgressInfo();
        float total = 51f;
        for (var x = 0; x < EastwoodCoinsInfo.Length; x++)
        {
            if (EastwoodCoinsInfo[x] == 1)
            {
                collected += 1.0f;
            }
        }
        for(int x = 4; x < 7; x++)
        {
            if (Player.UniqueCoins[x] == 1)
            {
                collected += 1.0f;
            }
        }
        return collected / total;
    }
    float setProgressValueBoenaha()
    {
        float collected = 0.0f;
        getPlayerProgressInfo();
        float total = 50f;
        for (var x = 0; x < BoenahaCoinsInfo.Length; x++)
        {
            if (BoenahaCoinsInfo[x] == 1)
            {
                collected += 1.0f;
            }
        }
        for (int x = 7; x < 9; x++)
        {
            if (Player.UniqueCoins[x] == 1)
            {
                collected += 1.0f;
            }
        }
        return collected / total;
    }
    void getPlayerProgressInfo()
    {
        //TODO: Import all Player status info.
        HatfieldCoinsInfo = Player.VillageOneCollectableInfo;
        EastwoodCoinsInfo = Player.VillageTwoCollectableInfo;
        BoenahaCoinsInfo = Player.VillageThreeCollectableInfo;
        PlayerNight = Player.night;
        UniqueCollectableInfo = Player.UniqueCoins;
    }

}
