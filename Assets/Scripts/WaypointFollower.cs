using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints1;
    [SerializeField] private GameObject[] waypoints2;
    [SerializeField] private GameObject movingObject;
    public bool followSecondPath = false;
    private GameObject[] currentWaypoints;
    private int currentTargetIndex = 0;
    public float speed = 100f;
    public float rotationSpeed = 1f;
    public float bobbingSpeed = 2f;
    public float bobbingAmount = 0.5f;
    private float originalY;
    private bool shouldMove = true;

    void Start()
    {
        originalY = movingObject.transform.position.y;
        UpdatePath();
    }

    public void ChangePath()
    {
        followSecondPath = true;
        UpdatePath();
    }

    private void UpdatePath()
    {
        currentWaypoints = followSecondPath ? waypoints2 : waypoints1;
        currentTargetIndex = 0; // Reset the index when the path is changed
    }

    void Update()
    {
        if (shouldMove && currentWaypoints.Length > 0)
        {
            GameObject currentTarget = currentWaypoints[currentTargetIndex];
            Vector3 direction = currentTarget.transform.position - movingObject.transform.position;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            movingObject.transform.rotation = Quaternion.Slerp(movingObject.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Vector3 targetPosition = new Vector3(currentTarget.transform.position.x, originalY, currentTarget.transform.position.z);
            movingObject.transform.position = Vector3.MoveTowards(movingObject.transform.position, targetPosition, speed * Time.deltaTime);

            float newY = originalY + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
            movingObject.transform.position = new Vector3(movingObject.transform.position.x, newY, movingObject.transform.position.z);

            if (Vector3.Distance(new Vector3(movingObject.transform.position.x, originalY, movingObject.transform.position.z), targetPosition) < 0.1f)
            {
                currentTargetIndex = (currentTargetIndex + 1) % currentWaypoints.Length;
            }
        }
    }
}