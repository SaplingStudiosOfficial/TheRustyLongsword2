using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class fadeInAudio : MonoBehaviour
{
    [SerializeField] public AudioSource music;
    [SerializeField] public AudioClip[] themes;
    public PlayerController player;

    public bool fadingIn = false;
    public bool fadingOut = false;
    private float fadeSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        music.volume = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.worldMusic == 0 && !fadingIn && !fadingOut)
        {
            music.clip = themes[0];
            if (!music.isPlaying)
            {
                music.Play();
                music.loop = true;
            }
            fadingIn = true;
            FadeInMusic();
        }
        else if (player.worldMusic == 1 && !fadingOut && !fadingIn)
        {
            if (!music.isPlaying)
            {
                music.Play();
                music.loop = true;
            }
            music.clip = themes[1];
            fadingIn = true;
            FadeInMusic();
        }
        else if (player.worldMusic == 2 && !fadingOut && !fadingIn)
        {
            if (!music.isPlaying)
            {
                music.Play();
                music.loop = true;
            }
            music.clip = themes[2];
            fadingIn = true;
            FadeInMusic();
        }
        else if (player.worldMusic == 3 && !fadingOut && !fadingIn)
        {
            if (!music.isPlaying)
            {
                music.Play();
                music.loop = true;
            }
            music.clip = themes[3];
            fadingIn = true;
            FadeInMusic();
        }
        else if (player.worldMusic != 2 && player.worldMusic != 1 && player.worldMusic != 0 && !fadingOut && !fadingIn)
        {
            fadingOut = true;
            FadeOutMusic();
        }
    }

    public void FadeInMusic()
    {
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        
        while (music.volume < 1f)
        {
            music.volume += fadeSpeed * Time.deltaTime;
            yield return null;
        }

        fadingIn = false;
    }

    public void FadeOutMusic()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        while (music.volume > 0f)
        {
            music.volume -= fadeSpeed * Time.deltaTime;
            yield return null;
        }

        fadingOut = false;
    }
}
