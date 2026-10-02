# Assistance Mission Mod — Handover Notes v1

For the next dev picking this up. Read this first.

## 0. Fast-start checklist (do these before anything else)
- [ ] Working copy: real git clone of `https://github.com/Harrod200/Assistance`, branch `cap-and-breakdown`, at commit `a303429`
- [ ] Build: run `dotnet build` from the workspace root or build via Visual Studio 2022+ — must be green before you touch anything
  - **Note:** Build currently fails due to missing reference DLLs (HarmonyLib, Terra Invicta game assemblies). These are expected to be in `/scratch/u10000/refs/` and `/tmp/umod/` per HintPaths in Assistance.csproj. The project uses FrameworkPathOverride for .NET Framework 4.8 compatibility.
- [ ] Read the README.md (141 lines) — covers features, installation, configuration, code organization, and version history
- [ ] Consult any available reference docs for Terra Invicta decompiled source, HarmonyLib API, and vanilla game state structures

## 1. Session summary
**Initial handover creation — no prior sessions to document changes from.**

### Decisions made
- This handover was created at commit `a303429` (HEAD on `cap-and-breakdown`) to establish a baseline for future work
- Branch `cap-and-breakdown` contains the most recent feature: configurable assist bonus cap (default 25) and restored mission breakdown UI patches
- All documentation assumes that reference DLLs will be resolved via environment setup (FrameworkPathOverride and HintPath corrections)

### Known issues
- Build fails due to missing reference assemblies (environment-dependent, not a code defect)

