using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// =============================================================
// CRYBT MANAGER
// =============================================================
//
// Coordinates the encounter stage machine.
//
// WHAT THIS CLASS NO LONGER DOES:
//
// Scoring maths ......... CrybtScoring  (pure, testable)
// Damage maths .......... CrybtCombat   (pure, testable)
// Easing ................ Easing        (shared with Card)
// Boon bookkeeping ...... BoonShop
// Every Button and Text . EncounterHud
// Tuning numbers ........ CrybtRules asset
//
// It coordinates. It does not calculate, and it does not draw.
// =============================================================

public class CrybtManager : MonoBehaviour, ICardClickHandler
{
    // =========================================================
    // RULES
    // =========================================================

    [Header("Rules")]

    [SerializeField]
    [Tooltip("Tuning asset. Create via Assets > Create > Crybt > Rules.")]
    private CrybtRules rules;


    // =========================================================
    // HUD
    // =========================================================

    [Header("HUD")]

    [SerializeField]
    [Tooltip("Owns every Button and Text. See EncounterHud.")]
    private EncounterHud hud;


    // =========================================================
    // BOON SHOP
    // =========================================================

    [Header("Boons")]

    [SerializeField]
    private BoonShop boonShop = new BoonShop();


    // =========================================================
    // LEGACY UI REFERENCES - MIGRATION ONLY
    // =========================================================
    //
    // These fields used to drive the HUD directly. They are
    // kept, hidden, for ONE release so that the references
    // already wired up in the Crybt scene are not lost when
    // the class changes.
    //
    // TO MIGRATE:
    //
    //   1. Open the Crybt scene.
    //   2. Tools > Crybt > Migrate HUD References
    //   3. Save the scene.
    //   4. Delete this entire block and the EncounterHudMigration
    //      editor script.
    //
    // Do not add anything new here.

    // 0649 = never assigned (Unity assigns these from the scene)
    // 0169 = never used     (nothing reads them any more - that
    //                        is the point; the migration tool
    //                        copies them across)
#pragma warning disable 0649, 0169

    [HideInInspector] [SerializeField] private UnityEngine.UI.Button KeepHeroButton;
    [HideInInspector] [SerializeField] private UnityEngine.UI.Button SwitchHeroButton;
    [HideInInspector] [SerializeField] private UnityEngine.UI.Button PeekCardButton;
    [HideInInspector] [SerializeField] private UnityEngine.UI.Button ConfirmComboButton;
    [HideInInspector] [SerializeField] private UnityEngine.UI.Button EndEncounterButton;

    [HideInInspector] [SerializeField] private GameObject TrashDisplay;
    [HideInInspector] [SerializeField] private GameObject AttackScore;
    [HideInInspector] [SerializeField] private GameObject Boons;

    [HideInInspector] [SerializeField] private UnityEngine.UI.Text HealthDisplay;
    [HideInInspector] [SerializeField] private UnityEngine.UI.Text ModifierDisplay;
    [HideInInspector] [SerializeField] private UnityEngine.UI.Text ScoreDisplay;
    [HideInInspector] [SerializeField] private UnityEngine.UI.Text MonsterAPDisplay;
    [HideInInspector] [SerializeField] private UnityEngine.UI.Text EncounterDisplay;
    [HideInInspector] [SerializeField] private UnityEngine.UI.Text CrawlDisplay;
    [HideInInspector] [SerializeField] private UnityEngine.UI.Text OfferingPointsDisplay;

#pragma warning restore 0649, 0169


    // =========================================================
    // ENCOUNTER STATE
    // =========================================================

    [SerializeField] private EncounterStage currentStage;


    // =========================================================
    // CARD LISTS
    // =========================================================

    [Header("Card Lists")]

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

    // Reused when handing card data to CrybtScoring so that
    // scoring a combination does not allocate a new list.
    private readonly List<CardValue> scoringBuffer =
        new List<CardValue>();

