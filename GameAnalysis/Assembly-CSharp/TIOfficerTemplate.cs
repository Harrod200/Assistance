using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200037B RID: 891
public class TIOfficerTemplate : TIDataTemplate
{
	// Token: 0x06001012 RID: 4114 RVA: 0x000533F0 File Offset: 0x000515F0
	public override TIGameState CreateGameState()
	{
		return GameStateManager.CreateNewGameState<TIOfficerState>();
	}

	// Token: 0x06001013 RID: 4115 RVA: 0x000533F8 File Offset: 0x000515F8
	public List<OfficerEffect> GetOfficerEffectsByLevel(int level)
	{
		if (this._cachedEffectsByLevel == null)
		{
			this._cachedEffectsByLevel = new Dictionary<int, List<OfficerEffect>>();
			for (int i = 1; i <= 3; i++)
			{
				this._cachedEffectsByLevel.Add(i, new List<OfficerEffect>());
				foreach (OfficerEffect officerEffect in this.effects)
				{
					if (officerEffect.level == i)
					{
						this._cachedEffectsByLevel[i].Add(officerEffect);
					}
				}
			}
		}
		return this._cachedEffectsByLevel[level];
	}

	// Token: 0x06001014 RID: 4116 RVA: 0x0005349C File Offset: 0x0005169C
	public bool OfficerTypeAllowedForShip(TISpaceShipState candidateShip, bool swap, int additionalProposedTransfersToShip)
	{
		return this.OfficerTypeAllowedForShipFailReasons(candidateShip, swap, additionalProposedTransfersToShip).Count == 0;
	}

	// Token: 0x06001015 RID: 4117 RVA: 0x000534B0 File Offset: 0x000516B0
	public int MaxOfficersofTypeAllowedForShip()
	{
		if (!this.requirements.Any<OfficerRequirement>((OfficerRequirement x) => x.requirement == OfficerRequirementType.MaxPerShip))
		{
			return int.MaxValue;
		}
		return (int)this.requirements.FirstOrDefault<OfficerRequirement>((OfficerRequirement x) => x.requirement == OfficerRequirementType.MaxPerShip).value;
	}

