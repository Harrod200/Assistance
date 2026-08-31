# Assistance Project - Documentation Index

**Project:** Terra Invicta Councilor Assist Mission Mod  
**Status:** ✅ COMPLETE - Fixed initial crash, ready for testing  
**Version:** 1.0.1 (crash fix applied)  
**Last Updated:** 2026-08-31

---

## 🔄 Latest Update (Aug 31, 13:47)

**Issue Found & Fixed:** Game crashed when hovering over mission UI  
**Root Cause:** Mission resolution type needed proper modifier lists  
**Solution:** Changed from TIMissionResolution_Automatic to Contested with Persuasion modifier  
**Result:** Mission now displays correctly in UI without crashes  

See **CRASH_FIX_LOG.md** for full technical details.

---

## 📚 Documentation Structure

This project includes comprehensive documentation for easy handoff to future AI developers. All files are in the `Assistance\` directory.

### Core Documentation Files

#### 0. **CRASH_FIX_LOG.md** (2,500 lines when formatted)
   - **For:** Understanding the UI crash that occurred and how it was fixed
   - **Contains:** Detailed crash analysis, root cause, solution, lessons learned
   - **Read this if:** Game crashes occur or you need to understand mission resolution mechanics
   - **Key sections:**
     - Crash symptom and stack trace
     - Root cause analysis (NullReferenceException in get_primaryAttackerStat)
     - Solution explanation (Contested vs Automatic resolution)
     - Impact analysis and balance implications
     - Prevention strategies for future development

#### 1. **SESSION_SUMMARY.md** (265 lines)
   - **For:** Developers resuming the project after a break
   - **Contains:** Complete project overview, architecture, all components explained
   - **Read this first** if you're new to the project
   - **Key sections:**
	 - Project overview & mod capabilities
	 - Complete project structure
	 - Detailed component descriptions (all 5 files)
	 - Critical implementation details
	 - Game API reference (from decompiled Assembly-CSharp)
	 - **Known issues & solutions (UPDATED with crash fix)**
	 - Testing checklist
	 - Next steps for future development

#### 2. **TECHNICAL_REFERENCE.md** (265 lines)
   - **For:** Quick lookups during development
   - **Contains:** Code snippets, API reference, common modifications
   - **Use when:** Implementing features, debugging, modifying code
   - **Key sections:**
	 - Quick facts table
	 - Project file structure
	 - Data flow diagram
	 - Code snippets for common tasks
	 - Assembly references (.csproj)
	 - Common modifications checklist
	 - Testing commands
	 - Debugging tips
	 - API reference (TICouncilorState, TIMissionEffect, etc.)
	 - Learning path for advanced features

#### 3. **TESTING_GUIDE.md** (262 lines)
   - **For:** Testing the mod in-game
   - **Contains:** Launch steps, test cases, troubleshooting
   - **Use when:** Running the mod, verifying features, debugging issues
   - **Key sections:**
	 - Pre-launch checklist
	 - Step-by-step launch instructions
	 - How to use the mission in-game
	 - 7 comprehensive test cases with expected results
	 - Troubleshooting guide
	 - Bug report template
	 - Performance expectations

---

## 🎯 Which Document to Read First?

### Scenario 1: "I'm taking over this project for the first time"
```
→ Read: **CRASH_FIX_LOG.md** (understand what crashed and why it was fixed)
→ Then: SESSION_SUMMARY.md (full overview)
→ Then: TESTING_GUIDE.md (understand current state)
→ Then: TECHNICAL_REFERENCE.md (for coding tasks)
```

### Scenario 2: "I need to add a new feature"
```
→ Read: TECHNICAL_REFERENCE.md (code snippets section)
→ Then: SESSION_SUMMARY.md (architecture section for context)
→ Then: Modify code and reference API section
```

### Scenario 3: "The mod isn't working, help!"
```
→ Read: TESTING_GUIDE.md (troubleshooting section)
→ Then: SESSION_SUMMARY.md (known issues section)
→ Then: Check Player.log per debugging tips
```

### Scenario 4: "I just need to rebuild and deploy"
```
→ Read: TECHNICAL_REFERENCE.md (build & deploy section)
→ Execute commands in .md file
→ Done in 2 minutes
```

---

## 📂 Project Organization

```
C:\Users\Chris\source\repos\Assistance\
│
├── Assistance.slnx                    [Visual Studio solution file]
│
├── SESSION_SUMMARY.md                 [← START HERE: Full overview]
├── TECHNICAL_REFERENCE.md             [← Developer reference]
├── TESTING_GUIDE.md                   [← QA/Testing guide]
├── README.md                          [← This file]
│
├── Assistance\                        [Main project folder]
│   ├── Assistance.csproj              [Project configuration]
│   ├── Main.cs                        [UMM entry point, GUI]
│   ├── Settings.cs                    [Configuration storage]
│   ├── TIMissionTemplate_Assist.cs    [Mission definition]
│   ├── TIMissionEffect_Assist.cs      [Stat transfer logic]
│   ├── AssistMissionBootstrapPatch.cs [Mission registration patch]
│   ├── Properties\
│   │   └── AssemblyInfo.cs            [Version info]
│   └── bin\Debug\
│       └── Assistance.dll             [Compiled mod (11,264 bytes)]
│
└── GameAnalysis\                      [Reference: Decompiled game code]
	└── Assembly-CSharp\               [2000+ decompiled .cs files]
