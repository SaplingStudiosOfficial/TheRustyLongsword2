using UnityEngine;

public class CircularMotion : MonoBehaviour
{
    public float radius = 5f;      // Radius of the circle
    public float speed = 1f;       // Speed of the movement
    private float angle = 0f;      // Current angle
    private Vector3 centerPoint;    // Center point of the circular path

    void Start()
    {
        // Set the center point based on the current position and desired radius
        centerPoint = transform.position + new Vector3(radius, 0, 0);
    }

    void Update()
    {
        // Increment the angle based on speed and time
        angle += speed * Time.deltaTime;

        // Calculate the new position around the center point
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        Vector3 newPosition = centerPoint + new Vector3(x, 0, z);

        // Set the object's position
        transform.position = newPosition;

        // Calculate the next position for directional facing
        Vector3 nextPosition = centerPoint + new Vector3(Mathf.Cos(angle + speed * Time.deltaTime) * radius, 0, Mathf.Sin(angle + speed * Time.deltaTime) * radius);

        // Calculate a direction vector that is 90 degrees to the left of the movement direction
        Vector3 leftDirection = Quaternion.Euler(0, -90, 0) * (nextPosition - newPosition).normalized;

        // Rotate the object to face 90 degrees to the left of its current direction
        transform.rotation = Quaternion.LookRotation(leftDirection);
    }
}
