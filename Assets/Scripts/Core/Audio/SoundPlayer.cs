using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// =============================================================
// SOUND PLAYER
// =============================================================
//
// Plays a sound against any AudioSource, on demand, with
// optional pitch and volume variation.
//
// HOW TO USE IT:
//
// Drop it on an object, fill in a clip, and call Play() - from
// code, or straight from a Button's onClick with no code at
// all. Right-click the component in Play Mode and choose
// "Play Test" to hear it without writing anything.
//
// WHAT IT SOLVES:
//
// Pitch belongs to the AudioSource, not to the sound. Set the
// pitch and fire a second one-shot and the FIRST one's tail
// shifts with it, which is audible on anything that repeats
// quickly. So each overlapping one-shot borrows its own spare
// source from an ObjectPool, configured from the source it was
// asked to play against - mixer group, spatial blend and
// distances carried across, so a 3D sound stays where it was.
//
// WHAT IT DELIBERATELY DOES NOT DO:
//
// It does not find anything. No FindObjectOfType, no singleton.
// Its AudioSource and its PoolHost are assigned or handed in,
// the same way Card is given its handler rather than going
// looking for one.
//
// It does not own volume settings for the game. MasterAudioManager
// still owns the WorldSound mixer's MasterVolume parameter;
// nothing here writes to it.
// =============================================================

public class SoundPlayer : MonoBehaviour
{
    // =========================================================
    // ACTIVE VOICE
    // =========================================================
    //
    // A borrowed source and the coroutines that are watching
    // it. Both handles are kept so that recycling a voice early
    // can cancel them - a release timer that outlives its voice
    // is how a pool starts handing the same source to two
    // callers.

    private class ActiveVoice
    {
        public AudioSource Source;
        public Coroutine Release;
        public Coroutine Fade;

        // False for the no-pool path, where Source belongs to
        // the scene and must be handed back untouched.
        public bool Pooled;
    }


    // =========================================================
    // SOUND
    // =========================================================
    //
    // A definition wins when one is assigned, exactly as
    // CardDefinition wins over Card's loose integers. With no
    // definition the inline settings below are used.

    [Header("Sound")]

    [SerializeField]
    [Tooltip("Optional. When assigned, this asset supplies " +
             "every setting and the inline settings below are " +
             "ignored. Create via Assets > Create > Audio > Sound.")]
    private SoundDefinition definition;

    [SerializeField]
    private SoundSettings settings = new SoundSettings();


    // =========================================================
    // OUTPUT
    // =========================================================

    [Header("Output")]

    [SerializeField]
    [Tooltip("The source this plays against. Left empty, the " +
             "AudioSource on this object is used, and one is " +
             "added if there is none.")]
    private AudioSource targetSource;

    [SerializeField]
    [Tooltip("On: each overlapping one-shot borrows its own " +
             "spare source and keeps its own pitch. Off: " +
             "everything plays on the source above, and a new " +
             "shot re-pitches any still ringing out.")]
    private bool useVoicePool = true;

    [SerializeField]
    [Tooltip("Most sounds this component may have going at " +
             "once. Past this the oldest is stopped and reused, " +
             "which keeps a runaway caller off the engine's " +
             "32-voice budget.")]
    private int maxVoices = 8;

    [SerializeField]
    [Tooltip("Optional. Assign a shared PoolHost so several " +
             "SoundPlayers draw voices from one pool. Left " +
             "empty, this component keeps a small pool of its own.")]
    private PoolHost poolHost;

    [SerializeField]
    [Tooltip("Spare sources to build up front, so the first " +
             "sound does not stutter.")]
    private int prewarmVoices = 2;


    // =========================================================
    // STARTUP
    // =========================================================

    [Header("Startup")]

    [SerializeField]
    [Tooltip("Play as soon as the object starts. Useful for " +
             "ambient loops.")]
    private bool playOnStart = false;


    // =========================================================
    // RUNTIME STATE
    // =========================================================
    //
    // WHY THE PROGRESSION LIVES HERE:
    //
    // A SoundDefinition is a ScriptableObject and can be shared
    // by twenty components. A cursor stored on the asset would
    // be shared with them, so two objects climbing a scale at
    // once would fight over one position. The shaper is pure
    // and the cursor is per-component. See SoundShaper.

    private ObjectPool localPool;

