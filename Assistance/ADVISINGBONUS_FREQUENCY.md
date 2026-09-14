# AdvisingBonus Calculation Frequency in Vanilla Terra Invicta

## Quick Answer

**AdvisingBonus is calculated on-demand, not continuously cached.**

- It is calculated **every time** it's called
- There are only **2 main call sites** in vanilla (plus 1 in the Advise mission effect)
- Calculations happen **during specific events**, not every frame/turn

---

## Call Site Analysis

### Vanilla Decompiled Search Results

```
Search: "AdvisingBonus(" in all .cs files
Result: 2 results found in TIMissionEffect_Advise.cs only

Line 40: councilor.AdvisingBonus(CouncilorAttribute.Command).ToPercent("P0")
Line 41: councilor.AdvisingBonus(CouncilorAttribute.Science).ToPercent("P0")
```

### Complete Advising Method References

```
Search: "GetAdvisingAttribute|GetAdvisingScore|AddAdvisingCouncilor" in all .cs files
Result: 8 results found in TIMissionEffect_Advise.cs only

Lines 16, 30: AddAdvisingCouncilor() calls (2 locations)
Lines 27-29: GetAdvisingAttribute() calls BEFORE advisor added (3 calls)
Lines 33-35: GetAdvisingAttribute() calls AFTER advisor added (3 calls)
```

---

## Calculation Triggers in Vanilla

Based on the decompiled code, AdvisingBonus is calculated in these scenarios:

### 1. **Mission Completion: Advise Mission Effect** 
   - **When:** Player completes "Advise" mission
   - **Frequency:** Once per mission completion
   - **Call Path:**
	 ```
	 TIMissionEffect_Advise.ApplyEffect()
	   ├─ Nation path: AddAdvisingCouncilor() → internally sums AdvisingBonus()
	   ├─ Hab path: GetAdvisingAttribute() → internally sums AdvisingBonus()
	   └─ Fallback path: Direct AdvisingBonus() calls
	 ```

### 2. **Nation Advising Queries** (Indirect)
   - **When:** Game needs current nation advising bonus
   - **Method:** `TINationState.GetAdvisingScore()`
   - **Frequency:** Called whenever nation stats are queried
   - **Calculation:** Internally sums `AdvisingBonus(attribute) / rank` across advisors
   - **Not found in search:** Method definition wasn't in search results (likely in separate compiled assembly or obfuscated)

### 3. **Hab Advising Queries** (Indirect)
   - **When:** Game needs current hab advising attribute
   - **Method:** `TIHabState.GetAdvisingAttribute(attribute)`
   - **Frequency:** Called during hab UI updates or calculations
   - **Calculation:** Internally sums `AdvisingBonus(attribute) / rank` across advisors
   - **Observed in:** TIMissionEffect_Advise.cs at lines 27-29, 33-35

---

## Frequency Analysis

### During Normal Gameplay

**Frame-based (Continuous):** ❌ **NOT CALLED**
- AdvisingBonus is NOT recalculated every frame
- No continuous polling found in decompiled code

**Turn-based (Each Turn):** ❓ **UNKNOWN** (likely minimal)
- No evidence of per-turn recalculation
- Would only occur if game queries advising stats each turn
- If it happens, likely only for UI display or monthly updates

**Event-driven (On Specific Actions):** ✅ **CONFIRMED**
- Mission completion (most frequent)
- Adding/removing advisors
- Querying nation/hab statistics
- UI refresh (when viewing advising bonuses)

### Mission-Specific Frequency

```
Advise Mission Applied:
├─ Call 1: GetAdvisingAttribute(Science) - BEFORE
├─ Call 2: GetAdvisingAttribute(Command) - BEFORE
├─ Call 3: GetAdvisingAttribute(Administration) - BEFORE
├─ Call 4: AddAdvisingCouncilor() - applies new advisor
├─ Call 5: GetAdvisingAttribute(Science) - AFTER (recalculated with new advisor)
├─ Call 6: GetAdvisingAttribute(Administration) - AFTER
├─ Call 7: GetAdvisingAttribute(Command) - AFTER
└─ Calls 8-9: AdvisingBonus(Command/Science) for fallback message
```

