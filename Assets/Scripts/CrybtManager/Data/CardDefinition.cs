using UnityEngine;

// =============================================================
// CARD DEFINITION
// =============================================================
//
// One asset per card. Replaces the three integers currently
// baked into each of the 53 card prefab variants.
//
// Create via: Assets > Create > Crybt > Card Definition
// Or generate all 53 at once via:
//     Tools > Crybt > Generate Card Definitions From Prefabs
//
//
// WHY THIS EXISTS:
//
// Today the deck is 53 prefab variants hand-placed in the
// Crybt scene and wired into a serialized List<Card>. That
// means the deck cannot change without editing a scene, there
// can only ever be one deck, and card data is unreviewable in
// a pull request because it lives in scene YAML.
//
//
// MIGRATION STATUS - READ BEFORE CHANGING THIS:
//
//   Step 1 (done) .... this asset type exists, and the editor
//                      tool generates the 53 assets FROM the
//                      existing prefabs so the data is
//                      provably identical.
//
//   Step 2 (done) .... Card.Bind(CardDefinition) exists and
//                      Card reads through properties. The
//                      serialized ints are still there and
//                      still win when no definition is set,
//                      so behaviour is unchanged.
//
//   Step 3 (todo) .... verify the game plays identically with
//                      definitions assigned.
//
//   Step 4 (todo) .... switch to instantiating one CardView
//                      prefab per DeckDefinition entry, then
//                      delete the 53 prefab variants.
//
// Steps 3 and 4 involve scene surgery and are deliberately
// NOT bundled into this change.
// =============================================================

[CreateAssetMenu(
    menuName = "Crybt/Card Definition",
    fileName = "Card")]
public class CardDefinition : ScriptableObject
{
    [SerializeField]
    [Tooltip("True rank. Ace = 1, Jack = 11, Queen = 12, King = 13.")]
    private int rank = 1;

    [SerializeField]
    [Tooltip("Suit identifier, matching the numbering already " +
             "used by the card prefabs.")]
    private int suit = 0;

    [SerializeField]
    [Tooltip("Optional face art, for when the deck is built at " +
             "runtime from a single CardView prefab.")]
    private Sprite face;


    public int Rank => rank;

    public int Suit => suit;

    public Sprite Face => face;


    // =========================================================
    // PIP VALUE
    // =========================================================
    //
    // Jack, Queen, and King count as 10 when calculating
    // numerical card values.
    //
    // The rule lives with the data it describes rather than as
    // a literal inside Card.GetValue().

    public int PipValue =>
        rank >= CardConstants.LowestFaceRank
            ? CardConstants.FaceCardValue
            : rank;


    // =========================================================
    // AS CARD VALUE
    // =========================================================

    public CardValue ToCardValue()
    {
        return new CardValue(
            PipValue,
            rank,
            suit
        );
    }


#if UNITY_EDITOR

    // =========================================================
    // EDITOR SEEDING
    // =========================================================
    //
    // Used only by the generator tool. Editor-only so that
    // nothing at runtime can write to card data.

    public void EditorInitialiseFrom(
        int sourceRank,
        int sourceSuit,
        Sprite sourceFace)
    {
        rank = sourceRank;
        suit = sourceSuit;
        face = sourceFace;
    }

#endif
}
