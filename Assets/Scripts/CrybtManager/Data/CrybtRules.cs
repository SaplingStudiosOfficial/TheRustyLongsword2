using UnityEngine;

// =============================================================
// CRYBT RULES
// =============================================================
//
// Every Crybt number a designer might want to change, in one
// asset that can be edited without recompiling - and without
// opening a scene.
//
// Create via: Assets > Create > Crybt > Rules
//
// WHY AN ASSET RATHER THAN A STATIC CLASS:
//
// A const is baked into the build. You would still need a
// programmer and a recompile to change a boon price.
//
// With an asset you can duplicate it to make a Hard Mode
// ruleset, edit values while the game is playing and see the
// effect immediately, and review a balance change as a
// one-line diff in git.
//
// WHAT IS DELIBERATELY NOT HERE:
//
// Health, Modifier, and cardSpacing remain serialized on
// CrybtManager because they already hold values authored in
// the Crybt scene. Moving them here would silently discard
// those values.
//
// Structural facts (13 ranks per suit, face cards = 10) live
// in CardConstants instead - changing those would be a bug,
// not a tuning decision.
// =============================================================

[CreateAssetMenu(
    menuName = "Crybt/Rules",
    fileName = "CrybtRules")]
public class CrybtRules : ScriptableObject
{
    // =========================================================
    // CRAWL STRUCTURE
    // =========================================================

    [Header("Crawl Structure")]

    [SerializeField]
    [Tooltip("Encounters the player must survive before the Boon Shop opens.")]
    private int encountersPerCrawl = 4;

    [SerializeField]
    [Tooltip("Cards drawn into the Hand at the start of each encounter.")]
    private int handSize = 5;


    // =========================================================
    // COMBINATION SCORING
    // =========================================================

    [Header("Combination Scoring")]

    [SerializeField]
    [Tooltip("The pip total a selection must hit to score a Fifteen.")]
    private int fifteenTarget = 15;

    [SerializeField]
    private int fifteenPoints = 2;

    [SerializeField]
    [Tooltip("Points awarded per distinct pair within a matching-rank group.")]
    private int pointsPerPair = 2;

    [SerializeField]
    [Tooltip("Shortest selection that can score as a Run.")]
    private int minimumRunLength = 3;

    [SerializeField]
    [Tooltip("Smallest selection that can score as a Flush.")]
    private int minimumFlushSize = 4;

    [SerializeField]
    [Tooltip("Largest selection that can score as a Flush. A 6-card " +
             "same-suit selection scores nothing, which is existing " +
             "behaviour - raise this only if that is intended to change.")]
    private int maximumFlushSize = 5;


    // =========================================================
    // OFFERING / BOON SHOP
    // =========================================================

    [Header("Offering / Boon Shop")]

    [SerializeField]
    [Tooltip("Points per Fifteen found anywhere in the Offering pile.")]
    private int offeringFifteenPoints = 2;

    [SerializeField]
    [Tooltip("Points per matching-rank pair found in the Offering pile.")]
    private int offeringPairPoints = 2;

    [SerializeField]
    [Tooltip("Bonus Offering Points per card resting in the Graveyard.")]
    private int graveyardBonusPerCard = 1;

    [SerializeField]
    [Tooltip("Offering Points left unspent when the Boon Shop closes " +
             "are normally lost.")]
    private bool unspentPointsCarryOver = false;


    // =========================================================
    // READ-ONLY ACCESS
    // =========================================================
    //
    // The fields are private so only the Inspector can write
    // them. Gameplay code reads through these properties and
    // cannot accidentally mutate the balance data at runtime.

    public int EncountersPerCrawl => encountersPerCrawl;

    public int HandSize => handSize;

    public int FifteenTarget => fifteenTarget;

    public int FifteenPoints => fifteenPoints;

    public int PointsPerPair => pointsPerPair;

    public int MinimumRunLength => minimumRunLength;

    public int MinimumFlushSize => minimumFlushSize;

    public int MaximumFlushSize => maximumFlushSize;

    public int OfferingFifteenPoints => offeringFifteenPoints;

    public int OfferingPairPoints => offeringPairPoints;

    public int GraveyardBonusPerCard => graveyardBonusPerCard;

    public bool UnspentPointsCarryOver => unspentPointsCarryOver;
}
