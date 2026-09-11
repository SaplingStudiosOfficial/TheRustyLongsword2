using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MasterAudioManager : MonoBehaviour
{
    [SerializeField] public AudioMixer WorldAudio;
    [SerializeField] public PlayerController Player;


    // Start is called before the first frame update
    void Start()
    {
        SetVolume(Player.VolumeValue);
    }

    // Update is called once per frame
    void Update()
    {
        SetVolume(Player.VolumeValue);
    }

    public void SetVolume(float volume)
    {
        if (volume <= 0)
        {
            WorldAudio.SetFloat("MasterVolume", -80);
        }
        else
        {
            float db = Mathf.Log10(volume) * 20;
            WorldAudio.SetFloat("MasterVolume", db);
        }
    }

}