// =============================================================
// CARD SUIT
// =============================================================
//
// Used for Flushes, and for nothing else - the Crybt has no
// trump suit and no suit ordering.
//
//
// THE NUMBERS ARE THE SERIALISED DATA - DO NOT RENUMBER.
//
// These values were read back off the card prefabs rather than
// invented, so the enum matches what is already on disk:
//
//   0  Hearts      1_Heart, 11_J_Heart, 12_Q_Heart, ...
//   1  Diamonds    1_Diamond Variant, ...
//   2  Clubs       1_Clubs Variant, ...
//   3  Spades      1_Spades Variant, ...
//   4  Back        Card_Back_Red Variant
//
// Renumbering a member re-suits every card holding that value.
//
//
// Back is a real value in the data, not a placeholder: the face
// -down card carries suit 4. It must never match anything in a
// flush, which it cannot, because no playable card shares it.
// =============================================================

public enum CardSuit
{
    Hearts = 0,
    Diamonds = 1,
    Clubs = 2,
    Spades = 3,

    Back = 4
}
