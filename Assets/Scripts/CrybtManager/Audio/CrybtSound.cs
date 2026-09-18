// =============================================================
// CRYBT SOUND
// =============================================================
//
// Every sound the Crybt can make, named once.
//
// WHY THIS EXISTS:
//
// The alternative is twenty AudioClip fields on CrybtManager,
// which is how PlayerController ended up with four of them and
// two AudioSources. An enum keeps the manager saying WHAT
// happened and leaves WHICH clip to an asset.
//
// NOTE:
//
// Serialised as integers inside CrybtSoundTable, exactly like
// EncounterStage. Reordering these silently re-points every
// row of an authored table. Add new members at the END.
// =============================================================

public enum CrybtSound
{
    // =========================================================
    // CARDS
    // =========================================================

    // Mouse moves onto a card that can be clicked.
    CardHover = 0,

    // Card added to the combination being built.
    CardSelect = 1,

    // Card taken back out of it.
    CardDeselect = 2,

    // Cards dealt off the deck.
    CardDeal = 3,

    // Card committed - the Offering.
    CardPlay = 4,

    // Card sent to the Discard.
    CardDiscard = 5,

    // The deck is shuffled, including the reshuffle of the
    // Discard when the deck runs out.
    DeckShuffle = 6,


    // =========================================================
    // ENCOUNTER
    // =========================================================

    EncounterStart = 7,

    EncounterEnd = 8,

    // Torch reveals the Monster early.
    MonsterPeek = 9,

    MonsterReveal = 10,

    MonsterDefeated = 11,

    // A combination scored. The hit, not the tally.
    PlayerAttack = 12,

    PlayerHurt = 13,

    PlayerDeath = 14,

    // The tally. Climbs a scale across successive scoring
    // combinations and resets at the start of each encounter -
    // this is the sound the musical stepping was built for.
    ScoreTick = 15,


    // =========================================================
    // CRAWL AND SHOP
    // =========================================================

    CrawlComplete = 16,

    // Offering Points awarded at the end of a crawl.
    OfferingPointsEarned = 17,

    BoonPurchased = 18,

    BoonPhaseEnd = 19,


    // =========================================================
    // UI
    // =========================================================

    ButtonPress = 20,

    // Anything the game refuses: an empty confirm, a
    // combination that does not score, a boon that cannot be
    // afforded or has no effect yet.
    ActionRefused = 21,

    // The looping bed. Started once, never stopped.
    Ambience = 22
}
