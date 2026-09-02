# Assistance Mod - Quick Reference

## At-a-Glance Project Info

| Item | Value |
|------|-------|
| **Project Name** | Assistance |
| **Type** | Terra Invicta Mod (HarmonyLib patches) |
| **Target Framework** | .NET Framework 4.8 |
| **License** | (Check repo) |
| **Repository** | https://github.com/Harrod200/Assistance |
| **Main Language** | C# |

## Core Concept

**Assist Councilor Mission**: Players assign one councilor to temporarily boost another's stats.

```
Councilor A (assister) → Assist Mission → Councilor B (target)
										  ↓
							Receives temporary +2 Persuasion
							(if A has 8 Persuasion, 25% setting)

Councilor B's stat: 6 Base + 2 Bonus = 8 Total (displayed in orange)

When assist mission completes → Bonus cleared
```

## File Organization

### Mission Core (4 files)
- `TIMissionTemplate_Assist.cs` - Mission definition
- `TIMissionEffect_Assist.cs` - Apply bonuses
- `TIMissionModifier_AssistFlat.cs` - Flat bonus modifier
- `TIMissionModifier_AssistStat.cs` - Stat-based modifier

### Mission Conditions (3 files)
- `TIMissionCondition_MyFactionCouncilor.cs` - Same faction check
- `TIMissionCondition_PlayerFactionOnly.cs` - Player only
- `TIMissionCondition_NotCurrentlyAssisting.cs` - No duplicate assists

### Integration Patches (7 files)
- `AssistMissionBootstrapPatch.cs` - Register & grant
- `FinalizeCouncilorMissions_AssistPriorityPatch.cs` - Resolve first
- `TIMissionResolution_Contested_AssistBonusPatch.cs` - Apply to contests
- `TICouncilorState_GetPossibleMissionListPatch.cs` - Filter AI
- `TICouncilorState_CompleteMissionPatch.cs` - Clear on complete
- `CouncilorView_GetAttributeStringPatch.cs` - Display in view
- `CouncilGridController_SetStatValuePatch.cs` - Display in grid

### UI & Config (3 files)
- `CouncilorMissionCanvasController_UpdateModifierListPatch.cs` - Modifier list
- `Main.cs` - Mod entry & settings UI
- `Settings.cs` - Configuration storage

### Bonus System (1 file)
- `AssistBonusTracker.cs` - Central tracking

### Other (2 files)
- `TIMissionTemplate.en` - Localization
- `Properties/AssemblyInfo.cs` - Assembly metadata

## Key Classes & Methods

### AssistBonusTracker (Static)
```csharp
RecordBonus(councilor, stat, amount)      // Record a bonus
GetStatBonus(councilor, stat)              // Get single stat bonus
GetTotalBonus(councilor)                   // Get total across stats
RemoveBonuses(councilor)                   // Clear all bonuses
ClearAll()                                 // Clear everything
```

### TIMissionEffect_Assist
```csharp
ApplyEffect(targetObj, success, councilor, mission, outcome)
```
Applies bonuses for all stats based on assister's stats × percentage.

### TIMissionCondition_*
Each implements:
```csharp
Validate(mission, councilor, target) → null (valid) or error message
```

## Patch Summary

| Patch | Hooks | Purpose |
|-------|-------|---------|
| AssistMissionBootstrapPatch | SolarSystemBootstrap.Initialize | Register mission |
| FinalizeCouncilorMissions_AssistPriorityPatch | FinalizeCouncilorMissions.StaggerMissionResolutions | Resolve assist first |
| TIMissionResolution_Contested_AssistBonusPatch | TIMissionResolution_Contested.SumAttackingModifiers SumDefendingModifiers | Add bonus to contests |
| TICouncilorState_GetPossibleMissionListPatch | TICouncilorState.GetPossibleMissionList | Remove from AI |
| TICouncilorState_CompleteMissionPatch | TICouncilorState.SetCompletedMission | Clear bonuses |
| CouncilorView_GetAttributeStringPatch | CouncilorView.GetAttributeString | Show combined total |
| CouncilGridController_SetStatValuePatch | CouncilGridController.SetStatValue | Update grid UI |
| CouncilorMissionCanvasController_UpdateModifierListPatch | CouncilorMissionCanvasController.UpdateModifierList | Safe display |

## Configuration

### Settings (Main.cs settings UI)
| Setting | Type | Default | Range |
|---------|------|---------|-------|
| Assist Percentage | float | 25 | 0-100 |
| Enable Assist Mission | bool | true | - |
| Enable Debug Logging | bool | false | - |

## Mission Configuration

| Property | Value | Notes |
|----------|-------|-------|
| Resolution | Automatic | Guaranteed success |
| Context | Unlimited | Works everywhere |
| Resolution Order | 0 | Resolves first |
| Movement Rule | MoveToTarget | Go to target |
| XP on Success | 2 | Standard reward |

