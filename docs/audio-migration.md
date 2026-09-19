# Audio migration notes

This document changes no code. It records what the project's audio looks like today, what
`SoundPlayer` would replace, and what would have to happen first. Nothing here is scheduled.

Every script named below is **frozen** under `code-index.md` §0. None of it was touched by the
`feature/audio` branch, and none of it should be touched without the freeze being lifted first.

---

## What the new code is

| Path | What |
|---|---|
| `Assets/Scripts/Core/Audio/SoundPlayer.cs` | The component. Plays a sound against any AudioSource, with optional pitch and volume variation, looping or one-shot, mixer routing overridable at runtime. |
| `Assets/Scripts/Core/Audio/SoundSettings.cs` | Everything that describes a sound. Held inline by `SoundPlayer` or by a `SoundDefinition`. |
| `Assets/Scripts/Core/Audio/SoundDefinition.cs` | ScriptableObject preset. `Assets > Create > Audio > Sound`. |
| `Assets/Scripts/Core/Audio/SoundShaper.cs` | Picks the next pitch or volume: none, random, or stepping. Pure — holds no cursor. |
| `Assets/Scripts/Core/Audio/PitchScale.cs` | Scale-step to pitch. Pure maths, no Unity state. |
| `Assets/Scripts/Core/Audio/MixerVolume.cs` | Linear volume to decibels, and back. |
| `Assets/Scripts/Core/Audio/SoundEnums.cs` | The enums. Serialized as ints — **do not reorder**. |
| `Assets/Scripts/Core/Pooling/ObjectPool.cs` | String-keyed pool, general purpose. |
| `Assets/Scripts/Core/Pooling/PoolHost.cs` | MonoBehaviour that owns one pool. |
| `Assets/Scripts/Core/Pooling/PoolKeys.cs` | The keys. |

---

## The mixer

`Assets/WorldSound.mixer` had **one group, `Master`**, with **one exposed parameter,
`MasterVolume`**, one `Attenuation` effect and one snapshot — no Music / SFX / UI split.
It now has all three as children of `Master`.

### Manual step — adding the groups (DONE)

`Master` now has `Music`, `SFX` and `UI` children, exposed as `MusicVolume`, `SfxVolume`
and `UiVolume`. `Master` and `MasterVolume` were not touched. Kept below for the record,
and for whoever has to do it again on another mixer:

1. `Window > Audio > Audio Mixer`, select **WorldSound**.
2. On the **Groups** panel, right-click `Master` → **Add child group**. Name it `Music`.
3. Repeat for `SFX` and `UI`.
4. Select `Music`. In the Inspector, right-click the **Volume** label → **Expose 'Volume (of Music)'
   to script**.
5. In the **Exposed Parameters** dropdown (top right of the Audio Mixer window), rename the new
   entry to `MusicVolume`.
6. Repeat steps 4–5 for `SFX` → `SfxVolume` and `UI` → `UiVolume`.

Leave `Master` and `MasterVolume` alone. `MasterAudioManager` writes `MasterVolume` every frame and
is frozen; adding children under `Master` does not change what that parameter does, so the existing
volume slider keeps working exactly as it does now.

Nothing in any scene is re-routed to the new groups by this branch.

---

## What would migrate, and to what

### `Assets/Scripts/fadeInAudio.cs` — world music crossfade

107 lines. Four copy-pasted branches on `player.worldMusic == 0..3`, each assigning `music.clip` and
calling `music.Play()`, then two coroutines that ramp `music.volume` up and down with
`Time.deltaTime`.

- Becomes one `SoundPlayer` in **Loop** mode with a fade-in and fade-out duration, plus a clip swap.
- The fade coroutines go away entirely. `SoundPlayer` fades on **unscaled** time, so a fade no
  longer freezes halfway when the game is paused.
- **Latent bug, not fixed:** in three of the four branches the clip is assigned *after* `Play()` is
  called, so the first frame of the new theme plays the old clip. Left as found.

### `Assets/Scripts/ChangeWorldMusic.cs`

10 lines, no audio API at all — three ints that `fadeInAudio` reads through `PlayerController`.
Would survive a migration unchanged as the trigger that tells the music player which theme to use.

### `Assets/Scripts/PlayButtonSound.cs` — UI click

26 lines. `audioPlayer.PlayOneShot(buttonNoise)`, wired from `Button.onClick` in the Inspector.

- Replaced outright. `SoundPlayer.Play()` is already a no-argument public method, so the same
  `onClick` wiring points at it directly and the script is deleted.
- Gains pitch variation for free, which is what stops a rapidly clicked button sounding like a
  machine gun.

### `Assets/Scripts/PlayerController.cs` — walk, run, teleport, treasure

