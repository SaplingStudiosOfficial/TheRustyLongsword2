using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteChanger : MonoBehaviour
{
    public bool isActive = false;
    [SerializeField] private WaypointFollower Pirate;
    [SerializeField] private GameObject InactiveModel;
    [SerializeField] private GameObject ActiveModel;
    [SerializeField] private AudioSource SoundPlayer;
    [SerializeField] private GameObject CharacterInteractionPrompt;
    public bool canInteract = false;

    // Start is called before the first frame update
    void Start()
    {
        SoundPlayer = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            ActivateObject();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" && !(isActive))
        {
            canInteract = true;
            CharacterInteractionPrompt.SetActive(true);
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            canInteract = false;
            CharacterInteractionPrompt.SetActive(false);
        }
    }

    void ActivateObject()
    {
        if (SoundPlayer.isPlaying)
        {
            SoundPlayer.Stop();
        }
        SoundPlayer.Play();
        isActive = true;
        Pirate.ChangePath();
        InactiveModel.SetActive(false);
        ActiveModel.SetActive(true);
        CharacterInteractionPrompt.SetActive(false);
    }
}
