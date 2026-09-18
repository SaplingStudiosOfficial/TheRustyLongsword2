using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

// =============================================================
// CRYBT AUDIO GENERATOR
// =============================================================
//
// Builds one SoundDefinition asset per CrybtSound, pointed at
// the matching clip in Assets/AudioLines/CryptAudio, routed to
// the right mixer group, and collects them into the sound table
// CrybtAudio loads at runtime.
//
// USAGE:
//
//   Tools > Crybt > Generate Crybt Sound Assets
//
// WHY THIS IS A TOOL AND NOT HAND-WIRING:
//
// Twenty-three sounds is twenty-three assets, each needing a
// clip, a group, a level and a pitch range. Done by hand that
// is an afternoon and a dozen inconsistencies. The table below
// is also the only place the "what should this sound like"
// decisions are written down.
//
// Safe to re-run. An asset that already exists is left exactly
// as it is - so a level tweaked by hand survives. Only the
// table is rewritten, and only to add rows it was missing.
//
// That is also what makes it useless for tuning, so there is a
// second menu item:
//
//   Tools > Crybt > Rebuild Crybt Sound Assets (Overwrite)
//
// which re-applies the table below to assets that already
// exist, discarding hand edits. It asks before it does.
//
// SCOPE:
//
// Crybt only. Nothing here touches the overworld, and the
// mixer's Master group and MasterVolume parameter are read but
// never modified.
// =============================================================

public static class CrybtAudioGenerator
{
    private const string SoundFolder = "Assets/Resources/Crybt/Sounds";

    private const string TablePath =
        "Assets/Resources/Crybt/CrybtSounds.asset";

    private const string MixerPath = "Assets/WorldSound.mixer";

    private const string EffectsFolder =
        "Assets/AudioLines/CryptAudio/In-GameEffects/";

    private const string UiFolder =
        "Assets/AudioLines/CryptAudio/UIEffectsAudio/";


    // =========================================================
    // THE SCORE TALLY
    // =========================================================
    //
    // The one sound that climbs rather than jitters.
    //
    // It starts a fourth BELOW the recorded tick so the run has
    // somewhere to go. Two octaves above 0.75 is exactly 3.0,
    // which is Unity's pitch ceiling - raise this base and the
    // top of the run gets quietly squashed against it.
    //
    // NOTE ON THE KEY:
    //
    // This is the major SCALE, not the key of C. Pitching a
    // recorded sample puts the root wherever the clip itself
    // sits; only a tick recorded at C would make this literally
    // C major.

    private const float ScoreTickBasePitch = 0.75f;

    // Seven notes to the octave in a major scale, so fourteen
    // steps is two octaves - and step fourteen is reached,
    // because Clamp includes its endpoint where Loop does not.
    private const float ScoreTickTopStep = 14f;


    // =========================================================
    // WHAT EACH SOUND IS
    // =========================================================
    //
    // clip ...... file name inside its folder
    // ui ........ true routes to the UI group, false to SFX
    // volume .... base level before any variation
    // low/high .. random pitch range; 1/1 means no variation
    //
    // The ranges are deliberately narrow. Wide random pitch
    // stops sounding like variation and starts sounding like a
    // broken tape - a few percent is what reads as "another one
    // of the same thing".

    private struct SoundSpec
    {
        public CrybtSound Id;
        public string Clip;
        public bool Ui;
        public float Volume;
        public float PitchLow;
        public float PitchHigh;
        public string Description;
    }