Frozen and 1,498 lines. Holds `WalkNoise`, `SlowWalkNoise`, `TeleNoise`, `TreasureSound`, two
AudioSources, and the `SoundPlayer.Stop(); SoundPlayer.PlayOneShot(...)` pattern in four places.

- The footstep loops are the strongest case for the new component anywhere in the project: they
  repeat constantly, on one source, with no variation, and `Stop()` before each shot is exactly the
  self-cutting-off this component avoids.
- Migration would mean adding fields to `PlayerController`, which `code-index.md` §10 forbids
  outright. It would have to be a separate emitter object instead, driven by the existing state.
- **Note:** the private field in `PlayerController` is itself called `SoundPlayer`. That name now
  collides conceptually with the new class. Renaming it is a frozen-file edit; leave it.

### `Assets/Scripts/JumpScript.cs`, `Player_Seer.cs`, `RouteChanger.cs`, `QuackController.cs`

Each is the same shape: one clip, one source, stop-then-play. Each becomes one `SoundPlayer` with a
small random pitch range, called from wherever the existing script is called from.

### `Assets/Scripts/CoinManager.cs`

`CoinSound.Play()` in three village branches. A single `SoundPlayer` in **Step** mode with
`MusicalSteps` on a pentatonic scale is the obvious upgrade — successive coins climb, which is the
effect the mode was built for.

- **Dead field, not removed:** `public AudioClip CoinClip` is declared and never read.

### `Assets/Scripts/DialogManagerScript.cs` — mouth noise

`TalkingNoise.Play()` gated on `isTalking && !isPlaying`. A **Loop**-mode `SoundPlayer` started and
stopped on the same condition, with a fade-out so speech does not cut abruptly.

- **Note:** line 32 re-grabs the component with `GetComponent`, overwriting whatever was assigned in
  the Inspector.

### `Assets/Scripts/ScriptReader.cs` — Ink runner

Declares `public AudioClip[] audioLines` and bounds-checks `lineIndex` against it, but **never
plays anything**. Dead wiring. If VO per line is ever wanted, this is where a `SoundPlayer` with
`PickClip` in `InOrder` mode would go.

### `Assets/Scripts/NightManagerScript.cs`

`Entry` AudioSource is assigned in `Awake` and never used. Dead field.

---

## What does not migrate

### `Assets/Scripts/AudioManager.cs` — already dead

189 lines, fifteen `AudioClip` fields, fourteen near-identical `voice.Stop(); voice.PlayOneShot(lineN);`
blocks. It appears only in `Scene1.unity`, **which is not in build settings**, so it cannot run in a
build. `code-index.md` §7 already flags this. Migrating it would change nothing a player can hear.

### `Assets/MasterAudioManager.cs` and `Assets/SetVolumeSlider.cs`

These stay. They are the only mixer-aware code in the project and they own `MasterVolume`. Nothing
in `Core/Audio` writes that parameter — `MixerVolume` deliberately follows the same
`Mathf.Log10(v) * 20` conversion `MasterAudioManager` already uses, so the two agree rather than
fighting.

- **Note:** `MasterAudioManager` calls `SetVolume` from `Update`, so it writes the mixer every
  frame. Harmless, but it means a snapshot can never move `MasterVolume`.
- **Dead field:** `SetVolumeSlider` declares an `AudioListener` it never uses.

### `MixerManager.cs`, `MixerMugManager.cs`, `mixerTutorialManager.cs`

Not audio. These are the drink-mixing minigame.

---

## Things worth knowing before any of this happens

- **`3DEnviroment.unity` has 18 AudioListeners.** Unity expects exactly one and behaves
  unpredictably with more. This predates everything here and is frozen.
- **~140 AudioSources** exist across the scenes, all in frozen territory.
- **`Crybt.unity` has zero authored AudioSources** and one AudioListener. Crybt audio now
  runs entirely on pooled voices created at runtime, so that stays true.
- **23 of the 31 committed clips are now in use** through `CrybtSoundTable`. The eight not
  yet spoken for are `MonsterAttack`, `MonsterHurtStagger`, `PlayerHeal`,
  `AttackBlockedParried`, `ChestDoorOpenInCrypt`, `SaveConfirmation`, `SliderTick`,
  `PauseUnpause` and `BackCancel` — each needs a game event that does not exist yet
  (there is no heal, no block, no pause and no save in the Crybt).
- **A `SoundSettings` ScriptableObject existed on the reverted refactor branch** and is mentioned in
  `code-index.md` §9 and in a stale comment at `Core/Editor/DefaultDataAssetGenerator.cs:30`. That
  type is **not** this one, does not exist on this branch, and should not be referenced.
