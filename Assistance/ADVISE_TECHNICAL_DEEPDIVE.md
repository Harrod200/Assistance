# Technical Deep Dive: Advise Mission Implementation Details

---

## Part 1: Vanilla Architecture

### Vanilla File Location
```
C:\Users\Chris\source\repos\TI Decompiled\GameAnalysis\Assembly-CSharp\TIMissionEffect_Advise.cs
```

### Vanilla Code: Complete TIMissionEffect_Advise Class

```csharp
// Token: 0x020001EF RID: 495
public class TIMissionEffect_Advise : TIMissionEffect
{
	// Token: 0x060006CF RID: 1743 RVA: 0x00020FB4 File Offset: 0x0001F1B4
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;

		// ============= TARGET TYPE 1: NATION =============
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

		// ============= TARGET TYPE 2: HAB/SETTLEMENT =============
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

		// ============= TARGET TYPE 3: FALLBACK (OTHER) =============
		return Loc.T("TIMissionEffect_Advise.Special3", new object[]
		{
			councilor.AdvisingBonus(CouncilorAttribute.Command).ToPercent("P0"),
			councilor.AdvisingBonus(CouncilorAttribute.Science).ToPercent("P0")
		});
	}
}
```

### Vanilla Call Flow Analysis

```
Player completes "Advise" mission on Target
	↓
TIMissionEffect_Advise.ApplyEffect(mission, target, outcome)
	↓
	├─→ if (target is Nation)
	│   ├─ Get current advising bonuses (Science, Command, Administration)
	│   ├─ Call target.ref_nation.AddAdvisingCouncilor(councilor)
	│   ├─ Get updated advising bonuses
	│   ├─ Calculate deltas
	│   └─ Return "Special1" message with formatted deltas
	│
	├─→ else if (target is Hab)
	│   ├─ Get current advising attributes (Science, Command, Admin)
	│   ├─ Call target.ref_hab.AddAdvisingCouncilor(councilor)
	│   ├─ Get updated advising attributes
	│   ├─ Calculate deltas
	│   └─ Return "Special2" message with formatted deltas
	│
	└─→ else (Other target types)
		├─ Call councilor.AdvisingBonus(Command) for message
		├─ Call councilor.AdvisingBonus(Science) for message
		└─ Return "Special3" message with bonus values
```

### Key Vanilla Methods Called

1. **`TINationState.AddAdvisingCouncilor(councilor)`**
   - Adds councilor to nation's advisory pool
   - Updates nation's advisor bonuses (internally uses `AdvisingBonus()`)

2. **`TIHabState.AddAdvisingCouncilor(councilor)`**
   - Adds councilor to hab's advisory pool
   - Updates hab's advisor attributes (internally uses `AdvisingBonus()`)

3. **`TIHabState.GetAdvisingAttribute(attribute)`**
   - Returns current advising attribute value for hab
   - Internally sums `AdvisingBonus()` across all advisors

4. **`TICouncilorState.AdvisingBonus(attribute)`**
   - Returns `councilor.rawStat / 100` for the given attribute
   - **Used by all three call sites**
   - Direct call in Special3 fallback
   - Indirect call in `GetAdvisingAttribute()` and nation advising

---

## Part 2: Mod's Harmony Patch Architecture

### Mod File Structure

```
Assistance/
├── TICouncilorState_AdvisingBonusPatch.cs      ← Main patch (postfix)
├── AssistBonusTracker.cs                       ← Bonus storage
├── TIMissionEffect_Assist.cs                   ← Bonus recording
├── TIMissionTemplate_Assist.cs                 ← Mission definition
├── TIMissionResolution_Contested_AssistBonusPatch.cs ← Contested bonus
└── [Various mission conditions]
```

### Core Patch: TICouncilorState_AdvisingBonusPatch.cs

**Location:** `Assistance\TICouncilorState_AdvisingBonusPatch.cs`