    private static readonly SoundSpec[] Specs =
    {
        Spec(CrybtSound.CardHover, UiFolder + "ButtonHover.wav", true, 0.35f, 0.95f, 1.05f,
            "Mouse moves onto a clickable card. Quiet on purpose - it fires constantly."),

        Spec(CrybtSound.CardSelect, EffectsFolder + "CardSelectCardDeselect.wav", false, 0.70f, 1.00f, 1.08f,
            "Card added to the combination. Pitched slightly up against the deselect."),

        Spec(CrybtSound.CardDeselect, EffectsFolder + "CardSelectCardDeselect.wav", false, 0.60f, 0.90f, 0.98f,
            "Card taken back out. Same clip as select, pitched down so the pair reads as on and off."),

        Spec(CrybtSound.CardDeal, EffectsFolder + "CardDrawDealIntoHand.mp3", false, 0.80f, 0.95f, 1.05f,
            "Cards dealt off the deck."),

        Spec(CrybtSound.CardPlay, EffectsFolder + "CardPlay.wav", false, 0.85f, 0.97f, 1.03f,
            "The Offering is committed."),

        Spec(CrybtSound.CardDiscard, EffectsFolder + "CardDiscard.wav", false, 0.80f, 0.95f, 1.05f,
            "Card sent to the Discard - the old Hero when one is switched out."),

        Spec(CrybtSound.DeckShuffle, EffectsFolder + "DeckShuffle.wav", false, 0.80f, 0.98f, 1.02f,
            "Deck shuffled, including the reshuffle when it runs out."),

        Spec(CrybtSound.EncounterStart, EffectsFolder + "RoomTransition.wav", false, 0.80f, 1f, 1f,
            "A new encounter begins."),

        Spec(CrybtSound.EncounterEnd, EffectsFolder + "FootStepsMovementbetweenrooms.wav", false, 0.70f, 1f, 1f,
            "Moving on from a finished encounter."),

        Spec(CrybtSound.MonsterPeek, UiFolder + "TooltipAppearORTutorialPromtAppear.wav", true, 0.70f, 1f, 1f,
            "Torch reveals the Monster early."),

        Spec(CrybtSound.MonsterReveal, EffectsFolder + "MonsterSpawn.wav", false, 1.00f, 0.95f, 1.05f,
            "The Monster is turned face up."),

        Spec(CrybtSound.MonsterDefeated, EffectsFolder + "MonsterDeath.wav", false, 1.00f, 0.95f, 1.05f,
            "Monster matched and sent to the Graveyard."),

        Spec(CrybtSound.PlayerAttack, EffectsFolder + "PlayerAttack.wav", false, 0.90f, 0.93f, 1.07f,
            "A combination scored. The hit, not the tally."),

        Spec(CrybtSound.PlayerHurt, EffectsFolder + "PlayerTakesDamage.wav", false, 1.00f, 0.95f, 1.05f,
            "The Monster got through."),

        Spec(CrybtSound.PlayerDeath, EffectsFolder + "PlayerDeath.wav", false, 1.00f, 1f, 1f,
            "Health reached zero."),

        // Pitch is set separately below - this is the one that
        // climbs rather than jitters.
        Spec(CrybtSound.ScoreTick, UiFolder + "ScoreCounterTick.wav", true, 0.70f, 1f, 1f,
            "The tally. Starts a fourth below the recorded tick and climbs one note of "
            + "the major scale per scoring combination, holding on the note two octaves "
            + "up rather than looping. Resets to the bottom on each new hand."),

        Spec(CrybtSound.CrawlComplete, EffectsFolder + "VictoryRunComplete.wav", false, 1.00f, 1f, 1f,
            "The crawl is survived."),

        Spec(CrybtSound.OfferingPointsEarned, EffectsFolder + "GoldOrLootPickup.wav", false, 0.80f, 0.97f, 1.03f,
            "Offering Points awarded at the end of a crawl."),

        Spec(CrybtSound.BoonPurchased, UiFolder + "AchievementOrUnlockChime.wav", true, 0.90f, 1f, 1f,
            "A boon is bought."),

        Spec(CrybtSound.BoonPhaseEnd, UiFolder + "MenuOpenMenuCloseORTabSwitchPage.wav", true, 0.80f, 1f, 1f,
            "The shop closes and the next crawl begins."),

        Spec(CrybtSound.ButtonPress, UiFolder + "ButtonClick.mp3", true, 0.80f, 0.96f, 1.04f,
            "Keep Hero, Switch Hero, and the other confirmations."),

        Spec(CrybtSound.ActionRefused, UiFolder + "ErrorInvalidActionBuzz.wav", true, 0.80f, 1f, 1f,
            "Anything the game refuses: an empty confirm, a combination that does not "
            + "score or has already scored, a boon that cannot be afforded or has no effect yet."),

        // Looping bed - handled separately below.
        Spec(CrybtSound.Ambience, EffectsFolder + "AmbientCryptLoop.ogg", false, 0.35f, 1f, 1f,
            "The looping bed under the whole scene. Routed to Music, fades in and out."),
    };