```

---

## 🔑 Key Information at a Glance

| Aspect | Value | Location |
|--------|-------|----------|
| **Mod ID** | AssistMission | ModInfo.json |
| **DLL Name** | Assistance.dll | bin\Debug\ |
| **Entry Point** | Assistance.Main.Load | ModInfo.json |
| **Deployment** | C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\ | - |
| **Target Framework** | .NET 4.8 | .csproj |
| **Game Version** | Terra Invicta 1.0.38+ | ModInfo.json |
| **UMM Version** | 0.33.0.0+ | ModInfo.json |
| **Load Order** | 1200 | ModInfo.json |
| **Default Assist %** | 25% | Settings.cs |
| **Mission Name** | "Assist" | TIMissionTemplate_Assist.cs |
| **Affected Stats** | 7 (Persuasion, Investigation, Espionage, Command, Administration, Science, Security) | TIMissionEffect_Assist.cs |

---

## 🚀 Quick Start (30 seconds)

1. **Deploy:** Copy `Assistance\bin\Debug\Assistance.dll` → `C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\`
2. **Enable:** Launch Terra Invicta → UMM Manager → Enable "Councilor Assist Mission"
3. **Use:** Select councilor → Pick "Assist Councilor" mission → Target another councilor
4. **Verify:** Check target's stats - they should be higher after mission

For full details, read **TESTING_GUIDE.md**

---

## 💾 File Sizes & Dependencies

### Source Files
```
Main.cs                         ~2.5 KB
Settings.cs                     ~0.5 KB
TIMissionTemplate_Assist.cs     ~2.5 KB
TIMissionEffect_Assist.cs       ~2.5 KB
AssistMissionBootstrapPatch.cs  ~3.0 KB
----------------------------------------
Total Source (before compile):  ~11 KB
```

### Compiled Output
```
Assistance.dll                  11,264 bytes (no dependencies bundled)
ModInfo.json                    ~1 KB
----------------------------------------
Deployed Folder:                ~12 KB total
```

### Game References (NOT bundled)
- Assembly-CSharp.dll
- UnityEngine.dll, UnityEngine.CoreModule.dll, UnityEngine.IMGUIModule.dll
- UnityModManager.dll
- 0Harmony.dll

---

## 🎮 Feature Overview

### What the Mod Does
- **Adds:** "Assist Councilor" mission to all councilors
- **Transfers:** Configurable percentage (0-100%, default 25%) of source stats to target
- **Affects:** 7 councilor stats (Persuasion, Investigation, Espionage, Command, Administration, Science, Security)
- **Bonus Stacking:** Multiple assists add up (can boost same councilor multiple times)
- **Configuration:** In-game GUI slider for assist percentage

### What the Mod Does NOT Do (Yet)
- Add cost/resource requirement to mission
- Apply temporary buff (boost is permanent)
- Support Loyalty or ApparentLoyalty transfer (by design)
- Create faction-specific assist restrictions
- Support cross-faction assists (inherently disabled by mission targeting)

---

## 🔄 Development Workflow

### For Bug Fixes
```
1. Read TESTING_GUIDE.md (identify bug via test case)
2. Read SESSION_SUMMARY.md (find related code)
3. Read TECHNICAL_REFERENCE.md (find code snippets)
4. Modify the .cs file
5. Build in Visual Studio (or cmd: dotnet build)
6. Copy DLL to mods folder
7. Test in-game per TESTING_GUIDE.md
```

### For New Features
```
1. Read SESSION_SUMMARY.md (understand architecture)
2. Read TECHNICAL_REFERENCE.md (find examples)
3. Identify which file needs changes
4. Code the feature
5. Add test case to TESTING_GUIDE.md
6. Build and deploy
7. Test thoroughly
8. Update SESSION_SUMMARY.md with new feature
```

### For Handoff to Next Developer
```
1. Ensure all 3 .md files are up-to-date
2. Add any new discoveries to SESSION_SUMMARY.md
3. Add code snippets to TECHNICAL_REFERENCE.md
4. Add test cases to TESTING_GUIDE.md
5. Commit to version control with detailed message
6. Leave session notes at end of SESSION_SUMMARY.md
```

---

## 🔗 External Resources

### UMM & Harmony
- **UMM Page:** https://www.nexusmods.com/site/mods/21
- **Harmony Docs:** https://harmony.pardeike.net/
- **Terra Invicta Steam:** https://store.steampowered.com/app/1396160/Terra_Invicta/

### Decompiled References
- **Assembly-CSharp Analysis:** `GameAnalysis/Assembly-CSharp/` (2000+ files)
- **Example Mod (MoreRealisticNukes):** `ModAnalysis/MoreRealisticNukes/`

### .NET & C# Documentation
- **.NET 4.8:** https://docs.microsoft.com/en-us/dotnet/framework/
- **C# 7.3:** https://docs.microsoft.com/en-us/dotnet/csharp/

---

## ✅ Verification Checklist

Before starting development, verify:

- [ ] All 4 .md files exist in `Assistance\` folder
- [ ] `Assistance\bin\Debug\Assistance.dll` exists (11,264 bytes)
- [ ] `C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\ModInfo.json` exists
- [ ] Visual Studio solution opens without errors
- [ ] Project compiles successfully (`Build → Build Solution`)
- [ ] Can read `SESSION_SUMMARY.md` without issues

---

## 📞 Contact/Notes

- **Workspace:** C:\Users\Chris\source\repos\Assistance\
- **Game Directory:** C:\Games\Steam\steamapps\common\Terra Invicta\
- **Game Logs:** C:\Users\Chris\AppData\LocalLow\Pavonis Interactive\TerraInvicta\Player.log
- **UMM Config:** C:\Games\Steam\steamapps\common\Terra Invicta\UnityModManagerConfig.xml

---

## 📋 Document Maintenance

Each document should be updated when:

| Document | When | By Whom |
|----------|------|---------|
| SESSION_SUMMARY.md | Major changes, architecture shifts, new components | Any developer |
| TECHNICAL_REFERENCE.md | New code snippets added, API changes discovered | Developer implementing feature |
| TESTING_GUIDE.md | New features added, bugs fixed, new test cases | QA/Developer testing |
| README.md (this file) | Project structure changes, file organization updates | Project lead |

---

## 🎓 For AI Developers

If you're an AI instance resuming this project:

1. **Always read SESSION_SUMMARY.md first** - It has all critical context
2. **Use TECHNICAL_REFERENCE.md as your coding reference** - It's indexed for quick lookups
3. **Follow TESTING_GUIDE.md for verification** - Every change needs testing
4. **Update these files as you work** - Future instances depend on accurate info
5. **Keep all 3 files in sync** - Contradictions cause confusion

---

**Status:** Ready for Development  
**Version:** 1.0.0  
**Last Built:** 2026-08-31 13:36 UTC  
**Next Action:** Read SESSION_SUMMARY.md or launch Terra Invicta to test

---

*For questions or issues, refer to the appropriate .md file above.*
