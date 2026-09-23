using UnityEngine;
using UnityEngine.Audio;

// =============================================================
// SOUND SETTINGS
// =============================================================
//
// Everything that describes a sound: which clip, how loud, what
// pitch, looping or not, which mixer group, how it fades.
//
// WHY THIS IS ITS OWN CLASS:
//
// The same block of settings is needed in two places - inline
// on a SoundPlayer for a one-off, and inside a SoundDefinition
// asset for a sound used in twenty places. Writing the fields
// twice is how the two drift apart, so they are written once
// here and both hosts hold one of these.
//
// It holds NO progression state. See SoundShaper for why.
// =============================================================

[System.Serializable]
public class SoundSettings
{
    // =========================================================
    // CLIPS
    // =========================================================

    [SerializeField]
    [Tooltip("One clip, or several to vary between.")]
    private AudioClip[] clips = new AudioClip[0];

    [SerializeField]
    [Tooltip("How the next clip is chosen when there is more " +
             "than one. RandomNoRepeat never plays the same " +
             "clip twice running.")]
    private SoundClipPickMode pickMode =
        SoundClipPickMode.RandomNoRepeat;


    // =========================================================
    // PLAYBACK
    // =========================================================

    [SerializeField]
    [Tooltip("OneShot overlaps rather than cutting itself off. " +
             "Loop runs until something stops it.")]
    private SoundPlaybackMode playback = SoundPlaybackMode.OneShot;

    [SerializeField]
    [Tooltip("Keep playing while the game is paused. Tick this " +
             "for menu and UI sounds.")]
    private bool ignoreListenerPause = false;


    // =========================================================
    // VOLUME
    // =========================================================

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("The sound's own level, before any variation.")]
    private float volume = 1f;

    [SerializeField]
    [Tooltip("Leave Mode at None for a sound that should never " +
             "vary in level.")]
    private SoundShaper volumeShaper = new SoundShaper();


    // =========================================================
    // PITCH
    // =========================================================
    //
    // WHY THERE IS A UNIT HERE:
    //
    // Pitch is logarithmic. A shaper stepping by a flat 0.1 does
    // not climb evenly, and never lands on a note. Switching the
    // unit to MusicalSteps makes the shaper's low, high and step
    // count steps of a scale instead - so successive sounds walk
    // up an actual scale. See PitchScale.

    [SerializeField]
    [Tooltip("The sound's own pitch, before any variation. A " +
             "multiplier: 1 is the clip as recorded.")]
    private float pitch = 1f;

    [SerializeField]
    [Tooltip("Multiplier treats the shaper's numbers as raw " +
             "playback rate. MusicalSteps treats them as steps " +
             "of the scale below.")]
    private SoundPitchUnit pitchUnit = SoundPitchUnit.Multiplier;

    [SerializeField]
    [Tooltip("Which notes a musical progression may land on. " +
             "Only used when Pitch Unit is MusicalSteps.")]
    private MusicalScale scale = MusicalScale.Chromatic;

    [SerializeField]
    [Tooltip("Leave Mode at None for a sound that should never " +
             "vary in pitch.")]
    private SoundShaper pitchShaper = new SoundShaper();

    [SerializeField]
    [Tooltip("Seconds of silence after which a stepping " +
             "progression starts again from the bottom. Zero " +
             "means it never resets on its own.")]
    private float progressionResetSeconds = 0f;


    // =========================================================
    // MIXER
    // =========================================================

    [SerializeField]
    [Tooltip("Inherit uses whatever group the AudioSource is " +
             "already set to, and changes nothing. Override " +
             "sends this sound to the group below instead.")]
    private SoundMixerRouting routing = SoundMixerRouting.Inherit;

    [SerializeField]
    [Tooltip("Only used when Routing is Override.")]
    private AudioMixerGroup outputGroup;


    // =========================================================
    // FADES
    // =========================================================

    [SerializeField]
    [Tooltip("Seconds to ramp up from silence when the sound " +
             "starts. Zero starts at full level.")]
    private float fadeInSeconds = 0f;

    [SerializeField]
    [Tooltip("Seconds to ramp down when the sound is stopped. " +
             "Zero cuts immediately.")]
    private float fadeOutSeconds = 0f;


    public AudioClip[] Clips => clips;

    public SoundClipPickMode PickMode => pickMode;

    public SoundPlaybackMode Playback => playback;

    public bool Loops => playback == SoundPlaybackMode.Loop;

    public bool IgnoreListenerPause => ignoreListenerPause;

    public float Volume => volume;

    public SoundShaper VolumeShaper => volumeShaper;

    public float Pitch => pitch;

    public SoundPitchUnit PitchUnit => pitchUnit;

    public MusicalScale Scale => scale;

    public SoundShaper PitchShaper => pitchShaper;

    public float ProgressionResetSeconds => progressionResetSeconds;

    public SoundMixerRouting Routing => routing;

    public AudioMixerGroup OutputGroup => outputGroup;

    public float FadeInSeconds => fadeInSeconds;

    public float FadeOutSeconds => fadeOutSeconds;

    public bool HasClips => clips != null && clips.Length > 0;


    // =========================================================
    // RESOLVED PITCH
    // =========================================================
    //
    // Turns a shaper value into a pitch the AudioSource will
    // accept.
    //
    // Two clamps matter here:
    //
    // Unity's pitch range is -3 to 3, and anything outside it is
    // clamped by the engine anyway.
    //
    // A pitch of zero never finishes - the clip sits at sample
    // nought forever, holding a voice that nothing ever frees.
    // The magnitude floor below is what stops a mistyped range
    // from leaking voices.

    public const float MinPitchMagnitude = 0.01f;
    public const float MaxPitchMagnitude = 3f;


    public float ResolvePitch(float travel)
    {
        float raw =
            pitchUnit == SoundPitchUnit.MusicalSteps
                ? ResolveMusicalPitch(travel)
                : pitchShaper.Evaluate(pitch, travel);

        return ClampPitch(raw);
    }


    private float ResolveMusicalPitch(float travel)
    {
        if (!pitchShaper.Varies)
        {
            return pitch;
        }

        float steps = pitchShaper.Evaluate(0f, travel);

        // The authored pitch stays in play as the root the
        // scale climbs from, so a sound can sit a little low
        // and still step correctly.
        return pitch * PitchScale.PitchFromStep(scale, steps);
    }


    public static float ClampPitch(float value)
    {
        float magnitude = Mathf.Abs(value);

        if (magnitude < MinPitchMagnitude)
        {
            return value < 0f
                ? -MinPitchMagnitude
                : MinPitchMagnitude;
        }

        if (magnitude > MaxPitchMagnitude)
        {
            return value < 0f
                ? -MaxPitchMagnitude
                : MaxPitchMagnitude;
        }

        return value;
    }


    // =========================================================
    // RESOLVED VOLUME
    // =========================================================

    public float ResolveVolume(float travel)
    {
        return Mathf.Clamp01(
            volumeShaper.Evaluate(volume, travel)
        );
    }