    // Same idea for the score breakdown shown under the score:
    // the preview is rebuilt on every card toggle, so neither
    // the rule list nor the string builder is reallocated.
    private readonly List<ScoreItem> breakdownBuffer =
        new List<ScoreItem>();

    private readonly System.Text.StringBuilder breakdownText =
        new System.Text.StringBuilder();

    [SerializeField]
    [Tooltip("Shown under the score when the selected cards " +
             "form no scoring combination.")]
    private string noScoreText = "No score";


    // =========================================================
    // CARD POSITIONS
    // =========================================================

    [Header("Card Slots")]

    [SerializeField] private GameObject HandSlot;
    [SerializeField] private GameObject DeckSlot;
    [SerializeField] private GameObject HeroSlot;
    [SerializeField] private GameObject MonsterSlot;
    [SerializeField] private GameObject DiscardSlot;
    [SerializeField] private GameObject OfferingSlot;
    [SerializeField] private GameObject GraveyardSlot;

    [SerializeField] private float cardSpacing = 200f;


    // =========================================================
    // PLAYER DATA
    // =========================================================
    //
    // These stay serialized here rather than moving into
    // CrybtRules because they already hold values authored in
    // the Crybt scene.

    [Header("Player")]

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


    // =========================================================
    // TORCH
    // =========================================================

    private Card peekedMonster = null;


    // =========================================================
    // MONSTER ATTACK POWER READOUT
    // =========================================================
    //
    // null renders as "?" - the Monster has not been revealed.

    private int? displayedMonsterAttackPower = null;


    // =========================================================
    // CARD ROTATION
    // =========================================================
    //
    // WHY THIS DICTIONARY EXISTS:
    //
    // MoveCards() is called from ten different places and used
    // to start a fresh rotation coroutine for every Graveyard
    // card each time, without stopping the previous ones.
    //
    // Two coroutines animating the same transform, each lerping
    // from a different captured start rotation, made cards snap
    // and jitter on the EndEncounter -> StartEncounter path.
    //
    // One rotation per card, tracked and cancelled here.

    private readonly Dictionary<Card, Coroutine> rotationRoutines =
        new Dictionary<Card, Coroutine>();

    private const float rotationDuration = 0.25f;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        EnsureRules();

        EnsureBoons();

        EnsureDeck();

        EnsureHud();

        BindCards();

        currentStage =
            EncounterStage.StartingEncounter;

        offeringPoints = 0;

        displayedMonsterAttackPower = null;

        RefreshHud();

        Shuffle();

