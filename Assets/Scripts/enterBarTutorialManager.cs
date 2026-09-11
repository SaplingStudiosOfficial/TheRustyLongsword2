using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enterBarTutorialManager : MonoBehaviour
{
    [SerializeField] public PlayerController player;
    [SerializeField] public GameObject enterBarTutorial;
    public int town;

    // Start is called before the first frame update
    void Start()
    {
        if (player.enterBarTutorialDone != true)
        {
            enterBarTutorial.SetActive(true);
            player.enterBarTutorialDone = true;
            player.SaveData();
            Cursor.visible = true;
            player.isPlaying = false;
        }
    }
}
