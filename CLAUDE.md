# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Read this first

`docs/code-index.md` is the authoritative map of this repo: what actually runs, what
is frozen, what is unreachable, and what Editor wiring is still outstanding. It was
derived from the asset graph, not from file names. **Read it before changing anything**,
especially §0 (scope) and §10 (notes for AI). This file is the short version.

## What this project is

A Unity 2020.3.19f1 LTS game (`Zeblock and Rana`, part of *The Rusty Longsword*), URP,
legacy Input Manager, Ink for dialogue. A new card game — **the Crybt** — is being built
on top of an older 3D overworld adventure. ~283 `.cs` files in `Assets/`, of which **63%
is vendor code** (Ink, BOXOPHOBIC, LowPolyWater) and only ~96 files are live game code.

## Scope — the most important rule

**The Crybt is the only part under active development. Everything else is frozen.**

Editable:

- `Assets/Scripts/CrybtManager/**`
- `Assets/Scripts/Core/Math/Easing.cs`, `Core/Logging/GameLog.cs`,
  `Core/Extensions/UiVisibilityExtensions.cs`, `Core/Editor/DefaultDataAssetGenerator.cs`
- `Assets/Resources/Crybt/**` (the `CrybtRules` and six `BoonDefinition` assets)
- `Assets/Scenes/Crybt.unity` — **via the Unity Editor only, never by editing YAML**
- `Assets/csc.rsp`, `docs/**`

Frozen: everything else — `PlayerController`, `SaveSystem`/`SaveManagerScript`, all
dialogue and Ink code, coins/collectables, movement, the night loop, taverns, audio,
and every scene other than `Crybt`. Read frozen files freely to understand the game;
do not edit, refactor, tidy, or bug-fix them. If Crybt work genuinely needs a frozen
file changed, stop and ask.

An earlier pass refactored ~45 frozen files and was reverted; it lives on
`backup/refactor-solid-full`. Do not reapply it, and do not reference the types it
added (`PlayerSaveData`, `QuestFlag`, `SceneNames`, `Tags`, …) — they will not compile
on this branch.

## Commands

There is no test suite, no linter, and no build script in this repo. Everything runs
through the Editor.

Open the project (Unity 2020.3.19f1 — do not open it in a newer Editor, that would
upgrade the project irreversibly):

```bash
"C:/Program Files/Unity/Hub/Editor/2020.3.19f1/Editor/Unity.exe" -projectPath "D:/repo/The Rusty Longsword 2/The Rusty Longsword/Zeblock and Rana"
```

Headless compile check after editing C# (this is the closest thing to a build/test
gate; it imports assets and compiles, then exits — it fails if the Editor is already
holding the project lock):

```bash
"C:/Program Files/Unity/Hub/Editor/2020.3.19f1/Editor/Unity.exe" -batchmode -quit -logFile - -projectPath "D:/repo/The Rusty Longsword 2/The Rusty Longsword/Zeblock and Rana"
```

Editor menu tools (Editor-only, deleted once their migration has been run — see
`docs/code-index.md` §9):

- `Tools > Generate Default Data Assets` — recreates `CrybtRules` + the six boons
- `Tools > Crybt > Migrate HUD References` — copies legacy `CrybtManager` UI fields onto `EncounterHud`
- `Tools > Crybt > Generate Card Definitions From Prefabs`

## Runtime shape — what can actually be reached

Seven scenes ship; **four of them cannot be entered at runtime**:

```
StoryBook (build index 0, entry) → 3DEnviroment (overworld) → [one Button] → Crybt (terminal)
UNREACHABLE: InBar, InBar_Eastwood, InBar_Boenaha, NightRecap
```

Consequences worth keeping in mind:

- The overworld is not optional scaffolding — it is the only corridor into the Crybt.
- The Crybt has **no exit and writes nothing**: it never touches `SaveSystem`,
  `SceneManager` or `PlayerController`.
- A "fix" in a tavern or `NightRecap` scene fixes nothing a player can see today.
- The overworld Button points at `Crybt` instead of `InBar` deliberately, and predates
  this branch. Raise it; do not silently re-point it.

