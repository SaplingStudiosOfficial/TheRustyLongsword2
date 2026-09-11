using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveManagerScript
{
    public int amtPine;
    public int amtFlies;
    public bool tutorialDone;
    public bool mixerTutorialDone;
    public bool hasSeenCutscene;
    public int night;
    public List<string> invitedCharacters = new List<string>();
    public float posX;
    public float posY;
    public float posZ;
    public bool T1HDone;
    public bool T2HDone;
    public bool T1MDone;
    public bool T2MDone;
    public bool T3Done;
    public bool ItachiEndingOne;
    public bool ItachiEndingTwo;
    public bool AnkokuEndingOne;
    public bool AnkokuEndingTwo;
    public bool WinstonEndingOne;
    public bool WinstonEndingTwo;
    public bool JesseEndingOne;
    public bool JesseEndingTwo;
    public bool DuckIn;
    public int VillageTwoCollectable;
    public int VillageThreeCollectable;
    public int[] VillageOneCollectableInfo = new int[50];
    public int[] VillageTwoCollectableInfo = new int[50];
    public int[] VillageThreeCollectableInfo = new int[50];
    public int[] UniqueCoins = new int[11];
    public int VillageOneCollectables;

    //Character States
    public string HensonState;
    public string MayorKState;
    public string FarmerKl3pState;
    public string JefferyState;
    public string SmithState;
    public string DuckState;
    public string MrSmithState;
    public bool enterBarTutorialDone;
    public int worldMusic;
    public string ItachiState;
    public string AnkokuState;
    public string SukiState;
    public string WinstonState;
    public string JesseState;
    public string JessyState;
    public string SherriffState;
    public float VolumeValue;
    public float mouseSensitivity;

    //High Scores

    public SaveManagerScript(PlayerController player)
    {
        WinstonState = player.WinstonState;
        JesseState = player.JesseState;
        JessyState = player.JessyState;
        SherriffState = player.SherriffState;
        HensonState = player.HensonState;
        MayorKState = player.MayorKState;
        AnkokuState = player.AnkokuState;
        SukiState = player.SukiState;
        FarmerKl3pState = player.FarmerKl3pState;
        JefferyState = player.JefferyState;
        SmithState = player.SmithState;
        DuckState = player.DuckState;
        MrSmithState = player.MrSmithState;
        amtPine = player.amtPine;
        amtFlies = player.amtFly;
        tutorialDone = player.tutorialDone;
        mixerTutorialDone = player.mixerTutorialDone;
        hasSeenCutscene = player.hasSeenCutscene;
        night = player.night;
        posX = player.transform.position.x;
        posY = player.transform.position.y;
        posZ = player.transform.position.z;
        invitedCharacters = new List<string>(player.invitedCharacters);
        T1HDone = player.T1HDone;
        T2HDone = player.T2HDone;
        T1MDone = player.T1MDone;
        T2MDone = player.T2MDone;
        T3Done = player.T3Done;
        ItachiEndingOne = player.ItachiEndingOne;
        ItachiEndingTwo = player.ItachiEndingTwo;
        AnkokuEndingOne = player.AnkokuEndingOne;
        AnkokuEndingTwo = player.AnkokuEndingTwo;
        JesseEndingOne = player.JesseEndingOne;
        JesseEndingTwo = player.JesseEndingTwo;
        WinstonEndingOne = player.WinstonEndingOne;
        WinstonEndingTwo = player.WinstonEndingTwo;
        DuckIn = player.DuckIn;
        worldMusic = player.worldMusic;
        enterBarTutorialDone = player.enterBarTutorialDone;
        ItachiState = player.ItachiState;
        VolumeValue = player.VolumeValue;
        mouseSensitivity = player.mouseSensitivity;

        for (int i = 0; i < 50; i++)
        {
            VillageOneCollectableInfo[i] = player.VillageOneCollectableInfo[i];
        }
        for(int i = 0; i < UniqueCoins.Length; i++)
        {
            UniqueCoins[i] = player.UniqueCoins[i];
        }
        for (int i = 0; i < 50; i++)
        {
            VillageTwoCollectableInfo[i] = player.VillageTwoCollectableInfo[i];
        }
        for (int i = 0; i < 50; i++)
        {
            VillageThreeCollectableInfo[i] = player.VillageThreeCollectableInfo[i];
        }
        VillageOneCollectables = player.VillageOneCollectable;
        VillageTwoCollectable = player.VillageTwoCollectable;
        VillageThreeCollectable = player.VillageThreeCollectable;
    }
}
