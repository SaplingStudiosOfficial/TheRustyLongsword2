# Code Index — what actually runs in the shipped game

**Generated:** 2026-09-13 · **Unity:** 2020.3.19f1 LTS · **Branch:** `refactor/solid`

This index exists because the project is a new game built on top of an old one,
and it is no longer obvious by looking which files are load-bearing. Every
statement below is derived from the asset graph, not from names or intuition.

---

## 0. ACTIVE SCOPE — read this before touching anything

**The Crybt is the only part of this project under active development.**
Everything else is frozen. This is a deliberate decision by the project owner,
made after §4 established that the overworld, taverns and recap are either
legacy or unreachable.

### In scope — change these freely

| Path | What |
|---|---|
| `Assets/Scripts/CrybtManager/**` | The whole Crybt layer: `CrybtManager`, `Card`, `CaveIntro`, `ShakeOnEnable`, `SimpleTextShadow`, plus `Data/`, `Rules/`, `Runtime/`, `View/`, `Editor/` |
| `Assets/Scripts/Core/Math/Easing.cs` | Used by `Card`, `CrybtManager`, `CaveIntro` |
| `Assets/Scripts/Core/Logging/GameLog.cs` | Used by `Card`, `CrybtManager`, `CrybtScoring` |
| `Assets/Scripts/Core/Extensions/UiVisibilityExtensions.cs` | `SetVisible`, used by `EncounterHud` |
| `Assets/Scripts/Core/Editor/DefaultDataAssetGenerator.cs` | Creates `CrybtRules` + the six `BoonDefinition` assets |
| `Assets/Scripts/Core/Audio/**` | `SoundPlayer` and its supporting types — see §11 |
| `Assets/Scripts/Core/Pooling/**` | `ObjectPool`, `PoolHost`, `PoolKeys` — see §11 |
| `Assets/csc.rsp` | Project-wide `CS0649` suppression — **options only, never comments** (see below) |
| `Assets/Resources/Crybt/**` | `CrybtRules` and the six `BoonDefinition` assets, loaded at runtime when unassigned |
| `Assets/Scenes/Crybt.unity` | Via the Editor only — never by editing YAML |
| `docs/**` | This index and the player's guide |

That is the entire surface. Four files plus two shared folders in `Core/`, one scene,
two Crybt folders.

> **`csc.rsp` has no comment syntax in Unity 2020.3.** Unity splits the file on
> whitespace and passes every token to the compiler as an argument — it does
> **not** strip `#` comments the way Roslyn would. A comment containing a bare
> `-` becomes the compiler option `/`, which fails with `CS2007` **for every
> assembly in the project**, including `UnityEngine.UI` and `TestRunner`, and
> blocks Player builds. The whole file must be compiler options, one per line,
> nothing else. Put the rationale in this index instead.
>
> The rationale: every `CS0649` in this project is a `[SerializeField] private`
> field that Unity assigns from the Inspector. The compiler cannot see that, so
> the warning is always wrong here. Unity already suppresses `CS0169` for the
> same reason; `-nowarn:0649` covers the other half, which is what makes
> `[SerializeField] private` usable as the house pattern instead of falling back
> to public fields purely to silence the compiler.

### Frozen — do NOT modify

Everything else, specifically including:

- **`PlayerController`** and its whole orbit — inventory, quest flags, saving,
  camera, audio settings
- **`SaveSystem`**, `SaveManagerScript`, and anything about save format
- **Characters and dialogue** — `NPCDialogManager`, `DialogManagerScript`,
  `ScriptReader` (Ink), all mouth-movers, `JefferyManager`
- **Coins and collectables** — `CoinManager`, `UniqueCoinManager`,
  `CollectableIconManager`, `DisplayManager`, `ProgressTracker`, `IconManagerScript`
- **Movement** — `JumpScript`, `CameraController`, `PixelatedCamera`,
  `SensitivityMangerScript`
- **Night loop** — `NightEnder`, `NightManagerScript`, `NextPageInfoBool`,
  `nextPageInfo`, `OpenBookScript`, `PageFlipAnimationManager`
- **Taverns** — `MixerManager`, `MixerMugManager`, `mixerTutorialManager`,
  `QuackController`, `ConfirmationMenuManager`, `SetTimeline*`
- **Audio** — `MasterAudioManager`, `AudioManager`, `SetVolumeSlider`,
  `SensitivitySliderScript`, `ChangeWorldMusic`, `fadeInAudio`
- **World plumbing** — `ExitGame`, `ExitTitleMenu`, `LoadBarScene`,
  `EnterBarManager`, `ChangeWorldMusic`, every `Tags`/scene-name constant
- All scenes other than `Crybt`

### Rules for frozen code

1. **Read it, cite it, do not edit it.** It is fine — often necessary — to read
   frozen files to understand how the Crybt is entered or what the save format
   looks like. Changing them is out of scope.
2. **Do not refactor it in passing.** No renames, no extracting constants, no
   swapping `Debug.Log` for `GameLog`, no tidying. A mechanical sweep across
   frozen files is exactly what this scope exists to prevent.
3. **Do not "fix" bugs you find there.** §7 and §8 list several real ones. Report
   them; leave them. They are not observable in play today.
