using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// =============================================================
// CRYBT DECK MIGRATION
// =============================================================
//
// STEPS 4 AND 5 of removing the per-card prefabs: empty the
// scene of its 52 card objects, then delete the 52 prefab
// variants.
//
// These are the destructive steps, so they are deliberately
// SEPARATE menu items, run in order, each refusing to proceed
// unless the previous one provably worked:
//
//   Tools > Crybt > Deck Migration >
//     1. Validate
//     2. Repair Card Prefab Ranks
//     3. Move Scene Deck Into Deck Asset
//     4. Delete Card Prefab Variants
//
// Nothing here edits scene or prefab YAML by hand. Everything
// goes through SerializedObject / AssetDatabase so Unity
// rewrites the files and keeps its reference database honest.
//
// All of it is reversible with git as long as you have not
// committed in between.
// =============================================================

public static class CrybtDeckMigration
{
    private const string PrefabFolder = "Assets/PreFabs/Cards";

    private const string DeckAssetPath =
        "Assets/GameData/Cards/StandardDeck.asset";

    private const int ExpectedCards = 52;


    // =========================================================
    // 1. VALIDATE
    // =========================================================
    //
    // Read-only. Says whether the deck asset is safe to switch
    // to, and what is wrong if not.

    [MenuItem("Tools/Crybt/Deck Migration/1. Validate", priority = 1)]
    private static void Validate()
    {
        StringBuilder report = new StringBuilder();

        bool ok = Check(report);

        Debug.Log("[Crybt] Deck validation:\n" + report);

        EditorUtility.DisplayDialog(
            "Deck Validation",
            report.ToString()
            + "\n"
            + (ok
                ? "SAFE to continue to step 3."
                : "NOT safe. Fix the above first."),
            "OK"
        );
    }


    private static bool Check(StringBuilder report)
    {
        DeckDefinition deck =
            AssetDatabase.LoadAssetAtPath<DeckDefinition>(DeckAssetPath);

        if (deck == null)
        {
            report.AppendLine(
                "MISSING: " + DeckAssetPath
                + "\nRun Tools > Crybt > Generate Card "
                + "Definitions From Prefabs first."
            );

            return false;
        }

        bool ok = true;

        report.AppendLine("Deck asset: " + DeckAssetPath);
        report.AppendLine("Cards: " + deck.Count);

        if (deck.Count != ExpectedCards)
        {
            report.AppendLine(
                "  PROBLEM: expected " + ExpectedCards + "."
            );

            ok = false;
        }

        if (deck.CardPrefab == null)
        {
            report.AppendLine("  PROBLEM: no card prefab set.");

            ok = false;
        }
        else
        {
            report.AppendLine(
                "Template prefab: " + deck.CardPrefab.name
            );
        }

        if (deck.CardBackSprite == null)
        {
            report.AppendLine("  PROBLEM: no card back sprite.");

            ok = false;
        }


        // =====================================================
        // EVERY RANK IN EVERY SUIT, EXACTLY ONCE
        // =====================================================
        //
        // This is the check that catches bad prefab data. A
        // card whose rank is wrong shows up here as a duplicate
        // AND a gap - which is exactly what the Ace of Spades
        // does while it still inherits King's rank.

        HashSet<string> seen = new HashSet<string>();

        int missingFace = 0;

        for (int i = 0; i < deck.Count; i++)
        {
            DeckEntry e = deck.Cards[i];

            if (e == null)
            {
                report.AppendLine("  PROBLEM: null row at " + i);

                ok = false;

                continue;
            }

            string key = e.Suit + "/" + e.Rank;

            if (!seen.Add(key))
            {
                report.AppendLine("  DUPLICATE: " + key);

                ok = false;
            }

            if (e.Rank == CardRank.None)
            {
                report.AppendLine("  PROBLEM: row " + i + " has no rank.");

                ok = false;
            }

            if (e.Face == null)
            {
                missingFace++;
            }
        }

        if (missingFace > 0)
        {
            report.AppendLine(
                "  PROBLEM: " + missingFace + " row(s) have no face sprite."
            );

            ok = false;
        }

        CardSuit[] suits =
        {
            CardSuit.Hearts, CardSuit.Diamonds,
            CardSuit.Clubs, CardSuit.Spades
        };

        for (int s = 0; s < suits.Length; s++)
        {
            for (int r = (int)CardRank.Ace; r <= (int)CardRank.King; r++)
            {
                string key = suits[s] + "/" + (CardRank)r;

                if (!seen.Contains(key))
                {
                    report.AppendLine("  MISSING: " + key);

                    ok = false;
                }
            }
        }

        return ok;
    }


