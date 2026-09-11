using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

public class NightManagerScript : MonoBehaviour
{
    public Text introText;
    [SerializeField] PlayerController player;
    [SerializeField] GameObject endScreen;
    [SerializeField] GameObject Henson;
    [SerializeField] GameObject MayorKing;
    [SerializeField] GameObject Duck;
    [SerializeField] GameObject Jeffery;
    [SerializeField] GameObject FadeOut;
    [SerializeField] GameObject Itachi;
    [SerializeField] GameObject Ankoku;
    [SerializeField] GameObject Jesse;
    public List<string> shows;
    public List<string> here;


    GameObject[] characters;
    AudioSource Entry;

    // Start is called before the first frame update
    void Start()
    {

        if(player.worldMusic == 0)
        {
            shows.Add("MayorK");
            shows.Add("Henson");
        }else if (player.worldMusic == 1) {
            shows.Add("Jesse");
        }
        else if (player.worldMusic == 2) { 
            shows.Add("Itachi");
            shows.Add("Ankoku+Yurei");
        }
        for(int i = 0; i < player.invitedCharacters.Count; i++){
            if(shows.Contains(player.invitedCharacters[i])){
            here.Add(player.invitedCharacters[i]);
            }
        }
        
        Entry = GetComponent<AudioSource>();

        
        if (player.invitedCharacters.Contains("Duck"))
        {
            Duck.SetActive(true);
            player.invitedCharacters.Remove("Duck");
        }
    }

    void Update()
    {
         if (here.Count == 0)
        {
            EndNight();
            endScreen.SetActive(true);
            gameObject.SetActive(false);
        }
        StartCoroutine(Wait());
        
    }

    public void EndNight()
    {
        player.FinishNight();
        player.SaveData();
    }

    public IEnumerator Wait()
    {

        
        yield return new WaitForSeconds(7f);

        if (here.Contains("MayorK"))
        {
            MayorKing.SetActive(true);
            gameObject.SetActive(false);
        }
        else if (here.Contains("Henson"))
        {
            Henson.SetActive(true);
            gameObject.SetActive(false);
        }else if(here.Contains("Itachi"))
        {
            Itachi.SetActive(true);
            gameObject.SetActive(false);
        }
        else if (here.Contains("Ankoku+Yurei"))
        {
            Ankoku.SetActive(true);
            gameObject.SetActive(false);
        }
        else if (here.Contains("Jesse"))
        {
            Jesse.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void removeName(string name)
    {
        here.Remove(name);
    }
    
}