4. **If Crybt work genuinely requires a frozen file to change, stop and ask.**
   Do not decide unilaterally that a dependency justifies the edit.

> **History:** an earlier pass refactored ~45 frozen files (interfaces,
> ScriptableObjects, a save-format v2, constant extraction). That work was
> reverted to keep this branch to the Crybt. It is preserved on the branch
> `backup/refactor-solid-full` if any of it is ever wanted — do not reapply it
> wholesale.

---

## 1. How this was derived

The build settings name the only scenes that ship. Everything reachable from
those scenes is live; everything else is not. The walk was:

1. **`ProjectSettings/EditorBuildSettings.asset`** → the 7 scenes in the build.
2. **Scene YAML** → every `guid:` reference, resolved through `.meta` files.
3. **Recurse** into every referenced `.prefab`, `.asset`, `.controller`, `.playable`.
4. **Instance analysis** — for every `MonoBehaviour` (class `114`), resolve its
   `m_GameObject`, walk the transform tree for `m_IsActive`, and read the
   component's own `m_Enabled`.
5. **Code closure** — from the scene-reached set, follow C# type references
   transitively. This is what catches the plain classes, statics, enums and
   ScriptableObjects that no scene references directly.

Two things this method is deliberately careful about:

- **`Resources.Load` is used in exactly one place.** `CrybtManager` loads the
  shipped `CrybtRules` and boon assets from `Assets/Resources/Crybt/` when the
  Inspector references are empty. Those assets are therefore live without any
  scene reference — the walk below would not otherwise see them. Nothing else
  in the game calls it; the rest of the asset graph is authoritative.
- **New files have no `.meta` yet.** Files added in this refactor are invisible
  to a GUID walk by construction, so step 5 is what proves them live.

To regenerate, re-run the same walk; nothing here was entered by hand.

---

## 2. Headline numbers

Measured on `refactor/solid` after the branch was narrowed to Crybt scope (§0).

| Category | Files | Note |
|---|---:|---|
| **Live game code** | **96** | 72 scene-attached, 5 prefab-attached, 19 code-reachable |
| Vendor / third-party | 178 | Ink 117, Boxophobic 59, LowPolyWater 2 |
| Editor tools (Crybt) | 3 | one-shot migrations, not shipped |
| **Dead** | **6** | no reference from any shipping scene |
| **Total `.cs` in `Assets/`** | **283** | |

**63% of the `.cs` in this project is vendor code you did not write and should
not touch.** The game itself is 96 files, ~10,300 lines.

### The assumption this index was built to test

The concern was that character, tavern and coin code might be leftovers from
the old game. The answer is split, and the split is the most important fact in
this document:

- **Coins — live.** 144 coin prefab instances in `3DEnviroment`, all active
  (`Coin` 47, `west_coin` 49, `Boenaha_Coin` 48), each carrying `CoinManager`.
  `UniqueCoinManager` has 9 active instances.
- **Characters — live.** `NPCDialogManager` has 17 active instances in
  `3DEnviroment`, driving 45 dialog panels.
- **Tavern — shipped but UNREACHABLE.** All three `InBar*` scenes are in build
  settings and full of correctly-wired active components, so every static check
  says they are live. **No code path can load them.** See §4.

That last row is why file-level analysis is not enough here. A scene can be in
the build, compile clean, and still be unreachable. **Four of the seven shipped
scenes cannot be reached at runtime.**

This branch now changes 3 existing files (`CrybtManager`, `Card`, `CaveIntro`)
and adds 18 new ones, all inside the Crybt scope defined in §0. The ~45
overworld files an earlier pass touched have been reverted.

---

## 3. The build path

Scene order is the build order. **`StoryBook` is index 0 — it is the entry point.**

| # | Scene | Role | Scripts | Reachable at runtime? |
|---:|---|---|---:|---|
| 0 | `Scenes/StoryBook.unity` | Opening book / title | 11 | **yes** — entry point |
| 1 | `Scenes/3DEnviroment.unity` | The overworld — villages, coins, NPCs | 43 | **yes** — from StoryBook |
| 2 | `Scenes/InBar.unity` | Hatfield tavern | 25 | **NO** — see §4 |
| 3 | `Scenes/InBar_Eastwood.unity` | Eastwood tavern | 22 | **NO** — see §4 |
| 4 | `Scenes/InBar_Boenaha.unity` | Boenaha tavern | 23 | **NO** — see §4 |
| 5 | `Scenes/NightRecap.unity` | End-of-night book | 11 | **NO** — see §4 |
| 6 | `Scenes/Crybt.unity` | The Crybt card game | 6 | **yes** — one Button, no exit |

Being listed here is necessary but not sufficient. §4 traces which of these are
actually entered.

### Scenes that exist but do NOT ship

These are in the repo and open fine in the Editor, but are **not in build
settings**, so `SceneManager.LoadScene` can never reach them at runtime:

| Scene | What it holds | Consequence |
|---|---|---|
| `Scene1.unity` | The voiced intro (`AudioManager` ×3, `ScriptReader`) | **`AudioManager` is dead code** |
| `StealthLevelConcept.unity` | Prototype stealth level | nothing lost — `ManageStoryLock` also ships on `Gate.prefab` |
| `Filler.unity`, `SampleScene.unity` | Empty / scratch | — |