    private readonly List<ActiveVoice> activeVoices =
        new List<ActiveVoice>();

    private ActiveVoice loopVoice;

    private float pitchTravel;
    private float volumeTravel;
    private bool progressionStarted;
    private float lastPlayUnscaledTime;

    private int orderIndex;
    private int lastClipIndex = -1;

    private AudioMixerGroup runtimeGroup;
    private bool hasRuntimeGroup;

    private float volumeScale = 1f;

    private bool warnedNoClips;
    private bool warnedNoVoice;

    private bool didAwake;


    // =========================================================
    // PUBLIC READ
    // =========================================================

    public SoundSettings ActiveSettings =>
        definition != null && definition.Settings != null
            ? definition.Settings
            : settings;

    public int ActiveVoiceCount => activeVoices.Count;

    public bool IsLoopPlaying =>
        loopVoice != null
        && loopVoice.Source != null
        && loopVoice.Source.isPlaying;


    // =========================================================
    // LIFECYCLE
    // =========================================================

    private void Awake()
    {
        EnsureTargetSource();

        if (poolHost == null)
        {
            localPool = new ObjectPool();
        }

        EnsurePoolRegistered();

        if (prewarmVoices > 0 && useVoicePool)
        {
            Pool?.Prewarm(PoolKeys.AudioVoice, prewarmVoices);
        }

        didAwake = true;
    }


    // =========================================================
    // CONFIGURE
    // =========================================================
    //
    // For players built in code rather than authored in a
    // scene - see CrybtAudio, which makes one per sound.
    //
    // HOW TO CALL IT SAFELY:
    //
    // Create the carrier GameObject INACTIVE, AddComponent,
    // Configure, then activate. Unity does not run Awake on an
    // inactive object, so the player wakes up already knowing
    // which pool it belongs to and never builds a private one
    // it is about to throw away.
    //
    // Calling it later still works - the pool is re-resolved
    // below - but any voices already borrowed stay with the
    // old pool until they are returned.
    //
    // prewarm defaults to zero here on purpose: twenty players
    // sharing one host should prewarm the host once, not
    // twenty times over.

    public void Configure(
        SoundDefinition sound,
        PoolHost host,
        int prewarm = 0)
    {
        definition = sound;
        poolHost = host;
        prewarmVoices = prewarm;

        if (!didAwake)
        {
            return;
        }

        if (poolHost == null && localPool == null)
        {
            localPool = new ObjectPool();
        }

        EnsurePoolRegistered();
    }


    private void Start()
    {
        if (playOnStart)
        {
            Play();
        }
    }


    // Coroutines die with the component, so a disabled player
    // must hand its voices back itself or the pool leaks them.
    private void OnDisable()
    {
        StopImmediate();
    }


    // =========================================================
    // PLAY
    // =========================================================
    //
    // Play() uses the component's own source. The overload
    // plays against whatever source is handed in - its mixer
    // group, spatial blend, distances and position are used,
    // so "play this sound over there, on that bus" needs no
    // setup at the call site.

    public void Play()
    {
        PlayInternal(null);
    }


    public void Play(AudioSource source)
    {
        PlayInternal(source);
    }


    public void PlayClip(AudioClip clip)
    {
        PlayInternal(null, clip);
    }


    private void PlayInternal(
        AudioSource explicitSource,
        AudioClip explicitClip = null)
    {
        SoundSettings active = ActiveSettings;

        AudioClip clip =
            explicitClip != null
                ? explicitClip
                : PickClip(active);

        if (clip == null)
        {
            WarnNoClips();

            return;
        }

        PrepareProgression(active);

        float pitch = active.ResolvePitch(pitchTravel);

        float volume =
            Mathf.Clamp01(
                active.ResolveVolume(volumeTravel) * volumeScale
            );

        AudioSource template = ResolveTemplate(explicitSource);

        if (active.Loops)
        {
            StartLoop(active, template, clip, pitch, volume);
        }
        else
        {
            StartOneShot(active, template, clip, pitch, volume);
        }

        // Advanced AFTER the value is used, so the first play of
        // a progression sits at the bottom of the range rather
        // than one step into it.
        pitchTravel = active.PitchShaper.Advance(pitchTravel);
        volumeTravel = active.VolumeShaper.Advance(volumeTravel);

        lastPlayUnscaledTime = Time.unscaledTime;
    }