    // =========================================================
    // 2. REPAIR CARD PREFAB RANKS
    // =========================================================
    //
    // One prefab ships with the wrong rank: 1_Spades Variant
    // overrides value and suit but not rank, so it inherits 13
    // from the King it was branched off. The Crybt scene papers
    // over it with an instance override, but the generator
    // reads the PREFAB, so the deck table would get two Kings
    // of Spades and no Ace.
    //
    // Rather than special-case that one file, compare every
    // prefab's rank against the number its filename starts with
    // and report every disagreement.

    [MenuItem("Tools/Crybt/Deck Migration/2. Repair Card Prefab Ranks", priority = 2)]
    private static void RepairRanks()
    {
        string[] guids =
            AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolder });

        List<string> fixes = new List<string>();

        List<Object> targets = new List<Object>();

        List<int> wanted = new List<int>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
            {
                continue;
            }

            Card card = prefab.GetComponent<Card>();

            if (card == null)
            {
                continue;
            }

            string file = System.IO.Path.GetFileNameWithoutExtension(path);

            int underscore = file.IndexOf('_');

            int nameRank;

            if (underscore <= 0 ||
                !int.TryParse(file.Substring(0, underscore), out nameRank))
            {
                // Card_Back_Red and anything else unnumbered.
                continue;
            }

            if ((int)card.EditorRawRank == nameRank)
            {
                continue;
            }

            fixes.Add(
                file + ": rank " + card.EditorRawRank
                + " -> " + (CardRank)nameRank
            );

            targets.Add(card);

            wanted.Add(nameRank);
        }

        if (fixes.Count == 0)
        {
            EditorUtility.DisplayDialog(
                "Repair Card Prefab Ranks",
                "Every card prefab's rank already matches its "
                + "filename. Nothing to do.",
                "OK"
            );

            return;
        }

        if (!EditorUtility.DisplayDialog(
                "Repair Card Prefab Ranks",
                "These prefabs disagree with their filenames:\n\n"
                + string.Join("\n", fixes.ToArray())
                + "\n\nFix them?",
                "Fix", "Cancel"))
        {
            return;
        }

        for (int i = 0; i < targets.Count; i++)
        {
            SerializedObject so = new SerializedObject(targets[i]);

            SerializedProperty rank = so.FindProperty("rank");

            if (rank == null)
            {
                continue;
            }

            rank.enumValueIndex = EnumIndexForRank(wanted[i]);

            so.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(targets[i]);
        }

        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();

        Debug.Log(
            "[Crybt] Repaired " + fixes.Count + " prefab rank(s):\n"
            + string.Join("\n", fixes.ToArray())
        );
    }


    // CardRank is not contiguous from 0 - it starts at None = 0
    // then Ace = 1 - so the enum POPUP index happens to equal
    // the value here. Resolved explicitly rather than assumed.
    private static int EnumIndexForRank(int rankValue)
    {
        System.Array values = System.Enum.GetValues(typeof(CardRank));

        for (int i = 0; i < values.Length; i++)
        {
            if ((int)(CardRank)values.GetValue(i) == rankValue)
            {
                return i;
            }
        }

        return 0;
    }


    // =========================================================
    // 3. MOVE SCENE DECK INTO DECK ASSET
    // =========================================================
    //
    // Assigns the deck asset, deletes the card objects, clears
    // the Deck list, saves the scene.

    [MenuItem("Tools/Crybt/Deck Migration/3. Move Scene Deck Into Deck Asset", priority = 3)]
    private static void MoveSceneDeck()
    {
        StringBuilder report = new StringBuilder();

        if (!Check(report))
        {
            EditorUtility.DisplayDialog(
                "Move Scene Deck",
                "The deck asset is not valid yet, so the scene "
                + "cards must stay:\n\n" + report,
                "OK"
            );

            return;
        }

        CrybtManager manager =
            Object.FindObjectOfType<CrybtManager>(true);

        if (manager == null)
        {
            EditorUtility.DisplayDialog(
                "Move Scene Deck",
                "No CrybtManager in the open scene. Open the "
                + "Crybt scene first.",
                "OK"
            );

            return;
        }

        DeckDefinition deck =
            AssetDatabase.LoadAssetAtPath<DeckDefinition>(DeckAssetPath);

        SerializedObject so = new SerializedObject(manager);

        SerializedProperty deckList = so.FindProperty("Deck");

        SerializedProperty deckAsset = so.FindProperty("deckDefinition");

        if (deckList == null || deckAsset == null)
        {
            EditorUtility.DisplayDialog(
                "Move Scene Deck",
                "Could not find the Deck list or the "
                + "deckDefinition field on CrybtManager.",
                "OK"
            );

            return;
        }

        int sceneCards = deckList.arraySize;

        if (!EditorUtility.DisplayDialog(
                "Move Scene Deck",
                "This will DELETE " + sceneCards + " card object(s) "
                + "from the open scene and drive the deck from "
                + deck.name + " (" + deck.Count + " cards) instead.\n\n"
                + "The scene is not saved automatically - check it "
                + "first, then save.\n\nContinue?",
                "Delete and switch", "Cancel"))
        {
            return;
        }


        // Collect first: destroying while walking the
        // SerializedProperty array invalidates it.
        List<GameObject> doomed = new List<GameObject>();

        for (int i = 0; i < deckList.arraySize; i++)
        {
            Card card =
                deckList.GetArrayElementAtIndex(i).objectReferenceValue
                    as Card;

            if (card != null)
            {
                doomed.Add(card.gameObject);
            }
        }

        deckList.ClearArray();

        deckAsset.objectReferenceValue = deck;

        so.ApplyModifiedProperties();

        for (int i = 0; i < doomed.Count; i++)
        {
            Undo.DestroyObjectImmediate(doomed[i]);
        }

        EditorUtility.SetDirty(manager);

        EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);

        Debug.Log(
            "[Crybt] Removed " + doomed.Count + " card object(s) "
            + "and assigned " + deck.name
            + ". SAVE THE SCENE to keep this."
        );

        EditorUtility.DisplayDialog(
            "Move Scene Deck",
            "Removed " + doomed.Count + " card object(s).\n\n"
            + "Press Play and check the deck deals correctly, "
            + "then SAVE THE SCENE.",
            "OK"
        );
    }


    // =========================================================
    // 4. DELETE CARD PREFAB VARIANTS
    // =========================================================
    //
    // Last, and only once the scene no longer references them.
    // The template the deck asset points at is kept.

    [MenuItem("Tools/Crybt/Deck Migration/4. Delete Card Prefab Variants", priority = 4)]
    private static void DeleteVariants()
    {
        DeckDefinition deck =
            AssetDatabase.LoadAssetAtPath<DeckDefinition>(DeckAssetPath);

        if (deck == null || deck.CardPrefab == null)
        {
            EditorUtility.DisplayDialog(
                "Delete Card Prefab Variants",
                "No deck asset, or it has no template prefab. "
                + "Nothing would be left to build cards from.",
                "OK"
            );

            return;
        }

        CrybtManager manager =
            Object.FindObjectOfType<CrybtManager>(true);

        if (manager != null)
        {
            SerializedObject so = new SerializedObject(manager);

            SerializedProperty deckList = so.FindProperty("Deck");

            if (deckList != null && deckList.arraySize > 0)
            {
                EditorUtility.DisplayDialog(
                    "Delete Card Prefab Variants",
                    "The open scene still has " + deckList.arraySize
                    + " card object(s) in its Deck list. Run step 3 "
                    + "first, or those cards lose their prefabs.",
                    "OK"
                );

                return;
            }
        }

        string templatePath =
            AssetDatabase.GetAssetPath(deck.CardPrefab);

        string[] guids =
            AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolder });

        List<string> doomed = new List<string>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            if (path == templatePath)
            {
                continue;
            }

            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null &&
                prefab.GetComponent<Card>() != null)
            {
                doomed.Add(path);
            }
        }

        if (doomed.Count == 0)
        {
            EditorUtility.DisplayDialog(
                "Delete Card Prefab Variants",
                "Nothing to delete.",
                "OK"
            );

            return;
        }

        if (!EditorUtility.DisplayDialog(
                "Delete Card Prefab Variants",
                "Permanently delete " + doomed.Count
                + " card prefab(s)?\n\nKeeping: "
                + System.IO.Path.GetFileName(templatePath)
                + "\n\nThis cannot be undone from the Editor - "
                + "use git to reverse it.",
                "Delete", "Cancel"))
        {
            return;
        }

        List<string> failed = new List<string>();

        AssetDatabase.DeleteAssets(doomed.ToArray(), failed);

        AssetDatabase.Refresh();

        Debug.Log(
            "[Crybt] Deleted " + (doomed.Count - failed.Count)
            + " card prefab(s). Kept " + templatePath
            + (failed.Count > 0
                ? ". FAILED: " + string.Join(", ", failed.ToArray())
                : ".")
        );
    }
}