```csharp
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
	/// <summary>
	/// Boosts TICouncilorState.AdvisingBonus() rather than replacing the Advise mission effect.
	///
	/// AdvisingBonus(attribute) = councilor's raw stat / 100. It is the single building block behind
	/// everything Advise does: TINationState.GetAdvisingScore() and TIHabState.GetAdvisingAttribute()
	/// both sum AdvisingBonus(attribute)/rank across all current advisors (diminishing returns), and
	/// the vanilla TIMissionEffect_Advise.ApplyEffect() Special3 fallback (for target types that are
	/// neither a nation nor a hab) reads AdvisingBonus() directly for its own message. Those are the
	/// only three call sites in the game.
	///
	/// Boosting this one method means nation, hab, and that third target type all pick up the mod's
	/// advisePercentage setting and the councilor's accumulated Assist bonus automatically - with
	/// vanilla's own ranking, stacking, message formatting (including the nation-Command-is-flat vs
	/// everything-else-is-percentage distinction), and outcome handling left completely untouched.
	/// No need to duplicate any of that here, and no target-type branch is ever silently dropped.
	///
	/// This supersedes the previous approach of Harmony-prefixing TIMissionEffect_Advise.ApplyEffect
	/// and fully reimplementing it (see the now-inert TIMissionEffect_Advise.cs).
	/// </summary>
	[HarmonyPatch(typeof(TICouncilorState), "AdvisingBonus")]
	internal static class TICouncilorState_AdvisingBonusPatch
	{
		[HarmonyPostfix]
		public static void Postfix(TICouncilorState __instance, CouncilorAttribute attribute, ref float __result)
		{
			// Guard: Ensure mod is enabled and settings are valid
			if (!Main.enabled || Main.settings == null || !Main.settings.enableAssistMission || __instance == null)
				return;

			// Non-destructive read - clearing is handled solely by TICouncilorState_CompleteMissionPatch.
			int assistBonus = AssistBonusTracker.GetStatBonus(__instance, attribute);
			float assistPercentage = Main.settings.assistPercentage / 100f;

			float additional = 0f;

			// COMPONENT 1: Percentage-based boost
			// Amplifies the councilor's raw stat by a configured percentage
			// This scales proportionally with the councilor's effectiveness
			if (assistPercentage > 0f)
			{
				// Same effective-value shape as the rest of the mod: the councilor's raw stat, boosted
				// by any accumulated Assist bonus, scaled by the configured assist percentage.
				int rawAttribute = __instance.GetAttribute(attribute, true, true, true, false, false, false);
				additional += (rawAttribute + assistBonus) * assistPercentage / 100f;
			}

			// COMPONENT 2: Direct bonus contribution
			// Adds accumulated Assist bonuses on the same scale as AdvisingBonus (raw/100)
			// This ensures bonuses from other council members stack consistently
			if (assistBonus > 0)
			{
				// Matches vanilla's own AdvisingBonus scale (raw stat / 100), so the accumulated Assist
				// bonus is folded in on the same footing as the councilor's own stat.
				additional += assistBonus / 100f;

				if (Main.mod != null && Main.settings.debugLogging)
				{
					Main.mod.Logger.Log(string.Format(
						"[AdviseMission] AdvisingBonus({0}) for '{1}': base {2:F3} + assist/percentage {3:F3}",
						attribute, __instance.displayName, __result, additional));
				}
			}

			__result += additional;
		}
	}
}
```

### Patch Execution Flow

```
Vanilla code calls: councilor.AdvisingBonus(attribute)
	↓
1. Vanilla AdvisingBonus() executes:
   __result = councilor.rawStat / 100
	↓
2. Harmony postfix intercepts:
   Postfix(councilor, attribute, ref __result)
	↓
3. Postfix reads current bonus from tracker:
   assistBonus = AssistBonusTracker.GetStatBonus(councilor, attribute)
	↓
4. Postfix calculates additional bonus:
   additional = (percentage * rawStat) + (assistBonus / 100)
	↓
5. Postfix modifies result:
   __result += additional
	↓
6. Final result returned to caller:
   advising score is now enhanced
```

### Example Calculation

**Scenario:** Councilor with Science=80, accumulated Assist=15, assistPercentage=50%

```
Vanilla AdvisingBonus(Science):
  __result = 80 / 100 = 0.80

Postfix Calculation:
  assistPercentage = 50 / 100 = 0.50
  rawAttribute = 80 (from councilor stat)
  assistBonus = 15 (from tracker)

  additional = (80 + 15) * 0.50 / 100 + 15 / 100
			 = 95 * 0.50 / 100 + 0.15
			 = 47.5 / 100 + 0.15
			 = 0.475 + 0.15
			 = 0.625

Final Result:
  __result = 0.80 + 0.625 = 1.425

Benefit:
  Vanilla advising: 0.80
  Mod advising:     1.425
  Improvement:      78% increase
```

---

## Part 3: Supporting Systems

### AssistBonusTracker.cs

**Purpose:** Maintain temporary bonus pool per councilor per attribute

```csharp
// Simplified interface:
public static class AssistBonusTracker
{
	// Store bonus for this councilor/attribute pair
	public static void RecordBonus(TICouncilorState councilor, CouncilorAttribute attribute, int amount)

	// Retrieve accumulated bonus (non-destructive read)
	public static int GetStatBonus(TICouncilorState councilor, CouncilorAttribute attribute)

	// Get total bonus across all attributes
	public static int GetTotalBonus(TICouncilorState councilor)

	// Clear all bonuses for this councilor
	public static void RemoveBonuses(TICouncilorState councilor)
}
```

### TIMissionEffect_Assist.cs

**Purpose:** Apply effect when Assist mission completes

