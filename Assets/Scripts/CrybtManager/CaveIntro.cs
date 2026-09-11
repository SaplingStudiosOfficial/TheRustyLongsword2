using UnityEngine;

public class CaveIntro : MonoBehaviour
{
    [SerializeField] private Transform Cave_Left;
    [SerializeField] private Transform Cave_Right;

    [SerializeField] private float moveDistance = 12f;
    [SerializeField] private float moveDuration = 1.25f;

    private Vector3 leftStartPosition;
    private Vector3 rightStartPosition;

    private Vector3 leftTargetPosition;
    private Vector3 rightTargetPosition;

    private float timer = 0f;
    private bool opening = false;

    private void Awake()
    {
        leftStartPosition = Cave_Left.position;
        rightStartPosition = Cave_Right.position;

        leftTargetPosition =
            leftStartPosition
            + Vector3.left * moveDistance;

        rightTargetPosition =
            rightStartPosition
            + Vector3.right * moveDistance;

        opening = true;
    }

    private void Update()
    {
        if (!opening)
        {
            return;
        }

        timer += Time.deltaTime;

        float t =
            Mathf.Clamp01(
                timer / moveDuration
            );

        // Smooth ease-in / ease-out.
        float easedT;

        if (t < 0.5f)
        {
            easedT =
                2f * t * t;
        }
        else
        {
            easedT =
                1f
                - Mathf.Pow(
                    -2f * t + 2f,
                    2f
                ) / 2f;
        }

        Cave_Left.position =
            Vector3.Lerp(
                leftStartPosition,
                leftTargetPosition,
                easedT
            );

        Cave_Right.position =
            Vector3.Lerp(
                rightStartPosition,
                rightTargetPosition,
                easedT
            );

        if (t >= 1f)
        {
            opening = false;
        }
    }
}