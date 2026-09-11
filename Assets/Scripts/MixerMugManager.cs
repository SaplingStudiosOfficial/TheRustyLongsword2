using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixerMugManager : MonoBehaviour
{
    public bool slideOn;
    public Animator anim;
    public GameObject Parent;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (slideOn)
        {
            SlideOnScreen();
        }
    }

    public void SlideOnScreen()
    {
        if(Parent.transform.position.x < Screen.width / 4)
        {
            Parent.transform.position = new Vector3(Parent.transform.position.x + Time.deltaTime * 4000, Parent.transform.position.y, Parent.transform.position.z);
            anim.SetBool("MoveLeft", true);
            anim.SetBool("Idle", false);
        }
        else if (Parent.transform.position.x >= Screen.width/4)
        {
            anim.SetBool("MoveLeft", false);
            anim.SetBool("Idle", true);
        }
    }

    public void StartSlide()
    {
        slideOn = true;
    }
}