    private static SoundSpec Spec(
        CrybtSound id,
        string clip,
        bool ui,
        float volume,
        float pitchLow,
        float pitchHigh,
        string description)
    {
        return new SoundSpec
        {
            Id = id,
            Clip = clip,
            Ui = ui,
            Volume = volume,
            PitchLow = pitchLow,
            PitchHigh = pitchHigh,
            Description = description
        };
    }


    // =========================================================
    // GENERATE
    // =========================================================

    [MenuItem("Tools/Crybt/Generate Crybt Sound Assets")]
    private static void Generate()
    {
        Run(false);
    }


    // =========================================================
    // REBUILD
    // =========================================================
    //
    // The same tool, but it re-applies the table above to assets
    // that already exist.
    //
    // WHY THIS IS SEPARATE:
    //
    // Generate is safe precisely because it never touches an
    // existing asset - a level nudged by hand survives it. That
    // safety is also what makes it useless for tuning: change a
    // number in the table above and nothing happens.
    //
    // This one discards hand edits, so it asks first.

    [MenuItem("Tools/Crybt/Rebuild Crybt Sound Assets (Overwrite)")]
    private static void Rebuild()
    {
        bool confirmed =
            EditorUtility.DisplayDialog(
                "Rebuild Crybt Sound Assets",
                "This re-applies the generator's settings to every "
                + "sound asset that already exists, discarding any "
                + "levels, ranges or clips changed by hand.",
                "Overwrite",
                "Cancel"
            );

        if (confirmed)
        {
            Run(true);
        }
    }


    private static void Run(bool overwrite)
    {
        EnsureFolder("Assets/Resources");
        EnsureFolder("Assets/Resources/Crybt");
        EnsureFolder(SoundFolder);

        AudioMixerGroup sfxGroup = FindGroup("SFX");
        AudioMixerGroup uiGroup = FindGroup("UI");
        AudioMixerGroup musicGroup = FindGroup("Music");

        int created = 0;
        int rebuilt = 0;
        int missingClips = 0;

        List<CrybtSoundTable.Entry> rows =
            new List<CrybtSoundTable.Entry>();

        for (int i = 0; i < Specs.Length; i++)
        {
            SoundSpec spec = Specs[i];

            string path =
                SoundFolder + "/Sound_" + spec.Id + ".asset";

            SoundDefinition sound =
                AssetDatabase.LoadAssetAtPath<SoundDefinition>(path);

            bool isNew = sound == null;

            if (isNew || overwrite)
            {
                AudioClip clip =
                    AssetDatabase.LoadAssetAtPath<AudioClip>(spec.Clip);

                if (clip == null)
                {
                    Debug.LogWarning(
                        "[Crybt Audio] No clip at "
                        + spec.Clip
                        + " for "
                        + spec.Id
                        + ". The asset is still written, with no clip."
                    );

                    missingClips++;
                }

                if (isNew)
                {
                    sound =
                        ScriptableObject.CreateInstance<SoundDefinition>();
                }

                Configure(
                    sound,
                    spec,
                    clip,
                    spec.Ui ? uiGroup : sfxGroup,
                    musicGroup
                );

                if (isNew)
                {
                    AssetDatabase.CreateAsset(sound, path);

                    created++;
                }
                else
                {
                    EditorUtility.SetDirty(sound);

                    rebuilt++;
                }
            }

            CrybtSoundTable.Entry row =
                new CrybtSoundTable.Entry();

            row.EditorInitialise(spec.Id, sound);

            rows.Add(row);
        }

        WriteTable(rows);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "[Crybt Audio] "
            + created
            + " new and "
            + rebuilt
            + " rebuilt sound asset(s) under "
            + SoundFolder
            + ", table written to "
            + TablePath
            + (overwrite
                ? "."
                : ". Existing assets were left alone.")
        );

