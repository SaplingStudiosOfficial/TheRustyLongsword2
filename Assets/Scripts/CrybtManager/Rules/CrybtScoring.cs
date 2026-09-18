using System.Collections.Generic;

// =============================================================
// CRYBT SCORING
// =============================================================
//
// Pure scoring rules. No UnityEngine reference, no state, no
// lifecycle - which means this can be called from a test, a
// balance tool, or a replay without entering Play Mode.
//
// The point values arrive as a CrybtRules parameter rather
// than being hardcoded here. The formula is code; the balance
// is data.
//
//
// IMPORTANT - HOW ENCOUNTER SCORING WORKS:
//
// The EXACT cards selected by the player must form the
// scoring combination.
//
// This does NOT search a large selection looking for smaller
// hidden combinations.
//
// Examples:
//
// 5 + King        = 15     = 2 points
// 7 + 7           = Pair   = 2 points
// 7 + 7 + 7       = Triple = 6 points
// 2 + 3 + 4       = Run    = 3 points
// 2 + 3 + 4 + King         = NOT a run
//
// Offering scoring is different - see ScoreOfferingPile.
// =============================================================

public static class CrybtScoring
{
    // =========================================================
    // SCORE COMBINATION
    // =========================================================
    //
    // Scores one player-confirmed selection. A single
    // selection can satisfy more than one rule at once
    // (a five-card run that is also a flush scores both).

    public static int ScoreCombination(
        IReadOnlyList<CardValue> cards,
        CrybtRules rules)
    {
        return Evaluate(
            cards,
            rules,
            null
        );
    }


    // =========================================================
    // DESCRIBE COMBINATION
    // =========================================================
    //
    // Same scoring pass as ScoreCombination, but also records
    // WHICH rules fired and for how much.
    //
    // Both go through the one Evaluate() below, so the label
    // the player reads can never disagree with the points they
    // are awarded. That is the whole reason this is not a
    // separate "explain" function.
    //
    // 'into' is cleared and refilled, so the caller can keep
    // one list and reuse it - no allocation per toggle.

    public static int DescribeCombination(
        IReadOnlyList<CardValue> cards,
        CrybtRules rules,
        List<ScoreItem> into)
    {
        if (into != null)
        {
            into.Clear();
        }

        return Evaluate(
            cards,
            rules,
            into
        );
    }


    // =========================================================
    // EVALUATE
    // =========================================================
    //
    // The single source of truth for combination scoring.
    //
    // breakdown may be null, in which case nothing is recorded
    // and nothing is allocated - that is the hot path used by
    // the live preview.

    private static int Evaluate(
        IReadOnlyList<CardValue> cards,
        CrybtRules rules,
        List<ScoreItem> breakdown)
    {
        if (cards == null ||
            cards.Count == 0 ||
            rules == null)
        {
            return 0;
        }

        int points = 0;


        // =====================================================
        // FIFTEEN
        // =====================================================
        //
        // The entire selected group must total exactly 15.

        if (cards.Count >= 2 &&
            PipTotal(cards) == rules.FifteenTarget)
        {
            points += rules.FifteenPoints;

            Record(
                breakdown,
                rules.FifteenTarget == 15
                    ? "Fifteen"
                    : "Total " + rules.FifteenTarget,
                rules.FifteenPoints
            );
        }


        // =====================================================
        // PAIR / TRIPLE / FOUR OF A KIND
        // =====================================================
        //
        // ALL selected cards must share a rank.
        //
        // 2 cards = 1 pair  = 2 points
        // 3 cards = 3 pairs = 6 points
        // 4 cards = 6 pairs = 12 points

        if (cards.Count >= 2 &&
            cards.Count <= 4 &&
            AllSameRank(cards))
        {
            int pairPoints =
                PairCount(cards.Count)
                * rules.PointsPerPair;

            points += pairPoints;

            Record(
                breakdown,
                NameForSameRank(cards.Count),
                pairPoints
            );
        }


        // =====================================================
        // RUN
        // =====================================================
        //
        // The ENTIRE selected group must be consecutive.

        if (cards.Count >= rules.MinimumRunLength &&
            IsRun(cards))
        {
            points += cards.Count;

            Record(
                breakdown,
                "Run of " + cards.Count,
                cards.Count
            );
        }


        // =====================================================
        // FLUSH
        // =====================================================
        //
        // The entire selected group must share a suit, and the
        // group size must fall inside the flush window.
        //
        // A 6-card same-suit selection scores nothing. That is
        // existing behaviour, preserved deliberately - widen
        // MaximumFlushSize in the rules asset to change it.

        if (cards.Count >= rules.MinimumFlushSize &&
            cards.Count <= rules.MaximumFlushSize &&
            IsFlush(cards))
        {
            points += cards.Count;

            Record(
                breakdown,
                cards.Count + "-Card Flush",
                cards.Count
            );
        }


        return points;
    }


    // =========================================================
    // RECORD
    // =========================================================

    private static void Record(
        List<ScoreItem> breakdown,
        string name,
        int points)
    {
        if (breakdown == null)
        {
            return;
        }

        breakdown.Add(
            new ScoreItem(
                name,
                points
            )
        );
    }


