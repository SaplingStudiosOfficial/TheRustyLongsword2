using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource voice;
    [SerializeField] private AudioClip line0;
    [SerializeField] private AudioClip line1;
    [SerializeField] private AudioClip line2;
    [SerializeField] private AudioClip line3;
    [SerializeField] private AudioClip line4;
    [SerializeField] private AudioClip line5;
    [SerializeField] private AudioClip line6;
    [SerializeField] private AudioClip line7;
    [SerializeField] private AudioClip line8;
    [SerializeField] private AudioClip line9;
    [SerializeField] private AudioClip line10;
    [SerializeField] private AudioClip line11;
    [SerializeField] private AudioClip line12;
    [SerializeField] private AudioClip line13;
    [SerializeField] private AudioClip line14;
    public bool hadFadeOut;
    public bool exitEarly;
    [SerializeField] private GameObject FadeOutUI;

    private int counter;
    public bool stopLastLine;

    [SerializeField] ScriptReader reader;

    // Start is called before the first frame update
    void Start()
    {
        voice = GetComponent<AudioSource>();
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && counter == 0 && !(reader.CR_running))
        {
            voice.Stop();
            voice.PlayOneShot(line1);
            counter += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && counter == 1 && !(reader.CR_running))
        {
            voice.Stop();
            voice.PlayOneShot(line2);
            counter += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && counter == 2 && !(reader.CR_running))
        {
            voice.Stop();
            voice.PlayOneShot(line3);
            counter += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && counter == 3 && !(reader.CR_running))
        {
            voice.Stop();
            voice.PlayOneShot(line4);
            counter += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && counter == 4 && !(reader.CR_running))
        {
            voice.Stop();
            voice.PlayOneShot(line5);
            counter += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && counter == 5 && !(reader.CR_running))
        {
            voice.Stop();
            voice.PlayOneShot(line6);
            counter += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && counter == 6 && !(reader.CR_running))
        {
            voice.Stop();
            voice.PlayOneShot(line7);
            counter += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && counter == 7 && !(reader.CR_running))
        {
            voice.Stop();
            voice.PlayOneShot(line8);
            counter += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && counter == 8 && !(reader.CR_running))
        {
            voice.Stop();
            voice.PlayOneShot(line9);
            counter += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && counter == 9 && !(reader.CR_running))
        {
            voice.Stop();
            voice.PlayOneShot(line10);
            counter += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && counter == 10 && !(reader.CR_running))
        {
            voice.Stop();
            voice.PlayOneShot(line11);
            counter += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && counter == 11 && !(reader.CR_running))
        {
            voice.Stop();
            voice.PlayOneShot(line12);
            counter += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && counter == 12 && !(reader.CR_running))
        {
            voice.Stop();
            voice.PlayOneShot(line13);
            counter += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && counter == 13 && !(reader.CR_running))
        {
            voice.Stop();
            voice.PlayOneShot(line14);
            counter += 1;
            if (exitEarly)
            {
                FadeOutUI.SetActive(true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space) && counter == 14 && !(reader.CR_running))
        {
            if (hadFadeOut) {
                FadeOutUI.SetActive(true);
            }
            if (stopLastLine) {
                voice.Stop();
            }
            
        }
    }
}
