# Assistance Mission Mod — Handover Notes v2 (supersedes v1)

For the next dev picking this up. Read this first.

## 0. Fast-start checklist (do these before anything else)
- [ ] Working copy: real git clone of `https://github.com/Harrod200/Assistance`, branch `cap-and-breakdown`, at commit TBD
- [ ] Build: run `dotnet build` from the workspace root or build via Visual Studio 2022+ — must be green before you touch anything
  - ✅ References now local in `ref-dlls/` — no environment-dependent paths
- [ ] Read `Docs/Terra Invicta Class & Method Reference.md` (2.9 MB — verified full-game class/method index)
- [ ] Read the checkpoint table (section 4). Claim the next one.

## 1. Session summary
**v0.8.2 patch: Fixed double-counting bug in advising bonus calculations**

### What changed since v1
1. **Bug fix:** Assist bonus was being counted twice in `AdvisingBonus()` calculations
   - Nation/hab bonuses were ~2x inflated
   - Fixed by simplifying patch to apply assist bonus once, scaled by efficiency
   - **Impact:** Game balance restored for assisted councilors

2. **Project setup:**
   - Added local `ref-dlls/` with all required Terra Invicta and UMM assemblies
   - Updated `Assistance.csproj` to use local ref paths instead of environment-dependent paths
   - Added `ref-dlls/` to `.gitignore`
   - Fixed delegate conversion ambiguity in `Main.cs`

3. **Documentation:**
   - Created `Handover/Reference Guide.md` — how to use decompiled code and reference docs
   - Created `Handover/Fix Summary - Double Counting Bug.md` — detailed bug analysis and fix

### Decisions made
- **Assist bonus in advising:** `assistPercentage` setting applies to advising to simulate skill overlap/efficiency loss, but bonus is counted ONCE (not twice)
  - Formula: `AdvisingBonus += (assistBonus * assistPercentage) / 100 / 100`
  - This respects the mod's design intent while fixing the balance bug

- **Version bump:** Patch version (0.8.1 → 0.8.2) because this is a bug fix, not a new feature

### Bugs found and fixed
- **Double-counting in advising:** Assist bonus counted twice → nation bonuses ~2x inflated
  - **Repro:** Create high-stat councilor, run assist mission, check nation bonus contribution
  - **Fix:** `TICouncilorState_AdvisingBonusPatch.cs` — removed redundant calculation
  - **Commit message:** "Fix: Double-counting bug in advising bonus calculations"

