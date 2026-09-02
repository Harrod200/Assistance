# Assistance Mod - Documentation Index

Welcome to the Assistance Mod documentation. This comprehensive guide covers all aspects of the project.

## 📚 Documentation Files

### Getting Started
Start here if you're new to the project:

1. **[QUICK_REFERENCE.md](QUICK_REFERENCE.md)** (5 minutes)
   - At-a-glance project info
   - File organization
   - Key classes and methods
   - Common issues & solutions
   - Quick testing checklist

2. **[README.md](README.md)** (10 minutes)
   - Project overview and features
   - How the Assist mission works
   - Configuration options
   - Project architecture summary
   - Known limitations

### Understanding the Design

3. **[ARCHITECTURE.md](ARCHITECTURE.md)** (15 minutes)
   - System overview and components
   - Component interactions
   - Data flow diagrams
   - Patch categories and purposes
   - Design decisions and rationale
   - Extension points
   - Performance considerations

### Implementation & Development

4. **[DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md)** (20 minutes)
   - Project setup and structure
   - Building the project
   - Code style and conventions
   - How to add new features
   - Debugging techniques
   - Common issues & solutions
   - Testing checklist
   - Future improvements

5. **[API_REFERENCE.md](API_REFERENCE.md)** (15 minutes)
   - Complete API documentation
   - All classes and methods
   - Harmony patches
   - Data structures
   - Settings and configuration
   - Integration points
   - Common usage patterns

## 🎯 Quick Navigation

### For Different Roles

**Project Manager / Stakeholder**
- Read: [README.md](README.md)
- Time: 10 minutes
- Gets: Feature overview and capabilities

**New Developer**
- Read: [QUICK_REFERENCE.md](QUICK_REFERENCE.md) → [README.md](README.md) → [ARCHITECTURE.md](ARCHITECTURE.md)
- Time: 30 minutes
- Gets: Project overview and how things work

**Experienced Developer**
- Read: [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) → [API_REFERENCE.md](API_REFERENCE.md)
- Time: 35 minutes
- Gets: Implementation details and code patterns

**Contributor**
- Read: All documentation in order
- Time: 75 minutes
- Gets: Complete understanding for modifications

## 📋 Documentation by Topic

