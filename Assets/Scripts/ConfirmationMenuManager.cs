using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationMenuManager : MonoBehaviour
{
    //GameObjects that are being interacted with by the script.
    [SerializeField] GameObject TimeLine1;
    [SerializeField] GameObject TimeLine2;
    [SerializeField] GameObject TimeLine3;
    [SerializeField] GameObject Rum;
    [SerializeField] GameObject FlyTai;
    [SerializeField] GameObject PineColada;
    [SerializeField] MixerManager Mixer;
    [SerializeField] GameObject MixerGameObject;

    public void Update()
    {
        

        if (Mixer.drink[0] == 0 && Mixer.drink[1] == 0 && Mixer.drink[2] == 0)
        {
            
            Rum.SetActive(true);
            FlyTai.SetActive(false);
            PineColada.SetActive(false);

        }
        if (Mixer.drink[0] == 1 && Mixer.drink[1] == 0 && Mixer.drink[2] == 0)
        {
            FlyTai.SetActive(true);
            Rum.SetActive(false);

            PineColada.SetActive(false);
        }
        if (Mixer.drink[0] == 0 && Mixer.drink[1] == 1 && Mixer.drink[2] == 0)
        {
            FlyTai.SetActive(true);
            Rum.SetActive(false);

            PineColada.SetActive(false);
        }
        if (Mixer.drink[0] == 0 && Mixer.drink[1] == 0 && Mixer.drink[2] == 1)
        {
            FlyTai.SetActive(true);
            Rum.SetActive(false);

            PineColada.SetActive(false);
        }

        if (Mixer.drink[0] == 2 && Mixer.drink[1] == 0 && Mixer.drink[2] == 0)
        {
            PineColada.SetActive(true);
            Rum.SetActive(false);

            FlyTai.SetActive(false);

        }
        if (Mixer.drink[0] == 0 && Mixer.drink[1] == 2 && Mixer.drink[2] == 0)
        {
            PineColada.SetActive(true);
            Rum.SetActive(false);

            FlyTai.SetActive(false);
        }
        if (Mixer.drink[0] == 0 && Mixer.drink[1] == 0 && Mixer.drink[2] == 2)
        {
            PineColada.SetActive(true);
            Rum.SetActive(false);

            FlyTai.SetActive(false);
        }
        if (Mixer.drink[0] == 3 && Mixer.drink[1] == 0 && Mixer.drink[2] == 0)
        {
            PineColada.SetActive(true);
            Rum.SetActive(false);

            FlyTai.SetActive(false);
        }
        if (Mixer.drink[0] == 0 && Mixer.drink[1] == 3 && Mixer.drink[2] == 0)
        {
            PineColada.SetActive(true);
            Rum.SetActive(false);

            FlyTai.SetActive(false);
        }
        if (Mixer.drink[0] == 0 && Mixer.drink[1] == 0 && Mixer.drink[2] == 3)
        {
            PineColada.SetActive(true);
            Rum.SetActive(false);

            FlyTai.SetActive(false);
        }
    }

    public void MakeDrink()
    {
        if (Mixer.drink[0] == 0 && Mixer.drink[1] == 0 && Mixer.drink[2] == 0)
        {
            TimeLine3.SetActive(true);
            
            gameObject.SetActive(false);

        }
        if (Mixer.drink[0] == 1 && Mixer.drink[1] == 0 && Mixer.drink[2] == 0)
        {
           
            TimeLine1.SetActive(true);
            Destroy(gameObject);
        }
        if (Mixer.drink[0] == 0 && Mixer.drink[1] == 1 && Mixer.drink[2] == 0)
        {
            
            TimeLine1.SetActive(true);
            Destroy(gameObject);
        }
        if (Mixer.drink[0] == 0 && Mixer.drink[1] == 0 && Mixer.drink[2] == 1)
        {
           
            TimeLine1.SetActive(true);
            Destroy(gameObject);
        }

        if (Mixer.drink[0] == 2 && Mixer.drink[1] == 0 && Mixer.drink[2] == 0)
        {
          
            TimeLine2.SetActive(true);
            Destroy(gameObject);
        }
        if (Mixer.drink[0] == 0 && Mixer.drink[1] == 2 && Mixer.drink[2] == 0)
        {
           
            TimeLine2.SetActive(true);
            Destroy(gameObject);
        }
        if (Mixer.drink[0] == 0 && Mixer.drink[1] == 0 && Mixer.drink[2] == 2)
        {
          
            TimeLine2.SetActive(true);
            Destroy(gameObject);
        }
        if (Mixer.drink[0] == 3 && Mixer.drink[1] == 0 && Mixer.drink[2] == 0)
        {
           
            TimeLine1.SetActive(true);
            Destroy(gameObject);
        }
        if (Mixer.drink[0] == 0 && Mixer.drink[1] == 3 && Mixer.drink[2] == 0)
        {
         
            TimeLine1.SetActive(true);
            Destroy(gameObject);
        }
        if (Mixer.drink[0] == 0 && Mixer.drink[1] == 0 && Mixer.drink[2] == 3)
        {
          
            TimeLine1.SetActive(true);
            Destroy(gameObject);
        }
    }
    public void goBack()
    {
        MixerGameObject.SetActive(true);
        gameObject.SetActive(false);

    }
}
