using UnityEngine;
using UnityEngine.Audio;

// =============================================================
// MIXER VOLUME
// =============================================================
//
// Converts a plain 0-to-1 volume into the decibels an exposed
// mixer parameter actually wants, and writes it.
//
// WHY THIS EXISTS:
//
// An AudioSource's volume field is linear, so a slider can be
// wired straight to it. An exposed mixer parameter is NOT - it
// is decibels, running from -80 (silent) to 0 (unchanged).
// Assigning a 0-to-1 slider value to one gives a fader that
// does almost nothing until the last hair of its travel.
//
// The conversion is 20 times the base-10 log. It has one trap:
// log of zero is undefined, so a slider dragged fully down has
// to be special-cased to -80 rather than fed to the maths.
//
// MasterAudioManager already does exactly this for the
// WorldSound mixer's MasterVolume parameter. That script is
// frozen and keeps owning that parameter; this file exists so
// new code agrees with it instead of inventing a second curve.
//
// NOTE:
//
// Writing an exposed parameter from code takes it away from
// mixer snapshots permanently - from the first SetFloat on, a
// snapshot transition will no longer move it.
// =============================================================

public static class MixerVolume
{
    // Unity's exposed volume parameters bottom out here.
    public const float SilenceDecibels = -80f;

    // Below this a linear volume is treated as silence rather
    // than sent through a log that cannot represent it.
    private const float SilenceThreshold = 0.0001f;


    // =========================================================
    // LINEAR TO DECIBELS
    // =========================================================
    //
    // 0 becomes -80. 1 becomes 0. 0.5 becomes about -6, which
    // is the half-as-loud the ear expects.

    public static float LinearToDecibels(float linear)
    {
        if (linear <= SilenceThreshold)
        {
            return SilenceDecibels;
        }

        return Mathf.Log10(Mathf.Clamp01(linear)) * 20f;
    }


    // =========================================================
    // DECIBELS TO LINEAR
    // =========================================================
    //
    // The way back, for pointing a slider at whatever the mixer
    // currently holds.

    public static float DecibelsToLinear(float decibels)
    {
        if (decibels <= SilenceDecibels)
        {
            return 0f;
        }

        return Mathf.Clamp01(
            Mathf.Pow(10f, decibels / 20f)
        );
    }


    // =========================================================
    // SET
    // =========================================================
    //
    // Write a 0-to-1 volume to an exposed parameter. Returns
    // false, with one warning, when the parameter is not
    // actually exposed on that mixer - a typo here is otherwise
    // completely silent.

    public static bool Set(
        AudioMixer mixer,
        string exposedParameter,
        float linear)
    {
        if (mixer == null)
        {
            GameLog.Warning(
                "MixerVolume.Set was given no mixer."
            );

            return false;
        }

        if (string.IsNullOrEmpty(exposedParameter))
        {
            GameLog.Warning(
                "MixerVolume.Set was given no parameter name for "
                + "mixer '" + mixer.name + "'."
            );

            return false;
        }

        bool written =
            mixer.SetFloat(
                exposedParameter,
                LinearToDecibels(linear)
            );

        if (!written)
        {
            GameLog.Warning(
                "Mixer '" + mixer.name + "' has no exposed "
                + "parameter called '" + exposedParameter
                + "'. Expose it on the group in the Audio Mixer "
                + "window, or fix the name."
            );
        }

        return written;
    }


    // =========================================================
    // GET
    // =========================================================
    //
    // Reads an exposed parameter back as a 0-to-1 volume.
    // Returns the fallback when the parameter is not exposed.

    public static float Get(
        AudioMixer mixer,
        string exposedParameter,
        float fallback = 1f)
    {
        if (mixer == null || string.IsNullOrEmpty(exposedParameter))
        {
            return fallback;
        }

        if (!mixer.GetFloat(exposedParameter, out float decibels))
        {
            return fallback;
        }

        return DecibelsToLinear(decibels);
    }
}
