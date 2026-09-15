// =============================================================
// SCORE ITEM
// =============================================================
//
// One rule that fired while scoring a combination, and what it
// paid.
//
// Produced by CrybtScoring.DescribeCombination. Deliberately
// has no UnityEngine reference and no formatting opinion - how
// it is shown is the HUD's business, not the rules engine's.
//
// readonly struct so building a breakdown costs no garbage
// beyond the list itself.
// =============================================================

public readonly struct ScoreItem
{
    // What fired, already player-facing:
    // "Fifteen", "Pair", "Run of 3", "4-Card Flush".
    public readonly string Name;

    // What it paid, in points.
    public readonly int Points;


    public ScoreItem(
        string name,
        int points)
    {
        Name = name;
        Points = points;
    }


    public override string ToString()
    {
        return Name
            + " +"
            + Points;
    }
}
