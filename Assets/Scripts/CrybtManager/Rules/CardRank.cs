// =============================================================
// CARD RANK
// =============================================================
//
// The true rank of a card, used for Pairs and Runs.
//
// NOT the same as pip value: a King ranks 13 but is worth 10
// when totalling to Fifteen. Card keeps those separate on
// purpose - see CardValue.
//
//
// THE NUMBERS ARE THE SERIALISED DATA - DO NOT RENUMBER.
//
// Unity stores an enum field as its underlying integer, so the
// 53 card prefabs already on disk hold "rank: 1", "rank: 13"
// and so on. These members were chosen to match those values
// exactly, which is why swapping the field from int to this
// enum needed no migration and changed no YAML.
//
// Renumbering a member silently reassigns every card that
// carries that number. Adding a member is safe; changing one
// is not.
//
//
// None exists only so that a card with no rank authored (the
// card back) does not silently read as an Ace.
// =============================================================

public enum CardRank
{
    None = 0,

    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,

    Jack = 11,
    Queen = 12,
    King = 13
}
