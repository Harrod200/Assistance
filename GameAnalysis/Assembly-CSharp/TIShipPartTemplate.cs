using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003CE RID: 974
public abstract class TIShipPartTemplate : TIDataTemplate
{
	// Token: 0x17000205 RID: 517
	// (get) Token: 0x0600124A RID: 4682 RVA: 0x00058700 File Offset: 0x00056900
	public virtual List<ShipModuleSlotType> allowedSlots { get; }

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x0600124B RID: 4683 RVA: 0x00058708 File Offset: 0x00056908
	public virtual int internalSize
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x17000207 RID: 519
	// (get) Token: 0x0600124C RID: 4684 RVA: 0x0005870B File Offset: 0x0005690B
	public virtual bool hasModel
	{
		get
		{
			return this.modelResource != null;
		}
	}

	// Token: 0x17000208 RID: 520
	// (get) Token: 0x0600124D RID: 4685 RVA: 0x00058718 File Offset: 0x00056918
	public float hitPoints
	{
		get
		{
			float? num = this.hp;
			if (num == null)
			{
				return (float)(3 * this.internalSize);
			}
			return num.GetValueOrDefault();
		}
	}

	// Token: 0x0600124E RID: 4686
	public abstract float buildMass_tons(float value1 = 0f, float value2 = 0f, float value3 = 0f, float value4 = 0f, bool bValue = false);

	// Token: 0x0600124F RID: 4687
	public abstract TIResourcesCost buildCost(float value = 0f, float value2 = 0f);

	// Token: 0x17000209 RID: 521
	// (get) Token: 0x06001250 RID: 4688 RVA: 0x00058746 File Offset: 0x00056946
	public virtual TIDriveTemplate ref_drive
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700020A RID: 522
	// (get) Token: 0x06001251 RID: 4689 RVA: 0x00058749 File Offset: 0x00056949
	public virtual TIPowerPlantTemplate ref_powerPlant
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700020B RID: 523
	// (get) Token: 0x06001252 RID: 4690 RVA: 0x0005874C File Offset: 0x0005694C
	public virtual TIBatteryTemplate ref_battery
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700020C RID: 524
	// (get) Token: 0x06001253 RID: 4691 RVA: 0x0005874F File Offset: 0x0005694F
	public virtual TIRadiatorTemplate ref_radiator
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x06001254 RID: 4692 RVA: 0x00058752 File Offset: 0x00056952
	public virtual TIHeatSinkTemplate ref_heatSink
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700020E RID: 526
	// (get) Token: 0x06001255 RID: 4693 RVA: 0x00058755 File Offset: 0x00056955
	public virtual TIUtilityModuleTemplate ref_utilityModule
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700020F RID: 527
	// (get) Token: 0x06001256 RID: 4694 RVA: 0x00058758 File Offset: 0x00056958
	public virtual TIShipArmorTemplate ref_armor
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000210 RID: 528
	// (get) Token: 0x06001257 RID: 4695 RVA: 0x0005875B File Offset: 0x0005695B
	public virtual TIShipWeaponTemplate ref_weapon
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000211 RID: 529
	// (get) Token: 0x06001258 RID: 4696 RVA: 0x0005875E File Offset: 0x0005695E
	public virtual TIBeamWeaponTemplate ref_beamWeapon
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000212 RID: 530
	// (get) Token: 0x06001259 RID: 4697 RVA: 0x00058761 File Offset: 0x00056961
	public virtual TILaserWeaponTemplate ref_laserWeapon
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000213 RID: 531
	// (get) Token: 0x0600125A RID: 4698 RVA: 0x00058764 File Offset: 0x00056964
	public virtual TIParticleWeaponTemplate ref_particleWeapon
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000214 RID: 532
	// (get) Token: 0x0600125B RID: 4699 RVA: 0x00058767 File Offset: 0x00056967
	public virtual TIMissileTemplate ref_missileWeapon
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000215 RID: 533
	// (get) Token: 0x0600125C RID: 4700 RVA: 0x0005876A File Offset: 0x0005696A
	public virtual TIProjectileWeaponTemplate ref_projectileWeapon
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000216 RID: 534
	// (get) Token: 0x0600125D RID: 4701 RVA: 0x0005876D File Offset: 0x0005696D
	public virtual TIGunTypeWeaponTemplate ref_gunWeapon
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000217 RID: 535
	// (get) Token: 0x0600125E RID: 4702 RVA: 0x00058770 File Offset: 0x00056970
	public bool isAlien
	{
		get
		{
			return this.requiredProjectName == TemplateManager.global.alienMasterProject || this.requiredProjectName == TemplateManager.global.alienAdvancedMasterProject;
		}
	}

