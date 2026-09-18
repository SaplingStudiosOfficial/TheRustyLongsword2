using UnityEngine;

// =============================================================
// SOUND DEFINITION
// =============================================================
//
// One asset per sound. Author it once, use it everywhere.
//
// Create via: Assets > Create > Audio > Sound
//
// WHY THIS EXISTS:
//
// The Crybt alone has 31 clips waiting in
// Assets/AudioLines/CryptAudio. Filling in the same handful of
// settings 31 times across 31 Inspectors is how those settings
// drift apart, and how "the card sounds" end up with four
// different pitch ranges between them.
//
// A SoundPlayer with a definition assigned uses the definition
// and ignores its own inline settings. With no definition it
// uses its own. Same arrangement as Card and CardDefinition.
//
// NOTE:
//
// This is tuning data, not saved state. In the Editor it keeps
// Play Mode edits; in a build it resets every launch. Nothing
// here is persisted, and nothing here should be.
//
// Progression state is deliberately NOT stored on the asset -
// several SoundPlayers can share one definition, and a cursor
// here would be shared with them. See SoundShaper.
// =============================================================

[CreateAssetMenu(
    menuName = "Audio/Sound",
    fileName = "Sound")]
public class SoundDefinition : ScriptableObject
{
    [SerializeField]
    [Tooltip("What this sound is for. Notes only - nothing " +
             "reads this at runtime.")]
    [TextArea]
    private string description = "";

    [SerializeField]
    private SoundSettings settings = new SoundSettings();


    public SoundSettings Settings => settings;

    public string Description => description;
}
