using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class ScriptReader : MonoBehaviour
{
    [SerializeField] PlayerController playerObject;
    [SerializeField] private TextAsset _JsonTestAsset;
    [SerializeField] private Story _StoryScript;
    [SerializeField] private GameObject CurrentScene;

    [SerializeField] private GameObject Mixer;
    [SerializeField] private GameObject ScriptReading;

    public bool endInteraction;
    public bool midInteraction;

    public TMP_Text dialogBox;
    public TMP_Text nameTag;

    public float typingSpeed;
    public bool canContinue = true;

    // 'new' because this deliberately hides UnityEngine.Object.name.
    // It is the SPEAKER's name and the scenes have real authored
    // values in it ("MayorK", "Henson"), so it must not be renamed.
    public new string name;

    public AudioClip[] audioLines;
    public AudioSource player;
    public int lineIndex;
    [SerializeField] private GameObject mixerObject;
    [SerializeField] private GameObject nightManagerObject;
    private NightManagerScript nightManager;
    [SerializeField] private GameObject endMenu;
    [SerializeField] private GameObject characterObject;
    [SerializeField] private GameObject PopPlayer;
    public bool isEnd;


    public bool CR_running;

    // Start is called before the first frame update
    void Start()
    {
        nightManager = nightManagerObject.GetComponent<NightManagerScript>();
        CR_running = false;
        LoadStory();
        player = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CR_running)
        {

            typingSpeed = .0002f;

        }

        if (Input.GetKeyDown(KeyCode.Space) && !CR_running)
        {
            dialogBox.color = new Color(69f / 255f, 190f / 255f, 119f / 255f);
            nameTag.color = new Color(69f / 255f, 190f / 255f, 119f / 255f);
            DisplayNextLine();
            lineIndex++;
        }
        if (lineIndex > audioLines.Length)
        {
            
            if (midInteraction) 
            {

                PopPlayer.SetActive(false);
                mixerObject.SetActive(true);
                characterObject.SetActive(false);
                
            }
            
            if (endInteraction)
            {
                PopPlayer.SetActive(false);
                nightManager.removeName(name);
                characterObject.SetActive(false);
                if(name != "Henson" && name != "MayorK")
                {
                    CurrentScene.SetActive(false);
                }
                nightManagerObject.SetActive(true);
                
                
            } 
            
        }
    }

    void LoadStory()
    {
        _StoryScript = new Story(_JsonTestAsset.text);

        _StoryScript.BindExternalFunction("Name", (string charName) => ChangeName(charName));

    }

    public void DisplayNextLine()
    {
        nameTag.text = " ";
        dialogBox.text = " ";
        if (_StoryScript.canContinue)
        {
            StartCoroutine(Type());
        }
        else
        {
            Mixer.SetActive(true);
            nameTag.text = " ";
            dialogBox.text = "[press space to continue]"; 
            gameObject.SetActive(false);
        }
    }

    public void ChangeName(string name)
    {
        string SpeakerName = name;
        nameTag.text = SpeakerName;
    }
    IEnumerator Type()
    {
        CR_running = true;
        PopPlayer.SetActive(CR_running);
        string line = _StoryScript.Continue();
        foreach (char letter in line.ToCharArray())
        {
            dialogBox.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        CR_running = false;
        PopPlayer.SetActive(CR_running);
    }
}
