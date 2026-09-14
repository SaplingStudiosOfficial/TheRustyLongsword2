using UnityEngine;
using UnityEngine.UI;

// =============================================================
// ENCOUNTER HUD
// =============================================================
//
// Owns every Crybt UI widget. CrybtManager owns none.
//
// WHY THIS EXISTS:
//
// CrybtManager previously held nine serialized UI references
// and about 200 lines of UI code, which meant a button layout
// change and a scoring change landed in the same 2,515-line
// file.
//
// Now the rules engine builds an EncounterView and calls
// Render once. It never touches a Button or a Text.
//
//
// NOTE ON THE SINGLE RENDER PATH:
//
// The old UpdateEncounterButtons() hid every control and then
// re-showed some of them per stage. Adding a tenth control
// meant remembering to edit both halves, and forgetting left
// a button lingering into the wrong stage.
//
// Every control below gets exactly ONE assignment, so that
// class of bug is no longer possible.
// =============================================================

public class EncounterHud : MonoBehaviour
{
    // =========================================================
    // ENCOUNTER BUTTONS
    // =========================================================

    [Header("Encounter Buttons")]

    [SerializeField] private Button keepHeroButton;
    [SerializeField] private Button switchHeroButton;
    [SerializeField] private Button peekCardButton;
    [SerializeField] private Button confirmComboButton;
    [SerializeField] private Button endEncounterButton;


    // =========================================================
    // PANELS
    // =========================================================

    [Header("Panels")]

    [SerializeField] private GameObject trashDisplay;
    [SerializeField] private GameObject attackScore;
    [SerializeField] private GameObject boons;


    // =========================================================
    // READOUTS
    // =========================================================

    [Header("Readouts")]

    [SerializeField] private Text healthDisplay;
    [SerializeField] private Text modifierDisplay;
    [SerializeField] private Text scoreDisplay;
    [SerializeField] private Text monsterAPDisplay;
    [SerializeField] private Text encounterDisplay;
    [SerializeField] private Text crawlDisplay;
    [SerializeField] private Text offeringPointsDisplay;


    // =========================================================
    // SCORE BREAKDOWN
    // =========================================================
    //
    // Lists which rules the selected cards satisfy, under the
    // score itself.
    //
    // Leave this EMPTY and one is built at runtime by cloning
    // the score display, so it inherits that label's font,
    // colour and pixel-perfect sizing instead of guessing at
    // them. Assign a Text here to place it yourself and the
    // clone is never made.

    [SerializeField]
    [Tooltip("Optional. Leave empty to auto-create one under " +
             "the score display.")]
    private Text scoreBreakdownDisplay;

    // Layout for the auto-created label.
    //
    // Positioned relative to the HAND SLOT rather than to the
    // board or the score, because "under the player's hand" is
    // what it means - and the hand slot is the only thing that
    // actually knows where that is. CrybtManager supplies it
    // via SetHandAnchor().

    [SerializeField]
    [Tooltip("Distance below the hand slot's centre. Needs to " +
             "clear half a card plus a margin.")]
    private float autoBreakdownGapBelowHand = 110f;

    [SerializeField]
    [Tooltip("Width and height of the label, in the same " +
             "units the card slots use. Wide and short: the " +
             "breakdown is one line across the player's side.")]
    private Vector2 autoBreakdownSize =
        new Vector2(900f, 120f);

    [SerializeField]
    [Tooltip("Font size relative to the score display. The " +
             "score is one huge number; this is a caption.")]
    [Range(0.1f, 1f)]
    private float autoBreakdownFontScale = 0.25f;

    private RectTransform handAnchor;

    private bool breakdownResolved;


    // =========================================================
    // SET HAND ANCHOR
    // =========================================================
    //
    // Optional. Without it the breakdown falls back to sitting
    // under the score display instead.

    public void SetHandAnchor(RectTransform handSlot)
    {
        handAnchor = handSlot;
    }


    // =========================================================
    // UNREVEALED MONSTER
    // =========================================================

    [SerializeField]
    [Tooltip("Shown in place of the Monster's Attack Power " +
             "before it is revealed.")]
    private string unknownAttackPowerText = "?";


    // =========================================================
    // ADOPT WIDGETS
    // =========================================================
    //
    // Runtime fallback for a scene that has not had
    // Tools > Crybt > Migrate HUD References run on it yet.
    //
    // CrybtManager still carries the original widget
    // references in its hidden legacy block - Unity keeps
    // deserialising them because the field names in the scene
    // YAML have not changed. This lets the manager hand them
    // over on the first frame, so the HUD works with no Editor
    // step at all.
    //
    // Anything already assigned in the Inspector WINS: a real
    // migrated HUD is never overwritten by this.