	// Token: 0x06001016 RID: 4118 RVA: 0x00053520 File Offset: 0x00051720
	public List<OfficerRequirement> OfficerTypeAllowedForShipFailReasons(TISpaceShipState candidateShip, bool swap, int additionalProposedTransfersToShip)
	{
		List<OfficerRequirement> list = new List<OfficerRequirement>();
		foreach (OfficerRequirement officerRequirement in this.requirements)
		{
			switch (officerRequirement.requirement)
			{
			case OfficerRequirementType.MaxPerShip:
				if (!swap && (float)candidateShip.officers.Count<TIOfficerState>((TIOfficerState x) => x.templateName == base.dataName) >= officerRequirement.value)
				{
					list.Add(officerRequirement);
				}
				break;
			case OfficerRequirementType.MaxTotalOfficersPerShip:
				if (candidateShip.officers.Count<TIOfficerState>((TIOfficerState x) => x.template.requirements.Select<OfficerRequirement, OfficerRequirementType>((OfficerRequirement x) => x.requirement).Contains(OfficerRequirementType.MaxTotalOfficersPerShip)) + additionalProposedTransfersToShip >= candidateShip.hull.maxOfficers)
				{
					list.Add(new OfficerRequirement
					{
						requirement = officerRequirement.requirement,
						value = (float)candidateShip.hull.maxOfficers
					});
				}
				break;
			case OfficerRequirementType.CrewMin:
				if ((float)candidateShip.template.crewBillets < officerRequirement.value)
				{
					list.Add(officerRequirement);
				}
				break;
			case OfficerRequirementType.FlagBridge:
				if (candidateShip.utilityModuleTemplates.None<TIShipModuleTemplate>(delegate(TIShipModuleTemplate x)
				{
					TIUtilityModuleTemplate ref_utilityModule = x.ref_utilityModule;
					return ref_utilityModule != null && ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.ReduceFleetMCConsumption);
				}))
				{
					list.Add(officerRequirement);
				}
				break;
			case OfficerRequirementType.MissileWeapons:
				if (candidateShip.allWeaponTemplates.None<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isMissileWeapon && x.attackMode))
				{
					list.Add(officerRequirement);
				}
				break;
			case OfficerRequirementType.EnergyWeapons:
				if (candidateShip.allWeaponTemplates.None<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isBeamWeapon && x.attackMode))
				{
					list.Add(officerRequirement);
				}
				break;
			case OfficerRequirementType.GunWeapons:
				if (candidateShip.allWeaponTemplates.None<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isGunTypeWeapon && x.attackMode))
				{
					list.Add(officerRequirement);
				}
				break;
			case OfficerRequirementType.DefenseWeapons:
				if (candidateShip.allWeaponTemplates.None<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.defenseMode))
				{
					list.Add(officerRequirement);
				}
				break;
			case OfficerRequirementType.Marines:
				if (candidateShip.utilityModuleTemplates.None<TIShipModuleTemplate>(delegate(TIShipModuleTemplate x)
				{
					TIUtilityModuleTemplate ref_utilityModule2 = x.ref_utilityModule;
					return ref_utilityModule2 != null && ref_utilityModule2.specialModuleRules.Contains(SpecialModuleRule.Assault);
				}))
				{
					list.Add(officerRequirement);
				}
				break;
			case OfficerRequirementType.TunableDrive:
				if (candidateShip.drive.singleThrusterTemplate.thrustCap == 1f)
				{
					list.Add(officerRequirement);
				}
				break;
			}
		}
		return list;
	}

	// Token: 0x06001017 RID: 4119 RVA: 0x00053804 File Offset: 0x00051A04
	public List<OfficerEffect> GetOfficerEffects(OfficerEffectType officerEffectType, int level)
	{
		return (from x in this.GetOfficerEffectsByLevel(level)
			where x.effect == officerEffectType
			select x).ToList<OfficerEffect>();
	}

	// Token: 0x06001018 RID: 4120 RVA: 0x0005383B File Offset: 0x00051A3B
	public string GetRankString(int rank)
	{
		return Loc.T(new StringBuilder("TIOfficerTemplate.").Append(base.dataName).Append("_").Append(rank.ToString("N0"))
			.ToString());
	}

	// Token: 0x06001019 RID: 4121 RVA: 0x00053877 File Offset: 0x00051A77
	public string GetIconPath(int rank)
	{
		return new StringBuilder(this.baseIconPath).Append(rank.ToString("N0")).ToString();
	}

	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x0600101A RID: 4122 RVA: 0x0005389A File Offset: 0x00051A9A
	public string description
	{
		get
		{
			return Loc.T(new StringBuilder("TIOfficerTemplate.description.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x0600101B RID: 4123 RVA: 0x000538BB File Offset: 0x00051ABB
	public string flagOfficerAndRank(int rank)
	{
		return Loc.T("TIOfficerTemplate.OfficerTypeAndRank", new object[]
		{
			this.displayName,
			this.GetRankString(rank)
		});
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x000538E0 File Offset: 0x00051AE0
	public static string RequirementText(OfficerRequirement req, TIShipHullTemplate hull)
	{
		if (req.requirement == OfficerRequirementType.MaxPerShip && hull != null)
		{
			return Loc.T("TIOfficerTemplate.MaxTotalOfficersPerShip_Hull", new object[] { hull.displayName, hull.maxOfficers });
		}
		if (req.requirement == OfficerRequirementType.FlagBridge)
		{
			return Loc.T(new StringBuilder("TIOfficerTemplate.").Append(req.requirement).ToString(), new object[] { TemplateManager.Find<TIUtilityModuleTemplate>("FlagBridge", false).displayName });
		}
		return Loc.T(new StringBuilder("TIOfficerTemplate.").Append(req.requirement).ToString(), new object[] { req.value });
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x000539A0 File Offset: 0x00051BA0
	public string FullDescriptionAtRank(int rank, TIShipHullTemplate hull = null, bool alwaysShowRequirements = false, List<OfficerRequirementType> failReasons = null)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(this.flagOfficerAndRank(rank));
		stringBuilder.AppendLine(this.description);
		if (rank == 1 || alwaysShowRequirements)
		{
			stringBuilder.AppendLine().AppendLine(Loc.T("TIOfficerTemplate.RequirementsHeader"));
			foreach (OfficerRequirement officerRequirement in this.requirements)
			{
				if (failReasons != null && failReasons.Contains(officerRequirement.requirement))
				{
					stringBuilder.AppendLine(TIUtilities.RedLine(TIOfficerTemplate.RequirementText(officerRequirement, hull)));
				}
				else
				{
					stringBuilder.AppendLine(TIOfficerTemplate.RequirementText(officerRequirement, hull));
				}
			}
			stringBuilder.AppendLine().AppendLine(Loc.T("TIOfficerTemplate.PromotionHeader"));
			stringBuilder.AppendLine(Loc.T(new StringBuilder("TIOfficerTemplate.").Append(this.spawnEventType.ToString()).ToString()));
		}
		stringBuilder.AppendLine().AppendLine(Loc.T("TIOfficerTemplate.Bonuses"));
		stringBuilder.Append(this.EffectsAtRankString(rank));
		return stringBuilder.ToString();
	}

	// Token: 0x0600101E RID: 4126 RVA: 0x00053AD8 File Offset: 0x00051CD8
	public string EffectsAtRankString(int rank)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (OfficerEffect officerEffect in this.GetOfficerEffectsByLevel(rank))
		{
			if (officerEffect.effect != OfficerEffectType.none)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				string text = new StringBuilder("TIOfficerTemplate.").Append(officerEffect.effect.ToString()).ToString();
				object[] array = new object[5];
				int num = 0;
				float value = officerEffect.value;
				array[num] = value.ToString();
				array[1] = officerEffect.value.ToPercent("P0");
				array[2] = (1f - officerEffect.value).ToPercent("P0");
				array[3] = (officerEffect.value - 1f).ToPercent("P0");
				array[4] = (-officerEffect.value).ToPercent("P0");
				stringBuilder2.AppendLine(Loc.T(text, array));
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600101F RID: 4127 RVA: 0x00053BE4 File Offset: 0x00051DE4
	public static string BuildOfficerPromotionReport(List<TIOfficerState> promotions, TIFactionState forFaction)
	{
		StringBuilder stringBuilder = new StringBuilder();
		promotions = (from x in promotions
			where x.ref_faction == forFaction
			orderby x.ship.displayName, x.template.sortOrder
			select x).ToList<TIOfficerState>();
		if (promotions.Count > 0)
		{
			stringBuilder.AppendLine(Loc.T("UI.Notifications.OfficerPromoted_Header"));
			foreach (TIOfficerState tiofficerState in promotions)
			{
				stringBuilder.AppendLine(Loc.T("TIOfficerTemplate.OfficerTypeAndRank", new object[]
				{
					tiofficerState.ship.displayName,
					tiofficerState.DisplayNameAndJob
				}));
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x00053CF0 File Offset: 0x00051EF0
	public static string BuildOfficerDeathsReport(List<TIOfficerState> deaths, TIFactionState forFaction)
	{
		StringBuilder stringBuilder = new StringBuilder();
		deaths = (from x in deaths
			where x.ref_faction == forFaction
			orderby x.ship.displayName, x.template.sortOrder
			select x).ToList<TIOfficerState>();
		if (deaths.Count > 0)
		{
			stringBuilder.AppendLine(Loc.T("UI.Precombat.OfficerKilled_Header"));
			foreach (TIOfficerState tiofficerState in deaths)
			{
				stringBuilder.AppendLine(Loc.T("TIOfficerTemplate.OfficerTypeAndRank", new object[]
				{
					tiofficerState.ship.displayName,
					tiofficerState.DisplayNameAndJob
				}));
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04001058 RID: 4184
	public const int MaxOfficerLevel = 3;

	// Token: 0x04001059 RID: 4185
	public OfficerSpawnEventType spawnEventType;

	// Token: 0x0400105A RID: 4186
	public float spawnChance;

	// Token: 0x0400105B RID: 4187
	public string baseIconPath;

	// Token: 0x0400105C RID: 4188
	public List<OfficerRequirement> requirements = new List<OfficerRequirement>();

	// Token: 0x0400105D RID: 4189
	public List<OfficerEffect> effects = new List<OfficerEffect>();

	// Token: 0x0400105E RID: 4190
	public ShipSystem location;

	// Token: 0x0400105F RID: 4191
	public int sortOrder;

	// Token: 0x04001060 RID: 4192
	private Dictionary<int, List<OfficerEffect>> _cachedEffectsByLevel;

	// Token: 0x04001061 RID: 4193
	public static readonly Dictionary<OfficerEffectType, StatModSetOperation> OfficerEffectOperation = new Dictionary<OfficerEffectType, StatModSetOperation>
	{
		{
			OfficerEffectType.none,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.FleetMissionControl,
			StatModSetOperation.Multiplicative
		},
		{
			OfficerEffectType.ShipMissionControl,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.OfficerPromotionChance,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.MissileTargeting,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.MissileDamage,
			StatModSetOperation.Multiplicative
		},
		{
			OfficerEffectType.BeamTargeting,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.BeamDamage,
			StatModSetOperation.Multiplicative
		},
		{
			OfficerEffectType.BeamCooldown,
			StatModSetOperation.Multiplicative
		},
		{
			OfficerEffectType.GunTargeting,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.GunDamage,
			StatModSetOperation.Multiplicative
		},
		{
			OfficerEffectType.MagWeaponCooldown,
			StatModSetOperation.Multiplicative
		},
		{
			OfficerEffectType.PointDefenseCooldown,
			StatModSetOperation.Multiplicative
		},
		{
			OfficerEffectType.GlobalDamage,
			StatModSetOperation.Multiplicative
		},
		{
			OfficerEffectType.DamageControlSpeed,
			StatModSetOperation.Multiplicative
		},
		{
			OfficerEffectType.InternalDamageTaken,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.DockRepairSpeed,
			StatModSetOperation.Multiplicative
		},
		{
			OfficerEffectType.DockResupplySpeed,
			StatModSetOperation.Multiplicative
		},
		{
			OfficerEffectType.Salvage,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.MaxTrajectoryDuration_Months,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.GlobalTargeting,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.GlobalWeaponCooldown,
			StatModSetOperation.Multiplicative
		},
		{
			OfficerEffectType.ECM,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.OfficerDeathChance,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.AssaultCombatValue,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.ShipDefectionChance,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.DriveCombatThrustMultiplier,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.MaxSurvivableCombatAcceleration,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.MaxSurvivableCruiseAcceleration,
			StatModSetOperation.Additive
		},
		{
			OfficerEffectType.RadiationDamageReduction,
			StatModSetOperation.Multiplicative
		}
	};
}
