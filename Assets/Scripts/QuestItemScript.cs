using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItemScript : MonoBehaviour
{
    [SerializeField] public NPCDialogManager questGiver;
    [SerializeField] public GameObject questItemFound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            questGiver.FinishQuest();
            questItemFound.SetActive(true);
            Destroy(gameObject);
            
        }
    }
}
