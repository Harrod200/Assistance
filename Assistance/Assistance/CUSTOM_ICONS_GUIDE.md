# Mission Icon Selection & Custom Icons - Implementation Guide

## Overview

The Assistance Mod currently uses a vanilla game icon (`councilor_missions/ICO_inspire`) for the Assist mission. This guide explains how mission icons are selected and how to incorporate a custom one.

---

## Current Implementation

### In TIMissionTemplate_Assist.cs (Line 74):

```csharp
this.missionIconImagePath = "councilor_missions/ICO_inspire";
```

This references a **vanilla game asset** from the game's built-in asset bundles.

---

## How Mission Icons Work

### Icon Path Structure

Mission icons use a path format that points to Unity Sprites in asset bundles:
```
"path/to/icon_name"
```

Examples:
- Vanilla: `"councilor_missions/ICO_inspire"`
- Vanilla: `"operations/Launch_Nuke"` (from MoreRealisticNukes mod)
- Custom: `"mod_custom_icon/assist_mission_icon"`

### Icon Variants

The game automatically generates two icon variants from a single base path:
```csharp
public string missionIconImagePath_On
{
	get { return missionIconImagePath + "_on"; }  // "councilor_missions/ICO_inspire_on"
}

public string missionIconImagePath_Off
{
	get { return missionIconImagePath + "_off"; }  // "councilor_missions/ICO_inspire_off"
}
```

**Requirements:**
- You must provide BOTH variants: `IconName_on` and `IconName_off`
- `_on` = icon when mission is available/selected
- `_off` = icon when mission is unavailable/not selected

---

## How Icons Are Loaded

### Asset Loading Pipeline

1. **Request:** Mission UI needs an icon sprite
2. **Path Lookup:** System uses `missionIconImagePath` to find asset
3. **AssetBundleManager:** Loads sprite from asset bundle via `LoadAsset<Sprite>(path)`
4. **Caching:** Sprite is cached in `_cachedAssets` dictionary
5. **UI Assignment:** Sprite assigned to Image component

### Asset Bundle System

The game uses Unity's AssetBundle system:
- Assets are pre-compiled into `.bundle` files
- ModManager scans `Mods/Enabled/` for `.assetbundle` files
- Each `.assetbundle` must have an accompanying `.manifest` file
- Mod assets loaded through same system as vanilla assets

---

## Option 1: Use Vanilla Icons (No Custom Asset Required)

### Available Vanilla Mission Icons

You can reference any existing game icon:

| Icon Path | Description | Use Case |
|-----------|-------------|----------|
| `councilor_missions/ICO_inspire` | Light bulb icon | Support/help missions |
| `councilor_missions/ICO_infiltrate` | Spy icon | Covert operations |
| `councilor_missions/ICO_travel` | Arrow icon | Movement missions |
| `councilor_missions/ICO_investigate` | Magnifying glass icon | Investigation missions |
| `councilor_missions/ICO_construct` | Gear icon | Building/construction |
| `councilor_missions/ICO_train` | Shield icon | Military training |
| `councilor_missions/ICO_propagandize` | Megaphone icon | Information warfare |

### Implementation

Simply change the path in TIMissionTemplate_Assist.cs:

```csharp
// Current (Light bulb - help/support):
this.missionIconImagePath = "councilor_missions/ICO_inspire";

// Alternative (Spy - covert):
this.missionIconImagePath = "councilor_missions/ICO_infiltrate";

// Alternative (Magnifying glass - investigation):
this.missionIconImagePath = "councilor_missions/ICO_investigate";
```

**Advantage:** No asset creation needed, instant change  
**Disadvantage:** Reuses vanilla icons, less unique

---

## Option 2: Create Custom Asset Bundle with Icon (Advanced)

### Requirements

1. **Unity Editor** (2020.3.49f1 - matches game version)
2. **Image Editor** (Photoshop, GIMP, or similar) to create PNG sprites
3. **Knowledge of Unity AssetBundles** (complex setup)

### Step-by-Step Process

#### Step 1: Create Icon Sprites

**File format:**
- PNG image files
- Transparent background (PNG with alpha)
- Recommended size: 256x256 or 512x512 pixels
- Create two versions: `assist_icon_on.png` and `assist_icon_off.png`

**Visual differences:**
- `_on`: Bright, saturated colors (when available)
- `_off`: Desaturated or greyed out (when unavailable)

Example:
```
assist_icon_on.png   - Bright blue handshake icon
assist_icon_off.png  - Desaturated grey handshake icon
```

#### Step 2: Set Up Unity Project

1. Create new Unity 2020.3.49f1 project
2. Create folder: `Assets/CustomAssets/assist_icon/`
3. Import both PNG files to this folder
4. For EACH PNG in Inspector:
   - Set **Texture Type** to `Sprite (2D and UI)`
   - Set **Sprite Mode** to `Single`
   - Apply settings

#### Step 3: Create AssetBundle

