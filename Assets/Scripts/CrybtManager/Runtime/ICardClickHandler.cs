// =============================================================
// I CARD CLICK HANDLER
// =============================================================
//
// What a Card needs from whatever is running the table.
//
// WHY THIS EXISTS:
//
// Card.Awake() used to call FindObjectOfType<CrybtManager>().
// With 53 cards in the scene that is 53 full scene scans at
// startup, and it welded every card to one concrete manager -
// so a card could not be driven by a test, a tutorial, or a
// replay.
//
// The manager now pushes itself down via Card.Bind() instead.
// =============================================================

public interface ICardClickHandler
{
    void CardClicked(Card clickedCard);
}
