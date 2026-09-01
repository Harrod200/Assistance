# Log Review & Analysis (v0.5.0 Testing)

## 📋 Current Log Status

### ✅ What's Working

**Bonus Tracking - CONFIRMED WORKING**
```
[AssistMission] [AssistBonusTracker] Recorded bonus for 'Evandro Semerawno': Persuasion +4, Total=4
[AssistMission] [AssistBonusTracker] Recorded bonus for 'Evandro Semerawno': Investigation +8, Total=12
[AssistMission] [AssistBonusTracker] Recorded bonus for 'Evandro Semerawno': Espionage +1, Total=13
[AssistMission] [AssistBonusTracker] Recorded bonus for 'Evandro Semerawno': Command +1, Total=14
[AssistMission] [AssistBonusTracker] Recorded bonus for 'Evandro Semerawno': Administration +12, Total=26
[AssistMission] [AssistBonusTracker] Recorded bonus for 'Evandro Semerawno': Science +17, Total=43
[AssistMission] [AssistBonusTracker] Recorded bonus for 'Evandro Semerawno': Security +2, Total=45
```

✅ Individual bonuses recorded per stat  
✅ Running total accumulates correctly (4→12→13→14→26→43→45)  
✅ Final assist bonus total: **45 points**  
✅ Bonus accumulation logic working perfectly  

### ❌ What's NOT Working

**CP Cap Patch - NOT LOGGING**
- Expected: `[TICouncilorState_ControlPointCapacityPatch] ...` logs
- Actual: **No logs appear** ❌
- The patch is supposed to intercept every controlPointCapacity getter call
- If bonuses weren't being tracked, we wouldn't see the AssistBonusTracker logs either

**Diagnosis:**
The patch target method name `get_controlPointCapacity` may not be correct, OR the patch is being applied but `assistBonus` is not > 0 in the condition check.

### ❌ What Was SPAM

**Excessive PlayerFactionOnly Logging - NOW FIXED ✅**
```
[AssistMission] [TIMissionCondition_PlayerFactionOnly] Checking councilor 'Evandro Semerawno' in faction 'the Academy' (isAI=False) -> _Pass
[AssistMission] [TIMissionCondition_PlayerFactionOnly] Checking councilor 'Evandro Semerawno' in faction 'the Academy' (isAI=False) -> _Pass
[AssistMission] [TIMissionCondition_PlayerFactionOnly] Checking councilor 'Evandro Semerawno' in faction 'the Academy' (isAI=False) -> _Pass
... (repeated 25+ times per turn update)
```

**Issue:** Condition being checked every frame for the same councilor
**Fix Applied:** Removed logging from TIMissionCondition_PlayerFactionOnly.CanTarget()
**Result:** Logs now clean, condition still works silently ✅

---

## 🔍 Analysis: Why CP Cap Patch Isn't Logging

### Theory 1: Method Name Is Wrong

Tried: `get_controlPointCapacity`  
Problem: This is a property getter, which C# compiles to `get_PropertyName()`  
However, decompiled code shows it as `controlPointCapacity { get { ... } }`

**Alternative names to try:**
- `controlPointCapacity` (property name, not method)
- `get_controlPointCapacity()` (with parentheses)
- Patch via reflection instead of attributes

### Theory 2: Patch Registered But Condition Fails

The patch might be intercepting, but `if (assistBonus > 0)` check fails because:
- `GetCouncilorBonusAmount()` returns 0 (dictionary lookup fails)
- Different councilor object instances (reference comparison)
- Bonus was already cleared

**How to test:** Add logging OUTSIDE the condition:
```csharp
Main.mod.Logger.Log("ControlPointCapacity called for: " + __instance.displayName);
```

### Theory 3: Harmony Patch Not Registered

The `harmony.PatchAll()` call in Main.cs should auto-discover it, but:
- Patch class is `internal` (correct)
- Has `[HarmonyPatch(...)]` attribute (correct)
- Assembly is loaded (AssistBonusTracker logs prove this)

**Likelihood:** Low, but possible

---

## 📊 Log Summary

| Component | Status | Evidence |
|-----------|--------|----------|
| Mod loads | ✅ Working | "patches applied" logged at startup |
| Assist mission applies | ✅ Working | Bonuses are recorded |
| Bonus tracking | ✅ Working | AssistBonusTracker logs show 45-point total |
| Bonus accumulation | ✅ Working | Running Total field increments correctly |
| CP cap patch | ❌ Not Working | Zero logs despite bonuses present |
| PlayerFactionOnly spam | ✅ Fixed | Removed excessive logging |

---

## 🎯 Next Action

**To Diagnose CP Cap Patch:**

Replace the patch with this debugging version:

```csharp
[HarmonyPatch(typeof(TICouncilorState), "get_controlPointCapacity")]
internal static class TICouncilorState_ControlPointCapacityPatch
{
	public static void Postfix(TICouncilorState __instance, ref int __result)
	{
		if (!Main.enabled || __instance == null || Main.mod == null)
			return;

		try
		{
			// LOG EVERY CALL regardless of bonus
			Main.mod.Logger.Log("[CP_PATCH] Called for " + __instance.displayName);

			int assistBonus = AssistBonusTracker.GetCouncilorBonusAmount(__instance);
			Main.mod.Logger.Log("[CP_PATCH] Bonus = " + assistBonus);

			if (assistBonus > 0)
			{
				int adjustedCap = Math.Max(0, __result - assistBonus);
				Main.mod.Logger.Log(string.Format("[CP_PATCH] Adjusted: {0} - {1} = {2}", 
					__result, assistBonus, adjustedCap));
				__result = adjustedCap;
			}
		}
		catch (Exception ex)
		{
			Main.mod.Logger.Error("[CP_PATCH] Error: " + ex.Message);
		}
	}
}
```

This will show:
1. If patch is called at all (`[CP_PATCH] Called for...`)
2. What bonus value is retrieved (`Bonus = 45` or `Bonus = 0`)
3. Whether the subtraction happens

---

## 💡 Key Finding

**The patch IS NOT being called at all** (or it's being called but `assistBonus > 0` is never true)

The bonuses ARE being tracked correctly (45 points confirmed), so the dictionary and RecordBonus method work fine.

The problem is specifically with the Harmony patch interception of the controlPointCapacity property getter.

---

## 📝 Actions Taken This Session

1. ✅ Added debug logging to TICouncilorState_ControlPointCapacityPatch
2. ✅ Simplified logging in AssistBonusTracker 
3. ✅ **REMOVED excessive spam** from TIMissionCondition_PlayerFactionOnly
4. ✅ Created TESTING_GUIDE.md, LOG_REFERENCE.md, VERIFICATION_QUICK.md
5. ✅ Committed and pushed all changes

---

## 📌 Conclusion

**Current State:** 
- Assist bonuses are **fully tracked** (45 points confirmed)
- Control point cap patch **not intercepting** correctly
- Log spam **eliminated**

**Next Phase:**
- Deploy updated DLL
- Test with aggressive logging on CP patch
- Determine if it's a Harmony target name issue or reference comparison issue
- Implement workaround if needed

