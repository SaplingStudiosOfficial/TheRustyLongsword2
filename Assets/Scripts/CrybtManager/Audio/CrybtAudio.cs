using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// =============================================================
// CRYBT AUDIO
// =============================================================
//
// The Crybt's one audio entry point. CrybtManager says WHAT
// happened; this decides what that sounds like.
//
// HOW TO WIRE IT:
//
// Drop it on the same object as CrybtManager. That is the whole
// job - CrybtManager finds it on its own object when its own
// field is empty, the sound table loads from Resources when its
// field is empty, and a SoundPlayer per sound is built at
// runtime. No per-sound scene wiring, and nothing to re-do
// when a sound is added.
//
// WHY A PLAYER PER SOUND:
//
// A SoundPlayer owns the progression cursor for its sound - the
// position the score tick has climbed to, which clip in a list
// came last. One shared player would mix all of that together.
// They all draw voices from one PoolHost, so the cost is a
// component each, not an AudioSource each.
// =============================================================

public class CrybtAudio : MonoBehaviour
{
    // Shipped table, loaded when nothing is wired up in the
    // Inspector. Same arrangement as CrybtManager and its rules.
    private const string TableResourcePath = "Crybt/CrybtSounds";


    [SerializeField]
    [Tooltip("Optional. Loads from Resources/Crybt/CrybtSounds " +
             "when left empty. Generate one via Tools > Crybt > " +
             "Generate Crybt Sound Assets.")]
    private CrybtSoundTable table;

    [SerializeField]
    [Tooltip("Optional. Where the shared AudioSource pool " +
             "lives. One is added to this object when left empty.")]
    private PoolHost poolHost;

    [SerializeField]
    [Tooltip("Optional. Sends every Crybt sound to this mixer " +
             "group, overriding whatever each sound asset says. " +
             "Left empty, each sound keeps its own routing.")]
    private AudioMixerGroup groupOverride;

    [SerializeField]
    [Tooltip("Spare AudioSources built up front, shared by " +
             "every sound, so the first card deal does not stutter.")]
    private int prewarmVoices = 6;


    private readonly Dictionary<CrybtSound, SoundPlayer> players =
        new Dictionary<CrybtSound, SoundPlayer>();

    // One warning per missing sound, not one per play.
    private readonly HashSet<CrybtSound> warnedMissing =
        new HashSet<CrybtSound>();


    // =========================================================
    // LIFECYCLE
    // =========================================================

    private void Awake()
    {
        EnsureTable();

        EnsurePoolHost();

        BuildPlayers();

        if (prewarmVoices > 0 && poolHost != null)
        {
            poolHost.Pool.Prewarm(
                PoolKeys.AudioVoice,
                prewarmVoices
            );
        }
    }


    // =========================================================
    // PLAY
    // =========================================================
    //
    // The whole public surface, near enough. A sound with no
    // row, or a row with no asset, is silence - not an
    // exception and not a stream of warnings.

    public void Play(CrybtSound id)
    {
        SoundPlayer player = Find(id);

        if (player == null)
        {
            return;
        }

        player.Play();
    }


    public void Stop(CrybtSound id)
    {
        SoundPlayer player = Find(id);

        if (player != null)
        {
            player.Stop();
        }
    }


    // =========================================================
    // RESET PROGRESSION
    // =========================================================
    //
    // Puts a climbing sound back at the bottom of its range.
    // The score tick is reset at the start of every encounter,
    // so each encounter's first combination sounds like the
    // first and not like wherever the last one left off.

    public void ResetProgression(CrybtSound id)
    {
        SoundPlayer player = Find(id);

        if (player != null)
        {
            player.ResetProgression();
        }
    }


    public SoundPlayer Find(CrybtSound id)
    {
        if (players.TryGetValue(id, out SoundPlayer player))
        {
            return player;
        }

        WarnMissing(id);

        return null;
    }


    // =========================================================
    // BUILD
    // =========================================================
    //
    // WHY THE CARRIER STARTS INACTIVE:
    //
    // Unity runs Awake the instant AddComponent is called on an
    // ACTIVE object. A SoundPlayer woken before Configure would
    // build itself a private pool and prewarm it, then be told
    // to use the shared one - leaving orphaned AudioSources
    // behind. Building it inactive means it wakes up already
    // knowing where it belongs.

    private void BuildPlayers()
    {
        if (table == null)
        {
            return;
        }

        CrybtSoundTable.Entry[] entries = table.Entries;

        if (entries == null)
        {
            return;
        }

        for (int i = 0; i < entries.Length; i++)
        {
            CrybtSoundTable.Entry entry = entries[i];

            if (entry == null || entry.Sound == null)
            {
                continue;
            }

            if (players.ContainsKey(entry.Id))
            {
                GameLog.Warning(
                    "CrybtSoundTable has more than one row for "
                    + entry.Id + ". The first one wins."
                );

                continue;
            }

            players.Add(
                entry.Id,
                BuildPlayer(entry)
            );
        }
    }


    private SoundPlayer BuildPlayer(CrybtSoundTable.Entry entry)
    {
        GameObject carrier =
            new GameObject("Sound - " + entry.Id);

        carrier.transform.SetParent(transform, false);

        carrier.SetActive(false);

        SoundPlayer player =
            carrier.AddComponent<SoundPlayer>();

        player.Configure(entry.Sound, poolHost);

        if (groupOverride != null)
        {
            player.SetOutputGroup(groupOverride);
        }

        carrier.SetActive(true);

        return player;
    }


    // =========================================================
    // ENSURE
    // =========================================================
    //
    // Inspector wins, then Resources, then say so. The same
    // shape as CrybtManager.EnsureRules - except that a missing
    // table means a silent game, not a broken one, so it warns
    // once and carries on rather than inventing defaults.

    private void EnsureTable()
    {
        if (table != null)
        {
            return;
        }

        table =
            Resources.Load<CrybtSoundTable>(TableResourcePath);

        if (table != null)
        {
            return;
        }

        GameLog.Warning(
            "CrybtAudio has no sound table assigned and none was "
            + "found at Resources/" + TableResourcePath
            + ". The Crybt will run silently. Generate one via "
            + "Tools > Crybt > Generate Crybt Sound Assets."
        );
    }


    private void EnsurePoolHost()
    {
        if (poolHost != null)
        {
            return;
        }

        if (TryGetComponent(out PoolHost existing))
        {
            poolHost = existing;

            return;
        }

        poolHost = gameObject.AddComponent<PoolHost>();
    }


    private void WarnMissing(CrybtSound id)
    {
        if (!warnedMissing.Add(id))
        {
            return;
        }

        GameLog.Info(
            "No sound is assigned for " + id
            + ". Add a row for it to the Crybt sound table if "
            + "it should be audible."
        );
    }
}
