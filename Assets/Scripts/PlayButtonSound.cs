using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonSound : MonoBehaviour
{
    [SerializeField] AudioClip buttonNoise;
    [SerializeField] AudioSource audioPlayer;
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playButtonNoise()
    {
        audioPlayer.PlayOneShot(buttonNoise);
    }
}
