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
        // Shared with the card movement so the whole scene
        // opens with the same feel.
        float easedT =
            Easing.QuadInOut(t);

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