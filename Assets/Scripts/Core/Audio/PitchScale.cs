using UnityEngine;

// =============================================================
// PITCH SCALE
// =============================================================
//
// Turns a count of scale steps into an AudioSource pitch.
//
// WHY THIS EXISTS:
//
// Pitch is logarithmic and the ear hears it that way. Adding a
// flat 0.1 to pitch on every play does not climb evenly - the
// first step sounds like a big jump and the tenth barely moves.
// Successive sounds that are meant to climb have to step by an
// INTERVAL, not by a number.
//
// One semitone multiplies pitch by the twelfth root of two, so
// twelve of them multiply it by exactly two: one octave.
//
// This file is deliberately pure. It has no state, touches no
// component, and every function is a plain input-to-output, so
// the maths can be read and checked on its own.
// =============================================================

public static class PitchScale
{
    // =========================================================
    // THE SCALES
    // =========================================================
    //
    // Semitone offsets from the root, one octave's worth. Step
    // past the end and Degree() carries into the next octave.
    //
    // These arrays are written once here and never assigned to
    // again. They are constants in everything but the keyword,
    // which C# does not offer for arrays.

    private static readonly int[] ChromaticDegrees =
        { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

    private static readonly int[] MajorDegrees =
        { 0, 2, 4, 5, 7, 9, 11 };

    private static readonly int[] NaturalMinorDegrees =
        { 0, 2, 3, 5, 7, 8, 10 };

    private static readonly int[] PentatonicMajorDegrees =
        { 0, 2, 4, 7, 9 };

    private static readonly int[] PentatonicMinorDegrees =
        { 0, 3, 5, 7, 10 };

    private static readonly int[] WholeToneDegrees =
        { 0, 2, 4, 6, 8, 10 };


    private const int SemitonesPerOctave = 12;


    // =========================================================
    // SEMITONES
    // =========================================================
    //
    // How far above the root the given step of a scale sits.
    //
    // Step 0 is the root. Steps past the top of the scale keep
    // climbing into the octave above; negative steps descend
    // into the octave below.

    public static int Semitones(MusicalScale scale, int step)
    {
        int[] degrees = DegreesOf(scale);
        int length = degrees.Length;

        // Floor division, so -1 lands on the LAST degree of the
        // octave below rather than on the root.
        int octave = Mathf.FloorToInt((float)step / length);
        int index = step - (octave * length);

        return degrees[index] + (octave * SemitonesPerOctave);
    }


    // =========================================================
    // PITCH FROM SEMITONES
    // =========================================================
    //
    // The whole point of the file: 2 to the power of semitones
    // over twelve. Zero semitones is pitch 1, twelve semitones
    // is pitch 2, minus twelve is pitch 0.5.

    public static float PitchFromSemitones(float semitones)
    {
        return Mathf.Pow(
            2f,
            semitones / SemitonesPerOctave
        );
    }


    // =========================================================
    // PITCH FROM STEP
    // =========================================================
    //
    // What a shaper's value means when the pitch unit is set to
    // musical steps.
    //
    // The value is rounded to a whole step first. Landing
    // between two notes of a scale is the thing this file
    // exists to prevent, so a fractional step is snapped rather
    // than honoured.

    public static float PitchFromStep(MusicalScale scale, float step)
    {
        int whole = Mathf.RoundToInt(step);

        return PitchFromSemitones(
            Semitones(scale, whole)
        );
    }


    // =========================================================
    // STEPS PER OCTAVE
    // =========================================================
    //
    // How many steps of this scale add up to one octave. Useful
    // when authoring a range that should span a fixed interval.

    public static int StepsPerOctave(MusicalScale scale)
    {
        return DegreesOf(scale).Length;
    }


    // =========================================================
    // INTERNALS
    // =========================================================

    private static int[] DegreesOf(MusicalScale scale)
    {
        switch (scale)
        {
            case MusicalScale.Major:
                return MajorDegrees;

            case MusicalScale.NaturalMinor:
                return NaturalMinorDegrees;

            case MusicalScale.PentatonicMajor:
                return PentatonicMajorDegrees;

            case MusicalScale.PentatonicMinor:
                return PentatonicMinorDegrees;

            case MusicalScale.WholeTone:
                return WholeToneDegrees;

            default:
                return ChromaticDegrees;
        }
    }
}
