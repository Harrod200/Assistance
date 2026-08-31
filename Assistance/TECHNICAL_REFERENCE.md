# Terra Invicta AssistMission Mod - Technical Reference

**Quick Reference for Developers**

---

## 🔧 Quick Facts

| Item | Value |
|------|-------|
| Mod ID | AssistMission |
| Assembly | Assistance.dll |
| Entry Method | Assistance.Main.Load |
| Target Framework | .NET 4.8 |
| Game Version | Terra Invicta 1.0.38+ |
| UMM Version | 0.33.0.0+ |
| Mission Name | "Assist" |
| Mission Display | "Assist Councilor" |
| Affected Stats | 7 (all except Loyalty/ApparentLoyalty) |
| Default Assist % | 25% |
| Load Order | 1200 |
| DLL Size | ~11 KB |

---

## 📂 Project Files

```
Assistance/
  ├─ Assistance.csproj              [Project config, assembly references]
  ├─ Main.cs                        [UMM entry, GUI, patch setup]
  ├─ Settings.cs                    [Config storage]
  ├─ TIMissionTemplate_Assist.cs    [Mission definition]
  ├─ TIMissionEffect_Assist.cs      [Effect logic]
  ├─ AssistMissionBootstrapPatch.cs [Harmony patch]
  └─ Properties/AssemblyInfo.cs     [Version info]
```

---

## 🎯 Data Flow

```
Game Start
	↓
SolarSystemBootstrap.Initialize (Harmony Postfix)
	↓
AssistMissionBootstrapPatch.Postfix()
	├─ TemplateManager.Add(new TIMissionTemplate_Assist())
	└─ Grant to all TICouncilorTypeTemplate
	↓
Councilor gets "Assist" mission in available list
	↓
Player selects Assist + target councilor
	↓
Mission resolves (TIMissionResolution_Automatic → auto-success)
	↓
TIMissionEffect_Assist.ApplyEffect(mission, target, outcome)
	├─ Get source & target councilor
	├─ For each stat: bonus = source[stat] * (assistPercentage / 100)
	├─ ModifyAttribute(stat, bonus)
	└─ Return description string
	↓
Mission marked complete, XP awarded
```

---

## 💻 Code Snippets for Common Tasks

### Get All Councilor Stats
```csharp
CouncilorAttribute[] allStats = new CouncilorAttribute[] {
	CouncilorAttribute.Persuasion,
	CouncilorAttribute.Investigation,
	CouncilorAttribute.Espionage,
	CouncilorAttribute.Command,
	CouncilorAttribute.Administration,
	CouncilorAttribute.Science,
	CouncilorAttribute.Security
};

foreach (var stat in allStats) {
	int value = councilor.GetAttribute(stat, true, true, true, false, false, false);
}
```

### Register a Mission Template
```csharp
TemplateManager.Add(new TIMissionTemplate_Assist(), typeof(TIMissionTemplate), true);
```

### Grant Mission to Councilor Type
```csharp
councilorType.missionNames = Append(councilorType.missionNames, "Assist");
councilorType._missions = null; // Clear cache
```

### Create Mission Condition
```csharp
this.conditions = new List<TIMissionCondition> {
	new TIMissionCondition_TargetInRange(),
	new TIMissionCondition_CouncilorOnEarth(),
	new TIMissionCondition_HasNukes() // Example custom condition
};
```

### Harmony Patch Template
```csharp
[HarmonyPatch(typeof(SolarSystemBootstrap), "Initialize")]
internal static class MyBootstrapPatch {
	public static void Postfix() {
		// Runs after original method
	}
}
```

### Draw GUI Slider
```csharp
Main.settings.assistPercentage = Main.DrawNamedFloat(
	"Assist Percentage (0-100%)", 
	Main.settings.assistPercentage, 
	200f
);
Main.settings.assistPercentage = Mathf.Clamp(Main.settings.assistPercentage, 0f, 100f);
```

---

## 🔌 Assembly References (in .csproj)

```xml
<Reference Include="Assembly-CSharp">
  <HintPath>C:\Games\Steam\steamapps\common\Terra Invicta\TerraInvicta_Data\Managed\Assembly-CSharp.dll</HintPath>
  <Private>False</Private>
</Reference>

<Reference Include="UnityModManager">
  <HintPath>C:\Games\Steam\steamapps\common\Terra Invicta\TerraInvicta_Data\Managed\UnityModManager\UnityModManager.dll</HintPath>
  <Private>False</Private>
</Reference>

<Reference Include="0Harmony">
  <HintPath>C:\Games\Steam\steamapps\common\Terra Invicta\TerraInvicta_Data\Managed\UnityModManager\0Harmony.dll</HintPath>
  <Private>False</Private>
</Reference>
```

---

## 📋 Common Modifications Checklist

### Add New Stat to Transfer
- [ ] File: TIMissionEffect_Assist.cs
- [ ] Line: ~22-30 (stats array)
- [ ] Add: `CouncilorAttribute.NewStat`
- [ ] Test: Build and verify stat appears in result message

### Change Default Assist Percentage
- [ ] File: Settings.cs
- [ ] Line: 7
- [ ] Change: `= 25f;` to desired value
- [ ] Rebuild and test GUI slider

