using UnityEngine;

public class ResolutionSetter : MonoBehaviour
{
    void Start()
    {
        // Set the game to run at 1920x1080 resolution
        Screen.SetResolution(1920, 1080, true); // The third parameter is for fullscreen mode
    }
}
