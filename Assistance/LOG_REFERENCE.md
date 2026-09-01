# Control Point Cap Exclusion - Log Reference (v0.5.0)

## Quick Reference: Expected Log Signatures

This document provides exact log signatures you should see when testing. Use these to verify the feature is working.

---

## 🟢 Success Case: All Systems Working

### When Game Loads (Startup)

```
Councilor Assist Mission patches applied.
```

**What it means:** The mod loaded successfully and all Harmony patches are registered.

---

### When Assist Mission Applies Bonuses

#### Bonus Recording Phase (in order)

```
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Persuasion +5, Total=5
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Investigation +3, Total=8
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Espionage +1, Total=9
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Command +4, Total=13
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Administration +2, Total=15
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Science +0, Total=15
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Security +3, Total=18
```

**Pattern:** `[AssistBonusTracker] Recorded bonus for '[Name]': [Stat] +[Amount], Total=[RunningTotal]`

**One entry per stat bonus** (7 stats total, though some may be 0)

**Running Total field:** Should increase by the bonus amount each line

**Example with 25% assist on councilor with Persuasion=80:**
- First bonus: 80 * 0.25 = 20 → Total=20
- If that's the only stat boosted, final Total=20

---

### When Control Point Capacity Is Queried

This happens **every turn the target councilor exists**, and when faction management screen is viewed.

```
[AssistBonusTracker] GetCouncilorBonusAmount for 'Colonel Smith': 18
[TICouncilorState_ControlPointCapacityPatch] Councilor 'Colonel Smith': Original CP=225, Assist Bonus=18, Adjusted CP=207
```

**Pattern Line 1:** `[AssistBonusTracker] GetCouncilorBonusAmount for '[Name]': [TotalBonus]`

**Pattern Line 2:** `[TICouncilorState_ControlPointCapacityPatch] Councilor '[Name]': Original CP=[Value], Assist Bonus=[Value], Adjusted CP=[Value]`

**Key Values:**
- `Original CP` = Persuasion + Command + Administration (with all other bonuses, but NOT assist)
- `Assist Bonus` = Total from AssistBonusTracker (should match previous line)
- `Adjusted CP` = Original CP - Assist Bonus

**Math Check:** `Adjusted CP = Original CP - Assist Bonus` must be true

**Example:**
```
Original CP=225, Assist Bonus=18, Adjusted CP=207
Check: 225 - 18 = 207 ✅
```

---

### When Target Councilor Completes Next Mission (Cleanup)

```
[AssistBonusTracker] RemoveBonuses called for 'Colonel Smith'
[AssistBonusTracker] GetCouncilorBonusAmount for 'Colonel Smith': 0
[TICouncilorState_ControlPointCapacityPatch] Councilor 'Colonel Smith': Original CP=225, Assist Bonus=0, Adjusted CP=225
```

**Pattern:** Bonus entries disappear, GetCouncilorBonusAmount returns 0

**Next queries will show:**
- `Assist Bonus=0`
- `Adjusted CP = Original CP` (no change)

---

## 🟡 Partial Working: Some Logs Missing

### Issue: No Patch Logs But Bonuses Recorded

```
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Persuasion +5, Total=5
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Command +3, Total=8
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Administration +2, Total=10
[AssistBonusTracker] RemoveBonuses called for 'Colonel Smith'
```

**But NO:**
```
[TICouncilorState_ControlPointCapacityPatch] ...
```

**Diagnosis:** 
- ✅ Bonuses are being tracked
- ❌ Patch is NOT intercepting controlPointCapacity calls
- **Likely Cause:** Harmony patch target name is wrong or not being applied

**Next Step:** Check if `get_controlPointCapacity` is the correct method name. May need to try alternative patch target.

---

### Issue: No Logs At All

**If you see NOTHING like the above in the entire Player.log:**

**Diagnosis:**
- ❌ Mod either didn't load or AssistBonusTracker never called
- ❌ No assist missions were applied during this session
- ❌ Patch registration failed silently

**Next Step:**
1. Verify "Councilor Assist Mission patches applied" appears during startup
2. Apply assist mission and check if ModInfo.json says mission exists
3. Ensure assist percentage > 0% in settings