    // =========================================================
    // NAME FOR SAME RANK
    // =========================================================
    //
    // Only reached for 2..4 cards - the guard above rules out
    // everything else.

    private static string NameForSameRank(int count)
    {
        switch (count)
        {
            case 2:  return "Pair";
            case 3:  return "Triple";
            case 4:  return "Four of a Kind";
            default: return count + " of a Kind";
        }
    }


    // =========================================================
    // SCORE OFFERING PILE
    // =========================================================
    //
    // Unlike a confirmed combination, the Offering is scored as
    // a whole pile, so every subset is searched for Fifteens
    // and every card pairing is checked.
    //
    // graveyardCount contributes a flat bonus per card.
    //
    // NOTE ON COST:
    //
    // Subset enumeration is 2^n. The Offering holds one card
    // per encounter plus one at crawl end, so n is small and
    // this is trivial. The guard below exists so that a future
    // boon which grows the Offering fails loudly instead of
    // silently freezing a frame.

    public const int MaximumOfferingSizeForSubsetSearch = 16;

    public static int ScoreOfferingPile(
        IReadOnlyList<CardValue> offering,
        int graveyardCount,
        CrybtRules rules)
    {
        if (rules == null)
        {
            return 0;
        }

        int points = 0;

        int offeringCount =
            offering == null
                ? 0
                : offering.Count;


        // =====================================================
        // FIFTEENS - EVERY SUBSET
        // =====================================================

        if (offeringCount > 0 &&
            offeringCount <= MaximumOfferingSizeForSubsetSearch)
        {
            int subsetCount = 1 << offeringCount;

            for (int mask = 1;
                mask < subsetCount;
                mask++)
            {
                int total = 0;

                for (int i = 0;
                    i < offeringCount;
                    i++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        total += offering[i].PipValue;
                    }
                }

                if (total == rules.FifteenTarget)
                {
                    points += rules.OfferingFifteenPoints;
                }
            }
        }

        else if (offeringCount > MaximumOfferingSizeForSubsetSearch)
        {
            GameLog.Error(
                "Offering pile of "
                + offeringCount
                + " cards is too large to score by subset search. "
                + "Fifteens were skipped."
            );
        }


        // =====================================================
        // PAIRS
        // =====================================================

        for (int i = 0;
            i < offeringCount;
            i++)
        {
            for (int j = i + 1;
                j < offeringCount;
                j++)
            {
                if (offering[i].Rank == offering[j].Rank)
                {
                    points += rules.OfferingPairPoints;
                }
            }
        }


        // =====================================================
        // GRAVEYARD BONUS
        // =====================================================

        points +=
            graveyardCount
            * rules.GraveyardBonusPerCard;


        return points;
    }


    // =========================================================
    // PAIR COUNT
    // =========================================================
    //
    // n cards of the same rank make n(n-1)/2 distinct pairs.

    public static int PairCount(int sameRankCount)
    {
        if (sameRankCount < 2)
        {
            return 0;
        }

        return
            (sameRankCount * (sameRankCount - 1))
            / 2;
    }


    // =========================================================
    // PIP TOTAL
    // =========================================================

    public static int PipTotal(
        IReadOnlyList<CardValue> cards)
    {
        if (cards == null)
        {
            return 0;
        }

        int total = 0;

        for (int i = 0;
            i < cards.Count;
            i++)
        {
            total += cards[i].PipValue;
        }

        return total;
    }


    // =========================================================
    // ALL SAME RANK
    // =========================================================

    public static bool AllSameRank(
        IReadOnlyList<CardValue> cards)
    {
        if (cards == null ||
            cards.Count < 2)
        {
            return false;
        }

        CardRank rank = cards[0].Rank;

        for (int i = 1;
            i < cards.Count;
            i++)
        {
            if (cards[i].Rank != rank)
            {
                return false;
            }
        }

        return true;
    }


    // =========================================================
    // RUN CHECK
    // =========================================================
    //
    // Consecutive with no duplicates and no gaps.

    public static bool IsRun(
        IReadOnlyList<CardValue> cards)
    {
        if (cards == null ||
            cards.Count < 3)
        {
            return false;
        }

        // A run is the one rule that needs rank ARITHMETIC
        // rather than equality, so the enum is unwrapped here
        // and nowhere else. The underlying values are the real
        // card ranks (Ace 1 .. King 13), so ordering and
        // adjacency mean what they look like.
        List<int> ranks =
            new List<int>(cards.Count);

        for (int i = 0;
            i < cards.Count;
            i++)
        {
            ranks.Add((int)cards[i].Rank);
        }

        ranks.Sort();

        for (int i = 1;
            i < ranks.Count;
            i++)
        {
            if (ranks[i] != ranks[i - 1] + 1)
            {
                return false;
            }
        }

        return true;
    }


    // =========================================================
    // FLUSH CHECK
    // =========================================================

    public static bool IsFlush(
        IReadOnlyList<CardValue> cards)
    {
        if (cards == null ||
            cards.Count == 0)
        {
            return false;
        }

        CardSuit suit = cards[0].Suit;

        for (int i = 1;
            i < cards.Count;
            i++)
        {
            if (cards[i].Suit != suit)
            {
                return false;
            }
        }

        return true;
    }
}