`SceneNames.Intro = "Scene1"` therefore points at a scene that cannot load.
Nothing calls it, so this is latent, not a live bug — but it is a trap.

---

## 4. Runtime reachability — what actually loads

Being in build settings only means a scene *can* be loaded. This section traces
whether anything ever *does*.

Every route between scenes was enumerated: `SceneManager.LoadScene` calls in
C#, `UnityEvent` persistent calls in scene YAML, animation events (`.anim`
`functionName`), and Timeline signals. There are no Timeline signal assets and
no `SignalReceiver` in the project, the five animation events are local effects
(`Quack`, `Deactivate`, `setPageFlipActive`, `disableCollectable`, `StopQuack`),
and the only `Resources.Load` in the game loads Crybt tuning assets, not a
scene. The graph below is complete.

```
  [ StoryBook ]  ← build index 0, the entry point
        │
        │  new player:      Space through the page chain (nextPageInfo ×59)
        │                   → the last page activates CameraZoomIn → loads World
        │  returning player: CheckifAlreadyseen sees hasSeenCutscene,
        │                   waits 2s → loads World
        ▼
  [ 3DEnviroment ]  ← the overworld. 43 scripts, 144 coins, 17 NPC managers
        │
        ├── one Button → LoadBarScene.LoadBar() ────────────┐
        │                                                   ▼
        ├── ExitTitleMenu.Exit / ExitGame.exitGame    [ Crybt ]  ← TERMINAL
        │      → Application.Quit()                    no exit of any kind
        │
        └── CreditsScript → reloads 3DEnviroment

  UNREACHABLE (in the build, but no inbound edge exists):
      [ InBar ]   [ InBar_Eastwood ]   [ InBar_Boenaha ]   [ NightRecap ]
```

### The taverns are orphaned

No C# file contains the literal `"InBar"` anywhere. The only thing that could
name those scenes is `LoadBarScene.LoadScene(string sceneToLoad)` — and:

- Its parameter **shadows** the `[SerializeField] string sceneToLoad` field, so
  the five instances carrying `sceneToLoad: InBar` in `3DEnviroment` are inert
  data. The field is never read.
- `LoadScene` is not wired to any `UnityEvent` in any scene.

The one Button that used to open the tavern now calls the **other** method,
`LoadBar()`, which is hardcoded to `"Crybt"`. The evidence is still sitting in
the scene file — the call carries a stale leftover argument:

```yaml
m_TargetAssemblyTypeName: LoadBarScene, Assembly-CSharp
m_MethodName: LoadBar          # takes no parameters
m_StringArgument: InBar        # ignored — left over from LoadScene("InBar")
```

**This predates the refactor.** At `HEAD`, `LoadBar()` already read
`SceneManager.LoadScene("Crybt")`; the only change made was swapping the
literal for `SceneNames.Crybt`. The tavern was re-pointed at the Crybt before
any of this work started.

### NightRecap is orphaned by consequence

The only route into `NightRecap` is `ExitGame.loadRecap()`. Nothing calls it
directly — no `UnityEvent` in any scene names it. It is reached solely through
`ExitGame.autoExit`, which fires `WaitThenRecap()` from `Start()`. And
`autoExit: 1` is set on exactly three components in the whole project: one in
each tavern scene.

So the intended loop was **overworld → tavern → night recap → overworld**.
Cutting the tavern out of the chain silently removed the recap too. That is why
`NightRecap` still contains a working `NightEnder` that resets the player's
position per village and calls `SaveData()` — code that can no longer run.

### The Crybt is a terminal, isolated scene

`Crybt` contains six scripts and five `UnityEvent`s (`ConfirmCombination`,
`FinishScoring`, `KeepHero`, `PeekMonster`, `SwitchHero`) — all gameplay, none
navigational. Beyond that:

- **No `PlayerController`, no `ExitGame`, no `ExitTitleMenu`** in the scene.
- `CrybtManager` never references `SceneManager`, `SaveSystem` or
  `PlayerController`.
- The only two `DontDestroyOnLoad` calls in the project are in
  `IngredientsManagerScript` and `InventoryManager` — both **dead** (§7).

So nothing survives the transition into the Crybt, and nothing comes back out.
Entering it ends the session: no scene change, no save write, no quit button.
Progress earned in the overworld is not read by it, and progress earned in it
is not written anywhere.

### What this means

The intuition that "the only active part is the Crybt" is **half right, and the
half that is wrong matters**. The game is currently two disconnected halves:

| | Status |
|---|---|
| `StoryBook` → `3DEnviroment` | **Fully live.** The old game still runs, and it is the *only* way to reach the Crybt. Coins, NPCs, quests, saving — all executing. |
| `Crybt` | **Live, terminal, isolated.** Reachable by exactly one Button; cannot be left. |
| `InBar` ×3, `NightRecap` | **Unreachable.** ~11 scripts and 4 scenes of working content with no door. |

Roughly **40% of the shipped scenes cannot be entered**, and the overworld is
not optional scaffolding — it is the entry corridor to the Crybt.

Three things follow, none of which this index can decide for you:

1. **The Crybt has no way out.** If it is meant to be the game, it still needs
   an exit or an end state. If it is meant to be a mode inside the overworld, it
   needs a return path.
