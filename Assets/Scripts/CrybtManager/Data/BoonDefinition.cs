using UnityEngine;

// =============================================================
// BOON DEFINITION
// =============================================================
//
// One asset per boon. Replaces six near-identical BuyBoonN()
// methods and twelve loose bool fields.
//
// Create via: Assets > Create > Crybt > Boon
//
// Adding a seventh boon is now: create an asset, drop it in
// the BoonShop list, and add the one place that reads
// BoonShop.IsActive(...). No edits to six scattered blocks.
// =============================================================

[CreateAssetMenu(
    menuName = "Crybt/Boon",
    fileName = "Boon")]
public class BoonDefinition : ScriptableObject
{
    [SerializeField]
    [Tooltip("Stable identity. Must be unique across all boon assets.")]
    private BoonId id = BoonId.Torch;

    [SerializeField]
    [Tooltip("Offering Points required to purchase this boon.")]
    private int cost = 2;

    [SerializeField]
    private string displayName = "Torch";

    [SerializeField]
    [TextArea]
    private string rulesText = "";


    // =========================================================
    // IMPLEMENTED
    // =========================================================
    //
    // WHY THIS FLAG EXISTS:
    //
    // Boons 2-6 were purchasable but had no gameplay effect -
    // their flags were set, reset, and never read once. A
    // player could spend up to 12 Offering Points for nothing.
    //
    // Marking a boon as not implemented lets the shop disable
    // its button instead of taking the player's points, and
    // makes the gap visible in the Inspector rather than
    // invisible in code.
    //
    // Set this true as each boon's effect is written.

    [SerializeField]
    [Tooltip("Untick until this boon's effect is actually " +
             "wired into gameplay. Unticked boons cannot be bought.")]
    private bool implemented = false;


    public BoonId Id => id;

    public int Cost => cost;

    public string DisplayName => displayName;

    public string RulesText => rulesText;

    public bool Implemented => implemented;


#if UNITY_EDITOR

    public void EditorInitialise(
        BoonId boonId,
        int boonCost,
        string name,
        bool isImplemented)
    {
        id = boonId;
        cost = boonCost;
        displayName = name;
        implemented = isImplemented;
    }

#endif
}
