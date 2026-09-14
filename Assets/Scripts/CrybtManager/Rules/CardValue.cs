// =============================================================
// CARD VALUE
// =============================================================
//
// A card reduced to just the three numbers scoring cares about.
//
// WHY THIS EXISTS:
//
// CrybtScoring used to operate on Card MonoBehaviours, which
// meant scoring could not run without a scene, a GameObject,
// and Play Mode.
//
// This struct has no Unity dependency at all, so the scoring
// rules can be called directly from a test, a replay, or a
// balance tool.
//
// It is a readonly struct so that passing a hand around does
// not allocate.
// =============================================================

public readonly struct CardValue
{
    // Numerical value used for Fifteens.
    // Face cards report 10 here.
    public readonly int PipValue;

    // True rank used for Pairs and Runs.
    // Ace = 1, Jack = 11, Queen = 12, King = 13.
    public readonly int Rank;

    // Suit identifier used for Flushes.
    public readonly int Suit;


    public CardValue(
        int pipValue,
        int rank,
        int suit)
    {
        PipValue = pipValue;
        Rank = rank;
        Suit = suit;
    }
}
