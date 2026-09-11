using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JefferyManager : MonoBehaviour
{
    [SerializeField] GameObject Jeffery;
    [SerializeField] TextMeshProUGUI nameTag;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (nameTag.text == "Goblin")
        {
            Jeffery.SetActive(true);
        }
    }
}