#if UNITY_EDITOR

    // =========================================================
    // EDITOR SETUP
    // =========================================================
    //
    // For tools that build sound assets - see
    // CrybtAudioGenerator. Split into small pieces rather than
    // one fourteen-argument call, so a generator table reads
    // as what it changes rather than as a row of magic numbers.

    public void EditorInitialise(
        AudioClip clip,
        SoundPlaybackMode playbackMode,
        float baseVolume,
        AudioMixerGroup group)
    {
        clips = clip != null
            ? new[] { clip }
            : new AudioClip[0];

        playback = playbackMode;
        volume = baseVolume;

        if (group != null)
        {
            routing = SoundMixerRouting.Override;
            outputGroup = group;
        }
        else
        {
            routing = SoundMixerRouting.Inherit;
            outputGroup = null;
        }
    }


    public void EditorSetRandomPitch(float low, float high)
    {
        pitchUnit = SoundPitchUnit.Multiplier;
        pitchShaper.EditorSetRandom(low, high);
    }


    public void EditorSetMusicalStep(
        MusicalScale musicalScale,
        float low,
        float high,
        float step,
        SoundStepWrap wrap,
        float resetSeconds)
    {
        pitchUnit = SoundPitchUnit.MusicalSteps;
        scale = musicalScale;
        progressionResetSeconds = resetSeconds;

        pitchShaper.EditorSetStep(
            low,
            high,
            step,
            wrap
        );
    }


    // The note a musical progression climbs FROM. Below 1 the
    // clip starts lower than recorded, which is what leaves
    // room above it for the run to rise into.
    public void EditorSetBasePitch(float value)
    {
        pitch = ClampPitch(value);
    }


    // Back to a plain, unvaried sound. A tool that rewrites an
    // existing asset has to start from here, or settings the
    // new spec does not mention survive from the old one.
    public void EditorResetVariation()
    {
        pitch = 1f;
        pitchUnit = SoundPitchUnit.Multiplier;
        scale = MusicalScale.Chromatic;
        progressionResetSeconds = 0f;

        pitchShaper.EditorSetNone();
        volumeShaper.EditorSetNone();

        fadeInSeconds = 0f;
        fadeOutSeconds = 0f;
        ignoreListenerPause = false;
    }


    public void EditorSetFades(float fadeIn, float fadeOut)
    {
        fadeInSeconds = fadeIn;
        fadeOutSeconds = fadeOut;
    }


    public void EditorSetIgnoreListenerPause(bool value)
    {
        ignoreListenerPause = value;
    }

#endif
}
