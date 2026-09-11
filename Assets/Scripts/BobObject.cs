using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobObject : MonoBehaviour
{
    public float amplitude = 0.5f; // The height of the bob
    public float frequency = 1.0f; // Speed of the bob

    private float originalY;

    void Start()
    {
        // Save the original Y position of the object
        originalY = transform.position.y;
    }

    void Update()
    {
        // Calculate the new Y position using a sine wave
        float newY = originalY + amplitude * Mathf.Sin(Time.time * frequency);

        // Update the position of the object
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }


}
