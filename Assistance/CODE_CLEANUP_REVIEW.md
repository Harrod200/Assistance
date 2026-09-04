# Code Cleanup Review - Assistance Mod
**Date:** 2026-09-11  
**Status:** Analysis Complete - Awaiting Approval

---

## Executive Summary

Found **8 items** requiring cleanup across 6 files:
- **3 unused using statements** - Safe to remove
- **3 bare catch blocks** - Should be refactored to catch specific exceptions
- **Defensive code in UI patch** - Questionable necessity, review recommended
- **GUI helper methods** - Minor issue, currently in use

---

## Detailed Findings

### 1. UNUSED USING STATEMENTS (Remove)

#### TIMissionModifier_AssistStat.cs:2
```csharp
using System.Text;  // ❌ UNUSED - Never referenced in file
```
**Status:** Safe to remove  
**Reason:** `System.Text` is imported but the file uses no types from it (no StringBuilder, no string manipulation beyond string literals).  
**Action:** Remove line 2

---

#### TIMissionCondition_MyFactionCouncilor.cs:1
```csharp
using System;  // ❌ UNUSED - Never referenced in file
```
**Status:** Safe to remove  
**Reason:** File has no exception handling or types from System namespace. Bare `catch` blocks (if present) don't require System.  
**Action:** Remove line 1

---

#### TIMissionCondition_PlayerFactionOnly.cs:1
```csharp
using System;  // ❌ UNUSED - Never referenced in file
```
**Status:** Safe to remove  
**Reason:** Same as above - no System types are used in this file.  
**Action:** Remove line 1

---

### 2. BARE CATCH BLOCKS (Refactor)

#### TIMissionModifier_AssistStat.cs:34
```csharp
catch              // ❌ BAD PRACTICE - Catches all exceptions with no type
{
	return "Persuasion";
}
```
**Status:** Should be refactored  
**Reason:** Bare catch blocks catch `Exception` and `SystemException` indiscriminately, hiding bugs. Should catch only expected exceptions.  
**Recommended Fix:**
```csharp
catch (Exception ex)  // ✅ Explicit, can log if needed
{
	Main.mod?.Logger?.Log("[AssistMission] Error in TIMissionModifier_AssistStat.displayName: " + ex.Message);
	return "Persuasion";
}
```

---

#### TIMissionModifier_AssistFlat.cs:35
```csharp
catch              // ❌ BAD PRACTICE - Bare catch blocks
{
	// Ultimate fallback
	return "Bonus";
}
```
**Status:** Should be refactored  
**Reason:** Same as above - indiscriminate exception handling.  
**Recommended Fix:**
```csharp
catch (Exception ex)  // ✅ Explicit
{
	Main.mod?.Logger?.Log("[AssistMission] Error in TIMissionModifier_AssistFlat.displayName: " + ex.Message);
	return "Bonus";
}
```

---

#### TICouncilorState_GetPossibleMissionListPatch.cs:46
```csharp
catch  // ❌ BAD PRACTICE - Silent failure with no logging
{
	// Silently fail to avoid spam
}
```
**Status:** Should be refactored  
**Reason:** Completely swallows exceptions with no logging. This makes debugging impossible if the patch fails.  
**Context:** Comment says "avoid spam" but this is overly cautious. A single log message is better than silent failure.  
**Recommended Fix:**
```csharp
catch (Exception ex)  // ✅ At least log it once
{
	if (Main.mod != null)
		Main.mod.Logger.Error("[Patch] TICouncilorState.GetPossibleMissionList error: " + ex.Message);
}
```

---

### 3. DEFENSIVE CODE REVIEW

#### CouncilorMissionCanvasController_UpdateModifierListPatch.cs (Full File)

**Issue:** File contains multiple defensive layers that appear to be legacy safety code, potentially unnecessary.

**Structure Analysis:**
1. **Loc_T_NullSafetyPatch** - Catches null returns from localization and provides fallback
2. **CouncilorMissionCanvasController_UpdateModifierListPatch** - Wraps UpdateModifierList with Prefix/Postfix guards

**Problems:**
- `TargetMethod()` returns `null` if method can't be found (line 64), causing the patch to silently fail
- Prefix/Postfix do minimal real work (mostly validation + logging)
- The Postfix only logs success - no actual fix is applied

**Assessment:**
- ✅ **Loc_T_NullSafetyPatch** - KEEP (useful safety layer for localization)
- ⚠️ **CouncilorMissionCanvasController_UpdateModifierListPatch** - REVIEW
  - If the method is reliably found, the guards are overkill
  - If the method might be missing, returning null is dangerous (silent failure)
  - **Recommendation:** Either strengthen error handling OR remove if no longer needed

