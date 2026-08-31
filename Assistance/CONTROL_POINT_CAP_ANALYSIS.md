# Control Point Cap Calculation & Bonus Exclusion Strategy

## How Control Point Cap Works

### Current Implementation

**Location:** `GameAnalysis/Assembly-CSharp/PavonisInteractive/TerraInvicta/TICouncilorState.cs:2507-2513`

```csharp
public int controlPointCapacity
{
	get
	{
		return this.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false) 
			 + this.GetAttribute(CouncilorAttribute.Command, true, true, true, false, false, false) 
			 + this.GetAttribute(CouncilorAttribute.Administration, true, true, true, false, false, false);
	}
}
```

**Key Finding:** Control point capacity is the **sum of three councilor stats:**
1. **Persuasion**
2. **Command**
3. **Administration**

Each `GetAttribute()` call includes:
- Base attribute value (`this.attributes[type]`)
- Trait bonuses (`ApplyTraitStatValue`)
- Org bonuses (`GetStatBonus`)
- Capping at `maxCouncilorAttribute` (typically 100)

### Usage in Faction Control Point Maintenance

**Location:** `GameAnalysis/Assembly-CSharp/PavonisInteractive/TerraInvicta/TIFactionState.cs:2892-2899`

```csharp
public float GetControlPointMaintenanceFreebieCap()
{
	if (!this.IsAlienFaction)
	{
		return (float)(
			TIGlobalValuesState.GlobalValues.controlPointMaintenanceFreebies 
			+ (this.isActivePlayer ? 0 : TIGlobalValuesState.GlobalValues.scenarioCustomizations.controlPointMaintenanceFreebieBonusAI) 
			+ this.activeCouncilors.Sum<TICouncilorState>((TICouncilorState x) => x.controlPointCapacity)  // ← Uses controlPointCapacity
			+ this.habs.Sum<TIHabState>((TIHabState x) => x.controlPointCapacityValue)
		) - TIEffectsState.SumEffectsModifiers(Context.ControlPointMaintenance, this, ...);
	}
	return 20000f;
}
```

**Impact:** The sum of all councilors' `controlPointCapacity` determines how many control points a faction can maintain without penalties.

---

## The Problem with Assist Mission Bonuses

Currently, when the Assist mission applies stat bonuses via `TIMissionEffect_Assist`, it modifies the target councilor's stats directly. This inflates their `controlPointCapacity`, which:

1. **Increases maintenance freebie cap** - allowing more free control points
2. **Masks the true capacity** - bonuses are temporary but counted permanently until the next mission
3. **Unintended advantage** - player gets bonus capacity without paying the normal cost

---

## Solution Strategy: Exclude Assist Bonuses from Control Point Cap Calculation

### Approach 1: Separate Bonus Pool (Recommended)

**Concept:** Store Assist mission bonuses in a separate tracking structure that `GetAttribute()` skips when calculating control point capacity.

**Implementation:**

```csharp
// In TICouncilorState.cs - new field
private Dictionary<CouncilorAttribute, int> assistMissionBonuses = new Dictionary<CouncilorAttribute, int>();

// Method to add/remove assist bonuses (called by mod's TIMissionEffect_Assist)
public void ApplyAssistBonus(CouncilorAttribute attribute, int bonusAmount)
{
	if (!assistMissionBonuses.ContainsKey(attribute))
		assistMissionBonuses[attribute] = 0;
	assistMissionBonuses[attribute] += bonusAmount;
	this.SetAttributesDirty(); // Force recalculation of cached values
}

public void RemoveAssistBonus(CouncilorAttribute attribute, int bonusAmount)
{
	if (assistMissionBonuses.ContainsKey(attribute))
		assistMissionBonuses[attribute] -= bonusAmount;
	this.SetAttributesDirty();
}

// Modified GetAttribute to skip assist bonuses when calculating CP capacity
public int GetAttributeForControlPointCap(CouncilorAttribute type)
{
	// Same as GetAttribute, but only counts:
	// - Base attributes
	// - Trait bonuses
	// - Org bonuses
	// EXCLUDES: Assist mission bonuses

	return this.GetAttribute(type, true, true, true, false, false, false) 
		 - (assistMissionBonuses.ContainsKey(type) ? assistMissionBonuses[type] : 0);
}
```

