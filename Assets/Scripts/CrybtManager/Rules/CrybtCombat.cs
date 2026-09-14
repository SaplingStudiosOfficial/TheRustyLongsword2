// =============================================================
// CRYBT COMBAT
// =============================================================
//
// The encounter resolution formulas, extracted from the middle
// of CrybtManager.ResolveEncounter().
//
// Pure functions. No UnityEngine reference - Mathf.Max is
// deliberately avoided so this stays testable outside Unity.
// =============================================================

public static class CrybtCombat
{
    // =========================================================
    // MONSTER ATTACK POWER
    // =========================================================
    //
    // Monster AP = card pip value + the crawl's global
    // modifier, floored at zero.
    //
    // The modifier is passed in rather than read from a rules
    // asset because it is authored per-scene on CrybtManager.

    public static int MonsterAttackPower(
        int cardPipValue,
        int modifier)
    {
        int attackPower =
            cardPipValue
            + modifier;

        return attackPower < 0
            ? 0
            : attackPower;
    }


    // =========================================================
    // DAMAGE TAKEN
    // =========================================================
    //
    // Damage is whatever the player failed to cover.
    //
    // Exactly zero means the monster is DEFEATED, not merely
    // survived - see CrybtManager.DefeatMonster().

    public static int DamageTaken(
        int monsterAttackPower,
        int combinationPoints)
    {
        int damage =
            monsterAttackPower
            - combinationPoints;

        return damage < 0
            ? 0
            : damage;
    }


    // =========================================================
    // IS MONSTER DEFEATED
    // =========================================================
    //
    // Named so the rule reads the same way in code as it does
    // in the player's guide: matching the Monster's AP exactly
    // sends it to the Graveyard.

    public static bool IsMonsterDefeated(int damage)
    {
        return damage == 0;
    }
}
