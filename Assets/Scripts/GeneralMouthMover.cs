using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GeneralMouthMover : MonoBehaviour
{
    private Animator animator;

    [SerializeField] TextMeshProUGUI nameTag;
    [SerializeField] TextMeshProUGUI bodyText;
    [SerializeField] ScriptReader DialogManager;

    // 'new' because this deliberately hides UnityEngine.Object.name.
    // It is the SPEAKER's name and the scenes have real authored
    // values in it, so it must not be renamed.
    public new string name;

    Color color;

    


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(name == "Mayor King")
        {

            color = new Color(255f / 255f, 93f / 255f, 89f / 255f);

        }

        if (nameTag.text == name && DialogManager.CR_running)
        {
            animator.SetBool("isTalking", true);
            nameTag.color = color;
            bodyText.color = color;
        }
        else
        {
            animator.SetBool("isTalking", false);
        }
    }
}
