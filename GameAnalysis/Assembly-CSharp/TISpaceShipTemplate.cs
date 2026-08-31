using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AssetBundles;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020003F4 RID: 1012
public class TISpaceShipTemplate : TIDataTemplate
{
	// Token: 0x06001401 RID: 5121 RVA: 0x0005DDF0 File Offset: 0x0005BFF0
	public void FinishDesigningShip()
	{
		this.isIncompleteDesign = false;
	}

	// Token: 0x170002BD RID: 701
	// (get) Token: 0x06001402 RID: 5122 RVA: 0x0005DDF9 File Offset: 0x0005BFF9
	public bool isAlien
	{
		get
		{
			return TemplateManager.Find<TIShipHullTemplate>(this.hullName, false).alien;
		}
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x0005DE0C File Offset: 0x0005C00C
	public TISpaceShipTemplate()
	{
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x0005DFD9 File Offset: 0x0005C1D9
	public override TIGameState CreateGameState()
	{
		return GameStateManager.CreateNewGameState<TISpaceShipState>();
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x0005DFE0 File Offset: 0x0005C1E0
	public TISpaceShipTemplate(string dataNameToSet)
	{
		base.dataName = dataNameToSet;
		this.SetHullTemplate(this.hullName);
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x0005E1C0 File Offset: 0x0005C3C0
	public TISpaceShipState CreateDummyShip()
	{
		TISpaceShipState tispaceShipState = Activator.CreateInstance(typeof(TISpaceShipState)) as TISpaceShipState;
		tispaceShipState.Initialize();
		tispaceShipState.InitWithTemplate(this);
		tispaceShipState.isDummy = true;
		return tispaceShipState;
	}

	// Token: 0x06001407 RID: 5127 RVA: 0x0005E1EB File Offset: 0x0005C3EB
	public void SetDisplayName(string displayNameToSet)
	{
		this._displayName = displayNameToSet;
	}

	// Token: 0x06001408 RID: 5128 RVA: 0x0005E1F4 File Offset: 0x0005C3F4
	public void SetClassDisplayName(bool forceRefresh = false)
	{
		if (!this.hasDisplayName || forceRefresh)
		{
			if (this.hullTemplate.noShipyardBuild)
			{
				if (this.hullTemplate.isAlien)
				{
					this.SetDisplayName(Loc.T("UI.Precombat.SkirmishExoSquadron2"));
				}
				else
				{
					this.SetDisplayName(Loc.T("UI.Precombat.SkirmishExoSquadron1"));
				}
			}
			else
			{
				TIFactionTemplate tifactionTemplate = TemplateManager.Find<TIFactionTemplate>(this.factionName, false);
				this.SetDisplayName(this.GenerateRandomClassName(tifactionTemplate));
			}
		}
		this.hasDisplayName = true;
	}

	// Token: 0x06001409 RID: 5129 RVA: 0x0005E270 File Offset: 0x0005C470
	public TISpaceShipTemplate Clone(string dataName, string factionName)
	{
		return new TISpaceShipTemplate(dataName)
		{
			factionName = factionName,
			hullName = this.hullName,
			driveName = this.driveName,
			powerPlantName = this.powerPlantName,
			radiatorName = this.radiatorName,
			propellantTanks = this.propellantTanks,
			hullAppearanceIndex = this.hullAppearanceIndex,
			noseArmor = new ArmorFacingTemplate(this.noseArmor.materialName, this.noseArmor.armorValue),
			lateralArmor = new ArmorFacingTemplate(this.lateralArmor.materialName, this.lateralArmor.armorValue),
			tailArmor = new ArmorFacingTemplate(this.tailArmor.materialName, this.tailArmor.armorValue),
			moduleTemplateEntries = new List<ModuleDataTemplateEntry>(this.moduleTemplateEntries),
			hullWeaponTemplateEntries = new List<ModuleDataTemplateEntry>(this.hullWeaponTemplateEntries),
			noseWeaponTemplateEntries = new List<ModuleDataTemplateEntry>(this.noseWeaponTemplateEntries),
			fireModeTemplateEntries = new List<FireModeDataTemplateEntry>(this.fireModeTemplateEntries),
			_combatValue = this._combatValue,
			_baseCruiseAcceleration_mps2 = this._baseCruiseAcceleration_mps2,
			_baseCruiseDeltaV_kps = this._baseCruiseDeltaV_kps,
			role = this.role
		};
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x0005E3AC File Offset: 0x0005C5AC
	public void InitAtRunTime(bool skipNaming = false)
	{
		if (!skipNaming)
		{
			this.SetClassDisplayName(false);
		}
		this.moduleTemplateEntries = new List<ModuleDataTemplateEntry>();
		this.hullWeaponTemplateEntries = new List<ModuleDataTemplateEntry>();
		this.noseWeaponTemplateEntries = new List<ModuleDataTemplateEntry>();
		this.fireModeTemplateEntries = new List<FireModeDataTemplateEntry>();
		this.noseArmor = new ArmorFacingTemplate(string.Empty, 0);
		this.lateralArmor = new ArmorFacingTemplate(string.Empty, 0);
		this.tailArmor = new ArmorFacingTemplate(string.Empty, 0);
		this.isIncompleteDesign = true;
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x0005E429 File Offset: 0x0005C629
	public void CacheTemplateValues(bool skipCost = false)
	{
		if (!skipCost)
		{
			this.spaceResourceConstructionCost(true, null, true, false, false);
		}
		this.dryMass_tons(true);
		this.baseCruiseAcceleration_mps2(true);
		this.baseCruiseDeltaV_mps(true);
		this.BatteryCapacity_GJ(true);
		this.HeatCapacity_GJ(true);
	}

	// Token: 0x170002BE RID: 702
	// (get) Token: 0x0600140C RID: 5132 RVA: 0x0005E462 File Offset: 0x0005C662
	public string modelResource
	{
		get
		{
			return this.hullTemplate.modelResource[this.hullAppearanceIndex];
		}
	}

	// Token: 0x170002BF RID: 703
	// (get) Token: 0x0600140D RID: 5133 RVA: 0x0005E478 File Offset: 0x0005C678
	public int GetHullAppearanceIndex
	{
		get
		{
			int num = this.hullAppearanceIndex;
			if (num > 1 && !AssetBundleManager.AreDLCBundlesLoaded(1))
			{
				num -= 2;
			}
			return num;
		}
	}

	// Token: 0x170002C0 RID: 704
	// (get) Token: 0x0600140E RID: 5134 RVA: 0x0005E49D File Offset: 0x0005C69D
	public TIFactionState designingFaction
	{
		get
		{
			if (this._designingFaction == null)
			{
				this._designingFaction = GameStateManager.FindByTemplate<TIFactionState>(this.factionName, false);
			}
			return this._designingFaction;
		}
	}

	// Token: 0x0600140F RID: 5135 RVA: 0x0005E4BF File Offset: 0x0005C6BF
	public void SetHullTemplate(string templateName)
	{
		this.hullName = templateName;
		this._hullTemplate = TemplateManager.Find<TIShipHullTemplate>(this.hullName, false);
	}

	// Token: 0x170002C1 RID: 705
	// (get) Token: 0x06001410 RID: 5136 RVA: 0x0005E4DA File Offset: 0x0005C6DA
	public TIShipHullTemplate hullTemplate
	{
		get
		{
			if (this._hullTemplate == null)
			{
				this._hullTemplate = TemplateManager.Find<TIShipHullTemplate>(this.hullName, false);
			}
			return this._hullTemplate;
		}
	}

	// Token: 0x06001411 RID: 5137 RVA: 0x0005E4FC File Offset: 0x0005C6FC
	public void SetDriveTemplate(string templateName)
	{
		this.driveName = templateName;
		this._driveTemplate = TemplateManager.Find<TIDriveTemplate>(this.driveName, false);
	}

	// Token: 0x170002C2 RID: 706
	// (get) Token: 0x06001412 RID: 5138 RVA: 0x0005E517 File Offset: 0x0005C717
	public TIDriveTemplate driveTemplate
	{
		get
		{
			if (this._driveTemplate == null)
			{
				this._driveTemplate = TemplateManager.Find<TIDriveTemplate>(this.driveName, false);
			}
			return this._driveTemplate;
		}
	}

	// Token: 0x170002C3 RID: 707
	// (get) Token: 0x06001413 RID: 5139 RVA: 0x0005E539 File Offset: 0x0005C739
	public int thrusterCount
	{
		get
		{
			if (this.driveTemplate == null)
			{
				return 0;
			}
			return this.driveTemplate.thrusters;
		}
	}

	// Token: 0x06001414 RID: 5140 RVA: 0x0005E550 File Offset: 0x0005C750
	public void SetPowerPlantTemplate(string templateName)
	{
		this.powerPlantName = templateName;
		this._powerPlantTemplate = TemplateManager.Find<TIPowerPlantTemplate>(this.powerPlantName, false);
	}

	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x06001415 RID: 5141 RVA: 0x0005E56B File Offset: 0x0005C76B
	public TIPowerPlantTemplate powerPlantTemplate
	{
		get
		{
			if (this._powerPlantTemplate == null)
			{
				this._powerPlantTemplate = TemplateManager.Find<TIPowerPlantTemplate>(this.powerPlantName, false);
			}
			return this._powerPlantTemplate;
		}
	}

	// Token: 0x06001416 RID: 5142 RVA: 0x0005E58D File Offset: 0x0005C78D
	public void SetRadiatorTemplate(string templateName)
	{
		this.radiatorName = templateName;
		this._radiatorTemplate = TemplateManager.Find<TIRadiatorTemplate>(this.radiatorName, false);
		this.spaceResourceConstructionCost(true, null, true, false, false);
		this.dryMass_tons(true);
	}

	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x06001417 RID: 5143 RVA: 0x0005E5BC File Offset: 0x0005C7BC
	public TIRadiatorTemplate radiatorTemplate
	{
		get
		{
			if (this._radiatorTemplate == null)
			{
				this._radiatorTemplate = TemplateManager.Find<TIRadiatorTemplate>(this.radiatorName, false);
			}
			return this._radiatorTemplate;
		}
	}

	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x06001418 RID: 5144 RVA: 0x0005E5E0 File Offset: 0x0005C7E0
	public List<TIBatteryTemplate> batteryTemplates
	{
		get
		{
			List<TIBatteryTemplate> list = new List<TIBatteryTemplate>();
			foreach (ModuleDataEntry moduleDataEntry in this.utilityModules)
			{
				if (moduleDataEntry.moduleTemplate.isBattery)
				{
					list.Add(moduleDataEntry.moduleTemplate.ref_battery);
				}
			}
			return list;
		}
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x0005E64C File Offset: 0x0005C84C
	public void SetNoseArmorTemplate(string templateName)
	{
		this.noseArmor = new ArmorFacingTemplate(templateName, this.noseArmor.armorValue);
		this._noseArmorTemplate = TemplateManager.Find<TIShipArmorTemplate>(this.noseArmor.materialName, true);
		this.TrySetArmor(ShipModuleSlotType.NoseArmor, this.noseArmorValue);
	}

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x0600141A RID: 5146 RVA: 0x0005E689 File Offset: 0x0005C889
	public TIShipArmorTemplate noseArmorTemplate
	{
		get
		{
			if (this._noseArmorTemplate == null)
			{
				this._noseArmorTemplate = TemplateManager.Find<TIShipArmorTemplate>(this.noseArmor.materialName, true);
			}
			return this._noseArmorTemplate;
		}
	}

	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x0600141B RID: 5147 RVA: 0x0005E6B0 File Offset: 0x0005C8B0
	public int noseArmorValue
	{
		get
		{
			return this.noseArmor.armorValue;
		}
	}

	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x0600141C RID: 5148 RVA: 0x0005E6BD File Offset: 0x0005C8BD
	public float noseArmorThickness
	{
		get
		{
			return this.noseArmorTemplate.armor_section_thickness_m((float)this.noseArmorValue);
		}
	}

	// Token: 0x0600141D RID: 5149 RVA: 0x0005E6D1 File Offset: 0x0005C8D1
	public void SetLateralArmorTemplate(string templateName)
	{
		this.lateralArmor = new ArmorFacingTemplate(templateName, this.lateralArmor.armorValue);
		this._lateralArmorTemplate = TemplateManager.Find<TIShipArmorTemplate>(this.lateralArmor.materialName, true);
		this.TrySetArmor(ShipModuleSlotType.LateralArmor, this.lateralArmorValue);
	}

	// Token: 0x170002CA RID: 714
	// (get) Token: 0x0600141E RID: 5150 RVA: 0x0005E70E File Offset: 0x0005C90E
	public TIShipArmorTemplate lateralArmorTemplate
	{
		get
		{
			if (this._lateralArmorTemplate == null)
			{
				this._lateralArmorTemplate = TemplateManager.Find<TIShipArmorTemplate>(this.lateralArmor.materialName, true);
			}
			return this._lateralArmorTemplate;
		}
	}

	// Token: 0x170002CB RID: 715
	// (get) Token: 0x0600141F RID: 5151 RVA: 0x0005E735 File Offset: 0x0005C935
	public int lateralArmorValue
	{
		get
		{
			return this.lateralArmor.armorValue;
		}
	}

	// Token: 0x170002CC RID: 716
	// (get) Token: 0x06001420 RID: 5152 RVA: 0x0005E742 File Offset: 0x0005C942
	public float lateralArmorThickness_m
	{
		get
		{
			TIShipArmorTemplate lateralArmorTemplate = this.lateralArmorTemplate;
			if (lateralArmorTemplate == null)
			{
				return 0f;
			}
			return lateralArmorTemplate.armor_section_thickness_m((float)this.lateralArmorValue);
		}
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x0005E760 File Offset: 0x0005C960
	public void SetTailArmorTemplate(string templateName)
	{
		this.tailArmor = new ArmorFacingTemplate(templateName, this.tailArmor.armorValue);
		this._tailArmorTemplate = TemplateManager.Find<TIShipArmorTemplate>(this.tailArmor.materialName, true);
		this.TrySetArmor(ShipModuleSlotType.TailArmor, this.tailArmorValue);
	}

	// Token: 0x170002CD RID: 717
	// (get) Token: 0x06001422 RID: 5154 RVA: 0x0005E79D File Offset: 0x0005C99D
	public TIShipArmorTemplate tailArmorTemplate
	{
		get
		{
			if (this._tailArmorTemplate == null)
			{
				this._tailArmorTemplate = TemplateManager.Find<TIShipArmorTemplate>(this.tailArmor.materialName, true);
			}
			return this._tailArmorTemplate;
		}
	}

	// Token: 0x170002CE RID: 718
	// (get) Token: 0x06001423 RID: 5155 RVA: 0x0005E7C4 File Offset: 0x0005C9C4
	public int tailArmorValue
	{
		get
		{
			return this.tailArmor.armorValue;
		}
	}

	// Token: 0x170002CF RID: 719
	// (get) Token: 0x06001424 RID: 5156 RVA: 0x0005E7D1 File Offset: 0x0005C9D1
	public float tailArmorThickness
	{
		get
		{
			return this.tailArmorTemplate.armor_section_thickness_m((float)this.tailArmorValue);
		}
	}

	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x06001425 RID: 5157 RVA: 0x0005E7E5 File Offset: 0x0005C9E5
	public ShipSize size
	{
		get
		{
			if (this.hullTemplate.smallHull)
			{
				return ShipSize.Small;
			}
			if (!this.hullTemplate.largeHull && !this.hullTemplate.hugeHull)
			{
				return ShipSize.Medium;
			}
			return ShipSize.Large;
		}
	}

	// Token: 0x170002D1 RID: 721
	// (get) Token: 0x06001426 RID: 5158 RVA: 0x0005E813 File Offset: 0x0005CA13
	public bool requiresExotics
	{
		get
		{
			if (this._requiredExotics == -1f)
			{
				this._requiredExotics = this.spaceResourceConstructionCost(false, null, true, false, false).GetSingleCostValue(FactionResource.Exotics);
			}
			return this._requiredExotics > 0f;
		}
	}

	// Token: 0x170002D2 RID: 722
	// (get) Token: 0x06001427 RID: 5159 RVA: 0x0005E847 File Offset: 0x0005CA47
	public bool requiresAntimatter
	{
		get
		{
			if (this._requiredAntimatter == -1f)
			{
				this._requiredAntimatter = this.spaceResourceConstructionCost(false, null, true, false, false).GetSingleCostValue(FactionResource.Antimatter);
			}
			return this._requiredAntimatter > 0f;
		}
	}

	// Token: 0x06001428 RID: 5160 RVA: 0x0005E87C File Offset: 0x0005CA7C
	public float baseCruiseAcceleration_mps2(bool forceUpdate)
	{
		if (this._baseCruiseAcceleration_mps2 <= 0f || forceUpdate)
		{
			float num = (this.isAlien ? TemplateManager.global.maxAlienCruiseAcceleration_g : TemplateManager.global.baselineMaxHumanCruiseAcceleration_g);
			if (this._designingFaction != null)
			{
				num += TIEffectsState.SumEffectsModifiers(Context.Ship_MaxSurvivableCruiseAcceleration_Bonus, this._designingFaction, num, null);
			}
			this._baseCruiseAcceleration_mps2 = Mathf.Min(this.modifiedThrust_N / this.wetMass_kg, num * 9.80665f);
		}
		return this._baseCruiseAcceleration_mps2;
	}

	// Token: 0x170002D3 RID: 723
	// (get) Token: 0x06001429 RID: 5161 RVA: 0x0005E904 File Offset: 0x0005CB04
	public float baseCombatAcceleration_mps2
	{
		get
		{
			float num = (this.isAlien ? TemplateManager.global.maxAlienCombatAcceleration_g : TemplateManager.global.baselineMaxHumanCombatAcceleration_g);
			if (this._designingFaction != null)
			{
				num += TIEffectsState.SumEffectsModifiers(Context.Ship_MaxSurvivableCombatAcceleration_Bonus, this._designingFaction, num, null);
			}
			return Mathf.Min(this.baseCombatThrust_N / this.wetMass_kg, num * 9.80665f);
		}
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x0005E96C File Offset: 0x0005CB6C
	public float basePursuitAcceleration_mps2(bool forceUpdate)
	{
		return this.baseCruiseAcceleration_mps2(forceUpdate);
	}

	// Token: 0x0600142B RID: 5163 RVA: 0x0005E975 File Offset: 0x0005CB75
	public float baseCruiseDeltaV_kps(bool forceUpdate)
	{
		if (this._baseCruiseDeltaV_kps <= 0f || forceUpdate)
		{
			this._baseCruiseDeltaV_kps = this.modifiedEV_kps * Mathf.Log(this.wetMass_tons / this.dryMass_tons(forceUpdate));
		}
		return this._baseCruiseDeltaV_kps;
	}

	// Token: 0x170002D4 RID: 724
	// (get) Token: 0x0600142C RID: 5164 RVA: 0x0005E9B1 File Offset: 0x0005CBB1
	public static IEnumerable<TISpaceShipTemplate.TestCombat> TestCombats
	{
		get
		{
			if (TISpaceShipTemplate.testCombats == null)
			{
				TISpaceShipTemplate.GenerateTestCombats();
			}
			return TISpaceShipTemplate.testCombats;
		}
	}

	// Token: 0x0600142D RID: 5165 RVA: 0x0005E9C4 File Offset: 0x0005CBC4
	public static void GenerateTestCombats()
	{
		TISpaceShipTemplate.<>c__DisplayClass112_0 CS$<>8__locals1 = new TISpaceShipTemplate.<>c__DisplayClass112_0();
		CS$<>8__locals1.alienFaction = null;
		CS$<>8__locals1.humanFaction = null;
		CS$<>8__locals1.skirmishMode = GameControl.control.skirmishMode || !GameStateManager.HasGamestates;
		if (CS$<>8__locals1.skirmishMode)
		{
			IEnumerable<TIFactionTemplate> enumerable = TemplateManager.IterateByClass<TIFactionTemplate>(true);
			CS$<>8__locals1.alienFaction = TIFactionState.CreateDummy(enumerable.First<TIFactionTemplate>((TIFactionTemplate x) => x.isAlien));
			CS$<>8__locals1.humanFaction = TIFactionState.CreateDummy(enumerable.First<TIFactionTemplate>((TIFactionTemplate x) => !x.isAlien));
		}
		else
		{
			if (GameStateManager.AlienFaction().ideology == null)
			{
				return;
			}
			CS$<>8__locals1.alienFaction = GameStateManager.AlienFaction();
			CS$<>8__locals1.humanFaction = GameStateManager.AllHumanFactions().First<TIFactionState>();
		}
		TIUtilities.PushRandomState(new int?(17));
		List<TIFactionState.CombatLog> list = new List<TIFactionState.CombatLog>();
		if (!CS$<>8__locals1.skirmishMode)
		{
			list = (from x in (from x in GameStateManager.AllFactions()
					select x.CombatLogs into x
					where x != null
					select x).SelectMany<List<TIFactionState.CombatLog>, TIFactionState.CombatLog>((List<TIFactionState.CombatLog> x) => x)
				where x.AliensPresent && x.FleetVsFleet
				where x.Attacks.Count<TIFactionState.CombatLog.Attack>() > 0
				select x).ToList<TIFactionState.CombatLog>();
		}
		IOrderedEnumerable<TISpaceShipTemplate> orderedEnumerable = from x in TemplateManager.IterateByClass<TISpaceShipTemplate>(true)
			orderby TIUtilities.RandomFloatValue()
			select x;
		CS$<>8__locals1.alienShipTemplates = orderedEnumerable.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.isAlien).ToList<TISpaceShipTemplate>();
		CS$<>8__locals1.humanShipTemplates = orderedEnumerable.Except<TISpaceShipTemplate>(CS$<>8__locals1.alienShipTemplates).ToList<TISpaceShipTemplate>();
		int num = (CS$<>8__locals1.skirmishMode ? 7 : GameStateManager.AllHumanFactions().Length);
		CS$<>8__locals1.activePlayerTemplateWeight = 1f;
		CS$<>8__locals1.unownedDesigns = new HashSet<TISpaceShipTemplate>(orderedEnumerable.Where<TISpaceShipTemplate>((TISpaceShipTemplate shipDesign) => CS$<>8__locals1.skirmishMode || GameStateManager.AllFactions().None<TIFactionState>((TIFactionState y) => y.shipDesigns.Contains(shipDesign))));
		List<TIFactionState.CombatLog> list2 = CS$<>8__locals1.<GenerateTestCombats>g__GenerateCombatLogs|5(10, delegate(int shipCount)
		{
			List<TISpaceShipTemplate> list10 = new List<TISpaceShipTemplate>();
			for (int l = 0; l < shipCount; l++)
			{
				list10.Add(CS$<>8__locals1.humanShipTemplates.SelectRandomWeightedItem<TISpaceShipTemplate>(new Func<TISpaceShipTemplate, float>(base.<GenerateTestCombats>g__GetShipTemplateWeight|9), -1f, 1E-37f));
				list10.Add(CS$<>8__locals1.alienShipTemplates.SelectRandomWeightedItem<TISpaceShipTemplate>(new Func<TISpaceShipTemplate, float>(base.<GenerateTestCombats>g__GetShipTemplateWeight|9), -1f, 1E-37f));
			}
			return list10;
		});
		CS$<>8__locals1.humanShipStates = new List<TISpaceShipState>();
		if (!CS$<>8__locals1.skirmishMode)
		{
			CS$<>8__locals1.humanShipStates = (from x in GameStateManager.AllHumanFactions().SelectMany<TIFactionState, TISpaceShipState>((TIFactionState x) => x.ships)
				where x.combatant
				select x).ToList<TISpaceShipState>();
		}
		CS$<>8__locals1.alienShipStates = new List<TISpaceShipState>();
		if (!CS$<>8__locals1.skirmishMode)
		{
			CS$<>8__locals1.alienShipStates = GameStateManager.AlienFaction().ships.Where<TISpaceShipState>((TISpaceShipState x) => x.combatant).ToList<TISpaceShipState>();
		}
		CS$<>8__locals1.activePlayerShipStateWeight = Mathf.Pow((float)num / 2f, 0.7f);
		List<TIFactionState.CombatLog> list3 = new List<TIFactionState.CombatLog>();
		if (CS$<>8__locals1.humanShipStates.Count > 0 && CS$<>8__locals1.alienShipStates.Count > 0)
		{
			list3 = CS$<>8__locals1.<GenerateTestCombats>g__GenerateCombatLogs|5(10, delegate(int shipCount)
			{
				List<TISpaceShipState> list11 = new List<TISpaceShipState>();
				List<TISpaceShipState> list12 = new List<TISpaceShipState>();
				for (int m = 0; m < shipCount; m++)
				{
					list11.Add(CS$<>8__locals1.humanShipStates.SelectRandomWeightedItem<TISpaceShipState>(new Func<TISpaceShipState, float>(base.<GenerateTestCombats>g__GetShipStateWeight|14), -1f, 1E-37f));
					list12.Add(CS$<>8__locals1.alienShipStates.SelectRandomWeightedItem<TISpaceShipState>(new Func<TISpaceShipState, float>(base.<GenerateTestCombats>g__GetShipStateWeight|14), -1f, 1E-37f));
				}
				while (list11.Count < shipCount)
				{
					list11.Add(CS$<>8__locals1.humanShipStates.SelectRandomItem<TISpaceShipState>());
				}
				while (list12.Count < shipCount)
				{
					list12.Add(CS$<>8__locals1.alienShipStates.SelectRandomItem<TISpaceShipState>());
				}
				return from x in list11.Concat<TISpaceShipState>(list12)
					select x.template;
			});
		}
		TISpaceShipTemplate.testCombats = new List<TISpaceShipTemplate.TestCombat>();
		int num2 = 500;
		CS$<>8__locals1.weaponTemplates = (from x in list.Concat<TIFactionState.CombatLog>(list3).Concat<TIFactionState.CombatLog>(list2).SelectMany<TIFactionState.CombatLog, TIFactionState.CombatLog.Attack>((TIFactionState.CombatLog x) => x.Attacks)
			select x.WeaponDataName).Distinct<string>().ToDictionary<string, string, TIShipWeaponTemplate>((string x) => x, (string x) => TemplateManager.Find<TIShipWeaponTemplate>(x, true));
		List<ValueTuple<string, List<TIFactionState.CombatLog>, int>> list4 = new List<ValueTuple<string, List<TIFactionState.CombatLog>, int>>();
		int num3 = list.Sum<TIFactionState.CombatLog>(delegate(TIFactionState.CombatLog x)
		{
			IEnumerable<TIFactionState.CombatLog.Attack> attacks2 = x.Attacks;
			Func<TIFactionState.CombatLog.Attack, bool> func3;
			if ((func3 = CS$<>8__locals1.<>9__35) == null)
			{
				func3 = (CS$<>8__locals1.<>9__35 = (TIFactionState.CombatLog.Attack y) => CS$<>8__locals1.weaponTemplates[y.WeaponDataName].isAlien);
			}
			return attacks2.Count<TIFactionState.CombatLog.Attack>(func3);
		});
		int num4 = list.Sum<TIFactionState.CombatLog>((TIFactionState.CombatLog x) => x.Attacks.Count<TIFactionState.CombatLog.Attack>()) - num3;
		int num5 = Mathf.Clamp(Mathf.Min((float)list.Count / 2.5f + 0.5f, (float)Mathf.Min(num3, num4) / 7.5f).RoundDown(), 0, 18);
		int num6 = Mathf.Clamp(((float)Mathf.Min(CS$<>8__locals1.humanShipStates.Count, CS$<>8__locals1.alienShipStates.Count) / 3f).RoundDown(), 0, 24);
		num6 = Mathf.Min(24 - num5, num6);
		int num7 = 28 - num5 - num6;
		list4.Add(new ValueTuple<string, List<TIFactionState.CombatLog>, int>("Observed", list, num5));
		list4.Add(new ValueTuple<string, List<TIFactionState.CombatLog>, int>("Ship-Generated", list3, num6));
		list4.Add(new ValueTuple<string, List<TIFactionState.CombatLog>, int>("Template-Generated", list2, num7));
		foreach (ValueTuple<string, List<TIFactionState.CombatLog>, int> valueTuple in list4)
		{
			List<TIFactionState.CombatLog> item = valueTuple.Item2;
			if (item.Count != 0)
			{
				List<TIFactionState.CombatLog.Attack> list5 = item.SelectMany<TIFactionState.CombatLog, TIFactionState.CombatLog.Attack>((TIFactionState.CombatLog x) => x.Attacks).ToList<TIFactionState.CombatLog.Attack>();
				Func<TIFactionState.CombatLog.Attack, bool> func;
				if ((func = CS$<>8__locals1.<>9__37) == null)
				{
					func = (CS$<>8__locals1.<>9__37 = (TIFactionState.CombatLog.Attack x) => CS$<>8__locals1.weaponTemplates[x.WeaponDataName].isAlien);
				}
				List<TIFactionState.CombatLog.Attack> list6 = list5.Where<TIFactionState.CombatLog.Attack>(func).ToList<TIFactionState.CombatLog.Attack>();
				List<TIFactionState.CombatLog.Attack> list7 = list5.Except<TIFactionState.CombatLog.Attack>(list6).ToList<TIFactionState.CombatLog.Attack>();
				float num8 = 0f;
				float num9 = 0f;
				float num10 = 0f;
				foreach (TIFactionState.CombatLog combatLog in item)
				{
					int count = combatLog.Ships.FirstOrDefault<KeyValuePair<TIFactionState, List<ValueTuple<string, string>>>>(([TupleElementNames(new string[] { "TemplateName", "HullName" })] KeyValuePair<TIFactionState, List<ValueTuple<string, string>>> x) => x.Key.IsAlienFaction).Value.Count;
					int num11 = combatLog.Ships.Sum<KeyValuePair<TIFactionState, List<ValueTuple<string, string>>>>(([TupleElementNames(new string[] { "TemplateName", "HullName" })] KeyValuePair<TIFactionState, List<ValueTuple<string, string>>> x) => x.Value.Count) - count;
					IEnumerable<TIFactionState.CombatLog.Attack> attacks = combatLog.Attacks;
					Func<TIFactionState.CombatLog.Attack, bool> func2;
					if ((func2 = CS$<>8__locals1.<>9__40) == null)
					{
						func2 = (CS$<>8__locals1.<>9__40 = (TIFactionState.CombatLog.Attack x) => CS$<>8__locals1.weaponTemplates[x.WeaponDataName].isAlien);
					}
					int num12 = attacks.Count<TIFactionState.CombatLog.Attack>(func2);
					int num13 = combatLog.Attacks.Count<TIFactionState.CombatLog.Attack>() - num12;
					float num14 = (float)num11 / (float)count;
					if (num14 > 1f)
					{
						num14 = 1f / num14;
					}
					num10 += num14;
					num8 += (float)num12 / (float)count;
					num9 += (float)num13 / (float)num11;
				}
				float num15 = num8 / num10;
				float num16 = num9 / num10;
				List<TISpaceShipTemplate.TestCombat> list8 = new List<TISpaceShipTemplate.TestCombat>();
				for (int i = 0; i < valueTuple.Item3; i++)
				{
					TISpaceShipTemplate.TestCombat testCombat = new TISpaceShipTemplate.TestCombat();
					for (int j = 0; j < num2; j++)
					{
						TIFactionState.CombatLog.Attack attack;
						if (TIUtilities.RandomFloatValue() < num15 / (num15 + num16))
						{
							attack = list6.SelectRandomItem<TIFactionState.CombatLog.Attack>();
						}
						else
						{
							attack = list7.SelectRandomItem<TIFactionState.CombatLog.Attack>();
						}
						testCombat.AddAttack(new TISpaceShipTemplate.TestCombat.Attack
						{
							Weapon = CS$<>8__locals1.weaponTemplates[attack.WeaponDataName],
							Range_km = attack.Range_km,
							ArmorFacing = attack.ArmorFacing,
							Angle = attack.Angle,
							Roll = TIUtilities.RandomFloatValue(),
							TargetingBonus = attack.TargetingBonus
						});
					}
					list8.Add(testCombat);
				}
				TISpaceShipTemplate.testCombats.AddRange(list8);
			}
		}
		if (true)
		{
			List<TISpaceShipTemplate.TestCombat.Attack> list9 = (from x in TISpaceShipTemplate.testCombats.SelectMany<TISpaceShipTemplate.TestCombat, TISpaceShipTemplate.TestCombat.Attack>((TISpaceShipTemplate.TestCombat x) => x.Attacks)
				orderby TIUtilities.RandomFloatValue()
				select x).ToList<TISpaceShipTemplate.TestCombat.Attack>();
			foreach (TISpaceShipTemplate.TestCombat testCombat2 in TISpaceShipTemplate.testCombats)
			{
				testCombat2.Attacks.Clear();
			}
			for (int k = 0; k < list9.Count; k++)
			{
				TISpaceShipTemplate.testCombats[k % TISpaceShipTemplate.testCombats.Count].Attacks.Add(list9[k]);
			}
		}
		TIUtilities.PopRandomState();
	}

	// Token: 0x0600142E RID: 5166 RVA: 0x0005F2E8 File Offset: 0x0005D4E8
	public static float GetUnnormalizedSpaceCombatValueFromParameters(float survivability, [TupleElementNames(new string[] { "DPS", "ammoDuration_s", "lifetime_s" })] IEnumerable<ValueTuple<float, float, float>> weapons, float miscModifier)
	{
		float num = 3f;
		float expectedFlattenedCombatLength_s = num * 4f * 60f;
		return Mathf.Pow(weapons.Sum<ValueTuple<float, float, float>>(delegate([TupleElementNames(new string[] { "DPS", "ammoDuration_s", "lifetime_s" })] ValueTuple<float, float, float> x)
		{
			float num2 = Mathf.Pow(x.Item1, 1f);
			float num3 = Mathf.Pow(survivability, 0.8f);
			float num4 = Mathf.Sin(Mathf.Clamp01(x.Item2 / expectedFlattenedCombatLength_s) * 3.1415927f / 2f);
			float num5 = Mathf.Sin(Mathf.Clamp01(x.Item3 / expectedFlattenedCombatLength_s) * 3.1415927f / 2f);
			return num2 * num3 * num4 * num5;
		}) * miscModifier, 1f);
	}

	// Token: 0x0600142F RID: 5167 RVA: 0x0005F33C File Offset: 0x0005D53C
	public float UnnormalizedTemplateSpaceCombatValue(bool forceUpdate = false, float fidelity = 1f)
	{
		if (float.IsNaN(this._unnormalizedCombatValue) || float.IsInfinity(this._unnormalizedCombatValue) || this._unnormalizedCombatValue <= 0f || forceUpdate || fidelity != 1f)
		{
			if (TISpaceShipTemplate.TestCombats == null || this.noseWeaponTemplates.Count<TIShipWeaponTemplate>() + this.hullWeaponTemplates.Count<TIShipWeaponTemplate>() == 0)
			{
				this._unnormalizedCombatValue = 0f;
			}
			else
			{
				TISpaceShipTemplate.<>c__DisplayClass114_0 CS$<>8__locals1 = new TISpaceShipTemplate.<>c__DisplayClass114_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.weaponTemplates = this.noseWeaponTemplates.Union<TIShipWeaponTemplate>(this.hullWeaponTemplates).ToDictionary<TIShipWeaponTemplate, string, TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.dataName, (TIShipWeaponTemplate x) => x);
				CS$<>8__locals1.weapons = this.noseWeapons.Concat<ModuleDataEntry>(this.hullWeapons).ToDictionary<ModuleDataEntry, ModuleDataEntry, ValueTuple<TIShipWeaponTemplate, float, float, float>>((ModuleDataEntry weapon) => weapon, delegate(ModuleDataEntry weapon)
				{
					TIShipWeaponTemplate tishipWeaponTemplate = CS$<>8__locals1.weaponTemplates[weapon.moduleTemplateName];
					float num5 = tishipWeaponTemplate.GenericScore();
					float expectedCombatRange_km = CS$<>8__locals1.<>4__this.role.GetExpectedCombatRange_km();
					float bonusPowerForWeapon_Multiplier = CS$<>8__locals1.<>4__this.GetBonusPowerForWeapon_Multiplier(tishipWeaponTemplate, expectedCombatRange_km, null);
					num5 *= bonusPowerForWeapon_Multiplier;
					TIProjectileWeaponTemplate tiprojectileWeaponTemplate = tishipWeaponTemplate as TIProjectileWeaponTemplate;
					float num6;
					if (tiprojectileWeaponTemplate != null)
					{
						num6 = tishipWeaponTemplate.averageCooldown_s * (float)tiprojectileWeaponTemplate.FullAmmoCount_Max(CS$<>8__locals1.<>4__this);
					}
					else
					{
						num6 = float.PositiveInfinity;
					}
					return new ValueTuple<TIShipWeaponTemplate, float, float, float>(tishipWeaponTemplate, num5, num6, 0f);
				});
				CS$<>8__locals1.ecmValue = this.ECMValue(!this.isAlien, null);
				float num = 1f;
				if (this.ValidTemplate)
				{
					num = CS$<>8__locals1.<UnnormalizedTemplateSpaceCombatValue>g__GetExpectedLifetime_s|4();
				}
				if (float.IsNaN(num))
				{
					num = 1f;
					Log.Error("expectedLifetime_s was NaN!", Array.Empty<object>());
				}
				if (this.HasSpecialModuleCapability(SpecialModuleRule.Repair))
				{
					num *= 1.1f;
				}
				float num2 = ((this.baseCombatAcceleration_mps2 == 0f) ? 0f : (this.baseCruiseDeltaV_mps(true) / this.baseCombatAcceleration_mps2));
				float num3 = 0.9f + 0.1f * Mathf.Pow(num2 / 600f, 0.25f * this.baseCombatAcceleration_gs / 4f);
				float num4 = TISpaceShipTemplate.GetUnnormalizedSpaceCombatValueFromParameters(num, from x in CS$<>8__locals1.weapons
					where x.Value.Item1.attackMode
					select new ValueTuple<float, float, float>(x.Value.Item2, x.Value.Item3, x.Value.Item4), num3);
				if (float.IsNaN(num4) || float.IsInfinity(num4))
				{
					Log.Error("_unnormalizedCombatValue was invalid! " + num4.ToString(), Array.Empty<object>());
					num4 = 0f;
				}
				if (fidelity != 1f)
				{
					return num4;
				}
				this._unnormalizedCombatValue = num4;
			}
		}
		return this._unnormalizedCombatValue;
	}

	// Token: 0x06001430 RID: 5168 RVA: 0x0005F5B0 File Offset: 0x0005D7B0
	public static float GetNormalizedSpaceCombatValue(float unnormalizedSpaceCombatValue, float minimumFraction = 0.1f)
	{
		if (float.IsNaN(TIGlobalValuesState.BaselineUnnormalizedSpaceCombatValue) || float.IsInfinity(TIGlobalValuesState.BaselineUnnormalizedSpaceCombatValue) || TIGlobalValuesState.BaselineUnnormalizedSpaceCombatValue <= 0f || (TISpaceShipTemplate.baselineUnormalizedSCVUpdatedFrame != TIFrameCounter.FrameCount && TISpaceShipTemplate.AllowDynamicTemplateSpaceCombatValue()))
		{
			Dictionary<TISpaceShipTemplate, int> dictionary;
			if (GameStateManager.HasGamestates && GameStateManager.AlienFaction().ships.Count > 0 && !GameControl.control.skirmishMode)
			{
				dictionary = (from x in GameStateManager.AlienFaction().ships
					group x by x.template).ToDictionary<IGrouping<TISpaceShipTemplate, TISpaceShipState>, TISpaceShipTemplate, int>((IGrouping<TISpaceShipTemplate, TISpaceShipState> x) => x.Key, (IGrouping<TISpaceShipTemplate, TISpaceShipState> x) => x.Count<TISpaceShipState>());
			}
			else
			{
				dictionary = (from x in TemplateManager.IterateByClass<TISpaceShipTemplate>(true)
					where x.isAlien
					select x).ToDictionary<TISpaceShipTemplate, TISpaceShipTemplate, int>((TISpaceShipTemplate x) => x, (TISpaceShipTemplate x) => 1);
			}
			TIGlobalValuesState.BaselineUnnormalizedSpaceCombatValue = dictionary.Sum<KeyValuePair<TISpaceShipTemplate, int>>((KeyValuePair<TISpaceShipTemplate, int> x) => x.Key.UnnormalizedTemplateSpaceCombatValue(false, 1f) * (float)x.Value) / (float)dictionary.Sum<KeyValuePair<TISpaceShipTemplate, int>>((KeyValuePair<TISpaceShipTemplate, int> x) => x.Value);
			TISpaceShipTemplate.baselineUnormalizedSCVUpdatedFrame = TIFrameCounter.FrameCount;
		}
		float num = unnormalizedSpaceCombatValue / TIGlobalValuesState.BaselineUnnormalizedSpaceCombatValue;
		num = (1f - minimumFraction) * num + minimumFraction;
		return 100f * num;
	}

	// Token: 0x06001431 RID: 5169 RVA: 0x0005F780 File Offset: 0x0005D980
	public float TemplateSpaceCombatValue(bool forceUpdate = false, float updateFraction = -1f, float fidelity = 1f, bool fast = false)
	{
		if (!float.IsNaN(this._combatValue) && !float.IsInfinity(this._combatValue) && this._combatValue >= 0f && !forceUpdate && updateFraction <= 0f && fidelity == 1f)
		{
			if (this._combatValue != 0f)
			{
				goto IL_010D;
			}
			if (!this.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.attackMode))
			{
				goto IL_010D;
			}
		}
		if (fast && this.hullTemplate.simpleHull)
		{
			return AIEvaluators.GetTypicalSTOFighterSpaceCombatValue();
		}
		float num = this.UnnormalizedTemplateSpaceCombatValue(forceUpdate, fidelity);
		if (num == 0f)
		{
			this._combatValue = 0f;
		}
		else
		{
			float normalizedSpaceCombatValue = TISpaceShipTemplate.GetNormalizedSpaceCombatValue(num, this.hullTemplate.simpleHull ? 0.05f : 0.1f);
			if (fidelity != 1f)
			{
				return normalizedSpaceCombatValue;
			}
			if (this._combatValue > 0f && updateFraction > 0f)
			{
				this._combatValue = Mathf.Lerp(this._combatValue, normalizedSpaceCombatValue, updateFraction);
			}
			else
			{
				this._combatValue = normalizedSpaceCombatValue;
			}
		}
		IL_010D:
		return this._combatValue;
	}

	// Token: 0x06001432 RID: 5170 RVA: 0x0005F8A0 File Offset: 0x0005DAA0
	public static bool AllowDynamicTemplateSpaceCombatValue()
	{
		return GameStateManager.AllHumanFactions().None<TIFactionState>((TIFactionState x) => x.unlockedVictoryObjective);
	}

	// Token: 0x06001433 RID: 5171 RVA: 0x0005F8CC File Offset: 0x0005DACC
	public float BombardmentValue(TISpaceBodyState spaceBody)
	{
		float num = 0f;
		foreach (TIShipWeaponTemplate tishipWeaponTemplate in this.allWeaponTemplates)
		{
			num += tishipWeaponTemplate.GetLocalBombardmentValue(spaceBody);
		}
		return num;
	}

	// Token: 0x06001434 RID: 5172 RVA: 0x0005F92C File Offset: 0x0005DB2C
	public float InvasionCombatValue()
	{
		float num = 0f;
		if (this.utilitySlotModuleTemplates.Any<TIShipModuleTemplate>(delegate(TIShipModuleTemplate x)
		{
			if (x.isUtilityModule)
			{
				TIUtilityModuleTemplate ref_utilityModule = x.ref_utilityModule;
				return ref_utilityModule != null && ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.LandArmy);
			}
			return false;
		}))
		{
			ModuleDataEntry moduleDataEntry = this.utilityModules.First<ModuleDataEntry>(delegate(ModuleDataEntry x)
			{
				TIUtilityModuleTemplate ref_utilityModule2 = x.moduleTemplate.ref_utilityModule;
				return ref_utilityModule2 != null && ref_utilityModule2.specialModuleRules.Contains(SpecialModuleRule.LandArmy);
			});
			num += moduleDataEntry.moduleTemplate.ref_utilityModule.marineOpsValue * (float)TemplateManager.global.alienArmiesFromLanding;
		}
		return num;
	}

	// Token: 0x06001435 RID: 5173 RVA: 0x0005F9B6 File Offset: 0x0005DBB6
	public float GetMonthlyNetIncome(FactionResource resource)
	{
		if (resource == FactionResource.Money)
		{
			return this.hullTemplate.monthlyIncome_Money;
		}
		return 0f;
	}

	// Token: 0x06001436 RID: 5174 RVA: 0x0005F9CD File Offset: 0x0005DBCD
	public float GetMonthlyGrossRevenue(FactionResource resource)
	{
		if (resource != FactionResource.Money)
		{
			return 0f;
		}
		if (this.hullTemplate.monthlyIncome_Money <= 0f)
		{
			return 0f;
		}
		return this.hullTemplate.monthlyIncome_Money;
	}

	// Token: 0x06001437 RID: 5175 RVA: 0x0005F9FC File Offset: 0x0005DBFC
	public float GetMonthlyExpenses(FactionResource resource)
	{
		if (resource != FactionResource.Money)
		{
			return 0f;
		}
		if (this.hullTemplate.monthlyIncome_Money >= 0f)
		{
			return 0f;
		}
		return -this.hullTemplate.monthlyIncome_Money;
	}

	// Token: 0x06001438 RID: 5176 RVA: 0x0005FA2C File Offset: 0x0005DC2C
	public ArmorFacingTemplate GetArmorFacingTemplateInSlot(ShipModuleSlotType slot)
	{
		switch (slot)
		{
		case ShipModuleSlotType.NoseArmor:
			return this.noseArmor;
		case ShipModuleSlotType.TailArmor:
			return this.tailArmor;
		}
		return this.lateralArmor;
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x0005FA58 File Offset: 0x0005DC58
	public void TrySetArmor(ShipModuleSlotType armorSlot, int numPointsToSet)
	{
		float num;
		numPointsToSet = Mathf.Clamp(numPointsToSet, 0, this.GetMaxAllowedArmorBySlot(armorSlot, out num, null));
		switch (armorSlot)
		{
		case ShipModuleSlotType.NoseArmor:
			this.noseArmor.armorValue = numPointsToSet;
			return;
		case ShipModuleSlotType.LateralArmor:
			this.lateralArmor.armorValue = numPointsToSet;
			return;
		case ShipModuleSlotType.TailArmor:
			this.tailArmor.armorValue = numPointsToSet;
			return;
		default:
			return;
		}
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x0005FAB4 File Offset: 0x0005DCB4
	public int TryAddArmorPoints(ShipModuleSlotType armorSlot, int numPointsToAdd)
	{
		float num;
		int maxAllowedArmorBySlot = this.GetMaxAllowedArmorBySlot(armorSlot, out num, null);
		switch (armorSlot)
		{
		case ShipModuleSlotType.NoseArmor:
		{
			int armorValue = this.noseArmor.armorValue;
			this.noseArmor.armorValue = this.noseArmor.armorValue + numPointsToAdd;
			this.noseArmor.armorValue = Mathf.Clamp(this.noseArmor.armorValue, 0, maxAllowedArmorBySlot);
			return this.noseArmor.armorValue - armorValue;
		}
		case ShipModuleSlotType.LateralArmor:
		{
			int armorValue2 = this.lateralArmor.armorValue;
			this.lateralArmor.armorValue = this.lateralArmor.armorValue + numPointsToAdd;
			this.lateralArmor.armorValue = Mathf.Clamp(this.lateralArmor.armorValue, 0, maxAllowedArmorBySlot);
			return this.lateralArmor.armorValue - armorValue2;
		}
		case ShipModuleSlotType.TailArmor:
		{
			int armorValue3 = this.tailArmor.armorValue;
			this.tailArmor.armorValue = this.tailArmor.armorValue + numPointsToAdd;
			this.tailArmor.armorValue = Mathf.Clamp(this.tailArmor.armorValue, 0, maxAllowedArmorBySlot);
			return this.tailArmor.armorValue - armorValue3;
		}
		default:
			return -1;
		}
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x0005FBC0 File Offset: 0x0005DDC0
	public int GetMaxAllowedArmorBySlot(ShipModuleSlotType armorSlot, out float maxDepth_m, TIShipArmorTemplate prospectiveArmor = null)
	{
		TIShipArmorTemplate tishipArmorTemplate;
		switch (armorSlot)
		{
		case ShipModuleSlotType.NoseArmor:
			maxDepth_m = this.hullTemplate.maxNoseArmorDepth_m;
			tishipArmorTemplate = this.noseArmorTemplate;
			break;
		case ShipModuleSlotType.LateralArmor:
			maxDepth_m = this.hullTemplate.maxLateralArmorDepth_m;
			tishipArmorTemplate = this.lateralArmorTemplate;
			break;
		case ShipModuleSlotType.TailArmor:
			maxDepth_m = this.hullTemplate.maxTailArmorDepth_m;
			tishipArmorTemplate = this.tailArmorTemplate;
			break;
		default:
			maxDepth_m = -1f;
			return -1;
		}
		maxDepth_m *= 1f + this.utilitySlotModuleTemplates.Sum<TIShipModuleTemplate>(delegate(TIShipModuleTemplate x)
		{
			TIUtilityModuleTemplate ref_utilityModule = x.ref_utilityModule;
			if (ref_utilityModule == null)
			{
				return 0f;
			}
			return ref_utilityModule.armorMaxBonus;
		});
		if (prospectiveArmor != null)
		{
			tishipArmorTemplate = prospectiveArmor;
		}
		if (tishipArmorTemplate != null)
		{
			decimal num = (decimal)maxDepth_m * 1000m;
			decimal num2 = (decimal)(tishipArmorTemplate.plate_thickness_m * 1000f);
			return (int)Math.Truncate(num / num2);
		}
		return 0;
	}

	// Token: 0x170002D5 RID: 725
	// (get) Token: 0x0600143C RID: 5180 RVA: 0x0005FCA8 File Offset: 0x0005DEA8
	public List<TIShipPartTemplate> partTemplates
	{
		get
		{
			List<TIShipPartTemplate> list = new List<TIShipPartTemplate>();
			if (this.hullTemplate != null)
			{
				list.Add(this.hullTemplate);
			}
			if (this.driveTemplate != null)
			{
				list.Add(this.driveTemplate);
			}
			if (this.powerPlantTemplate != null)
			{
				list.Add(this.powerPlantTemplate);
			}
			if (this.radiatorTemplate != null)
			{
				list.Add(this.radiatorTemplate);
			}
			if (this.noseArmorTemplate != null)
			{
				list.Add(this.noseArmorTemplate);
			}
			if (this.lateralArmorTemplate != null)
			{
				list.Add(this.lateralArmorTemplate);
			}
			if (this.tailArmorTemplate != null)
			{
				list.Add(this.tailArmorTemplate);
			}
			list.AddRange(this.utilitySlotModuleTemplates);
			list.AddRange(this.noseWeaponTemplates);
			list.AddRange(this.hullWeaponTemplates);
			return list;
		}
	}

	// Token: 0x0600143D RID: 5181 RVA: 0x0005FD6C File Offset: 0x0005DF6C
	public bool ValidAssignedSlotForLocation(TIShipPartTemplate partTemplate, int slot)
	{
		if (slot < this.hullTemplate.shipModuleSlots.Count && partTemplate.allowedSlots.Contains(this.hullTemplate.shipModuleSlots[slot].moduleSlotType))
		{
			TIShipWeaponTemplate ref_weapon = partTemplate.ref_weapon;
			if (ref_weapon != null)
			{
				switch (ref_weapon.mount)
				{
				case Mount.HalfNose:
				case Mount.HalfHull:
					return this.hullTemplate.noShipyardBuild;
				case Mount.OneHull:
				case Mount.OneNose:
					return true;
				case Mount.TwoHullHoriz:
				case Mount.TwoHullVert:
				case Mount.ThreeHullHoriz:
				case Mount.FourHull:
				case Mount.TwoNoseHoriz:
				case Mount.TwoNoseVert:
				case Mount.ThreeNoseAngle:
				case Mount.FourNose:
				{
					using (List<List<TIShipHullTemplate.ShipModuleSlot>>.Enumerator enumerator = this.hullTemplate.ValidBigWeaponSlotSets(ref_weapon.mount).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							List<TIShipHullTemplate.ShipModuleSlot> list = enumerator.Current;
							if (this.hullTemplate.slotIndex(list[0]) == slot)
							{
								return true;
							}
						}
						return false;
					}
					break;
				}
				default:
					return false;
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x0600143E RID: 5182 RVA: 0x0005FE7C File Offset: 0x0005E07C
	public bool ValidAssignedSlotForLocation(ModuleDataEntry moduleDataEntry)
	{
		return this.ValidAssignedSlotForLocation(moduleDataEntry.moduleTemplate, moduleDataEntry.slotIndex);
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x0005FE90 File Offset: 0x0005E090
	public bool ValidAssignedSlotForLocation(ModuleDataTemplateEntry moduleDataTemplateEntry)
	{
		TIShipPartTemplate tishipPartTemplate = TemplateManager.Find<TIShipPartTemplate>(moduleDataTemplateEntry.moduleName, true);
		return this.ValidAssignedSlotForLocation(tishipPartTemplate, moduleDataTemplateEntry.slot);
	}

	// Token: 0x06001440 RID: 5184 RVA: 0x0005FEB7 File Offset: 0x0005E0B7
	public bool SlotIndexOccupied(int slotIndex, bool testSecondarySlotsForWeapons)
	{
		return this.GetPartInHullSlotIndex(slotIndex, testSecondarySlotsForWeapons) != null;
	}

	// Token: 0x06001441 RID: 5185 RVA: 0x0005FEC4 File Offset: 0x0005E0C4
	public TIShipPartTemplate GetPartInHullSlotIndex(int slotIndex, bool testSecondarySlotsForWeapons)
	{
		foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in this.hullTemplate.shipModuleSlots)
		{
			if (this.hullTemplate.slotIndex(shipModuleSlot) == slotIndex)
			{
				switch (shipModuleSlot.moduleSlotType)
				{
				case ShipModuleSlotType.Utility:
				{
					using (IEnumerator<ModuleDataEntry> enumerator2 = this.utilityModules.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							ModuleDataEntry moduleDataEntry = enumerator2.Current;
							if (moduleDataEntry.slotIndex == slotIndex)
							{
								return moduleDataEntry.moduleTemplate;
							}
						}
						continue;
					}
					break;
				}
				case ShipModuleSlotType.PowerPlant:
					return this.powerPlantTemplate;
				case ShipModuleSlotType.Radiator:
					return this.radiatorTemplate;
				case ShipModuleSlotType.Drive:
					return this.driveTemplate;
				case ShipModuleSlotType.NoseArmor:
					return this.noseArmorTemplate;
				case ShipModuleSlotType.LateralArmor:
					return this.lateralArmorTemplate;
				case ShipModuleSlotType.TailArmor:
					return this.tailArmorTemplate;
				case ShipModuleSlotType.NoseHardPoint:
					break;
				case ShipModuleSlotType.HullHardPoint:
					goto IL_01AA;
				default:
					continue;
				}
				using (IEnumerator<ModuleDataEntry> enumerator2 = this.noseWeapons.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ModuleDataEntry moduleDataEntry2 = enumerator2.Current;
						if (moduleDataEntry2.slotIndex == slotIndex)
						{
							return moduleDataEntry2.moduleTemplate;
						}
						if (testSecondarySlotsForWeapons)
						{
							TIShipWeaponTemplate ref_weapon = moduleDataEntry2.moduleTemplate.ref_weapon;
							TIShipHullTemplate.ShipModuleSlot shipModuleSlot2 = this.hullTemplate.shipModuleSlots[moduleDataEntry2.slotIndex];
							TIShipHullTemplate.ShipModuleSlot shipModuleSlot3 = this.hullTemplate.shipModuleSlots[slotIndex];
							if (this.hullTemplate.WeaponSlotSet(shipModuleSlot2, ref_weapon.mount).Contains(shipModuleSlot3))
							{
								return ref_weapon;
							}
						}
					}
					continue;
				}
				IL_01AA:
				foreach (ModuleDataEntry moduleDataEntry3 in this.hullWeapons)
				{
					if (moduleDataEntry3.slotIndex == slotIndex)
					{
						return moduleDataEntry3.moduleTemplate;
					}
					if (testSecondarySlotsForWeapons)
					{
						TIShipWeaponTemplate ref_weapon2 = moduleDataEntry3.moduleTemplate.ref_weapon;
						TIShipHullTemplate.ShipModuleSlot shipModuleSlot4 = this.hullTemplate.shipModuleSlots[moduleDataEntry3.slotIndex];
						TIShipHullTemplate.ShipModuleSlot shipModuleSlot5 = this.hullTemplate.shipModuleSlots[slotIndex];
						if (this.hullTemplate.WeaponSlotSet(shipModuleSlot4, ref_weapon2.mount).Contains(shipModuleSlot5))
						{
							return ref_weapon2;
						}
					}
				}
			}
		}
		return null;
	}

	// Token: 0x06001442 RID: 5186 RVA: 0x000601A8 File Offset: 0x0005E3A8
	public TIShipPartTemplate GetPartInHullSlot(TIShipHullTemplate.ShipModuleSlot shipModuleSlot, bool testSecondarySlotsForWeapons)
	{
		return this.GetPartInHullSlotIndex(this.hullTemplate.slotIndex(shipModuleSlot), testSecondarySlotsForWeapons);
	}

	// Token: 0x06001443 RID: 5187 RVA: 0x000601BD File Offset: 0x0005E3BD
	public TIShipPartTemplate GetPartInHullSlot(Vector2 coordinates, bool testSecondarySlotsForWeapons)
	{
		return this.GetPartInHullSlot(this.hullTemplate.GetSlotByCoordinates(coordinates), testSecondarySlotsForWeapons);
	}

	// Token: 0x06001444 RID: 5188 RVA: 0x000601D4 File Offset: 0x0005E3D4
	public FireModeDataTemplateEntry GetFireModeDataEntryFromSlot(int slot)
	{
		for (int i = 0; i < this.fireModeTemplateEntries.Count; i++)
		{
			if (this.fireModeTemplateEntries[i].slot == slot)
			{
				return this.fireModeTemplateEntries[i];
			}
		}
		return new FireModeDataTemplateEntry(0, FireMode.Idle);
	}

	// Token: 0x06001445 RID: 5189 RVA: 0x00060220 File Offset: 0x0005E420
	public void SetFireModeForSlot(int slot, FireMode fireMode)
	{
		for (int i = 0; i < this.fireModeTemplateEntries.Count; i++)
		{
			if (this.fireModeTemplateEntries[i].slot == slot)
			{
				this.fireModeTemplateEntries.Remove(this.fireModeTemplateEntries[i]);
				FireModeDataTemplateEntry fireModeDataTemplateEntry = new FireModeDataTemplateEntry
				{
					slot = slot,
					fireMode = fireMode
				};
				this.fireModeTemplateEntries.Add(fireModeDataTemplateEntry);
				return;
			}
		}
		Debug.LogError("Could Not Set FireModeEntry Associated with Slot: " + slot.ToString());
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x000602AC File Offset: 0x0005E4AC
	public void ReCacheUtilityModules()
	{
		this.cachedUtilityModules = null;
		this.cachedUtilityModuleTemplates = null;
	}

	// Token: 0x170002D6 RID: 726
	// (get) Token: 0x06001447 RID: 5191 RVA: 0x000602BC File Offset: 0x0005E4BC
	public IEnumerable<ModuleDataEntry> utilityModules
	{
		get
		{
			if (!this.isIncompleteDesign && this.cachedUtilityModules != null)
			{
				return this.cachedUtilityModules;
			}
			List<ModuleDataEntry> list = new List<ModuleDataEntry>();
			foreach (ModuleDataTemplateEntry moduleDataTemplateEntry in this.moduleTemplateEntries)
			{
				if (!string.IsNullOrEmpty(moduleDataTemplateEntry.moduleName) && moduleDataTemplateEntry.moduleName != "Empty")
				{
					TIShipModuleTemplate tishipModuleTemplate = TemplateManager.Find<TIShipModuleTemplate>(moduleDataTemplateEntry.moduleName, true);
					if (tishipModuleTemplate != null)
					{
						if (this.ValidAssignedSlotForLocation(tishipModuleTemplate, moduleDataTemplateEntry.slot))
						{
							list.Add(new ModuleDataEntry(tishipModuleTemplate, moduleDataTemplateEntry.slot));
						}
						else
						{
							Log.Error("Bad utility module placement for " + moduleDataTemplateEntry.moduleName + " in " + base.dataName, Array.Empty<object>());
						}
					}
					else
					{
						Log.Error("Bad utility module templatename " + moduleDataTemplateEntry.moduleName + " in " + base.dataName, Array.Empty<object>());
					}
				}
			}
			if (!this.isIncompleteDesign)
			{
				this.cachedUtilityModules = list;
			}
			return list;
		}
	}

	// Token: 0x170002D7 RID: 727
	// (get) Token: 0x06001448 RID: 5192 RVA: 0x000603DC File Offset: 0x0005E5DC
	public IEnumerable<TIShipModuleTemplate> utilitySlotModuleTemplates
	{
		get
		{
			if (!this.isIncompleteDesign && this.cachedUtilityModuleTemplates != null)
			{
				return this.cachedUtilityModuleTemplates;
			}
			List<TIShipModuleTemplate> list = new List<TIShipModuleTemplate>();
			foreach (ModuleDataTemplateEntry moduleDataTemplateEntry in this.moduleTemplateEntries)
			{
				if (moduleDataTemplateEntry.moduleName != "Empty" && !string.IsNullOrEmpty(moduleDataTemplateEntry.moduleName))
				{
					TIShipModuleTemplate tishipModuleTemplate = TemplateManager.Find<TIShipModuleTemplate>(moduleDataTemplateEntry.moduleName, true);
					if (tishipModuleTemplate != null)
					{
						if (this.ValidAssignedSlotForLocation(tishipModuleTemplate, moduleDataTemplateEntry.slot))
						{
							list.Add(tishipModuleTemplate);
						}
						else
						{
							Log.Error("Bad utility module placement for " + moduleDataTemplateEntry.moduleName + " in " + base.dataName, Array.Empty<object>());
						}
					}
					else
					{
						Log.Error("Bad utility module templatename " + moduleDataTemplateEntry.moduleName + " in " + base.dataName, Array.Empty<object>());
					}
				}
			}
			if (!this.isIncompleteDesign)
			{
				this.cachedUtilityModuleTemplates = list;
			}
			return list;
		}
	}

	// Token: 0x06001449 RID: 5193 RVA: 0x000604F4 File Offset: 0x0005E6F4
	public float TargetingBonus(TIFactionState faction = null)
	{
		if (faction == null)
		{
			faction = this.designingFaction;
		}
		float num = 0f;
		IEnumerable<TIShipModuleTemplate> enumerable = this.utilitySlotModuleTemplates.Where<TIShipModuleTemplate>((TIShipModuleTemplate x) => x.ref_utilityModule != null);
		if (enumerable.Any<TIShipModuleTemplate>())
		{
			num = enumerable.Max<TIShipModuleTemplate>((TIShipModuleTemplate x) => x.ref_utilityModule.targetingValue);
		}
		if (this.hullTemplate.noShipyardBuild && faction != null)
		{
			num += TIEffectsState.SumEffectsModifiers(Context.TargetingComputerBonus, faction, num, null);
		}
		return num;
	}

	// Token: 0x0600144A RID: 5194 RVA: 0x00060598 File Offset: 0x0005E798
	public float ECMValue(bool attackerIsAlien, TIFactionState faction = null)
	{
		if (faction == null)
		{
			faction = this.designingFaction;
		}
		if (attackerIsAlien && !this.isAlien)
		{
			if (faction == null)
			{
				return 0f;
			}
			if (TIEffectsState.CheckForAnyEffectInContext(Context.HumanECMAgainstAliens, this.designingFaction))
			{
				return 0f;
			}
		}
		float num = 0f;
		IEnumerable<TIShipModuleTemplate> enumerable = this.utilitySlotModuleTemplates.Where<TIShipModuleTemplate>((TIShipModuleTemplate x) => x.ref_utilityModule != null);
		if (enumerable.Any<TIShipModuleTemplate>())
		{
			num = enumerable.Max<TIShipModuleTemplate>((TIShipModuleTemplate x) => x.ref_utilityModule.ECMValue);
		}
		if (this.hullTemplate.noShipyardBuild && faction != null)
		{
			num += TIEffectsState.SumEffectsModifiers(Context.TargetingComputerBonus, faction, num, null);
		}
		if (faction != null)
		{
			num += TIEffectsState.SumEffectsModifiers(Context.GlobalECMBonus, faction, num, null);
		}
		return num;
	}

	// Token: 0x170002D8 RID: 728
	// (get) Token: 0x0600144B RID: 5195 RVA: 0x00060688 File Offset: 0x0005E888
	public IEnumerable<ModuleDataEntry> noseWeapons
	{
		get
		{
			if (!this.isIncompleteDesign && this.cachedNoseWeapons != null)
			{
				return this.cachedNoseWeapons;
			}
			List<ModuleDataEntry> list = new List<ModuleDataEntry>();
			foreach (ModuleDataTemplateEntry moduleDataTemplateEntry in this.noseWeaponTemplateEntries)
			{
				if (!string.IsNullOrEmpty(moduleDataTemplateEntry.moduleName) && moduleDataTemplateEntry.moduleName != "Empty")
				{
					TIShipWeaponTemplate tishipWeaponTemplate = TemplateManager.Find<TIShipWeaponTemplate>(moduleDataTemplateEntry.moduleName, true);
					if (tishipWeaponTemplate != null)
					{
						if (this.ValidAssignedSlotForLocation(moduleDataTemplateEntry))
						{
							list.Add(new ModuleDataEntry(tishipWeaponTemplate, moduleDataTemplateEntry.slot));
						}
						else
						{
							this.ValidAssignedSlotForLocation(moduleDataTemplateEntry);
							Log.Error("Bad nose weapon placement for " + moduleDataTemplateEntry.moduleName + " in " + base.dataName, Array.Empty<object>());
						}
					}
					else
					{
						Log.Error("Bad nose module templatename " + moduleDataTemplateEntry.moduleName + " in " + base.dataName, Array.Empty<object>());
					}
				}
			}
			if (!this.isIncompleteDesign)
			{
				this.cachedNoseWeapons = list;
			}
			return list;
		}
	}

	// Token: 0x170002D9 RID: 729
	// (get) Token: 0x0600144C RID: 5196 RVA: 0x000607B0 File Offset: 0x0005E9B0
	public IEnumerable<TIShipWeaponTemplate> noseWeaponTemplates
	{
		get
		{
			if (!this.isIncompleteDesign && this.cachedNoseWeaponTemplates != null)
			{
				return this.cachedNoseWeaponTemplates;
			}
			List<TIShipWeaponTemplate> list = new List<TIShipWeaponTemplate>();
			foreach (ModuleDataTemplateEntry moduleDataTemplateEntry in this.noseWeaponTemplateEntries)
			{
				if (moduleDataTemplateEntry.moduleName != "Empty" && !string.IsNullOrEmpty(moduleDataTemplateEntry.moduleName))
				{
					TIShipWeaponTemplate tishipWeaponTemplate = TemplateManager.Find<TIShipWeaponTemplate>(moduleDataTemplateEntry.moduleName, true);
					if (tishipWeaponTemplate != null)
					{
						if (this.ValidAssignedSlotForLocation(tishipWeaponTemplate, moduleDataTemplateEntry.slot))
						{
							list.Add(tishipWeaponTemplate);
						}
						else
						{
							Log.Error("Bad nose weapon placement for " + moduleDataTemplateEntry.moduleName + " in " + base.dataName, Array.Empty<object>());
						}
					}
					else
					{
						Log.Error("Bad nose weapon templatename " + moduleDataTemplateEntry.moduleName + " in " + base.dataName, Array.Empty<object>());
					}
				}
			}
			if (!this.isIncompleteDesign)
			{
				this.cachedNoseWeaponTemplates = list;
			}
			return list;
		}
	}

	// Token: 0x170002DA RID: 730
	// (get) Token: 0x0600144D RID: 5197 RVA: 0x000608C8 File Offset: 0x0005EAC8
	public IEnumerable<ModuleDataEntry> hullWeapons
	{
		get
		{
			if (!this.isIncompleteDesign && this.cachedHullWeapons != null)
			{
				return this.cachedHullWeapons;
			}
			List<ModuleDataEntry> list = new List<ModuleDataEntry>();
			foreach (ModuleDataTemplateEntry moduleDataTemplateEntry in this.hullWeaponTemplateEntries)
			{
				if (!string.IsNullOrEmpty(moduleDataTemplateEntry.moduleName) && moduleDataTemplateEntry.moduleName != "Empty")
				{
					TIShipWeaponTemplate tishipWeaponTemplate = TemplateManager.Find<TIShipWeaponTemplate>(moduleDataTemplateEntry.moduleName, true);
					if (tishipWeaponTemplate != null)
					{
						if (this.ValidAssignedSlotForLocation(tishipWeaponTemplate, moduleDataTemplateEntry.slot))
						{
							list.Add(new ModuleDataEntry(tishipWeaponTemplate, moduleDataTemplateEntry.slot));
						}
						else
						{
							Log.Error("Bad hull weapon placement for " + moduleDataTemplateEntry.moduleName + " in " + base.dataName, Array.Empty<object>());
						}
					}
					else
					{
						Log.Error("Bad hull weapon module templatename " + moduleDataTemplateEntry.moduleName + " in " + base.dataName, Array.Empty<object>());
					}
				}
			}
			if (!this.isIncompleteDesign)
			{
				this.cachedHullWeapons = list;
			}
			return list;
		}
	}

	// Token: 0x170002DB RID: 731
	// (get) Token: 0x0600144E RID: 5198 RVA: 0x000609E8 File Offset: 0x0005EBE8
	public IEnumerable<TIShipWeaponTemplate> hullWeaponTemplates
	{
		get
		{
			if (!this.isIncompleteDesign && this.cachedHullWeaponTemplates != null)
			{
				return this.cachedHullWeaponTemplates;
			}
			List<TIShipWeaponTemplate> list = new List<TIShipWeaponTemplate>();
			foreach (ModuleDataTemplateEntry moduleDataTemplateEntry in this.hullWeaponTemplateEntries)
			{
				if (moduleDataTemplateEntry.moduleName != "Empty" && !string.IsNullOrEmpty(moduleDataTemplateEntry.moduleName))
				{
					TIShipWeaponTemplate tishipWeaponTemplate = TemplateManager.Find<TIShipWeaponTemplate>(moduleDataTemplateEntry.moduleName, true);
					if (tishipWeaponTemplate != null)
					{
						if (this.ValidAssignedSlotForLocation(moduleDataTemplateEntry))
						{
							list.Add(tishipWeaponTemplate);
						}
						else
						{
							Log.Error("Bad hull weapon placement for " + moduleDataTemplateEntry.moduleName + " in " + base.dataName, Array.Empty<object>());
						}
					}
					else
					{
						Log.Error("Bad hull weapon templatename " + moduleDataTemplateEntry.moduleName + " in " + base.dataName, Array.Empty<object>());
					}
				}
			}
			if (!this.isIncompleteDesign)
			{
				this.cachedHullWeaponTemplates = list;
			}
			return list;
		}
	}

	// Token: 0x170002DC RID: 732
	// (get) Token: 0x0600144F RID: 5199 RVA: 0x00060AF4 File Offset: 0x0005ECF4
	public List<ModuleDataEntry> allWeapons
	{
		get
		{
			List<ModuleDataEntry> list = new List<ModuleDataEntry>(this.noseWeapons);
			list.AddRange(this.hullWeapons);
			return list;
		}
	}

	// Token: 0x170002DD RID: 733
	// (get) Token: 0x06001450 RID: 5200 RVA: 0x00060B0D File Offset: 0x0005ED0D
	public List<TIShipWeaponTemplate> allWeaponTemplates
	{
		get
		{
			List<TIShipWeaponTemplate> list = new List<TIShipWeaponTemplate>(this.noseWeaponTemplates);
			list.AddRange(this.hullWeaponTemplates);
			return list;
		}
	}

	// Token: 0x170002DE RID: 734
	// (get) Token: 0x06001451 RID: 5201 RVA: 0x00060B28 File Offset: 0x0005ED28
	public List<ModuleDataEntry> heatSinkModules
	{
		get
		{
			List<ModuleDataEntry> list = new List<ModuleDataEntry>();
			foreach (ModuleDataEntry moduleDataEntry in this.utilityModules)
			{
				if (moduleDataEntry.moduleTemplate is TIHeatSinkTemplate)
				{
					list.Add(moduleDataEntry);
				}
			}
			return list;
		}
	}

	// Token: 0x170002DF RID: 735
	// (get) Token: 0x06001452 RID: 5202 RVA: 0x00060B8C File Offset: 0x0005ED8C
	public List<ModuleDataEntry> batteryModules
	{
		get
		{
			List<ModuleDataEntry> list = new List<ModuleDataEntry>();
			foreach (ModuleDataEntry moduleDataEntry in this.utilityModules)
			{
				if (moduleDataEntry.moduleTemplate.isBattery)
				{
					list.Add(moduleDataEntry);
				}
			}
			return list;
		}
	}

	// Token: 0x170002E0 RID: 736
	// (get) Token: 0x06001453 RID: 5203 RVA: 0x00060BF0 File Offset: 0x0005EDF0
	public int crewBillets
	{
		get
		{
			int num = 0;
			num += this.hullTemplate.crew;
			if (!this.hullTemplate.simpleHull)
			{
				num += ((this.driveTemplate != null) ? this.driveTemplate.crew : 0);
				num += ((this.powerPlantTemplate != null) ? this.powerPlantTemplate.crew : 0);
				num += ((this.radiatorTemplate != null) ? this.radiatorTemplate.crew : 0);
				foreach (TIShipWeaponTemplate tishipWeaponTemplate in this.hullWeaponTemplates)
				{
					num += tishipWeaponTemplate.crew;
				}
				foreach (TIShipWeaponTemplate tishipWeaponTemplate2 in this.noseWeaponTemplates)
				{
					num += tishipWeaponTemplate2.crew;
				}
				foreach (TIShipModuleTemplate tishipModuleTemplate in this.utilitySlotModuleTemplates)
				{
					num += tishipModuleTemplate.crew;
				}
			}
			return num;
		}
	}

	// Token: 0x170002E1 RID: 737
	// (get) Token: 0x06001454 RID: 5204 RVA: 0x00060D30 File Offset: 0x0005EF30
	public int damConCrewBillets
	{
		get
		{
			float num = 0f;
			foreach (ModuleDataEntry moduleDataEntry in this.utilityModules)
			{
				TIUtilityModuleTemplate ref_utilityModule = moduleDataEntry.moduleTemplate.ref_utilityModule;
				if (ref_utilityModule != null && ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Repair))
				{
					num += (float)moduleDataEntry.moduleTemplate.crew;
				}
			}
			return Mathf.CeilToInt((float)this.hullTemplate.crew / 2f + num);
		}
	}

	// Token: 0x170002E2 RID: 738
	// (get) Token: 0x06001455 RID: 5205 RVA: 0x00060DC4 File Offset: 0x0005EFC4
	public bool shipHasALaser
	{
		get
		{
			return this.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.ref_laserWeapon != null);
		}
	}

	// Token: 0x170002E3 RID: 739
	// (get) Token: 0x06001456 RID: 5206 RVA: 0x00060DF0 File Offset: 0x0005EFF0
	public bool shipHasAParticleBeam
	{
		get
		{
			return this.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.ref_particleWeapon != null);
		}
	}

	// Token: 0x06001457 RID: 5207 RVA: 0x00060E1C File Offset: 0x0005F01C
	public bool ValidPartForDesign(TIShipPartTemplate part)
	{
		TIUtilityModuleTemplate ref_utilityModule = part.ref_utilityModule;
		if (ref_utilityModule != null)
		{
			if (!this.ValidUtilityModuleForDrive(ref_utilityModule, this.driveTemplate))
			{
				return false;
			}
			if (ref_utilityModule.laserPowerBonus_MW > 0f && !this.shipHasALaser)
			{
				return false;
			}
			if (ref_utilityModule.particleBeamPowerBonus_MW > 0f && !this.shipHasAParticleBeam)
			{
				return false;
			}
			if (ref_utilityModule.grouping != -1)
			{
				foreach (ModuleDataEntry moduleDataEntry in this.utilityModules)
				{
					TIUtilityModuleTemplate ref_utilityModule2 = moduleDataEntry.moduleTemplate.ref_utilityModule;
					if (ref_utilityModule2 != null && ref_utilityModule2.grouping == ref_utilityModule.grouping)
					{
						return false;
					}
				}
			}
			if (this.hullTemplate.GetAllSlotsOfType(ShipModuleSlotType.Utility).Count == 0)
			{
				return false;
			}
			if (this.hullTemplate.consTier < ref_utilityModule.minConsTier)
			{
				return false;
			}
		}
		else
		{
			if (part.ref_drive == part)
			{
				if (!this.validDrivesForPowerPlant.Contains(part))
				{
					return false;
				}
				using (IEnumerator<ModuleDataEntry> enumerator = this.utilityModules.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ModuleDataEntry moduleDataEntry2 = enumerator.Current;
						if (!this.ValidUtilityModuleForDrive(moduleDataEntry2.moduleTemplate.ref_utilityModule, part.ref_drive))
						{
							return false;
						}
					}
					return true;
				}
			}
			if (part.isBattery)
			{
				if (this.batteryTemplates.Count > 0 && part != this.batteryTemplates[0])
				{
					return false;
				}
			}
			else
			{
				if (part.ref_powerPlant == part && !this.validPowerPlantsForDrive.Contains(part))
				{
					return false;
				}
				TIShipWeaponTemplate ref_weapon = part.ref_weapon;
				if (ref_weapon != null)
				{
					if (ref_weapon.noseWeapon && this.hullTemplate.GetAllSlotsOfType(ShipModuleSlotType.NoseHardPoint).Count == 0)
					{
						return false;
					}
					if (ref_weapon.hullWeapon && this.hullTemplate.GetAllSlotsOfType(ShipModuleSlotType.HullHardPoint).Count == 0)
					{
						return false;
					}
					if (ref_weapon.multiSlot && this.hullTemplate.ValidBigWeaponSlotSets(ref_weapon.mount).Count == 0)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06001458 RID: 5208 RVA: 0x00061030 File Offset: 0x0005F230
	private bool ValidUtilityModuleForDrive(TIUtilityModuleTemplate utilityModule, TIDriveTemplate drive)
	{
		return utilityModule == null || drive == null || ((!utilityModule.requiresFusionDrive || drive.fusionDrive) && (!utilityModule.requiresNuclearDrive || drive.nuclearThermalDrive) && (!utilityModule.requiresHydrogenPropellant || drive.propellant == Propellant.Hydrogen) && (!utilityModule.requiresFissionDrive || drive.fissionDrive) && (!utilityModule.requiresNonISRUDrive || !drive.freeISRU) && (!utilityModule.specialModuleRules.Contains(SpecialModuleRule.RefuelFromUnimprovedSites) || drive.perTankPropellantMaterials.antimatter <= 0f));
	}

	// Token: 0x06001459 RID: 5209 RVA: 0x000610C3 File Offset: 0x0005F2C3
	private bool ValidPowerPlantForShipsDrive(TIPowerPlantTemplate powerPlantToCheck)
	{
		return this.driveTemplate == null || this.driveTemplate.IsCompatible(powerPlantToCheck);
	}

	// Token: 0x170002E4 RID: 740
	// (get) Token: 0x0600145A RID: 5210 RVA: 0x000610DC File Offset: 0x0005F2DC
	public List<TIPowerPlantTemplate> validPowerPlantsForDrive
	{
		get
		{
			List<TIPowerPlantTemplate> list = new List<TIPowerPlantTemplate>();
			foreach (TIPowerPlantTemplate tipowerPlantTemplate in TemplateManager.IterateByClass<TIPowerPlantTemplate>(true))
			{
				if (this.ValidPowerPlantForShipsDrive(tipowerPlantTemplate))
				{
					list.Add(tipowerPlantTemplate);
				}
			}
			return list;
		}
	}

	// Token: 0x0600145B RID: 5211 RVA: 0x0006113C File Offset: 0x0005F33C
	public bool validDriveForShipsPowerPlant(TIDriveTemplate driveToCheck)
	{
		return (this.powerPlantTemplate == null || driveToCheck.requiredPowerPlant == PowerPlantRequirement.Any_General || this.powerPlantTemplate.powerPlantClass == driveToCheck.requiredPowerPlant || (driveToCheck.requiredPowerPlant == PowerPlantRequirement.Any_Magnetic_Confinement_Fusion && this.powerPlantTemplate.magneticFusionPlant) || (this.powerPlantTemplate.powerPlantClass == PowerPlantRequirement.Molten_Salt_Core_Fission && (driveToCheck.requiredPowerPlant == PowerPlantRequirement.Solid_Core_Fission || driveToCheck.requiredPowerPlant == PowerPlantRequirement.Liquid_Core_Fission))) && (this.powerPlantTemplate == null || driveToCheck.powerRequirement_GW <= this.powerPlantTemplate.maxOutput_GW);
	}

	// Token: 0x170002E5 RID: 741
	// (get) Token: 0x0600145C RID: 5212 RVA: 0x000611C0 File Offset: 0x0005F3C0
	public List<TIDriveTemplate> validDrivesForPowerPlant
	{
		get
		{
			List<TIDriveTemplate> list = new List<TIDriveTemplate>();
			foreach (TIDriveTemplate tidriveTemplate in TemplateManager.IterateByClass<TIDriveTemplate>(true))
			{
				if (this.validDriveForShipsPowerPlant(tidriveTemplate))
				{
					list.Add(tidriveTemplate);
				}
			}
			return list;
		}
	}

	// Token: 0x0600145D RID: 5213 RVA: 0x00061220 File Offset: 0x0005F420
	public static List<TIDriveTemplate> ValidDrivesForPowerPlants(List<TIDriveTemplate> candidateDrives, IEnumerable<TIPowerPlantTemplate> availablePowerPlants)
	{
		List<TIDriveTemplate> list = new List<TIDriveTemplate>();
		foreach (TIDriveTemplate tidriveTemplate in candidateDrives)
		{
			foreach (TIPowerPlantTemplate tipowerPlantTemplate in availablePowerPlants)
			{
				if ((tidriveTemplate.requiredPowerPlant == PowerPlantRequirement.Any_General || tipowerPlantTemplate.powerPlantClass == tidriveTemplate.requiredPowerPlant || (tidriveTemplate.requiredPowerPlant == PowerPlantRequirement.Any_Magnetic_Confinement_Fusion && tipowerPlantTemplate.magneticFusionPlant) || (tipowerPlantTemplate.powerPlantClass == PowerPlantRequirement.Molten_Salt_Core_Fission && (tidriveTemplate.requiredPowerPlant == PowerPlantRequirement.Solid_Core_Fission || tidriveTemplate.requiredPowerPlant == PowerPlantRequirement.Liquid_Core_Fission))) && tidriveTemplate.powerRequirement_GW <= tipowerPlantTemplate.maxOutput_GW)
				{
					list.Add(tidriveTemplate);
					break;
				}
			}
		}
		return list;
	}

	// Token: 0x0600145E RID: 5214 RVA: 0x00061304 File Offset: 0x0005F504
	public int GetIdealPropellentTankCount(float desiredDV, out float actualDV, float minimumReturnRatio, float maximumReturnRatio)
	{
		float cachedEV_kps = this.modifiedEV_kps;
		float cachedDryMass_tons = this.dryMass_tons(false);
		float cachedWetMassPerTank_tons = 100f;
		Func<int, float> GetDV = delegate(int tankCount)
		{
			float num5 = cachedWetMassPerTank_tons * (float)tankCount + cachedDryMass_tons;
			return cachedEV_kps * Mathf.Log(num5 / cachedDryMass_tons);
		};
		Func<int, float> func = delegate(int tankCount)
		{
			float num6 = GetDV(tankCount);
			return (GetDV(tankCount + 1) - num6) / num6 / (1f / (float)tankCount);
		};
		int num = 1;
		float num2 = func(num);
		int num3 = 0;
		int num4 = 40;
		while ((GetDV(num) < desiredDV || num2 > maximumReturnRatio) && num2 > minimumReturnRatio)
		{
			if (num3++ > num4)
			{
				actualDV = GetDV(num);
				return num;
			}
			num += Mathf.Max((int)((float)num * 0.2f), 1);
			num2 = func(num);
		}
		if (GetDV(num) < desiredDV)
		{
			actualDV = GetDV(num);
			return num;
		}
		while (GetDV(num - 1) > desiredDV && func(num - 1) < maximumReturnRatio)
		{
			num--;
		}
		actualDV = GetDV(num);
		return num;
	}

	// Token: 0x0600145F RID: 5215 RVA: 0x0006140C File Offset: 0x0005F60C
	public int GetIdealPropellentTankCount(float desiredDV, out float actualDV)
	{
		float num = 0.74f;
		if (this.role == ShipRole.InnerSystemColonyShip || this.role == ShipRole.OuterSystemColonyShip)
		{
			num = 0.6f;
		}
		else if (this.hullTemplate.smallHull)
		{
			num -= 0.06f;
		}
		float num2 = 0.9f;
		return this.GetIdealPropellentTankCount(desiredDV, out actualDV, num, num2);
	}

	// Token: 0x06001460 RID: 5216 RVA: 0x00061460 File Offset: 0x0005F660
	public float GetRelativeValueOfRefit(TISpaceShipTemplate refit)
	{
		string text;
		if (refit == null || !refit.IsAValidRefitFor(this, out text, false))
		{
			return -1f;
		}
		float num = 1f;
		num *= refit.baseCruiseDeltaV_kps(false) / this.baseCruiseDeltaV_kps(false);
		if (num < 1f)
		{
			num = Mathf.Pow(num, 3f);
		}
		num *= refit.baseCruiseAcceleration_mps2(false) / this.baseCruiseAcceleration_mps2(false);
		return num * (refit.TemplateSpaceCombatValue(false, -1f, 1f, false) / this.TemplateSpaceCombatValue(false, -1f, 1f, false));
	}

	// Token: 0x06001461 RID: 5217 RVA: 0x000614EC File Offset: 0x0005F6EC
	public TIResourcesCost RefitResourceCost(TIHabModuleState shipyard, TISpaceShipTemplate originalDesign, bool includePropellant = true, bool includeRefuel = false, TISpaceShipState shipRefitting = null)
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		if (this.driveTemplate != originalDesign.driveTemplate || this.powerPlantTemplate != originalDesign.powerPlantTemplate || this.radiatorTemplate != originalDesign.radiatorTemplate)
		{
			tiresourcesCost.SumCosts_NoDuration(this.driveTemplate.buildCost(0f, 0f));
			tiresourcesCost.SubtractRefitDiscountCost(originalDesign.driveTemplate.buildCost(0f, 0f));
			tiresourcesCost.SumCosts_NoDuration(this.powerPlantBuildCost);
			tiresourcesCost.SubtractRefitDiscountCost(originalDesign.powerPlantBuildCost);
			tiresourcesCost.SumCosts_NoDuration(this.radiatorsBuildCost);
			tiresourcesCost.SubtractRefitDiscountCost(originalDesign.radiatorsBuildCost);
		}
		if (this.lateralArmorTemplate != originalDesign.lateralArmorTemplate || this.lateralArmor.armorValue != originalDesign.lateralArmorValue)
		{
			tiresourcesCost.SumCosts_NoDuration(this.lateralArmorBuildCost);
			tiresourcesCost.SubtractRefitDiscountCost(originalDesign.lateralArmorBuildCost);
		}
		if (this.noseArmorTemplate != originalDesign.noseArmorTemplate || this.noseArmor.armorValue != originalDesign.noseArmorValue)
		{
			tiresourcesCost.SumCosts_NoDuration(this.noseArmorBuildCost);
			tiresourcesCost.SubtractRefitDiscountCost(originalDesign.noseArmorBuildCost);
		}
		if (this.tailArmorTemplate != originalDesign.tailArmorTemplate || this.tailArmor.armorValue != originalDesign.tailArmorValue)
		{
			tiresourcesCost.SumCosts_NoDuration(this.tailArmorBuildCost);
			tiresourcesCost.SubtractRefitDiscountCost(originalDesign.tailArmorBuildCost);
		}
		if (includePropellant)
		{
			tiresourcesCost.SumCosts_NoDuration(this.propellantTanksBuildCost(((shipyard != null) ? shipyard.ref_faction : null) ?? this.designingFaction));
			tiresourcesCost.SubtractRefitPropellantCost(originalDesign.propellantTanksBuildCost(((shipyard != null) ? shipyard.ref_faction : null) ?? this.designingFaction));
			if (includeRefuel && shipRefitting != null)
			{
				tiresourcesCost.SumCosts_NoDuration(ResupplyOperation.ExpectedShipRefuelCost(shipRefitting, this.designingFaction, this));
			}
		}
		TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
		if (shipRefitting != null)
		{
			Dictionary<TISpaceShipState, int> dictionary;
			tiresourcesCost2 = RepairFleetOperation.ExpectedRefitShipRepairCost(shipRefitting, (shipyard != null) ? shipyard.hab : null, this.designingFaction, out dictionary);
			tiresourcesCost.SumCosts_NoDuration(tiresourcesCost2);
		}
		List<TIShipModuleTemplate> list = new List<TIShipModuleTemplate>();
		List<TIShipModuleTemplate> list2 = new List<TIShipModuleTemplate>();
		List<TIShipWeaponTemplate> list3 = new List<TIShipWeaponTemplate>();
		List<TIShipWeaponTemplate> list4 = new List<TIShipWeaponTemplate>();
		List<TIShipModuleTemplate> list5 = new List<TIShipModuleTemplate>(originalDesign.utilitySlotModuleTemplates);
		foreach (TIShipModuleTemplate tishipModuleTemplate in this.utilitySlotModuleTemplates)
		{
			bool flag = true;
			foreach (TIShipModuleTemplate tishipModuleTemplate2 in list5)
			{
				if (tishipModuleTemplate2 == tishipModuleTemplate)
				{
					flag = false;
					list5.Remove(tishipModuleTemplate2);
					break;
				}
			}
			if (flag)
			{
				list.Add(tishipModuleTemplate);
			}
		}
		List<TIShipModuleTemplate> list6 = new List<TIShipModuleTemplate>(this.utilitySlotModuleTemplates);
		foreach (TIShipModuleTemplate tishipModuleTemplate3 in originalDesign.utilitySlotModuleTemplates)
		{
			bool flag2 = true;
			foreach (TIShipModuleTemplate tishipModuleTemplate4 in list6)
			{
				if (tishipModuleTemplate3 == tishipModuleTemplate4)
				{
					flag2 = false;
					list6.Remove(tishipModuleTemplate4);
					break;
				}
			}
			if (flag2)
			{
				list2.Add(tishipModuleTemplate3);
			}
		}
		List<TIShipWeaponTemplate> list7 = new List<TIShipWeaponTemplate>(originalDesign.allWeaponTemplates);
		foreach (TIShipWeaponTemplate tishipWeaponTemplate in this.allWeaponTemplates)
		{
			bool flag3 = true;
			foreach (TIShipWeaponTemplate tishipWeaponTemplate2 in list7)
			{
				if (tishipWeaponTemplate2 == tishipWeaponTemplate)
				{
					flag3 = false;
					list7.Remove(tishipWeaponTemplate2);
					break;
				}
			}
			if (flag3)
			{
				list3.Add(tishipWeaponTemplate);
			}
		}
		List<TIShipWeaponTemplate> list8 = new List<TIShipWeaponTemplate>(this.allWeaponTemplates);
		foreach (TIShipWeaponTemplate tishipWeaponTemplate3 in originalDesign.allWeaponTemplates)
		{
			bool flag4 = true;
			foreach (TIShipWeaponTemplate tishipWeaponTemplate4 in list8)
			{
				if (tishipWeaponTemplate3 == tishipWeaponTemplate4)
				{
					flag4 = false;
					list8.Remove(tishipWeaponTemplate4);
					break;
				}
			}
			if (flag4)
			{
				list4.Add(tishipWeaponTemplate3);
			}
		}
		foreach (TIShipModuleTemplate tishipModuleTemplate5 in list)
		{
			tiresourcesCost.SumCosts_NoDuration(tishipModuleTemplate5.buildCost(0f, 0f));
		}
		foreach (TIShipModuleTemplate tishipModuleTemplate6 in list2)
		{
			tiresourcesCost.SubtractRefitDiscountCost(tishipModuleTemplate6.buildCost(0f, 0f));
		}
		foreach (TIShipWeaponTemplate tishipWeaponTemplate5 in list3)
		{
			tiresourcesCost.SumCosts_NoDuration(tishipWeaponTemplate5.buildCost(0f, 0f));
		}
		foreach (TIShipWeaponTemplate tishipWeaponTemplate6 in list4)
		{
			tiresourcesCost.SubtractRefitDiscountCost(tishipWeaponTemplate6.buildCost(0f, 0f));
		}
		float num = this.GetRefitBuildTimeDays(shipyard, originalDesign);
		if (shipRefitting != null)
		{
			num += tiresourcesCost2.completionTime_days;
		}
		tiresourcesCost.SetCompletionTime_Days(num);
		return tiresourcesCost;
	}

	// Token: 0x06001462 RID: 5218 RVA: 0x00061AF4 File Offset: 0x0005FCF4
	public float GetRefitBuildTimeDays(TIHabModuleState shipyard, TISpaceShipTemplate originalDesign)
	{
		float num = 0f;
		if (this.driveTemplate != originalDesign.driveTemplate)
		{
			num += 0.25f;
		}
		if (this.powerPlantTemplate != originalDesign.powerPlantTemplate)
		{
			num += 0.25f;
		}
		if (this.radiatorTemplate != originalDesign.radiatorTemplate)
		{
			num += 0.05f;
		}
		if (this.lateralArmorTemplate != originalDesign.lateralArmorTemplate || this.lateralArmor.armorValue != originalDesign.lateralArmorValue)
		{
			num += 0.05f;
		}
		if (this.noseArmorTemplate != originalDesign.noseArmorTemplate || this.noseArmor.armorValue != originalDesign.noseArmorValue)
		{
			num += 0.05f;
		}
		if (this.tailArmorTemplate != originalDesign.tailArmorTemplate || this.tailArmor.armorValue != originalDesign.tailArmorValue)
		{
			num += 0.05f;
		}
		List<TIShipModuleTemplate> list = new List<TIShipModuleTemplate>();
		List<TIShipWeaponTemplate> list2 = new List<TIShipWeaponTemplate>();
		List<TIShipModuleTemplate> list3 = new List<TIShipModuleTemplate>(originalDesign.utilitySlotModuleTemplates);
		foreach (TIShipModuleTemplate tishipModuleTemplate in this.utilitySlotModuleTemplates)
		{
			bool flag = true;
			foreach (TIShipModuleTemplate tishipModuleTemplate2 in list3)
			{
				if (tishipModuleTemplate2 == tishipModuleTemplate)
				{
					flag = false;
					list3.Remove(tishipModuleTemplate2);
					break;
				}
			}
			if (flag)
			{
				list.Add(tishipModuleTemplate);
			}
		}
		List<TIShipWeaponTemplate> list4 = new List<TIShipWeaponTemplate>(originalDesign.allWeaponTemplates);
		foreach (TIShipWeaponTemplate tishipWeaponTemplate in this.allWeaponTemplates)
		{
			bool flag2 = true;
			foreach (TIShipWeaponTemplate tishipWeaponTemplate2 in list4)
			{
				if (tishipWeaponTemplate2 == tishipWeaponTemplate)
				{
					flag2 = false;
					list4.Remove(tishipWeaponTemplate2);
					break;
				}
			}
			if (flag2)
			{
				list2.Add(tishipWeaponTemplate);
			}
		}
		foreach (TIShipModuleTemplate tishipModuleTemplate3 in list)
		{
			num += 0.05f;
		}
		foreach (TIShipWeaponTemplate tishipWeaponTemplate3 in list2)
		{
			num += 0.05f;
		}
		if (num > TemplateManager.global.refitBuildTimeCap)
		{
			num = TemplateManager.global.refitBuildTimeCap;
		}
		return ((shipyard != null) ? this.hullTemplate.constructionTime_Days(shipyard) : this.hullTemplate.noShipyardConstructionTime_Days(this.designingFaction)) * num;
	}

	// Token: 0x06001463 RID: 5219 RVA: 0x00061DE0 File Offset: 0x0005FFE0
	public bool IsAValidRefitFor(TISpaceShipTemplate oldShipTemplate, out string reason, bool getReason = false)
	{
		reason = string.Empty;
		if (oldShipTemplate == null || oldShipTemplate == this || this.IsDuplicateOf(oldShipTemplate))
		{
			if (getReason)
			{
				reason = new StringBuilder().Append(Environment.NewLine).Append(Environment.NewLine).Append(TIUtilities.RedLine(Loc.T("UI.Fleets.RefitFailDuplicate")))
					.ToString();
			}
			return false;
		}
		if (this.hullTemplate != oldShipTemplate.hullTemplate)
		{
			if (getReason)
			{
				reason = new StringBuilder().Append(Environment.NewLine).Append(Environment.NewLine).Append(TIUtilities.RedLine(Loc.T("UI.Fleets.RefitFailHull")))
					.ToString();
			}
			return false;
		}
		if ((this.powerPlantTemplate == null && oldShipTemplate.powerPlantTemplate != null) || !this.powerPlantTemplate.IsValidRefitPart(oldShipTemplate))
		{
			if (getReason)
			{
				reason = new StringBuilder().Append(Environment.NewLine).Append(Environment.NewLine).Append(TIUtilities.RedLine(Loc.T("UI.Fleets.RefitFailPowerPlant")))
					.ToString();
			}
			return false;
		}
		if ((this.driveTemplate == null && oldShipTemplate.driveTemplate != null) || !this.driveTemplate.IsValidRefitPart(oldShipTemplate))
		{
			if (getReason)
			{
				reason = new StringBuilder().Append(Environment.NewLine).Append(Environment.NewLine).Append(TIUtilities.RedLine(Loc.T("UI.Fleets.RefitFailDrive")))
					.ToString();
			}
			return false;
		}
		if (this.heatSinkModules.Count < oldShipTemplate.heatSinkModules.Count)
		{
			if (getReason)
			{
				reason = new StringBuilder().Append(Environment.NewLine).Append(Environment.NewLine).Append(TIUtilities.RedLine(Loc.T("UI.Fleets.RefitFailHeatSink")))
					.ToString();
			}
			return false;
		}
		if (this.batteryModules.Count < oldShipTemplate.batteryModules.Count)
		{
			if (getReason)
			{
				reason = new StringBuilder().Append(Environment.NewLine).Append(Environment.NewLine).Append(TIUtilities.RedLine(Loc.T("UI.Fleets.RefitFailBattery")))
					.ToString();
			}
			return false;
		}
		if (!this.AreUtilityModulesValidForRefit(oldShipTemplate))
		{
			if (getReason)
			{
				reason = new StringBuilder().Append(Environment.NewLine).Append(Environment.NewLine).Append(TIUtilities.RedLine(Loc.T("UI.Fleets.RefitFailUtilityModule")))
					.ToString();
			}
			return false;
		}
		if (!this.AreWeaponModulesValidForRefit(oldShipTemplate))
		{
			if (getReason)
			{
				reason = new StringBuilder().Append(Environment.NewLine).Append(Environment.NewLine).Append(TIUtilities.RedLine(Loc.T("UI.Fleets.RefitFailWeaponModule")))
					.ToString();
			}
			return false;
		}
		return true;
	}

	// Token: 0x06001464 RID: 5220 RVA: 0x0006205C File Offset: 0x0006025C
	public bool AreUtilityModulesValidForRefit(TISpaceShipTemplate oldShipTemplate)
	{
		int num = 0;
		foreach (ModuleDataEntry moduleDataEntry in oldShipTemplate.utilityModules)
		{
			if (moduleDataEntry.moduleTemplate.isUtilityModule && moduleDataEntry.moduleTemplate.ref_utilityModule.grouping > num - 1)
			{
				num = moduleDataEntry.moduleTemplate.ref_utilityModule.grouping + 1;
			}
		}
		int num2 = 0;
		foreach (ModuleDataEntry moduleDataEntry2 in this.utilityModules)
		{
			if (moduleDataEntry2.moduleTemplate.isUtilityModule && moduleDataEntry2.moduleTemplate.ref_utilityModule.grouping > num2 - 1)
			{
				num2 = moduleDataEntry2.moduleTemplate.ref_utilityModule.grouping + 1;
			}
		}
		int num3 = Mathf.Max(num2, num);
		List<int> list = new List<int>(new int[num3 + 1]);
		foreach (ModuleDataEntry moduleDataEntry3 in oldShipTemplate.utilityModules)
		{
			if (moduleDataEntry3.moduleTemplate.isUtilityModule)
			{
				List<int> list2 = list;
				int num4 = moduleDataEntry3.moduleTemplate.ref_utilityModule.grouping + 1;
				int num5 = list2[num4];
				list2[num4] = num5 + 1;
			}
		}
		List<int> list3 = new List<int>(new int[num3 + 1]);
		foreach (ModuleDataEntry moduleDataEntry4 in this.utilityModules)
		{
			if (moduleDataEntry4.moduleTemplate.isUtilityModule)
			{
				List<int> list4 = list3;
				int num5 = moduleDataEntry4.moduleTemplate.ref_utilityModule.grouping + 1;
				int num4 = list4[num5];
				list4[num5] = num4 + 1;
			}
		}
		for (int i = 0; i < list3.Count; i++)
		{
			if (list3[i] < list[i])
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001465 RID: 5221 RVA: 0x00062294 File Offset: 0x00060494
	public bool AreWeaponModulesValidForRefit(TISpaceShipTemplate oldShipTemplate)
	{
		List<int> list = new List<int>(new int[6]);
		List<int> list2 = new List<int>(new int[6]);
		List<int> list3 = new List<int>(new int[6]);
		List<int> list4 = new List<int>(new int[6]);
		List<int> list5 = new List<int>(new int[6]);
		List<int> list6 = new List<int>(new int[6]);
		List<int> list7 = new List<int>(new int[6]);
		List<int> list8 = new List<int>(new int[6]);
		List<int> list9 = new List<int>(new int[6]);
		List<int> list10 = new List<int>(new int[6]);
		List<int> list11 = new List<int>(new int[6]);
		List<int> list12 = new List<int>(new int[6]);
		List<int> list13 = new List<int>(new int[6]);
		List<int> list14 = new List<int>(new int[6]);
		List<int> list15 = new List<int>(new int[6]);
		List<int> list16 = new List<int>(new int[6]);
		foreach (ModuleDataEntry moduleDataEntry in this.allWeapons)
		{
			TIShipWeaponTemplate ref_weapon = moduleDataEntry.moduleTemplate.ref_weapon;
			switch (moduleDataEntry.moduleTemplate.ref_weapon.internalSize)
			{
			case 1:
				switch (moduleDataEntry.moduleTemplate.ref_weapon.weaponClass)
				{
				case WeaponClass.NavalGun:
					if (ref_weapon.hullWeapon)
					{
						List<int> list17 = list;
						int num = list17[0];
						list17[0] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list18 = list5;
						int num = list18[0];
						list18[0] = num + 1;
					}
					break;
				case WeaponClass.Laser:
					if (ref_weapon.hullWeapon)
					{
						List<int> list19 = list;
						int num = list19[1];
						list19[1] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list20 = list5;
						int num = list20[1];
						list20[1] = num + 1;
					}
					break;
				case WeaponClass.Particle:
					if (ref_weapon.hullWeapon)
					{
						List<int> list21 = list;
						int num = list21[2];
						list21[2] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list22 = list5;
						int num = list22[2];
						list22[2] = num + 1;
					}
					break;
				case WeaponClass.Magnetic:
					if (ref_weapon.hullWeapon)
					{
						List<int> list23 = list;
						int num = list23[3];
						list23[3] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list24 = list5;
						int num = list24[3];
						list24[3] = num + 1;
					}
					break;
				case WeaponClass.Plasma:
					if (ref_weapon.hullWeapon)
					{
						List<int> list25 = list;
						int num = list25[4];
						list25[4] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list26 = list5;
						int num = list26[4];
						list26[4] = num + 1;
					}
					break;
				case WeaponClass.Missile:
					if (ref_weapon.hullWeapon)
					{
						List<int> list27 = list;
						int num = list27[5];
						list27[5] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list28 = list5;
						int num = list28[5];
						list28[5] = num + 1;
					}
					break;
				}
				break;
			case 2:
				switch (moduleDataEntry.moduleTemplate.ref_weapon.weaponClass)
				{
				case WeaponClass.NavalGun:
					if (ref_weapon.hullWeapon)
					{
						List<int> list29 = list2;
						int num = list29[0];
						list29[0] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list30 = list6;
						int num = list30[0];
						list30[0] = num + 1;
					}
					break;
				case WeaponClass.Laser:
					if (ref_weapon.hullWeapon)
					{
						List<int> list31 = list2;
						int num = list31[1];
						list31[1] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list32 = list6;
						int num = list32[1];
						list32[1] = num + 1;
					}
					break;
				case WeaponClass.Particle:
					if (ref_weapon.hullWeapon)
					{
						List<int> list33 = list2;
						int num = list33[2];
						list33[2] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list34 = list6;
						int num = list34[2];
						list34[2] = num + 1;
					}
					break;
				case WeaponClass.Magnetic:
					if (ref_weapon.hullWeapon)
					{
						List<int> list35 = list2;
						int num = list35[3];
						list35[3] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list36 = list6;
						int num = list36[3];
						list36[3] = num + 1;
					}
					break;
				case WeaponClass.Plasma:
					if (ref_weapon.hullWeapon)
					{
						List<int> list37 = list2;
						int num = list37[4];
						list37[4] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list38 = list6;
						int num = list38[4];
						list38[4] = num + 1;
					}
					break;
				case WeaponClass.Missile:
					if (ref_weapon.hullWeapon)
					{
						List<int> list39 = list2;
						int num = list39[5];
						list39[5] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list40 = list6;
						int num = list40[5];
						list40[5] = num + 1;
					}
					break;
				}
				break;
			case 3:
				switch (moduleDataEntry.moduleTemplate.ref_weapon.weaponClass)
				{
				case WeaponClass.NavalGun:
					if (ref_weapon.hullWeapon)
					{
						List<int> list41 = list3;
						int num = list41[0];
						list41[0] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list42 = list7;
						int num = list42[0];
						list42[0] = num + 1;
					}
					break;
				case WeaponClass.Laser:
					if (ref_weapon.hullWeapon)
					{
						List<int> list43 = list3;
						int num = list43[1];
						list43[1] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list44 = list7;
						int num = list44[1];
						list44[1] = num + 1;
					}
					break;
				case WeaponClass.Particle:
					if (ref_weapon.hullWeapon)
					{
						List<int> list45 = list3;
						int num = list45[2];
						list45[2] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list46 = list7;
						int num = list46[2];
						list46[2] = num + 1;
					}
					break;
				case WeaponClass.Magnetic:
					if (ref_weapon.hullWeapon)
					{
						List<int> list47 = list3;
						int num = list47[3];
						list47[3] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list48 = list7;
						int num = list48[3];
						list48[3] = num + 1;
					}
					break;
				case WeaponClass.Plasma:
					if (ref_weapon.hullWeapon)
					{
						List<int> list49 = list3;
						int num = list49[4];
						list49[4] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list50 = list7;
						int num = list50[4];
						list50[4] = num + 1;
					}
					break;
				case WeaponClass.Missile:
					if (ref_weapon.hullWeapon)
					{
						List<int> list51 = list3;
						int num = list51[5];
						list51[5] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list52 = list7;
						int num = list52[5];
						list52[5] = num + 1;
					}
					break;
				}
				break;
			case 4:
				switch (moduleDataEntry.moduleTemplate.ref_weapon.weaponClass)
				{
				case WeaponClass.NavalGun:
					if (ref_weapon.hullWeapon)
					{
						List<int> list53 = list4;
						int num = list53[0];
						list53[0] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list54 = list8;
						int num = list54[0];
						list54[0] = num + 1;
					}
					break;
				case WeaponClass.Laser:
					if (ref_weapon.hullWeapon)
					{
						List<int> list55 = list4;
						int num = list55[1];
						list55[1] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list56 = list8;
						int num = list56[1];
						list56[1] = num + 1;
					}
					break;
				case WeaponClass.Particle:
					if (ref_weapon.hullWeapon)
					{
						List<int> list57 = list4;
						int num = list57[2];
						list57[2] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list58 = list8;
						int num = list58[2];
						list58[2] = num + 1;
					}
					break;
				case WeaponClass.Magnetic:
					if (ref_weapon.hullWeapon)
					{
						List<int> list59 = list4;
						int num = list59[3];
						list59[3] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list60 = list8;
						int num = list60[3];
						list60[3] = num + 1;
					}
					break;
				case WeaponClass.Plasma:
					if (ref_weapon.hullWeapon)
					{
						List<int> list61 = list4;
						int num = list61[4];
						list61[4] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list62 = list8;
						int num = list62[4];
						list62[4] = num + 1;
					}
					break;
				case WeaponClass.Missile:
					if (ref_weapon.hullWeapon)
					{
						List<int> list63 = list4;
						int num = list63[5];
						list63[5] = num + 1;
					}
					if (ref_weapon.noseWeapon)
					{
						List<int> list64 = list8;
						int num = list64[5];
						list64[5] = num + 1;
					}
					break;
				}
				break;
			}
		}
		foreach (ModuleDataEntry moduleDataEntry2 in oldShipTemplate.allWeapons)
		{
			TIShipWeaponTemplate ref_weapon2 = moduleDataEntry2.moduleTemplate.ref_weapon;
			switch (moduleDataEntry2.moduleTemplate.ref_weapon.internalSize)
			{
			case 1:
				switch (moduleDataEntry2.moduleTemplate.ref_weapon.weaponClass)
				{
				case WeaponClass.NavalGun:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list65 = list9;
						int num = list65[0];
						list65[0] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list66 = list13;
						int num = list66[0];
						list66[0] = num + 1;
					}
					break;
				case WeaponClass.Laser:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list67 = list9;
						int num = list67[1];
						list67[1] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list68 = list13;
						int num = list68[1];
						list68[1] = num + 1;
					}
					break;
				case WeaponClass.Particle:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list69 = list9;
						int num = list69[2];
						list69[2] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list70 = list13;
						int num = list70[2];
						list70[2] = num + 1;
					}
					break;
				case WeaponClass.Magnetic:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list71 = list9;
						int num = list71[3];
						list71[3] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list72 = list13;
						int num = list72[3];
						list72[3] = num + 1;
					}
					break;
				case WeaponClass.Plasma:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list73 = list9;
						int num = list73[4];
						list73[4] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list74 = list13;
						int num = list74[4];
						list74[4] = num + 1;
					}
					break;
				case WeaponClass.Missile:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list75 = list9;
						int num = list75[5];
						list75[5] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list76 = list13;
						int num = list76[5];
						list76[5] = num + 1;
					}
					break;
				}
				break;
			case 2:
				switch (moduleDataEntry2.moduleTemplate.ref_weapon.weaponClass)
				{
				case WeaponClass.NavalGun:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list77 = list10;
						int num = list77[0];
						list77[0] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list78 = list14;
						int num = list78[0];
						list78[0] = num + 1;
					}
					break;
				case WeaponClass.Laser:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list79 = list10;
						int num = list79[1];
						list79[1] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list80 = list14;
						int num = list80[1];
						list80[1] = num + 1;
					}
					break;
				case WeaponClass.Particle:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list81 = list10;
						int num = list81[2];
						list81[2] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list82 = list14;
						int num = list82[2];
						list82[2] = num + 1;
					}
					break;
				case WeaponClass.Magnetic:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list83 = list10;
						int num = list83[3];
						list83[3] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list84 = list14;
						int num = list84[3];
						list84[3] = num + 1;
					}
					break;
				case WeaponClass.Plasma:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list85 = list10;
						int num = list85[4];
						list85[4] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list86 = list14;
						int num = list86[4];
						list86[4] = num + 1;
					}
					break;
				case WeaponClass.Missile:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list87 = list10;
						int num = list87[5];
						list87[5] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list88 = list14;
						int num = list88[5];
						list88[5] = num + 1;
					}
					break;
				}
				break;
			case 3:
				switch (moduleDataEntry2.moduleTemplate.ref_weapon.weaponClass)
				{
				case WeaponClass.NavalGun:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list89 = list11;
						int num = list89[0];
						list89[0] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list90 = list15;
						int num = list90[0];
						list90[0] = num + 1;
					}
					break;
				case WeaponClass.Laser:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list91 = list11;
						int num = list91[1];
						list91[1] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list92 = list15;
						int num = list92[1];
						list92[1] = num + 1;
					}
					break;
				case WeaponClass.Particle:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list93 = list11;
						int num = list93[2];
						list93[2] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list94 = list15;
						int num = list94[2];
						list94[2] = num + 1;
					}
					break;
				case WeaponClass.Magnetic:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list95 = list11;
						int num = list95[3];
						list95[3] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list96 = list15;
						int num = list96[3];
						list96[3] = num + 1;
					}
					break;
				case WeaponClass.Plasma:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list97 = list11;
						int num = list97[4];
						list97[4] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list98 = list15;
						int num = list98[4];
						list98[4] = num + 1;
					}
					break;
				case WeaponClass.Missile:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list99 = list11;
						int num = list99[5];
						list99[5] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list100 = list15;
						int num = list100[5];
						list100[5] = num + 1;
					}
					break;
				}
				break;
			case 4:
				switch (moduleDataEntry2.moduleTemplate.ref_weapon.weaponClass)
				{
				case WeaponClass.NavalGun:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list101 = list12;
						int num = list101[0];
						list101[0] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list102 = list16;
						int num = list102[0];
						list102[0] = num + 1;
					}
					break;
				case WeaponClass.Laser:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list103 = list12;
						int num = list103[1];
						list103[1] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list104 = list16;
						int num = list104[1];
						list104[1] = num + 1;
					}
					break;
				case WeaponClass.Particle:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list105 = list12;
						int num = list105[2];
						list105[2] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list106 = list16;
						int num = list106[2];
						list106[2] = num + 1;
					}
					break;
				case WeaponClass.Magnetic:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list107 = list12;
						int num = list107[3];
						list107[3] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list108 = list16;
						int num = list108[3];
						list108[3] = num + 1;
					}
					break;
				case WeaponClass.Plasma:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list109 = list12;
						int num = list109[4];
						list109[4] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list110 = list16;
						int num = list110[4];
						list110[4] = num + 1;
					}
					break;
				case WeaponClass.Missile:
					if (ref_weapon2.hullWeapon)
					{
						List<int> list111 = list12;
						int num = list111[5];
						list111[5] = num + 1;
					}
					if (ref_weapon2.noseWeapon)
					{
						List<int> list112 = list16;
						int num = list112[5];
						list112[5] = num + 1;
					}
					break;
				}
				break;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (list[i] != list9[i])
			{
				return false;
			}
			if (list2[i] != list10[i])
			{
				return false;
			}
			if (list3[i] != list11[i])
			{
				return false;
			}
			if (list4[i] != list12[i])
			{
				return false;
			}
			if (list5[i] != list13[i])
			{
				return false;
			}
			if (list6[i] != list14[i])
			{
				return false;
			}
			if (list7[i] != list15[i])
			{
				return false;
			}
			if (list8[i] != list16[i])
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x000633A0 File Offset: 0x000615A0
	public static string GetRefitSuffix(int iteration)
	{
		return new StringBuilder(" ").Append(Loc.T("UI.Fleets.RefitIterationSuffix")).Append(" ").Append(iteration)
			.ToString();
	}

	// Token: 0x170002E6 RID: 742
	// (get) Token: 0x06001467 RID: 5223 RVA: 0x000633D0 File Offset: 0x000615D0
	public float drivePowerRequirement_GW
	{
		get
		{
			TIDriveTemplate driveTemplate = this.driveTemplate;
			if (driveTemplate == null)
			{
				return 0f;
			}
			return driveTemplate.powerRequirement_GW;
		}
	}

	// Token: 0x170002E7 RID: 743
	// (get) Token: 0x06001468 RID: 5224 RVA: 0x000633E8 File Offset: 0x000615E8
	public float shipPowerProductionRequirement_GW
	{
		get
		{
			float drivePowerRequirement_GW = this.drivePowerRequirement_GW;
			float num = this.requiredSystemsPower_GW;
			TIPowerPlantTemplate powerPlantTemplate = this.powerPlantTemplate;
			float num2 = drivePowerRequirement_GW + ((num / ((powerPlantTemplate != null) ? new float?(powerPlantTemplate.efficiency) : null)) ?? 1f);
			num = this.requiredWeaponsPowerGeneration_GW;
			TIPowerPlantTemplate powerPlantTemplate2 = this.powerPlantTemplate;
			return num2 + ((num / ((powerPlantTemplate2 != null) ? new float?(powerPlantTemplate2.efficiency) : null)) ?? 1f);
		}
	}

	// Token: 0x170002E8 RID: 744
	// (get) Token: 0x06001469 RID: 5225 RVA: 0x000634C0 File Offset: 0x000616C0
	public float requiredSystemsPower_GW
	{
		get
		{
			float num = (float)this.crewBillets * 5E-06f;
			float num2 = num;
			TIShipHullTemplate hullTemplate = this.hullTemplate;
			num = num2 + (float)((hullTemplate != null) ? hullTemplate.consTier : 0) * 0.005f;
			foreach (TIShipModuleTemplate tishipModuleTemplate in this.utilitySlotModuleTemplates)
			{
				float num3 = num;
				TIUtilityModuleTemplate ref_utilityModule = tishipModuleTemplate.ref_utilityModule;
				num = num3 + ((ref_utilityModule != null) ? (ref_utilityModule.powerRequirement_MW / 1000f) : 0f);
			}
			num *= 1.1f;
			return num;
		}
	}

	// Token: 0x170002E9 RID: 745
	// (get) Token: 0x0600146A RID: 5226 RVA: 0x00063558 File Offset: 0x00061758
	public float requiredWeaponsPowerGeneration_GW
	{
		get
		{
			float num = 0f;
			foreach (TIShipWeaponTemplate tishipWeaponTemplate in this.allWeaponTemplates)
			{
				if (!tishipWeaponTemplate.selfPowered)
				{
					if (tishipWeaponTemplate.salvo_shots == 1)
					{
						num += tishipWeaponTemplate.EnergyUsage_GJ(this.GetBonusPowerForWeapon_GJ(tishipWeaponTemplate, null)) / tishipWeaponTemplate.cooldown_s;
					}
					else
					{
						num += tishipWeaponTemplate.EnergyUsage_GJ(this.GetBonusPowerForWeapon_GJ(tishipWeaponTemplate, null)) / tishipWeaponTemplate.intraSalvoCooldown_s;
					}
				}
			}
			return num;
		}
	}

	// Token: 0x170002EA RID: 746
	// (get) Token: 0x0600146B RID: 5227 RVA: 0x000635F0 File Offset: 0x000617F0
	public float requiredWeaponsPowerStorage_GJ
	{
		get
		{
			float num = 0f;
			foreach (TIShipWeaponTemplate tishipWeaponTemplate in this.allWeaponTemplates)
			{
				if (!tishipWeaponTemplate.selfPowered)
				{
					num += tishipWeaponTemplate.EnergyUsage_GJ(this.GetBonusPowerForWeapon_GJ(tishipWeaponTemplate, null));
				}
			}
			return num;
		}
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x0006365C File Offset: 0x0006185C
	public float GetLaserBonusPower_MJ(Func<ModuleDataEntry, float> GetPartFunction = null)
	{
		if (GetPartFunction == null)
		{
			GetPartFunction = (ModuleDataEntry x) => 1f;
		}
		float num = 0f;
		foreach (ModuleDataEntry moduleDataEntry in this.utilityModules)
		{
			TIUtilityModuleTemplate ref_utilityModule = moduleDataEntry.moduleTemplate.ref_utilityModule;
			if (ref_utilityModule != null)
			{
				float laserPowerBonus_MW = ref_utilityModule.laserPowerBonus_MW;
				if (laserPowerBonus_MW > 0f)
				{
					num += laserPowerBonus_MW * GetPartFunction(moduleDataEntry);
				}
			}
		}
		return num;
	}

	// Token: 0x0600146D RID: 5229 RVA: 0x000636FC File Offset: 0x000618FC
	public float GetParticleBonusPower_MJ(Func<ModuleDataEntry, float> GetPartFunction = null)
	{
		if (GetPartFunction == null)
		{
			GetPartFunction = (ModuleDataEntry x) => 1f;
		}
		float num = 0f;
		foreach (ModuleDataEntry moduleDataEntry in this.utilityModules)
		{
			TIUtilityModuleTemplate ref_utilityModule = moduleDataEntry.moduleTemplate.ref_utilityModule;
			if (ref_utilityModule != null)
			{
				float particleBeamPowerBonus_MW = ref_utilityModule.particleBeamPowerBonus_MW;
				if (particleBeamPowerBonus_MW > 0f)
				{
					num += particleBeamPowerBonus_MW * GetPartFunction(moduleDataEntry);
				}
			}
		}
		return num;
	}

	// Token: 0x0600146E RID: 5230 RVA: 0x0006379C File Offset: 0x0006199C
	public float GetBonusPowerForWeapon_MJ(TIShipWeaponTemplate weapon, Func<ModuleDataEntry, float> GetPartFunction = null)
	{
		if (weapon.isLaserWeapon)
		{
			return this.GetLaserBonusPower_MJ(GetPartFunction) * (weapon.attackMode ? 1f : 0.5f);
		}
		if (weapon.isParticleWeapon)
		{
			return this.GetParticleBonusPower_MJ(GetPartFunction) * (weapon.attackMode ? 1f : 0.5f);
		}
		return 0f;
	}

	// Token: 0x0600146F RID: 5231 RVA: 0x000637F8 File Offset: 0x000619F8
	public float GetBonusPowerForWeapon_GJ(TIShipWeaponTemplate weapon, Func<ModuleDataEntry, float> GetPartFunction = null)
	{
		return this.GetBonusPowerForWeapon_MJ(weapon, GetPartFunction) / 1000f;
	}

	// Token: 0x06001470 RID: 5232 RVA: 0x00063808 File Offset: 0x00061A08
	public float GetBonusPowerForWeapon_Multiplier(TIShipWeaponTemplate weapon, float range_km, Func<ModuleDataEntry, float> GetPartFunction = null)
	{
		float num = weapon.BaseDamageAtRange_MJ(range_km, true);
		if (num == 0f)
		{
			return 1f;
		}
		float bonusPowerForWeapon_MJ = this.GetBonusPowerForWeapon_MJ(weapon, GetPartFunction);
		return 1f + bonusPowerForWeapon_MJ / num;
	}

	// Token: 0x170002EB RID: 747
	// (get) Token: 0x06001471 RID: 5233 RVA: 0x0006383E File Offset: 0x00061A3E
	public float wasteHeat_GW
	{
		get
		{
			TIPowerPlantTemplate powerPlantTemplate = this.powerPlantTemplate;
			if (powerPlantTemplate == null)
			{
				return 0f;
			}
			TIDriveTemplate driveTemplate = this.driveTemplate;
			return powerPlantTemplate.WasteHeat_GW(driveTemplate != null && driveTemplate.openCycleCooling, this.drivePowerRequirement_GW, this.requiredSystemsPower_GW + this.requiredWeaponsPowerGeneration_GW);
		}
	}

	// Token: 0x170002EC RID: 748
	// (get) Token: 0x06001472 RID: 5234 RVA: 0x0006387C File Offset: 0x00061A7C
	public float modifiedCapSurfaceArea_m2
	{
		get
		{
			return 3.1415927f * ((this.hullTemplate.capSurfaceArea_m2 + this.lateralArmorThickness_m * 2f) / 2f) * ((this.hullTemplate.capSurfaceArea_m2 + this.lateralArmorThickness_m * 2f) / 2f);
		}
	}

	// Token: 0x170002ED RID: 749
	// (get) Token: 0x06001473 RID: 5235 RVA: 0x000638CC File Offset: 0x00061ACC
	public int magazineModuleCount
	{
		get
		{
			return this.utilityModules.Count<ModuleDataEntry>(delegate(ModuleDataEntry x)
			{
				TIUtilityModuleTemplate ref_utilityModule = x.moduleTemplate.ref_utilityModule;
				return ref_utilityModule != null && ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Magazine);
			});
		}
	}

	// Token: 0x170002EE RID: 750
	// (get) Token: 0x06001474 RID: 5236 RVA: 0x000638F8 File Offset: 0x00061AF8
	public float magazineModuleMultiplier
	{
		get
		{
			return this.utilityModules.Where<ModuleDataEntry>((ModuleDataEntry x) => x.moduleTemplate.ref_utilityModule != null && x.moduleTemplate.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Magazine)).Sum<ModuleDataEntry>((ModuleDataEntry x) => x.moduleTemplate.ref_utilityModule.specialModuleValue);
		}
	}

	// Token: 0x170002EF RID: 751
	// (get) Token: 0x06001475 RID: 5237 RVA: 0x00063953 File Offset: 0x00061B53
	public float powerPlantMass_tons
	{
		get
		{
			TIPowerPlantTemplate powerPlantTemplate = this.powerPlantTemplate;
			if (powerPlantTemplate == null)
			{
				return 0f;
			}
			return powerPlantTemplate.buildMass_tons(this.shipPowerProductionRequirement_GW, 0f, 0f, 0f, false);
		}
	}

	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x06001476 RID: 5238 RVA: 0x00063980 File Offset: 0x00061B80
	public float radiatorMass_tons
	{
		get
		{
			if (this.radiatorTemplate == null || this.powerPlantTemplate == null || this.driveTemplate == null)
			{
				return 0f;
			}
			return this.radiatorTemplate.buildMass_tons(this.wasteHeat_GW, 0f, 0f, 0f, false);
		}
	}

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x06001477 RID: 5239 RVA: 0x000639CC File Offset: 0x00061BCC
	public float noseArmorMass_tons
	{
		get
		{
			float num = 0f;
			TIShipArmorTemplate noseArmorTemplate = this.noseArmorTemplate;
			return Mathf.Max(num, (noseArmorTemplate != null) ? noseArmorTemplate.buildMass_tons(this.lateralArmorThickness_m, (float)this.noseArmorValue, this.hullTemplate.length_m, this.hullTemplate.width_m, false) : 0f);
		}
	}

	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x06001478 RID: 5240 RVA: 0x00063A20 File Offset: 0x00061C20
	public float lateralArmorMass_tons
	{
		get
		{
			float num = 0f;
			TIShipArmorTemplate lateralArmorTemplate = this.lateralArmorTemplate;
			return Mathf.Max(num, (lateralArmorTemplate != null) ? lateralArmorTemplate.buildMass_tons(this.lateralArmorThickness_m, (float)this.lateralArmorValue, this.hullTemplate.length_m, this.hullTemplate.width_m, true) : 0f);
		}
	}

	// Token: 0x170002F3 RID: 755
	// (get) Token: 0x06001479 RID: 5241 RVA: 0x00063A74 File Offset: 0x00061C74
	public float tailArmorMass_tons
	{
		get
		{
			float num = 0f;
			TIShipArmorTemplate tailArmorTemplate = this.tailArmorTemplate;
			return Mathf.Max(num, (tailArmorTemplate != null) ? tailArmorTemplate.buildMass_tons(this.lateralArmorThickness_m, (float)this.tailArmorValue, this.hullTemplate.length_m, this.hullTemplate.width_m, false) : 0f);
		}
	}

	// Token: 0x170002F4 RID: 756
	// (get) Token: 0x0600147A RID: 5242 RVA: 0x00063AC5 File Offset: 0x00061CC5
	public float totalArmorMass_tons
	{
		get
		{
			return this.noseArmorMass_tons + this.lateralArmorMass_tons + this.tailArmorMass_tons;
		}
	}

	// Token: 0x170002F5 RID: 757
	// (get) Token: 0x0600147B RID: 5243 RVA: 0x00063ADB File Offset: 0x00061CDB
	public float propellantMass_tons
	{
		get
		{
			return (float)this.propellantTanks * 100f;
		}
	}

	// Token: 0x170002F6 RID: 758
	// (get) Token: 0x0600147C RID: 5244 RVA: 0x00063AEA File Offset: 0x00061CEA
	public float dryMass_kg
	{
		get
		{
			return this.dryMass_tons(false) * 1000f;
		}
	}

	// Token: 0x170002F7 RID: 759
	// (get) Token: 0x0600147D RID: 5245 RVA: 0x00063AF9 File Offset: 0x00061CF9
	public float propellantMass_kg
	{
		get
		{
			return this.propellantMass_tons * 1000f;
		}
	}

	// Token: 0x170002F8 RID: 760
	// (get) Token: 0x0600147E RID: 5246 RVA: 0x00063B07 File Offset: 0x00061D07
	public float wetMass_tons
	{
		get
		{
			return this.dryMass_tons(false) + this.propellantMass_tons;
		}
	}

	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x0600147F RID: 5247 RVA: 0x00063B17 File Offset: 0x00061D17
	public float wetMass_kg
	{
		get
		{
			return this.wetMass_tons * 1000f;
		}
	}

	// Token: 0x170002FA RID: 762
	// (get) Token: 0x06001480 RID: 5248 RVA: 0x00063B25 File Offset: 0x00061D25
	public float allBatteriesMass_tons
	{
		get
		{
			return this.utilityModules.Sum<ModuleDataEntry>(delegate(ModuleDataEntry x)
			{
				if (!x.moduleTemplate.isBattery)
				{
					return 0f;
				}
				return x.moduleTemplate.buildMass_tons(0f, 0f, 0f, 0f, false);
			});
		}
	}

	// Token: 0x170002FB RID: 763
	// (get) Token: 0x06001481 RID: 5249 RVA: 0x00063B51 File Offset: 0x00061D51
	public float crewMass_tons
	{
		get
		{
			return 4f * (float)this.crewBillets;
		}
	}

	// Token: 0x170002FC RID: 764
	// (get) Token: 0x06001482 RID: 5250 RVA: 0x00063B60 File Offset: 0x00061D60
	public float weaponsMass_tons
	{
		get
		{
			return this.allWeaponTemplates.Sum<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.buildMass_tons(this.magazineModuleMultiplier, 0f, 0f, 0f, false));
		}
	}

	// Token: 0x06001483 RID: 5251 RVA: 0x00063B7C File Offset: 0x00061D7C
	public float dryMass_tons(bool forceUpdate = false)
	{
		if (this.cachedDryMass_tons <= 0f || forceUpdate)
		{
			this.cachedDryMass_tons = 0f;
			this.cachedDryMass_tons += this.hullTemplate.buildMass_tons(0f, 0f, 0f, 0f, false);
			float num = this.cachedDryMass_tons;
			TIDriveTemplate driveTemplate = this.driveTemplate;
			this.cachedDryMass_tons = num + ((driveTemplate != null) ? driveTemplate.buildMass_tons(0f, 0f, 0f, 0f, false) : 0f);
			this.cachedDryMass_tons += this.powerPlantMass_tons;
			this.cachedDryMass_tons += this.radiatorMass_tons;
			float magazineModuleMultiplier = this.magazineModuleMultiplier;
			foreach (TIShipWeaponTemplate tishipWeaponTemplate in this.allWeaponTemplates)
			{
				this.cachedDryMass_tons += tishipWeaponTemplate.buildMass_tons(magazineModuleMultiplier, 0f, 0f, 0f, false);
			}
			foreach (TIShipModuleTemplate tishipModuleTemplate in this.utilitySlotModuleTemplates)
			{
				this.cachedDryMass_tons += tishipModuleTemplate.buildMass_tons(0f, 0f, 0f, 0f, false);
			}
			this.cachedDryMass_tons += this.noseArmorMass_tons;
			this.cachedDryMass_tons += this.lateralArmorMass_tons;
			this.cachedDryMass_tons += this.tailArmorMass_tons;
			this.cachedDryMass_tons += this.crewMass_tons;
		}
		return this.cachedDryMass_tons;
	}

	// Token: 0x06001484 RID: 5252 RVA: 0x00063D54 File Offset: 0x00061F54
	public void SetDryMass_tons(float dryMass_tons)
	{
		this.cachedDryMass_tons = dryMass_tons;
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x00063D5D File Offset: 0x00061F5D
	public void SetBaseCruiseDeltaV_kps(float baseCruiseDeltaV_kps)
	{
		this._baseCruiseDeltaV_kps = baseCruiseDeltaV_kps;
	}

	// Token: 0x170002FD RID: 765
	// (get) Token: 0x06001486 RID: 5254 RVA: 0x00063D66 File Offset: 0x00061F66
	public TIResourcesCost powerPlantBuildCost
	{
		get
		{
			if (this.powerPlantTemplate == null)
			{
				return new TIResourcesCost();
			}
			return this.powerPlantTemplate.buildCost(this.shipPowerProductionRequirement_GW, 0f);
		}
	}

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x06001487 RID: 5255 RVA: 0x00063D8C File Offset: 0x00061F8C
	public TIResourcesCost radiatorsBuildCost
	{
		get
		{
			if (this.radiatorTemplate == null || this.powerPlantTemplate == null)
			{
				return new TIResourcesCost();
			}
			return this.radiatorTemplate.buildCost(this.wasteHeat_GW, 0f);
		}
	}

	// Token: 0x06001488 RID: 5256 RVA: 0x00063DBC File Offset: 0x00061FBC
	public TIResourcesCost singlePropellantTankCost(TIFactionState faction, float tankFillFraction = 1f)
	{
		TIDriveTemplate driveTemplate = this.driveTemplate;
		if (driveTemplate == null)
		{
			return null;
		}
		return driveTemplate.GetPerTankPropellantMaterials(faction).ToResourcesCost(100f * TemplateManager.global.spaceResourceToTons * tankFillFraction);
	}

	// Token: 0x06001489 RID: 5257 RVA: 0x00063DF8 File Offset: 0x00061FF8
	public TIResourcesCost propellantTanksBuildCost(TIFactionState faction)
	{
		if (this.driveTemplate == null)
		{
			return new TIResourcesCost();
		}
		return this.driveTemplate.GetPerTankPropellantMaterials(faction).ToResourcesCost((float)this.propellantTanks * 100f * TemplateManager.global.spaceResourceToTons);
	}

	// Token: 0x170002FF RID: 767
	// (get) Token: 0x0600148A RID: 5258 RVA: 0x00063E3F File Offset: 0x0006203F
	public TIResourcesCost noseArmorBuildCost
	{
		get
		{
			if (this.noseArmor.materialTemplate == null)
			{
				return new TIResourcesCost();
			}
			return this.noseArmor.materialTemplate.buildCost(this.noseArmorMass_tons, 0f);
		}
	}

	// Token: 0x17000300 RID: 768
	// (get) Token: 0x0600148B RID: 5259 RVA: 0x00063E6F File Offset: 0x0006206F
	public TIResourcesCost lateralArmorBuildCost
	{
		get
		{
			if (this.lateralArmor.materialTemplate == null)
			{
				return new TIResourcesCost();
			}
			return this.lateralArmor.materialTemplate.buildCost(this.lateralArmorMass_tons, 0f);
		}
	}

	// Token: 0x17000301 RID: 769
	// (get) Token: 0x0600148C RID: 5260 RVA: 0x00063E9F File Offset: 0x0006209F
	public TIResourcesCost tailArmorBuildCost
	{
		get
		{
			if (this.tailArmor.materialTemplate == null)
			{
				return new TIResourcesCost();
			}
			return this.tailArmor.materialTemplate.buildCost(this.tailArmorMass_tons, 0f);
		}
	}

	// Token: 0x0600148D RID: 5261 RVA: 0x00063ECF File Offset: 0x000620CF
	public float propellantTanksBuildCost(TIFactionState faction, FactionResource resource)
	{
		return this.propellantTanksBuildCost(faction).GetSingleCostValue(resource);
	}

	// Token: 0x0600148E RID: 5262 RVA: 0x00063EE0 File Offset: 0x000620E0
	public TIResourcesCost spaceResourceConstructionCost(bool forceUpdateToCache, TIHabModuleState shipyard, bool includePropellant = true, bool skipConstructionTime = false, bool updateWithoutCaching = false)
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		if (this._spaceResourceConstructionCost == null || forceUpdateToCache || updateWithoutCaching)
		{
			tiresourcesCost.SumCosts_NoDuration(this.hullTemplate.buildCost(0f, 0f));
			if (this.driveTemplate != null)
			{
				tiresourcesCost.SumCosts_NoDuration(this.driveTemplate.buildCost(0f, 0f));
			}
			if (this.powerPlantTemplate != null && this.driveTemplate != null)
			{
				tiresourcesCost.SumCosts_NoDuration(this.powerPlantBuildCost);
			}
			if (this.radiatorTemplate != null && this.powerPlantTemplate != null && this.driveTemplate != null)
			{
				tiresourcesCost.SumCosts_NoDuration(this.radiatorsBuildCost);
			}
			foreach (TIShipWeaponTemplate tishipWeaponTemplate in this.allWeaponTemplates)
			{
				tiresourcesCost.SumCosts_NoDuration(tishipWeaponTemplate.buildCost(this.magazineModuleMultiplier, 0f));
			}
			foreach (TIShipModuleTemplate tishipModuleTemplate in this.utilitySlotModuleTemplates)
			{
				tiresourcesCost.SumCosts_NoDuration(tishipModuleTemplate.buildCost(0f, 0f));
			}
			if (this.noseArmor.materialTemplate != null)
			{
				tiresourcesCost.SumCosts_NoDuration(this.noseArmorBuildCost);
			}
			if (this.lateralArmor.materialTemplate != null)
			{
				tiresourcesCost.SumCosts_NoDuration(this.lateralArmorBuildCost);
			}
			if (this.tailArmor.materialTemplate != null)
			{
				tiresourcesCost.SumCosts_NoDuration(this.tailArmorBuildCost);
			}
			tiresourcesCost.SumCosts_NoDuration(new ResourceCostBuilder
			{
				water = TemplateManager.global.crewBaselineWater_tons,
				volatiles = TemplateManager.global.crewBaselineVolatiles_tons
			}.ToResourcesCost((float)this.crewBillets * TemplateManager.global.spaceResourceToTons));
			if (shipyard != null)
			{
				tiresourcesCost = tiresourcesCost.MultiplyCost(TemplateManager.global.GetAIShipbuildingCostDifficultyScaling(this.designingFaction));
			}
			if (this.powerPlantTemplate != null && this.driveTemplate != null && includePropellant)
			{
				tiresourcesCost.SumCosts_NoDuration(this.propellantTanksBuildCost(((shipyard != null) ? shipyard.ref_faction : null) ?? this.designingFaction));
			}
			if (forceUpdateToCache)
			{
				this._spaceResourceConstructionCost = new TIResourcesCost(tiresourcesCost);
				this._requiredExotics = -1f;
				this._requiredAntimatter = -1f;
			}
		}
		else
		{
			tiresourcesCost = new TIResourcesCost(this._spaceResourceConstructionCost);
		}
		if (!skipConstructionTime)
		{
			tiresourcesCost.SetCompletionTime_Days((shipyard != null) ? this.hullTemplate.constructionTime_Days(shipyard) : this.hullTemplate.noShipyardConstructionTime_Days(this.designingFaction));
		}
		return tiresourcesCost;
	}

	// Token: 0x0600148F RID: 5263 RVA: 0x00064184 File Offset: 0x00062384
	public TIResourcesCost earthResourceConstructionCost(TIFactionState faction, TIHabModuleState shipyard)
	{
		TIResourcesCost tiresourcesCost = this.spaceResourceConstructionCost(false, shipyard, true, false, false);
		TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
		float num = 0f;
		float num2 = 0f;
		foreach (FactionResource factionResource in TIResourcesCost.replaceableSpaceResources)
		{
			num += tiresourcesCost.GetSingleCostValue(factionResource) * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(factionResource);
			num2 += tiresourcesCost.GetSingleCostValue(factionResource);
		}
		tiresourcesCost2.AddCost(FactionResource.Money, num, true);
		tiresourcesCost2.AddCost(FactionResource.Boost, (float)TISpaceObjectState.GenericTransferBoostFromEarthSurface(faction, shipyard.hab.IsBase ? shipyard.ref_spaceBody : shipyard.ref_orbit, num2 / TemplateManager.global.spaceResourceToTons), true);
		foreach (FactionResource factionResource2 in TIResourcesCost.irreplaceableSpaceResources)
		{
			tiresourcesCost2.AddCost(factionResource2, tiresourcesCost.GetSingleCostValue(factionResource2), true);
		}
		tiresourcesCost2.SetCompletionTime_Days((shipyard == null) ? this.hullTemplate.noShipyardConstructionTime_Days(faction) : (this.hullTemplate.constructionTime_Days(shipyard) + TISpaceObjectState.GenericTransferTime_d(shipyard.ref_faction, GameStateManager.Earth(), shipyard)));
		return tiresourcesCost2;
	}

	// Token: 0x06001490 RID: 5264 RVA: 0x000642DC File Offset: 0x000624DC
	public static TIResourcesCost MixedResourceConstructionCost(TIFactionState faction, TIHabState hab, TIResourcesCost baseCost, List<ResourceValue> availableSpaceResources = null, bool ignoreTime = false)
	{
		return baseCost.GetBoostSubstitutedCost(faction, hab, ignoreTime, availableSpaceResources);
	}

	// Token: 0x17000302 RID: 770
	// (get) Token: 0x06001491 RID: 5265 RVA: 0x000642EC File Offset: 0x000624EC
	public float modifiedThrust_N
	{
		get
		{
			float num = 1f;
			foreach (TIShipModuleTemplate tishipModuleTemplate in this.utilitySlotModuleTemplates)
			{
				TIUtilityModuleTemplate ref_utilityModule = tishipModuleTemplate.ref_utilityModule;
				if (ref_utilityModule != null && ref_utilityModule.thrustMultiplier != 0f)
				{
					num *= ref_utilityModule.thrustMultiplier;
				}
			}
			TIDriveTemplate driveTemplate = this.driveTemplate;
			if (driveTemplate == null)
			{
				return 0f;
			}
			return driveTemplate.thrust_N * num;
		}
	}

	// Token: 0x17000303 RID: 771
	// (get) Token: 0x06001492 RID: 5266 RVA: 0x00064370 File Offset: 0x00062570
	public float modifiedEV_kps
	{
		get
		{
			float num = 1f;
			foreach (TIShipModuleTemplate tishipModuleTemplate in this.utilitySlotModuleTemplates)
			{
				TIUtilityModuleTemplate ref_utilityModule = tishipModuleTemplate.ref_utilityModule;
				if (ref_utilityModule != null && ref_utilityModule.EVMultiplier != 0f)
				{
					num *= ref_utilityModule.EVMultiplier;
				}
			}
			TIDriveTemplate driveTemplate = this.driveTemplate;
			if (driveTemplate == null)
			{
				return 0f;
			}
			return driveTemplate.EV_kps * num;
		}
	}

	// Token: 0x06001493 RID: 5267 RVA: 0x000643F4 File Offset: 0x000625F4
	public float baseCruiseAcceleration_gs(bool forceUpdate)
	{
		return this.baseCruiseAcceleration_mps2(forceUpdate) / 9.80665f;
	}

	// Token: 0x06001494 RID: 5268 RVA: 0x00064403 File Offset: 0x00062603
	public float baseCruiseDeltaV_mps(bool forceUpdate)
	{
		return this.baseCruiseDeltaV_kps(forceUpdate) * 1000f;
	}

	// Token: 0x17000304 RID: 772
	// (get) Token: 0x06001495 RID: 5269 RVA: 0x00064414 File Offset: 0x00062614
	public float baseCombatThrust_N
	{
		get
		{
			float modifiedThrust_N = this.modifiedThrust_N;
			TIDriveTemplate driveTemplate = this.driveTemplate;
			return (modifiedThrust_N * ((driveTemplate != null) ? new float?(driveTemplate.thrustCap) : null)).GetValueOrDefault();
		}
	}

	// Token: 0x17000305 RID: 773
	// (get) Token: 0x06001496 RID: 5270 RVA: 0x00064472 File Offset: 0x00062672
	public float baseCombatExhaustVelocity_kps
	{
		get
		{
			if (this.driveTemplate != null)
			{
				return this.driveTemplate.EV_kps / this.driveTemplate.thrustCap;
			}
			return 0f;
		}
	}

	// Token: 0x17000306 RID: 774
	// (get) Token: 0x06001497 RID: 5271 RVA: 0x00064499 File Offset: 0x00062699
	public float baseCombatAcceleration_gs
	{
		get
		{
			return this.baseCombatAcceleration_mps2 / 9.80665f;
		}
	}

	// Token: 0x06001498 RID: 5272 RVA: 0x000644A8 File Offset: 0x000626A8
	public float HeatCapacity_GJ(bool forceUpdate = false)
	{
		if (forceUpdate || this._heatCapacity_GJ < 0f)
		{
			this._heatCapacity_GJ = 0f;
			foreach (TIShipModuleTemplate tishipModuleTemplate in this.utilitySlotModuleTemplates)
			{
				TIHeatSinkTemplate ref_heatSink = tishipModuleTemplate.ref_heatSink;
				if (ref_heatSink != null && ref_heatSink.heatCapacity_GJ > 0f)
				{
					this._heatCapacity_GJ += ref_heatSink.heatCapacity_GJ;
				}
			}
		}
		return this._heatCapacity_GJ;
	}

	// Token: 0x06001499 RID: 5273 RVA: 0x00064538 File Offset: 0x00062738
	public float BatteryCapacity_GJ(bool forceUpdate = false)
	{
		if (forceUpdate || this._batteryCapacity_GJ < 0f)
		{
			this._batteryCapacity_GJ = 0f;
			foreach (TIShipModuleTemplate tishipModuleTemplate in this.utilitySlotModuleTemplates)
			{
				TIBatteryTemplate ref_battery = tishipModuleTemplate.ref_battery;
				if (ref_battery != null)
				{
					this._batteryCapacity_GJ += ref_battery.GetCapacity(false);
				}
			}
		}
		return this._batteryCapacity_GJ;
	}

	// Token: 0x17000307 RID: 775
	// (get) Token: 0x0600149A RID: 5274 RVA: 0x000645BC File Offset: 0x000627BC
	public float baseManueverThrust
	{
		get
		{
			if (!this.isAlien)
			{
				return 2500000f + this.utilityModules.Sum<ModuleDataEntry>(delegate(ModuleDataEntry x)
				{
					TIUtilityModuleTemplate ref_utilityModule = x.moduleTemplate.ref_utilityModule;
					if (ref_utilityModule == null)
					{
						return 0f;
					}
					return ref_utilityModule.vectorThrustBonus;
				});
			}
			return 4000000f;
		}
	}

	// Token: 0x17000308 RID: 776
	// (get) Token: 0x0600149B RID: 5275 RVA: 0x000645FC File Offset: 0x000627FC
	public float baseAngularAcceleration_rads2
	{
		get
		{
			float num = 0.083333336f * this.wetMass_kg * Mathf.Pow(this.hullTemplate.length_m, 2f);
			return this.baseManueverThrust * 2f * this.hullTemplate.length_m / 2f / num;
		}
	}

	// Token: 0x17000309 RID: 777
	// (get) Token: 0x0600149C RID: 5276 RVA: 0x0006464C File Offset: 0x0006284C
	public float baseAngularAcceleration_degs2
	{
		get
		{
			return this.baseAngularAcceleration_rads2 * 57.29578f;
		}
	}

	// Token: 0x1700030A RID: 778
	// (get) Token: 0x0600149D RID: 5277 RVA: 0x0006465A File Offset: 0x0006285A
	public float maxAngularVelocity_mps
	{
		get
		{
			return Mathf.Sqrt((this.isAlien ? TemplateManager.global.maxAlienCombatAcceleration_g : TemplateManager.global.baselineMaxHumanCombatAcceleration_g) * 9.80665f * 0.5f * this.hullTemplate.length_m);
		}
	}

	// Token: 0x1700030B RID: 779
	// (get) Token: 0x0600149E RID: 5278 RVA: 0x00064697 File Offset: 0x00062897
	public float maxAngularVelocity_degs
	{
		get
		{
			return 57.29578f * (this.maxAngularVelocity_mps / this.hullTemplate.length_m * 0.5f);
		}
	}

	// Token: 0x1700030C RID: 780
	// (get) Token: 0x0600149F RID: 5279 RVA: 0x000646B7 File Offset: 0x000628B7
	public float maxDamageControlAngularVelocity_mps
	{
		get
		{
			return Mathf.Sqrt(1.2258313f * this.hullTemplate.length_m);
		}
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x000646CF File Offset: 0x000628CF
	public float GetCrossSectionalArea_m2(float angle_degrees = -3.4028235E+38f)
	{
		return 3.1415927f * this.hullTemplate.width_m * this.hullTemplate.length_m * 0.5f;
	}

	// Token: 0x1700030D RID: 781
	// (get) Token: 0x060014A1 RID: 5281 RVA: 0x000646FC File Offset: 0x000628FC
	public bool ValidTemplate
	{
		get
		{
			return this.hullTemplate != null && this.driveTemplate != null && this.radiatorTemplate != null && this.powerPlantTemplate != null && this.propellantTanks > 0 && this.noseArmorTemplate != null && this.lateralArmorTemplate != null && this.tailArmorTemplate != null && this.role != ShipRole.NoRole && this.AllowedRole(this.role);
		}
	}

	// Token: 0x1700030E RID: 782
	// (get) Token: 0x060014A2 RID: 5282 RVA: 0x00064760 File Offset: 0x00062960
	public string fullClassName
	{
		get
		{
			if (!this.hullTemplate.noShipyardBuild)
			{
				return Loc.T("UI.Fleets.FullClassName", new object[]
				{
					this.displayName,
					this.hullTemplate.displayNameCurrentForStartScreen()
				});
			}
			return Loc.T("UI.Precombat.SquadronName2", new object[] { this.displayName });
		}
	}

	// Token: 0x1700030F RID: 783
	// (get) Token: 0x060014A3 RID: 5283 RVA: 0x000647BB File Offset: 0x000629BB
	public string className
	{
		get
		{
			if (!this.hullTemplate.noShipyardBuild)
			{
				return Loc.T("UI.Fleets.ClassName", new object[] { this.displayName });
			}
			return this.displayName;
		}
	}

	// Token: 0x17000310 RID: 784
	// (get) Token: 0x060014A4 RID: 5284 RVA: 0x000647EC File Offset: 0x000629EC
	public static List<string> illegalShipClassNames
	{
		get
		{
			List<string> list = new List<string>();
			foreach (TISpaceShipTemplate tispaceShipTemplate in TemplateManager.IterateByClass<TISpaceShipTemplate>(true))
			{
				list.Add(tispaceShipTemplate.displayName);
			}
			foreach (TISpaceShipState tispaceShipState in GameStateManager.IterateByClass<TISpaceShipState>(false))
			{
				if (!tispaceShipState.archived)
				{
					list.Add(tispaceShipState.displayName);
				}
			}
			return list;
		}
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x00064894 File Offset: 0x00062A94
	public static void ClearUnusedTemplates()
	{
		IEnumerable<TISpaceShipTemplate> enumerable = TemplateManager.IterateByClass<TISpaceShipTemplate>(true);
		GameStateManager.IterateByClass<TISpaceShipState>(false);
		foreach (TISpaceShipTemplate tispaceShipTemplate in enumerable.Where<TISpaceShipTemplate>((TISpaceShipTemplate shipTemplate) => GameStateManager.AllFactions().None<TIFactionState>((TIFactionState x) => x.shipDesigns.Contains(shipTemplate)) && shipTemplate.CanDeleteDesign))
		{
			TemplateManager.Remove<TISpaceShipTemplate>(tispaceShipTemplate);
		}
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x0006490C File Offset: 0x00062B0C
	public string GenerateRandomClassName(TIFactionTemplate faction)
	{
		List<string> illegalShipClassNames = TISpaceShipTemplate.illegalShipClassNames;
		string empty = string.Empty;
		bool flag = true;
		int num = 0;
		TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>((faction != null) ? faction.dataName : null, false);
		new List<TIMapRegionTemplate>();
		if (tifactionState != null)
		{
			(from x in tifactionState.executiveNations.SelectMany<TINationState, TIRegionState>((TINationState x) => x.regions)
				select x.mapRegionTemplate).ToList<TIMapRegionTemplate>();
		}
		while (flag && num < 100)
		{
			flag = false;
			SpaceAssetName spaceAssetName;
			if (this.hullTemplate.largeHull || this.hullTemplate.hugeHull)
			{
				spaceAssetName = new SpaceAssetName((tifactionState != null && TIGlobalValuesState.Customizations.usingCustomizations && tifactionState.scenarioCustomizations.customFactionText.ContainsKey(tifactionState.templateName) && tifactionState.scenarioCustomizations.customFactionText[tifactionState.templateName].customLargeShipNameListIdx != null) ? tifactionState.scenarioCustomizations.customFactionText[tifactionState.templateName].customLargeShipNameListIdx : faction.largeShipNameListIdx, string.Empty);
			}
			else if (this.hullTemplate.smallHull)
			{
				spaceAssetName = new SpaceAssetName((tifactionState != null && TIGlobalValuesState.Customizations.usingCustomizations && tifactionState.scenarioCustomizations.customFactionText.ContainsKey(tifactionState.templateName) && tifactionState.scenarioCustomizations.customFactionText[tifactionState.templateName].customSmallShipNameListIdx != null) ? tifactionState.scenarioCustomizations.customFactionText[tifactionState.templateName].customSmallShipNameListIdx : faction.smallShipNameListIdx, string.Empty);
			}
			else
			{
				spaceAssetName = new SpaceAssetName((tifactionState != null && TIGlobalValuesState.Customizations.usingCustomizations && tifactionState.scenarioCustomizations.customFactionText.ContainsKey(tifactionState.templateName) && tifactionState.scenarioCustomizations.customFactionText[tifactionState.templateName].customMediumShipNameListIdx != null) ? tifactionState.scenarioCustomizations.customFactionText[tifactionState.templateName].customMediumShipNameListIdx : faction.mediumShipNameListIdx, string.Empty);
			}
			if (!GameControl.namelists.TryGetName<SpaceAssetName>(spaceAssetName, out empty))
			{
				Error.Log(string.Concat(new string[]
				{
					"Error getting ship name for ",
					faction.dataName,
					" ",
					this.hullTemplate.dataName,
					" :",
					empty
				}), Array.Empty<object>());
				flag = true;
				num++;
			}
			else if (illegalShipClassNames.Contains(empty))
			{
				flag = true;
				num++;
			}
		}
		return empty;
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x00064BE4 File Offset: 0x00062DE4
	public bool CanTakeOffFromSurfaceShipyard(TIHabModuleState shipyard)
	{
		return (double)this.baseCruiseDeltaV_kps(false) >= shipyard.sector.hab.habSite.MinDeltaVToLaunch_kps(this.baseCombatAcceleration_mps2) && (double)this.baseCombatAcceleration_gs >= shipyard.sector.hab.habSite.parentBody.surfaceGravity_g;
	}

	// Token: 0x060014A8 RID: 5288 RVA: 0x00064C3E File Offset: 0x00062E3E
	public bool CanBuildAtShipyard(TIHabModuleState shipyard)
	{
		return shipyard.active && shipyard.moduleTemplate.allowsShipConstruction && (shipyard.sector.hab.IsStation || this.CanTakeOffFromSurfaceShipyard(shipyard));
	}

	// Token: 0x060014A9 RID: 5289 RVA: 0x00064C72 File Offset: 0x00062E72
	public bool ShouldObsolete(TIFactionState faction)
	{
		return faction.shipDesigns.Any<TISpaceShipTemplate>(delegate(TISpaceShipTemplate x)
		{
			if (x.role != this.role || x.hullTemplate != this.hullTemplate || x.requiresExotics != this.requiresExotics || x.requiresAntimatter != this.requiresAntimatter || !this.combatant)
			{
				return x.baseCruiseDeltaV_kps(false) > this.baseCruiseDeltaV_kps(false) && x.baseCruiseAcceleration_mps2(false) > this.baseCruiseAcceleration_mps2(false);
			}
			return x.TemplateSpaceCombatValue(false, -1f, 1f, false) > this.TemplateSpaceCombatValue(false, -1f, 1f, false);
		});
	}

	// Token: 0x060014AA RID: 5290 RVA: 0x00064C8B File Offset: 0x00062E8B
	public bool Obsolete(TIFactionState faction)
	{
		return faction.obsoleteShipDesigns.Contains(base.dataName);
	}

	// Token: 0x060014AB RID: 5291 RVA: 0x00064CA0 File Offset: 0x00062EA0
	public bool IsDuplicateOf(TISpaceShipTemplate other)
	{
		return this.hullName == other.hullName && this.driveName == other.driveName && this.powerPlantName == other.powerPlantName && this.radiatorName == other.radiatorName && this.propellantTanks == other.propellantTanks && this.moduleTemplateEntries.All<ModuleDataTemplateEntry>((ModuleDataTemplateEntry x) => other.moduleTemplateEntries.Contains(x)) && this.noseWeaponTemplateEntries.All<ModuleDataTemplateEntry>((ModuleDataTemplateEntry x) => other.noseWeaponTemplateEntries.Contains(x)) && this.hullWeaponTemplateEntries.All<ModuleDataTemplateEntry>((ModuleDataTemplateEntry x) => other.hullWeaponTemplateEntries.Contains(x)) && this.fireModeTemplateEntries.All<FireModeDataTemplateEntry>((FireModeDataTemplateEntry x) => other.fireModeTemplateEntries.Contains(x)) && this.noseArmor.materialName == other.noseArmor.materialName && this.noseArmor.armorValue == other.noseArmor.armorValue && this.lateralArmor.materialName == other.lateralArmor.materialName && this.lateralArmor.armorValue == other.lateralArmor.armorValue && this.tailArmor.materialName == other.tailArmor.materialName && this.tailArmor.armorValue == other.tailArmor.armorValue;
	}

	// Token: 0x17000311 RID: 785
	// (get) Token: 0x060014AC RID: 5292 RVA: 0x00064E70 File Offset: 0x00063070
	public bool CanDeleteDesign
	{
		get
		{
			if (this.designingFaction != null && this.designingFaction.AISavingTarget.active && this.designingFaction.AISavingTarget.desiredPurchase == this)
			{
				return false;
			}
			if (GameStateManager.IterateByClass<TISpaceShipState>(false).Any<TISpaceShipState>((TISpaceShipState x) => !x.deleted && x.templateName == base.dataName))
			{
				return false;
			}
			foreach (TIFactionState tifactionState in from x in GameStateManager.AllFactions()
				orderby x == this.designingFaction descending
				select x)
			{
				foreach (TIHabModuleState tihabModuleState in tifactionState.nShipyardQueues.Keys)
				{
					foreach (ShipConstructionQueueItem shipConstructionQueueItem in tifactionState.nShipyardQueues[tihabModuleState])
					{
						if (shipConstructionQueueItem.shipDesign == this || shipConstructionQueueItem.refit_originalShipDesign == this)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
	}

	// Token: 0x060014AD RID: 5293 RVA: 0x00064FBC File Offset: 0x000631BC
	public string quickSummary(bool obfuscateAlienData, TISpaceShipState shipState, bool hideAlienDataDistance = false, bool includePartNames = false, bool listOfficers = false)
	{
		obfuscateAlienData = this.isAlien && obfuscateAlienData;
		hideAlienDataDistance = this.isAlien && hideAlienDataDistance;
		StringBuilder stringBuilder = new StringBuilder();
		if (shipState != null)
		{
			stringBuilder.AppendLine(shipState.NameWithDamageIcons());
		}
		stringBuilder.AppendLine(this.fullClassName);
		stringBuilder.AppendLine(this.roleStr);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine(Loc.T("UI.Fleets.WetMass", new object[] { this.wetMass_tons.ToString("N0") }));
		stringBuilder.AppendLine(Loc.T("UI.Fleets.Crew", new object[] { (obfuscateAlienData || hideAlienDataDistance) ? Loc.T("UI.Fleets.Unknown") : this.crewBillets.ToString("N0") }));
		if (!this.isAlien || !obfuscateAlienData)
		{
			stringBuilder.AppendLine(Loc.T("UI.Fleets.DamConCrew", new object[] { this.damConCrewBillets.ToString("N0") }));
		}
		if (includePartNames)
		{
			stringBuilder.AppendLine(this.driveTemplate.displayName);
			stringBuilder.AppendLine(this.powerPlantTemplate.displayName);
		}
		float num = ((shipState == null) ? this.baseCruiseDeltaV_kps(false) : shipState.currentDeltaV_kps);
		stringBuilder.AppendLine(Loc.T("UI.Fleets.TwoColumn", new object[]
		{
			Loc.T("UI.Fleets.CruiseDeltaVTab"),
			Loc.T("UI.Fleets.SingleDV", new object[] { num.ToString("N1") })
		}));
		stringBuilder.AppendLine(FleetsScreenController.accelerationStr((double)((shipState == null) ? this.baseCruiseAcceleration_gs(false) : shipState.cruiseAcceleration_gs), false, true, true));
		stringBuilder.AppendLine(FleetsScreenController.accelerationStr((double)((shipState == null) ? this.baseCombatAcceleration_gs : shipState.combatAcceleration_gs), true, true, true));
		stringBuilder.AppendLine(Loc.T("UI.Fleets.TwoColumn", new object[]
		{
			Loc.T("UI.Fleets.ArmorSummaryTab"),
			(!hideAlienDataDistance) ? Loc.T("UI.Fleets.ArmorSummaryValue", new object[] { this.noseArmorValue, this.lateralArmorValue, this.tailArmorValue }) : Loc.T("UI.Fleets.Unknown")
		}));
		if (!this.isAlien || !obfuscateAlienData)
		{
			stringBuilder.AppendLine(Loc.T("UI.Fleets.BatteryLine", new object[] { Loc.T("UI.Fleets.GJ", new object[] { this.BatteryCapacity_GJ(false).ToString("N0") }) }));
		}
		if (!hideAlienDataDistance)
		{
			stringBuilder.AppendLine(Loc.T("UI.Fleets.RadiatorsLine", new object[] { this.radiatorTemplate.displayName }));
		}
		if (!this.isAlien || !obfuscateAlienData)
		{
			stringBuilder.AppendLine(Loc.T("UI.Fleets.HeatSinkCapacity", new object[] { this.HeatCapacity_GJ(false).ToString("N0") }));
		}
		if (!hideAlienDataDistance)
		{
			stringBuilder.AppendLine();
			foreach (ModuleDataEntry moduleDataEntry in this.allWeapons)
			{
				if (moduleDataEntry.weaponTemplate.hasMagazine() && moduleDataEntry.weaponTemplate.ref_projectileWeapon != null)
				{
					if (shipState != null)
					{
						stringBuilder.AppendLine(Loc.T("UI.Fleets.TwoColumn", new object[]
						{
							moduleDataEntry.weaponTemplate.displayName,
							new StringBuilder(shipState.ammo[moduleDataEntry].ToString()).Append("/").Append(moduleDataEntry.weaponTemplate.ref_projectileWeapon.FullAmmoCount_Max(this).ToString()).ToString()
						}));
					}
					else
					{
						stringBuilder.AppendLine(Loc.T("UI.Fleets.TwoColumn", new object[]
						{
							moduleDataEntry.weaponTemplate.displayName,
							new StringBuilder(moduleDataEntry.weaponTemplate.ref_projectileWeapon.FullAmmoCount_Max(this).ToString()).Append("/").Append(moduleDataEntry.weaponTemplate.ref_projectileWeapon.FullAmmoCount_Max(this).ToString()).ToString()
						}));
					}
				}
				else
				{
					stringBuilder.AppendLine(moduleDataEntry.weaponTemplate.displayName);
				}
			}
		}
		if (!hideAlienDataDistance)
		{
			stringBuilder.AppendLine();
			foreach (TIShipModuleTemplate tishipModuleTemplate in this.utilitySlotModuleTemplates)
			{
				if (tishipModuleTemplate.displayName != "Unknown")
				{
					stringBuilder.AppendLine(tishipModuleTemplate.displayName);
				}
			}
		}
		if (shipState != null && listOfficers)
		{
			foreach (TIOfficerState tiofficerState in shipState.officers.OrderBy<TIOfficerState, int>((TIOfficerState x) => x.template.sortOrder))
			{
				stringBuilder.AppendLine(new StringBuilder(TIOfficerState.RankStarsInline(tiofficerState.rank)).Append(tiofficerState.template.displayName).ToString());
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x17000312 RID: 786
	// (get) Token: 0x060014AE RID: 5294 RVA: 0x0006555C File Offset: 0x0006375C
	public string roleStr
	{
		get
		{
			return Loc.T(new StringBuilder("UI.Fleets.").Append(this.role.ToString()).ToString());
		}
	}

	// Token: 0x17000313 RID: 787
	// (get) Token: 0x060014AF RID: 5295 RVA: 0x00065588 File Offset: 0x00063788
	public string roleDescription
	{
		get
		{
			return Loc.T(new StringBuilder("UI.Fleets.").Append(this.role.ToString()).Append(".description").ToString());
		}
	}

	// Token: 0x060014B0 RID: 5296 RVA: 0x000655C0 File Offset: 0x000637C0
	public bool AllowedRole(ShipRole role)
	{
		switch (role)
		{
		case ShipRole.NoRole:
			return false;
		case ShipRole.TroopCarrier:
			return this.utilityModules.Any<ModuleDataEntry>(delegate(ModuleDataEntry x)
			{
				TIUtilityModuleTemplate ref_utilityModule = x.moduleTemplate.ref_utilityModule;
				return ref_utilityModule != null && ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Assault);
			});
		case ShipRole.ArmyCarrier:
			return this.utilityModules.Any<ModuleDataEntry>(delegate(ModuleDataEntry x)
			{
				TIUtilityModuleTemplate ref_utilityModule2 = x.moduleTemplate.ref_utilityModule;
				return ref_utilityModule2 != null && ref_utilityModule2.specialModuleRules.Intersect<SpecialModuleRule>(this.armyCarrierRequirement).Any<SpecialModuleRule>();
			});
		case ShipRole.Explorer:
			return this.utilityModules.Any<ModuleDataEntry>(delegate(ModuleDataEntry x)
			{
				TIUtilityModuleTemplate ref_utilityModule3 = x.moduleTemplate.ref_utilityModule;
				return ref_utilityModule3 != null && ref_utilityModule3.specialModuleRules.Intersect<SpecialModuleRule>(this.explorerRequirement).Any<SpecialModuleRule>();
			});
		case ShipRole.InnerSystemColonyShip:
			if (this.isAlien)
			{
				return this.utilityModules.Any<ModuleDataEntry>(delegate(ModuleDataEntry x)
				{
					TIUtilityModuleTemplate ref_utilityModule4 = x.moduleTemplate.ref_utilityModule;
					return ref_utilityModule4 != null && ref_utilityModule4.specialModuleRules.Intersect<SpecialModuleRule>(TISpaceShipState.FoundSurveillanceStationRules).Any<SpecialModuleRule>();
				});
			}
			return this.utilityModules.Any<ModuleDataEntry>(delegate(ModuleDataEntry x)
			{
				TIUtilityModuleTemplate ref_utilityModule5 = x.moduleTemplate.ref_utilityModule;
				return ref_utilityModule5 != null && ref_utilityModule5.specialModuleRules.Intersect<SpecialModuleRule>(this.innerColonyShipRequirement).Any<SpecialModuleRule>();
			});
		case ShipRole.OuterSystemColonyShip:
			return this.utilityModules.Any<ModuleDataEntry>(delegate(ModuleDataEntry x)
			{
				TIUtilityModuleTemplate ref_utilityModule6 = x.moduleTemplate.ref_utilityModule;
				return ref_utilityModule6 != null && ref_utilityModule6.specialModuleRules.Intersect<SpecialModuleRule>(this.outerColonyShipRequirement).Any<SpecialModuleRule>();
			});
		case ShipRole.EarthSurveillance:
			return this.utilityModules.Any<ModuleDataEntry>(delegate(ModuleDataEntry x)
			{
				TIUtilityModuleTemplate ref_utilityModule7 = x.moduleTemplate.ref_utilityModule;
				return ref_utilityModule7 != null && ref_utilityModule7.specialModuleRules.Intersect<SpecialModuleRule>(this.surveillanceShipRequirement).Any<SpecialModuleRule>();
			});
		default:
			return true;
		}
	}

	// Token: 0x060014B1 RID: 5297 RVA: 0x000656D0 File Offset: 0x000638D0
	public bool HasSpecialModuleCapability(SpecialModuleRule rule)
	{
		return this.utilityModules.Any<ModuleDataEntry>(delegate(ModuleDataEntry x)
		{
			TIUtilityModuleTemplate ref_utilityModule = x.moduleTemplate.ref_utilityModule;
			return ref_utilityModule != null && ref_utilityModule.specialModuleRules.Contains(rule);
		});
	}

	// Token: 0x060014B2 RID: 5298 RVA: 0x00065704 File Offset: 0x00063904
	public bool HasSpecialModuleCapability(List<SpecialModuleRule> rules)
	{
		return this.utilityModules.Any<ModuleDataEntry>((ModuleDataEntry x) => x.moduleTemplate.ref_utilityModule != null && x.moduleTemplate.ref_utilityModule.specialModuleRules.Intersect<SpecialModuleRule>(rules).Any<SpecialModuleRule>());
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x00065735 File Offset: 0x00063935
	public bool HasFoundBaseCapability()
	{
		return TISpaceShipState.FoundBaseRules.Any<SpecialModuleRule>((SpecialModuleRule x) => this.HasSpecialModuleCapability(x));
	}

	// Token: 0x060014B4 RID: 5300 RVA: 0x0006574D File Offset: 0x0006394D
	public bool HasFoundStationCapability()
	{
		return TISpaceShipState.FoundAnyStationRules.Any<SpecialModuleRule>((SpecialModuleRule x) => this.HasSpecialModuleCapability(x));
	}

	// Token: 0x060014B5 RID: 5301 RVA: 0x00065765 File Offset: 0x00063965
	public bool HasFoundStandardStationCapability()
	{
		return TISpaceShipState.FoundStandardStationRules.Any<SpecialModuleRule>((SpecialModuleRule x) => this.HasSpecialModuleCapability(x));
	}

	// Token: 0x060014B6 RID: 5302 RVA: 0x0006577D File Offset: 0x0006397D
	public bool HasFoundSurveillanceStationCapability()
	{
		return TISpaceShipState.FoundSurveillanceStationRules.Any<SpecialModuleRule>((SpecialModuleRule x) => this.HasSpecialModuleCapability(x));
	}

	// Token: 0x060014B7 RID: 5303 RVA: 0x00065798 File Offset: 0x00063998
	public bool CanFulfillGoal(FactionGoal_Fleet goal)
	{
		GoalType goalType = goal.GetGoalType();
		switch (goalType)
		{
		case GoalType.ProspectSites:
			return this.HasSpecialModuleCapability(SpecialModuleRule.Prospector);
		case GoalType.FoundPlatform:
		case GoalType.FoundMaxStation:
			break;
		case GoalType.FoundBase:
			return this.HasSpecialModuleCapability(SpecialModuleRule.FoundFissionOutpost) || this.HasSpecialModuleCapability(SpecialModuleRule.FoundFusionOutpost) || this.HasSpecialModuleCapability(SpecialModuleRule.FoundSolarOutpost);
		default:
			switch (goalType)
			{
			case GoalType.DefendWithFleet:
			case GoalType.AttackWithFleet:
				return this.combatant;
			case GoalType.SecureEarthSpace:
				break;
			case GoalType.CaptureHab:
				return this.HasSpecialModuleCapability(SpecialModuleRule.Assault);
			default:
				switch (goalType)
				{
				case GoalType.InvadeEarth:
					return this.HasSpecialModuleCapability(SpecialModuleRule.LandArmy);
				case GoalType.SurveilEarth:
					return this.HasSpecialModuleCapability(SpecialModuleRule.Surveillance);
				case GoalType.FoundStation:
					goto IL_007D;
				case GoalType.FoundSurveillanceStation:
				{
					FactionGoal_FoundSurveillanceStation factionGoal_FoundSurveillanceStation = goal as FactionGoal_FoundSurveillanceStation;
					return (factionGoal_FoundSurveillanceStation.tier == 1 && this.HasSpecialModuleCapability(SpecialModuleRule.FoundSurveillancePlatform)) || (factionGoal_FoundSurveillanceStation.tier == 2 && this.HasSpecialModuleCapability(SpecialModuleRule.FoundSurveillanceOrbital)) || (factionGoal_FoundSurveillanceStation.tier == 3 && this.HasSpecialModuleCapability(SpecialModuleRule.FoundSurveillanceRing));
				}
				}
				break;
			}
			return true;
		}
		IL_007D:
		return this.HasSpecialModuleCapability(SpecialModuleRule.FoundFusionPlatform) || this.HasSpecialModuleCapability(SpecialModuleRule.FoundFissionPlatform) || this.HasSpecialModuleCapability(SpecialModuleRule.FoundSolarPlatform);
	}

	// Token: 0x060014B8 RID: 5304 RVA: 0x000658B0 File Offset: 0x00063AB0
	public float AssaultCombatValue(bool defense)
	{
		float num = 0f;
		foreach (ModuleDataEntry moduleDataEntry in this.utilityModules)
		{
			TIUtilityModuleTemplate ref_utilityModule = moduleDataEntry.moduleTemplate.ref_utilityModule;
			if (ref_utilityModule != null && (ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Assault) || (defense && this.HasSpecialModuleCapability(SpecialModuleRule.MarineOpsDefenseOnly))))
			{
				num += ref_utilityModule.marineOpsValue;
			}
		}
		return num;
	}

	// Token: 0x060014B9 RID: 5305 RVA: 0x00065930 File Offset: 0x00063B30
	public bool FitsRole(ShipRole role)
	{
		if (!this.AllowedRole(role))
		{
			return false;
		}
		switch (role)
		{
		case ShipRole.LS_Penetrator:
			return this.baseCruiseDeltaV_kps(false) > 200f && this.allWeaponTemplates.Count > 0 && this.noseArmorValue > 0;
		case ShipRole.LM_Protector:
			if (this.baseCruiseDeltaV_kps(false) > 200f && this.hullWeapons.Count<ModuleDataEntry>() > 1)
			{
				return this.hullWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.guardianMode);
			}
			return false;
		case ShipRole.LM_Interdictor:
			if (this.baseCruiseDeltaV_kps(false) > 200f)
			{
				if (this.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.targetingRange_km > 500f))
				{
					return this.noseArmorValue > 0;
				}
			}
			return false;
		case ShipRole.LL_Intruder:
			if (this.baseCruiseDeltaV_kps(false) > 200f)
			{
				if (this.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.targetingRange_km > 800f))
				{
					return this.noseArmorValue > 0;
				}
			}
			return false;
		case ShipRole.LL_Bomber:
			if (this.baseCruiseDeltaV_kps(false) > 200f && this.noseWeaponTemplates.Count<TIShipWeaponTemplate>() > 0)
			{
				return this.noseWeaponTemplates.All<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.canBombardThroughAtmosphere && x.bombardmentValue > 0f);
			}
			return false;
		case ShipRole.MS_Strike:
			return this.baseCruiseDeltaV_kps(false) > 100f && this.allWeaponTemplates.Count > 0 && this.noseArmorValue > 0;
		case ShipRole.MM_SpaceSuperiority:
			if (this.baseCruiseDeltaV_kps(false) > 100f)
			{
				if (this.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.targetingRange_km > 500f))
				{
					return this.noseArmorValue > 0;
				}
			}
			return false;
		case ShipRole.ML_Standoff:
			if (this.baseCruiseDeltaV_kps(false) > 100f)
			{
				if (this.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.targetingRange_km > 800f))
				{
					return this.noseArmorValue > 0;
				}
			}
			return false;
		case ShipRole.SS_Interceptor:
			return this.allWeaponTemplates.Count > 0 && this.noseArmorValue > 0;
		case ShipRole.SM_Patrol:
			return this.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.targetingRange_km > 500f) && this.noseArmorValue > 0;
		case ShipRole.SL_Defender:
			return this.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.targetingRange_km > 800f) && this.noseArmorValue > 0;
		default:
			return true;
		}
	}

	// Token: 0x060014BA RID: 5306 RVA: 0x00065C08 File Offset: 0x00063E08
	public ShipRole AssignRole()
	{
		foreach (ShipRole shipRole in this.orderToCheckRoles)
		{
			if (this.FitsRole(shipRole))
			{
				return shipRole;
			}
		}
		return ShipRole.CouncilorTransport;
	}

	// Token: 0x17000314 RID: 788
	// (get) Token: 0x060014BB RID: 5307 RVA: 0x00065C64 File Offset: 0x00063E64
	public bool nonCombatant
	{
		get
		{
			return !this.combatant;
		}
	}

	// Token: 0x17000315 RID: 789
	// (get) Token: 0x060014BC RID: 5308 RVA: 0x00065C6F File Offset: 0x00063E6F
	public bool combatant
	{
		get
		{
			return this.role.IsCombatantRole();
		}
	}

	// Token: 0x060014BD RID: 5309 RVA: 0x00065C7C File Offset: 0x00063E7C
	public static bool shortRangeStrategic(ShipRole role)
	{
		return role - ShipRole.SS_Interceptor <= 2;
	}

	// Token: 0x060014BE RID: 5310 RVA: 0x00065C88 File Offset: 0x00063E88
	public static bool mediumRangeStrategic(ShipRole role)
	{
		return role - ShipRole.MS_Strike <= 2;
	}

	// Token: 0x060014BF RID: 5311 RVA: 0x00065C94 File Offset: 0x00063E94
	public static bool longRangeStrategic(ShipRole role)
	{
		return role - ShipRole.TroopCarrier <= 11;
	}

	// Token: 0x060014C0 RID: 5312 RVA: 0x00065CA0 File Offset: 0x00063EA0
	public static bool longRangeCombatant(ShipRole role)
	{
		return role - ShipRole.LL_Intruder <= 1 || role == ShipRole.ML_Standoff || role == ShipRole.SL_Defender;
	}

	// Token: 0x060014C1 RID: 5313 RVA: 0x00065CB6 File Offset: 0x00063EB6
	public static bool shortRangeCombatant(ShipRole role)
	{
		return role == ShipRole.LS_Penetrator || role == ShipRole.MS_Strike || role == ShipRole.SS_Interceptor;
	}

	// Token: 0x060014C2 RID: 5314 RVA: 0x00065CC9 File Offset: 0x00063EC9
	public static bool mediumRangeCombatant(ShipRole role)
	{
		return role - ShipRole.LM_Protector <= 1 || role == ShipRole.MM_SpaceSuperiority || role == ShipRole.SM_Patrol;
	}

	// Token: 0x060014C3 RID: 5315 RVA: 0x00065CDF File Offset: 0x00063EDF
	public static bool SoloOperator(ShipRole role)
	{
		return role - ShipRole.Explorer <= 2 || role == ShipRole.CouncilorTransport || role - ShipRole.SS_Interceptor <= 2;
	}

	// Token: 0x060014C4 RID: 5316 RVA: 0x00065CF8 File Offset: 0x00063EF8
	public string DebugSummary()
	{
		TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>(this.factionName, false);
		int num = 0;
		if (tifactionState.shipsBuiltInClass.ContainsKey(base.dataName))
		{
			num = tifactionState.shipsBuiltInClass[base.dataName];
		}
		string[] array = new string[33];
		array[0] = this.factionName;
		array[1] = " (";
		array[2] = num.ToString();
		array[3] = ") ";
		array[4] = this.fullClassName;
		array[5] = " CV: ";
		array[6] = this.TemplateSpaceCombatValue(false, -1f, 1f, false).ToString("N2");
		array[7] = " ";
		array[8] = this.role.ToString();
		array[9] = "   DV: ";
		array[10] = this.baseCruiseDeltaV_kps(true).ToString("N1");
		array[11] = "   Acc ";
		array[12] = this.baseCruiseAcceleration_gs(true).ToString("N2");
		array[13] = " / ";
		array[14] = this.baseCombatAcceleration_gs.ToString("N2");
		array[15] = "   ";
		array[16] = this.driveTemplate.displayName;
		array[17] = "   ";
		array[18] = this.powerPlantTemplate.displayName;
		array[19] = "   Armor ";
		array[20] = this.noseArmor.materialName;
		array[21] = " ";
		array[22] = this.noseArmor.armorValue.ToString();
		array[23] = "/";
		array[24] = this.lateralArmor.armorValue.ToString();
		array[25] = "/";
		array[26] = this.tailArmor.armorValue.ToString();
		array[27] = "   ";
		array[28] = this.allWeaponTemplates.ToCommaSeparatedString<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.displayName);
		array[29] = "   ";
		array[30] = this.utilitySlotModuleTemplates.ToCommaSeparatedString<TIShipModuleTemplate>((TIShipModuleTemplate x) => x.displayName);
		array[31] = "   Cost: ";
		array[32] = this.spaceResourceConstructionCost(true, null, true, false, false).resourceCosts.ToCommaSeparatedString<ResourceValue>((ResourceValue x) => x.resource.ToString() + ":" + x.value.ToString());
		return string.Concat(array);
	}

	// Token: 0x060014C5 RID: 5317 RVA: 0x00065F67 File Offset: 0x00064167
	public static void ClearStaticData()
	{
		TISpaceShipTemplate.testCombats = null;
	}

	// Token: 0x04001216 RID: 4630
	public string factionName;

	// Token: 0x04001217 RID: 4631
	public string hullName;

	// Token: 0x04001218 RID: 4632
	public string driveName;

	// Token: 0x04001219 RID: 4633
	public string powerPlantName;

	// Token: 0x0400121A RID: 4634
	public string radiatorName;

	// Token: 0x0400121B RID: 4635
	public int propellantTanks;

	// Token: 0x0400121C RID: 4636
	public int refitIteration;

	// Token: 0x0400121D RID: 4637
	public ArmorFacingTemplate noseArmor;

	// Token: 0x0400121E RID: 4638
	public ArmorFacingTemplate lateralArmor;

	// Token: 0x0400121F RID: 4639
	public ArmorFacingTemplate tailArmor;

	// Token: 0x04001220 RID: 4640
	public List<ModuleDataTemplateEntry> moduleTemplateEntries;

	// Token: 0x04001221 RID: 4641
	public List<ModuleDataTemplateEntry> hullWeaponTemplateEntries;

	// Token: 0x04001222 RID: 4642
	public List<ModuleDataTemplateEntry> noseWeaponTemplateEntries;

	// Token: 0x04001223 RID: 4643
	public List<FireModeDataTemplateEntry> fireModeTemplateEntries;

	// Token: 0x04001224 RID: 4644
	public ShipRole role;

	// Token: 0x04001225 RID: 4645
	public const float longRange_km = 800f;

	// Token: 0x04001226 RID: 4646
	public const float mediumRange_km = 500f;

	// Token: 0x04001227 RID: 4647
	public const float shortRange_km = 200f;

	// Token: 0x04001228 RID: 4648
	public const float propellantTankMass_tons = 100f;

	// Token: 0x04001229 RID: 4649
	public const float mass_per_crew_tons = 4f;

	// Token: 0x0400122A RID: 4650
	public const float maneuverThrust_N_human = 2500000f;

	// Token: 0x0400122B RID: 4651
	public const float maneuverThrust_N_alien = 4000000f;

	// Token: 0x0400122C RID: 4652
	public const int maxThrusters = 6;

	// Token: 0x0400122D RID: 4653
	public bool hasDisplayName;

	// Token: 0x0400122E RID: 4654
	[SerializeField]
	private float _unnormalizedCombatValue = -1f;

	// Token: 0x0400122F RID: 4655
	private float _combatValue = -1f;

	// Token: 0x04001230 RID: 4656
	private float _baseCruiseDeltaV_kps = -1f;

	// Token: 0x04001231 RID: 4657
	private float _baseCruiseAcceleration_mps2 = -1f;

	// Token: 0x04001232 RID: 4658
	public int hullAppearanceIndex;

	// Token: 0x04001233 RID: 4659
	public bool hideInSkirmish;

	// Token: 0x04001234 RID: 4660
	[SerializeField]
	private bool isIncompleteDesign;

	// Token: 0x04001235 RID: 4661
	public TINationState nation;

	// Token: 0x04001236 RID: 4662
	private TIFactionState _designingFaction;

	// Token: 0x04001237 RID: 4663
	private TIShipHullTemplate _hullTemplate;

	// Token: 0x04001238 RID: 4664
	private TIDriveTemplate _driveTemplate;

	// Token: 0x04001239 RID: 4665
	private TIPowerPlantTemplate _powerPlantTemplate;

	// Token: 0x0400123A RID: 4666
	private TIRadiatorTemplate _radiatorTemplate;

	// Token: 0x0400123B RID: 4667
	private TIShipArmorTemplate _noseArmorTemplate;

	// Token: 0x0400123C RID: 4668
	private TIShipArmorTemplate _lateralArmorTemplate;

	// Token: 0x0400123D RID: 4669
	private TIShipArmorTemplate _tailArmorTemplate;

	// Token: 0x0400123E RID: 4670
	private float _requiredExotics = -1f;

	// Token: 0x0400123F RID: 4671
	private float _requiredAntimatter = -1f;

	// Token: 0x04001240 RID: 4672
	private static List<TISpaceShipTemplate.TestCombat> testCombats;

	// Token: 0x04001241 RID: 4673
	private static int baselineUnormalizedSCVUpdatedFrame = -1;

	// Token: 0x04001242 RID: 4674
	public const float StandardAlienShipStrength = 100f;

	// Token: 0x04001243 RID: 4675
	private List<ModuleDataEntry> cachedUtilityModules;

	// Token: 0x04001244 RID: 4676
	private List<TIShipModuleTemplate> cachedUtilityModuleTemplates;

	// Token: 0x04001245 RID: 4677
	private List<ModuleDataEntry> cachedNoseWeapons;

	// Token: 0x04001246 RID: 4678
	private List<TIShipWeaponTemplate> cachedNoseWeaponTemplates;

	// Token: 0x04001247 RID: 4679
	private List<ModuleDataEntry> cachedHullWeapons;

	// Token: 0x04001248 RID: 4680
	private List<TIShipWeaponTemplate> cachedHullWeaponTemplates;

	// Token: 0x04001249 RID: 4681
	private const float PDWeaponBonusPowerLimit = 0.5f;

	// Token: 0x0400124A RID: 4682
	private float cachedDryMass_tons;

	// Token: 0x0400124B RID: 4683
	private TIResourcesCost _spaceResourceConstructionCost;

	// Token: 0x0400124C RID: 4684
	private float _heatCapacity_GJ = -1f;

	// Token: 0x0400124D RID: 4685
	private float _batteryCapacity_GJ = -1f;

	// Token: 0x0400124E RID: 4686
	public const int numRotationalThrusters = 2;

	// Token: 0x0400124F RID: 4687
	private readonly List<SpecialModuleRule> outerColonyShipRequirement = new List<SpecialModuleRule>
	{
		SpecialModuleRule.FoundFissionOutpost,
		SpecialModuleRule.FoundFissionPlatform,
		SpecialModuleRule.FoundFusionOutpost,
		SpecialModuleRule.FoundFusionPlatform,
		SpecialModuleRule.FoundAutomatedFissionOutpost,
		SpecialModuleRule.FoundAutomatedFissionPlatform
	};

	// Token: 0x04001250 RID: 4688
	private readonly List<SpecialModuleRule> innerColonyShipRequirement = new List<SpecialModuleRule>
	{
		SpecialModuleRule.FoundFissionOutpost,
		SpecialModuleRule.FoundFissionPlatform,
		SpecialModuleRule.FoundFusionOutpost,
		SpecialModuleRule.FoundFusionPlatform,
		SpecialModuleRule.FoundAutomatedFissionOutpost,
		SpecialModuleRule.FoundAutomatedFissionPlatform,
		SpecialModuleRule.FoundSolarOutpost,
		SpecialModuleRule.FoundSolarPlatform,
		SpecialModuleRule.FoundAutomatedSolarOutpost,
		SpecialModuleRule.FoundAutomatedSolarPlatform
	};

	// Token: 0x04001251 RID: 4689
	private readonly List<SpecialModuleRule> explorerRequirement = new List<SpecialModuleRule> { SpecialModuleRule.Prospector };

	// Token: 0x04001252 RID: 4690
	private readonly List<SpecialModuleRule> armyCarrierRequirement = new List<SpecialModuleRule> { SpecialModuleRule.LandArmy };

	// Token: 0x04001253 RID: 4691
	private readonly List<SpecialModuleRule> surveillanceShipRequirement = new List<SpecialModuleRule> { SpecialModuleRule.Surveillance };

	// Token: 0x04001254 RID: 4692
	private readonly List<ShipRole> orderToCheckRoles = new List<ShipRole>
	{
		ShipRole.InnerSystemColonyShip,
		ShipRole.OuterSystemColonyShip,
		ShipRole.EarthSurveillance,
		ShipRole.TroopCarrier,
		ShipRole.ArmyCarrier,
		ShipRole.Explorer,
		ShipRole.LL_Bomber,
		ShipRole.LL_Intruder,
		ShipRole.LM_Interdictor,
		ShipRole.LM_Protector,
		ShipRole.LS_Penetrator,
		ShipRole.ML_Standoff,
		ShipRole.MM_SpaceSuperiority,
		ShipRole.MS_Strike,
		ShipRole.SL_Defender,
		ShipRole.SM_Patrol,
		ShipRole.SS_Interceptor,
		ShipRole.CouncilorTransport
	};

	// Token: 0x02000BEC RID: 3052
	public class TestCombat
	{
		// Token: 0x06006AAC RID: 27308 RVA: 0x00303C0C File Offset: 0x00301E0C
		public void AddAttack(TISpaceShipTemplate.TestCombat.Attack attack)
		{
			this.Attacks.Add(attack);
		}

		// Token: 0x04004C7A RID: 19578
		public List<TISpaceShipTemplate.TestCombat.Attack> Attacks = new List<TISpaceShipTemplate.TestCombat.Attack>();

		// Token: 0x020013E6 RID: 5094
		public struct Attack
		{
			// Token: 0x0400731A RID: 29466
			public TIShipWeaponTemplate Weapon;

			// Token: 0x0400731B RID: 29467
			public float Range_km;

			// Token: 0x0400731C RID: 29468
			public ArmorFacing ArmorFacing;

			// Token: 0x0400731D RID: 29469
			public float Angle;

			// Token: 0x0400731E RID: 29470
			public float Roll;

			// Token: 0x0400731F RID: 29471
			public float TargetingBonus;
		}
	}
}
