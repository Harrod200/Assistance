# Assistance Mod - Code Reference & Class Documentation

**Purpose:** Complete reference of all custom classes and methods in the Assistance Mod, with detailed explanations for future maintenance and enhancement.

---

## 📋 Table of Contents

1. [Core Mission Template](#core-mission-template)
2. [Mission Effects & Modifiers](#mission-effects--modifiers)
3. [Mission Conditions](#mission-conditions)
4. [Bonus Tracking](#bonus-tracking)
5. [Harmony Patches](#harmony-patches)
6. [Configuration & Entry](#configuration--entry)
7. [Game Data Structures Referenced](#game-data-structures-referenced)

---

## Core Mission Template

### **TIMissionTemplate_Assist.cs**

**Purpose:** Defines the Assist mission template - the core mission object that gets registered with the game.

**Key Properties:**
| Property | Value | Explanation |
|----------|-------|-------------|
| `dataName` | "Assist" | Unique identifier used throughout the mod for this mission |
| `friendlyName` | "Assist Councilor" | Display name shown to players in UI |
| `resolutionMethod` | `TIMissionResolution_Automatic` | Guarantees 100% success (no dice rolls like Contested resolution) |
| `attackingModifiers` | Empty List | Required for Automatic resolution (AI planner expects empty list) |
| `defendingModifiers` | Empty List | Required for Automatic resolution (AI planner expects empty list) |
| `attackerContexts` | `{Context.None, Context.None}` | Matches vanilla pattern for support missions |
| `defenderContexts` | `{Context.None, Context.None}` | Matches vanilla pattern for support missions |
| `conditions` | List of 2 conditions | Validates targets (see Mission Conditions section) |
| `targetEffects` | List with `TIMissionEffect_Assist` | Applies stat bonuses when mission succeeds |
| `cost` | `TIMissionCost_Bonus(FactionResource.None)` | Free mission - no IP/Influence cost |
| `resolutionOrder` | 0 | Resolves first each turn (fastest resolution) |
| `XPonSuccess` | 2 | Experience gained by source councilor on success |
| `sortOrder` | 23 | Position in mission list (after Inspire at 22) |
| `maximumTargetOptionCount` | 20 | Maximum number of valid targets shown |

**Constructor Flow:**
1. Initialize base mission template with name "Assist"
2. Set all mission properties to match Inspire mission pattern
3. Create mission conditions list
4. Create resolution method with empty modifier lists
5. Populate target effects list with bonus application effect
6. Handle exceptions and log errors

**Critical Notes:**
- Must use `TIMissionResolution_Automatic` (not Contested) because AI planner can't evaluate missions with empty modifier lists
- Empty context lists prevent dictionary lookup errors in AI mission planner
- Must be granted to councilor types via `GrantToAllCouncilors()` or mission never appears in available missions

---

## Mission Effects & Modifiers

### **TIMissionEffect_Assist.cs**

**Purpose:** Implements the actual game effect that applies stat bonuses to the target councilor when the mission succeeds.

**Class Inheritance:** `TIMissionEffect` (base class from game)

**Key Method:**
```csharp
public override void ApplyEffect(TICouncilorState targetCouncilor, TIMissionTemplate template, bool successfulCompletion, TIGameState originalTarget)
```

**Method Behavior:**
1. Retrieves the source councilor (who is performing the Assist mission) from the template
2. Gets the current assist percentage from mod settings (0-100%, default 25%)
3. Iterates through 7 key stats: Persuasion, Investigation, Espionage, Command, Administration, Science, Security
4. For each stat:
   - Gets source councilor's current value
   - Calculates bonus: `sourceValue * (assistPercentage / 100)`
   - Applies bonus via `targetCouncilor.ModifyAttribute()`
   - Records bonus in `AssistBonusTracker` for later removal
5. Logs the applied bonus amounts for debugging

**Key Properties:**
- `sourceMissionName` - Internal field to track which councilor is the source
- Stats array - The 7 specific stats that get boosted

**Critical Notes:**
- Must handle null checks for councilors
- Must track bonuses immediately after application
- Bonuses are TEMPORARY and will be removed by `AssistBonusTracker` when target completes next mission
- Uses `GetAttribute(..., true, true, true, false, false, false)` to get base stat without other modifiers

### **TIMissionModifier_AssistStat.cs**

**Purpose:** Custom modifier for mission resolution that calculates success modifier based on source councilor's Persuasion stat.

**Class Inheritance:** `TIMissionModifier_CouncilorStat`

**Key Property:**
```csharp
public new CouncilorAttribute attackerAttribute = CouncilorAttribute.Persuasion;
```
- Uses `new` keyword to explicitly indicate field shadowing (hides parent class field)
- Specifies that Persuasion stat determines mission success chance

**Key Method:**
```csharp
public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target, float resourcesSpent, FactionResource resource)
```
- Returns source councilor's Persuasion stat × base multiplier
- Used during mission resolution for success calculation
- Returns 0 if councilor is null

**Display Property:**
```csharp
public override string displayName { get; }
```
- Returns localized name of the Persuasion attribute for UI display
- Falls back to "Persuasion" if localization fails

**Critical Notes:**
- Currently not used with Automatic resolution (which has no modifier evaluation)
- Kept for potential future switch back to Contested resolution
- The `new` keyword is intentional and documented (not a mistake)

### **TIMissionModifier_AssistFlat.cs**

**Purpose:** Placeholder flat modifier for potential future use.

**Status:** Minimal implementation, not currently used in Automatic resolution

**Current Implementation:**
- Inherits from `TIMissionModifier_FlatModifier`
- Applies 0 modifier (neutral - no effect)
- Exists for compatibility and future enhancements

---

## Mission Conditions

### **TIMissionCondition_PlayerFactionOnly.cs**

**Purpose:** Custom condition that restricts Assist mission to player-controlled factions only (prevents AI from using it).

**Class Inheritance:** `TIMissionCondition` (base class from game)

**Key Method:**
```csharp
public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
```

**Method Behavior:**
1. Validates that councilor and faction are not null
2. Checks if faction's player is AI-controlled: `councilor.faction.player.isAI`
3. Returns `TIMissionCondition.fail` if faction is AI-controlled
4. Returns `TIMissionCondition.pass` if faction is player-controlled
5. Logs each condition evaluation for debugging

**Return Values:**
- `TIMissionCondition.pass` (= "_Pass") - Condition passes, mission available
- `TIMissionCondition.fail` (= "_Fail") - Condition fails, mission not available

**Critical Implementation Details:**
- **CORRECT CHECK:** `faction.player.isAI` (evaluates TIPlayerState.isAI property)
- **INCORRECT APPROACH:** `faction.playerControl != null` (always non-null, doesn't distinguish player vs AI)
- Must check via `faction.player`, not `faction.playerControl`
- Game's own `TIFactionCondition_bAIControlled` uses same pattern

**Critical Notes:**
- This condition alone is NOT sufficient to prevent AI evaluation (see Lesson #2 in AI_DEVELOPER_SUMMARY)
- Also relies on `TICouncilorState_GetPossibleMissionListPatch` to filter from AI mission lists
- Condition checks SOURCE councilor's faction, not target councilor's faction

### **TIMissionCondition_MyFactionCouncilor.cs**

**Purpose:** Custom condition that restricts Assist mission targets to same faction as source councilor.

**Implementation:** Custom implementation required (vanilla class exists but custom implementation needed for reliability)

**Behavior:**
- Ensures target is in same faction as source
- Typically prevents self-targeting

---

## Bonus Tracking

### **AssistBonusTracker.cs**

**Purpose:** Tracks all applied stat bonuses and automatically removes them when the target councilor completes any mission.

**Key Data Structure:**
```csharp
private static Dictionary<TICouncilorState, Dictionary<CouncilorAttribute, int>> bonusTracking;
```
- Maps each councilor to their bonuses by stat type
- Stores the bonus amount for each stat

**Key Static Methods:**

#### `RecordBonus(TICouncilorState councilor, CouncilorAttribute stat, int amount)`
- Called immediately after stat bonus is applied
- Stores the bonus amount in tracking dictionary
- Allows later removal of exact amount applied

#### `RemoveExpiredBonuses(TICouncilorState councilor)`
- Called when councilor completes any mission
- Looks up all bonuses for that councilor
- Removes each bonus using `ModifyAttribute(stat, -amount)`
- Clears councilor from tracking dictionary after removal
- Logs removal for debugging

**Lifecycle:**
1. Mission succeeds → `TIMissionEffect_Assist.ApplyEffect()` runs
2. `ApplyEffect()` calls `AssistBonusTracker.RecordBonus()` for each stat
3. Bonus is now tracked
4. When target councilor completes ANY mission → `TICouncilorState_CompleteMissionPatch` triggers
5. Patch calls `AssistBonusTracker.RemoveExpiredBonuses()`
6. All bonuses are reverted
7. Councilor is removed from tracking

**Critical Notes:**
- Uses static methods and static dictionary (global state)
- Assumes each stat can only have one active Assist bonus per councilor
- Thread-safe for single-threaded game execution
- Bonuses persist across turns but disappear on mission completion (by design)

---

## Harmony Patches

### **AssistMissionBootstrapPatch.cs**

**Purpose:** Registers the Assist mission template at game startup and grants it to all councilor types.

**Target:** `SolarSystemBootstrap.Initialize()` method - runs when solar system data loads

**Patch Type:** Postfix (runs AFTER original method)

**Key Methods:**

#### `Postfix()`
- Called after solar system bootstrap completes
- Checks if mod is enabled via `Main.enabled` and settings
- Calls `RegisterMissionTemplate()` to add mission to game
- Calls `GrantToAllCouncilors()` to add mission to councilor types
- Logs registration success with mission properties (dataName, conditions count, etc.)

#### `RegisterMissionTemplate()`
- Creates new `TIMissionTemplate_Assist` instance
- Logs mission properties for debugging:
  - dataName, friendlyName
  - Resolution method type
  - Modifier counts
  - Context counts
  - Conditions count
- Adds mission to `TemplateManager` with `TemplateManager.Add()`
- Exception handling with detailed error logging

#### `GrantToAllCouncilors()`
- Iterates through all councilor types via `TemplateManager.IterateByClass<TICouncilorTypeTemplate>(true)`
- For each councilor type:
  - Checks if Assist is already in `missionNames` array
  - If not present, adds it via `Append()` helper method
  - Clears `_missions` cache via `ClearPrivateCache()` to force repopulation
  - Increments counter and logs
- Returns count of councilor types that received the mission
- Exception handling with logging

#### `Append(string[] values, string value)` - Helper
- Creates new array with mission name appended
- Handles null input array case
- Returns new array with added element

#### `Contains(string[] values, string value)` - Helper
- Checks if mission name already exists in array
- Prevents duplicate additions

#### `ClearPrivateCache(object target, string fieldName)` - Helper
- Uses reflection to access private `_missions` field
- Sets it to null to force cache repopulation
- Called on councilor type after adding mission to `missionNames`

**Critical Notes:**
- **ISSUE (v0.4.0+):** `TemplateManager.IterateByClass<TICouncilorTypeTemplate>()` returns 0 results at `SolarSystemBootstrap.Initialize()` time
- Suggests councilor types haven't been loaded yet at this bootstrap phase
- Need alternative bootstrap hook or manual mission granting approach
- Previous disabled version (v0.3.12) avoided this by not granting to avoid AI planner crashes, but this defeats the purpose

### **TICouncilorState_GetPossibleMissionListPatch.cs**

**Purpose:** Filters the Assist mission from AI-controlled faction councilors to prevent crashes during AI mission planning.

**Target:** `TICouncilorState.GetPossibleMissionList(bool, bool, bool, TIOrgState, bool)` method

**Patch Type:** Postfix (runs AFTER original method returns mission list)

**Key Method:**

#### `Postfix(TICouncilorState __instance, ref List<TIMissionTemplate> __result)`
- Receives councilor instance and mission list from original method
- Checks if mission or result is null → early return
- Logs every call with:
  - Councilor display name
  - Faction control status (PLAYER or AI) based on `faction.player.isAI`
  - Number of missions in result list
  - Whether Assist is present in list
- **Filter Logic:**
  - Only filters for AI-controlled factions (`faction.player.isAI == true`)
  - Returns early for player-controlled factions (leaves Assist in list)
  - For AI factions: removes Assist mission from result list via `RemoveAll()`
  - Logs removal with councilor and faction names
- Exception handling with error logging

**Why This Patch Exists:**
1. Assist mission has empty modifier lists in Automatic resolution
2. AI mission planner evaluates missions before checking conditions
3. Empty modifier lists cause KeyNotFoundException in planner's dictionary
4. Filtering at retrieval stage (before AI evaluation) prevents crash entirely
5. Player factions can still use mission - only removed from AI evaluation

**Critical Notes:**
- Uses `faction.player.isAI` check (NOT `playerControl != null`)
- Must return early for player factions or mission gets filtered for them too
- Extensive logging helps debug mission availability issues
- This is a DEFENSIVE patch - prevents crashes, not primary availability mechanism

### **TICouncilorState_CompleteMissionPatch.cs**

**Purpose:** Triggers bonus removal when a councilor completes any mission.

**Target:** `TICouncilorState.SetCompletedMission()` method - called when mission completion is registered

**Patch Type:** Postfix (runs AFTER mission completion is processed)

**Key Method:**

#### `Postfix(TICouncilorState __instance)`
- Receives the councilor who just completed a mission
- Calls `AssistBonusTracker.RemoveExpiredBonuses(__instance)`
- Bonus removal happens automatically
- No additional logging needed (tracker logs removal details)

**Lifecycle Hook:**
- Runs every time any councilor completes any mission
- Removes Assist bonuses for that specific councilor
- Other councilors' bonuses remain active

**Critical Notes:**
- Simple postfix - just triggers tracker method
- No parameters or complex logic
- Logging handled by `AssistBonusTracker`

### **CouncilorMissionCanvasController_UpdateModifierListPatch.cs**

**Purpose:** UI patch for rendering mission modifiers in the mission selection UI (if needed).

**Status:** Less critical, used for UI enhancements

**Note:** Details depend on specific UI requirements

---

## Configuration & Entry

### **Main.cs**

**Purpose:** UMM (UnityModManager) entry point and GUI settings management.

**Key Static Fields:**
- `public static UnityModManager.ModEntry mod` - ModEntry instance (null if not initialized)
- `public static bool enabled` - Whether mod is currently enabled
- `public static Settings settings` - Settings instance with configurable values

**Key Methods:**

#### `Load(UnityModManager.ModEntry modEntry)`
- UMM entry point called when mod loads
- Sets `Main.mod = modEntry`
- Loads settings from file via `UnityModManager.FindSettings()`
- Registers GUI callback for settings panel
- Applies all Harmony patches via `var harmony = new Harmony(modEntry.Info.Id); harmony.PatchAll();`
- Enables mod and logs success

#### `OnGUI(UnityModManager.ModEntry modEntry)` (static)
- Renders the mod's settings GUI
- Creates UI controls for configurable options:
  - Enable/disable toggles
  - Assist percentage slider (0-100%)
  - Other configuration options
- Updates `settings` as user changes values

#### `OnSaveGUI(UnityModManager.ModEntry modEntry)` (static)
- Saves settings to file when user clicks Save
- Called automatically by UMM framework

**Critical Notes:**
- Entry point for all Harmony patches
- Settings GUI shows up in ModEntry options menu
- All patches depend on successful Load() execution

### **Settings.cs**

**Purpose:** Stores configuration values that persist between game sessions.

**Key Properties:**
- `enableAssistMission` - Boolean to toggle mission availability (default: true)
- `assistPercentage` - Integer 0-100 for stat transfer percentage (default: 25)
- Other future configuration options

**Serialization:**
- Automatically serialized to XML by UMM framework
- File location: `Mods/Enabled/AssistMission/Settings.xml`

**Default Values:**
```csharp
public bool enableAssistMission = true;      // Mission enabled by default
public int assistPercentage = 25;             // Transfer 25% of source stats by default
```

**Usage:**
- Retrieved in `Main.Load()` via `UnityModManager.FindSettings<Settings>()`
- Passed to other modules via `Main.settings`
- Used in effect application: `(int)(sourceValue * (assistPercentage / 100.0))`
- UI controls allow runtime modification

**Critical Notes:**
- `assistPercentage` must be 0-100 (validated in UI)
- Settings affect all Assist missions retroactively
- Changing percentage doesn't affect previously applied bonuses (only new missions)

---

## Game Data Structures Referenced

### **Key Terra Invicta Classes Used (Not Modified)**

| Class | Purpose | Usage in Assistance |
|-------|---------|-------------------|
| `TIMissionTemplate` | Base class for all missions | Inherited by `TIMissionTemplate_Assist` |
| `TIMissionEffect` | Base class for mission effects | Inherited by `TIMissionEffect_Assist` |
| `TIMissionCondition` | Base class for mission conditions | Inherited by `TIMissionCondition_PlayerFactionOnly` |
| `TICouncilorState` | Represents individual councilor in game | Effect target, bonus tracking source |
| `TIFactionState` | Represents faction data | Checked for player vs AI control |
| `TIPlayerState` | Represents player data | Checked for `isAI` property |
| `TIMissionResolution_Automatic` | Automatic resolution (no dice rolls) | Mission resolution method |
| `TIMissionCost_Bonus` | Mission cost type | Cost of Assist mission (none) |
| `Context` | Enum for mission contexts | Used in context lists |
| `CouncilorAttribute` | Enum for stat types | The 7 stats being boosted |
| `TemplateManager` | Registry for all templates | Adds mission, iterates councilor types |
| `FactionResource` | Enum for resource types | Assist is free (None) |

### **Enum: CouncilorAttribute** (7 stats transferred)
```csharp
Persuasion        // Diplomacy and influence
Investigation     // Intelligence gathering
Espionage         // Covert operations
Command           // Military leadership
Administration    // Internal management
Science           // Research and development
Security          // Defense and security
```

### **Key Property: TIPlayerState.isAI**
- Type: `bool { get; private set; }`
- Set via: `public void AssignAIStatus(bool isAI)`
- Indicates whether player controlling a faction is AI
- Correct way to distinguish player vs AI factions

### **Key Property: TICouncilorTypeTemplate.missionNames**
- Type: `public string[]`
- Contains mission names available to this councilor type
- Must be modified to grant Assist mission
- Modified array triggers cache clear via `_missions = null`

---

## Code Flow Diagram

```
Game Startup
	↓
SolarSystemBootstrap.Initialize()
	↓
AssistMissionBootstrapPatch.Postfix()
	├─→ RegisterMissionTemplate()
	│   └─→ TemplateManager.Add(TIMissionTemplate_Assist)
	└─→ GrantToAllCouncilors() [CURRENTLY BROKEN - returns 0 councilor types]
		└─→ Add "Assist" to each councilor type's missionNames array
			└─→ Clear _missions cache

When Player Views Mission List
	↓
TICouncilorState.GetPossibleMissionList()
	↓
TICouncilorState_GetPossibleMissionListPatch.Postfix()
	├─→ If AI faction → RemoveAll("Assist")
	└─→ If Player faction → Keep Assist

When Player Selects Assist Mission
	↓
Mission Conditions Evaluated
	├─→ TIMissionCondition_MyFactionCouncilor → Must be same faction
	└─→ TIMissionCondition_PlayerFactionOnly → Must be player faction

When Assist Mission Succeeds
	↓
TIMissionEffect_Assist.ApplyEffect()
	├─→ Get source councilor stats
	├─→ Calculate bonuses (sourceValue * percentage / 100)
	├─→ Apply bonuses via ModifyAttribute()
	└─→ Track bonuses in AssistBonusTracker

When Target Councilor Completes Any Mission
	↓
TICouncilorState.SetCompletedMission()
	↓
TICouncilorState_CompleteMissionPatch.Postfix()
	↓
AssistBonusTracker.RemoveExpiredBonuses()
	└─→ Revert all tracked bonuses for that councilor
```

---

## Known Issues & Investigation Notes

### Issue: Assist Mission Not Appearing (v0.4.0+)

**Symptoms:**
- Mission registered successfully
- No "Assist" appears in councilor mission lists
- Even for player-controlled factions

**Root Cause:**
- `TemplateManager.IterateByClass<TICouncilorTypeTemplate>(true)` returns 0 results at `SolarSystemBootstrap.Initialize()` time
- Suggests councilor type templates not loaded yet at this bootstrap phase

**Attempted Solutions:**
1. ✅ Added verbose logging to GrantToAllCouncilors() to diagnose
2. ✅ Verified mission template registration works (shows in logs)
3. ❌ `IterateByClass()` still returns empty list
4. ⏳ Need to find correct bootstrap hook where councilor types ARE loaded

**Next Steps:**
- Find alternative bootstrap hook that runs AFTER councilor types are loaded
- Check other mods for proper granting pattern
- Possible solutions:
  - Use different Harmony target (later in initialization pipeline)
  - Manually iterate and grant in real-time when mission is requested
  - Use lazy initialization on first mission access

---

## File Organization Summary

| File | Type | Purpose |
|------|------|---------|
| `TIMissionTemplate_Assist.cs` | Mission Core | Mission definition |
| `TIMissionEffect_Assist.cs` | Effect | Applies bonuses |
| `TIMissionModifier_AssistStat.cs` | Modifier | Resolution modifier |
| `TIMissionModifier_AssistFlat.cs` | Modifier | Placeholder |
| `TIMissionCondition_PlayerFactionOnly.cs` | Condition | Restricts to player factions |
| `AssistBonusTracker.cs` | Utility | Manages bonus lifecycle |
| `AssistMissionBootstrapPatch.cs` | Harmony Patch | Registration & granting |
| `TICouncilorState_GetPossibleMissionListPatch.cs` | Harmony Patch | AI filtering |
| `TICouncilorState_CompleteMissionPatch.cs` | Harmony Patch | Bonus removal trigger |
| `CouncilorMissionCanvasController_UpdateModifierListPatch.cs` | Harmony Patch | UI rendering |
| `Main.cs` | UMM Entry | Mod initialization |
| `Settings.cs` | Configuration | User settings |
| `Properties/AssemblyInfo.cs` | Metadata | Version info |