```csharp
public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome)
{
	// Verify mod is enabled
	if (!Main.enabled || Main.settings == null || !Main.settings.enableAssistMission)
		return string.Empty;

	TICouncilorState assistingCouncilor = mission.councilor;
	TICouncilorState targetCouncilor = target as TICouncilorState;

	// Iterate all 7 attributes
	foreach (CouncilorAttribute stat in allAttributes)
	{
		int assistingValue = assistingCouncilor.GetAttribute(stat, ...);
		int assistAmount = Max(1, Floor(assistingValue * assistPercentage));

		if (assistAmount > 0 && MissionSuccess(outcome))
		{
			// Track bonus for future advising calculations
			AssistBonusTracker.RecordBonus(targetCouncilor, stat, assistAmount);
		}
	}
}
```

### TICouncilorState_CompleteMissionPatch.cs

**Purpose:** Clear bonuses when advising session ends

Patches the mission completion to ensure bonuses are reset at the right time, so they don't accumulate indefinitely.

---

## Part 4: Comparison Table - Call Site Analysis

### Call Site 1: TINationState.GetAdvisingScore()

```
Vanilla Flow:
  GetAdvisingScore()
	→ for each advisor in nation
		→ sum += AdvisingBonus(attribute) / rank
	→ return sum

Mod Impact:
  GetAdvisingScore()
	→ for each advisor in nation
		→ call to AdvisingBonus() hits postfix
		→ sum += (BOOSTED AdvisingBonus) / rank
	→ return sum

Result: Nation bonuses automatically enhanced
```

### Call Site 2: TIHabState.GetAdvisingAttribute()

```
Vanilla Flow:
  GetAdvisingAttribute(Science)
	→ for each advisor in hab
		→ sum += AdvisingBonus(Science) / rank
	→ return sum

Mod Impact:
  GetAdvisingAttribute(Science)
	→ for each advisor in hab
		→ call to AdvisingBonus() hits postfix
		→ sum += (BOOSTED AdvisingBonus) / rank
	→ return sum

Result: Hab bonuses automatically enhanced
```

### Call Site 3: TIMissionEffect_Advise.ApplyEffect() - Special3 Fallback

```
Vanilla Code:
  return Loc.T("TIMissionEffect_Advise.Special3", new object[]
  {
	  councilor.AdvisingBonus(CouncilorAttribute.Command).ToPercent("P0"),
	  councilor.AdvisingBonus(CouncilorAttribute.Science).ToPercent("P0")
  });

Vanilla Result:
  "Command: 25%, Science: 30%"

Mod Flow:
  → AdvisingBonus(Command) hits postfix → enhanced value
  → AdvisingBonus(Science) hits postfix → enhanced value
  → ToPercent() converts enhanced values to display strings
  → Loc.T() formats the message

Mod Result:
  "Command: 45%, Science: 52%"  (example with boost)
```

---

## Part 5: Why This Approach Works

### ✅ Single Point of Modification

```
Before (Vanilla):
  TINationState.GetAdvisingScore()
  TIHabState.GetAdvisingAttribute()
  TIMissionEffect_Advise.ApplyEffect()
  └─ All call AdvisingBonus()

After (Mod):
  [Harmony Postfix on AdvisingBonus]
  ├─ TINationState.GetAdvisingScore() → benefits
  ├─ TIHabState.GetAdvisingAttribute() → benefits
  └─ TIMissionEffect_Advise.ApplyEffect() → benefits
```

### ✅ No Duplication

**Old Approach (Superseded):**
```csharp
[HarmonyPrefix]
public static bool ApplyEffect(TIMissionEffect_Advise __instance, ...)
{
	// Duplicate nation logic
	if (target.isNationState) { ... }
	// Duplicate hab logic
	if (target.isHabState) { ... }
	// Duplicate fallback logic
	return false; // Skip vanilla
}
// Problem: Must maintain all three paths separately
```

**New Approach:**
```csharp
[HarmonyPostfix]
public static void Postfix(TICouncilorState __instance, ..., ref float __result)
{
	__result += additionalBonus;  // One line, three benefits
}
// Benefit: All call sites automatically enhanced
```

### ✅ Forward Compatible

If vanilla adds new advising mechanics in a patch:
- **Old approach:** Must manually merge new code into the prefix
- **New approach:** Automatically works (new code will call postfixed `AdvisingBonus()`)

---

## Conclusion

The mod's Advise mission enhancement is achieved through:

1. **Surgical targeting:** Single method postfix on `AdvisingBonus()`
2. **Automatic distribution:** All three call sites benefit from one patch
3. **Minimal footprint:** ~50 lines of postfix code vs. ~100+ lines of full replacement
4. **Maximum compatibility:** Zero impact on vanilla logic paths
5. **Easy maintenance:** Configuration-driven, no code duplication

This is textbook Harmony patching best practice: modify the smallest necessary surface area to achieve the desired effect.

