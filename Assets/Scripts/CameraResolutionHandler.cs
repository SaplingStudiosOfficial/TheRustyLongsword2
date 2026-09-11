using UnityEngine;

public class CameraResolutionHandler : MonoBehaviour
{
    public Camera gameCamera;
    public float defaultAspectRatio = 16f / 9f; // Example: 16:9 aspect ratio

    void Start()
    {
        AdjustCamera();
    }

    void AdjustCamera()
    {
        if (gameCamera == null) return;

        float currentAspectRatio = (float)Screen.width / Screen.height;

        // Adjust FOV for a perspective camera or size for an orthographic camera
        if (gameCamera.orthographic)
        {
            gameCamera.orthographicSize *= defaultAspectRatio / currentAspectRatio;
        }
        else
        {
            gameCamera.fieldOfView *= defaultAspectRatio / currentAspectRatio;
        }
    }
}