	// Token: 0x17000218 RID: 536
	// (get) Token: 0x0600125F RID: 4703 RVA: 0x000587A0 File Offset: 0x000569A0
	public virtual bool isDrive
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000219 RID: 537
	// (get) Token: 0x06001260 RID: 4704 RVA: 0x000587A3 File Offset: 0x000569A3
	public virtual bool isPowerPlant
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700021A RID: 538
	// (get) Token: 0x06001261 RID: 4705 RVA: 0x000587A6 File Offset: 0x000569A6
	public virtual bool isBattery
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700021B RID: 539
	// (get) Token: 0x06001262 RID: 4706 RVA: 0x000587A9 File Offset: 0x000569A9
	public virtual bool isRadiator
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700021C RID: 540
	// (get) Token: 0x06001263 RID: 4707 RVA: 0x000587AC File Offset: 0x000569AC
	public virtual bool isHeatSink
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700021D RID: 541
	// (get) Token: 0x06001264 RID: 4708 RVA: 0x000587AF File Offset: 0x000569AF
	public virtual bool isUtilityModule
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700021E RID: 542
	// (get) Token: 0x06001265 RID: 4709 RVA: 0x000587B2 File Offset: 0x000569B2
	public virtual bool isArmor
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700021F RID: 543
	// (get) Token: 0x06001266 RID: 4710 RVA: 0x000587B5 File Offset: 0x000569B5
	public virtual bool isWeapon
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x06001267 RID: 4711 RVA: 0x000587B8 File Offset: 0x000569B8
	public virtual bool isBeamWeapon
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x06001268 RID: 4712 RVA: 0x000587BB File Offset: 0x000569BB
	public virtual bool isLaserWeapon
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000222 RID: 546
	// (get) Token: 0x06001269 RID: 4713 RVA: 0x000587BE File Offset: 0x000569BE
	public virtual bool isParticleWeapon
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000223 RID: 547
	// (get) Token: 0x0600126A RID: 4714 RVA: 0x000587C1 File Offset: 0x000569C1
	public virtual bool isMissileWeapon
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000224 RID: 548
	// (get) Token: 0x0600126B RID: 4715 RVA: 0x000587C4 File Offset: 0x000569C4
	public virtual bool isProjectileWeapon
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000225 RID: 549
	// (get) Token: 0x0600126C RID: 4716 RVA: 0x000587C7 File Offset: 0x000569C7
	public virtual bool isGunTypeWeapon
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000226 RID: 550
	// (get) Token: 0x0600126D RID: 4717 RVA: 0x000587CA File Offset: 0x000569CA
	public virtual bool isNavalGunWeapon
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000227 RID: 551
	// (get) Token: 0x0600126E RID: 4718 RVA: 0x000587CD File Offset: 0x000569CD
	public virtual bool isMagneticGunWeapon
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000228 RID: 552
	// (get) Token: 0x0600126F RID: 4719 RVA: 0x000587D0 File Offset: 0x000569D0
	public virtual bool isPlasmaWeapon
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000229 RID: 553
	// (get) Token: 0x06001270 RID: 4720 RVA: 0x000587D3 File Offset: 0x000569D3
	public virtual string description
	{
		get
		{
			return Loc.T(new StringBuilder(base.GetType().Name).Append(".description.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x00058804 File Offset: 0x00056A04
	public virtual float AIScoringValueForResearch()
	{
		return 1f;
	}

	// Token: 0x1700022A RID: 554
	// (get) Token: 0x06001272 RID: 4722 RVA: 0x0005880B File Offset: 0x00056A0B
	public virtual bool exoFighterPart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700022B RID: 555
	// (get) Token: 0x06001273 RID: 4723 RVA: 0x0005880E File Offset: 0x00056A0E
	public virtual float repairCostMultipler
	{
		get
		{
			return TemplateManager.global.shipPartRepairBaseCostMultiplier;
		}
	}

	// Token: 0x1700022C RID: 556
	// (get) Token: 0x06001274 RID: 4724 RVA: 0x0005881C File Offset: 0x00056A1C
	public TIProjectTemplate requiredProject
	{
		get
		{
			if (!string.IsNullOrEmpty(this.requiredProjectName))
			{
				if (this._cachedRequiredProject == null)
				{
					this._cachedRequiredProject = TemplateManager.Find<TIProjectTemplate>(this.requiredProjectName, false);
					if (this._cachedRequiredProject == null)
					{
						Log.Error("Bad requiredProjectName: " + this.requiredProjectName + " for ship part " + base.dataName, Array.Empty<object>());
					}
				}
				return this._cachedRequiredProject;
			}
			return null;
		}
	}

	// Token: 0x06001275 RID: 4725 RVA: 0x00058885 File Offset: 0x00056A85
	public bool FactionCanBuild(TIFactionState faction)
	{
		return (!faction.IsAlienFaction && this.requiredProject == null) || faction.completedProjects.Contains(this.requiredProject);
	}

	// Token: 0x06001276 RID: 4726 RVA: 0x000588AC File Offset: 0x00056AAC
	public string GetFullDescription(TISpaceShipState ship = null, TISpaceShipTemplate shipTemplate = null, bool prospective = false, ShipModuleSlotType slot = ShipModuleSlotType.None, bool splitFireModes = false)
	{
		StringBuilder stringBuilder = new StringBuilder(this.description).AppendLine().Append(this.GetDescriptionData(ship, shipTemplate, prospective, slot, splitFireModes));
		if (this.Explosive())
		{
			stringBuilder.AppendLine(Loc.T("UI.Fleets.Explosive"));
		}
		return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
	}

	// Token: 0x06001277 RID: 4727
	public abstract string GetDescriptionData(TISpaceShipState ship = null, TISpaceShipTemplate shipTemplate = null, bool prospective = false, ShipModuleSlotType slot = ShipModuleSlotType.None, bool splitFireModes = false);

	// Token: 0x06001278 RID: 4728 RVA: 0x00058908 File Offset: 0x00056B08
	public virtual string GetLocalizedMass()
	{
		return Loc.T("UI.Fleets.Mass", new object[] { TIUtilities.FormatBigOrSmallNumber(this.buildMass_tons(0f, 0f, 0f, 0f, false), 1, 7, 0, false, false) });
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x00058950 File Offset: 0x00056B50
	public virtual string GetLocalizedCost()
	{
		return Loc.T("UI.Fleets.Cost", new object[] { this.buildCost(0f, 0f).ToString("Relevant", false, false, null, false, FactionResource.None) });
	}

	// Token: 0x0600127A RID: 4730 RVA: 0x0005898F File Offset: 0x00056B8F
	public string GetLocalizedCrew()
	{
		return Loc.T("UI.Fleets.Crew", new object[] { this.crew.ToString("N0") });
	}

	// Token: 0x0600127B RID: 4731 RVA: 0x000589B4 File Offset: 0x00056BB4
	public bool Explosive(TISpaceShipState ship, ModuleDataEntry moduleData)
	{
		if (ship == null)
		{
			return this.Explosive();
		}
		return (this.isMissileWeapon && ship.ammo[moduleData] > 0) || this.isPowerPlant || (this.isDrive && ship.currentDeltaV_kps > 0f && (this.ref_drive.GetPerTankPropellantMaterials(ship.faction).antimatter > 0f || (this.ref_drive.GetPerTankPropellantMaterials(ship.faction).fissiles >= 0.2f && this.ref_drive.thrust_N > 10000000f))) || (this.isHeatSink && ship.heatFraction > 0.01f) || (this.isUtilityModule && (this.ref_utilityModule.weightedBuildMaterials.antimatter > 0f || this.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Magazine)));
	}

	// Token: 0x0600127C RID: 4732 RVA: 0x00058AA4 File Offset: 0x00056CA4
	public bool Explosive()
	{
		return this.isMissileWeapon || this.isPowerPlant || this.isHeatSink || (this.isDrive && (this.ref_drive.perTankPropellantMaterials.antimatter > 0f || (this.ref_drive.perTankPropellantMaterials.fissiles >= 0.2f && this.ref_drive.thrust_N > 10000000f))) || (this.isUtilityModule && (this.ref_utilityModule.weightedBuildMaterials.antimatter > 0f || this.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Magazine)));
	}

	// Token: 0x0600127D RID: 4733 RVA: 0x00058B50 File Offset: 0x00056D50
	public bool HighlyExplosive(TISpaceShipState ship)
	{
		return (this.isMissileWeapon && this.ref_missileWeapon.ammoMaterials.antimatter > 0f) || (this.isDrive && (this.ref_drive.GetPerTankPropellantMaterials(ship.faction).antimatter > 0f || (this.ref_drive.GetPerTankPropellantMaterials(ship.faction).fissiles >= 0.2f && this.ref_drive.thrust_N > 10000000f))) || (this.isPowerPlant && this.ref_powerPlant.weightedBuildMaterials.antimatter > 0f) || (this.isUtilityModule && (this.ref_utilityModule.weightedBuildMaterials.antimatter > 0f || (this.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Magazine) && ship.AllWeaponModuleData().Any<ModuleDataEntry>((ModuleDataEntry x) => x.moduleTemplate.isMissileWeapon && x.moduleTemplate.weightedBuildMaterials.antimatter > 0f && ship.ammo[x] > 0))));
	}

	// Token: 0x040010EE RID: 4334
	public string requiredProjectName;

	// Token: 0x040010EF RID: 4335
	public ResourceCostBuilder weightedBuildMaterials;

	// Token: 0x040010F0 RID: 4336
	public int crew;

	// Token: 0x040010F1 RID: 4337
	public string iconResource;

	// Token: 0x040010F2 RID: 4338
	public string modelResource;

	// Token: 0x040010F3 RID: 4339
	public string combatUIpath;

	// Token: 0x040010F4 RID: 4340
	public bool noCombatRepair;

	// Token: 0x040010F5 RID: 4341
	public float? hp;

	// Token: 0x040010F6 RID: 4342
	private TIProjectTemplate _cachedRequiredProject;

	// Token: 0x040010F7 RID: 4343
	public static readonly SpecialModuleRule[] PrimaryRoleModules = new SpecialModuleRule[]
	{
		SpecialModuleRule.Crashdown,
		SpecialModuleRule.LandHydra,
		SpecialModuleRule.LandArmy,
		SpecialModuleRule.Surveillance,
		SpecialModuleRule.FoundFissionOutpost,
		SpecialModuleRule.FoundFissionPlatform,
		SpecialModuleRule.FoundFusionOutpost,
		SpecialModuleRule.FoundFusionPlatform,
		SpecialModuleRule.FoundSolarOutpost,
		SpecialModuleRule.FoundSolarPlatform,
		SpecialModuleRule.FoundAutomatedFissionOutpost,
		SpecialModuleRule.FoundAutomatedFissionPlatform,
		SpecialModuleRule.FoundAutomatedSolarOutpost,
		SpecialModuleRule.FoundAutomatedSolarPlatform,
		SpecialModuleRule.FoundSurveillancePlatform,
		SpecialModuleRule.FoundSurveillanceOrbital,
		SpecialModuleRule.FoundSurveillanceRing
	};
}
