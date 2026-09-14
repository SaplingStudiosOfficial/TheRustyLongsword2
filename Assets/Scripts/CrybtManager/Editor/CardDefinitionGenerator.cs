using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// =============================================================
// CARD DEFINITION GENERATOR
// =============================================================
//
// STEP 1 OF THE CARD DATA MIGRATION.
//
// Reads every card prefab variant and writes an equivalent
// CardDefinition asset, so the data is provably identical to
// what the prefabs already hold rather than re-typed by hand.
//
// USAGE:
//
//   Tools > Crybt > Generate Card Definitions From Prefabs
//
// Safe to re-run: existing assets are updated in place, not
// duplicated.
//
// This tool does NOT touch any scene or prefab. It only reads
// prefabs and writes new assets.
// =============================================================

public static class CardDefinitionGenerator
{
    private const string PrefabFolder =
        "Assets/PreFabs/Cards";

    private const string OutputFolder =
        "Assets/GameData/Cards";

    private const string DeckAssetPath =
        "Assets/GameData/Cards/StandardDeck.asset";


    [MenuItem("Tools/Crybt/Generate Card Definitions From Prefabs")]
    private static void Generate()
    {
        if (!AssetDatabase.IsValidFolder(PrefabFolder))
        {
            EditorUtility.DisplayDialog(
                "Card Definition Generator",
                "Could not find " + PrefabFolder + ".",
                "OK"
            );

            return;
        }

        EnsureFolder("Assets/GameData");
        EnsureFolder(OutputFolder);


        string[] prefabGuids =
            AssetDatabase.FindAssets(
                "t:Prefab",
                new[] { PrefabFolder }
            );

        List<CardDefinition> generated =
            new List<CardDefinition>();

        int created = 0;
        int updated = 0;
        int skipped = 0;


        for (int i = 0; i < prefabGuids.Length; i++)
        {
            string prefabPath =
                AssetDatabase.GUIDToAssetPath(prefabGuids[i]);

            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                continue;
            }

            Card card =
                prefab.GetComponentInChildren<Card>(true);

            if (card == null)
            {
                skipped++;

                continue;
            }


            // =================================================
            // READ THE PREFAB'S BAKED DATA
            // =================================================

            CardRank rank = card.EditorRawRank;
            CardSuit suit = card.EditorRawSuit;

            Sprite face = FindFace(prefab);


            // =================================================
            // SANITY CHECK
            // =================================================
            //
            // value and rank should agree on non-face cards.
            // If they do not, the prefab is already
            // inconsistent and the generator should say so
            // rather than silently pick one.
            //
            // Face cards legitimately differ - a King is rank
            // 13 and value 10 - so they are exempt.

            int rawValue = card.EditorRawValue;

            bool faceCard =
                rank >= CardConstants.LowestFaceRank;

            if (!faceCard && rawValue != (int)rank)
            {
                Debug.LogWarning(
                    "[Crybt] "
                    + prefab.name
                    + " has value="
                    + rawValue
                    + " but rank="
                    + rank
                    + ". Using rank. Check this prefab."
                );
            }


            // =================================================
            // CREATE OR UPDATE THE ASSET
            // =================================================

            string assetPath =
                OutputFolder
                + "/"
                + SanitiseName(prefab.name)
                + ".asset";

            CardDefinition definition =
                AssetDatabase.LoadAssetAtPath<CardDefinition>(assetPath);

            if (definition == null)
            {
                definition =
                    ScriptableObject.CreateInstance<CardDefinition>();

                definition.EditorInitialiseFrom(rank, suit, face);

                AssetDatabase.CreateAsset(definition, assetPath);

                created++;
            }
            else
            {
                definition.EditorInitialiseFrom(rank, suit, face);

                EditorUtility.SetDirty(definition);

                updated++;
            }

            generated.Add(definition);
        }


        // =====================================================
        // BUILD THE DECK ASSET
        // =====================================================

        DeckDefinition deck =
            AssetDatabase.LoadAssetAtPath<DeckDefinition>(DeckAssetPath);

        if (deck == null)
        {
            deck = ScriptableObject.CreateInstance<DeckDefinition>();

            deck.EditorSetCards(generated);

            AssetDatabase.CreateAsset(deck, DeckAssetPath);
        }
        else
        {
            deck.EditorSetCards(generated);

            EditorUtility.SetDirty(deck);
        }


        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();


        Debug.Log(
            "[Crybt] Card definitions generated. "
            + created + " created, "
            + updated + " updated, "
            + skipped + " prefab(s) had no Card component. "
            + "Deck asset contains " + generated.Count + " card(s)."
        );
    }


    // =========================================================
    // FIND FACE SPRITE
    // =========================================================

    private static Sprite FindFace(GameObject prefab)
    {
        Image image =
            prefab.GetComponentInChildren<Image>(true);

        return image != null
            ? image.sprite
            : null;
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

        string parent = path.Substring(0, lastSlash);
        string leaf = path.Substring(lastSlash + 1);

        AssetDatabase.CreateFolder(parent, leaf);
    }


    private static string SanitiseName(string prefabName)
    {
        return prefabName
            .Replace(" Variant", "")
            .Replace(" ", "_");
    }
}
