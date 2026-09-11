using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject itemToSpawn1;
    [SerializeField] private GameObject itemToSpawn2;
    [SerializeField] private GameObject itemToSpawn3;
    [SerializeField] private GameObject itemToSpawn4;
    [SerializeField] private GameObject itemToSpawn5;

    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(0, 2) > 0)
        {
            itemToSpawn1.SetActive(true);
        }
        if (Random.Range(0, 2) > 0)
        {
            itemToSpawn2.SetActive(true);
        }
        if (Random.Range(0, 2) > 0)
        {
            itemToSpawn3.SetActive(true);
        }
        if (Random.Range(0, 2) > 0)
        {
            itemToSpawn4.SetActive(true);
        }
        if (Random.Range(0, 2) > 0)
        {
            itemToSpawn5.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
