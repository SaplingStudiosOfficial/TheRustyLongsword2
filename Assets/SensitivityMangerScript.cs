using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SensitivityMangerScript : MonoBehaviour
{
    [SerializeField] public CinemachineFreeLook freeLookCamera;
    [SerializeField] public PlayerController Player;
    public float newXAxisSpeed = 400f;
    public float newYAxisSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        if (freeLookCamera != null)
        {
            // Adjust the max speed for the X axis (horizontal movement)
            freeLookCamera.m_XAxis.m_MaxSpeed = newXAxisSpeed;

            // If you also want to adjust the speed for vertical input, you can do so here:
            // This value usually controls zoom or tilt, depending on your setup.
            freeLookCamera.m_YAxis.m_MaxSpeed = newYAxisSpeed;
        }
        else
        {
            Debug.LogError("CinemachineFreeLook camera is not assigned.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSpeed()
    {
        float newXSpeed = 400f * Player.mouseSensitivity;
        float newYSpeed = 3f * Player.mouseSensitivity;
        if (freeLookCamera != null)
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = newXSpeed;
            freeLookCamera.m_YAxis.m_MaxSpeed = newYSpeed;
        }
    }
}