        StartCrawl();
    }


    // =========================================================
    // ENSURE RULES
    // =========================================================
    //
    // The game should still run if the rules asset has not
    // been wired up yet, but it should say so loudly.

    // Shipped copies of the tuning data, loaded when nothing is
    // wired up in the Inspector. Under Resources/ so they load
    // with no scene reference and are guaranteed into the build.
    [SerializeField]
    [Tooltip("Optional. Builds the deck at runtime when the " +
             "Deck list below is empty, so the 52 card objects " +
             "can be deleted from the scene.")]
    private DeckDefinition deckDefinition;

    [SerializeField]
    [Tooltip("Optional parent for runtime-built cards. " +
             "Defaults to the Deck slot, which is where the " +
             "scene-authored cards already live.")]
    private Transform cardParent;


    private const string RulesResourcePath = "Crybt/CrybtRules";

    private const string BoonsResourceFolder = "Crybt/Boons";


    private void EnsureRules()
    {
        if (rules != null)
        {
            return;
        }

        rules =
            Resources.Load<CrybtRules>(RulesResourcePath);

        if (rules != null)
        {
            return;
        }

        GameLog.Warning(
            "CrybtManager has no CrybtRules asset assigned and "
            + "none was found at Resources/" + RulesResourcePath
            + ". Falling back to built-in defaults."
        );

        rules =
            ScriptableObject.CreateInstance<CrybtRules>();
    }


    // =========================================================
    // ENSURE BOONS
    // =========================================================
    //
    // An empty catalogue is not a cosmetic problem: Torch is
    // the one boon that actually does something, and with no
    // catalogue it cannot be bought at all.

    private void EnsureBoons()
    {
        if (boonShop == null)
        {
            return;
        }

        if (boonShop.Catalogue != null &&
            boonShop.Catalogue.Count > 0)
        {
            return;
        }

        BoonDefinition[] shipped =
            Resources.LoadAll<BoonDefinition>(
                BoonsResourceFolder
            );

        if (boonShop.FillIfEmpty(shipped))
        {
            return;
        }

        GameLog.Warning(
            "CrybtManager has an empty Boon Shop catalogue and "
            + "no boon assets were found in Resources/"
            + BoonsResourceFolder
            + ". No boon can be purchased."
        );
    }


    // =========================================================
    // ENSURE DECK
    // =========================================================
    //
    // Builds the deck from a DeckDefinition when the scene has
    // no cards of its own.
    //
    // The scene ALWAYS wins. While the 52 card objects are
    // still in the Deck list this does nothing at all, which is
    // what makes the migration safe to land before the scene is
    // touched: assign the asset, verify, then delete the
    // objects and this takes over.

    private void EnsureDeck()
    {
        if (Deck == null)
        {
            Deck = new List<Card>();
        }

        if (Deck.Count > 0)
        {
            return;
        }

        if (deckDefinition == null ||
            deckDefinition.Count == 0)
        {
            GameLog.Error(
                "CrybtManager has no cards. The Deck list is "
                + "empty and no DeckDefinition is assigned, so "
                + "there is nothing to deal. "
                + "FIX: assign a Deck asset, or run "
                + "Tools > Crybt > Generate Card Definitions "
                + "From Prefabs to create one."
            );

            return;
        }

        GameObject template = deckDefinition.CardPrefab;

        if (template == null)
        {
            GameLog.Error(
                "The Deck asset has no card prefab assigned, "
                + "so no card can be built."
            );

            return;
        }

        Transform parent =
            cardParent != null
                ? cardParent
                : (DeckSlot != null
                    ? DeckSlot.transform
                    : transform);

        IReadOnlyList<DeckEntry> entries =
            deckDefinition.Cards;

        for (int i = 0; i < entries.Count; i++)
        {
            DeckEntry entry = entries[i];

            if (entry == null)
            {
                continue;
            }

            GameObject built =
                Instantiate(template, parent);

            built.name =
                entry.Rank + "_" + entry.Suit;

            Card card =
                built.GetComponent<Card>();

            if (card == null)
            {
                GameLog.Error(
                    "The Deck asset's card prefab has no Card "
                    + "component."
                );

                Destroy(built);

                return;
            }

            card.Bind(
                entry.Rank,
                entry.Suit,
                entry.Face
            );

            Deck.Add(card);
        }

        GameLog.Info(
            "Built " + Deck.Count + " card(s) from "
            + deckDefinition.name + "."
        );
    }


    // =========================================================
    // ENSURE HUD
    // =========================================================
    //
    // Every UI update in this class now goes through
    // RefreshHud(), which does nothing when hud is null. That
    // is the correct behaviour for a missing optional
    // reference, but it fails SILENTLY and the result looks
    // like a gameplay bug rather than a wiring problem: no
    // button ever shows or hides, so the encounter cannot get
    // past "Keep Hero or Switch Hero", so cards can never be
    // toggled into a combination.
    //
    // So say so, loudly, once, at startup.

    private void EnsureHud()
    {
        if (hud != null)
        {
            return;
        }

        // The migration tool adds the component to this same
        // GameObject, so pick it up even if the inspector
        // reference was not saved with the scene.
        hud = GetComponent<EncounterHud>();

        if (hud == null)
        {
            // No migrated HUD. Build one at runtime rather
            // than shipping a dead UI: the widget references
            // are still sitting in the legacy block below,
            // because the scene YAML still carries them under
            // their original field names.
            hud = gameObject.AddComponent<EncounterHud>();

            GameLog.Info(
                "No EncounterHud found - built one at runtime "
                + "from the legacy widget references. The game "
                + "is playable as-is. To make this permanent "
                + "(and let the legacy block be deleted), run "
                + "Tools > Crybt > Migrate HUD References and "
                + "save the Crybt scene."
            );
        }

        // Fill in anything the Inspector left empty. Widgets
        // already assigned on a migrated HUD are kept.
        hud.AdoptWidgets(
            KeepHeroButton,
            SwitchHeroButton,
            PeekCardButton,
            ConfirmComboButton,
            EndEncounterButton,
            TrashDisplay,
            AttackScore,
            Boons,
            HealthDisplay,
            ModifierDisplay,
            ScoreDisplay,
            MonsterAPDisplay,
            EncounterDisplay,
            CrawlDisplay,
            OfferingPointsDisplay
        );

        // Lets the HUD place the score breakdown under the
        // player's hand without having to know the board
        // layout itself.
        if (HandSlot != null)
        {
            hud.SetHandAnchor(
                HandSlot.transform as RectTransform
            );
        }
    }


    // =========================================================
    // BIND CARDS
    // =========================================================
    //
    // Cards used to locate this manager themselves with
    // FindObjectOfType in Awake - 53 full scene scans, and a
    // hard dependency on the concrete manager type.
    //
    // The manager now pushes itself down once instead.

    private void BindCards()
    {
        BindCardList(Deck);
        BindCardList(Hand);
        BindCardList(Hero);
        BindCardList(Monster);
        BindCardList(Discard);
        BindCardList(Offering);
        BindCardList(Graveyard);
    }


    private void BindCardList(List<Card> cards)
    {
        if (cards == null)
        {
            return;
        }

        for (int i = 0;
            i < cards.Count;
            i++)
        {
            if (cards[i] != null)
            {
                cards[i].Bind(this);
            }
        }
    }


    // =========================================================
    // START CRAWL
    // =========================================================

    private void StartCrawl()
    {
        encounterNumber = 0;

        GameLog.Info(
            "================================"
        );

        GameLog.Info(
            "Starting Crawl "
            + crawlNumber
        );

        GameLog.Info(
            "Active Boons: "
            + boonShop.DescribeActive()
        );

        GameLog.Info(
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

        displayedMonsterAttackPower = null;

        RefreshHud();

        GameLog.Info(
            "Crawl "
            + crawlNumber
            + " complete."
        );

        // Draw final Offering card.
        DrawOfferingCard();

        // Calculate Boon Shop currency.
        offeringPoints =
            CalculateOfferingPoints();

        GameLog.Info(
            "Offering Points available to spend: "
            + offeringPoints
        );

        // Reset purchases for the upcoming crawl.
        boonShop.ClearPurchases();

        // Enter Boon Shop.
        currentStage =
            EncounterStage.ChoosingBoons;

        RefreshHud();
    }


    // =========================================================
    // START ENCOUNTER
    // =========================================================

    private void StartEncounter()
    {
        currentStage =
            EncounterStage.StartingEncounter;

        encounterScore = 0;

        peekedMonster = null;

        displayedMonsterAttackPower = null;

        ClearPlay();

        scoredCombinations.Clear();

        Hero.Clear();
        Monster.Clear();

        encounterNumber++;

        GameLog.Info(
            "Starting Encounter "
            + encounterNumber
            + " of "
            + rules.EncountersPerCrawl
        );

        DrawHand();

        // Player chooses Offering first.
        currentStage =
            EncounterStage.ChoosingOffering;

        RefreshHud();
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
            GameLog.Info(
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

        RefreshHud();

        GameLog.Info(
            "Hero drawn."
        );

        GameLog.Info(
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

        GameLog.Info(
            "Hero kept."
        );

        currentStage =
            EncounterStage.StartingEncounter;

        RefreshHud();

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
            GameLog.Info(
                "The Graveyard is empty."
            );

            return;
        }

        currentStage =
            EncounterStage.ChoosingHeroFromGraveyard;

        GameLog.Info(
            "Choose one card from the Graveyard."
        );

        RefreshHud();

        MoveCards();
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
            GameLog.Error(
                "There is no Hero to replace."
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

        GameLog.Info(
            oldHero.gameObject.name
            + " was discarded."
        );

        GameLog.Info(
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

        RefreshHud();
    }


    private IEnumerator RotateHeroUprightAfterMove(
        Card card)
    {
        // This should match the card's movement duration.
        yield return new WaitForSeconds(
            rotationDuration
        );

        // Now that the card has arrived at the Hero slot,
        // smoothly rotate it back upright.
        RotateTo(
            card,
            Quaternion.identity
        );
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

        if (!boonShop.IsActive(BoonId.Torch))
        {
            GameLog.Info(
                "Torch is not active."
            );

            return;
        }

        if (peekedMonster != null)
        {
            GameLog.Info(
                "Monster has already been peeked."
            );

            return;
        }

        CheckDeck();

        if (Deck.Count == 0)
        {
            GameLog.Info(
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

        displayedMonsterAttackPower =
            GetMonsterAttackPower(peekedMonster);

        GameLog.Info(
            "Torch reveals: "
            + peekedMonster.gameObject.name
        );

        GameLog.Info(
            "Peeked Monster AP: "
            + displayedMonsterAttackPower
        );

        RefreshHud();
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

            GameLog.Info(
                "Peeked Monster officially revealed."
            );
        }
        else
        {
            CheckDeck();

            if (Deck.Count == 0)
            {
                GameLog.Info(
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
            displayedMonsterAttackPower =
                GetMonsterAttackPower(Monster[0]);
        }

        MoveCards();

        RefreshHud();

        GameLog.Info(
            "Monster revealed."
        );

        if (Monster.Count > 0)
        {
            GameLog.Info(
                "Monster AP: "
                + displayedMonsterAttackPower
            );
        }

        GameLog.Info(
            "Build your combinations."
        );
    }


    // =========================================================
    // MONSTER ATTACK POWER
    // =========================================================

    private int GetMonsterAttackPower(Card monsterCard)
    {
        if (monsterCard == null)
        {
            return 0;
        }

        return CrybtCombat.MonsterAttackPower(
            monsterCard.GetValue(),
            Modifier
        );
    }


    // =========================================================
    // HUD
    // =========================================================
    //
    // One snapshot, one call. Replaces the old
    // UpdateEncounterButtons() and UpdateDisplays() pair.

    private void RefreshHud()
    {
        if (hud == null)
        {
            return;
        }

        hud.Render(
            BuildView()
        );
    }


    private EncounterView BuildView()
    {
        bool canPeek =
            boonShop.IsActive(BoonId.Torch)
            && peekedMonster == null;

        int? preview =
            PreviewSelectedCombination(
                out string breakdown
            );

        return new EncounterView(
            currentStage,
            Graveyard.Count > 0,
            canPeek,
            Health,
            Modifier,
            encounterScore,
            encounterNumber,
            rules.EncountersPerCrawl,
            crawlNumber,
            offeringPoints,
            displayedMonsterAttackPower,
            preview,
            breakdown
        );
    }


    // =========================================================
    // PREVIEW SELECTED COMBINATION
    // =========================================================
    //
    // What the cards on the table would score if the player
    // confirmed them right now.
    //
    // Safe to call every refresh: CrybtScoring is pure, takes
    // no Unity types and mutates nothing. It is the same call
    // ConfirmCombination() makes, so the preview can never
    // disagree with the real award.
    //
    // Returns null when there is nothing to preview, which
    // makes the readout fall back to the encounter total.

    private int? PreviewSelectedCombination(
        out string breakdown)
    {
        breakdown = string.Empty;

        if (currentStage !=
            EncounterStage.BuildingCombination)
        {
            return null;
        }

        if (Play == null ||
            Play.Count == 0)
        {
            return null;
        }

        FillScoringBuffer(Play);

        int points =
            CrybtScoring.DescribeCombination(
                scoringBuffer,
                rules,
                breakdownBuffer
            );

        breakdown =
            FormatBreakdown(breakdownBuffer);

        return points;
    }


    // =========================================================
    // FORMAT BREAKDOWN
    // =========================================================
    //
    // All rules on ONE wide line, separated by spaces:
    //
    //     FIFTEEN: 2   PAIR: 2
    //
    // Two deliberate choices here, both driven by the pixel
    // font this game uses:
    //
    //  - UPPERCASE, because the rest of the HUD is (HP, AP,
    //    ENCNTR) and a pixel font may carry no lowercase
    //    glyphs. A missing glyph renders as a blank box, which
    //    is exactly what the unrevealed-monster "?" already
    //    does.
    //
    //  - "NAME: n" rather than "NAME +n", because ":" is known
    //    to render in this font (HP:, AP:) and "+" is not
    //    known to.
    //
    // One line also keeps the label clear of the bottom of the
    // board, which a stacked list would run past.

    private string FormatBreakdown(
        List<ScoreItem> items)
    {
        if (items == null ||
            items.Count == 0)
        {
            return noScoreText;
        }

        breakdownText.Length = 0;

        for (int i = 0;
            i < items.Count;
            i++)
        {
            if (i > 0)
            {
                breakdownText.Append("   ");
            }

            breakdownText
                .Append(items[i].Name.ToUpperInvariant())
                .Append(": ")
                .Append(items[i].Points);
        }

        return breakdownText.ToString();
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

            GameLog.Info(
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

            // Rank and suit are logged because a card whose
            // rank disagrees with its face looks like a
            // scoring bug and is actually a prefab data bug.
            // See 1_Spades Variant.
            CardValue picked =
                selectedCard.ToCardValue();

            GameLog.Info(
                selectedCard.gameObject.name
                + " added to combination."
                + "  (pip " + picked.PipValue
                + ", rank " + picked.Rank
                + ", suit " + picked.Suit
                + ")"
            );
        }

        GameLog.Info(
            "Cards selected: "
            + Play.Count
        );

        // Every toggle changes what the selection would score,
        // so redraw. This is what makes the score readout a
        // live preview rather than a running total.
        RefreshHud();
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
            GameLog.Info(
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
            GameLog.Info(
                "This combination has already been scored."
            );

            ClearPlay();

            // ClearPlay() changes what is on the table, so the
            // HUD has to be told. The original code returned
            // here without refreshing, which left the display
            // showing state that no longer matched the board.
            RefreshHud();

            return;
        }

        FillScoringBuffer(Play);

        int points =
            CrybtScoring.ScoreCombination(
                scoringBuffer,
                rules
            );

        if (points <= 0)
        {
            GameLog.Info(
                "That is not a scoring combination."
            );

            ClearPlay();

            RefreshHud();

            return;
        }

        scoredCombinations.Add(
            combinationID
        );

        encounterScore +=
            points;

        GameLog.Info(
            "Combination scored "
            + points
            + " points."
        );

        GameLog.Info(
            "Encounter Score: "
            + encounterScore
        );

        if (Monster.Count > 0)
        {
            GameLog.Info(
                "Monster AP: "
                + GetMonsterAttackPower(Monster[0])
                + " | Player Score: "
                + encounterScore
            );
        }

        ClearPlay();

        RefreshHud();
    }


    // =========================================================
    // SCORING BUFFER
    // =========================================================
    //
    // Converts Card components into the plain CardValue data
    // that CrybtScoring works with.

    private void FillScoringBuffer(List<Card> cards)
    {
        scoringBuffer.Clear();

        for (int i = 0;
            i < cards.Count;
            i++)
        {
            scoringBuffer.Add(
                cards[i].ToCardValue()
            );
        }
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

        RefreshHud();

        ResolveEncounter();
    }


    // =========================================================
    // RESOLVE ENCOUNTER
    // =========================================================

    private void ResolveEncounter()
    {
        if (Monster.Count == 0)
        {
            GameLog.Error(
                "No Monster exists."
            );

            return;
        }

        int monsterAttackPower =
            GetMonsterAttackPower(Monster[0]);

        int damage =
            CrybtCombat.DamageTaken(
                monsterAttackPower,
                encounterScore
            );

        GameLog.Info(
            "Monster AP: "
            + monsterAttackPower
            + " | Combination Points: "
            + encounterScore
            + " | Damage: "
            + damage
        );


        if (CrybtCombat.IsMonsterDefeated(damage))
        {
            GameLog.Info(
                "Monster AP matched! "
                + "Monster goes to Graveyard."
            );

            DefeatMonster();
        }

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
            GameLog.Error(
                "No Monster to move to Graveyard."
            );

            return;
        }

        Card defeatedMonster =
            Monster[0];

        Monster.RemoveAt(0);

        Graveyard.Add(
            defeatedMonster
        );

        GameLog.Info(
            defeatedMonster.gameObject.name
            + " moved to Graveyard."
        );

        GameLog.Info(
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

        GameLog.Info(
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

        RefreshHud();
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

        displayedMonsterAttackPower = null;

        RefreshHud();

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
            rules.EncountersPerCrawl)
        {
            EndCrawl();
        }

        else
        {
            StartEncounter();
        }
    }


    // =========================================================
    // DRAW HAND
    // =========================================================

    private void DrawHand()
    {
        for (int i = 0;
            i < rules.HandSize;
            i++)
        {
            CheckDeck();

            if (Deck.Count == 0)
            {
                GameLog.Info(
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
            GameLog.Info(
                "No card available for final Offering draw."
            );

            return;
        }

        Offering.Add(
            Deck[0]
        );

        Deck.RemoveAt(0);

        GameLog.Info(
            "Final Offering card drawn."
        );

        MoveCards();
    }


    // =========================================================
    // CALCULATE OFFERING POINTS
    // =========================================================

    private int CalculateOfferingPoints()
    {
        FillScoringBuffer(Offering);

        int points =
            CrybtScoring.ScoreOfferingPile(
                scoringBuffer,
                Graveyard.Count,
                rules
            );

        GameLog.Info(
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
            GameLog.Info(
                "Not enough Offering Points."
            );

            return false;
        }

        offeringPoints -=
            cost;

        GameLog.Info(
            "Spent "
            + cost
            + " Offering Points."
        );

        GameLog.Info(
            "Remaining Offering Points: "
            + offeringPoints
        );

        RefreshHud();

        return true;
    }


    // =========================================================
    // BUY BOON
    // =========================================================
    //
    // Replaces six near-identical BuyBoonN() bodies.
    //
    // The numbered methods below are kept ONLY because the
    // Boon Shop buttons are wired to them by name in the Crybt
    // scene. Do not remove them without re-wiring those
    // Button OnClick entries.

    public void BuyBoon(BoonId id)
    {
        BoonDefinition boon =
            boonShop.Find(id);

        if (boon == null)
        {
            GameLog.Error(
                "No BoonDefinition asset assigned for "
                + id
                + ". Add it to the Boon Shop catalogue "
                + "on CrybtManager."
            );

            return;
        }

        if (boonShop.IsPurchased(id))
        {
            GameLog.Info(
                boon.DisplayName
                + " already purchased for the next Crawl."
            );

            return;
        }


        // =====================================================
        // UNIMPLEMENTED BOONS
        // =====================================================
        //
        // Boons 2-6 previously took the player's points and
        // did nothing at all, because nothing ever read their
        // flags. Refuse the sale until the effect exists.

        if (!boon.Implemented)
        {
            GameLog.Warning(
                boon.DisplayName
                + " has no gameplay effect yet, so it cannot "
                + "be purchased. Tick 'Implemented' on the "
                + "asset once its effect is wired up."
            );

            return;
        }

        if (!SpendOfferingPoints(boon.Cost))
        {
            return;
        }

        boonShop.MarkPurchased(id);

        GameLog.Info(
            boon.DisplayName
            + " purchased for Crawl "
            + (crawlNumber + 1)
            + "."
        );
    }


    // =========================================================
    // BOON BUTTON HOOKS
    // =========================================================
    //
    // Scene Button OnClick targets. Keep the names.

    public void BuyBoon1() => BuyBoon(BoonId.Torch);

    public void BuyBoon2() => BuyBoon(BoonId.WardingSigil);

    public void BuyBoon3() => BuyBoon(BoonId.BoneCharm);

    public void BuyBoon4() => BuyBoon(BoonId.HolyWater);

    public void BuyBoon5() => BuyBoon(BoonId.BlackGrimoire);

    public void BuyBoon6() => BuyBoon(BoonId.BrokenMirror);


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

        CleanUpCrawl();


        // Current boons expire, purchased boons become active,
        // and the purchase list resets - all three steps in one
        // place instead of three copy-paste blocks.
        boonShop.AdvanceToNextCrawl();


        // Unspent points do not carry over unless the rules
        // asset says they do.
        if (!rules.UnspentPointsCarryOver)
        {
            offeringPoints = 0;
        }

        crawlNumber++;

        currentStage =
            EncounterStage.StartingEncounter;

        RefreshHud();

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

        GameLog.Info(
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

        displayedMonsterAttackPower = null;

        RefreshHud();

        GameLog.Info(
            "GAME OVER - Reached Crawl "
            + crawlNumber
        );
    }


    // =========================================================
    // CARD ROTATION
    // =========================================================
    //
    // One rotation per card. Starting a new one cancels the
    // old one instead of letting two coroutines fight over the
    // same transform.

    private void RotateTo(
        Card card,
        Quaternion targetRotation)
    {
        if (card == null)
        {
            return;
        }

        // Already there - nothing to animate.
        if (Quaternion.Angle(
                card.transform.localRotation,
                targetRotation)
            < 0.01f)
        {
            return;
        }

        StopRotation(card);

        rotationRoutines[card] =
            StartCoroutine(
                RotateCard(
                    card,
                    targetRotation
                )
            );
    }


    private void StopRotation(Card card)
    {
        if (!rotationRoutines.TryGetValue(
                card,
                out Coroutine running))
        {
            return;
        }

        if (running != null)
        {
            StopCoroutine(running);
        }

        rotationRoutines.Remove(card);
    }


    private IEnumerator RotateCard(
        Card card,
        Quaternion targetRotation)
    {
        Quaternion startRotation =
            card.transform.localRotation;

        float timer = 0f;

        while (timer < rotationDuration)
        {
            timer += Time.deltaTime;

            float easedT =
                Easing.QuadInOut(
                    timer / rotationDuration
                );

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

        rotationRoutines.Remove(card);
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

        bool choosingFromGraveyard =
            currentStage ==
            EncounterStage.ChoosingHeroFromGraveyard;

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


            if (choosingFromGraveyard)
            {
                // Smoothly rotate sideways.
                RotateTo(
                    Graveyard[i],
                    Quaternion.Euler(
                        0f,
                        0f,
                        90f
                    )
                );

                // Bring the selectable cards forward.
                Graveyard[i].transform.SetAsLastSibling();

                Graveyard[i].ActivateCard();
            }

            else
            {
                // Keep the cards vertically spread,
                // but smoothly return them upright.
                RotateTo(
                    Graveyard[i],
                    Quaternion.identity
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
