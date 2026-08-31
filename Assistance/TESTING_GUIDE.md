# AssistMission Mod - Quick Start & Testing Guide

## ✅ Pre-Launch Checklist

Before running the mod, verify:

- [ ] `C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\Assistance.dll` exists (11,264 bytes)
- [ ] `C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\ModInfo.json` exists
- [ ] Terra Invicta is installed and launcher works
- [ ] UnityModManager is installed and enabled for Terra Invicta
- [ ] UMM version is 0.33.0.0 or later

---

## 🚀 Launch Steps

### Step 1: Enable Mod in UMM Manager
```
1. Launch Terra Invicta
2. Open UnityModManager (should show during launch splash screen)
   - Or: Ctrl+F10 in-game
3. Look for "Councilor Assist Mission" in the mod list
4. Check the checkbox to ENABLE the mod
5. Mod should show as active (green checkmark)
```

### Step 2: Start a New Game or Load Existing Save
```
- New Game: AssistMission will work automatically
- Existing Save: Mod loads retroactively when enabled
  - Note: Councilors won't have mission until next turn
```

### Step 3: Access Mod Settings
```
1. In-game, open the Mods menu (typically a button on HUD)
2. Click "Councilor Assist Mission"
3. Adjust "Assist Percentage" slider (0-100%, default 25%)
4. Toggle "Enable Assist Mission" if needed
5. Settings auto-save
```

---

## 🎮 How to Use the Mission

### Finding the Mission
```
1. Select any friendly councilor
2. Open mission list (typically right-click or Mission button)
3. Look for "Assist Councilor" mission
   - Icon: Inspire/diplomatic symbol
   - XP reward: 2
   - Cost: None (free)
```

### Executing the Mission
```
1. Click "Assist Councilor" mission
2. Select TARGET councilor (must be friendly, visible, selectable)
3. Confirm mission selection
4. Mission executes (auto-success with TIMissionResolution_Automatic)
5. Wait for completion (depends on mission timing)
6. Result shows: "Assisted [TargetName]: Persuasion +20, Command +18, ..."
```

### Verifying the Assist Worked
```
1. Open target councilor's detail panel
2. Check Stats/Attributes section
3. Stats should be HIGHER than before
   - Example: Persuasion was 60, now 80 (if 25% assist from 80-stat source)
4. Return to source councilor - their stats UNCHANGED (they only donated bonus)
```

---

## 🧪 Test Cases

### Test Case 1: Basic Assist Functionality
```
Objective: Verify stat transfer works
Steps:
  1. Select Councilor A (Persuasion 80)
  2. Execute Assist mission targeting Councilor B
  3. Check Councilor B's Persuasion
  4. Expected: B's Persuasion increased by ~20 (25% of 80)
Status: ✓ PASS if stats increased, ✗ FAIL if unchanged
```

### Test Case 2: GUI Configuration
```
Objective: Verify in-game settings work
Steps:
  1. Open Mod settings in-game (Mods → Assist Mission)
  2. Drag "Assist Percentage" slider to 50%
  3. Close settings
  4. Execute assist again
  5. Check bonus amount
  6. Expected: Bonus now ~50% of source stat (doubled from Test 1)
Status: ✓ PASS if percentage changes affect bonus, ✗ FAIL if no change
```

### Test Case 3: Multiple Assists
```
Objective: Verify multiple assists stack
Steps:
  1. Execute assist on target with 50% assistance
  2. Execute assist again on same target with another councilor
  3. Check target's stats
  4. Expected: Both assists applied (stats higher than single assist)
Status: ✓ PASS if both bonuses added, ✗ FAIL if only one applied
```

### Test Case 4: Different Stats
```
Objective: Verify all 7 stats transfer
Steps:
  1. Find councilor with diverse high stats (all 60+)
  2. Execute assist
  3. Check result message
  4. Expected: Message shows all 7 stats with bonuses
	 "Assisted Name: Persuasion +15, Investigation +14, Espionage +16, ..."
Status: ✓ PASS if all 7 appear, ✗ FAIL if some missing
```

### Test Case 5: Zero/Low Stats
```
Objective: Verify minimum 1-point bonus
Steps:
  1. Find councilor with some stats at 0-3
  2. Execute assist from different councilor
  3. Check low stats
  4. Expected: Stat 0 → 0, Stat 1-3 → +1 minimum
Status: ✓ PASS if minimum applied, ✗ FAIL if no transfer
```

### Test Case 6: Settings Persistence
```
Objective: Verify settings save between games
Steps:
  1. Set assist percentage to 75% in-game
  2. Exit to main menu
  3. Start new game or reload
  4. Open mod settings again
  5. Expected: Assist percentage still 75%
Status: ✓ PASS if setting persists, ✗ FAIL if reset to 25%
```

