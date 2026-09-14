// =============================================================
// CARD CONSTANTS
// =============================================================
//
// Structural facts about a standard deck.
//
// These are NOT balance knobs. Changing RanksPerSuit would not
// be a tuning decision, it would be a bug - which is exactly
// the test for "const in a static class" rather than
// "field on a ScriptableObject".
//
// Tunable values live in CrybtRules instead.
// =============================================================

public static class CardConstants
{
    public const CardRank AceRank = CardRank.Ace;

    public const CardRank HighestRank = CardRank.King;

    public const int RanksPerSuit = 13;

    public const int SuitCount = 4;


    // =========================================================
    // FACE CARDS
    // =========================================================
    //
    // Jack, Queen, and King all count as 10 when calculating
    // numerical card values, but keep their true rank (11, 12,
    // 13) for pairs and runs.

    public const CardRank LowestFaceRank = CardRank.Jack;

    public const int FaceCardValue = 10;


    // =========================================================
    // PIP VALUE FOR
    // =========================================================
    //
    // The single place that knows a face card is worth 10.
    // Used by CardDefinition and by cards built at runtime from
    // a DeckDefinition row, so the two cannot disagree.

    public static int PipValueFor(CardRank rank)
    {
        return rank >= LowestFaceRank
            ? FaceCardValue
            : (int)rank;
    }
}