2. **The Crybt writes nothing.** It never touches `SaveSystem`. Crawl results,
   boons and health vanish on exit.
3. **The taverns and recap are either content to restore or content to cut.**
   Restoring costs one Button re-wire (point it back at
   `LoadScene` with `"InBar"`). Cutting frees 4 scenes and ~11 scripts.

---

## 5. Live code, by scene

`active/total` counts component instances at scene load: the GameObject is
active **and** the component is enabled. A low ratio is normal for menus and
dialog panels — see §8 for which ones are genuinely inert.

### 0 · StoryBook

| Script | active/total | Lines | Role |
|---|---:|---:|---|
| `PlayerController` | 6/6 | 1498 | God object — inventory, quest flags, save, camera, audio |
| `JumpScript` | 2/2 | 100 | Jump + its own gravity constant |
| `OpenBookScript` | 2/2 | 33 | Book open animation |
| `ResolutionSetter` | 5/5 | 10 | Forces resolution on load |
| `CheckifAlreadyseen` | 1/2 | 27 | Skips cutscene if the save says it was seen |
| `nextPageInfo` | 1/59 | 35 | Page-turn payload; 58 are pages not yet shown |
| `CameraZoomIn` | 0/2 | 28 | Activated by `PlayerController` / `nextPageInfo` |
| `DeactivateGameObject` | 0/2 | 11 | Activated by `OpenBookScript` |
| `PageFlipAnimationManager` | 0/2 | 24 | Activated by `OpenBookScript` |
| `NightEnder` | 2/8 | 57 | Ends the night, advances day |
| `OutlineScript` | 0/14 | 37 | **Inert — see §8** |

### 1 · 3DEnviroment (the overworld)

| Script | active/total | Lines | Role |
|---|---:|---:|---|
| `PlayerController` | 6/6 | 1498 | God object |
| `NPCDialogManager` | 17/17 | 406 | Drives NPC conversations |
| `DialogManagerScript` | 0/45 | 190 | The 45 dialog panels it switches on |
| `ObjectRotator` | 23/23 | 22 | Spins collectables |
| `CanvasFader` | 27/55 | 61 | Generic canvas fade |
| `GetAmtPineScript` | 14/14 | 18 | Pine-cone counter readout |
| `ExitGame` | 13/20 | 36 | Quit button |
| `MoveUpAndFadeText` | 11/21 | 42 | Floating pickup text |
| `UniqueCoinManager` | 9/9 | 45 | The 11 named collectable coins |
| `BobObject` | 13/17 | 28 | Idle bob animation |
| `QuestItemScript` | 6/6 | 32 | Quest pickup trigger |
| `ChangeWorldMusic` | 5/5 | 10 | Village music zones |
| `ResolutionSetter` | 5/5 | 10 | Resolution |
| `PlayButtonSound` | 4/4 | 26 | UI click sound |
| `CollectableIconManager` | 2/3 | 52 | Per-village collectable icons (Boenaha starts off) |
| `EnterBarManager` | 2/2 | 30 | Tavern entry trigger |
| `ExitTitleMenu` | 2/12 | 44 | Return to title |
| `JumpScript` | 2/2 | 100 | Jump |
| `ProgressTracker` | 2/2 | 155 | `AreaProgressBar` + `GameProgressBar` |
| `SignManager` | 2/2 | 52 | Readable signs |
| `TreeDistanceEditor` | 2/2 | 15 | Tree LOD distance |
| `DisplayManager` | 1/1 | 128 | Coin count display |
| `MasterAudioManager` | 1/1 | 37 | Mixer volume |
| `SensitivityMangerScript` | 1/1 | 47 | Camera speed from settings |
| `SensitivitySliderScript` | 1/1 | 28 | Sensitivity slider |
| `SetVolumeSlider` | 1/1 | 29 | Volume slider |
| `CircularMotion` | 1/1 | 38 | Orbit animation |
| `CreditsScript` | 1/1 | 84 | Credits roll |
| `HideorShowCursor` | 1/1 | 28 | Cursor visibility |
| `PixelatedCamera` | 1/1 | 84 | Pixelation post effect |
| `TurnOnIfT1H` | 1/1 | 19 | Tutorial gate |
| `TutorialTextManager` | 1/1 | 37 | Tutorial text |
| `fadeInAudio` | 1/1 | 107 | Audio fade |
| `enterBarTutorialManager` | 1/3 | 23 | Tavern tutorial |
| `LoadBarScene` | 0/12 | 20 | **The one door out of the overworld — see §4 and §8** |
| `ObjectEnder` | 0/9 | 26 | Activated by `ObjectRotator` / `UniqueCoinManager` |
| `OutlineScript` | 0/14 | 37 | **Inert — see §8** |
| `CameraController` | 0/1 | 29 | Activated by `PlayerController` / `PixelatedCamera` |
| `CheckIfTutorialHasBeenDone` | 0/1 | 23 | Activated by `FadeIn` |
| `CreditsText` | 0/1 | 66 | Activated by `CreditsScript` |
| `FadeIn` | 0/1 | 78 | Activated by `CheckIfTutorialHasBeenDone` |
| `ItemSpawner` | 0/1 | 43 | Activated by `TurnOnIfT1H` |
| `IconManagerScript` | 0/1 | 76 | **Inert — see §8** |

