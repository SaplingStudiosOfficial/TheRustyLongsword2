using System.Diagnostics;
using Debug = UnityEngine.Debug;

// =============================================================
// GAME LOG
// =============================================================
//
// WHY THIS EXISTS:
//
// Debug.Log("Monster AP: " + ap) still builds the string in a
// shipping build even when log output is stripped, because the
// argument is evaluated before the call is made.
//
// [Conditional] is different. The C# compiler removes the call
// AND ITS ARGUMENTS entirely when the symbol is not defined,
// so the concatenation never happens at all.
//
// The rules-documenting logs in CrybtManager are valuable in
// the Editor and cost nothing in a release build once routed
// through here.
//
// NOTE:
//
// Warnings and errors are deliberately NOT conditional. A
// missing reference or a broken state machine needs to be
// visible in a build.
// =============================================================

public static class GameLog
{
    // =========================================================
    // INFO
    // =========================================================
    //
    // Editor and development builds only.

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void Info(string message)
    {
        Debug.Log(message);
    }


    // =========================================================
    // WARNING
    // =========================================================
    //
    // Always compiled. Something is wired wrong.

    public static void Warning(string message)
    {
        Debug.LogWarning(message);
    }


    // =========================================================
    // ERROR
    // =========================================================
    //
    // Always compiled. Something is broken.

    public static void Error(string message)
    {
        Debug.LogError(message);
    }
}