    // =========================================================
    // ONE-SHOT
    // =========================================================

    private void StartOneShot(
        SoundSettings active,
        AudioSource template,
        AudioClip clip,
        float pitch,
        float volume)
    {
        if (!useVoicePool)
        {
            PlayOnSourceDirectly(active, template, clip, pitch, volume);

            return;
        }

        ActiveVoice voice = AcquireVoice();

        if (voice == null)
        {
            WarnNoVoice();

            return;
        }

        AudioSource source = voice.Source;

        ConfigureVoice(source, template, active);

        source.clip = clip;
        source.loop = false;
        source.pitch = pitch;
        source.volume = active.FadeInSeconds > 0f ? 0f : volume;

        // A negative pitch plays backwards, but a source started
        // at sample zero is already AT the end going that way,
        // so it stops instantly. Starting it at the last sample
        // is what makes reverse playback actually sound.
        if (pitch < 0f)
        {
            source.timeSamples = Mathf.Max(0, clip.samples - 1);
        }

        source.Play();

        if (active.FadeInSeconds > 0f)
        {
            voice.Fade =
                StartCoroutine(
                    FadeVolume(voice, 0f, volume, active.FadeInSeconds, false)
                );
        }

        // Pitch changes how long a clip takes. Half speed is
        // twice the wall-clock time, and a release timer that
        // ignores that hands the voice back mid-sound.
        float duration = clip.length / Mathf.Abs(pitch);

        voice.Release =
            StartCoroutine(
                ReleaseWhenFinished(voice, duration)
            );
    }


    // The no-pool path. Honest about its limitation: this is
    // the behaviour the pool exists to avoid, offered for the
    // cases where a caller genuinely wants the sound on that
    // exact source.
    private void PlayOnSourceDirectly(
        SoundSettings active,
        AudioSource source,
        AudioClip clip,
        float pitch,
        float volume)
    {
        if (source == null)
        {
            WarnNoVoice();

            return;
        }

        ApplyRouting(source, active);

        source.ignoreListenerPause = active.IgnoreListenerPause;
        source.pitch = pitch;

        source.PlayOneShot(clip, volume);
    }


    // =========================================================
    // LOOP
    // =========================================================
    //
    // PlayOneShot ignores the loop flag entirely - a looping
    // sound has to go through Play() on a source of its own.
    //
    // Starting a loop that is already running does nothing,
    // rather than restarting it from the top.

    private void StartLoop(
        SoundSettings active,
        AudioSource template,
        AudioClip clip,
        float pitch,
        float volume)
    {
        if (IsLoopPlaying)
        {
            return;
        }

        if (loopVoice != null)
        {
            ReturnVoice(loopVoice);
            loopVoice = null;
        }

        AudioSource source;

        if (useVoicePool)
        {
            ActiveVoice voice = AcquireVoice();

            if (voice == null)
            {
                WarnNoVoice();

                return;
            }

            loopVoice = voice;
            source = voice.Source;

            ConfigureVoice(source, template, active);
        }
        else
        {
            if (template == null)
            {
                WarnNoVoice();

                return;
            }

            loopVoice =
                new ActiveVoice { Source = template, Pooled = false };
            source = template;

            ApplyRouting(source, active);

            source.ignoreListenerPause = active.IgnoreListenerPause;
        }

        source.clip = clip;
        source.loop = true;
        source.pitch = pitch;
        source.volume = active.FadeInSeconds > 0f ? 0f : volume;

        source.Play();

        if (active.FadeInSeconds > 0f)
        {
            loopVoice.Fade =
                StartCoroutine(
                    FadeVolume(loopVoice, 0f, volume, active.FadeInSeconds, false)
                );
        }
    }


    // =========================================================
    // STOP
    // =========================================================

    public void Stop()
    {
        StopFaded(ActiveSettings.FadeOutSeconds);
    }


    public void StopFaded(float seconds)
    {
        if (seconds <= 0f)
        {
            StopImmediate();

            return;
        }

        // Copied first: a fade that completes will mutate the
        // list it is being read from.
        ActiveVoice[] fading = activeVoices.ToArray();

        for (int i = 0; i < fading.Length; i++)
        {
            ActiveVoice voice = fading[i];

            if (voice.Source == null)
            {
                continue;
            }

            CancelRoutines(voice);

            voice.Fade =
                StartCoroutine(
                    FadeVolume(voice, voice.Source.volume, 0f, seconds, true)
                );
        }

        if (loopVoice != null && !activeVoices.Contains(loopVoice))
        {
            FadeOutUnpooledLoop(seconds);
        }
    }


