# Fix: Double-Counting Bug in Advising Bonus Calculations

## Issue
Users reported that assist missions were **double-counting councilor stats** when calculating nation bonuses.

## Root Cause
**File:** `Assistance/TICouncilorState_AdvisingBonusPatch.cs`

The assist bonus was being added to `AdvisingBonus()` twice:

```csharp
// OLD BUGGY CODE
float additional = 0f;

if (assistPercentage > 0f)
{
	// COUNT #1: assistBonus included here
	int rawAttribute = __instance.GetAttribute(...);
	additional += (rawAttribute + assistBonus) * assistPercentage;
}

if (assistBonus > 0)
{
	// COUNT #2: assistBonus added AGAIN here
	additional += assistBonus / 100f;
}

__result += additional;  // Both counts applied!
```

### Impact
- Nation research, defense, operations bonuses were **~2x stronger than intended**
- Hab bonuses were **~2x stronger than intended**
- Broke game balance for assisted councilors

## Solution
The assist bonus should be applied **once only**, scaled by `assistPercentage` to simulate efficiency loss:

```csharp
// NEW FIXED CODE
int assistBonus = AssistBonusTracker.GetStatBonus(__instance, attribute);

if (assistBonus > 0)
{
	// Apply assist bonus ONCE, scaled by efficiency
	float assistPercentage = Main.settings.assistPercentage / 100f;
	float additional = (assistBonus * assistPercentage) / 100f;
	__result += additional;

	// Logging...
}
```

## How It Works

The formula is now: `AdvisingBonus += (assistBonus * assistPercentage) / 100 / 100`

**Example with 50% efficiency (assistPercentage = 50):**
- Assist bonus: 20 points
- Calculation: `(20 * 0.5) / 100 = 0.1`
- Contribution to AdvisingBonus: 0.1 (scaled at vanilla's standard rate)
- Effect: Councilor's nation/hab bonus includes this 0.1 contribution

**Vanilla behavior preserved:**
- Base councilor attribute continues to be handled by vanilla's code (unchanged)
- Only the assist bonus is scaled by the efficiency setting
- No double-counting

## Changes Made
- **File modified:** `Assistance/TICouncilorState_AdvisingBonusPatch.cs`
- **Lines changed:** Entire `Postfix` method body (lines 24-47)
- **Build status:** ✅ Successful
- **Backward compatible:** Yes (fixes a bug, doesn't change intended behavior)

## Testing Recommendations

1. **Create a test scenario:**
   - Councilor A: Science 80
   - Councilor B: Science 40
   - Run assist mission (A assists B), success
   - Bonus should be ~(80 * 0.5) / 100 ≈ 0.4 Science bonus recorded

2. **Verify nation bonuses:**
   - Check nation research production with B advising
   - Bonus should be: B's base advising + (assist bonus * efficiency / 100)
   - Should NOT be 2x inflated

3. **Test with different efficiency settings:**
   - 50% efficiency: bonus = 0.4 (original example)
   - 100% efficiency: bonus = 0.8 (full assist applied)
   - 0% efficiency: bonus = 0 (no assist benefit)

## Related Code
- `AssistBonusTracker.cs` — Stores assist bonuses
- `TIMissionEffect_Assist.cs` — Applies bonuses after successful missions
- `Settings.cs` — Manages `assistPercentage` setting
