using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net;

public class HenryMouthManager : MonoBehaviour
{
    private Animator animator;
    [SerializeField] TextMeshProUGUI textObject;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] ScriptReader dialogManager;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if ((textObject.text == "Goblin" || textObject.text == "Rana and Goblin") && textObject.gameObject.activeSelf && dialogManager.CR_running)
        {
            animator.SetBool("isTalking", true);
            textObject.color = new Color(141f / 155f, 165f / 255, 151f / 255f);
            dialogText.color = new Color(141f / 155f, 165f / 255, 151f / 255f);
        }
        else
        {
            animator.SetBool("isTalking", false);
        }
    }
}
