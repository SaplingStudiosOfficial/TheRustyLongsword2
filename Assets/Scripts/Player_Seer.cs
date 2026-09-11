using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Seer : MonoBehaviour
{
    [SerializeField] private GameObject startPoint; // Assign this in the Unity Editor
    [SerializeField] private AudioClip TeleportNoise;

    private AudioSource SoundPlayer;

    public void Start()
    {
        SoundPlayer = this.GetComponent<AudioSource>();
    }

    public void Update()
    {
     
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Player Has been contacted");
            if (SoundPlayer.isPlaying)
            {
                SoundPlayer.Stop();
            }
            SoundPlayer.PlayOneShot(TeleportNoise);
        }
    }

    public GameObject GetStartPoint()
    {
        return startPoint;
    }
}
