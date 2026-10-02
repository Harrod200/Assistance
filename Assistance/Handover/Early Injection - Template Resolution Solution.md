# Solution: Early Injection to Prevent Null Mission Templates

## Overview
We've implemented an **early injection patch** that resolves null mission templates during initialization, preventing the NullReferenceException crash at its root cause.

## The Problem (Recap)
- Saved Assist missions reference "Assist" template by string name
- During deserialization, template lookup can fail if timing is wrong
- Mission ends up with `activeMission != null` but `activeMission.missionTemplate == null`
- UI code crashes when trying to access `missionTemplate.missionIconImagePath`

## The Solution: Two-Layer Defense

### Layer 1: Early Injection (Primary Fix)
**File:** `TIMissionState_PostGlobalGameStateCreateInit_2_Patch.cs`

Patches `TIMissionState.PostGlobalGameStateCreateInit_2()` which runs **after deserialization completes** but **before any code tries to use the mission**.

**How it works:**
1. Checks if `missionTemplate` is null
2. If null but `templateName` exists, tries to resolve it via `TemplateManager.Find()`
3. Uses reflection to set the `_missionTemplate` backing field (property is read-only)
4. Logs success/failure for debugging

```csharp
[HarmonyPostfix]
public static void Postfix(TIMissionState __instance)
{
	if (__instance.missionTemplate != null)
		return;  // Already valid

	if (!string.IsNullOrEmpty(__instance.templateName))
	{
		TIMissionTemplate template = TemplateManager.Find<TIMissionTemplate>(
			__instance.templateName, false);

		if (template != null)
		{
			// Use reflection to set backing field
			FieldInfo field = typeof(TIMissionState).GetField("_missionTemplate", 
				BindingFlags.Instance | BindingFlags.NonPublic);
			field.SetValue(__instance, template);
		}
	}
}
```

**Advantages:**
- ✅ Fixes the root cause (template resolution)
- ✅ Happens early in initialization, not during UI rendering
- ✅ Ensures template is valid before any code uses it
- ✅ Scalable to other mission template issues
- ✅ Would become unnecessary if vanilla fixes this

### Layer 2: Defensive UI Patch (Safety Net)
**File:** `TICouncilorState_GetCurrentMissionIconPatch.cs`

Patches `GetCurrentMissionIcon()` as a last resort safety net.

**Why keep it:**
- ✅ Extra defense against edge cases
- ✅ Protects against any code path that might still hit null
- ✅ No performance impact (simple null check)
- ✅ Defense-in-depth approach

## Why This Is Better Than Just The Defensive Patch

| Aspect | Defensive Only | With Early Injection |
|--------|---|---|
| **Root cause fixed** | ❌ No | ✅ Yes |
| **Happens during** | UI rendering | Initialization |
| **Scalability** | Won't help other null issues | Applies broadly to all missions |
| **Maintainability** | Temporary band-aid | Structural fix |
| **Future-proofing** | Depends on UI not changing | Works regardless of UI changes |
| **Debugging** | Silent fallback | Logs what was resolved |

## Technical Details

### Why PostGlobalGameStateCreateInit_2?
The TIGameState class has multiple post-initialization hooks:
- `PostGameStateCreateInit_OnCreationOnly_1()` - Runs once on creation
- **`PostGlobalGameStateCreateInit_2()`** - Called after deserialization, good place to fix references
- `PostCanvasManagerCreateInit_3()` - Later in initialization
- ... and more

We chose PostGlobalGameStateCreateInit_2 because:
1. Runs after mission is fully deserialized
2. Runs before UI code tries to use the mission
3. Part of standard initialization lifecycle
4. Easy to intercept with Harmony

### Why Use Reflection?
The `missionTemplate` property is read-only:
```csharp
public TIMissionTemplate missionTemplate
{
	get
	{
		if (this._missionTemplate == null)
			this._missionTemplate = this.GetMyTemplate<TIMissionTemplate>();
		return this._missionTemplate;
	}
}
```

To set it, we access the backing field `_missionTemplate` via reflection:
```csharp
FieldInfo field = typeof(TIMissionState).GetField("_missionTemplate",
	BindingFlags.Instance | BindingFlags.NonPublic);
field.SetValue(__instance, resolvedTemplate);
```

This is safe and standard practice when you need to set read-only properties.

## When Does TemplateManager.Initialize Run?

The Assist template is registered in `AssistTemplateRegistration.cs`:
```csharp
[HarmonyPatch(typeof(TemplateManager), "Initialize")]
internal static class AssistTemplateRegistration
{
	private static void Postfix()
	{
		if (TemplateManager.Find<TIMissionTemplate>("Assist", false) == null)
		{
			TemplateManager.Add(new TIMissionTemplate_Assist(), typeof(TIMissionTemplate));
		}
	}
}
```

**Timeline:**
1. Game starts
2. TemplateManager.Initialize runs
3. **AssistTemplateRegistration.Postfix** registers Assist template ← Template now exists
4. GameControl.Initialize runs
5. Missions deserialize
6. **TIMissionState_PostGlobalGameStateCreateInit_2_Patch.Postfix** resolves any null templates ← Can now find Assist template

This timing ensures the Assist template exists before our patch tries to resolve it.

## Testing Recommendations

1. **Load a save with active Assist mission**
   - Should load without NullReferenceException
   - Check debug logs for resolution messages

2. **Expected log output:**
   ```
   [TIMissionState_PostGlobalGameStateCreateInit_2] Successfully resolved template 'Assist' for mission [id]
   ```

3. **If template can't be found:**
   ```
   [TIMissionState_PostGlobalGameStateCreateInit_2] WARNING: Could not resolve template 'Assist' for mission [id]
   ```
   - This would be caught by the defensive GetCurrentMissionIcon patch

## Files Involved

| File | Purpose |
|------|---------|
| `TIMissionState_PostGlobalGameStateCreateInit_2_Patch.cs` | Early injection: Resolve null templates |
| `TICouncilorState_GetCurrentMissionIconPatch.cs` | Defensive: Handle null templates if they slip through |
| `AssistTemplateRegistration.cs` | Ensures Assist template is registered early |
| `TIGameState_LoadPatch.cs` | Clears session state during load |

## Design Philosophy

This solution exemplifies **defense in depth**:
1. **Primary defense:** Fix the root cause early (PostGlobalGameStateCreateInit_2)
2. **Secondary defense:** Guard critical UI paths (GetCurrentMissionIcon)
3. **Logging:** Debug output helps identify if primary defense is working

This is much better than just patching the UI method, which would hide the real issue.

## Future Improvement
If vanilla game ever implements proper template resolution for deserialized missions, this patch could be removed. The defensive UI patch would remain as an extra safety layer.

---

**Previous Attempts:**
- First approach: Tried to fix via AssistBonusTracker validation (wrong root cause)
- Second approach: Added defensive patch on GetCurrentMissionIcon (necessary but incomplete)
- **Third approach (current): Early injection to resolve templates before they're needed (complete fix)**
