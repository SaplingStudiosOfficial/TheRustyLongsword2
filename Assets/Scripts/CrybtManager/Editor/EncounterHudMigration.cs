using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// =============================================================
// ENCOUNTER HUD MIGRATION
// =============================================================
//
// ONE-SHOT EDITOR TOOL.
//
// Moves the fifteen UI references that used to live on
// CrybtManager onto an EncounterHud component, without anyone
// having to re-drag them by hand and without editing scene
// YAML directly.
//
// USAGE:
//
//   1. Open the Crybt scene.
//   2. Tools > Crybt > Migrate HUD References
//   3. Check the Console report, then save the scene.
//
// AFTER MIGRATING:
//
// Delete this script and the "LEGACY UI REFERENCES" block in
// CrybtManager. They exist only to carry the scene data
// across the refactor.
// =============================================================

public static class EncounterHudMigration
{
    private const string MenuPath =
        "Tools/Crybt/Migrate HUD References";


    // Old field name on CrybtManager -> new field name on EncounterHud.
    private static readonly (string legacy, string modern)[] FieldMap =
    {
        ("KeepHeroButton",        "keepHeroButton"),
        ("SwitchHeroButton",      "switchHeroButton"),
        ("PeekCardButton",        "peekCardButton"),
        ("ConfirmComboButton",    "confirmComboButton"),
        ("EndEncounterButton",    "endEncounterButton"),

        ("TrashDisplay",          "trashDisplay"),
        ("AttackScore",           "attackScore"),
        ("Boons",                 "boons"),

        ("HealthDisplay",         "healthDisplay"),
        ("ModifierDisplay",       "modifierDisplay"),
        ("ScoreDisplay",          "scoreDisplay"),
        ("MonsterAPDisplay",      "monsterAPDisplay"),
        ("EncounterDisplay",      "encounterDisplay"),
        ("CrawlDisplay",          "crawlDisplay"),
        ("OfferingPointsDisplay", "offeringPointsDisplay"),
    };


    [MenuItem(MenuPath)]
    private static void Migrate()
    {
        CrybtManager manager =
            Object.FindObjectOfType<CrybtManager>();

        if (manager == null)
        {
            EditorUtility.DisplayDialog(
                "Crybt HUD Migration",
                "No CrybtManager found in the open scene.\n\n"
                + "Open the Crybt scene first.",
                "OK"
            );

            return;
        }


        // =====================================================
        // FIND OR CREATE THE HUD
        // =====================================================

        EncounterHud hud =
            Object.FindObjectOfType<EncounterHud>();

        if (hud == null)
        {
            hud = manager.gameObject.AddComponent<EncounterHud>();

            Undo.RegisterCreatedObjectUndo(
                hud,
                "Create EncounterHud"
            );

            Debug.Log(
                "[Crybt] Added an EncounterHud component to "
                + manager.gameObject.name
                + "."
            );
        }


        // =====================================================
        // COPY THE REFERENCES
        // =====================================================

        Undo.RecordObject(hud, "Migrate HUD references");
        Undo.RecordObject(manager, "Migrate HUD references");

        SerializedObject source = new SerializedObject(manager);
        SerializedObject target = new SerializedObject(hud);

        int copied = 0;
        int missing = 0;

        for (int i = 0; i < FieldMap.Length; i++)
        {
            SerializedProperty from =
                source.FindProperty(FieldMap[i].legacy);

            SerializedProperty to =
                target.FindProperty(FieldMap[i].modern);

            if (from == null || to == null)
            {
                Debug.LogWarning(
                    "[Crybt] Could not map "
                    + FieldMap[i].legacy
                    + " -> "
                    + FieldMap[i].modern
                    + ". One of the fields no longer exists."
                );

                missing++;

                continue;
            }

            if (from.objectReferenceValue == null)
            {
                Debug.LogWarning(
                    "[Crybt] "
                    + FieldMap[i].legacy
                    + " was empty on CrybtManager - nothing to copy. "
                    + "Assign "
                    + FieldMap[i].modern
                    + " on EncounterHud by hand."
                );

                missing++;

                continue;
            }

            to.objectReferenceValue =
                from.objectReferenceValue;

            copied++;
        }

        target.ApplyModifiedProperties();


        // =====================================================
        // WIRE THE HUD ONTO THE MANAGER
        // =====================================================

        SerializedProperty hudField =
            source.FindProperty("hud");

        if (hudField != null)
        {
            hudField.objectReferenceValue = hud;

            source.ApplyModifiedProperties();
        }

        EditorUtility.SetDirty(hud);
        EditorUtility.SetDirty(manager);

        EditorSceneManager.MarkSceneDirty(
            manager.gameObject.scene
        );


        Debug.Log(
            "[Crybt] HUD migration complete. "
            + copied
            + " reference(s) copied, "
            + missing
            + " needing attention. SAVE THE SCENE to keep this."
        );
    }
}
