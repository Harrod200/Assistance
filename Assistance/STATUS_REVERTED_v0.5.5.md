# Revert Complete: v0.5.5 Stable

## ✅ Status Summary

| Aspect | Status |
|--------|--------|
| Version | v0.5.5 |
| Build | ✅ Successful |
| Deployed | ✅ To game folder |
| Git | ✅ Clean, up to date |
| Ready | ✅ For testing |

---

## What Happened

**v0.6.0 Plan:** Implement dual-patch strategy to exclude bonuses from vanilla effects while keeping them in contested missions

**Discovery:** Assist mission uses Automatic resolution (no modifiers) - the contested bonus strategy doesn't apply

**Solution:** Reverted to v0.5.5 working state

---

## Architecture (v0.5.5)

### How Assist Mission Works
1. **Apply Bonuses:** TIMissionEffect_Assist grants 25% of source's stats to target
2. **Track Bonuses:** AssistBonusTracker.RecordBonus() stores bonus amounts per stat
3. **Adjust CP:** TIFactionState_ControlPointMaintenanceCapPatch subtracts CP-affecting bonuses from faction cap
4. **Remove Bonuses:** On target's next mission completion, TICouncilorState_CompleteMissionPatch removes bonuses

### Bonus Behavior
- ✅ Applied immediately when mission resolves
- ✅ Affect ALL calculations (vanilla effects included)
- ✅ Removed when target completes next mission
- ✅ Tracked for CP cap adjustment

---

## File Status

### Core Mission Files
- ✅ TIMissionTemplate_Assist.cs - Automatic resolution, no modifiers
- ✅ TIMissionEffect_Assist.cs - Applies bonuses to target
- ✅ TIMissionModifier_AssistStat.cs - Unused (Automatic has no modifiers)

### Tracking & Adjustment
- ✅ AssistBonusTracker.cs - Tracks bonuses, calculates CP adjustment
- ✅ TICouncilorState_CompleteMissionPatch.cs - Removes bonuses on mission completion
- ✅ TIFactionState_ControlPointMaintenanceCapPatch.cs - Adjusts faction CP cap

### UI & Patches
- ✅ CouncilorMissionCanvasController_UpdateModifierListPatch.cs - UI crash prevention
- ✅ TICouncilorState_GetPossibleMissionListPatch.cs - Filters from AI councilors
- ✅ AssistMissionBootstrapPatch.cs - Registers mission

### Settings
- ✅ Main.cs - Entry point, debug logging toggle
- ✅ Settings.cs - Configuration storage
- ✅ TIMissionTemplate.en - Localization

---

## Key Features (v0.5.5)

✅ **Assist Mission**
- Free support mission
- Shares 0-100% of source councilor's stats with target
- 7 stats affected: Persuasion, Investigation, Espionage, Command, Administration, Science, Security
- Bonuses disappear when target completes next mission

✅ **Control Point Adjustment** (v0.5.2)
- CP-affecting stats (Persuasion, Command, Administration) reduce faction CP cap
- Adjustment applied at faction level in GetControlPointMaintenanceFreebieCap()

✅ **Debug Logging** (v0.5.3+)
- Toggleable via UMM settings
- Tracks bonus application, removal, and bonus amounts

✅ **AI Restriction** (v0.4.2+)
- Mission only available to player factions
- Filtered from AI councilor mission lists

---

## Deployment

**Deployed To:** `C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\`
- ✅ Assistance.dll (v0.5.5)
- ✅ ModInfo.json
- ✅ TIMissionTemplate.en
- ✅ Cache cleared

**Deployed From:** `C:\Users\Chris\source\repos\Assistance\Assistance\bin\Debug\`

---

## What Changed from v0.6.0 Attempt

**Removed:**
- TICouncilorState_GetAttributePatch.cs (new central patch)
- All debug logging enhancements for the patch
- Bonus re-inclusion logic in TIMissionModifier_AssistStat

**Restored:**
- Original TIMissionModifier_AssistStat (unused due to Automatic resolution)
- Original TIMissionEffect_Assist (simple bonus application)
- Original TICouncilorState_CompleteMissionPatch (simple bonus removal)

---

## Next Testing

1. Launch Terra Invicta
2. Enable Assist Mission in UMM
3. Optionally toggle Debug Logging to ON in mod settings
4. Load game save
5. Start Assist mission
6. Verify:
   - ✓ Mission resolves successfully
   - ✓ Target receives bonuses
   - ✓ Bonuses removed on next mission completion
   - ✓ No errors in log

---

## Documentation

Added: `REVERT_v0.6.0_TO_v0.5.5.md` - Details of revert and why

All v0.6.0 documentation removed (incompatible approach)

---

**Status: ✅ READY FOR TESTING WITH v0.5.5**

