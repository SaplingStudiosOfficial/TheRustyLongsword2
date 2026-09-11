using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBookScript : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] GameObject PageFlip;
    public bool startOpen;

    void Start()
    {
        if (startOpen)
        {
            anim.SetBool("Open", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetBool("Open", true);
        }
    }

    void setPageFlipActive()
    {
        anim.SetBool("StayOpen", true);
        PageFlip.SetActive(true);
    }
}
