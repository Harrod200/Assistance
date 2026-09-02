# Assistance Mod - Developer Guide

## Project Setup

### Prerequisites
- Visual Studio 2019 or later (Community edition is fine)
- .NET Framework 4.8
- Terra Invicta game files (for assembly references)
- HarmonyLib NuGet package

### Project Structure

```
Assistance/
├── Core Mission Files
│   ├── TIMissionTemplate_Assist.cs       # Mission template definition
│   ├── TIMissionEffect_Assist.cs         # Bonus application logic
│   └── TIMissionModifier_*.cs            # Mission modifier implementations
│
├── Mission Conditions
│   ├── TIMissionCondition_MyFactionCouncilor.cs
│   ├── TIMissionCondition_PlayerFactionOnly.cs
│   └── TIMissionCondition_NotCurrentlyAssisting.cs
│
├── Integration Patches
│   ├── AssistMissionBootstrapPatch.cs
│   ├── FinalizeCouncilorMissions_AssistPriorityPatch.cs
│   ├── TIMissionResolution_Contested_AssistBonusPatch.cs
│   ├── TICouncilorState_GetPossibleMissionListPatch.cs
│   ├── TICouncilorState_CompleteMissionPatch.cs
│   └── [More patches...]
│
├── Bonus System
│   └── AssistBonusTracker.cs             # Central bonus tracking
│
├── UI & Display
│   ├── CouncilorView_GetAttributeStringPatch.cs
│   ├── CouncilGridController_SetStatValuePatch.cs
│   └── CouncilorMissionCanvasController_UpdateModifierListPatch.cs
│
├── Configuration
│   ├── Main.cs                           # Mod entry point
│   ├── Settings.cs                       # Settings data
│   └── TIMissionTemplate.en              # Localization
│
└── Documentation
	├── README.md                          # Project overview
	├── ARCHITECTURE.md                    # System design
	└── DEVELOPER_GUIDE.md                 # This file
```

## Building the Project

### Visual Studio

1. Open `Assistance.slnx` in Visual Studio
2. Build → Build Solution (Ctrl+Shift+B)
3. Output assembly: `bin\Debug\Assistance.dll` or `bin\Release\Assistance.dll`

### Command Line

```powershell
# Navigate to project directory
cd C:\Users\Chris\source\repos\Assistance\

# Build project
dotnet build Assistance\Assistance.csproj

# Or using MSBuild
MSBuild.exe Assistance\Assistance.csproj /p:Configuration=Release
```

## Code Style & Conventions

### Naming Conventions

- **Classes**: PascalCase - `TIMissionCondition_NotCurrentlyAssisting`
- **Methods**: PascalCase - `ApplyEffect()`, `RecordBonus()`
- **Variables**: camelCase - `baseValue`, `assistBonus`
- **Constants**: UPPER_CASE - `DEFAULT_ASSIST_PERCENTAGE`
- **Private fields**: camelCase with underscore prefix (when needed)

### Comment Style

Use XML documentation comments for public methods:

```csharp
/// <summary>
/// Calculates the bonus amount based on assister's stat and configured percentage.
/// </summary>
/// <param name="assisterStat">The assisting councilor's stat value</param>
/// <param name="assistPercentage">Percentage to apply (0-100)</param>
/// <returns>Calculated bonus amount</returns>
private static int CalculateBonus(int assisterStat, float assistPercentage)
{
	// Implementation
}
```

Inline comments explain *why*, not *what*:

```csharp
// Good: Explains reason
// Only apply bonus if mission succeeded - failed assists don't grant bonuses
if (outcome == MissionOutcome.Success)

// Bad: Restates code
// Check if outcome equals Success
if (outcome == MissionOutcome.Success)
```

### Error Handling

All patches include try-catch with logging:

```csharp
try
{
	// Patch logic
	if (Main.mod != null)
	{
		Main.mod.Logger.Log("[PatchName] Debug info");
	}
}
catch (Exception ex)
{
	if (Main.mod != null)
	{
		Main.mod.Logger.Error("[PatchName] Error: " + ex.Message);
	}
}
```

### Harmony Patch Naming

Pattern: `[ClassName]_[PurposeSuffix]Patch.cs`

Examples:
- `TIMissionResolution_Contested_AssistBonusPatch.cs` - Applies assist bonus to contested missions
- `FinalizeCouncilorMissions_AssistPriorityPatch.cs` - Prioritizes assist missions
- `TICouncilorState_CompleteMissionPatch.cs` - Clears bonuses on completion

