using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public int coinIndex;
    [SerializeField] public PlayerController player;
    public AudioSource CoinSound;
    public AudioClip CoinClip;
    public GameObject CoinAnimation;
    public Collider coll;
    public GameObject coin;
    private bool hadChecked;
    [SerializeField] private int village;

    // Start is called before the first frame update
    void Start()
    {
        hadChecked = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (village == 1)
        {
            if (!hadChecked)
            {
                if (player.VillageOneCollectableInfo[coinIndex] != 0)
                {
                    Destroy(gameObject);
                }
                hadChecked = true;
            }
        }else if (village == 2)
        {
            if (!hadChecked)
            {
                if (player.VillageTwoCollectableInfo[coinIndex] != 0)
                {
                    Destroy(gameObject);
                }
                hadChecked = true;
            }
        }
        else if (village == 3)
        {
            if (!hadChecked)
            {
                if (player.VillageThreeCollectableInfo[coinIndex] != 0)
                {
                    Destroy(gameObject);
                }
                hadChecked = true;
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (village == 1)
        {
            if (collider.tag == "Player")
            {
                CoinSound.Play();
                player.VillageOneCollectableInfo[coinIndex] = 1;
                player.VillageOneCollectable += 1;
                coin.SetActive(false);
                coll.enabled = false;
                CoinAnimation.SetActive(true);
                player.SaveData();
            }
        }else if(village == 2)
        {
            if (collider.tag == "Player")
            {
                CoinSound.Play();
                player.VillageTwoCollectableInfo[coinIndex] = 1;
                player.VillageTwoCollectable += 1;
                coin.SetActive(false);
                coll.enabled = false;
                CoinAnimation.SetActive(true);
                player.SaveData();
            }
        }
        else if (village == 3)
        {
            if (collider.tag == "Player")
            {
                CoinSound.Play();
                player.VillageThreeCollectableInfo[coinIndex] = 1;
                player.VillageThreeCollectable += 1;
                coin.SetActive(false);
                coll.enabled = false;
                CoinAnimation.SetActive(true);
                player.SaveData();
            }
        }
    }

}
