using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixerManager : MonoBehaviour
{
    [SerializeField] public GameObject ConfirmMenu;
    [SerializeField] GameObject mixerMenu;
    [SerializeField] CanvasGroup mixerUI;
    [SerializeField] GameObject FlyTai;
    [SerializeField] GameObject PineColada;
    [SerializeField] GameObject Rum;

    [SerializeField] GameObject Light1;
    [SerializeField] GameObject Light2;
    [SerializeField] GameObject Light3;

    [SerializeField] GameObject TimeLine1;
    [SerializeField] GameObject TimeLine2;
    [SerializeField] GameObject TimeLine3;

    public PlayerController Inventory;

    [SerializeField] GameObject Rumbutton;
    [SerializeField] GameObject Flybutton;
    [SerializeField] GameObject Pinebutton;
    [SerializeField] GameObject Windbutton;
    [SerializeField] GameObject Glass;
    [SerializeField] GameObject Shelf;
    [SerializeField] GameObject BlockSpecials;

    public bool pullUp;
    public bool putAway;
    public bool canFade;




    public List<int> drink = new List<int>() { };

    // Start is called before the first frame update
    void Start()
    {
        canFade = false;
        Rumbutton.SetActive(false);
        Pinebutton.SetActive(false);
        Flybutton.SetActive(false);
        Windbutton.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        ButtonManager();
        LightController();

        if (pullUp)
        {
            pullUpMixer();
        }
        if (putAway)
        {
            putAwayMixer();
            canFade = true;
        }

        if (drink.Count == 3 && putAway == false)
        {
            
            StartCoroutine(Wait());
        }
    }


    public void pullUpMixer()
    {
        
        if (Shelf.transform.position.x > Screen.width * .75)
        {
            Shelf.transform.position = new Vector3(Shelf.transform.position.x - Time.deltaTime * 4000, Shelf.transform.position.y, Shelf.transform.position.z);
            
        }
        if (mixerUI.alpha < 1)
        {
            BlockSpecials.SetActive(false);
            mixerUI.alpha += Time.deltaTime * 2;   
        }
        

    }
    public void putAwayMixer()
    {
        pullUp = false;
        if (Glass.transform.position.x > -10000)
        {
            Glass.transform.position = new Vector3(Glass.transform.position.x - Time.deltaTime * 4000, Glass.transform.position.y, Glass.transform.position.z);
        }
        if (Shelf.transform.position.x < 10000)
        {
            Shelf.transform.position = new Vector3(Shelf.transform.position.x + Time.deltaTime * 4000, Shelf.transform.position.y, Shelf.transform.position.z);
        }
        if (canFade)
        {
            mixerUI.alpha -= Time.deltaTime * 2;
        }
        if (Glass.transform.position.x <= -10000 && Shelf.transform.position.x >= -10000)
        {
            StartCoroutine(WaitThenFade());
        }
        if (mixerUI.alpha <= 0 && Glass.transform.position.x <= -450 && Shelf.transform.position.x >= 2900)
        {
            canFade = false;
            putAway = false;
            gameObject.SetActive(false);
        }

    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);
        ConfirmMenu.SetActive(true);
        putAway = true;
        StartCoroutine(SecondWait());
    }
    public IEnumerator SecondWait()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }
    public IEnumerator WaitThenFade()
    {
        yield return new WaitForSeconds(2f);
        canFade = true;
        
    }
    public void AddRum()
    {
        drink.Add(0);
        Inventory.UseRum();
    }
    public void AddFly()
    {
        drink.Add(1);
        Inventory.UseFly();
    }
    public void AddPine()
    {
        drink.Add(2);
        Inventory.UsePine();
    }
    public void AddWind()
    {
        drink.Add(3);
        Inventory.UseSea();

    }

    private void LightController()
    {
        if (drink.Count >= 1)
        {
            Light1.SetActive(true);
        }
        if (drink.Count >= 2)
        {
            Light2.SetActive(true);
        }
        if (drink.Count >= 3)
        {
            Light3.SetActive(true);
        }
    }

    private void ButtonManager()
    {
        if (Inventory.amtRum > 0)
        {
            Rumbutton.SetActive(true);
        }else if (Inventory.amtRum == 0)
        {
            Rumbutton.SetActive(false);
        }
        if (Inventory.amtFly > 0)
        {
            Flybutton.SetActive(true);
        }
        else if (Inventory.amtFly == 0)
        {
            Flybutton.SetActive(false);
        }
        if (Inventory.amtPine > 0)
        {
            Pinebutton.SetActive(true);
        }
        else if (Inventory.amtPine == 0)
        {
            Pinebutton.SetActive(false);
        }
        if (Inventory.amtSea > 0)
        {
            Windbutton.SetActive(true);
        }
        else if (Inventory.amtSea == 0)
        {
            Windbutton.SetActive(false);
        }
    }
    public void ResetMixer()
    {
        
        Light1.SetActive(false);
        Light2.SetActive(false);
        Light3.SetActive(false);
        pullUp = true;
        putAway = false;
        foreach(int i in drink) { }
        drink.RemoveAt(0);
        drink.RemoveAt(0);
        drink.RemoveAt(0);
    }
}