    public void AdoptWidgets(
        Button keepHero,
        Button switchHero,
        Button peekCard,
        Button confirmCombo,
        Button endEncounter,
        GameObject trash,
        GameObject attack,
        GameObject boonsRoot,
        Text health,
        Text modifier,
        Text score,
        Text monsterAp,
        Text encounter,
        Text crawl,
        Text offeringPoints)
    {
        if (keepHeroButton == null)     keepHeroButton = keepHero;
        if (switchHeroButton == null)   switchHeroButton = switchHero;
        if (peekCardButton == null)     peekCardButton = peekCard;
        if (confirmComboButton == null) confirmComboButton = confirmCombo;
        if (endEncounterButton == null) endEncounterButton = endEncounter;

        if (trashDisplay == null)       trashDisplay = trash;
        if (attackScore == null)        attackScore = attack;
        if (boons == null)              boons = boonsRoot;

        if (healthDisplay == null)          healthDisplay = health;
        if (modifierDisplay == null)        modifierDisplay = modifier;
        if (scoreDisplay == null)           scoreDisplay = score;
        if (monsterAPDisplay == null)       monsterAPDisplay = monsterAp;
        if (encounterDisplay == null)       encounterDisplay = encounter;
        if (crawlDisplay == null)           crawlDisplay = crawl;
        if (offeringPointsDisplay == null)  offeringPointsDisplay = offeringPoints;
    }


    // =========================================================
    // RENDER
    // =========================================================
    //
    // The whole HUD, from one snapshot, in one pass.

    public void Render(in EncounterView view)
    {
        bool choosingOffering =
            view.Stage == EncounterStage.ChoosingOffering;

        bool choosingHero =
            view.Stage == EncounterStage.ChoosingHero;

        bool buildingCombination =
            view.Stage == EncounterStage.BuildingCombination;

        bool choosingBoons =
            view.Stage == EncounterStage.ChoosingBoons;


        // =====================================================
        // CONTROLS
        // =====================================================

        trashDisplay.SetVisible(choosingOffering);

        keepHeroButton.SetVisible(choosingHero);

        switchHeroButton.SetVisible(
            choosingHero
            && view.CanSwitchHero
        );

        peekCardButton.SetVisible(
            choosingHero
            && view.CanPeek
        );

        confirmComboButton.SetVisible(buildingCombination);
        endEncounterButton.SetVisible(buildingCombination);
        attackScore.SetVisible(buildingCombination);

        boons.SetVisible(choosingBoons);


        // =====================================================
        // READOUTS
        // =====================================================

        SetText(
            healthDisplay,
            view.Health
        );

        SetText(
            modifierDisplay,
            view.Modifier
        );

        // While cards are selected this reads as a live
        // preview of what they would score; with nothing
        // selected it falls back to the running encounter
        // total. See EncounterView.CombinationPreview.
        SetText(
            scoreDisplay,
            view.CombinationPreview ?? view.EncounterScore
        );

        RenderBreakdown(view);

        SetText(
            crawlDisplay,
            view.CrawlNumber
        );

        if (encounterDisplay != null)
        {
            encounterDisplay.text =
                view.EncounterNumber
                + " / "
                + view.EncountersPerCrawl;
        }


        // =====================================================
        // MONSTER ATTACK POWER
        // =====================================================

        if (monsterAPDisplay != null)
        {
            monsterAPDisplay.text =
                view.MonsterAttackPower.HasValue
                    ? view.MonsterAttackPower.Value.ToString()
                    : unknownAttackPowerText;
        }


        // =====================================================
        // OFFERING POINTS
        // =====================================================
        //
        // Only meaningful while the Boon Shop is open.

        offeringPointsDisplay.SetVisible(choosingBoons);

        if (choosingBoons)
        {
            SetText(
                offeringPointsDisplay,
                view.OfferingPoints
            );
        }
    }


    // =========================================================
    // SET TEXT
    // =========================================================

    // =========================================================
    // RENDER BREAKDOWN
    // =========================================================
    //
    // Only meaningful while a combination is being built, so
    // it is hidden the rest of the time rather than left
    // showing a stale list.

    private void RenderBreakdown(in EncounterView view)
    {
        Text target = ResolveBreakdownDisplay();

        if (target == null)
        {
            return;
        }

        bool show =
            view.CombinationPreview.HasValue
            && !string.IsNullOrEmpty(
                view.CombinationBreakdown
            );

        target.SetVisible(show);

        if (show)
        {
            target.text = view.CombinationBreakdown;
        }
    }


