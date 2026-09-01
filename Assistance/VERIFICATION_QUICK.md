# Control Point Cap Exclusion - Quick Verification (v0.5.0)

## 5-Minute Verification Checklist

Use this quick checklist to verify the feature is working. Do these checks in order.

---

## ✅ Pre-Flight Checks (2 min)

- [ ] **DLL Version Check**
  ```powershell
  [System.Reflection.AssemblyName]::GetAssemblyName("C:\Users\Chris\source\repos\Assistance\Assistance\bin\Debug\Assistance.dll").Version
  ```
  Expected: `0 5 0 0`

- [ ] **DLL Deployed**
  - File exists: `C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\Assistance.dll`
  - Timestamp is recent (after you copied it)

- [ ] **Game Closed**
  - Terra Invicta is not running (logs won't update while playing)

- [ ] **Log File Location**
  - `C:\Users\Chris\AppData\LocalLow\Pavonis Interactive\TerraInvicta\Player.log`
  - Can be opened in any text editor

---

## 🎮 In-Game Testing (2 min)

1. **Start Game**
   - Load a save with multiple councilors
   - Let it load completely

2. **Apply Assist Mission**
   - Have Councilor A assist Councilor B
   - Wait for mission to complete
   - Check that B's stats increased

3. **Check Faction Management**
   - View control point maintenance values
   - Take mental note (or screenshot) of the totals
   - **They should NOT increase from baseline**

4. **Complete Next Mission**
   - Have Councilor B complete a mission (any mission)
   - Assists should clear
   - Stats return to normal

---

## 🔍 Log Verification (1 min)

**Open** `Player.log` in Notepad or VS Code

**Search** for: `ControlPointCapacityPatch`

### If you find entries like this:

```
[TICouncilorState_ControlPointCapacityPatch] Councilor 'Name': Original CP=225, Assist Bonus=10, Adjusted CP=215
```

✅ **FEATURE IS WORKING**

### Math check: Is `225 - 10 = 215`? 

- YES → ✅ Correct
- NO → ❌ Math broken

### Also search for: `AssistBonusTracker`

You should see:
```
[AssistBonusTracker] Recorded bonus for '...': ... Total=10
[AssistBonusTracker] GetCouncilorBonusAmount for '...': 10
```

✅ Bonuses being tracked correctly

---

## 📊 Decision Tree

```
START
  │
  ├─ Can you find ANY ControlPointCapacityPatch logs?
  │  ├─ YES → Patch is running
  │  │  ├─ Do logs show: Adjusted CP < Original CP?
  │  │  │  ├─ YES → ✅ FEATURE WORKING
  │  │  │  └─ NO → ❌ Subtraction failed (math error)
  │  │  └─ Is the math correct? Original - Bonus = Adjusted?
  │  │     ├─ YES → ✅ FEATURE WORKING
  │  │     └─ NO → ❌ Math error in code
  │  │
  │  └─ NO → Patch not running
  │     ├─ Can you find AssistBonusTracker logs?
  │     │  ├─ YES → Bonuses tracked, but patch missing
  │     │  │  └─ ❌ Patch target method name wrong
  │     │  └─ NO → Nothing works
  │     │     └─ ❌ Mod didn't load at all
  │
  └─ DONE
```

---

## 🚀 Result Summary

Fill this out after testing:

### Result: ✅ WORKING / ❌ NOT WORKING / ⚠️ PARTIAL

**DLL Version Confirmed:** 0.5.0.0 ( ) Yes ( ) No

**Log Entries Found:** 
- [AssistBonusTracker] ( ) Yes ( ) No
- [TICouncilorState_ControlPointCapacityPatch] ( ) Yes ( ) No

**Math Verified:**
- Original CP - Assist Bonus = Adjusted CP ( ) Correct ( ) Wrong

**In-Game Behavior:**
- CP capacity unchanged during assist ( ) Yes ( ) No
- Bonuses removed when mission ends ( ) Yes ( ) No

**Overall Status:** ( ) Feature Works ( ) Needs Debug

---

## 🐛 If NOT Working

### Step 1: Verify DLL is Fresh
```powershell
# Delete old versions
Remove-Item "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\*.cache" -Force

# Copy new DLL
Copy-Item "C:\Users\Chris\source\repos\Assistance\Assistance\bin\Debug\Assistance.dll" `
  -Destination "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\" -Force

# Verify
[System.Reflection.AssemblyName]::GetAssemblyName("C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\Assistance.dll").Version
```

Should show: `0 5 0 0`

### Step 2: Check Mod Loads
Search `Player.log` for: `Councilor Assist Mission patches applied`

- Found → Mod loaded ✅
- Not found → Mod didn't load ❌

### Step 3: Check Bonuses Tracked
Search for: `[AssistBonusTracker]`

- Found → Tracking works ✅
- Not found → Assist mission not applied or RecordBonus not called ❌

### Step 4: Check Patch Registered
Search for: `[TICouncilorState_ControlPointCapacityPatch]`

- Found → Patch active ✅
- Not found → Harmony patch target wrong ❌

---

## 📝 Common Issues & Fixes

| Issue | Logs Show | Fix |
|-------|-----------|-----|
| No logs at all | Nothing | Reload DLL, check mod loads, verify assist mission exists |
| Bonuses tracked but no patch logs | [AssistBonusTracker] only | Method name wrong; try `get_controlPointCapacity` vs other variants |
| Patch runs but no subtraction | `Adjusted CP = Original CP` | Code error in patch; verify Math.Max logic |
| Math wrong | `225 - 10 = 225` | Should be 215; indicates += or subtraction broken |
| Partial logs | Some entries missing | Check for exceptions in logs; search for "Error" or "Exception" |

---

## ✨ Expected vs Actual

### ✅ Expected (Working)

```
Original CP = 225
Assist Bonus = 10
Adjusted CP = 215

Calculation: 225 - 10 = 215 ✅
```

### ❌ Actual (If Broken)

```
Original CP = 225
Assist Bonus = 10
Adjusted CP = 225

Calculation: 225 - 10 ≠ 225 ❌
```

---

## 🎯 Next Actions

**After completing this verification:**

1. **Share results** with findings and log excerpts
2. **If working:** Feature complete, ready for gameplay testing
3. **If not working:** Share relevant log sections for debugging
4. **If partial:** Identify which component is failing (tracking vs patching)

---

## 📞 For Reporting

When reporting results, include:

```
DLL Version: 0.5.0.0
Mod Loaded: YES / NO
Bonuses Tracked: YES / NO
Patch Active: YES / NO
Math Correct: YES / NO
Overall Status: WORKING / NOT WORKING / PARTIAL

Relevant Log Excerpt:
[paste key logs here]
```