        EditorUtility.DisplayDialog(
            "Crybt Sound Assets",
            created
            + " created, "
            + rebuilt
            + " rebuilt.\n"
            + (missingClips > 0
                ? missingClips + " clip(s) could not be found - see the Console.\n\n"
                : "\n")
            + "NEXT: add a CrybtAudio component to the object that "
            + "carries CrybtManager in the Crybt scene, then save "
            + "the scene. Everything else wires itself.",
            "OK"
        );
    }


    // =========================================================
    // BUILD ONE SOUND
    // =========================================================

    private static void Configure(
        SoundDefinition sound,
        SoundSpec spec,
        AudioClip clip,
        AudioMixerGroup group,
        AudioMixerGroup musicGroup)
    {
        // Rebuilding an existing asset has to start from a known
        // state, or a pitch range the spec no longer asks for
        // survives from the previous run.
        sound.Settings.EditorResetVariation();

        sound.EditorSetDescription(spec.Description);

        bool ambience = spec.Id == CrybtSound.Ambience;

        sound.Settings.EditorInitialise(
            clip,
            ambience
                ? SoundPlaybackMode.Loop
                : SoundPlaybackMode.OneShot,
            spec.Volume,
            ambience ? musicGroup : group
        );

        if (ambience)
        {
            // A bed that snaps on with the scene is the thing
            // everyone notices. Two seconds in, a second and a
            // half out.
            sound.Settings.EditorSetFades(2f, 1.5f);

            return;
        }

        // UI sounds keep playing while the game is paused,
        // because a menu that clicks silently feels broken.
        sound.Settings.EditorSetIgnoreListenerPause(spec.Ui);

        if (spec.Id == CrybtSound.ScoreTick)
        {
            sound.Settings.EditorSetBasePitch(ScoreTickBasePitch);

            sound.Settings.EditorSetMusicalStep(
                MusicalScale.Major,
                0f,
                ScoreTickTopStep,
                1f,

                // Clamp, not Loop. Running out of range should
                // sound like the top of the run, not like
                // starting the run again.
                SoundStepWrap.Clamp,

                // No idle reset. The only thing that puts the
                // tally back to the bottom is a new hand, which
                // CrybtManager does at StartEncounter. A player
                // who stops to think mid-hand should not lose
                // their place.
                0f
            );

            return;
        }

        if (spec.PitchLow < spec.PitchHigh)
        {
            sound.Settings.EditorSetRandomPitch(
                spec.PitchLow,
                spec.PitchHigh
            );
        }
    }


    // =========================================================
    // TABLE
    // =========================================================
    //
    // Rewritten every run, but built from whatever assets are
    // on disk - so re-running after adding a CrybtSound member
    // adds its row without disturbing the others.

    private static void WriteTable(List<CrybtSoundTable.Entry> rows)
    {
        CrybtSoundTable table =
            AssetDatabase.LoadAssetAtPath<CrybtSoundTable>(TablePath);

        if (table == null)
        {
            table = ScriptableObject.CreateInstance<CrybtSoundTable>();

            table.EditorInitialise(rows.ToArray());

            AssetDatabase.CreateAsset(table, TablePath);

            return;
        }

        table.EditorInitialise(rows.ToArray());

        EditorUtility.SetDirty(table);
    }


    // =========================================================
    // MIXER
    // =========================================================
    //
    // Groups are sub-assets of the mixer, so they are found by
    // walking its contents rather than by path. A missing group
    // is not fatal - the sound falls back to inheriting
    // whatever routing its AudioSource already has.

    private static AudioMixerGroup FindGroup(string groupName)
    {
        Object[] contents =
            AssetDatabase.LoadAllAssetsAtPath(MixerPath);

        if (contents == null || contents.Length == 0)
        {
            Debug.LogWarning(
                "[Crybt Audio] No mixer at " + MixerPath + "."
            );

            return null;
        }

        for (int i = 0; i < contents.Length; i++)
        {
            AudioMixerGroup group =
                contents[i] as AudioMixerGroup;

            if (group != null && group.name == groupName)
            {
                return group;
            }
        }

        Debug.LogWarning(
            "[Crybt Audio] WorldSound.mixer has no group called '"
            + groupName
            + "'. Sounds meant for it will inherit their routing "
            + "instead. Add the group in Window > Audio > Audio Mixer."
        );

        return null;
    }


    // =========================================================
    // FOLDERS
    // =========================================================

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        int split = path.LastIndexOf('/');

        AssetDatabase.CreateFolder(
            path.Substring(0, split),
            path.Substring(split + 1)
        );
    }
}