## Adding New Features

### Adding a New Mission Condition

1. Create new file: `TIMissionCondition_YourCondition.cs`
2. Inherit from `TIMissionCondition`
3. Implement validation logic

```csharp
public class TIMissionCondition_YourCondition : TIMissionCondition
{
	public override string Validate(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		// Return null if valid, error message if invalid
		if (!IsValidTarget(target))
		{
			return "Target does not meet condition";
		}
		return null;
	}
}
```

4. Add to mission conditions in `TIMissionTemplate_Assist`:

```csharp
this.conditions = new List<TIMissionCondition>
{
	new TIMissionCondition_MyFactionCouncilor(),
	new TIMissionCondition_PlayerFactionOnly(),
	new TIMissionCondition_NotCurrentlyAssisting(),
	new TIMissionCondition_YourCondition()  // Add here
};
```

### Adding a New Patch

1. Create new file: `[TargetClass]_[Purpose]Patch.cs`
2. Apply HarmonyPatch attribute

```csharp
[HarmonyPatch(typeof(TargetClass), "MethodName")]
internal static class TargetClass_PurposePatch
{
	private static void Postfix(TargetClass __instance, /* parameters */)
	{
		try
		{
			// Your patch logic
		}
		catch (Exception ex)
		{
			if (Main.mod != null)
			{
				Main.mod.Logger.Error("[PatchName] Error: " + ex.Message);
			}
		}
	}
}
```

3. Patch will be automatically applied by `Main.Load()` when it calls `harmony.PatchAll()`

### Adding a New Stat Bonus Type

1. Create new modifier class inheriting from `TIMissionModifier`
2. Add to mission in `TIMissionTemplate_Assist` if needed
3. Update `AssistBonusTracker` if tracking additional data

### Modifying Bonus Calculation

Edit `TIMissionEffect_Assist.cs` in the `ApplyEffect()` method:

```csharp
// Current calculation (line ~43)
float assistPercentage = Main.settings.assistPercentage / 100f;

// To change formula, modify here:
int assistAmount = Mathf.RoundToInt(assisterValue * assistPercentage);
```

Examples:
- **Flat bonus**: `int assistAmount = assisterValue / 4;` (25% as division)
- **Scaled bonus**: `int assistAmount = Mathf.Min(assisterValue / 2, maxCap);` (with cap)
- **Tiered bonus**: `int assistAmount = assisterValue < 5 ? 1 : assisterValue / 4;` (based on tier)

## Debugging

### Enable Debug Logging

1. Open mod settings in-game
2. Check "Enable Debug Logging"
3. View output in console or log file

### Common Debug Points

```csharp
if (Main.mod != null && Main.settings.debugLogging)
{
	Main.mod.Logger.Log($"[ComponentName] Variable: {value}");
}
```

### Debugging Patches

Add logging to track patch execution:

```csharp
private static void Postfix(/* params */)
{
	if (Main.mod != null)
	{
		Main.mod.Logger.Log("[PatchName] Postfix executed for " + __instance.GetType().Name);
	}

	try
	{
		// Logic here
	}
	catch (Exception ex)
	{
		Main.mod.Logger.Error("[PatchName] " + ex);
	}
}
```

### Debugging Bonus Tracking

Use `AssistBonusTracker` static methods to query state:

```csharp
// In patch or effect:
int total = AssistBonusTracker.GetTotalBonus(councilor);
if (Main.mod != null)
{
	Main.mod.Logger.Log($"Total bonus for {councilor.displayName}: {total}");
}
```

## Common Issues & Solutions

### Issue: Patch Not Applying

**Cause**: `harmony.PatchAll()` not finding patch or reflection binding failure

**Solution**:
1. Verify `[HarmonyPatch]` attribute syntax
2. Check method name matches exactly (case-sensitive)
3. Verify parameter types in signature
4. Add logging in patch to verify execution

### Issue: Bonuses Not Appearing

**Cause**: `AssistBonusTracker` not recording or UI not displaying

**Solution**:
1. Verify `TIMissionEffect_Assist.ApplyEffect()` is called
2. Check `AssistBonusTracker.RecordBonus()` receives valid data
3. Verify UI patches are applied (check CouncilorView_GetAttributeStringPatch)
4. Enable debug logging and trace execution

### Issue: AI Crashes on Assist Mission