## Development Quick Starts

### Build Project
```powershell
cd C:\Users\Chris\source\repos\Assistance\
dotnet build Assistance\Assistance.csproj
```

### Add New Condition
1. Create `TIMissionCondition_YourName.cs`
2. Inherit from `TIMissionCondition`
3. Implement `Validate()` method
4. Add to `TIMissionTemplate_Assist.this.conditions`

### Add New Patch
1. Create `[TargetClass]_[Purpose]Patch.cs`
2. Apply `[HarmonyPatch]` attribute
3. Implement Prefix/Postfix method
4. Auto-discovered by `harmony.PatchAll()`

### Debug Bonuses
```csharp
// Check if bonus tracked
int bonus = AssistBonusTracker.GetStatBonus(councilor, CouncilorAttribute.Persuasion);
if (Main.mod != null) Main.mod.Logger.Log($"Bonus: {bonus}");

// Enable debug logging in-game settings
```

## Common Issues

| Problem | Cause | Solution |
|---------|-------|----------|
| Mission not showing | AI faction or not enabled | Check `enableAssistMission` setting |
| Bonuses not applying | TIMissionEffect_Assist not called | Add logging to verify execution |
| Bonuses not displaying | UI patch not applied | Verify patch class name matches exactly |
| AI crashes | AI tries evaluate modifiers | Verify `GetPossibleMissionListPatch` removes mission |
| Bonuses persist | RemoveBonuses not called | Check `SetCompletedMission` patch execution |

## Testing Checklist (5 min)

- [ ] Assign assist mission - bonuses appear in UI
- [ ] Switch characters - bonuses show correctly
- [ ] Contested mission - bonus applied
- [ ] Mission completes - bonuses cleared
- [ ] Disable mission - option disappears
- [ ] Change percentage - bonus amount changes
- [ ] AI faction - no assist mission option

## Documentation Files

| File | Purpose | Read Time |
|------|---------|-----------|
| **README.md** | Project overview & features | 10 min |
| **ARCHITECTURE.md** | System design & components | 15 min |
| **DEVELOPER_GUIDE.md** | How to modify & extend | 15 min |
| **API_REFERENCE.md** | Classes, methods, patches | 15 min |
| **QUICK_REFERENCE.md** | This file | 5 min |

## Important Code Patterns

### Logging
```csharp
if (Main.mod != null)
	Main.mod.Logger.Log("[ComponentName] Message");
if (Main.mod != null)
	Main.mod.Logger.Error("[ComponentName] Error");
```

### Debug Conditional
```csharp
if (Main.settings.debugLogging && Main.mod != null)
	Main.mod.Logger.Log("[ComponentName] Debug info");
```

### Try-Catch in Patches
```csharp
try { /* patch logic */ }
catch (Exception ex)
{
	if (Main.mod != null) Main.mod.Logger.Error("[PatchName] " + ex);
}
```

### Reflection Field Access
```csharp
Image image = __instance.GetType()
	.GetField("fieldName", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance)
	?.GetValue(__instance) as Image;
```

## Key Game API

From Terra Invicta game assemblies:

```csharp
// Councilor stats
CouncilorAttribute.Persuasion        // 7 attributes
CouncilorAttribute.Investigation     // All available as enum

// Mission types
TIMissionTemplate                    // Base mission class
TIMissionOutcome.Success|Failure     // Mission result
TIMissionContext.Unlimited           // Where mission works

// Faction check
faction.IsPlayerControlled           // Is player faction?

// UI components
Image                                // UI image element
Color                                // Color (1.0f, 0.7f, 0.3f, 1.0f)

// State
TICouncilorState                     // Individual councilor
TIGameState                          // Any game state object
TIDateTime                           // Game time
```

## Performance Notes

- **Bonus Lookup**: O(1) dictionary lookup per stat
- **Patch Overhead**: Minimal (postfix only, runs after original)
- **Memory**: Static dictionary, ~1 entry per active bonus
- **UI Updates**: Only when stats displayed

## Next Steps

1. **First time**: Read README.md
2. **Understand design**: Read ARCHITECTURE.md
3. **Make changes**: Refer to DEVELOPER_GUIDE.md & API_REFERENCE.md
4. **Debug issues**: Enable debug logging in settings

## Useful Links

- **Game API**: Explore via dnSpy or ILSpy
- **HarmonyLib**: https://harmony.pardeike.net/
- **Unity/C# Docs**: https://docs.microsoft.com/en-us/dotnet/
- **Mod Manager**: UnityModManager documentation

## Version Info

- **Framework**: .NET Framework 4.8
- **IDE**: Visual Studio 2019+
- **Build**: MSBuild or dotnet CLI
- **Mod Framework**: UnityModManager
- **Patching**: HarmonyLib

## Support

- Check existing patches for similar patterns
- Enable debug logging for issue diagnosis
- Review error logs from game console
- Verify Harmony patches with logging in Load()
