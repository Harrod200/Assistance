# Assistance Mission Mod — Handover Notes v3

For the next dev picking this up. Read this first.

## 0. Fast-start checklist (do these before anything else)
- [ ] Working copy: real git clone of `https://github.com/Harrod200/Assistance`, branch `cap-and-breakdown`, at latest commit
- [ ] Build: run `dotnet build` from the workspace root or build via Visual Studio 2022+ — must be green before you touch anything
  - ✅ References are now local in `ref-dlls/` — no environment-dependent paths
- [ ] Read `Handover/Early Injection Solution - Summary.md` (5 min) — the most important recent fix
- [ ] Read the checkpoint table (section 4). Claim the next checkpoint.

## 1. Session summary

**v0.8.3: Fixed NullReferenceException crash during save/load via early template resolution**

### What changed since v2

#### Bug Fixes (Critical)
1. **NullReferenceException in GetCurrentMissionIcon()** (v0.8.3)
   - **Issue:** Game crashed when loading saves with active Assist missions
   - **Root Cause:** Mission template references could be null after deserialization
   - **Solution:** Two-layer fix:
	 - *Primary (Early Injection):* `TIMissionState_PostGlobalGameStateCreateInit_2_Patch.cs` resolves null templates during initialization
	 - *Secondary (Defensive):* `TICouncilorState_GetCurrentMissionIconPatch.cs` guards UI against edge cases
   - **Impact:** Saves with Assist missions now load without crashing

#### Architectural Improvements
1. **Early Injection Pattern:** Patch `PostGlobalGameStateCreateInit_2()` to fix template resolution before UI rendering
2. **Defense-in-Depth:** Multiple defensive layers (session cleanup, template resolution, UI guards)
3. **Better Debugging:** Comprehensive logging in all critical patches

### Decisions made
- **Prefer early injection over defensive patching:** Fixing root causes during initialization is better than handling failures in UI code
- **Keep defensive layers:** Two layers of defense prevent edge-case crashes
- **Log resolution events:** Debug logging helps diagnose similar issues in future
- **Version bump:** Patch version (0.8.2 → 0.8.3) for bug fixes

### Bugs found and fixed
- **NullReferenceException on load:** Missions with null templates caused crashes in Finder list rendering
  - **File:** `TIMissionState_PostGlobalGameStateCreateInit_2_Patch.cs` (early injection)
  - **Commit message:** "Fix: Resolve null mission templates during initialization (v0.8.3)"

### Known issues
- None currently identified

## 2. Environment