### 2–4 · InBar / InBar_Eastwood / InBar_Boenaha (taverns) — UNREACHABLE

> Everything below is correctly wired and would work if the scene loaded.
> Nothing can load it (§4). Treat all of it as suspended, not live: do not
> count it when estimating the blast radius of a change, and do not assume a
> bug here is observable in play.

The three tavern scenes are near-identical. Differences: `InBar` additionally
has `JefferyManager` (3/3) and `HenryMouthManager` (0/3) and `SetTimelineH`
(0/3); `InBar_Boenaha` additionally has `CameraResolutionHandler` (1/1).

| Script | active/total | Lines | Role |
|---|---:|---:|---|
| `ScriptReader` | 18/18 | 141 | **The only Ink integration point in the game** |
| `MixerManager` | 5/6 | 223 | Drink mixing minigame |
| `MixerMugManager` | 6/6 | 46 | Mug state |
| `mixerTutorialManager` | 6/6 | 28 | Mixing tutorial |
| `GeneralMouthMover` | 7/8 | 48 | Generic lipsync |
| `HensonMouthMove` | 2/8 | 67 | Henson lipsync |
| `NightManagerScript` | 3/3 | 113 | Night progression |
| `PauseMenuManager2D` | 3/3 | 33 | Pause menu |
| `QuackController` | 3/3 | 48 | Duck |
| `ConfirmationMenuManager` | 0/6 | 167 | Opened by `MixerManager` / `ScriptReader` |
| `SetTimeline` | 0/8 | 50 | Timeline cues, switched on per beat |
| `ShowDrinkThenHideDrink` | 0/10 | 62 | Drink reveal, switched on per beat |

Plus the shared set: `PlayerController`, `CanvasFader`, `BobObject`, `ExitGame`,
`ExitTitleMenu`, `GetAmtPineScript`, `NightEnder`, `PlayButtonSound`,
`ResolutionSetter`, `OutlineScript`.

### 5 · NightRecap — UNREACHABLE

> Reachable only from the taverns, which are themselves unreachable (§4).
> `NightEnder` here resets the player position per village and calls
> `SaveData()` — real logic that can no longer run.

| Script | active/total | Lines | Role |
|---|---:|---:|---|
| `NextPageInfoBool` | 1/7 | 133 | Recap page logic and flag writes |
| `nextPageInfo` | 1/59 | 35 | Page payloads |
| `PlayerController` | 6/6 | 1498 | God object |
| `CanvasFader` | 27/55 | 61 | Fades |
| `OpenBookScript` | 2/2 | 33 | Book |
| `NightEnder` | 2/8 | 57 | Night end |
| `CheckifAlreadyseen` | 1/2 | 27 | Cutscene skip |
| `CameraZoomIn`, `DeactivateGameObject`, `PageFlipAnimationManager`, `OutlineScript` | 0/n | — | switched on at runtime |

### 6 · Crybt (the card game)

The newest and cleanest part of the codebase.

| Script | active/total | Lines | Role |
|---|---:|---:|---|
| `CrybtManager` | 1/1 | 1987 | Encounter loop, offering, boons, HUD |
| `Card` | 53/53 | 568 | One per card — full 52 + back |
| `SimpleTextShadow` | 10/10 | 45 | Text shadow |
| `ShakeOnEnable` | 4/7 | 72 | Damage shake |
| `CaveIntro` | 1/1 | 75 | Cave intro animation |
| `BobObject` | 13/17 | 28 | Bob |

---

## 6. Live code with no scene presence

These carry no GUID reference from any scene, so a naive search calls them
dead. They are not — they are reached through prefabs or through C# alone.

### Attached to prefabs only

| Script | Lines | Ships via |
|---|---:|---|
| `CoinManager` | 100 | `Coin.prefab`, `west_coin.prefab`, `Boenaha_Coin.prefab` — **144 instances** |
| `ManageStoryLock` | 51 | `Gate.prefab` (51 refs in `3DEnviroment`) |
| `Player_Seer` | 35 | `PirateStealthSection 1.prefab` |
| `RouteChanger` | 60 | `PirateStealthSection 1.prefab` |
| `WaypointFollower` | 60 | `PirateStealthSection 1.prefab` |

> `ManageStoryLock` is the one to remember. It *also* appears in the non-shipping
> `StealthLevelConcept` scene, which makes it look dead. `Gate.prefab` is what
> keeps it alive.

### Reached through code only

Everything below is pulled in by a live script's type references. The Crybt
entries are new and **not yet wired in the Editor** — see §9.