    public void StopImmediate()
    {
        ActiveVoice[] stopping = activeVoices.ToArray();

        for (int i = 0; i < stopping.Length; i++)
        {
            ReturnVoice(stopping[i]);
        }

        activeVoices.Clear();

        if (loopVoice != null)
        {
            if (loopVoice.Source != null)
            {
                CancelRoutines(loopVoice);
                loopVoice.Source.Stop();
                loopVoice.Source.loop = false;
            }

            loopVoice = null;
        }
    }


    private void FadeOutUnpooledLoop(float seconds)
    {
        ActiveVoice voice = loopVoice;

        if (voice == null || voice.Source == null)
        {
            return;
        }

        CancelRoutines(voice);

        voice.Fade =
            StartCoroutine(
                FadeVolume(voice, voice.Source.volume, 0f, seconds, true)
            );
    }


    // =========================================================
    // PROGRESSION
    // =========================================================
    //
    // Puts a climbing sound back at the bottom of its range.
    // Call it when a streak breaks, or let the idle reset on
    // the sound do it after a gap.

    public void ResetProgression()
    {
        pitchTravel = 0f;
        volumeTravel = 0f;
        progressionStarted = false;
        orderIndex = 0;
        lastClipIndex = -1;
    }


    private void PrepareProgression(SoundSettings active)
    {
        float idleReset = active.ProgressionResetSeconds;

        bool goneQuiet =
            idleReset > 0f
            && progressionStarted
            && Time.unscaledTime - lastPlayUnscaledTime > idleReset;

        if (goneQuiet)
        {
            ResetProgression();
        }

        progressionStarted = true;
    }


    // =========================================================
    // RUNTIME OVERRIDES
    // =========================================================
    //
    // The runtime group beats the sound's own routing, which in
    // turn beats whatever the AudioSource was set to. Clearing
    // it hands control back.
    //
    // Both take effect from the NEXT sound - a shot already in
    // the air keeps the bus it started on.

    public void SetOutputGroup(AudioMixerGroup group)
    {
        runtimeGroup = group;
        hasRuntimeGroup = true;
    }


    public void ClearOutputGroup()
    {
        runtimeGroup = null;
        hasRuntimeGroup = false;
    }


    // A blanket multiplier over whatever the sound resolves to.
    // For ducking a whole emitter without editing its settings.
    public void SetVolumeScale(float scale)
    {
        volumeScale = Mathf.Clamp01(scale);
    }


    // Convenience pass-through so a caller holding a SoundPlayer
    // does not have to reach for MixerVolume separately. The
    // conversion, and the warning about snapshots, live there.
    public bool SetMixerVolume(
        AudioMixer mixer,
        string exposedParameter,
        float linear)
    {
        return MixerVolume.Set(mixer, exposedParameter, linear);
    }


    // =========================================================
    // CLIP CHOICE
    // =========================================================

    private AudioClip PickClip(SoundSettings active)
    {
        AudioClip[] clips = active.Clips;

        if (clips == null || clips.Length == 0)
        {
            return null;
        }

        if (clips.Length == 1)
        {
            return clips[0];
        }

        int index;

        switch (active.PickMode)
        {
            case SoundClipPickMode.InOrder:
                index = orderIndex % clips.Length;
                orderIndex = (orderIndex + 1) % clips.Length;
                break;

            case SoundClipPickMode.RandomNoRepeat:
                // An offset of at least one, taken modulo the
                // length, lands anywhere EXCEPT where we just
                // were - uniformly, and without a retry loop.
                index =
                    lastClipIndex < 0
                        ? Random.Range(0, clips.Length)
                        : (lastClipIndex
                           + 1
                           + Random.Range(0, clips.Length - 1))
                          % clips.Length;
                break;

            default:
                index = Random.Range(0, clips.Length);
                break;
        }

        lastClipIndex = index;

        return clips[index];
    }


    // =========================================================
    // VOICES
    // =========================================================

    private ObjectPool Pool =>
        poolHost != null ? poolHost.Pool : localPool;


    private Transform VoiceParent =>
        poolHost != null ? poolHost.PoolParent : transform;