`Resources.Load` is used in exactly one place — `CrybtManager` loading the Crybt tuning
assets when the Inspector fields are empty. The asset GUID graph is otherwise complete,
so "is this referenced by a scene or prefab?" is a reliable liveness test.

## Crybt architecture

`CrybtManager` (~2,400 lines) coordinates the encounter state machine and owns nothing
else. The split it enforces:

| Layer | Files | Rule |
|---|---|---|
| Pure rules | `Rules/CrybtScoring.cs` (no `UnityEngine` at all), `Rules/CrybtCombat.cs` | Static, testable, no Unity types |
| Data | `Data/CrybtRules.cs`, `BoonDefinition`, `CardDefinition`, `BoonId` | ScriptableObjects under `Assets/Resources/Crybt/` |
| Runtime state | `Runtime/BoonShop.cs`, `EncounterStage.cs`, `ICardClickHandler.cs` | |
| View | `View/EncounterHud.cs`, `View/EncounterView.cs` | Owns every `Button` and `Text` |
| Scene objects | `Card.cs` (53 instances), `CaveIntro`, `ShakeOnEnable`, `SimpleTextShadow` | |

Two invariants this layout exists to protect:

- **One render path.** The rules engine builds an immutable `EncounterView` struct and
  calls `hud.Render(view)` exactly once per refresh. `EncounterView` holds no Unity
  types and no back-reference, so the HUD cannot reach into the rules engine and
  `CrybtManager` never touches a widget. Every control gets exactly one assignment in
  `Render`, so "a button lingered into the wrong stage" is not expressible.
- **Cards are pushed, not pulled.** `Card` never calls `FindObjectOfType`; the manager
  calls `Card.Bind(ICardClickHandler)` (and `Bind(CardDefinition)` for its face data).

`EncounterStage` values are serialized as integers in the Crybt scene — **do not reorder
the enum.**

`ScriptableObject`s here are a runtime configuration channel, not persistence. In the
Editor they keep Play Mode edits; in a build they reset every launch.

## Conventions and hard rules

- **Never edit a `.unity` or `.prefab` file directly.** Scene changes are either
  described for a human to perform in the Editor, or made through a reviewable editor
  script.
- **Never rename a serialized field** without `[FormerlySerializedAs]` — Unity YAML is
  name-keyed. Adding and reordering fields is safe; renaming loses data.
- **Never rename a field in `SaveManagerScript`** — those names are literal JSON keys in
  save files on players' machines, including the inconsistent plural `VillageOneCollectables`.
- **Never put mutable state in a static class.**
- **Never add a field to `PlayerController`** (1,497 lines, frozen anyway).
- `[SerializeField] private` is the house pattern; `Assets/csc.rsp` carries
  `-nowarn:0649` to make it usable. **`csc.rsp` must contain compiler options only** —
  Unity 2020.3 splits it on whitespace and passes every token to the compiler, so a `#`
  comment containing a `-` produces `CS2007` for *every* assembly and blocks builds.
  Rationale goes in `docs/code-index.md`, not in the file.
- Rules-documenting logs go through `GameLog.Info` (`[Conditional]`, stripped from
  release builds along with its string concatenation), never `Debug.Log`. Warnings and
  errors stay unconditional.
- Never combine a behavioural change with a mechanical sweep in one commit.
- Never add a UPM/NuGet dependency (DOTween, UniTask, Zenject, Odin) without asking.
  Do not migrate input, do not introduce UI Toolkit, do not replace Ink or Cinemachine.

**Version ceiling: C# 8.** No records, no `init`, no target-typed `new`. These do not
exist in 2020.3: `UnityEngine.Pool.ObjectPool`, `FindObjectsByType`,
`FindAnyObjectByType`, `Awaitable`, runtime UI Toolkit. Available and preferred:
`TryGetComponent`, `CompareTag`, `[SerializeReference]`, `[CreateAssetMenu]`, switch
expressions.

**Definition of done:** compiles in 2020.3.19f1 with no new warnings; no serialized
field renamed or retyped without a migration note; behaviour unchanged unless the
commit body says otherwise; no incidental reformatting.