## 2. Environment
### Build command and framework
- **Target Framework:** .NET Framework 4.8
- **SDK:** Modern .NET SDK with `FrameworkPathOverride` pointing to `..\RefAsm\.NETFramework\v4.8\`
- **Build command:** `dotnet build Assistance.sln` or Visual Studio 2022+ Build menu
- **Output:** Debug: `Assistance\bin\Debug\Assistance.dll` | Release: `Assistance\bin\Release\Assistance.dll`

### Reference DLLs (NEW: now local)
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

### Build verification
- Run `dotnet build` — all references resolve from `ref-dlls/`
- ✅ No missing assembly errors
- ✅ No environment-specific path issues

## 3. Repository layout

| Path | Purpose | Git-tracked |
|------|---------|---|
| `Assistance.sln` | Solution file | Yes |
| `Assistance/Assistance.csproj` | Project file (SDK-style net48) | Yes |
| `Assistance/*.cs` | Core mod patches and mission system | Yes |
| `Assistance/ModInfo.json` | UMM mod metadata (version 0.8.2) | Yes |
| `Assistance/Properties/AssemblyInfo.cs` | Assembly version (0.8.2.0) | Yes |
| `Assistance/TIMissionTemplate.en` | Localization data | Yes |
| `Assistance/Handover/` | Handover docs and templates | No |
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
| C2 | Fix double-counting bug in advising | Build succeeds, no test regressions | ~8 | done (0.8.2) |

**Next checkpoint:** C3 - [open for future work]

**Rules:**
- Each checkpoint leaves the build green and repo pushable
- Record actual credit cost and recalibrate estimates

## 5. Design decisions in force (do not relitigate without owner sign-off)

### Amendment log
- **2026-02-10 (v2):** Clarified that `assistPercentage` applies to advising (simulates efficiency loss) but bonus is counted once, not twice

### Active design principles
1. **Stat Sharing Model (v0.7+):** Assist missions are contested missions where one councilor aids another. Success generates temporary stat bonuses for the assisted councilor.

2. **Efficiency Loss Simulation (v0.8.2+):** `assistPercentage` scales both contested mission bonuses AND advising bonuses, simulating skill overlap and efficiency loss. Applied once in each context, not redundantly.

3. **Configurable Bonus Cap (v0.8.0+, default 25):** Hard ceiling on total stat bonuses per councilor. Prevents stat stacking and maintains balance.

4. **Mission Breakdown Accuracy (v0.7.3+):** Modifier snapshots captured at calculation time, not post-mission, ensuring UI reflects exact modifiers used.

5. **HarmonyLib Patching:** All modifications use HarmonyLib decorators (Prefix/Postfix), not direct injection. Applied in `Main.cs` at mod init.

## 6. Vanilla API cheat sheet (grow this, never shrink it)

### Terra Invicta Core Types

#### TICouncilorState
- `AdvisingBonus(CouncilorAttribute)` → `float`
  - Returns base advising power for a stat (base / 100)
  - **Patched by:** `TICouncilorState_AdvisingBonusPatch` to add assist bonus
  - **Scaling:** `AdvisingBonus += (assistBonus * assistPercentage / 100) / 100`

- `GetAttribute(attribute, bContest, bSupports, bAdvise, bCommand, bRanks, bMaxes)` → `int`
  - Gets councilor's current stat value with modifiers
  - Used to fetch raw attribute for calculations

- `GetPossibleMissionList()` → `List<TIMissionTemplate>`
  - Returns available missions for this councilor
  - Patched by `AssistAvailabilityPatch` to inject Assist mission

#### TIMissionTemplate
- Base class for mission definitions (Inspire, Assist, etc.)
- **Assist version:** `TIMissionTemplate_Assist.cs` subclass

#### TIMissionState
- Runtime instance of active mission
- Patched by `TIMissionResolution_Contested_AssistBonusPatch` for bonus calculation

#### TIMissionModifier
- Single stat modifier (e.g., "Offense +5")
- Captured and cached for UI display

#### CouncilorAttribute (Enum)
- Loyalty, Offense, Defense, Research, Economics, LoyaltyRecovery (and more)
- Used to specify which stat a modifier affects

#### TIGameState
- Global game state singleton
- Used for turn tracking and bonus cleanup

### Common Vanilla Patterns
- **Advising calculation:** Sum `AdvisingBonus()` across all advisors, apply ranking/diminishing returns
- **Mission resolution:** Attack vs Defense check, then apply modifiers
- **Bonus tracking:** Use local dictionary keyed by councilor+stat, clean up as needed

### HarmonyLib API (v2.2)
- `[HarmonyPatch]` / `[HarmonyPatch(Type, Method)]` — decorator marking patch target
- `[HarmonyPostfix]` — runs after target method; receives `__result` (return value)
- `[HarmonyPrefix]` — runs before target; can skip target by returning `false`
- `AccessTools` — reflection utility for finding methods/fields

### Known Pitfalls (This Project)
- **Double-counting bonus:** Must apply assist bonus exactly once in each context (contested/advising). Previous version counted twice → fixed in 0.8.2.
- **Efficiency setting scope:** `assistPercentage` applies to BOTH contested missions (stat bonus calculation) AND advising. Not just contested.
- **Modifier cache key:** Use `(missionState.ID, side)` for accurate UI. Stale cache → wrong breakdown display.
- **Bonus cap:** Must check `Settings.ApplyCap()` before recording bonus. Prevents stat stacking exploits.

## 7. Known unknowns / open questions

1. **Bonus persistence across saves:** Does `AssistBonusTracker` survive save/load? Verify serialization.
2. **AI councilor assistance:** Do AI-controlled councilors generate Assist missions? (Currently gated to player-controlled only via `IsPlayerControlled` checks.)
3. **Contested mission order:** Multi-mission turn execution — which resolve first? Affects bonus cap interactions.
4. **Localization completeness:** Are all Assist mission strings properly translated? (Only .en file present.)
5. **Modifier cache uniqueness:** Is `(missionState.ID, side)` key sufficient for concurrent missions?

## 8. Docs in this package

Read in this order:

1. `README.md` — Feature overview, installation, configuration, version history
2. `Handover v2.md` (this file) — Current state, design decisions, API cheat sheet
3. `Efficiency Instructions for Future Instances.md` — Golden rules for rapid onboarding
4. `Reference Guide.md` — How to use decompiled code & reference docs
5. `Fix Summary - Double Counting Bug.md` — Detailed bug analysis
6. `Handover Template.md` — Template structure (reference only)

## 9. Local-only files (excluded from git, present in this folder)

| Item | Size | Why Excluded | Needed For |
|------|------|---|---|
| `ref-dlls/` | ~12 MB | Build dependencies; machine-portable | Build step (copied from game install) |
| `.vs/` | ~80 MB | IDE cache/state | Visual Studio convenience; regenerated on open |
| `Assistance/.vs/` | ~30 MB | Project IDE cache | Visual Studio debugging; regenerated on open |
| `Handover/` | ~50 KB | Developer notes (transient) | Team coordination; not part of mod distribution |

---

## What Changed in v0.8.2

### Code Changes
1. **`TICouncilorState_AdvisingBonusPatch.cs`** — Fixed double-counting bug
   - Removed redundant calculation that added assist bonus twice
   - Simplified to: `AdvisingBonus += (assistBonus * assistPercentage) / 100 / 100`
   - Improved logging to show bonus breakdown

2. **`Main.cs`** — Fixed delegate conversion ambiguity
   - Removed explicit `new System.Func<>()` / `new System.Action<>()` constructors
   - Now uses implicit delegate conversion

3. **`Assistance.csproj`** — Migrated to local ref-dlls
   - Updated all HintPath entries to point to `ref-dlls/TerraInvicta/` and `ref-dlls/UMM/`
   - Used `$(MSBuildThisFileDirectory)` for portability

4. **`.gitignore`** — Added ref-dlls exclusion
   - Prevents large binary DLLs from cluttering git

5. **`ModInfo.json`** and **`AssemblyInfo.cs`** — Version bump
   - `0.8.1` → `0.8.2`

### Files Added
- `Handover/Reference Guide.md` — Developer resource guide
- `Handover/Fix Summary - Double Counting Bug.md` — Bug documentation
- `Handover/Handover v2.md` (this file) — Updated state

### Testing Notes
To verify the fix:
1. Create councilor with Science 80
2. Target another councilor with Assist mission
3. Complete successfully → ~(80 * 0.5) / 100 ≈ 0.4 Science bonus recorded
4. Check nation research bonus
5. **Before fix:** ~2x inflated | **After fix:** Correct single application

---

**Handover v2 created at:** 2026-02-10  
**Branch:** `cap-and-breakdown` (commit: TBD)  
**Version:** 0.8.2  
**Next developer:** Update v3 when your session completes and push.
