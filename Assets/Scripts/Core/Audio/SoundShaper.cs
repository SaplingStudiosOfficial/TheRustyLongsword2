using UnityEngine;

// =============================================================
// SOUND SHAPER
// =============================================================
//
// Decides what number the next sound gets. The same shaper does
// pitch and volume, so the two behave identically.
//
// Three modes:
//
// None .... the sound plays exactly as authored.
// Random .. anywhere between low and high, fresh every time.
// Step .... starts at low and moves by a fixed step per play,
//           then either wraps round the range, turns around at
//           the ends, or stops at the top and stays there.
//
// WHY THIS HOLDS NO STATE:
//
// A shaper can live on a SoundDefinition, and a definition is a
// ScriptableObject shared by every component that uses it. A
// progression cursor stored here would be shared too, so two
// objects climbing at once would fight over one position.
//
// Instead the caller owns a single float - "travel", the
// distance walked so far - and hands it in. SoundPlayer keeps
// one per sound. The shaper stays pure: same inputs, same
// answer, every time.
// =============================================================

[System.Serializable]
public class SoundShaper
{
    [SerializeField]
    [Tooltip("None plays the sound as authored. Random picks " +
             "between Low and High. Step walks from Low to " +
             "High and back round.")]
    private SoundVariationMode mode = SoundVariationMode.None;

    [SerializeField]
    [Tooltip("Bottom of the range. Ignored when Mode is None.")]
    private float low = 1f;

    [SerializeField]
    [Tooltip("Top of the range. Ignored when Mode is None.")]
    private float high = 1f;

    [SerializeField]
    [Tooltip("How far each successive play moves. Negative " +
             "values descend. Step mode only.")]
    private float step = 1f;

    [SerializeField]
    [Tooltip("Loop jumps back to the far end and keeps going " +
             "the same way, and never lands ON High - set High " +
             "one step past the last value you want. PingPong " +
             "turns around at High. Clamp stops at High and " +
             "stays there. Step mode only.")]
    private SoundStepWrap wrap = SoundStepWrap.Loop;


    public SoundVariationMode Mode => mode;

    public bool Varies => mode != SoundVariationMode.None;

    public bool Steps => mode == SoundVariationMode.Step;

    public float Low => low;

    public float High => high;

    public float Step => step;


    // =========================================================
    // EVALUATE
    // =========================================================
    //
    // travel is the distance walked since the progression last
    // started. It is only read in Step mode; the other two
    // modes ignore it.
    //
    // A range of zero or less collapses to Low, which makes a
    // mistyped High harmless instead of silent.

    public float Evaluate(float authoredValue, float travel)
    {
        switch (mode)
        {
            case SoundVariationMode.Random:
                return Random.Range(
                    Mathf.Min(low, high),
                    Mathf.Max(low, high)
                );

            case SoundVariationMode.Step:
                return EvaluateStep(travel);

            default:
                return authoredValue;
        }
    }


    private float EvaluateStep(float travel)
    {
        float span = high - low;

        if (span <= 0f)
        {
            return low;
        }

        // Mathf.Repeat and Mathf.PingPong both handle negative
        // input, so a negative step needs no special case - it
        // simply walks the other way round the same range.
        //
        // The three wraps differ at the top of the range.
        // Repeat is EXCLUSIVE there - a range of 0 to 12 in
        // semitones with a step of 1 gives the twelve notes 0
        // to 11 and then starts again, so the octave itself is
        // only reached by setting High to 13. PingPong and
        // Clamp are both INCLUSIVE, because the turn, or the
        // stop, happens ON the endpoint.
        float offset;

        switch (wrap)
        {
            case SoundStepWrap.PingPong:
                offset = Mathf.PingPong(travel, span);
                break;

            case SoundStepWrap.Clamp:
                offset = Mathf.Clamp(travel, 0f, span);
                break;

            default:
                offset = Mathf.Repeat(travel, span);
                break;
        }

        return low + offset;
    }


    // =========================================================
    // ADVANCE
    // =========================================================
    //
    // Moves the caller's travel on by one play. Returns the new
    // value for the caller to store.
    //
    // travel is deliberately not wrapped here - Evaluate wraps
    // when it reads it. Keeping the raw total means PingPong can
    // tell an outward leg from a return leg without a direction
    // flag to get out of step.

    public float Advance(float travel)
    {
        if (mode != SoundVariationMode.Step)
        {
            return travel;
        }

        float moved = travel + step;

        float span = high - low;

        if (span <= 0f)
        {
            return moved;
        }

        // Clamp has nowhere further to go, so travel stops
        // accumulating at the end of the range. Without this a
        // long session keeps adding to a number nothing reads,
        // and a single step backwards would then do nothing
        // visible for a very long time.
        if (wrap == SoundStepWrap.Clamp)
        {
            return Mathf.Clamp(moved, 0f, span);
        }

        // A run long enough to lose float precision is not a
        // real case, but a cheap fold keeps it honest.
        //
        // PingPong repeats every TWO spans - out and back - so
        // that is the period the fold has to preserve.
        if (Mathf.Abs(moved) > span * 1024f)
        {
            moved =
                wrap == SoundStepWrap.PingPong
                    ? Mathf.Repeat(moved, span * 2f)
                    : Mathf.Repeat(moved, span);
        }

        return moved;
    }


#if UNITY_EDITOR

    // =========================================================
    // EDITOR SETUP
    // =========================================================
    //
    // For tools that build sound assets - see
    // CrybtAudioGenerator. Editor only; nothing at runtime
    // rewrites a shaper.

    public void EditorSetNone()
    {
        mode = SoundVariationMode.None;
    }


    public void EditorSetRandom(float lowValue, float highValue)
    {
        mode = SoundVariationMode.Random;
        low = lowValue;
        high = highValue;
    }


    public void EditorSetStep(
        float lowValue,
        float highValue,
        float stepValue,
        SoundStepWrap wrapMode)
    {
        mode = SoundVariationMode.Step;
        low = lowValue;
        high = highValue;
        step = stepValue;
        wrap = wrapMode;
    }

#endif
}
