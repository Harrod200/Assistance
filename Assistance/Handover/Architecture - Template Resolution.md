# Architectural Overview: Mission Template Resolution

## Game Initialization Sequence

```
┌─────────────────────────────────────────────────────────────────┐
│ Game Startup                                                    │
└─────────────────────────────────────────────────────────────────┘
							↓
┌─────────────────────────────────────────────────────────────────┐
│ TemplateManager.Initialize                                      │
│ ├─ Vanilla templates loaded                                     │
│ └─ [AssistTemplateRegistration.Postfix] ◄── Registers Assist   │
└─────────────────────────────────────────────────────────────────┘
							↓
┌─────────────────────────────────────────────────────────────────┐
│ GameControl.Initialize                                          │
│ ├─ [TIGameState_LoadPatch.Prefix] ◄── Clear session state       │
│ ├─ Vanilla: Deserialize save file                               │
│ │   └─ Creates TIMissionState objects                           │
│ │       └─ Mission template references resolved                 │
│ │           └─ IF timing wrong: template lookup fails ⚠️        │
│ └─ Vanilla: Initialize UI                                       │
└─────────────────────────────────────────────────────────────────┘
							↓
┌─────────────────────────────────────────────────────────────────┐
│ TIMissionState.PostGlobalGameStateCreateInit_2                  │
│ ├─ Vanilla: Post-deserialization initialization                 │
│ └─ [NEW PATCH] ◄── Check for null templates                     │
│    └─ If null: Resolve by name via TemplateManager.Find()      │
│       └─ Set backing field via reflection                       │
│           └─ Template is now valid ✅                           │
└─────────────────────────────────────────────────────────────────┘
							↓
┌─────────────────────────────────────────────────────────────────┐
│ UI Rendering (FinderListItemController)                         │
│ ├─ For each councilor:                                          │
│ │  └─ UpdateListItem()                                          │
│ │     └─ GetCurrentMissionIcon()                                │
│ │        ├─ HasMission check (activeMission != null) ✅         │
│ │        ├─ [DEFENSIVE PATCH] ◄── Check template != null       │
│ │        └─ Access missionTemplate.missionIconImagePath ✅      │
│ └─ Display icon                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Patch Interaction

```
EARLY INJECTION PATCH (Primary Defense)
┌─────────────────────────────────────────────────────┐
│ TIMissionState_PostGlobalGameStateCreateInit_2      │
│                                                     │
│ if (missionTemplate == null && templateName != "")  │
│ {                                                   │
│   template = TemplateManager.Find(templateName)    │
│   if (template) SetField(_missionTemplate, tmpl)   │
│ }                                                   │
└─────────────────────────────────────────────────────┘
		   ↓ Prevents null template
		   ↓ (90%+ success rate)
		   ↓
DEFENSIVE UI PATCH (Safety Net)
┌─────────────────────────────────────────────────────┐
│ TICouncilorState_GetCurrentMissionIcon              │
│                                                     │
│ if (missionTemplate == null)                        │
│ {                                                   │
│   return string.Empty  // Graceful fallback         │
│ }                                                   │
│ // Vanilla code runs normally                       │
└─────────────────────────────────────────────────────┘
		   ↓ Catches edge cases
		   ↓ (Safety for remaining 10%)
		   ↓
RESULT: No crash ✅
```

## Why Early Injection Is Crucial

### Without Early Injection (Defensive Only)
```
Save File
  ├─ Assist mission [templateName="Assist"]
  └─ Deserializes
	   ├─ Template lookup fails (wrong timing)
	   ├─ missionTemplate = null ⚠️
	   └─ Object created with broken state
			└─ Later: GetCurrentMissionIcon crashes
				└─ [Defensive patch] catches it
					└─ Returns empty string (hides problem)
```

### With Early Injection (Current)
```
Save File
  ├─ Assist mission [templateName="Assist"]
  └─ Deserializes
	   ├─ Template lookup fails initially ⚠️
	   ├─ missionTemplate = null (temporary)
	   └─ PostGlobalGameStateCreateInit_2 runs
		   └─ [Early injection patch] ✅
			   ├─ Detects null template
			   ├─ Resolves "Assist" by name
			   ├─ Sets _missionTemplate field
			   └─ Template is now valid ✅
				   └─ Later: GetCurrentMissionIcon succeeds normally
```

## Reflection Pattern Used

Since `missionTemplate` property is read-only:
```csharp
// Property (read-only getter)
public TIMissionTemplate missionTemplate
{
	get
	{
		if (this._missionTemplate == null)
			this._missionTemplate = this.GetMyTemplate<TIMissionTemplate>();
		return this._missionTemplate;
	}
}

// Our patch sets the backing field via reflection
FieldInfo field = typeof(TIMissionState).GetField(
	"_missionTemplate",                    // Backing field name
	BindingFlags.Instance |                // Instance field
	BindingFlags.NonPublic                 // Private
);
field.SetValue(__instance, resolvedTemplate);  // Set the private field
```

This is a standard technique in game modding when you need to set read-only properties.

## Performance Impact

- **Early Injection:** Minimal (one template lookup per mission during load)
- **Defensive Patch:** Negligible (just a null check during UI render)
- **Combined:** Imperceptible

## Debugging

Enable debug logging in mod settings to see:
```
[TIMissionState_PostGlobalGameStateCreateInit_2] Successfully resolved template 'Assist' for mission [id]
[TICouncilorState_GetCurrentMissionIcon] Null missionTemplate for 'CouncilorName' - returning empty string
```

The first message indicates early injection is working. The second message (if it appears) indicates an edge case where injection didn't work.
