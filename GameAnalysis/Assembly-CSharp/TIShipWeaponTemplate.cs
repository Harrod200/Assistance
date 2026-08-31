using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

// Token: 0x020003DB RID: 987
public abstract class TIShipWeaponTemplate : TIShipPartTemplate
{
	// Token: 0x06001301 RID: 4865 RVA: 0x0005A5D6 File Offset: 0x000587D6
	public override float AIScoringValueForResearch()
	{
		return 8f * this.GenericScore();
	}

	// Token: 0x17000262 RID: 610
	// (get) Token: 0x06001302 RID: 4866
	public abstract WeaponClass weaponClass { get; }

	// Token: 0x06001303 RID: 4867
	public abstract float EnergyUsage_GJ(float extraInput_MJ = 0f);

	// Token: 0x06001304 RID: 4868
	public abstract float HeatGeneration_GJ(float extraInput_MJ = 0f);

	// Token: 0x06001305 RID: 4869
	public abstract float chipping(float range_km = 0f);

	// Token: 0x06001306 RID: 4870
	public abstract bool hasMagazine();

	// Token: 0x06001307 RID: 4871 RVA: 0x0005A5E4 File Offset: 0x000587E4
	public virtual bool magazineRequiresResources()
	{
		return this.hasMagazine();
	}

	// Token: 0x17000263 RID: 611
	// (get) Token: 0x06001308 RID: 4872 RVA: 0x0005A5EC File Offset: 0x000587EC
	public virtual bool staticLauncher
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000264 RID: 612
	// (get) Token: 0x06001309 RID: 4873 RVA: 0x0005A5EF File Offset: 0x000587EF
	public virtual bool canBombardThroughAtmosphere
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600130A RID: 4874 RVA: 0x0005A5F2 File Offset: 0x000587F2
	public virtual bool CanOnlyDefensivelyTargetMissiles()
	{
		return false;
	}

	// Token: 0x17000265 RID: 613
	// (get) Token: 0x0600130B RID: 4875 RVA: 0x0005A5F5 File Offset: 0x000587F5
	public virtual bool selfPowered
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600130C RID: 4876
	public abstract float DamageAtRange_MJ(float range_km, float targetCrossSection_m, CombatWeaponCarrierState attacker, float finalVelocity_kps = 0f, float warheadMass_kg = 0f, bool applyChipping = true);

	// Token: 0x0600130D RID: 4877
	public abstract float BaseDamageAtRange_MJ(float range_km, bool applyChipping = true);

	// Token: 0x0600130E RID: 4878
	public abstract Damage GetComplexDamage(float range_km, IDamageableType targetType, float targetCrossSectionalArea_m2, float relativeVelocity_kps = -1f, CombatWeaponCarrierState attacker = null, TIFactionState attackingFaction = null, float warheadMassOverride_kg = -1f);

	// Token: 0x17000266 RID: 614
	// (get) Token: 0x0600130F RID: 4879 RVA: 0x0005A5F8 File Offset: 0x000587F8
	public string combatIconResource
	{
		get
		{
			return this.iconResource;
		}
	}

	// Token: 0x06001310 RID: 4880
	public abstract float EffectiveRangeAgainstProjectiles_km();

	// Token: 0x06001311 RID: 4881 RVA: 0x0005A600 File Offset: 0x00058800
	public float GetGffectiveRange_km(IDamageable target)
	{
		if (target.damageableType != IDamageableType.BallisticProjectile && target.damageableType != IDamageableType.Missile)
		{
			return this.targetingRange_km;
		}
		return this.EffectiveRangeAgainstProjectiles_km();
	}

	// Token: 0x17000267 RID: 615
	// (get) Token: 0x06001312 RID: 4882 RVA: 0x0005A621 File Offset: 0x00058821
	public override TIShipWeaponTemplate ref_weapon
	{
		get
		{
			return this;
		}
	}

	// Token: 0x17000268 RID: 616
	// (get) Token: 0x06001313 RID: 4883 RVA: 0x0005A624 File Offset: 0x00058824
	public override bool isWeapon
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x0005A627 File Offset: 0x00058827
	protected virtual List<FireMode> GetAllowedFireModes()
	{
		return new List<FireMode>
		{
			FireMode.Offense,
			FireMode.Defense,
			FireMode.Guardian,
			FireMode.Focus
		};
	}

