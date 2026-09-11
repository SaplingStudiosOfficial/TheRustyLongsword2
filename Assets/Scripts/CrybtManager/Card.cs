using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    // =========================================================
    // CARD DATA
    // =========================================================

    [SerializeField] private int value;
    [SerializeField] private int rank;
    [SerializeField] private int suit;


    // =========================================================
    // UI
    // =========================================================

    [SerializeField] private Button button;


    // =========================================================
    // HOVER / SELECTION
    // =========================================================

    [SerializeField] private float hoverDistance = 25f;

    private bool active = false;
    private bool selected = false;
    private bool hovered = false;


    // =========================================================
    // MOVEMENT
    // =========================================================

    // How long a normal card movement takes.
    [SerializeField] private float moveDuration = 0.25f;

    // Hovering should generally feel a little faster
    // than moving between card locations.
    [SerializeField] private float hoverDuration = 0.12f;

    private Vector3 basePosition;

    private Vector3 movementStartPosition;
    private Vector3 movementTargetPosition;

    private float movementTimer = 0f;
    private float currentMoveDuration = 0.25f;

    private bool isMoving = false;


    // =========================================================
    // REFERENCES
    // =========================================================

    private CrybtManager manager;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        manager =
            FindObjectOfType<CrybtManager>();


        basePosition =
            transform.position;


        movementTargetPosition =
            basePosition;


        if (button != null)
        {
            button.interactable = false;

            button.onClick.AddListener(
                CardClicked
            );
        }
        else
        {
            Debug.LogWarning(
                gameObject.name +
                " does not have a Button assigned."
            );
        }
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (!isMoving)
        {
            return;
        }


        movementTimer +=
            Time.deltaTime;


        float t =
            movementTimer /
            currentMoveDuration;


        t = Mathf.Clamp01(t);


        // =====================================================
        // QUADRATIC EASE IN / OUT
        // =====================================================
        //
        // First half:
        // Accelerates toward the destination.
        //
        // Second half:
        // Decelerates into the destination.
        //
        // This creates the parabolic-style acceleration
        // instead of moving at a constant speed.

        float easedT;


        if (t < 0.5f)
        {
            easedT =
                2f * t * t;
        }
        else
        {
            easedT =
                1f -
                Mathf.Pow(
                    -2f * t + 2f,
                    2f
                ) / 2f;
        }


        transform.position =
            Vector3.LerpUnclamped(
                movementStartPosition,
                movementTargetPosition,
                easedT
            );


        // =====================================================
        // FINISH MOVEMENT
        // =====================================================

        if (t >= 1f)
        {
            transform.position =
                movementTargetPosition;


            isMoving = false;
        }
    }


    // =========================================================
    // ACTIVATION
    // =========================================================

    public void ActivateCard()
    {
        active = true;


        if (button != null)
        {
            button.interactable = true;
        }
    }


    public void DeactivateCard()
    {
        active = false;

        selected = false;

        hovered = false;


        if (button != null)
        {
            button.interactable = false;
        }


        MoveToBasePosition();
    }


    // =========================================================
    // CLICKING
    // =========================================================

    private void CardClicked()
    {
        if (!active)
        {
            return;
        }


        if (manager == null)
        {
            Debug.LogWarning(
                "Card could not find CrybtManager."
            );

            return;
        }


        manager.CardClicked(
            this
        );
    }


    // =========================================================
    // SELECTION
    // =========================================================

    public void SelectCard()
    {
        selected = true;


        MoveToRaisedPosition();
    }


    public void DeselectCard()
    {
        selected = false;


        // If the mouse is still over the card,
        // leave it raised.
        if (hovered && active)
        {
            MoveToRaisedPosition();
        }
        else
        {
            MoveToBasePosition();
        }
    }


    public bool IsSelected()
    {
        return selected;
    }


    // =========================================================
    // HOVERING
    // =========================================================

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        if (!active)
        {
            return;
        }


        hovered = true;


        // A selected card is already raised.
        if (!selected)
        {
            MoveToRaisedPosition();
        }
    }


    public void OnPointerExit(
        PointerEventData eventData)
    {
        if (!active)
        {
            return;
        }


        hovered = false;


        // Selected cards remain raised.
        if (!selected)
        {
            MoveToBasePosition();
        }
    }


    // =========================================================
    // RAISED POSITION
    // =========================================================

    private Vector3 GetRaisedPosition()
    {
        return
            basePosition
            + new Vector3(
                0f,
                hoverDistance,
                0f
            );
    }


    // =========================================================
    // MOVE TO RAISED POSITION
    // =========================================================

    private void MoveToRaisedPosition()
    {
        StartMovement(
            GetRaisedPosition(),
            hoverDuration
        );
    }


    // =========================================================
    // MOVE TO BASE POSITION
    // =========================================================

    private void MoveToBasePosition()
    {
        StartMovement(
            basePosition,
            hoverDuration
        );
    }


    // =========================================================
    // SET BASE POSITION
    // =========================================================

    public void SetBasePosition(
        Vector3 newPosition)
    {
        basePosition =
            newPosition;


        // =====================================================
        // SELECTED CARD
        // =====================================================
        //
        // If the manager rearranges the hand while this
        // card is selected, it moves toward its new hand
        // position while remaining raised.

        if (selected)
        {
            StartMovement(
                GetRaisedPosition(),
                moveDuration
            );

            return;
        }


        // =====================================================
        // HOVERED CARD
        // =====================================================

        if (hovered && active)
        {
            StartMovement(
                GetRaisedPosition(),
                moveDuration
            );

            return;
        }


        // =====================================================
        // NORMAL CARD
        // =====================================================

        StartMovement(
            basePosition,
            moveDuration
        );
    }


    // =========================================================
    // START MOVEMENT
    // =========================================================

    private void StartMovement(
        Vector3 targetPosition,
        float duration)
    {
        // Start from wherever the card currently is.
        //
        // This is important because if a new movement
        // begins before the old one finishes, the card
        // continues smoothly from its current location.

        movementStartPosition =
            transform.position;


        movementTargetPosition =
            targetPosition;


        movementTimer =
            0f;


        currentMoveDuration =
            Mathf.Max(
                0.01f,
                duration
            );


        isMoving =
            true;
    }


    // =========================================================
    // CARD INFORMATION
    // =========================================================

    public int GetValue()
    {
        // Jack, Queen, and King count as 10
        // when calculating numerical card values.

        if (value >= 11 &&
            value <= 13)
        {
            return 10;
        }


        return value;
    }


    public int GetRank()
    {
        // Actual card rank:
        //
        // Ace   = 1
        // 2-10  = 2-10
        // Jack  = 11
        // Queen = 12
        // King  = 13

        return rank;
    }


    public int GetSuit()
    {
        return suit;
    }
}