    // =========================================================
    // RESOLVE BREAKDOWN DISPLAY
    // =========================================================
    //
    // Cloning the score label is deliberate: this project uses
    // a pixel font with a shadow component, and a Text created
    // from scratch would not match it. Copying the real one
    // inherits every setting, including anything added to it
    // later.

    private Text ResolveBreakdownDisplay()
    {
        if (scoreBreakdownDisplay != null)
        {
            return scoreBreakdownDisplay;
        }

        // Only ever attempt this once, whether or not it works.
        if (breakdownResolved)
        {
            return null;
        }

        breakdownResolved = true;

        if (scoreDisplay == null)
        {
            return null;
        }

        // Parent to the hand slot's own parent when we have
        // one, so the label lives in the same coordinate space
        // as the cards it sits under. Falling back to the
        // score's parent keeps it working if the slot was
        // never supplied.
        Transform parent =
            handAnchor != null
                ? handAnchor.parent
                : scoreDisplay.transform.parent;

        GameObject clone =
            Instantiate(
                scoreDisplay.gameObject,
                parent
            );

        clone.name = "ScoreBreakdownDisplay (auto)";

        scoreBreakdownDisplay =
            clone.GetComponent<Text>();

        if (scoreBreakdownDisplay == null)
        {
            Destroy(clone);

            return null;
        }

        LayOutBreakdown(
            clone.transform as RectTransform
        );

        StyleBreakdown(
            scoreBreakdownDisplay
        );

        return scoreBreakdownDisplay;
    }


    // =========================================================
    // LAY OUT BREAKDOWN
    // =========================================================
    //
    // Stretch across the player's side of the board, sitting
    // on the bottom edge under the hand.
    //
    // Anchors do the work rather than a fixed offset, so the
    // label keeps its relationship to the hand whatever the
    // board is scaled to. The pivot is on the bottom edge so
    // extra lines grow UPWARD - a four-rule combination can
    // never push itself off the bottom of the screen.

    private void LayOutBreakdown(RectTransform placed)
    {
        if (placed == null)
        {
            return;
        }

        RectTransform reference =
            handAnchor != null
                ? handAnchor
                : scoreDisplay.transform as RectTransform;

        if (reference == null)
        {
            return;
        }

        // Match the reference's anchoring so anchoredPosition
        // means the same thing for both. These slots use plain
        // centred anchors, not stretched ones.
        placed.anchorMin = reference.anchorMin;
        placed.anchorMax = reference.anchorMax;

        // Top-left pivot: the label hangs DOWN from its anchor
        // point and grows rightward, so its top edge stays put
        // no matter how many rules fire.
        placed.pivot = new Vector2(0f, 1f);

        placed.sizeDelta = autoBreakdownSize;

        placed.anchoredPosition =
            new Vector2(
                reference.anchoredPosition.x,
                reference.anchoredPosition.y
                - autoBreakdownGapBelowHand
            );

        placed.localScale = Vector3.one;

        placed.localRotation = Quaternion.identity;
    }


    // =========================================================
    // STYLE BREAKDOWN
    // =========================================================
    //
    // The clone inherits the score display's font and colour,
    // which is the point - but the score is a single large
    // number and this is a list, so it needs to be smaller and
    // it must never clip.
    //
    // verticalOverflow = Overflow is the important one: the
    // default Truncate is what cuts a multi-rule breakdown off
    // partway through.

    private void StyleBreakdown(Text target)
    {
        if (target == null)
        {
            return;
        }

        target.fontSize =
            Mathf.Max(
                1,
                Mathf.RoundToInt(
                    target.fontSize
                    * autoBreakdownFontScale
                )
            );

        target.alignment = TextAnchor.UpperLeft;

        target.horizontalOverflow = HorizontalWrapMode.Wrap;

        target.verticalOverflow = VerticalWrapMode.Overflow;

        // A best-fit setting inherited from the score label
        // would fight the font size just set.
        target.resizeTextForBestFit = false;


        // The cloned drop shadow was sized for a big number.
        // Shrink it in step with the font or it reads as a
        // smear at this size.
        //
        // Safe to set after Awake: SimpleTextShadow only
        // writes effectDistance from Awake and OnValidate,
        // both of which have already run on the clone.
        Shadow shadow = target.GetComponent<Shadow>();

        if (shadow != null)
        {
            shadow.effectDistance =
                shadow.effectDistance
                * autoBreakdownFontScale;
        }
    }


    private static void SetText(
        Text target,
        int value)
    {
        if (target == null)
        {
            return;
        }

        target.text = value.ToString();
    }
}
