# Control Point Cap Bonus Exclusion - Testing Guide (v0.5.0)

## Overview

This guide helps you verify that the Assist mission bonuses **do NOT** inflate a faction's control point capacity. The feature was implemented in v0.5.0 to prevent unfair gameplay advantages.

---

## 🎯 What We're Testing

**Feature:** Control Point Cap Bonus Exclusion

**Expected Behavior:**
- When an Assist mission grants +5 Persuasion, +3 Command, +2 Administration (total +10)
- The councilor's stats increase by those amounts ✅
- BUT the control point capacity should NOT increase ✅
- Formula: `CP_cap_after = (S1 + S2 + S3) - 10 = CP_cap_before`

**Why It Matters:**
- Without this fix: Extra 10 "free" control point maintenance capacity (unfair advantage)
- With this fix: No capacity boost, bonuses are purely stat improvements

---

## 📋 Pre-Test Checklist

Before testing, ensure:

- [ ] DLL version is 0.5.0 (check: `Assistance\bin\Debug\Assistance.dll`)
- [ ] Mod is deployed to game folder: `C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\Assistance.dll`
- [ ] Game is closed (important: log won't update while game is running)
- [ ] You have a save game with councilors ready to test
- [ ] You know the save location for quick access

---

## 🕹️ Test Procedure

### Phase 1: Baseline Measurement

1. **Load your save game** in Terra Invicta
2. **Open faction management** (Council view or faction panel)
3. **Note the Control Point Maintenance values:**
   - Record total "free" control points per turn
   - Record maintenance cost before assist mission
   - Record any control point overage penalties

4. **Identify test councilor:**
   - Pick a councilor with reasonable stats (Persuasion, Command, Administration)
   - Record their current individual stats (at least the three CP-contributing stats)
   - Note: "Assisted by" field should be empty

5. **Check game logs:**
   - Open `C:\Users\Chris\AppData\LocalLow\Pavonis Interactive\TerraInvicta\Player.log`
   - Scroll to the end
   - Search for any existing assist-related logs
   - Note the current log timestamp

### Phase 2: Apply Assist Mission

6. **Assign Assist mission:**
   - Have another councilor (helper) choose "Assist" mission
   - Target the test councilor
   - Ensure mission resolves successfully (Contested resolution)
   - Wait for mission to complete

7. **Immediately check target councilor's stats:**
   - They should now show elevated stats
   - UI should display the bonus (if displayed)
   - Example: Persuasion was 80, now shows 85

8. **Check faction maintenance:**
   - Go back to faction management
   - Compare control point values to baseline
   - **CRITICAL:** Total maintenance cost should NOT change
   - **If working correctly:** No increase in free CP maintenance capacity

---

## 🔍 Log Verification

This is the MOST IMPORTANT part. The logs will tell you exactly what's happening.

### Check the Logs

**Open:** `C:\Users\Chris\AppData\LocalLow\Pavonis Interactive\TerraInvicta\Player.log`

**Search for:** `[AssistBonusTracker]` and `[TICouncilorState_ControlPointCapacityPatch]`

### Expected Log Output (Success Scenario)

**When Assist mission applies:**
```
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Persuasion +5, Total=5
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Command +3, Total=8
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Administration +2, Total=10
```

**When controlPointCapacity is queried (every turn update):**
```
[AssistBonusTracker] GetCouncilorBonusAmount for 'Colonel Smith': 10
[TICouncilorState_ControlPointCapacityPatch] Councilor 'Colonel Smith': Original CP=225, Assist Bonus=10, Adjusted CP=215
```

**What this means:**
- Base CP capacity: 225
- Total assist bonus: 10
- After subtraction: 215 (unchanged, as intended)

**When mission completes (councilor finishes next mission):**
```
[AssistBonusTracker] GetCouncilorBonusAmount for 'Colonel Smith': 0
[TICouncilorState_ControlPointCapacityPatch] Councilor 'Colonel Smith': Original CP=225, Assist Bonus=0, Adjusted CP=225
```

---

## ✅ Success Indicators

The feature is working correctly if:

1. **Logs show patch is active:**
   - You see `[TICouncilorState_ControlPointCapacityPatch]` entries
   - You see `[AssistBonusTracker]` entries

2. **Bonus amounts are tracked:**
   - Individual stat bonuses summed correctly (e.g., 5+3+2=10)
   - `GetCouncilorBonusAmount` returns the correct total

3. **CP cap is reduced:**
   - Patch log shows: `Assist Bonus=10`
   - Patch log shows: `Adjusted CP` is less than `Original CP`
   - Difference equals the bonus amount

4. **Faction maintenance unchanged:**
   - In-game control point maintenance values don't increase during assist
   - No new penalties appear
   - No "free" CP boost during assist period

5. **Cleanup works:**
   - After mission completes, logs show `Assist Bonus=0`
   - Bonus entries disappear from tracker
   - CP capacity returns to normal

---

## ❌ Failure Scenarios & Troubleshooting

### Scenario 1: No Debug Logs Appear

**Problem:** You don't see any `[AssistBonusTracker]` or `[TICouncilorState_ControlPointCapacityPatch]` entries

**Possible Causes:**
1. Old DLL cached - game loaded previous version
2. Patch target method name wrong
3. Harmony patch registration failed

**Solution:**
1. Check DLL version: `[System.Reflection.AssemblyName]::GetAssemblyName("C:\Users\Chris\source\repos\Assistance\Assistance\bin\Debug\Assistance.dll").Version`
2. Should show: `0 5 0 0`
3. If old version, delete: `C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\*.cache`
4. Copy fresh DLL again
5. Restart game

### Scenario 2: Bonuses Are Recorded But No Patch Logs

**Problem:** You see `[AssistBonusTracker]` logs but NO `[TICouncilorState_ControlPointCapacityPatch]` logs

**Possible Causes:**
1. Harmony patch isn't intercepting the property getter
2. Method name `get_controlPointCapacity` is wrong
3. Patch is being suppressed or not registered

**Solution:**
1. Check if patch ever gets called - search logs for "ControlPointCapacity"
2. If absolutely nothing, the patch target name is wrong
3. Alternative: The game may cache the controlPointCapacity value

### Scenario 3: Logs Show Bonus Not Being Subtracted

**Problem:** Logs show: `Original CP=225, Assist Bonus=10, Adjusted CP=225` (no change)

**Possible Causes:**
1. Math.Max check is preventing subtraction
2. assistBonus value is 0 or negative
3. Logic is skipped somehow

**Solution:**
1. Check if `assistBonus > 0` condition is true in logs
2. Verify GetCouncilorBonusAmount returns correct value
3. Look for exception messages in logs

### Scenario 4: In-Game CP Still Increases

**Problem:** Logs look correct but faction management still shows higher CP maintenance capacity

**Possible Causes:**
1. Control point cap calculation is cached and not recalculated
2. Different method path is used for CP cap in faction calculations
3. Patch is working but game uses a different property/method

**Investigation:**
1. Check exact logs - is patch being called?
2. Look for any game errors after Assist mission applies
3. Check if faction actually benefits or if it's just display bug

---

## 📊 Detailed Testing Checklist

| Item | Check | Status | Notes |
|------|-------|--------|-------|
| DLL deployed | v0.5.0 correct version | ☐ | |
| Game started fresh | No cache issues | ☐ | |
| Test councilor identified | Has base CP capacity | ☐ | Record baseline |
| Assist mission applied | Bonuses visible in UI | ☐ | Watch for stats increase |
| Debug logs appear | See [AssistBonusTracker] | ☐ | Check Player.log |
| Bonuses recorded | Total bonus = sum | ☐ | e.g., 5+3+2=10 |
| Patch called | See [TICouncilorState_ControlPointCapacityPatch] | ☐ | |
| CP subtracted in logs | Adjusted < Original | ☐ | |
| Faction CP unchanged | No increase in maintenance | ☐ | In-game verification |
| Cleanup on mission end | Assist Bonus=0 | ☐ | After next mission |

---

## 🐛 If Everything Fails

**Step-by-step debugging:**

1. **Enable verbose logging:**
   - Check if mod has verbose logging mode
   - Current implementation should already be verbose

2. **Check for patch application errors:**
   - Search logs for "Harmony" or "patch" errors
   - Look for "Failed to apply patches" message

3. **Verify mod is loading:**
   - Look for: "Councilor Assist Mission patches applied"
   - Should appear during game startup

4. **Test with simple case:**
   - Use highest-stat councilor
   - Apply maximum assist percentage (100%)
   - Easier to see the difference

5. **Check game version compatibility:**
   - ModInfo.json shows: `"GameVersion": "1.0.38"`
   - Your game should be 1.0.38 or newer

---

## 📝 Testing Report Template

Use this to document your findings:

```
Test Date: [YYYY-MM-DD]
Game Version: [e.g., 1.0.53]
Save Game: [name/location]
Test Councilor: [Name and stats]
Assist Duration: [e.g., 1 turn]

Baseline:
- Total CP Maintenance: [value]
- Test Councilor CP Cap: [value]

After Assist Applied:
- Total CP Maintenance: [value]
- Test Councilor CP Cap: [should be same as baseline]
- Assist Bonus Amount (from logs): [value]

Logs Present:
- [AssistBonusTracker] entries: YES / NO
- [TICouncilorState_ControlPointCapacityPatch] entries: YES / NO

Result:
- CP cap correctly excluded from bonus: YES / NO / UNCLEAR
- Feature working as intended: YES / NO / NEEDS INVESTIGATION

Notes:
[Any observations or issues]
```

---

## 🎮 Additional Testing Ideas

1. **Multiple Assists:** Have multiple councilors assist the same target
   - Verify bonuses stack correctly
   - Patch should handle cumulative totals

2. **Partial Missions:** Test assist with different percentage settings
   - 25% (default)
   - 50%
   - 100%
   - Verify bonus calculation matches assist percentage

3. **Mixed Scenarios:** Test with other mods or effects
   - Traits that boost stats
   - Organizations providing bonuses
   - All should be tracked separately

4. **Performance Test:** Monitor for slowdowns
   - Patch adds one subtraction per CP cap calculation
   - Should be imperceptible

---

## 📞 Next Steps

After testing:

1. **If working:** Logs show success indicators → Feature complete ✅
2. **If partially working:** Some logs appear but not all → Needs investigation 🔍
3. **If failing:** No logs or wrong values → Patch target name wrong ❌

Post your findings with:
- DLL version number
- Relevant log excerpts
- In-game observations
- Which success indicators were met

This will help pinpoint the exact issue!