1. Select both sprites in Project window
2. In Inspector, find **AssetBundle** dropdown at bottom
3. Create new bundle: `enter "assist_icons"` → press Enter
4. Set **Sprite Name** for variant:
   - First sprite: name it `assist_mission_icon_on`
   - Second sprite: name it `assist_mission_icon_off`

#### Step 4: Build AssetBundle

1. Create script: `Assets/Editor/BuildAssetBundles.cs`

```csharp
using UnityEditor;
using System.IO;

public class BuildAssetBundles
{
	[MenuItem("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles()
	{
		string outputPath = "Assets/AssetBundles";
		if (!Directory.Exists(outputPath))
		{
			Directory.CreateDirectory(outputPath);
		}
		BuildPipeline.BuildAssetBundles(
			outputPath,
			BuildAssetBundleOptions.None,
			BuildTarget.StandaloneWindows64
		);
	}
}
```

2. Go to **Assets → Build AssetBundles**
3. Output files created in `Assets/AssetBundles/`:
   - `assist_icons` (no extension)
   - `assist_icons.manifest`

#### Step 5: Deploy to Mod Folder

Copy to: `C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\`

```
AssistMission/
├── Assistance.dll
├── ModInfo.json
├── Settings.xml
├── TIMissionTemplate.en
├── assist_icons           ← AssetBundle file
└── assist_icons.manifest  ← Manifest file
```

#### Step 6: Update Code

In TIMissionTemplate_Assist.cs:

```csharp
// Changed from vanilla icon to custom asset bundle icon
this.missionIconImagePath = "assist_icons/assist_mission_icon";
// Game will automatically look for:
// - assist_icons/assist_mission_icon_on
// - assist_icons/assist_mission_icon_off
```

**Advantage:** Fully custom, unique branding  
**Disadvantage:** Complex setup, requires Unity editor and AssetBundle knowledge

---

## Option 3: Reference Another Mod's Asset Bundle (Easiest Custom)

### How to Reference Other Mod Assets

Other mods' asset bundles are loaded into the same system. You can reference them:

```csharp
// Reference icon from "More Realistic Nukes" mod
this.missionIconImagePath = "operations/Launch_Nuke";
```

**Advantage:** Reuses community work, no setup needed  
**Disadvantage:** Depends on other mod being installed

---

## Troubleshooting

### Icon Not Showing

**Symptom:** Mission appears but icon is missing or shows error

**Causes & Solutions:**
1. **Wrong path format** → Check spelling and case sensitivity
2. **AssetBundle not loaded** → Ensure `.assetbundle` and `.manifest` files present in mod folder
3. **Missing `_on` or `_off` variant** → Both variants must exist
4. **Mod load order issue** → Increase `LoadOrder` in ModInfo.json if referencing another mod

### Icon Looks Blurry

**Solution:** Use higher resolution source (512x512 or 1024x1024)

### Both Variants Show Same Icon

**Check:** Ensure `_on` and `_off` are actually different sprites in the bundle

---

## Current Assistance Mod Status

**Current Icon:** `councilor_missions/ICO_inspire`  
**Icon Type:** Vanilla game asset (light bulb)  
**Reason:** Generic support mission icon matches mission purpose

### Suggestions for Custom Icon

If creating a custom icon, consider:
- **Handshake** - Represents assistance/support between two councilors
- **Boost/Arrow** - Represents stat transfer/empowerment
- **Plus/Addition** - Represents adding bonuses
- **Link/Connection** - Represents temporary connection between councilors

The current vanilla icon (light bulb/inspiration) works well, but a handshake would be more semantically accurate.

---

## Implementation Checklist for Custom Icon

### Quick Start (Use Vanilla)
- [ ] Change `missionIconImagePath` to vanilla icon path
- [ ] Rebuild mod
- [ ] Test in game

### Full Custom (AssetBundle)
- [ ] Create two PNG sprites (on/off variants)
- [ ] Set up Unity 2020.3.49f1 project
- [ ] Import sprites as Sprite type
- [ ] Create AssetBundle and build it
- [ ] Copy `.bundle` and `.manifest` to mod folder
- [ ] Update code with new icon path
- [ ] Rebuild mod
- [ ] Test in game

### Reference Other Mod
- [ ] Find icon path from other mod
- [ ] Update code with icon path
- [ ] Ensure other mod is installed/loaded first
- [ ] Rebuild mod
- [ ] Test in game

---

## Related Documentation

- **AI_DEVELOPER_SUMMARY.md** - Mission template details
- **CODE_REFERENCE.md** - TIMissionTemplate_Assist class documentation
- **Game File:** `TerraInvicta_Data/StreamingAssets/Templates/TIMissionTemplate.json` - Lists vanilla icons

---

## Resources

- [Unity AssetBundles Guide](https://docs.unity3d.com/Manual/AssetBundlesIntro.html)
- [Terra Invicta Mod Documentation](https://github.com/Harrod207/Assistance) (this repo)
- [More Realistic Nukes Mod](https://github.com) - Example of custom mission with vanilla icon reference