| Script | Kind | Lines | Purpose |
|---|---|---:|---|
| `CrybtScoring` | static | 383 | Pure scoring — no `UnityEngine` |
| `CrybtCombat` | static | 73 | Attack power / damage |
| `CrybtRules` | ScriptableObject | 142 | Crybt tuning values |
| `BoonShop` | class | 170 | Boon purchase state |
| `BoonDefinition` | ScriptableObject | 87 | One boon |
| `BoonId` | enum | 20 | Boon ids 0–5 |
| `CardDefinition` | ScriptableObject | 125 | One card |
| `CardValue` | struct | 44 | Rank/suit/pip |
| `CardConstants` | static | 35 | Deck shape |
| `EncounterHud` | MonoBehaviour | 208 | All 15 Crybt widgets |
| `EncounterView` | struct | 86 | HUD render payload |
| `EncounterStage` | enum | 27 | Encounter state machine |
| `ICardClickHandler` | interface | 21 | Card → manager callback |
| `Easing` | static | 51 | `QuadInOut` — `Card`, `CrybtManager`, `CaveIntro` |
| `GameLog` | static | 67 | `[Conditional]` logging |
| `UiVisibilityExtensions` | static | 67 | `SetVisible` — used by `EncounterHud` |
| `SaveSystem` | class | 41 | Save/load (frozen — overworld only) |
| `SaveManagerScript` | class | 127 | Save DTO; **field names are JSON keys on players' disks** (frozen) |
| `Camera1Script` | MonoBehaviour | 26 | Used by `PixelatedCamera` (frozen) |

> An earlier pass added a further ~11 files here — `PlayerSaveData`,
> `PlayerInterfaces`, `QuestFlag`, `CharacterIds`, `PlayerProgressLayout`,
> `IPlayerTrigger`, `SoundSettings`, `VillageCatalogue`, `Tags`, `SceneNames`,
> `AnimatorParams`. All were reverted when the branch narrowed to Crybt scope
> (§0). They exist on `backup/refactor-solid-full`, not here. Do not reference
> them; they will not compile.

---

## 7. Dead code

Nothing in any shipping scene or prefab reaches these.

| File | Lines | Why it is dead |
|---|---:|---|
| `Scripts/AudioManager.cs` | 189 | Only in `Scene1.unity`, **which is not in build settings** |
| `Scripts/AnimatorController.cs` | 26 | Zero references anywhere |
| `Scripts/IngredientsManagerScript.cs` | 38 | Zero references — superseded by `PlayerController` inventory |
| `Scripts/InventoryManager.cs` | 13 | Zero references — body is just `DontDestroyOnLoad` |
| `Scripts/LoadInventory.cs` | 8 | Zero references — empty class body |
| `CrybtManager/Data/DeckDefinition.cs` | 38 | Added in this refactor, nothing references it yet |

`AnimatorController` is also a name collision with `UnityEditor.Animations.AnimatorController`,
which makes it look referenced in a text search. It is not.

**This changes two pending tasks.** `AudioManager` was refactored and given a
migration tool (`Tools > Audio > Migrate AudioManager Lines`) — that work
targets a script that cannot run in a build. Either add `Scene1` to build
settings or skip that migration step; do not run it expecting a visible change.

### Editor-only tools (not shipped)

| File | Lines | Menu |
|---|---:|---|
| `Core/Editor/DefaultDataAssetGenerator.cs` | 181 | `Tools > Generate Default Data Assets` |
| `CrybtManager/Editor/EncounterHudMigration.cs` | 192 | `Tools > Crybt > Migrate HUD References` |
| `CrybtManager/Editor/CardDefinitionGenerator.cs` | 245 | `Tools > Crybt > Generate Card Definitions From Prefabs` |
Delete all three once the migrations have been run and the Crybt scene saved.

---

## 8. Authored but inert

These have instances in shipping scenes but never actually run. Distinguishing
them matters, because a disabled **GameObject** and a disabled **component**
fail differently:

- A disabled GameObject runs nothing, but `SetActive(true)` from any script
  brings the whole thing to life.