**Changes needed:**
1. Create dedicated `ApplyAssistBonus()` and `RemoveAssistBonus()` methods
2. Create `GetAttributeForControlPointCap()` that subtracts assist bonuses
3. Modify `controlPointCapacity` property to use the new method:

```csharp
public int controlPointCapacity
{
	get
	{
		return this.GetAttributeForControlPointCap(CouncilorAttribute.Persuasion) 
			 + this.GetAttributeForControlPointCap(CouncilorAttribute.Command) 
			 + this.GetAttributeForControlPointCap(CouncilorAttribute.Administration);
	}
}
```

4. Update mod's `TIMissionEffect_Assist.cs` to call the new methods instead of `ModifyAttribute()`

---

### Approach 2: Modifier Flag System (Alternative)

**Concept:** Tag stat modifiers with a source ID and allow selective filtering.

**Pros:**
- More elegant - keeps all modifier logic in one place
- Extensible for other mods

**Cons:**
- Requires decompilation/reflection to implement properly
- More complex integration with game's internals

---

## Recommended Implementation (Approach 1)

### Step-by-Step Patch Plan

1. **Create Harmony patch for `TICouncilorState.GetAttribute()`:**
   - Intercept and inject exclusion logic for assist bonuses
   - Maintain backward compatibility (other code still gets full bonuses)

2. **Modify `TIMissionEffect_Assist.ApplyEffect()` and cleanup:**
   - Instead of calling `ModifyAttribute()`, directly call new `ApplyAssistBonus()` method
   - Ensure cleanup code calls `RemoveAssistBonus()` when mission completes

3. **Track bonus sources in AssistBonusTracker:**
   - Add tracking of which councilor received which bonus amount
   - Simplifies cleanup logic

### Benefits of This Approach

✅ **Preserves UI & mission mechanics** - Bonuses still appear and work normally  
✅ **Fair gameplay** - CP cap reflects true capacity, not inflated by temporary bonuses  
✅ **Mod isolation** - Only affects control point cap calculation, not other stat uses  
✅ **Backward compatible** - Doesn't break existing game or other mods  
✅ **Clean separation** - Assist bonuses are explicitly tracked separately  

---

## Code Location References

| Component | File | Lines |
|-----------|------|-------|
| Control Point Cap Calculation | `TICouncilorState.cs` | 2507-2513 |
| GetAttribute Implementation | `TICouncilorState.cs` | 1832-1885 |
| Faction Maintenance Calc | `TIFactionState.cs` | 2892-2899 |
| Councilor Attributes Enum | `CouncilorAttribute.cs` | 1-29 |
| Assist Bonus Tracking | `Assistance/AssistBonusTracker.cs` | (Current) |
| Mission Effect Application | `Assistance/TIMissionEffect_Assist.cs` | (Current) |

---

## Verification & Testing

After implementation:

1. **Verify base scenario:** Councilor without Assist mission has correct CP capacity
2. **Test with Assist applied:** CP capacity remains unchanged during assist
3. **Test maintenance penalty:** No artificial penalty reduction during assist
4. **Test mission completion:** Bonuses removed, CP capacity returns to normal
5. **Test mixed scenarios:** Multiple councilors with/without assists
6. **Test AI factions:** Ensure AI doesn't exploit bonus capacity

---

## Impact Assessment

| Category | Current | After Patch | Impact |
|----------|---------|-------------|--------|
| Player capacity | Inflated during assist | Realistic | ✅ More balanced |
| Mission mechanics | Unchanged | Unchanged | ✅ No change |
| Bonus application | Works normally | Works normally | ✅ Transparent |
| Performance | N/A | Minimal (one extra subtraction) | ✅ Negligible |
| Mod compatibility | Isolated | Isolated | ✅ Safe |

---

## Next Steps

1. Research if game provides hook for selective attribute filtering
2. Implement `GetAttributeForControlPointCap()` as Harmony patch
3. Modify `TIMissionEffect_Assist` to use dedicated bonus tracking
4. Test thoroughly with various CP scenarios
5. Document the cap exclusion in mod notes

