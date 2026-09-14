# Advise Mission Amendments: Mod vs Vanilla Comparison

**Analysis Date:** Generated from current workspace  
**Mod:** Assistance (Harmony-based patches)  
**Vanilla Source:** TI Decompiled (Assembly-CSharp)

---

## Executive Summary

The Assistance mod **does NOT replace the Advise mission**. Instead, it uses a surgical Harmony postfix patch on `TICouncilorState.AdvisingBonus()` to amplify the effectiveness of the vanilla Advise mission across all three target types (Nation, Hab, and other). This approach preserves 100% of vanilla logic while adding a single configurable multiplier.

---

## Vanilla Advise Mission Architecture

### Vanilla Implementation: `TIMissionEffect_Advise`

**File:** `TIMissionEffect_Advise.cs`  
**Core Method:** `public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome)`

```csharp
// Vanilla ApplyEffect flow:
if (target.isNationState)
{
	// Target 1: Nation
	// Capture current advising bonus values
	float adviserScienceBonus = target.ref_nation.adviserScienceBonus;
	float adviserCommandBonus = target.ref_nation.adviserCommandBonus;
	float adviserAdministrationBonus = target.ref_nation.adviserAdministrationBonus;

	// Add the councilor as an advisor
	target.ref_nation.AddAdvisingCouncilor(councilor);

	// Calculate deltas and format results
	// Special1 localization message
}
else if (target.isHabState)
{
	// Target 2: Hab/Settlement
	// Get current advising attributes
	float advisingAttribute = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Science);
	float advisingAttribute2 = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Command);
	float advisingAttribute3 = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Administration);

	// Add the councilor as an advisor
	ref_hab.AddAdvisingCouncilor(councilor);

	// Calculate deltas and format results
	// Special2 localization message
}
else
{
	// Target 3: Fallback (for targets that are neither nation nor hab)
	// Special3 localization message
	// Uses AdvisingBonus() directly: Command and Science only
	councilor.AdvisingBonus(CouncilorAttribute.Command).ToPercent("P0")
	councilor.AdvisingBonus(CouncilorAttribute.Science).ToPercent("P0")
}
```

### Three Call Sites for AdvisingBonus

According to the mod's documentation, there are exactly 3 places where advisors affect gameplay:

1. **Nation Advising Score** → `TINationState.GetAdvisingScore()`
   - Sums `AdvisingBonus(attribute) / rank` across all advisors
   - Diminishing returns applied by rank

2. **Hab Advising Attributes** → `TIHabState.GetAdvisingAttribute(attribute)`
   - Sums `AdvisingBonus(attribute) / rank` across all advisors
   - Diminishing returns applied by rank

3. **Fallback Mission Effect Message** → `TIMissionEffect_Advise.ApplyEffect()` Special3
   - Direct read of `AdvisingBonus()` for display purposes (neither nation nor hab target)

---

## Mod Implementation Strategy

### Core Approach: Harmonic Postfix Patch

**File:** `TICouncilorState_AdvisingBonusPatch.cs`  
**Target:** `TICouncilorState.AdvisingBonus(CouncilorAttribute attribute) : float`  
**Patch Type:** Harmony Postfix (after vanilla execution)

```csharp
[HarmonyPatch(typeof(TICouncilorState), "AdvisingBonus")]
internal static class TICouncilorState_AdvisingBonusPatch
{
	[HarmonyPostfix]
	public static void Postfix(TICouncilorState __instance, CouncilorAttribute attribute, ref float __result)
	{
		if (!Main.enabled || Main.settings == null || !Main.settings.enableAssistMission || __instance == null)
			return;

		// Get accumulated Assist bonus from tracker
		int assistBonus = AssistBonusTracker.GetStatBonus(__instance, attribute);
		float assistPercentage = Main.settings.assistPercentage / 100f;

		float additional = 0f;

		// Component 1: Percentage boost to effective advising value
		if (assistPercentage > 0f)
		{
			int rawAttribute = __instance.GetAttribute(attribute, true, true, true, false, false, false);
			additional += (rawAttribute + assistBonus) * assistPercentage / 100f;
		}

		// Component 2: Direct bonus contribution (scaled to AdvisingBonus's 1/100 scale)
		if (assistBonus > 0)
		{
			additional += assistBonus / 100f;
		}

		__result += additional;
	}
}
```

