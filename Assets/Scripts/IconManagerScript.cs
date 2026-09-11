using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconManagerScript : MonoBehaviour
{
    [SerializeField] GameObject PineConeIcon;
    [SerializeField] GameObject RumIcon;
    [SerializeField] GameObject FlyIcon;
    [SerializeField] GameObject HatfieldCoins;
    [SerializeField] GameObject EastwoodCoins;
    [SerializeField] GameObject BeonahaCoins;

    public PlayerController Inventory;

    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Inventory.amtFly > 0)
        {
            FlyIcon.SetActive(true);
        }
        else
        {
            FlyIcon.SetActive(false);
        }
        if (Inventory.amtPine > 0)
        {
            PineConeIcon.SetActive(true);
        }
        else
        {
            PineConeIcon.SetActive(false);
        }
        if (Inventory.amtRum > 0)
        {
            RumIcon.SetActive(true);
        }
        else
        {
            RumIcon.SetActive(false);
        }
        if (Inventory.worldMusic == 0 && Inventory.isPlaying)
        {
            EastwoodCoins.SetActive(false);
            HatfieldCoins.SetActive(true);
            BeonahaCoins.SetActive(false);
        }
        else if (Inventory.worldMusic == 1 && Inventory.isPlaying)
        {
            EastwoodCoins.SetActive(true);
            HatfieldCoins.SetActive(false);
            BeonahaCoins.SetActive(false);
        }
        else if (Inventory.worldMusic == 2 && Inventory.isPlaying)
        {
            EastwoodCoins.SetActive(false);
            HatfieldCoins.SetActive(false);
            BeonahaCoins.SetActive(true);
        }
        else
        {
            EastwoodCoins.SetActive(false);
            HatfieldCoins.SetActive(false);
            BeonahaCoins.SetActive(false);
        }
    }
}
