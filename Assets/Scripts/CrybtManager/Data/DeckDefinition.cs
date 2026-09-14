using System.Collections.Generic;
using UnityEngine;

// =============================================================
// DECK DEFINITION
// =============================================================
//
// The composition of a deck, as an asset.
//
// Create via: Assets > Create > Crybt > Deck
//
// Once step 4 of the card migration lands, a second deck - a
// stripped test deck, a "cursed cards" variant, a Hard Mode
// deck - is a new asset rather than a new scene.
// =============================================================

[CreateAssetMenu(
    menuName = "Crybt/Deck",
    fileName = "Deck")]
public class DeckDefinition : ScriptableObject
{
    [SerializeField]
    private List<CardDefinition> cards = new List<CardDefinition>();

    public IReadOnlyList<CardDefinition> Cards => cards;

    public int Count => cards.Count;


#if UNITY_EDITOR

    public void EditorSetCards(List<CardDefinition> sourceCards)
    {
        cards = sourceCards;
    }

#endif
}
