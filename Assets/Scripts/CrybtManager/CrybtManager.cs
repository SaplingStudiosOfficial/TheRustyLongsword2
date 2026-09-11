using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrybtManager : MonoBehaviour
{
    // =========================================================
    // ENCOUNTER STATES
    // =========================================================

    public enum EncounterStage
    {
        StartingEncounter,
        ChoosingOffering,
        ChoosingHero,
        ChoosingHeroFromGraveyard,
        BuildingCombination,
        ResolvingEncounter,
        EndingEncounter,
        EndingCrawl,
        ChoosingBoons,
        GameOver
    }

    [SerializeField] private EncounterStage currentStage;


    // =========================================================
    // CARD LISTS
    // =========================================================

    [SerializeField] private List<Card> Hand;
    [SerializeField] private List<Card> Deck;
    [SerializeField] private List<Card> Hero;
    [SerializeField] private List<Card> Monster;
    [SerializeField] private List<Card> Discard;
    [SerializeField] private List<Card> Offering;
    [SerializeField] private List<Card> Graveyard;

    private List<Card> Play = new List<Card>();

    // Prevents the exact same collection of cards
    // from being scored more than once.
    private List<string> scoredCombinations =
        new List<string>();


    // =========================================================
    // CARD POSITIONS
    // =========================================================

    [SerializeField] private GameObject HandSlot;
    [SerializeField] private GameObject DeckSlot;
    [SerializeField] private GameObject HeroSlot;
    [SerializeField] private GameObject MonsterSlot;
    [SerializeField] private GameObject DiscardSlot;
    [SerializeField] private GameObject OfferingSlot;
    [SerializeField] private GameObject GraveyardSlot;

    [SerializeField] private float cardSpacing = 200f;


    // =========================================================
    // PLAYER / GAME UI
    // =========================================================

    [SerializeField] private Text HealthDisplay;
    [SerializeField] private Text ModifierDisplay;
    [SerializeField] private Text ScoreDisplay;
    [SerializeField] private Text MonsterAPDisplay;
    [SerializeField] private Text EncounterDisplay;
    [SerializeField] private Text CrawlDisplay;
    [SerializeField] private Text OfferingPointsDisplay;
    [SerializeField] private GameObject TrashDisplay;
    [SerializeField] private GameObject AttackScore;


    // =========================================================
    // ENCOUNTER BUTTONS
    // =========================================================

    [SerializeField] private Button KeepHeroButton;
    [SerializeField] private Button SwitchHeroButton;
    [SerializeField] private Button PeekCardButton;
    [SerializeField] private Button ConfirmComboButton;
    [SerializeField] private Button EndEncounterButton;


    // =========================================================
    // BOON UI
    // =========================================================

    [SerializeField] private GameObject Boons;


    // =========================================================
    // PLAYER DATA
    // =========================================================

    [SerializeField] private int Health = 20;
    [SerializeField] private int Modifier = 0;

    private int encounterScore = 0;

    // Spendable points during the Boon Shop.
    private int offeringPoints = 0;


    // =========================================================
    // CRAWL DATA
    // =========================================================

    private int encounterNumber = 0;
    private int crawlNumber = 1;

    private const int encountersPerCrawl = 4;


    // =========================================================
    // CURRENT CRAWL BOONS
    // =========================================================

    private bool boon1Torch = false;
    private bool boon2WardingSigil = false;
    private bool boon3BoneCharm = false;
    private bool boon4HolyWater = false;
    private bool boon5BlackGrimoire = false;
    private bool boon6BrokenMirror = false;


    // =========================================================
    // NEXT CRAWL BOONS
    // =========================================================

    private bool nextCrawlTorch = false;
    private bool nextCrawlWardingSigil = false;
    private bool nextCrawlBoneCharm = false;
    private bool nextCrawlHolyWater = false;
    private bool nextCrawlBlackGrimoire = false;
    private bool nextCrawlBrokenMirror = false;


    // =========================================================
    // TORCH
    // =========================================================

    private Card peekedMonster = null;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        currentStage =
            EncounterStage.StartingEncounter;

        if (Boons != null)
        {
            Boons.SetActive(false);
        }

        offeringPoints = 0;

        ClearMonsterAPDisplay();

        UpdateEncounterButtons();
        UpdateDisplays();

        Shuffle();

        StartCrawl();
    }


    // =========================================================
    // START CRAWL
    // =========================================================

    private void StartCrawl()
    {
        encounterNumber = 0;

        Debug.Log(
            "================================"
        );

        Debug.Log(
            "Starting Crawl "
            + crawlNumber
        );

        Debug.Log(
            "Torch Active: "
            + boon1Torch
        );

        Debug.Log(
            "================================"
        );

        StartEncounter();
    }


    // =========================================================
    // END CRAWL
    // =========================================================

    private void EndCrawl()
    {
        currentStage =
            EncounterStage.EndingCrawl;

        UpdateEncounterButtons();

        ClearMonsterAPDisplay();

        Debug.Log(
            "Crawl "
            + crawlNumber
            + " complete."
        );

        // Draw final Offering card.
        DrawOfferingCard();

        // Calculate Boon Shop currency.
        offeringPoints =
            CalculateOfferingPoints();

        Debug.Log(
            "Offering Points available to spend: "
            + offeringPoints
        );

        // Reset purchases for the upcoming crawl.
        nextCrawlTorch = false;
        nextCrawlWardingSigil = false;
        nextCrawlBoneCharm = false;
        nextCrawlHolyWater = false;
        nextCrawlBlackGrimoire = false;
        nextCrawlBrokenMirror = false;

        // Enter Boon Shop.
        currentStage =
            EncounterStage.ChoosingBoons;

        UpdateEncounterButtons();

        if (Boons != null)
        {
            Boons.SetActive(true);
        }

        UpdateDisplays();
    }


    // =========================================================
    // START ENCOUNTER
    // =========================================================

    private void StartEncounter()
    {
        currentStage =
            EncounterStage.StartingEncounter;

        UpdateEncounterButtons();

        encounterScore = 0;

        peekedMonster = null;

        ClearMonsterAPDisplay();

        ClearPlay();

        scoredCombinations.Clear();

        Hero.Clear();
        Monster.Clear();

        encounterNumber++;

        Debug.Log(
            "Starting Encounter "
            + encounterNumber
            + " of "
            + encountersPerCrawl
        );

        DrawFive();

        // Player chooses Offering first.
        currentStage =
            EncounterStage.ChoosingOffering;

        UpdateEncounterButtons();
        UpdateDisplays();
    }


    // =========================================================
    // CARD CLICK
    // =========================================================

    public void CardClicked(Card clickedCard)
    {
        switch (currentStage)
        {
            case EncounterStage.ChoosingOffering:

                SelectOffering(clickedCard);

                break;


            case EncounterStage.ChoosingHeroFromGraveyard:

                SelectNewHero(clickedCard);

                break;


            case EncounterStage.BuildingCombination:

                SelectForCombination(clickedCard);

                break;
        }
    }


    // =========================================================
    // SELECT OFFERING
    // =========================================================

    private void SelectOffering(Card selectedCard)
    {
        if (!Hand.Contains(selectedCard))
        {
            return;
        }

        Hand.Remove(selectedCard);

        Offering.Add(selectedCard);

        selectedCard.DeactivateCard();

        MoveCards();

        DrawHero();
    }


    // =========================================================
    // DRAW HERO
    // =========================================================

    private void DrawHero()
    {
        CheckDeck();

        if (Deck.Count == 0)
        {
            Debug.Log(
                "No cards available to draw Hero."
            );

            return;
        }

        Hero.Add(
            Deck[0]
        );

        Deck.RemoveAt(0);

        currentStage =
            EncounterStage.ChoosingHero;

        MoveCards();

        UpdateEncounterButtons();

        Debug.Log(
            "Hero drawn."
        );

        Debug.Log(
            "Choose Keep Hero or Switch Hero."
        );
    }


    // =========================================================
    // KEEP HERO
    // =========================================================

    public void KeepHero()
    {
        if (currentStage !=
            EncounterStage.ChoosingHero)
        {
            return;
        }

        Debug.Log(
            "Hero kept."
        );

        currentStage =
            EncounterStage.StartingEncounter;

        UpdateEncounterButtons();

        RevealMonster();
    }


    // =========================================================
    // SWITCH HERO
    // =========================================================

    public void SwitchHero()
    {
        if (currentStage !=
            EncounterStage.ChoosingHero)
        {
            return;
        }

        if (Graveyard.Count == 0)
        {
            Debug.Log(
                "The Graveyard is empty."
            );

            return;
        }

        currentStage =
            EncounterStage.ChoosingHeroFromGraveyard;

        Debug.Log(
            "Choose one card from the Graveyard."
        );

        UpdateEncounterButtons();

        MoveCards();
    }

    private System.Collections.IEnumerator RotateHeroUprightAfterMove(
    Card card)
    {
        // This should match the card's movement duration.
        yield return new WaitForSeconds(
            0.25f
        );

        // Now that the card has arrived at the Hero slot,
        // smoothly rotate it back upright.
        yield return StartCoroutine(
            RotateCard(
                card,
                Quaternion.identity
            )
        );
    }


    // =========================================================
    // SELECT NEW HERO
    // =========================================================

    private void SelectNewHero(Card selectedCard)
    {
        if (currentStage !=
            EncounterStage.ChoosingHeroFromGraveyard)
        {
            return;
        }

        if (!Graveyard.Contains(selectedCard))
        {
            return;
        }

        if (Hero.Count == 0)
        {
            Debug.Log(
                "ERROR: There is no Hero to replace."
            );

            return;
        }

        Card oldHero =
            Hero[0];

        // Remove chosen card from Graveyard.
        Graveyard.Remove(
            selectedCard
        );

        // Remove current Hero.
        Hero.Remove(
            oldHero
        );

        // Old Hero is discarded.
        Discard.Add(
            oldHero
        );

        // Graveyard card becomes Hero.
        Hero.Add(
            selectedCard
        );

        Debug.Log(
            oldHero.gameObject.name
            + " was discarded."
        );

        Debug.Log(
            selectedCard.gameObject.name
            + " is now the Hero."
        );

        currentStage =
            EncounterStage.ChoosingHero;

        MoveCards();

        // Wait for the selected Graveyard card
        // to finish moving into the Hero slot,
        // then rotate it upright.
        StartCoroutine(
            RotateHeroUprightAfterMove(
                selectedCard
            )
        );

        UpdateEncounterButtons();
    }


    // =========================================================
    // TORCH - PEEK MONSTER
    // =========================================================

    public void PeekMonster()
    {
        if (currentStage !=
            EncounterStage.ChoosingHero)
        {
            return;
        }

        if (!boon1Torch)
        {
            Debug.Log(
                "Torch is not active."
            );

            return;
        }

        if (peekedMonster != null)
        {
            Debug.Log(
                "Monster has already been peeked."
            );

            return;
        }

        CheckDeck();

        if (Deck.Count == 0)
        {
            Debug.Log(
                "No Monster available to peek."
            );

            return;
        }

        peekedMonster =
            Deck[0];

        Deck.RemoveAt(0);

        peekedMonster.SetBasePosition(
            MonsterSlot.transform.position
        );

        peekedMonster.DeactivateCard();

        ShowMonsterAP(
            peekedMonster
        );

        Debug.Log(
            "Torch reveals: "
            + peekedMonster.gameObject.name
        );

        Debug.Log(
            "Peeked Monster AP: "
            + GetMonsterAP(peekedMonster)
        );

        UpdateEncounterButtons();
    }


    // =========================================================
    // REVEAL MONSTER
    // =========================================================

    private void RevealMonster()
    {
        if (peekedMonster != null)
        {
            Monster.Add(
                peekedMonster
            );

            peekedMonster = null;

            Debug.Log(
                "Peeked Monster officially revealed."
            );
        }
        else
        {
            CheckDeck();

            if (Deck.Count == 0)
            {
                Debug.Log(
                    "No cards available to draw Monster."
                );

                return;
            }

            Monster.Add(
                Deck[0]
            );

            Deck.RemoveAt(0);
        }

        currentStage =
            EncounterStage.BuildingCombination;

        if (Monster.Count > 0)
        {
            ShowMonsterAP(
                Monster[0]
            );
        }

        MoveCards();

        UpdateEncounterButtons();

        Debug.Log(
            "Monster revealed."
        );

        if (Monster.Count > 0)
        {
            Debug.Log(
                "Monster AP: "
                + GetMonsterAP(
                    Monster[0]
                )
            );
        }

        Debug.Log(
            "Build your combinations."
        );
    }


    // =========================================================
    // MONSTER AP
    // =========================================================

    private int GetMonsterAP(Card monsterCard)
    {
        if (monsterCard == null)
        {
            return 0;
        }

        // Monster AP =
        // Card Value + Modifier

        int AP =
            monsterCard.GetValue()
            + Modifier;

        return Mathf.Max(
            0,
            AP
        );
    }


    private void ShowMonsterAP(Card monsterCard)
    {
        if (MonsterAPDisplay == null)
        {
            return;
        }

        MonsterAPDisplay.text =
            GetMonsterAP(
                monsterCard
            ).ToString();
    }


    private void ClearMonsterAPDisplay()
    {
        if (MonsterAPDisplay == null)
        {
            return;
        }

        MonsterAPDisplay.text = "?";
    }


    // =========================================================
    // UPDATE ENCOUNTER BUTTONS
    // =========================================================

    private void UpdateEncounterButtons()
    {
        // Hide everything first.

        if (KeepHeroButton != null)
        {
            KeepHeroButton.gameObject.SetActive(
                false
            );
        }

        if (SwitchHeroButton != null)
        {
            SwitchHeroButton.gameObject.SetActive(
                false
            );
        }

        if (PeekCardButton != null)
        {
            PeekCardButton.gameObject.SetActive(
                false
            );
        }

        if (ConfirmComboButton != null)
        {
            ConfirmComboButton.gameObject.SetActive(
                false
            );
        }

        if (EndEncounterButton != null)
        {
            EndEncounterButton.gameObject.SetActive(
                false
            );
        }

        if (TrashDisplay != null)
        {
            TrashDisplay.gameObject.SetActive(
                false
            );
        }

        if (AttackScore != null)
        {
            AttackScore.gameObject.SetActive(
                false
            );
        }



        // =====================================================
        // CHOOSING OFFERING
        // =====================================================

        if (currentStage ==
            EncounterStage.ChoosingOffering)
        {
            if (TrashDisplay != null)
            {
                TrashDisplay.SetActive(true);
            }

            return;
        }


        // =====================================================
        // CHOOSING HERO
        // =====================================================

        if (currentStage ==
            EncounterStage.ChoosingHero)
        {
            if (KeepHeroButton != null)
            {
                KeepHeroButton.gameObject.SetActive(
                    true
                );
            }

            if (SwitchHeroButton != null && Graveyard.Count > 0)
            {
                SwitchHeroButton.gameObject.SetActive(
                    true
                );
            }

            if (PeekCardButton != null)
            {
                bool canPeek =
                    boon1Torch
                    &&
                    peekedMonster == null;

                PeekCardButton.gameObject.SetActive(
                    canPeek
                );
            }

            return;
        }


        // =====================================================
        // BUILDING COMBINATION
        // =====================================================

        if (currentStage ==
            EncounterStage.BuildingCombination)
        {
            if (ConfirmComboButton != null)
            {
                ConfirmComboButton.gameObject.SetActive(
                    true
                );
            }

            if (EndEncounterButton != null)
            {
                EndEncounterButton.gameObject.SetActive(
                    true
                );
            }

            if (AttackScore != null)
            {
                AttackScore.gameObject.SetActive(
                    true
                );
            }

            return;
        }
    }


    // =========================================================
    // COMBINATION SELECTION
    // =========================================================

    private void SelectForCombination(Card selectedCard)
    {
        bool validCard =
            Hand.Contains(
                selectedCard
            );

        // Hero can participate in combinations.
        if (Hero.Contains(selectedCard))
        {
            validCard = true;
        }

        if (!validCard)
        {
            return;
        }

        // Deselect card.
        if (Play.Contains(selectedCard))
        {
            Play.Remove(
                selectedCard
            );

            selectedCard.DeselectCard();

            Debug.Log(
                selectedCard.gameObject.name
                + " removed from combination."
            );
        }

        // Select card.
        else
        {
            Play.Add(
                selectedCard
            );

            selectedCard.SelectCard();

            Debug.Log(
                selectedCard.gameObject.name
                + " added to combination."
            );
        }

        Debug.Log(
            "Cards selected: "
            + Play.Count
        );
    }


    // =========================================================
    // CONFIRM COMBINATION
    // =========================================================

    public void ConfirmCombination()
    {
        if (currentStage !=
            EncounterStage.BuildingCombination)
        {
            return;
        }

        if (Play.Count == 0)
        {
            Debug.Log(
                "Select cards before confirming."
            );

            return;
        }

        string combinationID =
            GetCombinationID(
                Play
            );

        // Prevent exact same group from scoring twice.
        if (scoredCombinations.Contains(
            combinationID))
        {
            Debug.Log(
                "This combination has already been scored."
            );

            ClearPlay();

            return;
        }

        int points =
            CalculateCombination(
                Play
            );

        if (points <= 0)
        {
            Debug.Log(
                "That is not a scoring combination."
            );

            ClearPlay();

            return;
        }

        scoredCombinations.Add(
            combinationID
        );

        encounterScore +=
            points;

        Debug.Log(
            "Combination scored "
            + points
            + " points."
        );

        Debug.Log(
            "Encounter Score: "
            + encounterScore
        );

        if (Monster.Count > 0)
        {
            Debug.Log(
                "Monster AP: "
                + GetMonsterAP(
                    Monster[0]
                )
                + " | Player Score: "
                + encounterScore
            );
        }

        ClearPlay();

        UpdateDisplays();
    }


    // =========================================================
    // CLEAR PLAY
    // =========================================================

    private void ClearPlay()
    {
        for (int i = 0;
            i < Play.Count;
            i++)
        {
            Play[i].DeselectCard();
        }

        Play.Clear();
    }


    // =========================================================
    // COMBINATION ID
    // =========================================================

    private string GetCombinationID(
        List<Card> cards)
    {
        List<int> IDs =
            new List<int>();

        for (int i = 0;
            i < cards.Count;
            i++)
        {
            IDs.Add(
                cards[i].GetInstanceID()
            );
        }

        IDs.Sort();

        string combinationID = "";

        for (int i = 0;
            i < IDs.Count;
            i++)
        {
            combinationID +=
                IDs[i].ToString()
                + "-";
        }

        return combinationID;
    }


    // =========================================================
    // CALCULATE COMBINATION
    // =========================================================
    //
    // IMPORTANT:
    //
    // The EXACT cards selected by the player must form
    // the scoring combination.
    //
    // The code does NOT search through a large random
    // selection looking for smaller hidden combinations.
    //
    // Examples:
    //
    // 5 + King
    // = 15
    // = 2 points
    //
    // 7 + 7
    // = Pair
    // = 2 points
    //
    // 7 + 7 + 7
    // = Triple
    // = 6 points
    //
    // 2 + 3 + 4
    // = Run
    // = 3 points
    //
    // 2 + 3 + 4 + King
    // = NOT a run
    //
    // =========================================================

    private int CalculateCombination(
        List<Card> cards)
    {
        if (cards == null ||
            cards.Count == 0)
        {
            return 0;
        }

        int points = 0;


        // =====================================================
        // FIFTEEN
        // =====================================================
        //
        // The entire selected group must total exactly 15.

        int total = 0;

        for (int i = 0;
            i < cards.Count;
            i++)
        {
            total +=
                cards[i].GetValue();
        }

        if (cards.Count >= 2 &&
            total == 15)
        {
            points += 2;

            Debug.Log(
                "Fifteen! +2"
            );
        }


        // =====================================================
        // PAIR / TRIPLE / FOUR OF A KIND
        // =====================================================
        //
        // ALL selected cards must have the same rank.
        //
        // 2 cards:
        // 1 pair = 2 points
        //
        // 3 cards:
        // 3 pairs = 6 points
        //
        // 4 cards:
        // 6 pairs = 12 points

        if (cards.Count >= 2 &&
            cards.Count <= 4 &&
            AllSameRank(cards))
        {
            int pairCount =
                (cards.Count *
                (cards.Count - 1))
                / 2;

            int pairPoints =
                pairCount * 2;

            points +=
                pairPoints;

            Debug.Log(
                "Matching ranks! +"
                + pairPoints
            );
        }


        // =====================================================
        // RUN
        // =====================================================
        //
        // The ENTIRE selected group must be consecutive.
        //
        // Ace = 1
        // Jack = 11
        // Queen = 12
        // King = 13

        if (cards.Count >= 3 &&
            IsRun(cards))
        {
            points +=
                cards.Count;

            Debug.Log(
                "Run of "
                + cards.Count
                + "! +"
                + cards.Count
            );
        }


        // =====================================================
        // FLUSH
        // =====================================================
        //
        // The entire selected group must be the same suit.
        //
        // 4-card Flush = 4
        // 5-card Flush = 5

        if (cards.Count == 4 &&
            IsFlush(cards))
        {
            points += 4;

            Debug.Log(
                "Four-card Flush! +4"
            );
        }

        else if (cards.Count == 5 &&
                 IsFlush(cards))
        {
            points += 5;

            Debug.Log(
                "Five-card Flush! +5"
            );
        }


        Debug.Log(
            "Combination total: "
            + points
        );

        return points;
    }


    // =========================================================
    // ALL SAME RANK
    // =========================================================

    private bool AllSameRank(
        List<Card> cards)
    {
        if (cards == null ||
            cards.Count < 2)
        {
            return false;
        }

        int rank =
            cards[0].GetRank();

        for (int i = 1;
            i < cards.Count;
            i++)
        {
            if (cards[i].GetRank()
                != rank)
            {
                return false;
            }
        }

        return true;
    }


    // =========================================================
    // RUN CHECK
    // =========================================================

    private bool IsRun(
        List<Card> cards)
    {
        if (cards == null ||
            cards.Count < 3)
        {
            return false;
        }

        List<int> ranks =
            new List<int>();

        for (int i = 0;
            i < cards.Count;
            i++)
        {
            ranks.Add(
                cards[i].GetRank()
            );
        }

        ranks.Sort();

        for (int i = 1;
            i < ranks.Count;
            i++)
        {
            if (ranks[i]
                != ranks[i - 1] + 1)
            {
                return false;
            }
        }

        return true;
    }


    // =========================================================
    // FLUSH CHECK
    // =========================================================

    private bool IsFlush(
        List<Card> cards)
    {
        if (cards == null ||
            cards.Count == 0)
        {
            return false;
        }

        int suit =
            cards[0].GetSuit();

        for (int i = 1;
            i < cards.Count;
            i++)
        {
            if (cards[i].GetSuit()
                != suit)
            {
                return false;
            }
        }

        return true;
    }


    // =========================================================
    // FINISH SCORING
    // =========================================================

    public void FinishScoring()
    {
        if (currentStage !=
            EncounterStage.BuildingCombination)
        {
            return;
        }

        ClearPlay();

        currentStage =
            EncounterStage.ResolvingEncounter;

        UpdateEncounterButtons();

        ResolveEncounter();
    }


    // =========================================================
    // RESOLVE ENCOUNTER
    // =========================================================

    private void ResolveEncounter()
    {
        if (Monster.Count == 0)
        {
            Debug.Log(
                "ERROR: No Monster exists."
            );

            return;
        }

        int monsterAP =
            GetMonsterAP(
                Monster[0]
            );

        // Damage =
        // Monster AP - Combination Points

        int damage =
            monsterAP
            - encounterScore;

        damage =
            Mathf.Max(
                0,
                damage
            );

        Debug.Log(
            "Monster AP: "
            + monsterAP
            + " | Combination Points: "
            + encounterScore
            + " | Damage: "
            + damage
        );


        // =====================================================
        // ZERO DAMAGE
        // =====================================================

        if (damage == 0)
        {
            Debug.Log(
                "Monster AP matched! "
                + "Monster goes to Graveyard."
            );

            DefeatMonster();
        }


        // =====================================================
        // DAMAGE
        // =====================================================

        else
        {
            TakeDamage(
                damage
            );

            if (Health <= 0)
            {
                return;
            }
        }

        EndEncounter();
    }


    // =========================================================
    // DEFEAT MONSTER
    // =========================================================

    private void DefeatMonster()
    {
        if (Monster.Count == 0)
        {
            Debug.Log(
                "ERROR: No Monster to move to Graveyard."
            );

            return;
        }

        Card defeatedMonster =
            Monster[0];

        Monster.RemoveAt(0);

        Graveyard.Add(
            defeatedMonster
        );

        Debug.Log(
            defeatedMonster.gameObject.name
            + " moved to Graveyard."
        );

        Debug.Log(
            "Graveyard now contains "
            + Graveyard.Count
            + " card(s)."
        );
    }


    // =========================================================
    // DAMAGE
    // =========================================================

    private void TakeDamage(
        int damage)
    {
        Health -=
            damage;

        Debug.Log(
            "Player takes "
            + damage
            + " damage."
        );

        if (Health <= 0)
        {
            Health = 0;

            GameOver();

            return;
        }

        UpdateDisplays();
    }


    // =========================================================
    // END ENCOUNTER
    // =========================================================

    private void EndEncounter()
    {
        if (currentStage ==
            EncounterStage.GameOver)
        {
            return;
        }

        currentStage =
            EncounterStage.EndingEncounter;

        UpdateEncounterButtons();

        ClearMonsterAPDisplay();

        ClearPlay();


        // =====================================================
        // HAND -> DISCARD
        // =====================================================

        while (Hand.Count > 0)
        {
            Discard.Add(
                Hand[0]
            );

            Hand.RemoveAt(0);
        }


        // =====================================================
        // HERO -> DISCARD
        // =====================================================

        while (Hero.Count > 0)
        {
            Discard.Add(
                Hero[0]
            );

            Hero.RemoveAt(0);
        }


        // =====================================================
        // MONSTER -> DISCARD
        // =====================================================
        //
        // If the Monster dealt 0 damage it has already
        // been moved to Graveyard by DefeatMonster().

        while (Monster.Count > 0)
        {
            Discard.Add(
                Monster[0]
            );

            Monster.RemoveAt(0);
        }


        // Safety for Torch peek.
        if (peekedMonster != null)
        {
            Discard.Add(
                peekedMonster
            );

            peekedMonster = null;
        }

        MoveCards();


        // =====================================================
        // NEXT ENCOUNTER / END CRAWL
        // =====================================================

        if (encounterNumber >=
            encountersPerCrawl)
        {
            EndCrawl();
        }

        else
        {
            StartEncounter();
        }
    }


    // =========================================================
    // DRAW FIVE
    // =========================================================

    private void DrawFive()
    {
        for (int i = 0;
            i < 5;
            i++)
        {
            CheckDeck();

            if (Deck.Count == 0)
            {
                Debug.Log(
                    "No more cards available."
                );

                break;
            }

            Hand.Add(
                Deck[0]
            );

            Deck.RemoveAt(0);
        }

        MoveCards();
    }


    // =========================================================
    // FINAL OFFERING DRAW
    // =========================================================

    private void DrawOfferingCard()
    {
        CheckDeck();

        if (Deck.Count == 0)
        {
            Debug.Log(
                "No card available for final Offering draw."
            );

            return;
        }

        Offering.Add(
            Deck[0]
        );

        Deck.RemoveAt(0);

        Debug.Log(
            "Final Offering card drawn."
        );

        MoveCards();
    }


    // =========================================================
    // CALCULATE OFFERING POINTS
    // =========================================================

    private int CalculateOfferingPoints()
    {
        int points = 0;


        // =====================================================
        // FIFTEENS
        // =====================================================
        //
        // Offering scoring searches every subset because
        // the Offering is being scored as a whole pile,
        // unlike manually confirmed encounter combinations.

        int subsetCount =
            1 << Offering.Count;

        for (int mask = 1;
            mask < subsetCount;
            mask++)
        {
            int total = 0;

            for (int i = 0;
                i < Offering.Count;
                i++)
            {
                if ((mask & (1 << i))
                    != 0)
                {
                    total +=
                        Offering[i].GetValue();
                }
            }

            if (total == 15)
            {
                points += 2;
            }
        }


        // =====================================================
        // PAIRS
        // =====================================================

        for (int i = 0;
            i < Offering.Count;
            i++)
        {
            for (int j = i + 1;
                j < Offering.Count;
                j++)
            {
                if (Offering[i].GetRank()
                    == Offering[j].GetRank())
                {
                    points += 2;
                }
            }
        }


        // =====================================================
        // GRAVEYARD BONUS
        // =====================================================

        points +=
            Graveyard.Count;

        Debug.Log(
            "Offering scored "
            + points
            + " spendable points."
        );

        return points;
    }


    // =========================================================
    // SPEND OFFERING POINTS
    // =========================================================

    private bool SpendOfferingPoints(
        int cost)
    {
        if (currentStage !=
            EncounterStage.ChoosingBoons)
        {
            return false;
        }

        if (offeringPoints < cost)
        {
            Debug.Log(
                "Not enough Offering Points."
            );

            return false;
        }

        offeringPoints -=
            cost;

        Debug.Log(
            "Spent "
            + cost
            + " Offering Points."
        );

        Debug.Log(
            "Remaining Offering Points: "
            + offeringPoints
        );

        UpdateDisplays();

        return true;
    }


    // =========================================================
    // BOON 1 - TORCH
    // =========================================================

    public void BuyBoon1()
    {
        const int cost = 2;

        if (nextCrawlTorch)
        {
            Debug.Log(
                "Torch already purchased for next Crawl."
            );

            return;
        }

        if (!SpendOfferingPoints(cost))
        {
            return;
        }

        nextCrawlTorch = true;

        Debug.Log(
            "Torch purchased for Crawl "
            + (crawlNumber + 1)
            + "."
        );
    }


    // =========================================================
    // BOON 2
    // =========================================================

    public void BuyBoon2()
    {
        const int cost = 4;

        if (nextCrawlWardingSigil)
        {
            Debug.Log(
                "Boon 2 already purchased."
            );

            return;
        }

        if (!SpendOfferingPoints(cost))
        {
            return;
        }

        nextCrawlWardingSigil = true;

        Debug.Log(
            "Warding Sigil purchased."
        );
    }


    // =========================================================
    // BOON 3
    // =========================================================

    public void BuyBoon3()
    {
        const int cost = 6;

        if (nextCrawlBoneCharm)
        {
            Debug.Log(
                "Boon 3 already purchased."
            );

            return;
        }

        if (!SpendOfferingPoints(cost))
        {
            return;
        }

        nextCrawlBoneCharm = true;

        Debug.Log(
            "Bone Charm purchased."
        );
    }


    // =========================================================
    // BOON 4
    // =========================================================

    public void BuyBoon4()
    {
        const int cost = 8;

        if (nextCrawlHolyWater)
        {
            Debug.Log(
                "Boon 4 already purchased."
            );

            return;
        }

        if (!SpendOfferingPoints(cost))
        {
            return;
        }

        nextCrawlHolyWater = true;

        Debug.Log(
            "Holy Water purchased."
        );
    }


    // =========================================================
    // BOON 5
    // =========================================================

    public void BuyBoon5()
    {
        const int cost = 10;

        if (nextCrawlBlackGrimoire)
        {
            Debug.Log(
                "Boon 5 already purchased."
            );

            return;
        }

        if (!SpendOfferingPoints(cost))
        {
            return;
        }

        nextCrawlBlackGrimoire = true;

        Debug.Log(
            "Black Grimoire purchased."
        );
    }


    // =========================================================
    // BOON 6
    // =========================================================

    public void BuyBoon6()
    {
        const int cost = 12;

        if (nextCrawlBrokenMirror)
        {
            Debug.Log(
                "Boon 6 already purchased."
            );

            return;
        }

        if (!SpendOfferingPoints(cost))
        {
            return;
        }

        nextCrawlBrokenMirror = true;

        Debug.Log(
            "Broken Mirror purchased."
        );
    }


    // =========================================================
    // FINISH BOON PHASE
    // =========================================================

    public void FinishBoonPhase()
    {
        if (currentStage !=
            EncounterStage.ChoosingBoons)
        {
            return;
        }

        if (Boons != null)
        {
            Boons.SetActive(false);
        }

        CleanUpCrawl();


        // =====================================================
        // CURRENT BOONS EXPIRE
        // =====================================================

        boon1Torch = false;
        boon2WardingSigil = false;
        boon3BoneCharm = false;
        boon4HolyWater = false;
        boon5BlackGrimoire = false;
        boon6BrokenMirror = false;


        // =====================================================
        // PURCHASED BOONS BECOME ACTIVE
        // =====================================================

        boon1Torch =
            nextCrawlTorch;

        boon2WardingSigil =
            nextCrawlWardingSigil;

        boon3BoneCharm =
            nextCrawlBoneCharm;

        boon4HolyWater =
            nextCrawlHolyWater;

        boon5BlackGrimoire =
            nextCrawlBlackGrimoire;

        boon6BrokenMirror =
            nextCrawlBrokenMirror;


        // =====================================================
        // RESET NEXT CRAWL PURCHASES
        // =====================================================

        nextCrawlTorch = false;
        nextCrawlWardingSigil = false;
        nextCrawlBoneCharm = false;
        nextCrawlHolyWater = false;
        nextCrawlBlackGrimoire = false;
        nextCrawlBrokenMirror = false;


        // Unspent points do not carry over.
        offeringPoints = 0;

        crawlNumber++;

        currentStage =
            EncounterStage.StartingEncounter;

        UpdateEncounterButtons();

        UpdateDisplays();

        StartCrawl();
    }


    // =========================================================
    // CLEAN UP CRAWL
    // =========================================================

    private void CleanUpCrawl()
    {
        // Offering cards move to Discard
        // after the Boon Shop.

        while (Offering.Count > 0)
        {
            Discard.Add(
                Offering[0]
            );

            Offering.RemoveAt(0);
        }

        MoveCards();
    }


    // =========================================================
    // CHECK DECK
    // =========================================================

    private void CheckDeck()
    {
        if (Deck.Count == 0)
        {
            ReshuffleDiscard();
        }
    }


    // =========================================================
    // SHUFFLE
    // =========================================================

    private void Shuffle()
    {
        for (int i = 0;
            i < Deck.Count;
            i++)
        {
            int randomIndex =
                Random.Range(
                    i,
                    Deck.Count
                );

            Card temp =
                Deck[i];

            Deck[i] =
                Deck[randomIndex];

            Deck[randomIndex] =
                temp;
        }
    }


    // =========================================================
    // RESHUFFLE DISCARD
    // =========================================================

    private void ReshuffleDiscard()
    {
        while (Discard.Count > 0)
        {
            Deck.Add(
                Discard[0]
            );

            Discard.RemoveAt(0);
        }

        Shuffle();

        MoveCards();

        Debug.Log(
            "Discard reshuffled into Deck."
        );
    }


    // =========================================================
    // GAME OVER
    // =========================================================

    private void GameOver()
    {
        currentStage =
            EncounterStage.GameOver;

        ClearMonsterAPDisplay();

        if (Boons != null)
        {
            Boons.SetActive(false);
        }

        UpdateEncounterButtons();

        UpdateDisplays();

        Debug.Log(
            "GAME OVER - Reached Crawl "
            + crawlNumber
        );
    }


    // =========================================================
    // UPDATE UI
    // =========================================================

    private void UpdateDisplays()
    {
        if (HealthDisplay != null)
        {
            HealthDisplay.text =
                Health.ToString();
        }

        if (ModifierDisplay != null)
        {
            ModifierDisplay.text =
                Modifier.ToString();
        }

        if (ScoreDisplay != null)
        {
            ScoreDisplay.text =
                encounterScore.ToString();
        }

        if (EncounterDisplay != null)
        {
            EncounterDisplay.text =
                encounterNumber
                + " / "
                + encountersPerCrawl;
        }

        if (CrawlDisplay != null)
        {
            CrawlDisplay.text =
                crawlNumber.ToString();
        }


        // =====================================================
        // OFFERING POINTS
        // =====================================================
        //
        // Only visible during the Boon Shop.

        if (OfferingPointsDisplay != null)
        {
            bool showOfferingPoints =
                currentStage ==
                EncounterStage.ChoosingBoons;

            OfferingPointsDisplay.gameObject.SetActive(
                showOfferingPoints
            );

            if (showOfferingPoints)
            {
                OfferingPointsDisplay.text =
                    offeringPoints.ToString();
            }
        }
    }

    private System.Collections.IEnumerator RotateCard(
    Card card,
    Quaternion targetRotation)
    {
        Quaternion startRotation =
            card.transform.localRotation;

        float duration = 0.25f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / duration
                );

            // Quadratic ease in/out.
            // Matches the feel of the existing
            // card movement animation.
            float easedT;

            if (t < 0.5f)
            {
                easedT =
                    2f * t * t;
            }
            else
            {
                easedT =
                    1f -
                    Mathf.Pow(
                        -2f * t + 2f,
                        2f
                    ) / 2f;
            }

            card.transform.localRotation =
                Quaternion.LerpUnclamped(
                    startRotation,
                    targetRotation,
                    easedT
                );

            yield return null;
        }

        card.transform.localRotation =
            targetRotation;
    }


    // =========================================================
    // MOVE CARDS
    // =========================================================

    private void MoveCards()
    {
        // =====================================================
        // DECK
        // =====================================================

        for (int i = 0;
            i < Deck.Count;
            i++)
        {
            Deck[i].SetBasePosition(
                DeckSlot.transform.position
            );

            Deck[i].DeactivateCard();
        }


        // =====================================================
        // HAND
        // =====================================================

        for (int i = 0;
            i < Hand.Count;
            i++)
        {
            float offset =
                (i - (Hand.Count - 1) / 2f)
                * cardSpacing;

            Vector3 position =
                HandSlot.transform.position
                + new Vector3(
                    offset,
                    0,
                    0
                );

            Hand[i].SetBasePosition(
                position
            );

            Hand[i].ActivateCard();
        }


        // =====================================================
        // HERO
        // =====================================================

        for (int i = 0;
            i < Hero.Count;
            i++)
        {
            Hero[i].SetBasePosition(
                HeroSlot.transform.position
            );

            // Hero participates in combinations.
            if (currentStage ==
                EncounterStage.BuildingCombination)
            {
                Hero[i].ActivateCard();
            }

            else
            {
                Hero[i].DeactivateCard();
            }
        }


        // =====================================================
        // MONSTER
        // =====================================================

        for (int i = 0;
            i < Monster.Count;
            i++)
        {
            Monster[i].SetBasePosition(
                MonsterSlot.transform.position
            );

            Monster[i].DeactivateCard();
        }


        // =====================================================
        // OFFERING
        // =====================================================

        for (int i = 0;
            i < Offering.Count;
            i++)
        {
            Offering[i].SetBasePosition(
                OfferingSlot.transform.position
            );

            Offering[i].DeactivateCard();
        }

        // =====================================================
        // GRAVEYARD
        // =====================================================

        for (int i = 0;
            i < Graveyard.Count;
            i++)
        {
            // Always spread Graveyard cards vertically.
            float offset =
                (i - (Graveyard.Count - 1) / 2f)
                * cardSpacing;

            Vector3 position =
                GraveyardSlot.transform.position
                + new Vector3(
                    0,
                    offset,
                    0
                );

            Graveyard[i].SetBasePosition(
                position
            );


            // =====================================================
            // SELECTING A HERO FROM GRAVEYARD
            // =====================================================

            if (currentStage ==
                EncounterStage.ChoosingHeroFromGraveyard)
            {
                // Smoothly rotate sideways.
                StartCoroutine(
                    RotateCard(
                        Graveyard[i],
                        Quaternion.Euler(
                            0f,
                            0f,
                            90f
                        )
                    )
                );

                // Bring the selectable cards forward.
                Graveyard[i].transform.SetAsLastSibling();

                Graveyard[i].ActivateCard();
            }


            // =====================================================
            // NORMAL GRAVEYARD DISPLAY
            // =====================================================

            else
            {
                // Keep the cards vertically spread,
                // but smoothly return them upright.
                StartCoroutine(
                    RotateCard(
                        Graveyard[i],
                        Quaternion.identity
                    )
                );

                Graveyard[i].DeactivateCard();
            }
        }


        // =====================================================
        // DISCARD
        // =====================================================

        for (int i = 0;
            i < Discard.Count;
            i++)
        {
            Discard[i].SetBasePosition(
                DiscardSlot.transform.position
            );

            Discard[i].DeactivateCard();
        }
    }
}