## 2. Environment
### Build command and framework
- **Target Framework:** .NET Framework 4.8
- **SDK:** Modern .NET SDK with FrameworkPathOverride pointing to `..\RefAsm\.NETFramework\v4.8\`
- **Build command:** `dotnet build Assistance.sln` or use Visual Studio 2022+ Build menu
- **Output:** Debug: `Assistance\bin\Debug\Assistance.dll` | Release: `Assistance\bin\Release\Assistance.dll`

### Reference DLLs location
Per Assistance.csproj HintPaths:
- **HarmonyLib 2.2:** `/scratch/u10000/refs/0Harmony22.dll` (set to not copy locally)
- **Terra Invicta Game Assemblies:** `/tmp/umod/` (UnityEngine.CoreModule.dll, game DLLs)
- **HintPaths may need corrections** if running on different machine or VM. Original HintPaths point to developer's paths; adjust to your environment.

### Post-build deployment
- Commented-out PostBuildEvent in Release config copies built DLL to Steam mod folder: `C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\`
- To enable: uncomment the `<PostBuildEvent>` in Assistance.csproj Release PropertyGroup

### Known environment quirks
- `/tmp/umod/` and reference paths may be VM-specific; verify DLL locations exist before attempting build
- FrameworkPathOverride is required; do not "fix" the SDK-style .csproj targeting v4.8 without owner approval

## 3. Repository layout

| Path | Purpose | Git-tracked |
|------|---------|---|
| `Assistance.sln` | Solution file | Yes |
| `Assistance/Assistance.csproj` | Project file (SDK-style net48) | Yes |
| `Assistance/*.cs` | Core mod patches and mission system | Yes |
| `Assistance/Properties/AssemblyInfo.cs` | Assembly metadata | Yes |
| `Assistance/TIMissionTemplate.en` | Localization data | Yes |
| `Assistance/Handover/` | Handover docs and templates | No (local-only) |
| `Assistance/bin/` | Build outputs | No |
| `Assistance/obj/` | Build artifacts | No |
| `.vs/` | VS cache | No |
| `README.md` | Feature overview and technical guide | Yes |
| `diff.patch` | Patch file (purpose TBD) | Yes |

## 4. Checkpoint table

| # | Deliverable | Verify | Est. credits | Status |
|---|---|---|---|---|
| C1 | Establish baseline handover (v1) | Build succeeds, all docs in place | ~3 | done (a303429) |

**Rules:**
- Each checkpoint leaves the build green and the repo pushable
- Record actual credit cost per checkpoint and recalibrate future estimates
- Size each checkpoint to fit ONE runtime window (~75 tool steps)

## 5. Design decisions in force (do not relitigate without owner sign-off)

### Amendment log
*None yet. Future amendments go here with date.*

### Active design principles
1. **Stat Sharing Model (v0.7+):** Assist missions are contested missions where one councilor aids another. The assisting councilor's attack roll generates a bonus pool; on success, the assisted councilor receives temporary stat bonuses (Loyalty, Offense, Defense, Research, Economics, Loyalty Recovery).

2. **Configurable Bonus Cap (v0.8.0+, default 25):** A hard ceiling on total stat bonuses per councilor. This prevents runaway stat stacking and maintains game balance. Configurable via UMM option.

3. **Mission Breakdown Accuracy (v0.7.3+):** Modifier snapshots are captured at mission calculation time (not post-mission) to ensure UI breakdowns reflect exact modifiers used, not post-mission reward bonuses. Implementation: `TIMissionResolution_Contested_AssistBonusPatch` caches modifiers; `TINotificationQueueState_MissionDetailBreakdownPatch` retrieves and displays cached data.

4. **Vanilla Mission Structure Compliance:** Assist mission template must match vanilla Inspire mission structure (or other similar contested mission) to avoid AI planner crashes (lesson learned in v0.3.1).

5. **HarmonyLib Patching Everywhere:** All game state modifications (bonus application, UI display, mission availability, etc.) use HarmonyLib decorators (Prefix/Postfix) rather than direct code injection. Patches are applied in `Main.cs` at mod init.

## 6. Vanilla API cheat sheet (grow this, never shrink it)

### Terra Invicta Core Types
**Note:** Exact signatures and method bodies should be verified against decompiled source. This section is a quick reference; do not assume completeness.

#### TICouncilorState
- `GetPossibleMissionList()` → `List<TIMissionTemplate>`
  - Returns list of missions available to this councilor
  - Patched by `AssistAvailabilityPatch.Postfix` to add Assist mission if target councilor exists in same faction

#### TIMissionTemplate
- Base class for mission definitions (Inspire, Assist, etc.)
- Fields: name, description, duration, contested flag, stat attributes
- Used to define Assist mission via `TIMissionTemplate_Assist.cs`

#### TIMissionState
- Runtime instance of an active mission
- Contains: template reference, participants, current state
- Patched by `TIMissionResolution_Contested_AssistBonusPatch` to apply bonus calculations

#### TIMissionResult / MissionResult
- Enum or class representing mission outcome (Success, Failure, Contested win/loss, etc.)
- Used in mission breakdown display logic

#### TIMissionModifier
- Represents a single stat modifier applied to mission (e.g., "Offense +5")
- Fields: name, base value, bonuses, description
- List captured and cached during mission resolution for accurate UI display

#### CouncilorAttribute
- Enum: Loyalty, Offense, Defense, Research, Economics, LoyaltyRecovery, etc.
- Used to specify which stat a modifier affects

#### TIGameState
- Global game state singleton; stores factions, councilors, missions, turn counter, etc.
- Used for turn tracking and cleanup of expired assist bonuses

#### Loc.T(string key)
- Localization lookup; translates string keys to in-game text
- Patched by `CouncilorMissionCanvasController_UpdateModifierListPatch.Loc_T_Postfix` to intercept and label assist modifiers

#### UnityEngine.UI.Text
- UI component for displaying text (mission breakdown, stat values, etc.)
- Referenced indirectly via HarmonyLib patching; not directly imported

### HarmonyLib API (v2.2)
- `[HarmonyPatch]` / `[HarmonyPatch(Type, Method)]` — decorator marking patch target
- `[HarmonyPrefix]` — runs before target method; can return false to skip target
- `[HarmonyPostfix]` — runs after target method; receives __result, __instance, method params via double-underscore names
- `[HarmonyTargetMethod]` — used with bare `[HarmonyPatch]` to locate target via method lookup
- `AccessTools` — utility for reflection-based method/field access (used in patches)

### Mission Calculation and Breakdown Flow
1. **Mission resolution:** `TIMissionResolution_Contested` calls attack/defense summing
2. **Modifier caching:** `TIMissionResolution_Contested_AssistBonusPatch` postfix captures modifier snapshots at this point
3. **Bonus application:** `TIMissionEffect_Assist` applies stat bonuses to assisted councilor; bonuses registered in `AssistBonusTracker`
4. **UI display:** `TINotificationQueueState_MissionDetailBreakdownPatch` retrieves cached modifiers and formats breakdown text
5. **Cache cleanup:** Auto-purge entries older than 1 minute to prevent unbounded growth

### Common pitfalls
- **Modifier list stale:** If UI breakdown shows post-mission rewards, the cache lookup failed. Check cache key construction and TTL.
- **AI crashes on new mission:** Ensure template structure matches vanilla patterns (e.g., Inspire mission). Incomplete template definitions confuse the AI planner.
- **Bonuses not applying:** Verify `MissionEffect` is registered in template and `AssistBonusTracker` is being called. Check for faction/councilor mismatch guards.
- **Localization missing:** Assist mission names/descriptions require entries in localization data (TIMissionTemplate.en or game localization system).

## 7. Known unknowns / open questions

1. **Bonus persistence across saves:** Does `AssistBonusTracker` survive save/load cycles? Verify serialization behavior.
2. **AI councilor assistance:** Do AI-controlled councilors generate Assist missions for each other? Intended or should be gated? Currently available to player-controlled only (see `IsPlayerControlled` guard clauses).
3. **Contested mission resolution order:** When multiple contested missions run in same turn, which modifiers resolve first? Can affect bonus cap interactions.
4. **Localization completeness:** Are all Assist mission strings properly localized? Only .en file present in repo; check if game auto-translates or if missing keys appear as errors.
5. **Modifier cache key uniqueness:** Is (missionState.ID, side) sufficient? What if two missions with same ID run concurrently?

## 8. Docs in this package

Read in this order:

1. `README.md` — Feature overview, installation, configuration, code organization, version history
2. `Handover v1.md` (this file) — Baseline environment, design decisions, API cheat sheet
3. `Efficiency Instructions for Future Instances.md` — Golden rules for rapid onboarding and cache-avoiding mistakes
4. `Handover Template.md` — Template structure (reference only; this doc instantiates it)

## 9. Local-only files (excluded from git, present in this folder)

| Item | Size | Why Excluded | Needed For |
|------|------|---|---|
| `.vs/` (Visual Studio cache) | ~50 MB | Build artifacts and IDE state | Development convenience; regenerated on open |
| `Assistance/.vs/` | ~30 MB | Project-level IDE cache | Visual Studio debugging and IntelliSense |
| `Handover/` (this folder) | ~10 KB | Transient developer notes | Handover documentation; not part of mod build |

---

## Quick navigation
- **Build issues?** Check section 2 (Environment) — likely missing reference DLLs or FrameworkPathOverride path.
- **What changed?** Section 1 (Session summary) — currently empty (v1 baseline).
- **How is code organized?** Section 3 (Repository layout) and README.md.
- **API question?** Section 6 (Vanilla API cheat sheet) or search decompiled source.
- **Design question?** Section 5 (Design decisions) — escalate to owner if you need to reverse a decision.
- **Stuck on something?** Section 7 (Known unknowns) — someone already asked.

---

**Handover created at:** 2026-02-10 (session baseline v1)  
**Branch:** `cap-and-breakdown` (commit a303429)  
**Next developer:** Please bump the version number and timestamp this section when you finish your session and prepare v2.
