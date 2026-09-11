using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterBarManager : MonoBehaviour
{
    public PlayerController player;
    public GameObject EnterBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.invitedCharacters.Count == 0)
        {

            EnterBar.SetActive(false);

        }
        else if (player.invitedCharacters.Count > 0)
        {
            EnterBar.SetActive(true);
        }
    }
}
