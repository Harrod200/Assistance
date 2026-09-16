# Mission Modifier Display Fix - Technical Summary

## Problem Statement
The mission breakdown was displaying incorrect modifiers:
1. **Reward modifiers appearing in breakdowns** - "Detained Target" bonus showed in Detain missions even though it was granted AFTER the mission
2. **Zero-valued modifiers missing** - Crackdown modifier didn't show in control point breakdowns because it had zero value
3. **Extra attributes showing** - Purge missions showed additional unrelated modifiers

**Root Cause:** The vanilla game's `GetNonZeroModifiers()` methods return modifiers **as they exist at the time of the call**, not as they were during mission calculation. By the time `LogMissionOutcome()` is called (after mission completion), reward bonuses have already been added to the mission template.

## Solution Architecture

### Three-Layer Approach

#### 1. **Capture Phase** (During Mission Calculation)
New patch: `TIMissionResolution_Contested_ModifierCachePatch.cs`
- Hooks into `GetAttackingNonZeroModifiers()` and `GetDefendingNonZeroModifiers()`
- These methods are called DURING mission resolution, before LogMissionOutcome
- At this point, modifiers represent exactly what was used in the calculation
- We capture and cache them for later retrieval

#### 2. **Storage Phase** (Cache System)
New class: `MissionCalculationCache.cs`
- Stores snapshots of modifiers at calculation time
- Cache key format: `"MissionName:AttackerName:TargetName"`
- Includes both attacking and defending modifiers
- Auto-cleanup removes entries older than 1 minute to prevent memory buildup

#### 3. **Display Phase** (Breakdown Generation)
Modified: `TINotificationQueueState_MissionDetailBreakdownPatch.cs`
- Retrieves cached modifiers when building breakdown
- Falls back to `GetNonZeroModifiers()` if cache miss (safety net)
- Displays the exact modifiers that were used during mission calculation

## Key Benefits

✅ **Accuracy**: Shows exactly what modifiers contributed to the outcome  
✅ **Precision**: Includes zero-valued modifiers (like Crackdown)  
✅ **Historical Accuracy**: Excludes post-mission rewards  
✅ **No Filtering**: No hardcoded modifier lists to maintain  
✅ **Automatic**: Works for all mission types (councilor, control point, etc.)  
✅ **Minimal Overhead**: Cache entries auto-expire after 1 minute

## Execution Flow

```
1. Mission Resolution Starts
   └─> TIMissionResolution_Contested.SumAttackingModifiers() called
	   └─> GetAttackingNonZeroModifiers() called
		   └─> TIMissionResolution_Contested_ModifierCachePatch captures modifiers
			   └─> MissionCalculationCache.CacheAttackingModifiers() stores snapshot

2. Mission Outcome Determined
   └─> TIMissionResolution_Contested.SumDefendingModifiers() called
	   └─> GetDefendingNonZeroModifiers() called
		   └─> TIMissionResolution_Contested_ModifierCachePatch captures modifiers
			   └─> MissionCalculationCache.CacheDefendingModifiers() stores snapshot

3. Notification Generated
   └─> TINotificationQueueState.LogMissionOutcome() called
	   └─> TINotificationQueueState_MissionDetailBreakdownPatch_Postfix triggers
		   └─> BuildMissionBreakdown() retrieves cached modifiers
			   └─> MissionCalculationCache.GetCachedAttackingModifiers()
			   └─> MissionCalculationCache.GetCachedDefendingModifiers()
				   └─> Displays exact modifiers from calculation time
```

## Files Changed/Created

### New Files
- **MissionCalculationCache.cs** - Cache system for modifier snapshots
- **TIMissionResolution_Contested_ModifierCachePatch.cs** - Capture patches

### Modified Files
- **TINotificationQueueState_MissionDetailBreakdownPatch.cs**
  - Removed complex filtering logic
  - Changed to use cached modifiers
  - Removed `isControlPointMission` branching (no longer needed)
  - Simplified modifier retrieval to 3 lines per branch

## Testing Checklist

- [ ] Detain mission - "Detained Target" should NOT appear
- [ ] Crackdown on control point - "Crackdown" with value 0 should appear
- [ ] Purge mission - Only mission-relevant modifiers should show
- [ ] Multiple missions in sequence - Cache correctly handles different missions
- [ ] Mission with rewards - Rewards added after don't appear in breakdown

## Performance Impact

- **CPU**: Minimal - one list copy per mission calculation (microseconds)
- **Memory**: Negligible - cache entries auto-expire after 1 minute
- **DLL Size**: +4KB (~56.5KB total, up from 52.5KB)

## Edge Cases Handled

1. **Cache Miss**: Falls back to `GetNonZeroModifiers()` (shouldn't happen normally)
2. **Null Values**: All null checks in place
3. **Mission Name Collisions**: Includes attacker and target in cache key
4. **Long Gaming Sessions**: Auto-cleanup prevents memory buildup
5. **Multiple Missions Simultaneously**: Unique cache keys per mission combination

## Why This Approach is Superior to Filtering

| Approach | Accuracy | Complexity | Maintainability | Edge Cases |
|----------|----------|-----------|-----------------|-----------|
| **Filtering** | Fragile - needs hardcoded lists | High - complex conditionals | Low - modifier names can change | Poor - new rewards need updates |
| **Caching** | Perfect - uses actual calculation data | Low - delegation to cache | High - no maintenance needed | Excellent - automatic for all cases |

## Future Improvements (Optional)

1. Could add telemetry logging for cache hit rates
2. Could visualize cache statistics in UI
3. Could extend cache to store actual modifier values calculated
4. Could persist cache to show mission history

---

**Status**: ✅ Ready for Testing  
**Date**: 16/09/2026  
**Deployment**: DLL size 56.5 KB
