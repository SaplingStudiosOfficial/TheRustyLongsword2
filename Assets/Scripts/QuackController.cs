using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuackController : MonoBehaviour
{
    public Animator anim;
    public AudioSource quack;
    public bool CR_running;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        quack = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!CR_running)
        {
            StartCoroutine(WaitQuackChance());
        }
    }

    public void StopQuack()
    {
        anim.SetBool("quacking", false);
    }

    public void Quack()
    {
        
        quack.Play();
    }
    public IEnumerator WaitQuackChance()
    {
        CR_running = true;
        int rand = Random.Range(0, 7);
        if (rand == 2)
        {
            anim.SetBool("quacking", true);
        }
        yield return new WaitForSeconds(1f);
        CR_running = false;
    }
}
