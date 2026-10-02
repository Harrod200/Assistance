# Terra Invicta Reference Guide

## Available Resources

### 1. **Terra Invicta Class & Method Reference** (TRUSTED SOURCE)
**Location:** `ref-dlls/TI Decompiled/Terra Invicta Class & Method Reference.md` (2.9 MB)

This is the **authoritative reference** for Terra Invicta's decompiled API. It contains:
- Full class hierarchy and method signatures
- Field definitions
- Complete API documentation indexed by class name
- Verified against actual decompiled source code

**GOLDEN RULE:** Before grepping the decompiled code or searching vanilla assemblies, search this reference first. If the answer is already documented here, use it. This saves time and prevents mistakes.

### 2. **Decompiled Game Code**
**Location:** `ref-dlls/TI Decompiled/GameAnalysis/Assembly-CSharp/`

Contains the full decompiled source code from `Assembly-CSharp.dll` (the main Terra Invicta game assembly). Use this to:
- Understand implementation details not fully captured in the reference doc
- Find edge cases and specific behavior
- Verify method signatures and field usage patterns
- Cross-check with the reference documentation

**Structure:**
- `solution.sln` — Decompiled project for browsing
- `Assembly-CSharp/` — Organized source files by namespace

**WARNING:** This is decompiled code and may have naming artifacts (obfuscation remnants). Always verify against the reference documentation first.

### 3. **String Resources**
**Location:** `ref-dlls/TI Decompiled/Strings/`

Localization strings extracted from game assemblies. Useful for:
- Finding exact string keys for UI display
- Understanding mission, building, and unit descriptions
- Localizing mod content to match game text

### 4. **Mod Analysis**
**Location:** `ref-dlls/TI Decompiled/ModAnalysis/`

Reference for how other mods work. May contain:
- Common patch patterns
- Established best practices
- Examples of working HarmonyLib patches

---

## How to Use These Resources

### Scenario 1: "I need to understand how TIMissionState works"
1. **First:** Search `Terra Invicta Class & Method Reference.md` for "TIMissionState"
2. **Get:** Full class definition, all methods, fields, and usage notes
3. **Only if Reference is unclear:** Navigate to `GameAnalysis/Assembly-CSharp/` and find the actual decompiled code

### Scenario 2: "I'm patching TIGameState.GetCouncilorById — what are the exact parameters?"
1. **First:** Search the reference for "GetCouncilorById"
2. **Verify:** Method signature, return type, and any documented side effects
3. **Then:** Use that in your HarmonyPatch attribute
4. **Cross-check:** Decompiled code if behavior seems odd

### Scenario 3: "I need a mission modifier string key for localization"
1. Check `Strings/` for mission-related keys
2. Search the reference for "TIMissionModifier"
3. Look at `Assistance/TIMissionTemplate.en` for examples of how this mod does it

---

## Key Classes for Assistance Mod

These are the main Terra Invicta classes the Assistance mod interacts with:

### TICouncilorState
- Main councilor entity
- Methods: `GetPossibleMissionList()`, `GetAttribute(CouncilorAttribute)`, `SetAttribute(CouncilorAttribute, float)`
- Fields: faction reference, mission state, current attributes
- **Reference location:** Search reference doc for "TICouncilorState"

### TIMissionTemplate
- Defines mission properties (name, duration, contested flag, stat attributes)
- Subclassed by mod via `TIMissionTemplate_Assist`
- **Reference location:** Search for "TIMissionTemplate"

### TIMissionState
- Runtime instance of an active mission
- Contains participants, current phase, bonuses applied
- **Reference location:** Search for "TIMissionState"

### TIMissionModifier
- Individual stat modifier (e.g., "Offense +5")
- **Reference location:** Search for "TIMissionModifier"

### TIGameState
- Global game state singleton
- Stores factions, current turn, active missions
- Used to track and clean up expired assist bonuses
- **Reference location:** Search for "TIGameState"

### CouncilorAttribute (Enum)
- Loyalty, Offense, Defense, Research, Economics, LoyaltyRecovery
- Used to specify which stat a modifier affects
- **Reference location:** Search for "CouncilorAttribute"

---

## When NOT to Search the Decompiled Code

**Don't grep the decompiled code if:**
- The reference doc already answers your question
- You're trying to understand behavior (ask in comments or log)
- You're looking for a method signature (it's in the reference)
- You have < 15 min for a task (reference lookups are faster)

**DO search the decompiled code if:**
- The reference doc is unclear or incomplete
- You need to understand the exact implementation for a complex patch
- You're debugging an edge case behavior
- You're verifying that a method still exists in the current game version

---

## Maintenance Notes

When you discover something not in the reference document:
1. Note it in `Assistance/Handover/Vanilla API Cheat Sheet.md`
2. Add it to the team's shared reference (if applicable)
3. Do NOT add it to the reference doc itself (it's auto-generated/trusted)

---

## File Sizes and Update Frequency

| Resource | Size | Last Updated | Notes |
|----------|------|---|---|
| Terra Invicta Class & Method Reference.md | 2.9 MB | Per game version | Regenerated when game updates |
| Decompiled Assembly-CSharp | ~50+ files | Per game version | Use with caution; subject to decompiler artifacts |
| Strings | Various | Per game version | Useful for UI text, less critical for logic |
| Mod Analysis | TBD | As needed | Community reference, not authoritative |

---

## Quick Navigation

- **API Question?** → `Terra Invicta Class & Method Reference.md`
- **Implementation detail?** → `ref-dlls/TI Decompiled/GameAnalysis/Assembly-CSharp/`
- **UI/Localization string?** → `ref-dlls/TI Decompiled/Strings/`
- **Mod pattern example?** → `ref-dlls/TI Decompiled/ModAnalysis/`
- **Project-specific findings?** → `Assistance/Handover/Vanilla API Cheat Sheet.md`

---

**Remember:** The reference documentation is your friend. Use it first; it will save you hours of grep and decompilation work.
