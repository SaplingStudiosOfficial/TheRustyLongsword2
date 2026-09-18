using UnityEngine;

// =============================================================
// UI VISIBILITY EXTENSIONS
// =============================================================
//
// WHY THIS EXISTS:
//
// This idiom appeared dozens of times across the project:
//
//     if (SomeButton != null)
//     {
//         SomeButton.gameObject.SetActive(false);
//     }
//
// An extension method is a STATIC method, so it can be called
// on a null reference without throwing. That means the null
// check moves in here and disappears from every call site:
//
//     SomeButton.SetVisible(false);
//
// This also encourages writing one assignment per control
// rather than "hide everything, then re-show some of it",
// which is where stale-UI bugs hide.
// =============================================================

public static class UiVisibilityExtensions
{
    // =========================================================
    // COMPONENT
    // =========================================================
    //
    // Covers Button, Text, Image, Slider, and any other
    // component reference held in the Inspector.
    //
    // Unity's overloaded == operator treats a destroyed object
    // as null, so this is safe after Destroy() as well.

    public static void SetVisible(
        this Component component,
        bool visible)
    {
        if (component == null)
        {
            return;
        }

        component.gameObject.SetActive(visible);
    }


    // =========================================================
    // GAME OBJECT
    // =========================================================

    public static void SetVisible(
        this GameObject target,
        bool visible)
    {
        if (target == null)
        {
            return;
        }

        target.SetActive(visible);
    }
}
