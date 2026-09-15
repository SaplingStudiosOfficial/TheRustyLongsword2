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
    //
    // MIGRATION IN PROGRESS - see CardDefinition.
    //
    // When a definition asset is assigned it wins. When it is
    // not, these serialized integers are used exactly as
    // before, so every existing card prefab keeps working
    // untouched.

    [SerializeField]
    [Tooltip("Optional. When assigned, this asset supplies the " +
             "card's rank and suit and the integers below are " +
             "ignored.")]
    private CardDefinition definition;

    [SerializeField] private int value;

    // rank/suit are enums, but Unity serialises an enum as its
    // underlying int - so these still read the SAME "rank: 1"
    // and "suit: 3" already authored in the 53 card prefabs.
    // Retyping them needed no migration and rewrote no YAML.
    [SerializeField] private CardRank rank;
    [SerializeField] private CardSuit suit;


    // =========================================================
    // UI
    // =========================================================

    [SerializeField] private Button button;

    [SerializeField]
    [Tooltip("The card's face. Left empty, the Image on this " +
             "object is used - which is where the prefab " +
             "variants put it.")]
    private Image faceImage;


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
    //
    // Pushed in by the manager via Bind(). The card no longer
    // goes looking for it - see ICardClickHandler.

    private ICardClickHandler handler;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
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
            GameLog.Warning(
                gameObject.name +
                " does not have a Button assigned."
            );
        }
    }


    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(
                CardClicked
            );
        }
    }


    // =========================================================
    // BIND
    // =========================================================
    //
    // Called once by whatever is running the table.

    public void Bind(ICardClickHandler clickHandler)
    {
        handler = clickHandler;
    }


    // =========================================================
    // BIND DEFINITION
    // =========================================================
    //
    // Used when the deck is built at runtime from
    // CardDefinition assets rather than from prefab variants.

    public void Bind(CardDefinition cardDefinition)
    {
        definition = cardDefinition;

        if (cardDefinition != null)
        {
            ApplyFace(cardDefinition.Face);
        }
    }


    // =========================================================
    // BIND DECK ROW
    // =========================================================
    //
    // Used when the deck is built at runtime from a
    // DeckDefinition, so a card needs no asset of its own.
    //
    // Writes the serialized fields directly rather than going
    // through a definition, which is what lets one prefab
    // become any card.

    public void Bind(
        CardRank cardRank,
        CardSuit cardSuit,
        Sprite face)
    {
        definition = null;

        rank = cardRank;
        suit = cardSuit;

        // GetValue() reads this, so it has to be kept in step
        // with the rank or every face card scores as 0.
        value = CardConstants.PipValueFor(cardRank);

        ApplyFace(face);
    }


    private void ApplyFace(Sprite face)
    {
        if (face == null)
        {
            return;
        }

        if (faceImage == null)
        {
            faceImage = GetComponent<Image>();
        }

        if (faceImage != null)
        {
            faceImage.sprite = face;
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


        // Quadratic ease in / out.
        // Shared with the card rotation animation so the two
        // always feel like the same movement.
        float easedT =
            Easing.QuadInOut(t);


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


        if (handler == null)
        {
            GameLog.Warning(
                gameObject.name
                + " was clicked but nothing has called Bind() "
                + "on it. Is this card in one of CrybtManager's "
                + "card lists?"
            );

            return;
        }


        handler.CardClicked(
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
        if (definition != null)
        {
            return definition.PipValue;
        }


        // Jack, Queen, and King count as 10
        // when calculating numerical card values.
        //
        // Defensive only: the prefabs already author face cards
        // with value 10, so this clamp has never fired. It is
        // kept because nothing enforces that.
        if (value >= (int)CardConstants.LowestFaceRank &&
            value <= CardConstants.RanksPerSuit)
        {
            return CardConstants.FaceCardValue;
        }


        return value;
    }


    public CardRank GetRank()
    {
        return definition != null
            ? definition.Rank
            : rank;
    }


    public CardSuit GetSuit()
    {
        return definition != null
            ? definition.Suit
            : suit;
    }


    // =========================================================
    // AS CARD VALUE
    // =========================================================
    //
    // The plain data form the scoring rules operate on.

    public CardValue ToCardValue()
    {
        return new CardValue(
            GetValue(),
            GetRank(),
            GetSuit()
        );
    }


#if UNITY_EDITOR

    // =========================================================
    // EDITOR ACCESS
    // =========================================================
    //
    // Used by the CardDefinition generator so it can read the
    // values baked into each prefab variant. Editor-only.

    public int EditorRawValue => value;

    public CardRank EditorRawRank => rank;

    public CardSuit EditorRawSuit => suit;

#endif
}
