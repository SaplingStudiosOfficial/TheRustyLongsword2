// =============================================================
// ENCOUNTER STAGE
// =============================================================
//
// Promoted out of CrybtManager so that the HUD can describe
// what it is rendering without taking a dependency on the
// whole rules engine.
//
// Unity serializes enums as integers, so the existing
// currentStage value in the Crybt scene is unaffected by the
// move - but the ordering below must not change, or saved and
// scene-authored values would shift meaning.
// =============================================================

public enum EncounterStage
{
    StartingEncounter = 0,
    ChoosingOffering = 1,
    ChoosingHero = 2,
    ChoosingHeroFromGraveyard = 3,
    BuildingCombination = 4,
    ResolvingEncounter = 5,
    EndingEncounter = 6,
    EndingCrawl = 7,
    ChoosingBoons = 8,
    GameOver = 9
}
