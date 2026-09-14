// =============================================================
// ENCOUNTER VIEW
// =============================================================
//
// A plain data snapshot of everything the HUD needs to draw
// one frame of the encounter.
//
// No Unity types, no references back into CrybtManager - so
// the HUD cannot reach into the rules engine, and the rules
// engine never touches a Button.
//
// readonly struct so passing it costs nothing.
// =============================================================

public readonly struct EncounterView
{
    public readonly EncounterStage Stage;


    // =========================================================
    // AVAILABLE ACTIONS
    // =========================================================
    //
    // Deliberately pre-computed by the rules engine rather
    // than re-derived by the HUD. The HUD should never need to
    // know WHY an action is available.

    public readonly bool CanSwitchHero;

    public readonly bool CanPeek;


    // =========================================================
    // PLAYER / CRAWL NUMBERS
    // =========================================================

    public readonly int Health;

    public readonly int Modifier;

    public readonly int EncounterScore;

    public readonly int EncounterNumber;

    public readonly int EncountersPerCrawl;

    public readonly int CrawlNumber;

    public readonly int OfferingPoints;


    // =========================================================
    // MONSTER ATTACK POWER
    // =========================================================
    //
    // null means "not yet revealed" and renders as "?".

    public readonly int? MonsterAttackPower;


    // =========================================================
    // COMBINATION PREVIEW
    // =========================================================
    //
    // What the cards currently selected WOULD score if the
    // player confirmed them right now. Recomputed on every
    // toggle.
    //
    // null means "nothing is selected", and the score readout
    // falls back to showing the running EncounterScore. So an
    // empty table still shows the encounter total, and the
    // moment a card is picked up the number becomes a preview
    // of that selection - including 0 for a combination that
    // does not score.
    //
    // This is a preview only. Nothing here is committed until
    // ConfirmCombination() succeeds.

    public readonly int? CombinationPreview;


    // =========================================================
    // COMBINATION BREAKDOWN
    // =========================================================
    //
    // Player-facing list of which scoring rules the current
    // selection satisfies, one per line:
    //
    //     Fifteen +2
    //     Pair +2
    //
    // Empty string when nothing is selected. Comes from the
    // same scoring pass as CombinationPreview, so the lines
    // always add up to the number shown.

    public readonly string CombinationBreakdown;


    public EncounterView(
        EncounterStage stage,
        bool canSwitchHero,
        bool canPeek,
        int health,
        int modifier,
        int encounterScore,
        int encounterNumber,
        int encountersPerCrawl,
        int crawlNumber,
        int offeringPoints,
        int? monsterAttackPower,
        int? combinationPreview,
        string combinationBreakdown)
    {
        CombinationPreview = combinationPreview;
        CombinationBreakdown = combinationBreakdown;
        Stage = stage;
        CanSwitchHero = canSwitchHero;
        CanPeek = canPeek;
        Health = health;
        Modifier = modifier;
        EncounterScore = encounterScore;
        EncounterNumber = encounterNumber;
        EncountersPerCrawl = encountersPerCrawl;
        CrawlNumber = crawlNumber;
        OfferingPoints = offeringPoints;
        MonsterAttackPower = monsterAttackPower;
    }
}