**Current Risk:**
```csharp
if (method != null)
{
	Debug.Log("[AssistMission] Successfully found UpdateModifierList method for patching");
	return method;
}

Debug.LogWarning("[AssistMission] Could not find UpdateModifierList method for patching");
return null;  // ❌ This causes the patch to silently fail
```

**Better Approach (if keeping):**
```csharp
if (method != null)
{
	Main.mod?.Logger?.Log("[AssistMission] Patching UpdateModifierList successfully");
	return method;
}

Main.mod?.Logger?.Error("[AssistMission] CRITICAL: Could not find UpdateModifierList method! Mod may not work correctly.");
return null;
```

---

### 4. MINOR: GUI HELPER METHODS

#### Main.cs:58-65
```csharp
private static float DrawNamedFloat(string label, float value, float labelWidth)
{
	GUILayout.BeginHorizontal(new GUILayoutOption[0]);
	GUILayout.Label(label, new GUILayoutOption[] { GUILayout.Width(labelWidth) });
	float num = Main.DrawFloat(value, 90f);
	GUILayout.EndHorizontal();
	return num;
}

private static float DrawFloat(float value, float width)
{
	string text = GUILayout.TextField(value.ToString("F1"), new GUILayoutOption[] { GUILayout.Width(width) });
	float num;
	if (float.TryParse(text, out num))
	{
		return num;
	}
	return value;
}
```

**Status:** Currently in use, not unused  
**Assessment:** These are small helper methods for UI layout. Not a problem, but could be inlined if desired (readability vs. DRY tradeoff).  
**Recommendation:** Keep as-is (improves readability of OnGUI method)

---

## Cleanup Priority

| Priority | Item | File | Action | Impact |
|----------|------|------|--------|--------|
| 🔴 High | Unused `using System.Text;` | TIMissionModifier_AssistStat.cs | Remove line 2 | None - unused |
| 🔴 High | Unused `using System;` | TIMissionCondition_MyFactionCouncilor.cs | Remove line 1 | None - unused |
| 🔴 High | Unused `using System;` | TIMissionCondition_PlayerFactionOnly.cs | Remove line 1 | None - unused |
| 🟠 Medium | Bare catch block | TIMissionModifier_AssistStat.cs | Change to `catch (Exception ex)` + log | Better error tracking |
| 🟠 Medium | Bare catch block | TIMissionModifier_AssistFlat.cs | Change to `catch (Exception ex)` + log | Better error tracking |
| 🟠 Medium | Bare catch block (silent) | TICouncilorState_GetPossibleMissionListPatch.cs | Add logging | Easier debugging if patch fails |
| 🟡 Low | Defensive UI patch | CouncilorMissionCanvasController_UpdateModifierListPatch.cs | Review necessity or strengthen error handling | Code clarity |
| ⚪ None | GUI helpers | Main.cs | Keep as-is | None - currently in use |

---

## Recommendations

### Phase 1 (Safe): Remove Unused Using Statements
✅ No functional impact  
✅ Improves code cleanliness  

Files to edit:
1. `TIMissionModifier_AssistStat.cs` - Remove `using System.Text;`
2. `TIMissionCondition_MyFactionCouncilor.cs` - Remove `using System;`
3. `TIMissionCondition_PlayerFactionOnly.cs` - Remove `using System;`

### Phase 2 (Recommended): Fix Bare Catch Blocks
✅ Follows C# best practices  
✅ Improves debugging and error tracking  

Files to edit:
1. `TIMissionModifier_AssistStat.cs` - Line 34: `catch` → `catch (Exception ex)`
2. `TIMissionModifier_AssistFlat.cs` - Line 35: `catch` → `catch (Exception ex)`
3. `TICouncilorState_GetPossibleMissionListPatch.cs` - Line 46: Add logging

### Phase 3 (Optional): Review UI Defensive Code
⚠️ Requires investigation of whether the patch is still needed  
⚠️ Determine if defensive layers are worth keeping  

### Phase 4 (Not Recommended): GUI Helpers
❌ Leave as-is - currently in use and improves code readability

---

## Next Steps

Awaiting approval to proceed with cleanup. Please confirm which phases to implement:
- [ ] Phase 1: Remove unused using statements
- [ ] Phase 2: Fix bare catch blocks
- [ ] Phase 3: Review UI defensive code
- [ ] All of the above
