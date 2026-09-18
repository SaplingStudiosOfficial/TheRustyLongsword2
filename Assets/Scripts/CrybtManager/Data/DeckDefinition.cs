using System;
using System.Collections.Generic;
using UnityEngine;

// =============================================================
// DECK DEFINITION
// =============================================================
//
// The whole deck as ONE asset: a table of rank / suit / face,
// plus the single prefab every card is built from.
//
// Create via: Assets > Create > Crybt > Deck
// Populate via: Tools > Crybt > Generate Deck From Prefabs
//
//
// WHY A TABLE INSTEAD OF 52 PREFAB VARIANTS:
//
// The 52 variants differ by exactly one thing - the sprite on
// their root Image. Everything else they override (position,
// rotation, size, name) is overwritten at runtime by MoveCards
// anyway. So the variants carry one row of data each, at the
// cost of a prefab each.
//
// Adding a card here is adding a ROW. No prefab, no variant
// chain, no scene object.
//
//
// WHY THE ROWS ARE GENERATED, NOT TYPED:
//
// The sprite sheet's names cannot be trusted as a lookup key:
// clubs are spelled "Slubs_*", "Ace_Hearts" is reversed
// relative to "Diamond_Ace"/"Spades_Ace", and thirteen
// non-card sprites (Table, Cave, Monster_1, buttons) share the
// same sheet. Deriving rank and suit from those names would
// need three special cases on day one and would break silently
// whenever art is renamed.
//
// The prefabs already pair rank + suit + sprite correctly, so
// the generator reads the mapping out of them once. After that
// the sheet's names never matter again.
// =============================================================

[Serializable]
public class DeckEntry
{
    [SerializeField]
    private CardRank rank = CardRank.Ace;

    [SerializeField]
    private CardSuit suit = CardSuit.Hearts;

    [SerializeField]
    [Tooltip("Face art. A sub-sprite of the card sprite sheet.")]
    private Sprite face;


    public CardRank Rank => rank;

    public CardSuit Suit => suit;

    public Sprite Face => face;


#if UNITY_EDITOR

    public void EditorInitialise(
        CardRank sourceRank,
        CardSuit sourceSuit,
        Sprite sourceFace)
    {
        rank = sourceRank;
        suit = sourceSuit;
        face = sourceFace;
    }

#endif
}


[CreateAssetMenu(
    menuName = "Crybt/Deck",
    fileName = "Deck")]
public class DeckDefinition : ScriptableObject
{
    [SerializeField]
    [Tooltip("The one prefab every card is built from. Its " +
             "rank, suit and face are overwritten per card.")]
    private GameObject cardPrefab;

    [SerializeField]
    [Tooltip("Face-down art. Not a playable card, so it is not " +
             "a row below.")]
    private Sprite cardBackSprite;

    [SerializeField]
    private List<DeckEntry> cards = new List<DeckEntry>();


    public GameObject CardPrefab => cardPrefab;

    public Sprite CardBackSprite => cardBackSprite;

    public IReadOnlyList<DeckEntry> Cards => cards;

    public int Count => cards == null ? 0 : cards.Count;


#if UNITY_EDITOR

    public void EditorSet(
        GameObject prefab,
        Sprite back,
        List<DeckEntry> entries)
    {
        cardPrefab = prefab;
        cardBackSprite = back;
        cards = entries;
    }

#endif
}
