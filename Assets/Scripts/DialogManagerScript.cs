using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogManagerScript : MonoBehaviour
{
    public string[] sentences;
    public int[] charactersSpeaking;
    public int index;
    public TextMeshProUGUI textDisplay;
    public float typingSpeed;
    public PlayerController player;
    public GameObject TextBox;
    public NPCDialogManager NPC;
    public NPCDialogManager NPC2;
    public bool inviteAfter;
    public string nextDialog;
    public string nextDialog2;
    public GameObject talkIcon;
    [SerializeField] private AudioSource TalkingNoise;
    public bool isTalking;
    public Animator TextAnimations;
    public int CharacterIndex;


    // Start is called before the first frame update
    void Start()
    {
        TextAnimations.SetInteger("WhoIsTalking", charactersSpeaking[0]);
        StartCoroutine(Type());
        TalkingNoise = this.GetComponent<AudioSource>();
    }
    void Update()
    {
        if (textDisplay.text == sentences[index])
        {
            nextSentence();
            isTalking = false;
            TalkingNoise.Stop();
        }
        if (textDisplay.text != sentences[index])
        {
            isTalking = true;
        }

        if (isTalking && TalkingNoise.isPlaying == false)
        {
            TalkingNoise.Play();
        }
        if (Input.GetKey(KeyCode.F))
        {
            typingSpeed = .001f;
        }
        else
        {
            typingSpeed = .02f;
        }
    }

    IEnumerator Type()
    {
        
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        
    }
    public void nextSentence()
    {
        if (index < sentences.Length - 1 && Input.GetKeyDown(KeyCode.F))
        {

            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
            if (index < charactersSpeaking.Length) {  
                TextAnimations.SetInteger("WhoIsTalking", charactersSpeaking[index]);
            }
        }
        else if (index >= sentences.Length - 1 && Input.GetKeyDown(KeyCode.F) && !inviteAfter)
        {

            index = 0;
            textDisplay.text = "";
            player.isPlaying = true;
            TextBox.SetActive(false);
            talkIcon.SetActive(true);
            if (NPC2 != null)
            {

                if (nextDialog2 != null)
                {

                    NPC2.interaction = nextDialog2;

                }

            }
            NPC.interaction = nextDialog;
            if (CharacterIndex == 0)
            {
                player.MayorKState = nextDialog;
                player.SaveData();
            }
            if (CharacterIndex == 1)
            {
                player.HensonState = nextDialog;
                player.SaveData();
            }
            if (CharacterIndex == 2)
            {
                player.JefferyState = nextDialog;
                player.SaveData();
            }
            if (CharacterIndex == 3)
            {
                player.FarmerKl3pState = nextDialog;
                player.SaveData();
            }
            if (CharacterIndex == 4)
            {
                player.DuckState = nextDialog;
                player.SaveData();
            }
            if (CharacterIndex == 5)
            {
                player.MrSmithState = nextDialog;
                player.SaveData();
            }
            if (CharacterIndex == 10)
            {
                player.ItachiState = nextDialog;
                player.SaveData();
            }
            
            gameObject.SetActive(false);

        }
        else if (index >= sentences.Length - 1 && Input.GetKeyDown(KeyCode.F) && inviteAfter)
        {

            index = 0;
            textDisplay.text = "";
            player.isPlaying = true;
            TextBox.SetActive(false);

            NPC.interaction = "dialog4";
            if (CharacterIndex == 0)
            {
                player.MayorKState = "dialog4";
                player.SaveData();
            }
            if (CharacterIndex == 1)
            {
                player.HensonState = "dialog4";
                player.SaveData();
            }
            if (CharacterIndex == 2)
            {
                player.JefferyState = "dialog4";
                player.SaveData();
            }
            if (CharacterIndex == 3)
            {
                player.FarmerKl3pState = "dialog4";
                player.SaveData();
            }
            if (CharacterIndex == 4)
            {
                player.DuckState = "dialog4";
                player.SaveData();
            }
            if (CharacterIndex == 5)
            {
                player.MrSmithState = "dialog4";
                player.SaveData();
            }
            if (CharacterIndex == 10)
            {
                player.ItachiState = "dialog4";
                player.SaveData();
            }
            gameObject.SetActive(false);
        }
    }
}