**Cause**: AI evaluates mission modifiers without bonus tracking context

**Solution**:
- Ensure `TICouncilorState_GetPossibleMissionListPatch` removes missions from AI
- Verify `if (!faction.IsPlayerControlled)` check in condition

### Issue: Bonuses Persist After Mission

**Cause**: `TICouncilorState_CompleteMissionPatch` not called or not working

**Solution**:
1. Add logging to verify patch execution
2. Check mission completion flow in game
3. Verify `AssistBonusTracker.RemoveBonuses()` is called
4. Check councilor state after mission completes

## Testing Checklist

### Functional Testing

- [ ] Assist mission appears in mission list for player-controlled factions
- [ ] Assist mission does NOT appear for AI-controlled factions
- [ ] Can select valid targets (same faction, not already assisting)
- [ ] Cannot select invalid targets (wrong faction, self, already assisting)
- [ ] Mission resolves successfully (automatically, no opposition)
- [ ] Bonuses are applied to target councilor
- [ ] Bonuses display in UI (orange color)
- [ ] Bonuses are applied in contested missions
- [ ] Bonuses are cleared when mission completes
- [ ] Changing assistance percentage updates bonus calculation

### Settings Testing

- [ ] Enable/disable toggle works
- [ ] Assistance percentage setting persists
- [ ] Debug logging toggle works
- [ ] Settings save and load correctly

### Edge Cases

- [ ] Multiple councilors assisting same target (should not happen due to conditions)
- [ ] Assist mission cancellation
- [ ] Councilor with assist bonus completing other missions
- [ ] Loading saves with active assist missions
- [ ] Rapid mission assignment/completion

### Performance

- [ ] No noticeable lag when assigning missions
- [ ] No lag in UI updates with bonuses
- [ ] Memory usage stable during gameplay
- [ ] No frame rate drops from patch overhead

## Extending the Bonus System

### Adding New Bonus Types

Currently, bonuses are tracked by stat attribute. To add new bonus categories:

```csharp
// In AssistBonusTracker.cs, modify dictionary structure:
private static Dictionary<TICouncilorState, Dictionary<string, int>> bonusesById;

// Example: By bonus source
bonusesById[councilor]["Assist_Persuasion"] = 2;
bonusesById[councilor]["Teach_Command"] = 1;
```

### Adding Bonus Expiration

Currently, bonuses last until mission completion. To add time-based expiration:

```csharp
private struct BonusRecord
{
	public int Amount;
	public TIDateTime ExpirationTime;
}

// Modify tracking to check expiration on retrieval
```

### Adding Multiplicative vs. Additive Modes

```csharp
// In TIMissionEffect_Assist.cs
if (Main.settings.bonusMode == BonusMode.Multiplicative)
{
	// Apply as multiplier: base * (1 + assistBonus%)
}
else
{
	// Apply as additive: base + assistBonus
}
```

## Release Checklist

Before releasing a new version:

- [ ] Code builds without warnings
- [ ] All tests pass
- [ ] Documentation updated
- [ ] Version number incremented in AssemblyInfo.cs
- [ ] Changelog updated
- [ ] Git commits cleaned up
- [ ] Code reviewed
- [ ] No debug logging enabled in final build
- [ ] Settings defaults are reasonable

## Useful Resources

### Terra Invicta Modding
- Check existing mission implementations for patterns
- Review game assemblies for class definitions
- Use dnSpy or similar tools to explore game code

### HarmonyLib
- Official docs: https://harmony.pardeike.net/
- Common patterns: Prefix (before), Postfix (after), Transpiler (IL-level)
- PatchAll() automatically discovers patches with `[HarmonyPatch]`

### .NET Framework 4.8
- MSDN documentation for System namespace
- Available at development time via Visual Studio

## Future Improvements

Potential enhancements to consider:

1. **Mission Variants**: Different assist mission types (train, mentor, assist)
2. **Bonus Decay**: Bonuses decrease over time
3. **Chain Assisting**: Councilors assisting other councilors assisting
4. **Limit System**: Prevent assisting same target multiple times
5. **Cost System**: Assign a resource cost to assist missions
6. **Stat Cap**: Prevent bonuses from exceeding game's max stat
7. **Faction Benefits**: Different factions get different bonuses
8. **AI Support**: Implement AI-safe bonus calculations
9. **UI Indicators**: Visual markers on councilors receiving assistance
10. **Bonus History**: Track which councilors have assisted which
