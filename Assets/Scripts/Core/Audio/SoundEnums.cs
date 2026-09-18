// =============================================================
// SOUND ENUMS
// =============================================================
//
// The choices a sound offers, in one place.
//
// NOTE:
//
// Unity serialises an enum as its underlying int. Once a
// SoundDefinition asset or a SoundPlayer has been authored in
// the Inspector, REORDERING these silently changes what every
// existing asset means. Add new members at the END.
// =============================================================

// =============================================================
// VARIATION MODE
// =============================================================
//
// How a shaper picks the next value for pitch or volume.

public enum SoundVariationMode
{
    // Play the sound exactly as authored.
    None = 0,

    // Anywhere between low and high, fresh every time.
    Random = 1,

    // Start at low and move by a fixed step each play.
    Step = 2
}


// =============================================================
// STEP WRAP
// =============================================================
//
// What a stepping shaper does when it runs off the end.

public enum SoundStepWrap
{
    // Jump back to the other end and keep going the same way.
    Loop = 0,

    // Turn around and walk back.
    PingPong = 1
}


// =============================================================
// PITCH UNIT
// =============================================================
//
// Pitch is not linear. Stepping it by a fixed number does not
// step it by a fixed musical interval, which is why anything
// meant to climb a scale has to say so.

public enum SoundPitchUnit
{
    // Low, high and step are raw playback-rate multipliers.
    Multiplier = 0,

    // Low, high and step are counted in steps of a scale.
    MusicalSteps = 1
}


// =============================================================
// MUSICAL SCALE
// =============================================================
//
// Which notes a musical progression is allowed to land on.

public enum MusicalScale
{
    // Every semitone.
    Chromatic = 0,

    Major = 1,

    NaturalMinor = 2,

    PentatonicMajor = 3,

    PentatonicMinor = 4,

    WholeTone = 5
}


// =============================================================
// CLIP PICK MODE
// =============================================================
//
// How a sound with more than one clip chooses between them.

public enum SoundClipPickMode
{
    // Cycle through the list and start again.
    InOrder = 0,

    // Anywhere in the list, repeats included.
    Random = 1,

    // Anywhere in the list except the one just played.
    RandomNoRepeat = 2
}


// =============================================================
// PLAYBACK MODE
// =============================================================

public enum SoundPlaybackMode
{
    // Fire and forget. Overlaps rather than cutting itself off.
    OneShot = 0,

    // Runs until something stops it.
    Loop = 1
}


// =============================================================
// MIXER ROUTING
// =============================================================

public enum SoundMixerRouting
{
    // Use whatever group the AudioSource is already set to.
    Inherit = 0,

    // Use the group named on the sound instead.
    Override = 1
}
