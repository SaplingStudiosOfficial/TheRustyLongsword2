using System;
using System.Collections.Generic;
using UnityEngine;

// =============================================================
// BOON SHOP
// =============================================================
//
// Replaces twelve loose bool fields:
//
//     boon1Torch ... boon6BrokenMirror
//     nextCrawlTorch ... nextCrawlBrokenMirror
//
// ...and the three copy-paste blocks that expired, activated,
// and reset them.
//
// A plain serializable class rather than a MonoBehaviour, so
// the catalogue can be assigned on CrybtManager without adding
// a new GameObject to the Crybt scene.
//
// The two sets are runtime state and are deliberately NOT
// serialized - a crawl always begins from a known position.
// =============================================================

[Serializable]
public class BoonShop
{
    // =========================================================
    // CATALOGUE
    // =========================================================
    //
    // Assign every BoonDefinition asset here in the Inspector.

    [SerializeField]
    [Tooltip("Every boon the shop can offer. Order drives nothing " +
             "- identity comes from each asset's BoonId.")]
    private List<BoonDefinition> catalogue = new List<BoonDefinition>();


    // =========================================================
    // RUNTIME STATE
    // =========================================================
    //
    // active    = boons in effect during the CURRENT crawl.
    // purchased = boons bought for the NEXT crawl.

    private readonly HashSet<BoonId> active =
        new HashSet<BoonId>();

    private readonly HashSet<BoonId> purchased =
        new HashSet<BoonId>();


    public IReadOnlyList<BoonDefinition> Catalogue => catalogue;


    // =========================================================
    // FILL CATALOGUE IF EMPTY
    // =========================================================
    //
    // An unassigned catalogue is not a harmless default - it
    // means no boon can be bought, including Torch, which IS
    // implemented. So the manager loads the shipped assets and
    // offers them here.
    //
    // Anything assigned in the Inspector wins outright: this
    // does nothing unless the list is genuinely empty.

    public bool FillIfEmpty(BoonDefinition[] defaults)
    {
        if (catalogue == null)
        {
            catalogue = new List<BoonDefinition>();
        }

        if (catalogue.Count > 0)
        {
            return false;
        }

        if (defaults == null ||
            defaults.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < defaults.Length; i++)
        {
            if (defaults[i] != null)
            {
                catalogue.Add(defaults[i]);
            }
        }

        return catalogue.Count > 0;
    }


    // =========================================================
    // LOOKUP
    // =========================================================

    public BoonDefinition Find(BoonId id)
    {
        for (int i = 0;
            i < catalogue.Count;
            i++)
        {
            if (catalogue[i] != null &&
                catalogue[i].Id == id)
            {
                return catalogue[i];
            }
        }

        return null;
    }


    // =========================================================
    // QUERIES
    // =========================================================
    //
    // IsActive is the single place gameplay asks whether a boon
    // is in effect. A boon with no effect is now a missing call
    // to this method - one searchable thing, rather than an
    // unread private bool hiding among eleven siblings.

    public bool IsActive(BoonId id)
    {
        return active.Contains(id);
    }


    public bool IsPurchased(BoonId id)
    {
        return purchased.Contains(id);
    }


    // =========================================================
    // PURCHASE
    // =========================================================
    //
    // The caller is responsible for spending the points - the
    // shop does not know about the encounter stage or the
    // player's wallet.

    public void MarkPurchased(BoonId id)
    {
        purchased.Add(id);
    }


    // =========================================================
    // ADVANCE CRAWL
    // =========================================================
    //
    // Replaces three separate blocks in FinishBoonPhase():
    // expire the current boons, activate the purchased ones,
    // reset the purchase list.

    public void AdvanceToNextCrawl()
    {
        active.Clear();

        active.UnionWith(purchased);

        purchased.Clear();
    }


    // =========================================================
    // CLEAR PURCHASES
    // =========================================================
    //
    // Called when the Boon Shop opens, so a crawl never starts
    // with leftovers from an abandoned shop visit.

    public void ClearPurchases()
    {
        purchased.Clear();
    }


    // =========================================================
    // DEBUG DESCRIPTION
    // =========================================================

    public string DescribeActive()
    {
        if (active.Count == 0)
        {
            return "none";
        }

        List<string> names = new List<string>(active.Count);

        foreach (BoonId id in active)
        {
            BoonDefinition definition = Find(id);

            names.Add(
                definition != null
                    ? definition.DisplayName
                    : id.ToString()
            );
        }

        return string.Join(", ", names);
    }
}
