using UnityEngine;

// =============================================================
// EASING
// =============================================================
//
// Pure easing curves. No state, no Unity lifecycle.
//
// WHY THIS EXISTS:
//
// The quadratic ease in/out below previously existed as two
// identical copies - one in Card.cs and one in
// CrybtManager.RotateCard().
//
// The card "feel" is a design decision. It should have one
// home so that changing it changes every card animation
// together.
// =============================================================

public static class Easing
{
    // =========================================================
    // QUADRATIC EASE IN / OUT
    // =========================================================
    //
    // First half:
    // Accelerates toward the destination.
    //
    // Second half:
    // Decelerates into the destination.
    //
    // This creates the parabolic-style acceleration
    // instead of moving at a constant speed.

    public static float QuadInOut(float t)
    {
        t = Mathf.Clamp01(t);

        if (t < 0.5f)
        {
            return 2f * t * t;
        }

        return
            1f -
            Mathf.Pow(
                -2f * t + 2f,
                2f
            ) / 2f;
    }
}