### Test Case 7: Mod Toggle
```
Objective: Verify enable/disable works
Steps:
  1. In UMM manager, toggle AssistMission OFF
  2. In-game, mission should NOT appear in councilor's mission list
  3. Re-enable mod in UMM manager
  4. Mission should reappear
  5. Expected: Mission presence matches mod enabled state
Status: ✓ PASS if toggle works, ✗ FAIL if mission always present
```

---

## 🔍 What to Check if Tests Fail

### "Assist Councilor" mission doesn't appear
```
Diagnostics:
  1. Check Player.log: tail -f "C:\Users\Chris\AppData\LocalLow\Pavonis Interactive\TerraInvicta\Player.log"
  2. Search for: "Assist mission registered"
  3. If found: Mission should exist - try reloading game
  4. If NOT found:
	 - Mod didn't load (check ModInfo.json syntax)
	 - Patch didn't apply (check Harmony error logs)
	 - Try: Verify Assistance.dll in correct folder
```

### Stats don't transfer
```
Diagnostics:
  1. Check game log for errors with "Assist"
  2. Verify TIMissionEffect_Assist runs (add logging if needed)
  3. Verify mission resolves to "Success" outcome (not Failure)
  4. Verify source councilor has stats > 0
  5. Try: Simple case with both councilors visible, same faction
```

### GUI slider doesn't work
```
Diagnostics:
  1. Check if mod menu opens at all
  2. Verify settings file exists: Settings.xml (should be created auto)
  3. Check console for GUILayout errors
  4. Try: Close/reopen mod settings menu
```

### Game crashes when running assist
```
Diagnostics:
  1. Most likely: Exception in TIMissionEffect_Assist
  2. Check Player.log for full exception stack trace
  3. Common causes:
	 - null councilor reference (target not found)
	 - Missing attribute enum value
	 - Invalid stat bonuses
  4. Try: Report error message to developer
```

---

## 📊 Performance & Stability

### Expected Behavior
- Mod loads in < 1 second
- Assist mission execution < 500ms
- No FPS impact when mission not running
- Settings load/save < 100ms
- GUI updates smooth at 60 FPS

### Known Limitations
- Mission effects apply immediately (no animation)
- Stats not capped by mod (game engine enforces caps)
- No party/squad-level assists yet
- Assistance is one-way (source isn't boosted)

---

## 🐛 Bug Report Template

If something goes wrong, use this format:

```
Title: [Brief description]
Version: 0.1.0
Game Version: 1.0.38+
UMM Version: [your version]

Steps to Reproduce:
1. [First step]
2. [Second step]
...

Expected Result:
[What should happen]

Actual Result:
[What actually happened]

Logs/Screenshots:
[Paste relevant lines from Player.log or attach screenshot]

Additional Notes:
[Any other context]
```

---

## 🔧 If Mod Needs Rebuild

(For developers resuming the project)

```powershell
# 1. Open Visual Studio
cd 'C:\Users\Chris\source\repos\Assistance\'
# 2. Open Assistance.slnx
# 3. Build → Build Solution (or Rebuild)
# 4. Check output: bin\Debug\Assistance.dll created
# 5. Copy to mods folder:
Copy-Item -Path 'Assistance\bin\Debug\Assistance.dll' -Destination 'C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\' -Force
# 6. Restart Terra Invicta
```

---

## 📞 Troubleshooting Checklist

| Symptom | Root Cause | Fix |
|---------|-----------|-----|
| Mod not in list | Not deployed or wrong folder | Copy DLL to mods folder |
| Mission missing | Patch didn't apply | Restart game, check logs |
| Stats unchanged | Effect didn't run | Check mission outcome is Success |
| GUI crashes | Layout error | Check OnGUI method implementation |
| Settings lost | Not inheriting ModSettings | Check Settings.cs inheritance |
| DLL not loading | Assembly version mismatch | Rebuild, verify DLL size ~11KB |

---

## 📝 Session Log Template

For next AI developer session:

```
Date: [Today]
Session Focus: [Testing / Debugging / Enhancement]

Starting State:
- Mod version: 0.1.0
- Last tested: [Date]
- Known issues: [None yet]

Work Completed:
1. [Task]
2. [Task]

Test Results:
- Test 1: [PASS/FAIL]
- Test 2: [PASS/FAIL]

Issues Found:
- [Issue]

Recommendations for Next Session:
- [Recommendation]

Files Modified:
- [File]
```

---

**Ready to Test!** 🎮

Launch the game and enjoy the new Assist Mission feature. Report any issues using the bug report template.