### Build command and framework
- **Target Framework:** .NET Framework 4.8
- **SDK:** Modern .NET SDK with `FrameworkPathOverride` pointing to `..\RefAsm\.NETFramework\v4.8\`
- **Build command:** `dotnet build Assistance.sln` or Visual Studio 2022+ Build menu
- **Output:** Debug: `Assistance\bin\Debug\Assistance.dll` | Release: `Assistance\bin\Release\Assistance.dll`

### Reference DLLs (Local)
**Location:** `ref-dlls/` (committed, portable across machines)

- **`ref-dlls/TerraInvicta/`**
  - `Assembly-CSharp.dll` (7.2 MB) — Game state, missions, councilors
  - `UnityEngine.CoreModule.dll` (1.1 MB)
  - `UnityEngine.IMGUIModule.dll` (171 KB)
  - `UnityEngine.UI.dll` (233 KB)

- **`ref-dlls/UMM/`**
  - `0Harmony22.dll` (909 KB) — Harmony v2.2 for patching
  - `0Harmony.dll` (2.2 MB) — Harmony base library
  - `UnityModManager.dll` (204 KB)

- **`ref-dlls/TI Decompiled/`**
  - Full decompiled source code for reference
  - Class & Method Reference (2.9 MB) — authoritative API docs

### Build verification
- Run `dotnet build` — all references resolve from local `ref-dlls/`
- ✅ No missing assembly errors
- ✅ No environment-specific path issues

## 3. Repository layout

| Path | Purpose | Git-tracked |
|------|---------|---|
| `Assistance.sln` | Solution file | Yes |
| `Assistance/Assistance.csproj` | Project file (SDK-style net48) | Yes |
| `Assistance/*.cs` | Core mod patches and mission system | Yes |
| `Assistance/ModInfo.json` | UMM mod metadata (version 0.8.3) | Yes |
| `Assistance/Properties/AssemblyInfo.cs` | Assembly version (0.8.3.0) | Yes |
| `Assistance/TIMissionTemplate.en` | Localization data | Yes |
| `Assistance/Handover/` | Handover docs and technical guides | No |
| `Assistance/bin/` | Build outputs | No |
| `Assistance/obj/` | Build artifacts | No |
| `.vs/` | VS cache | No |
| `ref-dlls/` | Terra Invicta & UMM assemblies | No (git-ignored) |
| `README.md` | Feature overview and technical guide | Yes |
| `.gitignore` | Excludes ref-dlls, obj, bin, .vs | Yes |

## 4. Checkpoint table

| # | Deliverable | Verify | Est. credits | Status |
|---|---|---|---|---|
| C1 | Establish baseline handover (v1) + ref-dlls setup | Build succeeds, references resolve | ~5 | done |
| C2 | Fix double-counting bug in advising (v0.8.2) | Build succeeds, no regressions | ~8 | done |
| C3 | Fix NullReferenceException on load (v0.8.3) | Save loads without crash, logs show resolution | ~10 | done |

**Next checkpoint:** C4 - [open for future work]

**Rules:**
- Each checkpoint leaves the build green and repo pushable
- Record actual credit cost and recalibrate estimates

## 5. Design decisions in force (do not relitigate without owner sign-off)

### Amendment log
- **2026-02-10 (v2):** Clarified `assistPercentage` applies to advising once, not redundantly
- **2026-02-11 (v3):** Early injection pattern for template resolution preferred over defensive patching

### Active design principles

1. **Stat Sharing Model (v0.7+):** Assist missions are contested missions where one councilor aids another. Success generates temporary stat bonuses for the assisted councilor.

2. **Efficiency Loss Simulation (v0.8.2+):** `assistPercentage` scales both contested mission bonuses AND advising bonuses, simulating skill overlap. Applied once in each context, not redundantly.

3. **Configurable Bonus Cap (v0.8.0+, default 25):** Hard ceiling on total stat bonuses per councilor. Prevents stat stacking and maintains balance.

4. **Mission Breakdown Accuracy (v0.7.3+):** Modifier snapshots captured at calculation time, not post-mission, ensuring UI reflects exact modifiers used.

5. **Early Injection Over Defensive Patching (v0.8.3+):** Fix root causes during initialization rather than defending against failures in UI code. Provides better debugging and scalability.

6. **HarmonyLib Patching:** All modifications use HarmonyLib decorators (Prefix/Postfix), not direct injection. Applied in `Main.cs` at mod init.

## 6. Critical patches and their purposes

### Mission Template Resolution (v0.8.3)
**Files:** 
- `TIMissionState_PostGlobalGameStateCreateInit_2_Patch.cs` (primary fix)
- `TICouncilorState_GetCurrentMissionIconPatch.cs` (defensive layer)

**Why needed:** Deserialized missions can have null templates if template lookup happens before registration. Early injection fixes this before UI rendering.

**Timing:** PostGlobalGameStateCreateInit_2 runs after deserialization, before UI rendering.

### Session State Cleanup (v0.8.3)
**File:** `TIGameState_LoadPatch.cs` (Prefix on GameControl.Initialize)

**Why needed:** AssistBonusTracker is session-scoped. Must clear old data before new game loads.

**Timing:** Prefix runs BEFORE vanilla init, ensuring clean state during deserialization.

### Double-Counting Prevention (v0.8.2)
**File:** `TICouncilorState_AdvisingBonusPatch.cs`

**Why needed:** Assist bonus was counted twice in advising calculations, inflating nation bonuses 2x.

**How it works:** Applies assist bonus ONCE, scaled by `assistPercentage`, to advising formula.

### Bonus Application After Mission Success
**File:** `TIMissionEffect_Assist.cs`

**What it does:** Records assist bonuses after successful mission. Bonuses are in-memory, session-scoped, not persisted to saves.

### Contested Mission Resolution
**File:** `TIMissionResolution_Contested_AssistBonusPatch.cs`

**What it does:** Applies stat-specific assist bonuses during contested mission checks. Each stat gets its own bonus, not pooled.

## 7. Vanilla API cheat sheet (grow this, never shrink it)

### Terra Invicta Core Types

#### TICouncilorState
- `activeMission` → `TIMissionState` (current mission, can be null)
  - **Important:** `activeMission.missionTemplate` can be null after deserialization
  - Use defensive checks: `if (activeMission?.missionTemplate != null)`
- `GetAttribute(attribute, ...)` → `int` (get stat with modifiers)
- `AdvisingBonus(attribute)` → `float` (base advising power)
  - **Patched by:** `TICouncilorState_AdvisingBonusPatch` to add assist bonuses
- `GetPossibleMissionList()` → `List<TIMissionTemplate>` (available missions)
  - **Patched by:** `AssistAvailabilityPatch` to inject Assist mission

#### TIMissionState
- `missionTemplate` → `TIMissionTemplate` (read-only property, has `_missionTemplate` backing field)
  - **Can be null during initialization** — use defensive patches
  - **Resolved by:** `TIMissionState_PostGlobalGameStateCreateInit_2_Patch` during `PostGlobalGameStateCreateInit_2()`
- `templateName` → `string` (mission template name reference)
- `PostGlobalGameStateCreateInit_2()` → `void` (post-deserialization hook)
  - **Patched by:** `TIMissionState_PostGlobalGameStateCreateInit_2_Patch` to resolve null templates

#### TIMissionTemplate
- Base class for mission definitions (Inspire, Assist, etc.)
- Subclassed by `TIMissionTemplate_Assist` to define Assist mission

#### TemplateManager
- `Initialize()` → `void` (called at game startup)
  - **Patched by:** `AssistTemplateRegistration` to register Assist template
- `Find<T>(name, required)` → `T` (lookup template by name)
  - Used by early injection patch to resolve null templates

#### GameControl
- `Initialize(bool loadingSave, IScenario scenario)` → `void`
  - **Patched by:** `TIGameState_LoadPatch` (Prefix) to clear session state before init

### Deserialization Lifecycle
1. **TemplateManager.Initialize** ← Templates registered (including ours)
2. **GameControl.Initialize** ← [Prefix] Clear session state
3. **Missions deserialized** ← May have null templates if timing is wrong
4. **TIMissionState.PostGlobalGameStateCreateInit_2** ← [Patch] Resolve null templates ✅
5. **UI rendering** ← Templates are now valid
6. **GetCurrentMissionIcon** ← [Defensive patch] Extra safety check

## 8. Debugging and troubleshooting

### Enable debug logging
In mod settings, check "Enable Debug Logging". Watch for:

```
[TIMissionState_PostGlobalGameStateCreateInit_2] Successfully resolved template 'Assist' for mission [id]
```
This indicates early injection is working.

```
[TICouncilorState_GetCurrentMissionIcon] Null missionTemplate for 'CouncilorName' - returning empty string
```
This indicates an edge case where early injection didn't catch it (should be rare).

### Common issues and solutions

**Issue:** Game crashes loading save with Assist mission
- **Check:** Enable debug logging, look for template resolution messages
- **If resolved:** Early injection is working, defensive patch caught nothing
- **If not resolved:** Report as new bug

**Issue:** Assist mission doesn't appear in mission list
- **Check:** `AssistAvailabilityPatch` runs on `GetPossibleMissionList()`
- **Check:** Councilor faction must be player-controlled
- **Check:** Mission conditions check target is in same faction

**Issue:** Assist bonuses seem wrong
- **Check:** `assistPercentage` setting (default 50%)
- **Check:** `capBonusEnabled` setting and `maxBonusStatValue` (default 25)
- **Note:** Bonuses are session-scoped and cleared on turn transitions

---

## How to Use This Handover

1. **First time?** Read sections 0, 1, and the summary
2. **Making changes?** Reference section 6 (critical patches) and section 7 (vanilla API)
3. **Debugging?** See section 8
4. **Deep dive?** Check the Handover folder for detailed technical docs

## Key Documentation Files

| File | Purpose | Read time |
|------|---------|-----------|
| `Early Injection Solution - Summary.md` | Overview of v0.8.3 fix | 5 min |
| `Architecture - Template Resolution.md` | Visual diagrams and flow | 10 min |
| `Root Cause Analysis - Mission Template Null.md` | Why the crash happens | 10 min |
| `Fix Summary - Double Counting Bug.md` | v0.8.2 advising fix | 10 min |
| `Reference Guide.md` | How to use decompiled docs | 5 min |

---

**Ready to extend the mod?** Claim checkpoint C4 and document your work in a new handover amendment.
