// =============================================================
// POOL KEYS
// =============================================================
//
// Every category an ObjectPool can hold, named once.
//
// WHY THIS EXISTS:
//
// ObjectPool files its contents under a string. Loose string
// literals at the call site are a typo away from silently
// creating a second, empty category that never shares anything
// with the first.
//
// NOTE:
//
// These are compile-time constants, not state. The project
// rule forbids mutable state in a static class - a const
// string cannot be mutated, so this is not that.
// =============================================================

public static class PoolKeys
{
    // =========================================================
    // AUDIO
    // =========================================================
    //
    // Spare AudioSources borrowed one per overlapping one-shot,
    // so each shot keeps the pitch it was given. See SoundPlayer.

    public const string AudioVoice = "AudioVoice";
}
