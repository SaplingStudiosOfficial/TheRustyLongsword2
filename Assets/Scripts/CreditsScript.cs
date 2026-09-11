using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScript : MonoBehaviour
{
    [SerializeField] private GameObject[] targets; // Array of target objects as serializable fields
    [SerializeField] private GameObject movingObject; // The object that will move towards the targets
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject Rana;
    [SerializeField] private GameObject Cambad;
    [SerializeField] private GameObject Camgood;
    [SerializeField] private GameObject CreditsText;
    private int currentTargetIndex = 0; // Index of the current target
    public float speed = 100f; // Movement speed
    public float rotationSpeed = 1f; // Rotation speed
    private bool shouldMove = false; // Flag to control movement

    void Update()
    {
        if (shouldMove)
        {
            player.worldMusic = 3;
            CreditsText.SetActive(true);
            player.isPlaying = false;
            Cambad.SetActive(false);
            Camgood.SetActive(true);
            Rana.transform.parent = movingObject.transform; // Parent Rana to the moving object

            // Set Rana's position to be in front of the moving object by 5 units
            Rana.transform.localPosition = new Vector3(0, 0, .5f);

            // Rotate Rana 180 degrees around the y-axis relative to the moving object
            Rana.transform.localRotation = Quaternion.Euler(0, 0, 0);

            GameObject currentTarget = targets[currentTargetIndex];

            // Calculate the rotation needed to look at the target
            Quaternion targetRotation = Quaternion.LookRotation(currentTarget.transform.position - movingObject.transform.position);

            // Rotate the moving object towards the current target
            movingObject.transform.rotation = Quaternion.Slerp(movingObject.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move the moving object towards the current target
            movingObject.transform.position = Vector3.MoveTowards(movingObject.transform.position, currentTarget.transform.position, speed * Time.deltaTime * 4.8f);

            // Check if the moving object has reached the target
            if (Vector3.Distance(movingObject.transform.position, currentTarget.transform.position) < 0.1f)
            {
                currentTargetIndex++;

                if (currentTargetIndex >= targets.Length)
                {
                    // New code to unparent children and set isPlaying to true
                    foreach (Transform child in movingObject.transform)
                    {
                        child.parent = null;
                    }
                    player.isPlaying = true;
                    player.deleteSave();
                    SceneManager.LoadScene("3DEnviroment");
                    Destroy(movingObject); // Destroy the moving object
                    return; // Exit the function to avoid further processing after destruction
                }
            }
        }
        else if (Rana.transform.parent == movingObject.transform)
        {
            player.isPlaying = true;
            Rana.transform.parent = null; // Detach Rana from the moving object
        }
    }

    // OnTriggerEnter is called when the Collider other enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            shouldMove = true; // Start moving the object towards targets when the player triggers it
        }
    }
}
