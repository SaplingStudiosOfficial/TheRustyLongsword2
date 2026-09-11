using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueCoinManager : MonoBehaviour
{

    [SerializeField] private Collider boxCollider;
    [SerializeField] private GameObject pickupCanvas;
    public PlayerController player;
    public int index;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (player.UniqueCoins[index] == 1)
        {
            StartCoroutine(WaitThenDestroy());
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            player.UniqueCoins[index] = 1;
            player.SaveData();
            pickupCanvas.SetActive(true);
            StartCoroutine(player.PlayTreasureAnimation(gameObject));
        }
    }

    public IEnumerator WaitThenDestroy()
    {
        yield return new WaitForSeconds(3.5f);
        Destroy(gameObject);
    }
}