### Mission System
- How assist missions work: [README.md - Features](README.md#features)
- Mission configuration: [README.md - Technical Details](README.md#technical-details)
- Mission definition: [API_REFERENCE.md - TIMissionTemplate_Assist](API_REFERENCE.md#timissiontemplate_assist)
- Adding conditions: [DEVELOPER_GUIDE.md - Adding a New Mission Condition](DEVELOPER_GUIDE.md#adding-a-new-mission-condition)

### Bonus Tracking System
- How bonuses work: [README.md - How It Works](README.md#how-it-works)
- API reference: [API_REFERENCE.md - AssistBonusTracker](API_REFERENCE.md#assistbonustracker)
- Calculation: [QUICK_REFERENCE.md - Core Concept](QUICK_REFERENCE.md#core-concept)
- Extending: [API_REFERENCE.md - Extension Examples](API_REFERENCE.md#extension-examples)

### Harmony Patches
- Overview: [ARCHITECTURE.md - Patch Categories](ARCHITECTURE.md#patch-categories)
- Details: [API_REFERENCE.md - Harmony Patches](API_REFERENCE.md#harmony-patches)
- Adding new: [DEVELOPER_GUIDE.md - Adding a New Patch](DEVELOPER_GUIDE.md#adding-a-new-patch)
- Flow diagrams: [ARCHITECTURE.md - Component Interactions](ARCHITECTURE.md#component-interactions)

### UI & Display
- How UI updates work: [ARCHITECTURE.md - UI Display Flow](ARCHITECTURE.md#4-ui-display-flow)
- Display patches: [API_REFERENCE.md - Harmony Patches](API_REFERENCE.md#harmony-patches)
- Adding UI features: [DEVELOPER_GUIDE.md - Extending the Bonus System](DEVELOPER_GUIDE.md#extending-the-bonus-system)

### Configuration & Settings
- Available settings: [README.md - Configuration](README.md#configuration)
- Accessing settings: [API_REFERENCE.md - Settings](API_REFERENCE.md#settings)
- Settings code: [Settings.cs](Settings.cs)

### Debugging
- Debugging guide: [DEVELOPER_GUIDE.md - Debugging](DEVELOPER_GUIDE.md#debugging)
- Common issues: [QUICK_REFERENCE.md - Common Issues](QUICK_REFERENCE.md#common-issues)
- Error handling: [ARCHITECTURE.md - Error Handling & Logging](ARCHITECTURE.md#error-handling--logging)

### Building & Testing
- Build instructions: [DEVELOPER_GUIDE.md - Building the Project](DEVELOPER_GUIDE.md#building-the-project)
- Testing: [DEVELOPER_GUIDE.md - Testing Checklist](DEVELOPER_GUIDE.md#testing-checklist)
- Performance: [ARCHITECTURE.md - Performance Considerations](ARCHITECTURE.md#performance-considerations)

## 🗂️ Project Structure

### Core Files (21 files)
```
Assistance/
├── Mission Definition
│   ├── TIMissionTemplate_Assist.cs          (82 lines)
│   └── TIMissionEffect_Assist.cs            (94 lines)
├── Mission Conditions
│   ├── TIMissionCondition_MyFactionCouncilor.cs    (39 lines)
│   ├── TIMissionCondition_PlayerFactionOnly.cs     (22 lines)
│   └── TIMissionCondition_NotCurrentlyAssisting.cs (34 lines)
├── Mission Modifiers
│   ├── TIMissionModifier_AssistFlat.cs      (43 lines)
│   └── TIMissionModifier_AssistStat.cs      (41 lines)
├── Integration Patches (7 files)
├── Bonus System
│   └── AssistBonusTracker.cs                (143 lines)
├── Configuration
│   ├── Main.cs                              (81 lines)
│   ├── Settings.cs                          (16 lines)
│   └── TIMissionTemplate.en                 (11 lines)
└── Properties/AssemblyInfo.cs
```

### Documentation (5 files)
```
├── README.md                 (Project overview)
├── ARCHITECTURE.md          (System design)
├── DEVELOPER_GUIDE.md       (How to develop)
├── API_REFERENCE.md         (Complete API)
├── QUICK_REFERENCE.md       (Quick lookup)
└── DOCUMENTATION_INDEX.md   (This file)
```

## 🔄 Reading Path by Goal

### "I want to understand what this mod does"
1. [QUICK_REFERENCE.md](QUICK_REFERENCE.md) - Core Concept (2 min)
2. [README.md](README.md) - Features section (5 min)

### "I want to set up the project for development"
1. [QUICK_REFERENCE.md](QUICK_REFERENCE.md) - File Organization (3 min)
2. [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) - Project Setup (5 min)
3. [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) - Building the Project (5 min)

### "I want to add a new feature"
1. [ARCHITECTURE.md](ARCHITECTURE.md) - System Overview (5 min)
2. [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) - Adding New Features (15 min)
3. [API_REFERENCE.md](API_REFERENCE.md) - Relevant API section (10 min)
4. [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) - Testing Checklist (5 min)

### "I want to fix a bug"
1. [QUICK_REFERENCE.md](QUICK_REFERENCE.md) - Common Issues (5 min)
2. [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) - Debugging (10 min)
3. [API_REFERENCE.md](API_REFERENCE.md) - Relevant API section (10 min)

### "I want to understand the mission flow"
1. [ARCHITECTURE.md](ARCHITECTURE.md) - Component Interactions (10 min)
2. [API_REFERENCE.md](API_REFERENCE.md) - TIMissionEffect_Assist (5 min)
3. [API_REFERENCE.md](API_REFERENCE.md) - Harmony Patches (10 min)

### "I want to extend the bonus system"
1. [ARCHITECTURE.md](ARCHITECTURE.md) - Bonus Tracking System (5 min)
2. [API_REFERENCE.md](API_REFERENCE.md) - AssistBonusTracker (10 min)
3. [API_REFERENCE.md](API_REFERENCE.md) - Extension Examples (10 min)

## 🔑 Key Concepts

### Assist Mission
A mission that allows one councilor to temporarily boost another councilor's stats.
- More info: [README.md - Features](README.md#features)

### Bonus Tracking
A separate system that maintains temporary stat bonuses without modifying base attributes.
- More info: [ARCHITECTURE.md - Bonus Tracking System](ARCHITECTURE.md#bonus-tracking-system)

### Harmony Patches
Code hooks that modify game behavior without changing game files.
- More info: [ARCHITECTURE.md - Patch Categories](ARCHITECTURE.md#patch-categories)

### Mission Conditions
Validation rules that determine if a target is valid for the assist mission.
- More info: [README.md - Technical Details](README.md#technical-details)

## 📊 Statistics

| Aspect | Value |
|--------|-------|
| Total Code Files | 21 |
| Total Lines of Code | ~1,500 |
| Harmony Patches | 8 |
| Mission Conditions | 3 |
| Documentation Files | 5 |
| Total Documentation Lines | ~2,500 |
| Code Coverage | Mission system, UI integration, bonus tracking |
| Test Areas | 8 categories with checklist |

## 🆘 Troubleshooting

### Can't find information about...
1. Check [QUICK_REFERENCE.md](QUICK_REFERENCE.md) index section
2. Search across all `.md` files for keywords
3. Check relevant source file comments in code

### Build errors?
- [DEVELOPER_GUIDE.md - Building the Project](DEVELOPER_GUIDE.md#building-the-project)

### Runtime issues?
- [DEVELOPER_GUIDE.md - Common Issues & Solutions](DEVELOPER_GUIDE.md#common-issues--solutions)
- [QUICK_REFERENCE.md - Common Issues](QUICK_REFERENCE.md#common-issues)

### Want to understand a specific patch?
- [API_REFERENCE.md - Harmony Patches](API_REFERENCE.md#harmony-patches)

### Want to extend functionality?
- [DEVELOPER_GUIDE.md - Adding New Features](DEVELOPER_GUIDE.md#adding-new-features)
- [API_REFERENCE.md - Extension Examples](API_REFERENCE.md#extension-examples)

## 📞 Support Resources

- **Game API Help**: Explore game assemblies with dnSpy
- **Harmony Help**: https://harmony.pardeike.net/
- **C# Help**: https://docs.microsoft.com/en-us/dotnet/csharp/
- **Visual Studio**: https://visualstudio.microsoft.com/

## ✅ Documentation Checklist

- [x] Project overview (README.md)
- [x] System architecture (ARCHITECTURE.md)
- [x] Developer guide (DEVELOPER_GUIDE.md)
- [x] API reference (API_REFERENCE.md)
- [x] Quick reference (QUICK_REFERENCE.md)
- [x] Documentation index (This file)
- [x] Code comments in source files
- [x] Configuration documentation
- [x] Troubleshooting guides
- [x] Extension examples

## 🎓 Learning Resources

### Beginner (Start here)
1. Read [QUICK_REFERENCE.md](QUICK_REFERENCE.md)
2. Read [README.md](README.md)
3. Explore [ARCHITECTURE.md](ARCHITECTURE.md) - System Overview section

### Intermediate
1. Read [ARCHITECTURE.md](ARCHITECTURE.md) - all sections
2. Read [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) - setup and conventions
3. Review source code structure

### Advanced
1. Read [API_REFERENCE.md](API_REFERENCE.md) - complete reference
2. Read [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) - adding features
3. Experiment with code modifications

## 📝 Notes

- All documentation is markdown format for easy viewing
- Code examples use C# 4.8 compatible syntax
- Diagrams use ASCII art for compatibility
- Documentation is version-agnostic (applies to current codebase)

---

**Last Updated**: Documentation created for Assistance Mod v0.6.0+  
**Total Documentation**: ~2,500 lines across 5 files  
**Completeness**: 100% - All major systems documented