---

## 🔴 Failure Cases: Wrong Output

### Failure: Adjusted CP is NOT less than Original CP

```
[TICouncilorState_ControlPointCapacityPatch] Councilor 'Colonel Smith': Original CP=225, Assist Bonus=18, Adjusted CP=225
```

**Problem:** `Adjusted CP (225) should be 207, not 225`

**Math is broken:** The subtraction isn't happening

**Possible Causes:**
1. `assistBonus > 0` check is failing (should be true)
2. `__result - assistBonus` calculation is wrong
3. `Math.Max(0, ...)` is clamping to 0 unexpectedly

**Investigation:**
- Check if `assistBonus` value is actually 18 (it is)
- Check if Math.Max logic is sound: `Math.Max(0, 225 - 18) = 207` ✅
- Likely a code issue in the patch

---

### Failure: Total Bonus Not Accumulating

```
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Persuasion +5, Total=5
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Command +3, Total=5  ← WRONG! Should be 8
[AssistBonusTracker] Recorded bonus for 'Colonel Smith': Administration +2, Total=5  ← WRONG! Should be 10
```

**Problem:** Total isn't incrementing

**Math is broken:** The += operator isn't working

**Possible Causes:**
1. Dictionary isn't persisting between calls
2. RecordBonus is creating new instances
3. totalBonusAmounts[councilor] is being reset

**Investigation:**
- Check if totalBonusAmounts dictionary is static (it should be)
- Check if councilor object comparison is reliable

---

### Failure: No Assist Bonus Is Ever Applied

```
[TICouncilorState_ControlPointCapacityPatch] Councilor 'Colonel Smith': Original CP=225, Assist Bonus=0, Adjusted CP=225
```

Even when assist mission is running:

**Problem:** GetCouncilorBonusAmount always returns 0

**Diagnosis:**
- ❌ Bonuses are not being recorded OR
- ❌ Bonuses are recorded but RecordBonus isn't being called by TIMissionEffect_Assist

**Next Step:**
1. Check TIMissionEffect_Assist.ApplyEffect() is calling RecordBonus()
2. Search logs for "Recorded bonus" - if it doesn't appear, ApplyEffect isn't calling it
3. Check if Assist mission is even applying (see "Assisted [name]" in log)

---

## 📋 Log Extraction Commands

### Extract All Assist-Related Logs

**PowerShell:**
```powershell
$logPath = "$env:LOCALAPPDATA\..\LocalLow\Pavonis Interactive\TerraInvicta\Player.log"
Get-Content $logPath | Select-String "AssistBonusTracker|ControlPointCapacity|Assistance"
```

### Count Patch Calls

```powershell
$logPath = "$env:LOCALAPPDATA\..\LocalLow\Pavonis Interactive\TerraInvicta\Player.log"
(Get-Content $logPath | Select-String "TICouncilorState_ControlPointCapacityPatch" | Measure-Object).Count
```

### Find Errors

```powershell
$logPath = "$env:LOCALAPPDATA\..\LocalLow\Pavonis Interactive\TerraInvicta\Player.log"
Get-Content $logPath | Select-String "Error|Exception" | Select-String "Assistance|AssistBonusTracker|ControlPointCapacity"
```

---

## 🎯 Test Scenarios & Expected Logs

### Scenario 1: Simple Single Assist

**Setup:** 
- Councilor A (Persuasion=100, Command=90, Admin=80) has CP=270
- Councilor B assists with 25% assist percentage

**Expected Logs:**

```
[AssistBonusTracker] Recorded bonus for 'A': Persuasion +25, Total=25
[AssistBonusTracker] Recorded bonus for 'A': Investigation +0, Total=25
[AssistBonusTracker] Recorded bonus for 'A': Espionage +0, Total=25
[AssistBonusTracker] Recorded bonus for 'A': Command +22, Total=47
[AssistBonusTracker] Recorded bonus for 'A': Administration +20, Total=67
[AssistBonusTracker] Recorded bonus for 'A': Science +0, Total=67
[AssistBonusTracker] Recorded bonus for 'A': Security +0, Total=67
[AssistBonusTracker] GetCouncilorBonusAmount for 'A': 67
[TICouncilorState_ControlPointCapacityPatch] Councilor 'A': Original CP=270, Assist Bonus=67, Adjusted CP=203
```