    private void EnsurePoolRegistered()
    {
        ObjectPool pool = Pool;

        if (pool == null || pool.IsRegistered(PoolKeys.AudioVoice))
        {
            return;
        }

        Transform parent = VoiceParent;

        // maxSize stays at zero on purpose. This component caps
        // itself at maxVoices, so the pool never has to recycle
        // a voice out from under whoever is holding it.
        pool.Register(
            PoolKeys.AudioVoice,
            () => CreateVoice(parent)
        );
    }


    private static AudioSource CreateVoice(Transform parent)
    {
        GameObject carrier = new GameObject("Pooled Audio Voice");

        carrier.transform.SetParent(parent, false);

        AudioSource source = carrier.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.loop = false;

        return source;
    }


    private ActiveVoice AcquireVoice()
    {
        bool atCap =
            maxVoices > 0
            && activeVoices.Count >= maxVoices;

        if (atCap)
        {
            ActiveVoice oldest = activeVoices[0];

            activeVoices.RemoveAt(0);
            CancelRoutines(oldest);

            if (oldest.Source != null)
            {
                oldest.Source.Stop();

                if (oldest == loopVoice)
                {
                    loopVoice = null;
                }

                activeVoices.Add(oldest);

                return oldest;
            }

            // The source was destroyed under us. Hand it back
            // and fall through to a fresh one.
            Pool?.Release(PoolKeys.AudioVoice, oldest.Source);
        }

        ObjectPool pool = Pool;

        if (pool == null)
        {
            return null;
        }

        EnsurePoolRegistered();

        AudioSource borrowed = pool.Get<AudioSource>(PoolKeys.AudioVoice);

        if (borrowed == null)
        {
            return null;
        }

        ActiveVoice voice =
            new ActiveVoice { Source = borrowed, Pooled = true };

        activeVoices.Add(voice);

        return voice;
    }


    private void ReturnVoice(ActiveVoice voice)
    {
        if (voice == null)
        {
            return;
        }

        activeVoices.Remove(voice);
        CancelRoutines(voice);

        if (voice == loopVoice)
        {
            loopVoice = null;
        }

        AudioSource source = voice.Source;

        if (source != null)
        {
            source.Stop();
        }

        if (!voice.Pooled)
        {
            // A scene-owned source keeps its clip and its loop
            // flag. Wiping those would quietly un-author the
            // AudioSource somebody set up in the Inspector.
            return;
        }

        if (source != null)
        {
            source.loop = false;
            source.clip = null;
        }

        Pool?.Release(PoolKeys.AudioVoice, source);
    }


    private void CancelRoutines(ActiveVoice voice)
    {
        if (voice.Release != null)
        {
            StopCoroutine(voice.Release);
            voice.Release = null;
        }

        if (voice.Fade != null)
        {
            StopCoroutine(voice.Fade);
            voice.Fade = null;
        }
    }


    // =========================================================
    // VOICE SETUP
    // =========================================================
    //
    // A borrowed voice is nobody's until it is told what it is.
    // Copying the template's spatial settings is what stops a
    // pooled 3D sound from either going silent or playing flat
    // in both ears at the listener's feet.

    private void ConfigureVoice(
        AudioSource voice,
        AudioSource template,
        SoundSettings active)
    {
        if (template != null)
        {
            voice.outputAudioMixerGroup = template.outputAudioMixerGroup;
            voice.spatialBlend = template.spatialBlend;
            voice.minDistance = template.minDistance;
            voice.maxDistance = template.maxDistance;
            voice.rolloffMode = template.rolloffMode;
            voice.dopplerLevel = template.dopplerLevel;
            voice.spread = template.spread;
            voice.panStereo = template.panStereo;
            voice.reverbZoneMix = template.reverbZoneMix;
            voice.bypassEffects = template.bypassEffects;
            voice.bypassListenerEffects = template.bypassListenerEffects;
            voice.bypassReverbZones = template.bypassReverbZones;
            voice.priority = template.priority;
            voice.ignoreListenerVolume = template.ignoreListenerVolume;

            if (template.rolloffMode == AudioRolloffMode.Custom)
            {
                voice.SetCustomCurve(
                    AudioSourceCurveType.CustomRolloff,
                    template.GetCustomCurve(
                        AudioSourceCurveType.CustomRolloff
                    )
                );
            }

            voice.transform.position = template.transform.position;
        }
        else
        {
            voice.outputAudioMixerGroup = null;
            voice.spatialBlend = 0f;
            voice.transform.position = transform.position;
        }

        ApplyRouting(voice, active);

        voice.ignoreListenerPause = active.IgnoreListenerPause;
    }