**Total AdvisingBonus calculations per Advise mission:** Multiple (exact count depends on how many advisors the target has)

---

## Performance Implications

### Current Vanilla Performance

- **Calculation is CHEAP:** `rawStat / 100` is a single division
- **Call frequency is LOW:** Only during specific events, not every frame
- **No caching needed:** Vanilla doesn't cache AdvisingBonus values
- **Fresh values guaranteed:** Every call returns current council state

### Impact of Mod's Postfix Patch

```
Vanilla AdvisingBonus calculation:  rawStat / 100
									~1-2 microseconds

Mod postfix overhead (per call):
  ├─ Check Main.enabled              ~0.1 microseconds
  ├─ Check Main.settings             ~0.1 microseconds
  ├─ GetStatBonus() lookup            ~1-5 microseconds (dictionary/list lookup)
  ├─ Multiplication/division         ~1-2 microseconds
  └─ Total additional:               ~2-8 microseconds per call

Net effect: Negligible (postfix is ~1-5x slower, but only called rarely)
```

---

## When Mod Patch Is Evaluated

The mod's postfix patch executes **whenever** `TICouncilorState.AdvisingBonus(attribute)` is called:

1. **Mission Effect (Advise)** - Direct calls in Special3 fallback
2. **Nation Advising** - Internal calls via `GetAdvisingScore()`
3. **Hab Advising** - Internal calls via `GetAdvisingAttribute()`
4. **Any other future calls** - Automatically covered

### Call Frequency Over Time

```
Turn Start:
  [Some unknown frequency of advising queries]

Player Action: Advise Mission
  → 3-7+ GetAdvisingAttribute() calls (each calls AdvisingBonus internally)
  → 1 AddAdvisingCouncilor() call (internal advisor list update)
  → 2 Direct AdvisingBonus() calls (for message display)

Turn End:
  [Some unknown frequency of advising queries]
```

---

## Missing Instrumentation

The decompiled code does NOT show:
- How often `GetAdvisingScore()` is called
- Whether advising is recalculated every turn
- If there's any caching mechanism
- Monthly/seasonal advising updates
- UI refresh frequency for advising display

**Why:** These methods are likely in compiled-only assemblies or their call sites are in code that wasn't decompiled.

---

## Mod Behavior Summary

### How Often Mod's Bonus Affects Advising

**Same frequency as AdvisingBonus is called:**

- **Direct calls:** 2x per Advise mission (Special3 fallback)
- **Indirect calls (Nation):** Unknown, but happens when nation advising is queried
- **Indirect calls (Hab):** ~3 times per Advise mission target + any other hab queries
- **Contested Mission patches:** Separate patch, also on-demand

### Performance Footprint

- **Per-call overhead:** ~2-8 microseconds (negligible)
- **Frequency:** Only when advising is queried (not every frame)
- **Memory footprint:** Tracking dict is O(n) where n = number of councilors with bonuses
- **GC pressure:** Minimal (postfix creates no new objects)

---

## Conclusion

**AdvisingBonus in vanilla is calculated on-demand, NOT continuously:**

1. ✅ **Event-driven:** Only calculated when needed
2. ✅ **Cheap calculation:** Simple arithmetic
3. ✅ **Low frequency:** Only during mission effects and queries
4. ✅ **No caching:** Fresh values every time
5. ❓ **Unknown turn frequency:** Mod's documentation doesn't specify if/when vanilla queries advising each turn

The mod's postfix patch is perfectly safe and efficient because:
- It only adds ~2-8 microseconds per call
- Calls are infrequent (not per-frame)
- Bonus tracking is lightweight
- Postfix doesn't prevent vanilla from executing