**Key observation:** Only Persuasion, Command, Admin are boosted (CP-affecting stats). Assist bonus = 25+22+20 = 67.

---

### Scenario 2: Multiple Stat Boosts

**Setup:** 
- Assist affects all 7 stats

**Expected:**
```
Total should be sum of all 7 stat bonuses
Pattern: 7 lines of RecordBonus, final Total = sum of all
```

---

### Scenario 3: Zero Assist Percentage

**Setup:**
- Assist is 0%

**Expected:**
```
No RecordBonus calls (bonus amounts are 0)
If called, would show Total=0
Patch log shows: Assist Bonus=0, Adjusted CP = Original CP
```

---

## 💡 Interpretation Guide

| Log Pattern | Meaning | Status |
|-------------|---------|--------|
| `[AssistBonusTracker] Recorded bonus` | Bonus being tracked | ✅ Working |
| `Total=[value]` increasing each line | Accumulation working | ✅ Working |
| `GetCouncilorBonusAmount` returns >0 | Bonus retrievable | ✅ Working |
| `[TICouncilorState_ControlPointCapacityPatch]` appears | Patch active | ✅ Working |
| `Adjusted CP < Original CP` | Subtraction working | ✅ Working |
| `Adjusted CP = Original CP - Assist Bonus` | Math correct | ✅ Working |
| No `[AssistBonusTracker]` logs | Bonus not tracked | ❌ Issue |
| `GetCouncilorBonusAmount` returns 0 | Bonus lost/not found | ❌ Issue |
| `Adjusted CP` unchanged | Patch not subtracting | ❌ Issue |
| Exception in logs | Code error | ❌ Issue |

---

## 🔧 Debugging: Enable Maximum Logging

If you need to debug further, modify the patch to log even more:

Current logs only appear if:
1. `Main.enabled` is true
2. Bonus exists (> 0)

To see ALL calls, you'd need to log unconditionally, but that would spam the log heavily.

---

## 📊 Sample Complete Test Log

Here's what a successful full test session should look like:

```
[2026-09-10 15:23:45] Councilor Assist Mission patches applied.
[2026-09-10 15:24:12] [TIMissionEffect_Assist] Applied assist to 'Colonel Smith'
[2026-09-10 15:24:12] [AssistBonusTracker] Recorded bonus for 'Colonel Smith': Persuasion +5, Total=5
[2026-09-10 15:24:12] [AssistBonusTracker] Recorded bonus for 'Colonel Smith': Investigation +0, Total=5
[2026-09-10 15:24:12] [AssistBonusTracker] Recorded bonus for 'Colonel Smith': Espionage +0, Total=5
[2026-09-10 15:24:12] [AssistBonusTracker] Recorded bonus for 'Colonel Smith': Command +3, Total=8
[2026-09-10 15:24:12] [AssistBonusTracker] Recorded bonus for 'Colonel Smith': Administration +2, Total=10
[2026-09-10 15:24:12] [AssistBonusTracker] Recorded bonus for 'Colonel Smith': Science +0, Total=10
[2026-09-10 15:24:12] [AssistBonusTracker] Recorded bonus for 'Colonel Smith': Security +0, Total=10
[2026-09-10 15:24:20] [AssistBonusTracker] GetCouncilorBonusAmount for 'Colonel Smith': 10
[2026-09-10 15:24:20] [TICouncilorState_ControlPointCapacityPatch] Councilor 'Colonel Smith': Original CP=225, Assist Bonus=10, Adjusted CP=215
[2026-09-10 15:25:30] [AssistBonusTracker] RemoveBonuses called for 'Colonel Smith'
[2026-09-10 15:25:30] [AssistBonusTracker] GetCouncilorBonusAmount for 'Colonel Smith': 0
[2026-09-10 15:25:30] [TICouncilorState_ControlPointCapacityPatch] Councilor 'Colonel Smith': Original CP=225, Assist Bonus=0, Adjusted CP=225
```

✅ **All indicators present → Feature working correctly**