    private void ApplyRouting(AudioSource source, SoundSettings active)
    {
        if (hasRuntimeGroup)
        {
            source.outputAudioMixerGroup = runtimeGroup;

            return;
        }

        if (active.Routing == SoundMixerRouting.Override)
        {
            source.outputAudioMixerGroup = active.OutputGroup;
        }

        // Inherit changes nothing on purpose - whatever the
        // source was already routed to keeps its routing.
    }


    private AudioSource ResolveTemplate(AudioSource explicitSource)
    {
        if (explicitSource != null)
        {
            return explicitSource;
        }

        return targetSource;
    }


    private void EnsureTargetSource()
    {
        if (targetSource != null)
        {
            return;
        }

        if (TryGetComponent(out AudioSource existing))
        {
            targetSource = existing;

            return;
        }

        // Nothing to inherit routing or spatial settings from,
        // and nothing to play on at all when the pool is off.
        // One is added rather than failing quietly.
        targetSource = gameObject.AddComponent<AudioSource>();
        targetSource.playOnAwake = false;
        targetSource.loop = false;
    }


    // =========================================================
    // COROUTINES
    // =========================================================
    //
    // Both of these run on unscaled time. The audio engine has
    // its own clock and ignores Time.timeScale entirely, so a
    // fade driven by scaled time freezes halfway the moment the
    // game is paused while the sound carries on regardless.

    private IEnumerator FadeVolume(
        ActiveVoice voice,
        float from,
        float to,
        float seconds,
        bool releaseAtEnd)
    {
        AudioSource source = voice.Source;

        if (source == null)
        {
            yield break;
        }

        source.volume = from;

        float elapsed = 0f;

        while (elapsed < seconds)
        {
            if (voice.Source == null)
            {
                yield break;
            }

            elapsed += Time.unscaledDeltaTime;

            voice.Source.volume =
                Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / seconds));

            yield return null;
        }

        if (voice.Source != null)
        {
            voice.Source.volume = to;
        }

        voice.Fade = null;

        if (releaseAtEnd)
        {
            if (voice.Source != null)
            {
                voice.Source.Stop();
            }

            ReturnVoice(voice);
        }
    }


    private IEnumerator ReleaseWhenFinished(
        ActiveVoice voice,
        float seconds)
    {
        if (seconds > 0f)
        {
            yield return new WaitForSecondsRealtime(seconds);
        }

        // A paused listener stops the sound advancing but leaves
        // the source playing, so the timer alone would hand the
        // voice back mid-sound. Waiting for the source to
        // actually finish covers both kinds of pause.
        while (voice.Source != null && voice.Source.isPlaying)
        {
            yield return null;
        }

        voice.Release = null;

        ReturnVoice(voice);
    }


    // =========================================================
    // WARNINGS
    // =========================================================
    //
    // Once each. A misconfigured emitter that fires every frame
    // should say so, not bury the console.

    private void WarnNoClips()
    {
        if (warnedNoClips)
        {
            return;
        }

        warnedNoClips = true;

        GameLog.Warning(
            "SoundPlayer on '" + name + "' was asked to play but "
            + "has no clip. Assign one on the component, or on "
            + "the SoundDefinition it points at."
        );
    }


    private void WarnNoVoice()
    {
        if (warnedNoVoice)
        {
            return;
        }

        warnedNoVoice = true;

        GameLog.Warning(
            "SoundPlayer on '" + name + "' could not get an "
            + "AudioSource to play on. Check its Target Source "
            + "and Pool Host."
        );
    }


    // =========================================================
    // EDITOR
    // =========================================================
    //
    // Right-click the component header in Play Mode. Saves
    // writing a throwaway script to hear what a range sounds
    // like.

    [ContextMenu("Play Test")]
    private void PlayTest()
    {
        if (!Application.isPlaying)
        {
            GameLog.Warning(
                "SoundPlayer test only works in Play Mode."
            );

            return;
        }

        Play();
    }


    [ContextMenu("Reset Progression")]
    private void ResetProgressionFromMenu()
    {
        ResetProgression();
    }
}
