using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

// Token: 0x020003DC RID: 988
public abstract class TIBeamWeaponTemplate : TIShipWeaponTemplate
{
	// Token: 0x06001340 RID: 4928 RVA: 0x0005B6F0 File Offset: 0x000598F0
	public override bool hasMagazine()
	{
		return false;
	}

	// Token: 0x17000274 RID: 628
	// (get) Token: 0x06001341 RID: 4929 RVA: 0x0005B6F3 File Offset: 0x000598F3
	public override bool isBeamWeapon
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000275 RID: 629
	// (get) Token: 0x06001342 RID: 4930 RVA: 0x0005B6F6 File Offset: 0x000598F6
	public override TIBeamWeaponTemplate ref_beamWeapon
	{
		get
		{
			return this;
		}
	}

	// Token: 0x17000276 RID: 630
	// (get) Token: 0x06001343 RID: 4931 RVA: 0x0005B6F9 File Offset: 0x000598F9
	public float shortRange
	{
		get
		{
			return 200f;
		}
	}

	// Token: 0x17000277 RID: 631
	// (get) Token: 0x06001344 RID: 4932 RVA: 0x0005B700 File Offset: 0x00059900
	public float mediumRange
	{
		get
		{
			return 400f;
		}
	}

	// Token: 0x17000278 RID: 632
	// (get) Token: 0x06001345 RID: 4933 RVA: 0x0005B707 File Offset: 0x00059907
	public float longRange
	{
		get
		{
			return 600f;
		}
	}

	// Token: 0x06001346 RID: 4934 RVA: 0x0005B70E File Offset: 0x0005990E
	public override float EnergyUsage_GJ(float extraInput_MJ = 0f)
	{
		return ((float)this.shotPower_MJ + extraInput_MJ) / this.efficiency / 1000f;
	}

	// Token: 0x06001347 RID: 4935 RVA: 0x0005B726 File Offset: 0x00059926
	public override float HeatGeneration_GJ(float extraInput_MJ = 0f)
	{
		return this.EnergyUsage_GJ(extraInput_MJ) * (1f - this.efficiency);
	}

	// Token: 0x06001348 RID: 4936 RVA: 0x0005B73C File Offset: 0x0005993C
	public override float buildMass_tons(float value1 = 0f, float value2 = 0f, float value3 = 0f, float value4 = 0f, bool bValue = false)
	{
		return this.baseWeaponMass_tons;
	}

	// Token: 0x06001349 RID: 4937 RVA: 0x0005B744 File Offset: 0x00059944
	public override TIResourcesCost buildCost(float value = 0f, float value2 = 0f)
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		tiresourcesCost.SumCosts_NoDuration(this.weightedBuildMaterials.ToResourcesCost(this.buildMass_tons(value, value2, 0f, 0f, false) * TemplateManager.global.spaceResourceToTons));
		return tiresourcesCost;
	}

	// Token: 0x0600134A RID: 4938 RVA: 0x0005B785 File Offset: 0x00059985
	public string GetLocalizedShotPower()
	{
		return Loc.T("TIBeamWeaponTemplate.PowerOnly", new object[] { this.shotPower_MJ.ToString("N1") });
	}

	// Token: 0x0600134B RID: 4939 RVA: 0x0005B7AC File Offset: 0x000599AC
	public override string GetLocalizedTargetingRange()
	{
		if (this.defenseMode && !this.attackMode)
		{
			return Loc.T("TIWeaponTemplate.DefenseTargetingRange", new object[] { Mathf.Min(this.EffectiveRangeAgainstProjectiles_km(), this.RangeToDoDamage_km(TemplateManager.global.DP_DestroyMissile, null)).ToString("N0") });
		}
		return Loc.T("TIWeaponTemplate.TargetingRange", new object[] { this.targetingRange_km.ToString("N0") });
	}

	// Token: 0x0600134C RID: 4940 RVA: 0x0005B82C File Offset: 0x00059A2C
	public override string GetLocalizedDefenseTargetingRange(TISpaceShipTemplate template = null)
	{
		if (template == null)
		{
			return base.GetLocalizedDefenseTargetingRange(template);
		}
		return Loc.T("TIWeaponTemplate.DefenseTargetingRange", new object[] { Mathf.Min(this.EffectiveRangeAgainstProjectiles_km(), this.RangeToDoDamage_km(TemplateManager.global.DP_DestroyMissile, null)).ToString("N0") });
	}

	// Token: 0x0600134D RID: 4941
	public abstract float RangeToDoDamage_km(float desiredDamage, TISpaceShipState ship);

	// Token: 0x0600134E RID: 4942 RVA: 0x0005B880 File Offset: 0x00059A80
	public override float EffectiveRangeAgainstProjectiles_km()
	{
		return Mathf.Max(200f, this.attackMode ? (this.targetingRange_km / 3f) : this.targetingRange_km);
	}

	// Token: 0x0600134F RID: 4943 RVA: 0x0005B8A8 File Offset: 0x00059AA8
	public override Damage GetComplexDamage(float range_km, IDamageableType targetType, float targetCrossSectionalArea_m2, float relativeVelocity_kps = -1f, CombatWeaponCarrierState attacker = null, TIFactionState attackingFaction = null, float warheadMassOverride_kg = -1f)
	{
		if (attacker != null)
		{
			attackingFaction = attacker.GetFaction();
		}
		DamageBreakdown damageBreakdown = this.DamageAtRange_points(range_km, targetCrossSectionalArea_m2, attacker, 0f, 0f, true);
		return new Damage(this, range_km, this.GetDamageType(), damageBreakdown.directDamage_Points, damageBreakdown.chippingDamage_Points, 0, attackingFaction);
	}

	// Token: 0x04001174 RID: 4468
	public int shotPower_MJ;

	// Token: 0x04001175 RID: 4469
	protected Dictionary<float, float> _damageMinRange = new Dictionary<float, float>();
}