### Mod Settings (Main.cs)

- **`enableAssistMission`** (bool): Toggle entire Assist mission system on/off
- **`assistPercentage`** (float): Percentage multiplier for advising bonuses (e.g., 50 = 50%)
- **`debugLogging`** (bool): Enable/disable detailed logging

### Bonus Tracking System

**File:** `AssistBonusTracker.cs`

Maintains a temporary bonus pool per councilor per attribute:
- `RecordBonus(councilor, attribute, bonusAmount)` - Add bonus when Assist mission completes
- `GetStatBonus(councilor, attribute)` - Retrieve accumulated bonus (read-only, non-destructive)
- `RemoveBonuses(councilor)` - Clear all bonuses when advising session ends

---

## Key Differences: Mod vs Vanilla

### 1. Advise Mission Effect (TIMissionEffect_Advise)

| Aspect | Vanilla | Mod |
|--------|---------|-----|
| **Mechanism** | Directly adds councilor to nation/hab advisor list | *Unchanged - vanilla logic preserved* |
| **Impact on Nation** | Via `TINationState.AddAdvisingCouncilor()` | *Same - unchanged* |
| **Impact on Hab** | Via `TIHabState.AddAdvisingCouncilor()` | *Same - unchanged* |
| **Fallback (Other)** | Shows Command/Science bonuses via `AdvisingBonus()` | *Same - unchanged* |
| **Result Messages** | Localizable, shows actual deltas | *Same - vanilla messages preserved* |

### 2. AdvisingBonus Calculation

| Aspect | Vanilla | Mod |
|--------|---------|-----|
| **Base Formula** | `councilor.rawStat / 100` | `(councilor.rawStat / 100) + mod boost` |
| **Boost Components** | None | 1. Assist percentage multiplier<br>2. Assist bonus contribution (scaled) |
| **Conditional** | Always same value | Only if Assist mission enabled & bonus exists |
| **Call Sites** | 3 (all preserved) | *Same 3 call sites - all benefit* |

### 3. Assist Mission (Mod-Only Addition)

The mod adds a **separate, optional Assist mission**:

**File:** `TIMissionTemplate_Assist.cs`

```csharp
public class TIMissionTemplate_Assist : TIMissionTemplate
{
	// Target Type: Councilor (same faction)
	// Outcome: Automatic success (guaranteed)
	// Effect: Tracks bonus with TIMissionEffect_Assist

	// Mission succeeds and records bonus stats with AssistBonusTracker
}
```

**Supporting Mission Conditions:**
- `TIMissionCondition_MyFactionCouncilor` - Target must be same faction
- `TIMissionCondition_NotCurrentlyAssisting` - Prevent duplicate assists
- `TIMissionCondition_PlayerFactionOnly` - Restrict to player-controlled factions

---

## Integration Points with Vanilla

### Direct Integration

1. **`TICouncilorState.AdvisingBonus()`** 
   - Patched: Postfix adds bonus
   - Impact: All three advising call sites immediately benefit

2. **`TINationState.GetAdvisingScore()`**
   - Unchanged, calls `AdvisingBonus()` internally
   - Automatically receives boost via patch

3. **`TIHabState.GetAdvisingAttribute()`**
   - Unchanged, calls `AdvisingBonus()` internally
   - Automatically receives boost via patch

4. **`TIMissionEffect_Advise.ApplyEffect()`**
   - Unchanged, calls `AdvisingBonus()` in Special3 fallback
   - Fallback messages automatically show boosted values

### Contested Mission Integration

**File:** `TIMissionResolution_Contested_AssistBonusPatch.cs`

Additionally patches contested mission modifiers to apply Assist bonuses:
- `SumAttackingModifiers()` - Attacking councilor's stats
- `SumDefendingModifiers()` - Defending councilor's stats

This is **separate from Advise** but uses the same `AssistBonusTracker`.

---

## Previous Implementation (Superseded)

**File:** `TIMissionEffect_Advise.cs` (now inert)

The mod originally took a different approach:
- Full Harmony prefix on `TIMissionEffect_Advise.ApplyEffect()`
- Completely reimplemented the method
- **Problem:** Requires maintaining duplicate nation/hab/fallback logic
- **Risk:** Vanilla enhancements silently dropped if not manually merged
- **Status:** Superseded by simpler `AdvisingBonus` postfix approach

