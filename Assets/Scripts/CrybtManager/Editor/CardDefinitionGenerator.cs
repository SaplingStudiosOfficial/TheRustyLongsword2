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

    // Root of the prefab variant chain; used as the one
    // prefab every runtime card is instantiated from.
    private const string TemplatePrefabName = "1_Heart.prefab";


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
        // BUILD THE DECK TABLE
        // =====================================================
        //
        // Self-contained: rank, suit and face sprite per row,
        // so the deck no longer depends on the 52 prefabs or
        // on the 52 CardDefinition assets above.
        //
        // The card BACK is not a row. It is not playable, it
        // has no rank, and giving it one would put a 53rd card
        // in a 52 card deck.

        List<DeckEntry> entries = new List<DeckEntry>();

        Sprite backSprite = null;

        GameObject template = null;

        for (int i = 0; i < prefabGuids.Length; i++)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(prefabGuids[i]);

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

            Sprite face = FaceSpriteOf(prefab);

            if (card.EditorRawSuit == CardSuit.Back)
            {
                backSprite = face;

                continue;
            }

            DeckEntry entry = new DeckEntry();

            entry.EditorInitialise(
                card.EditorRawRank,
                card.EditorRawSuit,
                face
            );

            entries.Add(entry);

            // Any card prefab works as the template - every
            // per-card difference is overwritten at bind time.
            // Prefer the one at the root of the variant chain.
            if (template == null ||
                path.EndsWith(TemplatePrefabName))
            {
                template = prefab;
            }
        }

        entries.Sort(
            (a, b) =>
            {
                int bySuit = a.Suit.CompareTo(b.Suit);

                return bySuit != 0
                    ? bySuit
                    : a.Rank.CompareTo(b.Rank);
            }
        );

        DeckDefinition deck =
            AssetDatabase.LoadAssetAtPath<DeckDefinition>(DeckAssetPath);

        bool deckIsNew = deck == null;

        if (deckIsNew)
        {
            deck = ScriptableObject.CreateInstance<DeckDefinition>();
        }

        deck.EditorSet(template, backSprite, entries);

        if (deckIsNew)
        {
            AssetDatabase.CreateAsset(deck, DeckAssetPath);
        }
        else
        {
            EditorUtility.SetDirty(deck);
        }


        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();


        Debug.Log(
            "[Crybt] Card definitions generated. "
            + created + " created, "
            + updated + " updated, "
            + skipped + " prefab(s) had no Card component. "
            + "Deck table: " + entries.Count + " card(s), "
            + (backSprite != null ? "back sprite found" : "NO BACK SPRITE")
            + ", template = "
            + (template != null ? template.name : "NONE")
            + "."
        );

        if (entries.Count != 52)
        {
            Debug.LogWarning(
                "[Crybt] Expected 52 cards, generated "
                + entries.Count
                + ". Check for missing or duplicate prefabs."
            );
        }
    }


    // =========================================================
    // FACE SPRITE OF
    // =========================================================
    //
    // The face is the Image on the prefab ROOT. The child
    // "Button" object has an Image too, which is the click
    // target, not the art.

    private static Sprite FaceSpriteOf(GameObject prefab)
    {
        Image image = prefab.GetComponent<Image>();

        return image != null
            ? image.sprite
            : null;
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
