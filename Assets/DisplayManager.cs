using System.Collections;
using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    // Duration of the shift in seconds
    public float shiftDuration = 2.0f;
    // The total distance to shift
    private float distance = 15.0f;
    // The starting position of the object
    private Vector3 startingPosition;
    // Maximum number of negative shifts allowed from the starting position
    private int maxNegativeShifts = 8;
    // Current shift count from the starting position
    public int currentShiftCount = 0;
    // Flag to track if movement is in progress
    private bool isMoving = false;
    // Import the Player to access the current collected info.
    [SerializeField] private PlayerController Player;
    // Display Coins
    [SerializeField] private GameObject UndiscoveredInfo;
    //Coins Display Info
    [SerializeField] private GameObject[] coinDisplays; // Array of coin displays
    [SerializeField] private GameObject[] coinInfos; // Array of coin infos
    [SerializeField] private GameObject[] coinBlankDisplays; // Array of coin blank displays

    void Start()
    {
        //
        // Store the starting position of the object
        startingPosition = transform.position;
    }

    void Update()
    {
        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MovePositive();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveNegative();
            }
        }

        for (int i = 0; i < coinDisplays.Length; i++)
        {
            bool isActive = Player.UniqueCoins[i] == 1 && currentShiftCount == -i;
            coinDisplays[i].SetActive(isActive);
            coinInfos[i].SetActive(isActive);
            coinBlankDisplays[i].SetActive(!isActive && currentShiftCount == -i);
        }

        UndiscoveredInfo.SetActive(currentShiftCount < -coinDisplays.Length || currentShiftCount > 0);
    }

    public void MovePositive()
    {
        if (currentShiftCount < 0)
        {
            SnapToNearestValidPoint();
            isMoving = true;
            StartCoroutine(ShiftPosition(distance));
        }
    }

    public void MoveNegative()
    {
        if (currentShiftCount > -maxNegativeShifts)
        {
            SnapToNearestValidPoint();
            isMoving = true;
            StartCoroutine(ShiftPosition(-distance));
        }
    }

    private void SnapToNearestValidPoint()
    {
        float offsetX = transform.position.x - startingPosition.x;
        int nearestShiftCount = Mathf.RoundToInt(offsetX / distance);

        // If the position is above the starting position, reset to the starting position.
        if (offsetX > 0)
        {
            transform.position = startingPosition;
            currentShiftCount = 0;
        }
        // If it exceeds the maximum negative shifts, snap back to the 8th negative shift.
        else if (nearestShiftCount < -maxNegativeShifts)
        {
            transform.position = startingPosition + new Vector3(-maxNegativeShifts * distance, 0, 0);
            currentShiftCount = -maxNegativeShifts;
        }
        else
        {
            float nearestValidX = startingPosition.x + nearestShiftCount * distance;
            transform.position = new Vector3(nearestValidX, transform.position.y, transform.position.z);
            currentShiftCount = nearestShiftCount;
        }
    }

    IEnumerator ShiftPosition(float shiftAmount)
    {
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + new Vector3(shiftAmount, 0, 0);

        while (elapsedTime < shiftDuration)
        {
            float fraction = elapsedTime / shiftDuration;
            float sinFraction = Mathf.Sin(fraction * Mathf.PI / 2);
            transform.position = Vector3.Lerp(startPosition, targetPosition, sinFraction);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the object is exactly at the target position at the end
        transform.position = targetPosition;

        // After movement, snap to the nearest valid point and update currentShiftCount
        SnapToNearestValidPoint();

        isMoving = false;
    }
}