### Add Mission Cost
- [ ] File: TIMissionTemplate_Assist.cs
- [ ] Line: ~60 (this.cost = null)
- [ ] Replace with:
```csharp
this.cost = new TIMissionCost_Bonus {
	resourceType = FactionResource.IP,
	costScale = 10f
};
```

### Add Mission Condition
- [ ] File: TIMissionTemplate_Assist.cs
- [ ] Line: ~51 (this.conditions list)
- [ ] Add: `new TIMissionCondition_Custom()`

### Change Mission Resolution Type
- [ ] File: TIMissionTemplate_Assist.cs
- [ ] Line: ~34 (this.resolutionMethod)
- [ ] Replace TIMissionResolution_Automatic with:
  - TIMissionResolution_Contested (needs modifiers)
  - TIMissionResolution_* (other types)

### Localize Mission Text
- [ ] Create: `ModInfo.json` (already done)
- [ ] Create: `TIMissionTemplate.en` (English)
- [ ] Create: `TIMissionTemplate.chs` (Chinese)
- [ ] Format: XML with key="TIMissionTemplate.description.Assist" entries

---

## 🧪 Testing Commands

### Build
```powershell
# In VS, Build → Assistance (or Ctrl+Shift+B)
# Output: bin\Debug\Assistance.dll
```

### Deploy to Mods Folder
```powershell
Copy-Item -Path 'Assistance\bin\Debug\Assistance.dll' `
  -Destination 'C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\' `
  -Force
```

### Check Mod Loads
```
1. Launch Terra Invicta
2. Open UMM Mod Manager
3. Look for "Councilor Assist Mission" in list
4. Verify "Assist Councilor" mission appears for any councilor
```

### Read Game Logs
```
File: C:\Users\Chris\AppData\LocalLow\Pavonis Interactive\TerraInvicta\Player.log
Search for: "Assist mission registered" or "Assistance" to find mod logs
```

---

## 🔍 Debugging Tips

| Issue | Check |
|-------|-------|
| Mod doesn't appear in list | ModInfo.json in correct folder, AssemblyName/EntryMethod correct |
| Mission doesn't show up | Check AssistMissionBootstrapPatch logged success in Player.log |
| Stats don't transfer | Verify TIMissionEffect_Assist.ApplyEffect is called (add logging) |
| Settings not saving | Settings class inherits ModSettings, Save() method exists |
| GUI crashes | Verify GUILayout calls are inside OnGUI method, not called at wrong time |
| Assembly not found | Check .csproj HintPath paths still exist, rebuild solution |

---

## 🎮 In-Game Debug Verification

```
To verify mod loaded:
1. Launch game with mod enabled
2. Hover over mission to see description
3. Select councilor as target
4. Execute mission
5. Check councilor stats in detail panel
6. Verify UI message shows stat transfers

Expected output:
"Assisted [CouncilorName]: Persuasion +20, Command +18, ..."
```

---

## 📊 Stat Transfer Formula

```
bonusPerStat = floor(sourceAttribute * (assistPercentage / 100))
minimumBonus = 1 (if sourceAttribute > 0)

Example with assistPercentage = 25%:
- Source has Persuasion 80 → Target gets +20
- Source has Persuasion 12 → Target gets +3
- Source has Persuasion 0 → Target gets +0
- Source has Persuasion 1-3 → Target gets +1 (minimum)
```

---

## 🔗 Key API Reference

### TICouncilorState Methods
```csharp
int GetAttribute(CouncilorAttribute attr, bool param2, bool param3, bool param4, bool param5, bool param6, bool param7)
void ModifyAttribute(CouncilorAttribute attr, int amount)
string displayName { get; }
TIFactionState faction { get; }
```

### TIMissionState Properties
```csharp
TICouncilorState councilor { get; }
TIGameState target { get; }
string missionTemplateName { get; }
TIMissionOutcome missionOutcome { get; }
```

### TIMissionEffect
```csharp
abstract string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
bool MissionSuccess(TIMissionOutcome outcome)
bool MissionFailure(TIMissionOutcome outcome)
```

### TemplateManager
```csharp
void Add<T>(T template, Type templateType, bool overwrite)
T GetTemplate<T>(string name)
List<T> GetAllTemplates<T>(bool includeDisabled)
```

---

## 📚 Documentation Links

- **Game Modding:** Game devs at Pavonis Interactive maintain Terra Invicta
- **UMM Docs:** https://www.nexusmods.com/site/mods/21 (Universal Mod Manager)
- **Harmony:** https://harmony.pardeike.net/ (Patching library)
- **C# 4.8 Docs:** https://docs.microsoft.com/en-us/dotnet/csharp/

---

## 🎓 Learning Path for Future Features

1. **Understand TIMissionTemplate** - Build complex mission with cost/conditions
2. **Study TIMissionModifier** - Add success modifiers based on councilor skills
3. **Explore TIMissionCondition** - Create custom condition (e.g., "same faction")
4. **Learn TIMissionResolution_Contested** - Make assist have success/failure outcomes
5. **Investigate LocalizationManager** - Add multi-language support
6. **Study Traits/Orgs** - Grant assist to specific councilor types via traits

---

**Version:** 1.0.0  
**Last Updated:** 2026-08-31  
**Status:** Production Ready