- A disabled component still **responds to UnityEvent calls** (`Button.onClick`
  reaches a disabled script's public method), but its `Start`/`Update`/
  `OnTriggerEnter` never fire and `SetActive` will not help.

| Script | State | Verdict |
|---|---|---|
| `OutlineScript` ×14 | GameObject on, **component disabled** | **Inert.** Its entire job is in `Start()`, which never runs on a disabled component. No script and no UnityEvent re-enables it. The outline effect is authored and switched off across all five scenes. |
| `IconManagerScript` ×1 | **GameObject off**, component enabled | **Inert.** No script, no UnityEvent and no parent references its GameObject, so nothing can switch it on. Its `Update` drives the Pine/Rum/Fly HUD icons and the per-village coin panels — none of that is running. |
| `LoadBarScene` ×12 | GameObject on, **component disabled** | **Live, but only one of them.** Exactly one `Button.onClick` in `3DEnviroment` calls `LoadBar()`; a disabled component still answers that. The other 11 instances are unreachable. |
| `CheckifAlreadyseen` | 1 of 2 active | Fine — the active one self-starts in `Start()`. |
| `DialogManagerScript` ×45, `ObjectEnder` ×9, `SetTimeline` ×8, `ShowDrinkThenHideDrink` ×10, `ConfirmationMenuManager` ×6, `nextPageInfo` ×58 | GameObject off | Fine — each has a live referrer that switches it on. |

The two worth a decision are `OutlineScript` and `IconManagerScript`. Both look
like features that were built and then quietly turned off. Neither is a
compile-time problem; both are either dead weight to delete or a regression to
fix, and only you can say which.

---

## 9. Editor wiring still outstanding

The Crybt refactor introduced ScriptableObjects and a HUD component. The **code**
is live (§6) but the **assets and references do not exist yet**, because scene
and prefab YAML must not be edited outside the Editor.

All of these are inside the Crybt scope (§0).

**None of it is required to play.** Every reference now resolves at runtime if
the Inspector is empty:

- `CrybtRules` and the boons load from `Assets/Resources/Crybt/`.
- `EncounterHud` is built from the legacy widget fields on first frame.

Doing the wiring properly is still worth it — it makes the values editable in
the Inspector and lets the fallback code be deleted:

1. Select `CrybtManager` in the `Crybt` scene → assign `Rules` = `CrybtRules`,
   and `Boon Shop > Catalogue` = the six boon assets from
   `Assets/Resources/Crybt/Boons/`.
2. `Tools > Crybt > Migrate HUD References` → **save the scene.**
3. *(Optional)* `Tools > Crybt > Generate Card Definitions From Prefabs`. Fix
   `1_Spades Variant.prefab` first — see §8.
4. Then delete the legacy `#pragma warning disable 0649, 0169` block in
   `CrybtManager`, the runtime fallbacks, and the three editor tools.

`Tools > Generate Default Data Assets` still exists, but the assets it creates
are already committed under `Assets/Resources/Crybt/`, so it is now only useful
for regenerating them from scratch.

Steps that existed before the scope narrowed — assigning `VillageCatalogue` on
`ProgressTracker`/`CollectableIconManager`, assigning `SoundSettings` on the
audio scripts, and the AudioManager line migration — **no longer apply.** Those
ScriptableObjects and that tool were reverted (§0); the components are back to
their original fields and need nothing.

---

## 10. Notes for AI

Read this section before changing anything in this repo.

**Scope comes first.** §0 defines what may be edited: the Crybt layer, four
`Core/` files, `csc.rsp`, the `Crybt` scene, and `docs/`. Everything else is
frozen. If a request would touch a frozen file, say so and ask before doing it.
Sections §5–§8 describe the frozen code so you can *understand* it — they are
not a licence to change it.

**Scope of the live game.** 107 files. If a file is not in §5 or §6, changing it
has no effect on the shipped game. Check before spending effort.

**Reachability is a separate question from liveness — always check both.** A
file can be attached to an active GameObject, in a scene that ships, and still
never execute, because four of the seven shipped scenes have no inbound edge
(§4). Before claiming a change is observable in play, confirm the scene is one
of `StoryBook`, `3DEnviroment` or `Crybt`. A bug fixed in `InBar*` or
`NightRecap` fixes nothing a player can see today.

**Do not "fix" the tavern routing on your own initiative.** The Button in
`3DEnviroment` pointing at the Crybt instead of `InBar` is a deliberate-looking
change that predates this refactor (it is already `"Crybt"` at `HEAD`). It is a
design decision, not a defect. Raise it; do not silently re-point it.

**Hard rules, carried from the architecture review:**

- **Never edit a `.unity` or `.prefab` file directly.** Scene changes are either
  described for a human to perform, or done through a reviewable editor script.
- **Never put mutable state in a static class.**
- **Never add a new field to `PlayerController`** — it is already 2056 lines.
- **Never rename a serialized field** without `[FormerlySerializedAs]`. Unity
  YAML is name-keyed: adding and reordering fields is safe, renaming loses data.
- **Never rename a field in `SaveManagerScript`** — those names are literal JSON
  keys in save files on players' machines, including the inconsistent plural
  `VillageOneCollectables`.
- **Never rename a `QuestFlag` member** — flags persist by name; a rename
  orphans the saved value.
- **Never delete a `Debug.Log` that documents a game rule** — route it through
  `GameLog` instead.
- **Never combine a behavioural change with a mechanical sweep in one commit.**
- **Never add a UPM/NuGet dependency** (DOTween, UniTask, Zenject, Odin)
  without asking.
- **Do not migrate input.** The legacy Input Manager is a deliberate choice.
- **Do not introduce UI Toolkit**, and do not replace Ink or Cinemachine.

**Version ceiling.** Unity 2020.3.19f1 means **C# 8** — no records, no `init`,
no target-typed `new`. These APIs do **not** exist: `UnityEngine.Pool.ObjectPool`,
`FindObjectsByType`, `FindAnyObjectByType`, `Awaitable`, runtime UI Toolkit.
Available and preferred: `TryGetComponent`, `CompareTag`, `[SerializeReference]`,
`[CreateAssetMenu]`, switch expressions.

**ScriptableObjects here are a runtime channel, not persistence.** In the Editor
they keep Play Mode edits; in a build they reset every launch. `PlayerController`
and `SaveSystem` remain the only owners of saved state.

**Definition of done.** Compiles in 2020.3.19f1 with no new warnings; no
serialized field renamed or retyped without a migration note; behaviour
unchanged unless the commit body says otherwise; no incidental reformatting.

**Where to start reading.** `PlayerController` (2056) and `CrybtManager` (1987)
are 30% of the game's code between them. `CrybtScoring`, `CrybtCombat`,
`SaveSystem` and `PlayerSaveData` are the pure, testable parts — change those
with confidence. Everything in §4 with a 1/1 instance count is a singleton
manager wired by hand in one scene; changing its public surface means re-wiring
the Inspector.

---

## 11. Shared code added on `feature/audio`

Two new folders under `Core/`, both additive. Nothing that already plays audio changed,
and nothing here is wired into a scene yet.

### `Core/Audio/**`

| File | Lines | What |
|---|---:|---|
| `SoundPlayer.cs` | ~950 | The component. Plays a sound against any AudioSource, one-shot or looping, with optional pitch and volume variation and mixer routing the caller can override at runtime. |
| `SoundSettings.cs` | ~250 | Everything that describes a sound. Held inline by `SoundPlayer` or inside a `SoundDefinition`. |
| `SoundDefinition.cs` | ~50 | ScriptableObject preset. `Assets > Create > Audio > Sound`. |
| `SoundShaper.cs` | ~160 | Picks the next pitch or volume: none, random, or stepping with loop/ping-pong wrap. |
| `PitchScale.cs` | ~160 | Scale step to pitch, `2^(n/12)`. Pure — no Unity state, no component. |
| `MixerVolume.cs` | ~160 | Linear volume to decibels and back, plus a guarded `SetFloat`. |
| `SoundEnums.cs` | ~130 | The enums. |

### `Core/Pooling/**`

| File | Lines | What |
|---|---:|---|
| `ObjectPool.cs` | ~390 | String-keyed pool. `Get<T>(key)`, `Release(key, item)`, `Prewarm`, `Clear`, counts. General purpose — not audio-specific. |
| `PoolHost.cs` | ~55 | MonoBehaviour that owns one `ObjectPool` and parents pooled GameObjects. |
| `PoolKeys.cs` | ~30 | The keys, as `const string`. |

### Invariants this layout protects

- **Pitch is per-voice, not per-source.** Setting pitch on a shared AudioSource re-pitches
  every one-shot still ringing out on it. Each overlapping shot borrows its own voice from
  the pool, configured from the template source — mixer group, spatial blend, distances and
  rolloff copied across, so a pooled 3D sound stays where it was.
- **No mutable state in a static class.** `ObjectPool` is an ordinary object that `PoolHost`
  owns. `PitchScale`, `MixerVolume`, `PoolKeys` are static and hold nothing that can change.
- **No progression state on a ScriptableObject.** A `SoundDefinition` can be shared by twenty
  components, so a cursor stored on the asset would be shared with them. `SoundShaper` is
  pure; `SoundPlayer` owns the cursor.
- **Nothing is pulled.** No `FindObjectOfType`, no singleton. The AudioSource and the
  `PoolHost` are assigned in the Inspector or handed in at the call site, the same way
  `Card` is given its `ICardClickHandler`.

### Enum ordering

`SoundVariationMode`, `SoundStepWrap`, `SoundPitchUnit`, `MusicalScale`, `SoundClipPickMode`,
`SoundPlaybackMode` and `SoundMixerRouting` are serialized as integers, exactly like
`EncounterStage`. **Do not reorder them.** Add new members at the end.

### The Crybt audio layer

Added on top of `Core/Audio`, under `Assets/Scripts/CrybtManager/Audio/`.

| File | What |
|---|---|
| `CrybtSound.cs` | 23 named moments — card deal, monster reveal, score tick, action refused. Serialized as ints inside the table; **do not reorder**. |
| `CrybtSoundTable.cs` | ScriptableObject mapping `CrybtSound` to `SoundDefinition`. Shipped copy at `Assets/Resources/Crybt/CrybtSounds.asset`. |
| `CrybtAudio.cs` | The Crybt's one audio entry point. Builds a `SoundPlayer` per sound at runtime, all sharing one `PoolHost`. |
| `Editor/CrybtAudioGenerator.cs` | `Tools > Crybt > Generate Crybt Sound Assets`. Builds the definitions and the table. Safe to re-run; existing assets kept. |
| `Editor/CrybtSoundEntryDrawer.cs` | Draws a table row as "Card Hover" with its asset on the same line, instead of "Element 0". **Permanent** — unlike the rest of that folder, it is not a one-shot migration to be deleted. |

Unity's built-in "use a field for the array element label" feature only reads a serialized
**string** field, so it does not apply to an enum-keyed row — hence the drawer rather than a
field rename, which would also have orphaned the enum value in every authored row.

`CrybtManager` gained one serialized `CrybtAudio` field and ~30 one-line `PlaySound(...)`
calls. Every one goes through a private null-checked helper, so removing `CrybtAudio`
from the scene silences the game without touching the state machine. `Card` gained
`BindAudio(CrybtAudio)` and plays a hover tick — pushed by the manager, so none of the
53 card prefabs changed.

The score tick is the one sound that climbs: major pentatonic, eleven steps, reset at the
start of every encounter, so a scoring streak reads as a run rather than the same blip
five times.

### Still outstanding

- **Run `Tools > Crybt > Generate Crybt Sound Assets` once.** Until it is run there is no
  sound table, and `CrybtAudio` warns and stays silent.
- **Add a `CrybtAudio` component** to the object carrying `CrybtManager` in `Crybt.unity`,
  then save the scene. That is the only scene edit needed — everything else self-wires.
- `Crybt.unity` still has zero authored AudioSources; every voice is pooled at runtime.
- Nothing in the overworld has been re-routed to the new `Music`/`SFX`/`UI` mixer groups.
