using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class NPCDialogManager : MonoBehaviour
{
    [SerializeField] private GameObject talkIcon;
    [SerializeField] private GameObject TalkCamera;
    [SerializeField] private CinemachineFreeLook freelookCam;
    private bool canSpeak;
    [SerializeField] PlayerController Player;
    [SerializeField] GameObject TextBox;
    [SerializeField] GameObject DialogManager;
    [SerializeField] GameObject DialogManager2;
    [SerializeField] GameObject DialogManager3;
    [SerializeField] GameObject DialogManagerInvite;
    public string interaction;
    [SerializeField] private GameObject InviteMenu;
    public bool giveInvite;
    public bool giveQuest;
    public bool canInteract;
    [SerializeField] public Animator anim1;
    [SerializeField] public Animator anim2;
    public int CharacterIndex;
    
    

    // Start is called before the first frame update
    void Start()
    {
        canSpeak = false;
        LoadState();

    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract)
        {
            if (canSpeak && Input.GetKeyDown(KeyCode.F) && interaction == "dialog1")
            {
                
                talkIcon.SetActive(false);
                Player.isPlaying = false;
                TextBox.SetActive(true);
                DialogManager.SetActive(true);
                anim1.SetInteger("state", 0);
                anim2.SetInteger("state", 0);
            }
            if (canSpeak && Input.GetKeyDown(KeyCode.F) && interaction == "dialog2" && giveInvite)
            {
                talkIcon.SetActive(false);
                Player.isPlaying = false;
                TextBox.SetActive(true);
                DialogManagerInvite.SetActive(true);
                anim1.SetInteger("state", 0);
                anim2.SetInteger("state", 0);
            }
            if (canSpeak && Input.GetKeyDown(KeyCode.F) && interaction == "dialog2" && giveQuest)
            {

                talkIcon.SetActive(false);
                Player.isPlaying = false;
                TextBox.SetActive(true);
                DialogManager2.SetActive(true);
                anim1.SetInteger("state", 0);
                anim2.SetInteger("state", 0);
                interaction = "dialog3"; 
                if (CharacterIndex == 0)
                {
                    Player.MayorKState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 1)
                {
                    Player.HensonState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 2)
                {
                    Player.JefferyState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 3)
                {
                    Player.FarmerKl3pState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 4)
                {
                    Player.DuckState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 5)
                {
                    Player.MrSmithState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 6)
                {
                    Player.JesseState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 7)
                {
                    Player.WinstonState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 8)
                {
                    Player.JessyState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 9)
                {
                    Player.SherriffState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 11)
                {
                    Player.AnkokuState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 12)
                {
                    Player.SukiState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 10)
                {
                    Player.ItachiState = "dialog3";
                    Player.SaveData();
                }
            }
            if (canSpeak && Input.GetKeyDown(KeyCode.F) && interaction == "dialog5")
            {
                talkIcon.SetActive(false);
                Player.isPlaying = false;
                TextBox.SetActive(true);
                DialogManager3.SetActive(true);
                anim1.SetInteger("state", 0);
                anim2.SetInteger("state", 0);
            }
            if (canSpeak && interaction == "dialog4" && giveInvite)
            {
                giveInvite = false;
                talkIcon.SetActive(false);
                Player.isPlaying = false;
               
                    InviteMenu.SetActive(true);
                
                Cursor.visible = true;
                anim1.SetInteger("state", 0);
                anim2.SetInteger("state", 0);
                interaction = "dialog3";
                if (CharacterIndex == 0)
                {
                    Player.MayorKState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 1)
                {
                    Player.HensonState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 2)
                {
                    Player.JefferyState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 3)
                {
                    Player.FarmerKl3pState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 4)
                {
                    Player.DuckState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 5)
                {
                    Player.MrSmithState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 6)
                {
                    Player.JesseState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 7)
                {
                    Player.WinstonState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 8)
                {
                    Player.JessyState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 9)
                {
                    Player.SherriffState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 12)
                {
                    Player.SukiState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 11)
                {
                    Player.AnkokuState = "dialog3";
                    Player.SaveData();
                }
                if (CharacterIndex == 10)
                {
                    Player.ItachiState = "dialog3";
                    Player.SaveData();
                }
            }
            
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player" && interaction != "dialog3" && Player.isPlaying)
        {
            
            talkIcon.SetActive(true);
            TalkCamera.SetActive(true);
            freelookCam.enabled = false;
            canSpeak = true;
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            talkIcon.SetActive(false);
            TalkCamera.SetActive(false);
            freelookCam.enabled = true;
            canSpeak = false;
        }
    }
    public void FinishQuest()
    {
        interaction = "dialog2";
        giveQuest = false;
        giveInvite = true;
    }

    public void LoadState()
    {
        StartCoroutine(LoadDelayedState());
    }

    public IEnumerator LoadDelayedState()
    {

        yield return new WaitForSeconds(.5f);
        if (CharacterIndex == 0 && Player.MayorKState != "")
        {
            interaction = Player.MayorKState;
            Player.SaveData();
        }
        else if (CharacterIndex == 1 && Player.HensonState != "")
        {
            interaction = Player.HensonState;
            Player.SaveData();
        }
        else if (CharacterIndex == 2 && Player.JefferyState != "")
        {
            interaction = Player.JefferyState;
            Player.SaveData();
        }
        else if (CharacterIndex == 3 && Player.FarmerKl3pState != "")
        {
            interaction = Player.FarmerKl3pState;
            Player.SaveData();
        }
        else if (CharacterIndex == 4 && Player.DuckState != "")
        {
            interaction = Player.DuckState;
            Player.SaveData();
        }
        else if (CharacterIndex == 5 && Player.MrSmithState != "")
        {
            interaction = Player.MrSmithState;
            Player.SaveData();
        }
        else if (CharacterIndex == 6 && Player.JesseState != "")
        {
            interaction = Player.JesseState;
            Player.SaveData();
        }
        else if (CharacterIndex == 7 && Player.WinstonState != "")
        {
            interaction = Player.WinstonState;
            Player.SaveData();
        }
        else if (CharacterIndex == 8 && Player.JessyState != "")
        {
            interaction = Player.JessyState;
            Player.SaveData();
        }
        else if (CharacterIndex == 9 && Player.SherriffState != "")
        {
            interaction = Player.SherriffState;
            Player.SaveData();
        }
        else if(CharacterIndex == 10 && Player.ItachiState != "")
        {
            interaction = Player.ItachiState;
            Player.SaveData();
        }
        else if (CharacterIndex == 11 && Player.AnkokuState != "")
        {
            interaction = Player.AnkokuState;
            Player.SaveData();
        }
        else if (CharacterIndex == 12 && Player.SukiState != "")
        {
            interaction = Player.SukiState;
            Player.SaveData();
        }
        else
        {
            interaction = "dialog1";
        }
    }

    public void endTalk()
    {
        interaction = "dialog3";
        if (CharacterIndex == 0)
        {
            Player.MayorKState = "dialog3";
            Player.SaveData();
        }
        if (CharacterIndex == 1)
        {
            Player.HensonState = "dialog3";
            Player.SaveData();
        }
        if (CharacterIndex == 2)
        {
            Player.JefferyState = "dialog3";
            Player.SaveData();
        }
        if (CharacterIndex == 3)
        {
            Player.FarmerKl3pState = "dialog3";
            Player.SaveData();
        }
        if (CharacterIndex == 4)
        {
            Player.DuckState = "dialog3";
            Player.SaveData();
        }
        if (CharacterIndex == 5)
        {
            Player.MrSmithState = "dialog3";
            Player.SaveData();
        }
        if (CharacterIndex == 6)
        {
            Player.JesseState = "dialog3";
            Player.SaveData();
        }
        if (CharacterIndex == 7)
        {
            Player.WinstonState = "dialog3";
            Player.SaveData();
        }
        if (CharacterIndex == 8)
        {
            Player.JessyState = "dialog3";
            Player.SaveData();
        }
        if (CharacterIndex == 9)
        {
            Player.SherriffState = "dialog3";
            Player.SaveData();
        }
        if (CharacterIndex == 10)
        {
            Player.ItachiState = "dialog3";
            Player.SaveData();
        }
        if (CharacterIndex == 11)
        {
            Player.AnkokuState = "dialog3";
            Player.SaveData();
        }
        if (CharacterIndex == 10)
        {
            Player.SukiState = "dialog3";
            Player.SaveData();
        }

    }
}
