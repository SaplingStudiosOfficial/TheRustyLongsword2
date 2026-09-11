using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HensonMouthMove : MonoBehaviour
{
    private Animator animator;
    private int counter = 0;
    [SerializeField] TextMeshProUGUI textObject;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] TextMeshProUGUI dialogTextT1;
    [SerializeField] TextMeshProUGUI dialogTextT2;
    [SerializeField] TextMeshProUGUI dialogTextT3;
    [SerializeField] TextMeshProUGUI textObjectT1;
    [SerializeField] TextMeshProUGUI textObjectT2;
    [SerializeField] TextMeshProUGUI textObjectT3;
    [SerializeField] ScriptReader DialogManager;
    public 

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if ((textObject.text == "Henson" || textObject.text == "???") && textObject.gameObject.activeSelf && DialogManager.CR_running)
        {
            animator.SetBool("isTalking", true);
            textObject.color = new Color(187f / 155f, 190f / 255, 69f / 255f);
            dialogText.color = new Color(187f / 155f, 190f / 255, 69f / 255f);
        }
        else if ((textObjectT1.text == "Henson" || textObjectT1.text == "???") && textObjectT1.gameObject.activeSelf && DialogManager.CR_running)
        {
            animator.SetBool("isTalking", true);
            textObjectT1.color = new Color(187f / 155f, 190f / 255, 69f / 255f);
            dialogTextT1.color = new Color(187f / 155f, 190f / 255, 69f / 255f);
        }
        else if ((textObjectT2.text == "Henson" || textObjectT2.text == "???") && textObjectT2.gameObject.activeSelf && DialogManager.CR_running)
        {
            animator.SetBool("isTalking", true);
            textObjectT2.color = new Color(187f / 155f, 190f / 255, 69f / 255f);
            dialogTextT2.color = new Color(187f / 155f, 190f / 255, 69f / 255f);
        }
        else if ((textObjectT3.text == "Henson" || textObjectT3.text == "???") && textObjectT3.gameObject.activeSelf && DialogManager.CR_running)
        {
            animator.SetBool("isTalking", true);
            textObjectT3.color = new Color(187f / 155f, 190f / 255, 69f / 255f);
            dialogTextT3.color = new Color(187f / 155f, 190f / 255, 69f / 255f);
        }else if (textObject.text == "??" || textObject.text == "Goblin")
        {
            animator.SetBool("isTalking", true);
            textObjectT3.color = new Color(141f / 155f, 165f / 255, 151f / 255f);
            dialogTextT3.color = new Color(141f / 155f, 165f / 255, 151f / 255f);
        }
        else
        {
            animator.SetBool("isTalking", false);
        }
    }
}