	// Token: 0x17000269 RID: 617
	// (get) Token: 0x06001315 RID: 4885 RVA: 0x0005A64C File Offset: 0x0005884C
	public virtual FireMode DefaultFireMode
	{
		get
		{
			if (this.attackMode && this.defenseMode && this.GetActualFireModes(false).Contains(FireMode.Guardian))
			{
				return FireMode.Guardian;
			}
			if (this.attackMode && this.defenseMode && this.GetActualFireModes(false).Contains(FireMode.Offense))
			{
				return FireMode.Offense;
			}
			if (this.attackMode && this.defenseMode)
			{
				return FireMode.Focus;
			}
			if (!this.attackMode)
			{
				return FireMode.Defense;
			}
			if (this.GetActualFireModes(false).Contains(FireMode.Offense))
			{
				return FireMode.Offense;
			}
			return FireMode.Focus;
		}
	}

	// Token: 0x06001316 RID: 4886
	public abstract DamageType GetDamageType();

	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06001317 RID: 4887 RVA: 0x0005A6C9 File Offset: 0x000588C9
	public float averageCooldown_s
	{
		get
		{
			if (this.salvo_shots > 1)
			{
				return (this.cooldown_s + this.intraSalvoCooldown_s * (float)(this.salvo_shots - 1)) / (float)this.salvo_shots;
			}
			return this.cooldown_s;
		}
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x0005A6FA File Offset: 0x000588FA
	public virtual float EstimateChanceToHit(float range_km, TISpaceShipState targetState = null, TISpaceShipTemplate targetTemplate = null, float overrideTargetAcceleration_mps2 = -1f)
	{
		return 1f;
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x0005A704 File Offset: 0x00058904
	public virtual float EstimateDPS(float expectedRange_km, TISpaceShipTemplate target = null, bool applyOverkillPenalty = true)
	{
		float num = ((this.targetingRange_km >= expectedRange_km) ? (this.BaseDamageAtRange_points(expectedRange_km, false) * 0.3f) : 0f) + ((this.targetingRange_km >= 200f) ? (this.BaseDamageAtRange_points(200f, false) * 0.15f) : 0f) + ((this.targetingRange_km >= 500f) ? (this.BaseDamageAtRange_points(500f, false) * 0.25f) : 0f) + ((this.targetingRange_km >= 800f) ? (this.BaseDamageAtRange_points(800f, false) * 0.3f) : 0f);
		if (applyOverkillPenalty)
		{
			num = this.ApplyOverkillPenalty(num);
		}
		float num2 = this.EstimateChanceToHit(expectedRange_km, null, target, -1f);
		float num3 = 1f;
		if (this.isPointDefenseTargetable)
		{
			num3 = 0.25f;
		}
		return num * num2 * num3 / this.averageCooldown_s;
	}

	// Token: 0x0600131A RID: 4890 RVA: 0x0005A7E0 File Offset: 0x000589E0
	protected float ApplyOverkillPenalty(float damage)
	{
		float num = 100f;
		if (base.isAlien)
		{
			num /= 1.5f;
		}
		if (damage > num)
		{
			damage = num + Mathf.Pow(Mathf.Min(1000f, damage) - num, 0.5f);
		}
		return damage;
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x0005A824 File Offset: 0x00058A24
	public float ScoreForRole(ShipRole role)
	{
		float num;
		if (this._combatScoresForRoles.TryGetValue(role, out num))
		{
			return num;
		}
		float expectedCombatRange_km = role.GetExpectedCombatRange_km();
		float num2 = this.EstimateDPS(expectedCombatRange_km, null, true) * 55f;
		if (this.weaponClass == WeaponClass.Laser && this.attackMode)
		{
			num2 = Mathf.Max(1f, num2 * 0.75f);
		}
		if (this.weaponClass == WeaponClass.Missile && this.ref_missileWeapon.warheadClass == WarheadClass.ShapedNuclear)
		{
			num2 /= this.ref_missileWeapon.shapedChargeAngle;
		}
		return this._combatScoresForRoles[role] = num2;
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x0005A8B4 File Offset: 0x00058AB4
	public float GetCuratedDesignScore(ShipRole role, IEnumerable<TIShipWeaponTemplate> allOptions, bool willUseRandomWeightedSelection)
	{
		TIShipWeaponTemplate.<>c__DisplayClass54_0 CS$<>8__locals1 = new TIShipWeaponTemplate.<>c__DisplayClass54_0();
		CS$<>8__locals1.role = role;
		float num = this.ScoreForRole(CS$<>8__locals1.role);
		bool flag = allOptions.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isMagneticGunWeapon);
		bool flag2 = allOptions.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isLaserWeapon);
		if (flag && this.isGunTypeWeapon)
		{
			IEnumerable<TIShipWeaponTemplate> enumerable = allOptions.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isGunTypeWeapon);
			CS$<>8__locals1.bestGunScore = num;
			float num2 = num;
			foreach (TIShipWeaponTemplate tishipWeaponTemplate in enumerable)
			{
				float num3 = tishipWeaponTemplate.ScoreForRole(CS$<>8__locals1.role);
				CS$<>8__locals1.bestGunScore = Mathf.Max(CS$<>8__locals1.bestGunScore, num3);
				num2 += num3;
			}
			num = CS$<>8__locals1.<GetCuratedDesignScore>g__GetAdjustedGunScore|3(num);
			if (willUseRandomWeightedSelection)
			{
				float num4 = enumerable.Sum<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => base.<GetCuratedDesignScore>g__GetAdjustedGunScore|3(x.ScoreForRole(CS$<>8__locals1.role)));
				num *= num2 / num4;
			}
		}
		if (this.attackMode)
		{
			switch (CS$<>8__locals1.role)
			{
			case ShipRole.LS_Penetrator:
			case ShipRole.MS_Strike:
			case ShipRole.SS_Interceptor:
				if (this.isLaserWeapon || (this.isNavalGunWeapon && !flag2 && !flag))
				{
					num *= 10000000f;
				}
				else
				{
					num /= 10000000f;
				}
				break;
			case ShipRole.LM_Protector:
				if (!this.guardianMode || this.isMissileWeapon)
				{
					num /= 10000000f;
				}
				break;
			case ShipRole.LM_Interdictor:
			case ShipRole.MM_SpaceSuperiority:
			case ShipRole.SM_Patrol:
				if (this.isMagneticGunWeapon || this.isParticleWeapon)
				{
					num *= 10000000f;
				}
				else
				{
					num /= 10000000f;
				}
				break;
			case ShipRole.LL_Intruder:
			case ShipRole.ML_Standoff:
			case ShipRole.SL_Defender:
				if (this.isMissileWeapon || this.isPlasmaWeapon)
				{
					num *= 10000000f;
				}
				else
				{
					num /= 10000000f;
				}
				break;
			case ShipRole.LL_Bomber:
				if (this.attackMode && this.bombardmentValue > 0f && !this.isMissileWeapon)
				{
					num *= Mathf.Max(1f, this.bombardmentValue * this.bombardmentValue);
				}
				else
				{
					num /= 10000000f;
				}
				break;
			}
		}
		return num;
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x0005AB18 File Offset: 0x00058D18
	public float GenericScore()
	{
		if (this._combatScore <= 0f)
		{
			this._combatScore = (this.ScoreForRole(ShipRole.LL_Intruder) + this.ScoreForRole(ShipRole.MM_SpaceSuperiority) + this.ScoreForRole(ShipRole.SS_Interceptor)) / 3f;
		}
		return this._combatScore;
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x0005AB53 File Offset: 0x00058D53
	public bool IsValidRefitPart(TIShipWeaponTemplate oldWeapon)
	{
		return this.weaponClass == oldWeapon.weaponClass && this.noseWeapon == oldWeapon.noseWeapon && this.internalSize == oldWeapon.internalSize;
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x0005AB84 File Offset: 0x00058D84
	public virtual DamageBreakdown DamageAtRange_points(float range_km, float defenderCrossSectionalArea_m2, CombatWeaponCarrierState attacker = null, float finalVelocity_kps = 0f, float warheadMass_kg = 0f, bool applyChipping = true)
	{
		float num = this.DamageAtRange_MJ(range_km, defenderCrossSectionalArea_m2, attacker, finalVelocity_kps, warheadMass_kg, false) / 20f;
		if (applyChipping)
		{
			float num2 = this.chipping(range_km);
			return new DamageBreakdown(num * (1f - num2), num * num2);
		}
		return new DamageBreakdown(num, 0f);
	}

	// Token: 0x06001320 RID: 4896 RVA: 0x0005ABCF File Offset: 0x00058DCF
	public virtual float BaseDamageAtRange_points(float range_km, bool applyChipping = true)
	{
		return this.BaseDamageAtRange_MJ(range_km, applyChipping) / 20f;
	}

	// Token: 0x06001321 RID: 4897 RVA: 0x0005ABDF File Offset: 0x00058DDF
	public float GetLocalBombardmentValue(TISpaceBodyState spaceBody)
	{
		if (this.bombardmentValue <= 0f || (!this.canBombardThroughAtmosphere && (spaceBody == null || spaceBody.restrictsOrbitalBombardment)))
		{
			return 0f;
		}
		return this.GenericScore();
	}

	// Token: 0x06001322 RID: 4898 RVA: 0x0005AC10 File Offset: 0x00058E10
	public float GetLocalBombardmentValue(TISpaceBodyState spaceBody, float range_km)
	{
		if (this.bombardmentValue <= 0f || (!this.canBombardThroughAtmosphere && (spaceBody == null || spaceBody.restrictsOrbitalBombardment)))
		{
			return 0f;
		}
		if (range_km <= TemplateManager.global.lowBombardmentAltitude_km)
		{
			return this.ScoreForRole(ShipRole.SS_Interceptor);
		}
		if (range_km <= TemplateManager.global.medBombardmentAltitude_km)
		{
			return this.ScoreForRole(ShipRole.MM_SpaceSuperiority);
		}
		return this.ScoreForRole(ShipRole.LL_Intruder);
	}

	// Token: 0x1700026B RID: 619
	// (get) Token: 0x06001323 RID: 4899 RVA: 0x0005AC78 File Offset: 0x00058E78
	public override int internalSize
	{
		get
		{
			switch (this.mount)
			{
			case Mount.TwoHullHoriz:
			case Mount.TwoHullVert:
			case Mount.TwoNoseHoriz:
			case Mount.TwoNoseVert:
				return 2;
			case Mount.ThreeHullHoriz:
			case Mount.ThreeNoseAngle:
				return 3;
			case Mount.FourHull:
			case Mount.FourNose:
				return 4;
			}
			return 1;
		}
	}

	// Token: 0x1700026C RID: 620
	// (get) Token: 0x06001324 RID: 4900 RVA: 0x0005ACCD File Offset: 0x00058ECD
	public override List<ShipModuleSlotType> allowedSlots
	{
		get
		{
			if (this.hullWeapon)
			{
				return new List<ShipModuleSlotType> { ShipModuleSlotType.HullHardPoint };
			}
			if (!this.noseWeapon)
			{
				return new List<ShipModuleSlotType>();
			}
			return new List<ShipModuleSlotType> { ShipModuleSlotType.NoseHardPoint };
		}
	}

	// Token: 0x1700026D RID: 621
	// (get) Token: 0x06001325 RID: 4901 RVA: 0x0005ACFF File Offset: 0x00058EFF
	public bool guardianMode
	{
		get
		{
			return this.attackMode && this.defenseMode;
		}
	}

	// Token: 0x1700026E RID: 622
	// (get) Token: 0x06001326 RID: 4902 RVA: 0x0005AD14 File Offset: 0x00058F14
	public bool noseWeapon
	{
		get
		{
			Mount mount = this.mount;
			return mount == Mount.HalfNose || mount - Mount.OneNose <= 4;
		}
	}

	// Token: 0x1700026F RID: 623
	// (get) Token: 0x06001327 RID: 4903 RVA: 0x0005AD38 File Offset: 0x00058F38
	public bool hullWeapon
	{
		get
		{
			Mount mount = this.mount;
			return mount - Mount.HalfHull <= 5;
		}
	}

	// Token: 0x17000270 RID: 624
	// (get) Token: 0x06001328 RID: 4904 RVA: 0x0005AD58 File Offset: 0x00058F58
	public bool shipWeapon
	{
		get
		{
			Mount mount = this.mount;
			return mount - Mount.HalfNose <= 11;
		}
	}

	// Token: 0x17000271 RID: 625
	// (get) Token: 0x06001329 RID: 4905 RVA: 0x0005AD78 File Offset: 0x00058F78
	public bool multiSlot
	{
		get
		{
			Mount mount = this.mount;
			return mount - Mount.TwoHullHoriz <= 3 || mount - Mount.TwoNoseHoriz <= 3;
		}
	}

	// Token: 0x17000272 RID: 626
	// (get) Token: 0x0600132A RID: 4906 RVA: 0x0005AD9C File Offset: 0x00058F9C
	public override bool exoFighterPart
	{
		get
		{
			return this.fighterOnlyWeapon;
		}
	}

	// Token: 0x17000273 RID: 627
	// (get) Token: 0x0600132B RID: 4907 RVA: 0x0005ADA4 File Offset: 0x00058FA4
	public bool fighterOnlyWeapon
	{
		get
		{
			Mount mount = this.mount;
			return mount - Mount.HalfNose <= 1;
		}
	}

	// Token: 0x0600132C RID: 4908
	public abstract string SpecificDescriptionData();

	// Token: 0x0600132D RID: 4909 RVA: 0x0005ADC4 File Offset: 0x00058FC4
	public List<FireMode> GetActualFireModes(bool includeIdle = false)
	{
		List<FireMode> allowedFireModes = this.GetAllowedFireModes();
		List<FireMode> list = new List<FireMode>();
		if (includeIdle)
		{
			list.Add(FireMode.Idle);
		}
		if (this.attackMode)
		{
			if (allowedFireModes.Contains(FireMode.Focus))
			{
				list.Add(FireMode.Focus);
				if (this.isMissileWeapon)
				{
					list.Add(FireMode.Salvo);
				}
			}
			if (allowedFireModes.Contains(FireMode.Offense))
			{
				list.Add(FireMode.Offense);
				if (this.defenseMode)
				{
					list.Add(FireMode.Guardian);
				}
				if (this.isGunTypeWeapon && !this.isPlasmaWeapon)
				{
					list.Add(FireMode.Bracket);
				}
			}
		}
		if (this.defenseMode)
		{
			list.Add(FireMode.Defense);
		}
		return list;
	}

	// Token: 0x0600132E RID: 4910 RVA: 0x0005AE58 File Offset: 0x00059058
	public override string GetDescriptionData(TISpaceShipState ship = null, TISpaceShipTemplate shipTemplate = null, bool prospective = false, ShipModuleSlotType slot = ShipModuleSlotType.None, bool splitFireModes = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (this.isParticleWeapon && this.defenseMode && this.CanOnlyDefensivelyTargetMissiles())
		{
			stringBuilder.AppendLine(Loc.T("TIParticleWeaponTemplate.Defense.MissilesOnly"));
		}
		else if (this.isParticleWeapon && this.defenseMode && !this.CanOnlyDefensivelyTargetMissiles() && this.ref_particleWeapon.heatFraction < 1f)
		{
			stringBuilder.AppendLine(Loc.T("TIParticleWeaponTemplate.Defense.ReducedEffectiveness"));
		}
		stringBuilder.AppendLine(this.GetLocalizedMountType());
		stringBuilder.AppendLine(this.GetLocalizedMass(prospective ? null : shipTemplate));
		if (this.crew > 0)
		{
			stringBuilder.AppendLine(base.GetLocalizedCrew());
		}
		if (splitFireModes)
		{
			stringBuilder.AppendLine(this.GetLocalizedSplitFireModes());
		}
		else
		{
			stringBuilder.AppendLine(this.GetLocalizedFireModes());
		}
		stringBuilder.Append(this.SpecificDescriptionData());
		if (this.salvo_shots > 1)
		{
			stringBuilder.AppendLine(this.GetCombinedSalvoData());
		}
		if (this.targetingRange_km > 0f)
		{
			stringBuilder.AppendLine(this.GetLocalizedTargetingRange());
		}
		if (this.defenseMode && this.EffectiveRangeAgainstProjectiles_km() != this.targetingRange_km)
		{
			stringBuilder.AppendLine(this.GetLocalizedDefenseTargetingRange(shipTemplate ?? ((ship != null) ? ship.template : null) ?? null));
		}
		if (this.EnergyUsage_GJ(0f) > 0f)
		{
			stringBuilder.AppendLine(this.GetLocalizedEnergyUsage());
		}
		stringBuilder.AppendLine(this.GetLocalizedCost());
		if (this.hasMagazine())
		{
			stringBuilder.AppendLine(this.GetLocalizedMagazineMaxAmmoCount(prospective ? null : shipTemplate));
		}
		if (this.magazineRequiresResources())
		{
			stringBuilder.AppendLine(this.GetLocalizedMagazineCost(prospective ? null : shipTemplate));
		}
		stringBuilder.AppendLine(this.GetLocalizedBombardmentDetail());
		if (this.isMissileWeapon && this.ref_missileWeapon.AOEWeapon)
		{
			stringBuilder.AppendLine(Loc.T("TIMissileTemplate.NoSalvage"));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600132F RID: 4911 RVA: 0x0005B03C File Offset: 0x0005923C
	public string GetTruncatedDescriptionData(TISpaceShipState ship = null, TISpaceShipTemplate shipTemplate = null, bool prospective = false, ShipModuleSlotType slot = ShipModuleSlotType.None)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(this.GetLocalizedMountType());
		stringBuilder.AppendLine(this.GetLocalizedPowerAndDamageAtRange(500f));
		if (this.targetingRange_km > 0f)
		{
			stringBuilder.AppendLine(this.GetLocalizedTargetingRange());
		}
		if (this.isMissileWeapon)
		{
			if (!this.fighterOnlyWeapon)
			{
				stringBuilder.AppendLine(this.ref_missileWeapon.GetLocalizedDV());
			}
			if (this.ref_missileWeapon.warheadClass == WarheadClass.ShapedNuclear)
			{
				stringBuilder.AppendLine(this.ref_missileWeapon.GetLocalizedShapeChargeAngle());
			}
			if (this.ref_missileWeapon.warheadClass == WarheadClass.ShapedNuclear || this.ref_missileWeapon.warheadClass == WarheadClass.Nuclear || this.ref_missileWeapon.warheadClass == WarheadClass.Antimatter)
			{
				stringBuilder.AppendLine(this.ref_missileWeapon.GetLocalizedEffectiveAOE());
			}
			stringBuilder.AppendLine(this.ref_projectileWeapon.GetLocalizedMagazineMaxAmmoCount(null));
		}
		if (this.defenseMode && this.EffectiveRangeAgainstProjectiles_km() != this.targetingRange_km)
		{
			stringBuilder.AppendLine(this.GetLocalizedDefenseTargetingRange(shipTemplate ?? ((ship != null) ? ship.template : null) ?? null));
		}
		stringBuilder.AppendLine(this.GetLocalizedCost());
		return stringBuilder.ToString();
	}

	// Token: 0x06001330 RID: 4912 RVA: 0x0005B168 File Offset: 0x00059368
	public string GetLocalizedMountType()
	{
		string text = (this.multiSlot ? "UI.Fleets.WeaponMountDataMult" : "UI.Fleets.WeaponMountDataOne");
		string text2 = (this.noseWeapon ? Loc.T("UI.Fleets.Nose") : Loc.T("UI.Fleets.Hull"));
		return Loc.T(text, new object[]
		{
			text2,
			this.internalSize.ToString("N0")
		});
	}

	// Token: 0x06001331 RID: 4913 RVA: 0x0005B1D0 File Offset: 0x000593D0
	public string GetLocalizedMass(TISpaceShipTemplate shipTemplate = null)
	{
		float num = this.buildMass_tons((shipTemplate != null) ? shipTemplate.magazineModuleMultiplier : 0f, 0f, 0f, 0f, false);
		return Loc.T("UI.Fleets.Mass", new object[] { TIUtilities.FormatBigOrSmallNumber(num, 1, 7, 0, false, false) });
	}

	// Token: 0x06001332 RID: 4914 RVA: 0x0005B224 File Offset: 0x00059424
	public string GetLocalizedFireModes()
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<FireMode> actualFireModes = this.GetActualFireModes(false);
		for (int i = 0; i < actualFireModes.Count; i++)
		{
			stringBuilder.Append(Loc.T("UI.Fleets." + actualFireModes[i].ToString()));
			if (i < actualFireModes.Count - 1)
			{
				stringBuilder.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
		}
		return Loc.T("UI.Fleets.FireModes", new object[] { stringBuilder.ToString() });
	}

	// Token: 0x06001333 RID: 4915 RVA: 0x0005B2B0 File Offset: 0x000594B0
	public string GetLocalizedSplitFireModes()
	{
		List<FireMode> actualFireModes = this.GetActualFireModes(false);
		if (actualFireModes.Count < 4)
		{
			return this.GetLocalizedFireModes();
		}
		int num = (actualFireModes.Count + 1) / 2;
		StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Fleets.ModuleTable.FireModes")).Append(Loc.T("UI.Nation.RelationsFeedback", new object[] { string.Empty }).Trim());
		stringBuilder.Append("<line-height=0.01%>").Append(Environment.NewLine).Append("<align=\"right\">");
		for (int i = 0; i < num; i++)
		{
			stringBuilder.Append(Loc.T("UI.Fleets." + actualFireModes[i].ToString()));
			if (i < actualFireModes.Count - 1)
			{
				stringBuilder.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
		}
		stringBuilder.Append("</align></line-height>").Append(Environment.NewLine).Append("<align=\"right\">");
		for (int j = num; j < actualFireModes.Count; j++)
		{
			stringBuilder.Append(Loc.T("UI.Fleets." + actualFireModes[j].ToString()));
			if (j < actualFireModes.Count - 1)
			{
				stringBuilder.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
		}
		stringBuilder.Append("</align>");
		return stringBuilder.ToString();
	}

	// Token: 0x06001334 RID: 4916 RVA: 0x0005B418 File Offset: 0x00059618
	public virtual string GetLocalizedTargetingRange()
	{
		if (this.defenseMode && !this.attackMode)
		{
			return Loc.T("TIWeaponTemplate.DefenseTargetingRange", new object[] { this.targetingRange_km.ToString("N0") });
		}
		return Loc.T("TIWeaponTemplate.TargetingRange", new object[] { this.targetingRange_km.ToString("N0") });
	}

	// Token: 0x06001335 RID: 4917 RVA: 0x0005B47C File Offset: 0x0005967C
	public virtual string GetLocalizedDefenseTargetingRange(TISpaceShipTemplate template = null)
	{
		return Loc.T("TIWeaponTemplate.DefenseTargetingRange", new object[] { this.EffectiveRangeAgainstProjectiles_km().ToString("N0") });
	}

	// Token: 0x06001336 RID: 4918 RVA: 0x0005B4B0 File Offset: 0x000596B0
	public string GetLocalizedEnergyUsage()
	{
		return Loc.T("TIWeaponTemplate.EnergyUsageGJ", new object[] { TIUtilities.FormatBigOrSmallNumber(this.EnergyUsage_GJ(0f), 1, 7, 0, false, false) });
	}

	// Token: 0x06001337 RID: 4919 RVA: 0x0005B4E5 File Offset: 0x000596E5
	public string GetLocalizedMagazineMaxAmmoCount(TISpaceShipTemplate shipTemplate = null)
	{
		return Loc.T("TIProjectileWeaponTemplate.Magazine", new object[] { this.ref_projectileWeapon.FullAmmoCount_Max(shipTemplate) });
	}

	// Token: 0x06001338 RID: 4920 RVA: 0x0005B50C File Offset: 0x0005970C
	public string GetLocalizedMagazineCost(TISpaceShipTemplate shipTemplate = null)
	{
		float num = ((shipTemplate != null) ? shipTemplate.magazineModuleMultiplier : 0f);
		TIResourcesCost tiresourcesCost = this.ref_projectileWeapon.magazineCost(num);
		return Loc.T("UI.Fleets.MagazineCost", new object[] { tiresourcesCost.ToString("Relevant", false, false, null, false, FactionResource.None) });
	}

	// Token: 0x06001339 RID: 4921 RVA: 0x0005B55C File Offset: 0x0005975C
	public string GetLocalizedCooldown()
	{
		if (this.salvo_shots <= 1)
		{
			return Loc.T("TIWeaponTemplate.Cooldown", new object[] { TIUtilities.FormatSmallNumber(this.cooldown_s, 7, 0, true, false) });
		}
		return Loc.T("TIWeaponTemplate.CooldownBetweenSalvos", new object[] { TIUtilities.FormatSmallNumber(this.cooldown_s, 7, 0, true, false) });
	}

	// Token: 0x0600133A RID: 4922 RVA: 0x0005B5B8 File Offset: 0x000597B8
	public string GetCombinedSalvoData()
	{
		return Loc.T("TIWeaponTemplate.CombinedSalvoData", new object[]
		{
			this.salvo_shots.ToString(),
			TIUtilities.FormatSmallNumber((float)(this.salvo_shots - 1) * this.intraSalvoCooldown_s, 7, 0, true, false)
		});
	}

	// Token: 0x0600133B RID: 4923 RVA: 0x0005B5FF File Offset: 0x000597FF
	public string GetLocalizedSalvoShotCount()
	{
		return Loc.T("TIWeaponTemplate.SalvoShotCount", new object[] { this.salvo_shots.ToString() });
	}

	// Token: 0x0600133C RID: 4924 RVA: 0x0005B620 File Offset: 0x00059820
	public string GetLocalizedSalvoCooldown()
	{
		return Loc.T("TIWeaponTemplate.SalvoCooldown", new object[] { TIUtilities.FormatSmallNumber(this.intraSalvoCooldown_s, 7, 0, true, false) });
	}

	// Token: 0x0600133D RID: 4925
	public abstract string GetLocalizedPowerAndDamageAtRange(float range);

	// Token: 0x0600133E RID: 4926 RVA: 0x0005B650 File Offset: 0x00059850
	public string GetLocalizedBombardmentDetail()
	{
		if (this.bombardmentValue <= 0f)
		{
			return Loc.T("UI.Fleets.Bombardment", new object[] { Loc.T("UI.Fleets.Bombardment_No") });
		}
		if (this.canBombardThroughAtmosphere)
		{
			return Loc.T("UI.Fleets.Bombardment", new object[] { Loc.T("UI.Fleets.Bombardment_Yes") });
		}
		return Loc.T("UI.Fleets.Bombardment", new object[] { Loc.T("UI.Fleets.Bombardment_NoAtmo") });
	}

	// Token: 0x04001162 RID: 4450
	public Mount mount;

	// Token: 0x04001163 RID: 4451
	public bool attackMode;

	// Token: 0x04001164 RID: 4452
	public bool defenseMode;

	// Token: 0x04001165 RID: 4453
	public float baseWeaponMass_tons;

	// Token: 0x04001166 RID: 4454
	public float cooldown_s;

	// Token: 0x04001167 RID: 4455
	public int salvo_shots = 1;

	// Token: 0x04001168 RID: 4456
	public float intraSalvoCooldown_s;

	// Token: 0x04001169 RID: 4457
	public float efficiency;

	// Token: 0x0400116A RID: 4458
	public float bombardmentValue;

	// Token: 0x0400116B RID: 4459
	public float flatDamage_MJ;

	// Token: 0x0400116C RID: 4460
	public float targetingRange_km;

	// Token: 0x0400116D RID: 4461
	public float pivotRange_deg;

	// Token: 0x0400116E RID: 4462
	public bool isPointDefenseTargetable;

	// Token: 0x0400116F RID: 4463
	protected float _combatScore = -1f;

	// Token: 0x04001170 RID: 4464
	protected Dictionary<ShipRole, float> _combatScoresForRoles = new Dictionary<ShipRole, float>();

	// Token: 0x04001171 RID: 4465
	public string effectResource;

	// Token: 0x04001172 RID: 4466
	public string fireSoundFXResource;

	// Token: 0x04001173 RID: 4467
	protected const float MIN_PD_TARGETING_RANGE = 200f;
}
