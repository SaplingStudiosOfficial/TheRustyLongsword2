using UnityEngine;

// =============================================================
// CRYBT SOUND TABLE
// =============================================================
//
// Which SoundDefinition answers which CrybtSound.
//
// Create via: Assets > Create > Crybt > Sound Table
// Generate a filled one via: Tools > Crybt > Generate Crybt
// Sound Assets
//
// WHY THIS IS AN ASSET AND NOT FIELDS ON A COMPONENT:
//
// Same reason CrybtRules is an asset. A designer changing which
// clip the monster reveal uses should not be editing a
// component on one scene object, and the shipped defaults
// should load from Resources when nothing is wired up.
//
// A row with no sound assigned is allowed and means silence -
// that is how a single sound gets turned off without hunting
// for the call site.
// =============================================================

[CreateAssetMenu(
    menuName = "Crybt/Sound Table",
    fileName = "CrybtSounds")]
public class CrybtSoundTable : ScriptableObject
{
    // =========================================================
    // ENTRY
    // =========================================================

    [System.Serializable]
    public class Entry
    {
        [SerializeField]
        [Tooltip("Which moment in the Crybt this sound answers.")]
        private CrybtSound id = CrybtSound.CardHover;

        [SerializeField]
        [Tooltip("Leave empty to silence this one sound.")]
        private SoundDefinition sound;


        public CrybtSound Id => id;

        public SoundDefinition Sound => sound;


#if UNITY_EDITOR

        public void EditorInitialise(
            CrybtSound soundId,
            SoundDefinition definition)
        {
            id = soundId;
            sound = definition;
        }

#endif
    }


    [SerializeField]
    private Entry[] entries = new Entry[0];


    public Entry[] Entries => entries;


#if UNITY_EDITOR

    public void EditorInitialise(Entry[] rows)
    {
        entries = rows;
    }

#endif
}
