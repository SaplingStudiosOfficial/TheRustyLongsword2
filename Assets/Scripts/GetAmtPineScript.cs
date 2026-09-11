using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetAmtPineScript : MonoBehaviour
{
    [SerializeField] public PlayerController player;
    [SerializeField] public TextMeshProUGUI amtPineText;
    [SerializeField] public TextMeshProUGUI amtFliesText;

    // Update is called once per frame
    void Update()
    {
        amtPineText.text = player.amtPine.ToString() + "x";
        amtFliesText.text = player.amtFly.ToString() + "x";
    }
}