---

## Vanilla Code References

### TIMissionEffect_Advise.ApplyEffect() - Complete Vanilla

```csharp
public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
{
	TICouncilorState councilor = mission.councilor;
	if (target.isNationState)
	{
		float adviserScienceBonus = target.ref_nation.adviserScienceBonus;
		float adviserCommandBonus = target.ref_nation.adviserCommandBonus;
		float adviserAdministrationBonus = target.ref_nation.adviserAdministrationBonus;
		target.ref_nation.AddAdvisingCouncilor(councilor);
		return Loc.T("TIMissionEffect_Advise.Special1", new object[]
		{
			(target.ref_nation.adviserScienceBonus - adviserScienceBonus).ToPercent("P0"),
			(target.ref_nation.adviserAdministrationBonus - adviserAdministrationBonus).ToPercent("P0"),
			(target.ref_nation.adviserCommandBonus - adviserCommandBonus).ToString("N2")
		});
	}
	if (target.isHabState)
	{
		TIHabState ref_hab = target.ref_hab;
		float advisingAttribute = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Science);
		float advisingAttribute2 = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Command);
		float advisingAttribute3 = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Administration);
		ref_hab.AddAdvisingCouncilor(councilor);
		return Loc.T("TIMissionEffect_Advise.Special2", new object[]
		{
			(ref_hab.GetAdvisingAttribute(CouncilorAttribute.Science) - advisingAttribute).ToPercent("P0"),
			(ref_hab.GetAdvisingAttribute(CouncilorAttribute.Administration) - advisingAttribute3).ToPercent("P0"),
			(ref_hab.GetAdvisingAttribute(CouncilorAttribute.Command) - advisingAttribute2).ToPercent("P0")
		});
	}
	return Loc.T("TIMissionEffect_Advise.Special3", new object[]
	{
		councilor.AdvisingBonus(CouncilorAttribute.Command).ToPercent("P0"),
		councilor.AdvisingBonus(CouncilorAttribute.Science).ToPercent("P0")
	});
}
```

---

## Benefits of Mod's Approach

### ✅ Non-Invasive
- Only patches one method
- No risk of breaking other advising code
- Vanilla logic remains 100% unchanged

### ✅ Centralized Control
- Single postfix handles all three call sites
- Bonus boost applies everywhere automatically
- No need to replicate mission effect logic

### ✅ Forward Compatible
- Future vanilla enhancements automatically included
- No manual merging needed
- Scales with game updates

### ✅ Configuration
- Simple percentage multiplier setting
- Optional separate Assist mission
- Can be disabled without affecting Advise mission

### ✅ Maintainable
- Minimal code duplication
- Clear separation of concerns
- Easy to debug (single postfix point)

---

## Comparison Table: All Advising Elements

| Element | Vanilla | Mod Behavior | Notes |
|---------|---------|--------------|-------|
| **Advise Mission Available** | ✅ Yes | ✅ Yes (unchanged) | Core mission unmodified |
| **AdvisingBonus() Base Value** | `stat / 100` | `stat / 100 + boost` | Postfix addition |
| **Assist Mission Available** | ❌ No | ✅ Yes (optional) | Mod addition, separate |
| **Assist Bonus Tracking** | ❌ No | ✅ Yes | Via `AssistBonusTracker` |
| **Nation Advising** | ✅ Works | ✅ Enhanced | Via boosted `AdvisingBonus()` |
| **Hab Advising** | ✅ Works | ✅ Enhanced | Via boosted `AdvisingBonus()` |
| **Fallback Advising** | ✅ Works | ✅ Enhanced | Via boosted `AdvisingBonus()` |
| **Contested Mission** | ✅ Works | ✅ Enhanced | Separate patch in mod |
| **Mission Messages** | ✅ Vanilla | ✅ Vanilla (unchanged) | Localization preserved |

---

## Conclusion

The Assistance mod's approach to advising enhancements is **minimally invasive and maximally compatible**. Rather than replacing the Advise mission, it:

1. **Amplifies** vanilla advising through a single-method postfix
2. **Adds** an optional Assist mission for bonus accumulation
3. **Preserves** all vanilla logic, messages, and mechanics
4. **Scales** automatically to any vanilla improvements

The result is a mod that enhances the Advise mission's effectiveness without requiring the maintenance burden of reimplementing vanilla logic.

