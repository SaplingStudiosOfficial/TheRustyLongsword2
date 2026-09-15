using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// =============================================================
// DEFAULT DATA ASSET GENERATOR
// =============================================================
//
// Creates the ScriptableObject assets the Crybt refactor
// introduced, pre-filled with the values the game already had
// hardcoded.
//
// USAGE:
//
//   Tools > Generate Default Data Assets
//
// WHY THIS MATTERS:
//
// The Boon Shop now looks its boons up from a catalogue asset.
// If that catalogue is empty, Torch cannot be purchased - so
// running this (or creating the assets by hand) is REQUIRED,
// not optional.
//
// Safe to re-run: existing assets are left alone.
//
//
// SCOPE:
//
// Crybt only. An earlier version of this tool also generated a
// VillageCatalogue and a SoundSettings asset for the overworld;
// those were removed when the branch narrowed to the Crybt.
// See docs/code-index.md section 0. The overworld versions are
// on the branch backup/refactor-solid-full.
// =============================================================

public static class DefaultDataAssetGenerator
{
    private const string GameDataFolder = "Assets/GameData";

    private const string BoonFolder = "Assets/GameData/Boons";

    private const string RulesPath = "Assets/GameData/CrybtRules.asset";


    // The original costs, straight out of the six BuyBoonN
    // methods that used to live in CrybtManager.
    //
    // Only Torch was ever actually read by gameplay - the other
    // five set a flag that nothing checked. They are created
    // here but marked NOT implemented, so the shop refuses the
    // sale instead of taking the player's points for nothing.
    private static readonly (BoonId id, int cost, string name, bool implemented)[] Boons =
    {
        (BoonId.Torch,          2,  "Torch",          true),
        (BoonId.WardingSigil,   4,  "Warding Sigil",  false),
        (BoonId.BoneCharm,      6,  "Bone Charm",     false),
        (BoonId.HolyWater,      8,  "Holy Water",     false),
        (BoonId.BlackGrimoire,  10, "Black Grimoire", false),
        (BoonId.BrokenMirror,   12, "Broken Mirror",  false),
    };


    [MenuItem("Tools/Generate Default Data Assets")]
    private static void Generate()
    {
        EnsureFolder(GameDataFolder);
        EnsureFolder(BoonFolder);

        CreateRules();

        List<BoonDefinition> boons = CreateBoons();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "[Data] Generated "
            + RulesPath
            + " and "
            + boons.Count
            + " boon asset(s).\n"
            + "NEXT: select the CrybtManager in the Crybt scene and "
            + "assign the Rules asset and all six Boon assets to its "
            + "Boon Shop catalogue, then save the scene."
        );

        EditorUtility.DisplayDialog(
            "Default Data Assets",
            "Created under Assets/GameData.\n\n"
            + "You still need to assign them:\n\n"
            + "1. CrybtManager -> Rules = CrybtRules\n"
            + "2. CrybtManager -> Boon Shop -> Catalogue = the 6 boons\n\n"
            + "Then save the Crybt scene.",
            "OK"
        );
    }


    // =========================================================
    // RULES
    // =========================================================

    private static void CreateRules()
    {
        CrybtRules rules =
            AssetDatabase.LoadAssetAtPath<CrybtRules>(RulesPath);

        if (rules != null)
        {
            return;
        }

        // The field initialisers already hold the original
        // values, so a fresh instance is correct by
        // construction.
        rules = ScriptableObject.CreateInstance<CrybtRules>();

        AssetDatabase.CreateAsset(rules, RulesPath);
    }


    // =========================================================
    // BOONS
    // =========================================================

    private static List<BoonDefinition> CreateBoons()
    {
        List<BoonDefinition> created =
            new List<BoonDefinition>();

        for (int i = 0; i < Boons.Length; i++)
        {
            string path =
                BoonFolder
                + "/Boon_"
                + Boons[i].id
                + ".asset";

            BoonDefinition boon =
                AssetDatabase.LoadAssetAtPath<BoonDefinition>(path);

            if (boon == null)
            {
                boon = ScriptableObject.CreateInstance<BoonDefinition>();

                boon.EditorInitialise(
                    Boons[i].id,
                    Boons[i].cost,
                    Boons[i].name,
                    Boons[i].implemented
                );

                AssetDatabase.CreateAsset(boon, path);
            }

            created.Add(boon);
        }

        return created;
    }


    // =========================================================
    // HELPERS
    // =========================================================

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        int lastSlash = path.LastIndexOf('/');

        AssetDatabase.CreateFolder(
            path.Substring(0, lastSlash),
            path.Substring(lastSlash + 1)
        );
    }
}
