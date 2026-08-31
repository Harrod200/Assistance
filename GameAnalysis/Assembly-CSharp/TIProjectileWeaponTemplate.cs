using System;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

// Token: 0x020003DF RID: 991
public abstract class TIProjectileWeaponTemplate : TIShipWeaponTemplate
{
	// Token: 0x17000284 RID: 644
	// (get) Token: 0x0600137A RID: 4986 RVA: 0x0005C4ED File Offset: 0x0005A6ED
	public virtual string ammoIconPath
	{
		get
		{
			return "ui_spacecombat/ICO_remaining_ammo_shell";
		}
	}

	// Token: 0x0600137B RID: 4987 RVA: 0x0005C4F4 File Offset: 0x0005A6F4
	public override float BaseDamageAtRange_points(float range_km, bool applyChipping = true)
	{
		if (applyChipping)
		{
			if (this._baseDamageAtRange_Points_WithChipping == -1f)
			{
				this._baseDamageAtRange_Points_WithChipping = base.BaseDamageAtRange_points(range_km, applyChipping);
			}
			return this._baseDamageAtRange_Points_WithChipping;
		}
		if (this._baseDamageATRange_Points_NoChipping == -1f)
		{
			this._baseDamageATRange_Points_NoChipping = base.BaseDamageAtRange_points(range_km, applyChipping);
		}
		return this._baseDamageATRange_Points_NoChipping;
	}

	// Token: 0x0600137C RID: 4988 RVA: 0x0005C547 File Offset: 0x0005A747
	public float EstimatedBaseDamageAtRange_points(float range_km, bool applyChipping = true)
	{
		return base.BaseDamageAtRange_points(range_km, applyChipping);
	}

	// Token: 0x17000285 RID: 645
	// (get) Token: 0x0600137D RID: 4989 RVA: 0x0005C553 File Offset: 0x0005A753
	public virtual float minDamageForPDToFire
	{
		get
		{
			return TemplateManager.global.DP_DestroyMissile;
		}
	}

	// Token: 0x17000286 RID: 646
	// (get) Token: 0x0600137E RID: 4990 RVA: 0x0005C55F File Offset: 0x0005A75F
	public float magazineMass_kg
	{
		get
		{
			return (float)this.magazine * this.ammoMass_kg;
		}
	}

	// Token: 0x17000287 RID: 647
	// (get) Token: 0x0600137F RID: 4991 RVA: 0x0005C56F File Offset: 0x0005A76F
	public float magazineMass_tons
	{
		get
		{
			return this.magazineMass_kg / 1000f;
		}
	}

	// Token: 0x06001380 RID: 4992 RVA: 0x0005C57D File Offset: 0x0005A77D
	public override bool hasMagazine()
	{
		return true;
	}

	// Token: 0x17000288 RID: 648
	// (get) Token: 0x06001381 RID: 4993 RVA: 0x0005C580 File Offset: 0x0005A780
	public override TIProjectileWeaponTemplate ref_projectileWeapon
	{
		get
		{
			return this;
		}
	}

	// Token: 0x17000289 RID: 649
	// (get) Token: 0x06001382 RID: 4994 RVA: 0x0005C583 File Offset: 0x0005A783
	public override bool isProjectileWeapon
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001383 RID: 4995 RVA: 0x0005C588 File Offset: 0x0005A788
	public int FullAmmoCount_Max(TISpaceShipTemplate ship)
	{
		return (int)((((float)1 + ((ship != null) ? new float?(ship.magazineModuleMultiplier) : null)) ?? 1f) * (float)this.magazine);
	}

	// Token: 0x06001384 RID: 4996 RVA: 0x0005C5F8 File Offset: 0x0005A7F8
	public int FullAmmoCount_Current(TISpaceShipState ship)
	{
		return (int)((((float)1 + ((ship != null) ? new float?(ship.functionalMagazineModulesAmmoMultiplier) : null)) ?? 1f) * (float)this.magazine);
	}

	// Token: 0x06001385 RID: 4997 RVA: 0x0005C668 File Offset: 0x0005A868
	public int FullAmmoCount_PendingRepairs(TISpaceShipState ship, float pendingRepairedMagazinesMultiplier)
	{
		return (int)(1f + ((pendingRepairedMagazinesMultiplier + ((ship != null) ? new float?(ship.functionalMagazineModulesAmmoMultiplier) : null)) ?? 1f) * (float)this.magazine);
	}

	// Token: 0x06001386 RID: 4998 RVA: 0x0005C6DA File Offset: 0x0005A8DA
	public override float buildMass_tons(float shipMagazineMultiplier = 0f, float value2 = 0f, float value3 = 0f, float value4 = 0f, bool bValue = false)
	{
		return this.baseWeaponMass_tons + (1f + shipMagazineMultiplier) * this.magazineMass_tons;
	}

	// Token: 0x06001387 RID: 4999 RVA: 0x0005C6F1 File Offset: 0x0005A8F1
	public override TIResourcesCost buildCost(float magazineMultiplier = 0f, float value2 = 0f)
	{
		TIResourcesCost emptyWeaponCost = this.emptyWeaponCost;
		emptyWeaponCost.SumCosts_NoDuration(this.magazineCost(magazineMultiplier));
		return emptyWeaponCost;
	}

	// Token: 0x1700028A RID: 650
	// (get) Token: 0x06001388 RID: 5000 RVA: 0x0005C706 File Offset: 0x0005A906
	public TIResourcesCost emptyWeaponCost
	{
		get
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			tiresourcesCost.SumCosts_NoDuration(this.weightedBuildMaterials.ToResourcesCost(this.baseWeaponMass_tons * TemplateManager.global.spaceResourceToTons));
			return tiresourcesCost;
		}
	}

	// Token: 0x06001389 RID: 5001 RVA: 0x0005C72F File Offset: 0x0005A92F
	public virtual TIResourcesCost magazineCost(float magazineMultiplier)
	{
		return this.ammoMaterials.ToResourcesCost((1f + magazineMultiplier) * this.magazineMass_tons * TemplateManager.global.spaceResourceToTons);
	}

	// Token: 0x0600138A RID: 5002 RVA: 0x0005C755 File Offset: 0x0005A955
	public override float chipping(float range_km = 0f)
	{
		return this.flatChipping;
	}

	// Token: 0x0600138B RID: 5003 RVA: 0x0005C75D File Offset: 0x0005A95D
	protected virtual float KineticEnergyDamage_MJ(float finalVelocity_kps, float warheadMass_kg)
	{
		return 0.5f * warheadMass_kg * Mathf.Pow(finalVelocity_kps * 1000f, 2f) * 1E-06f;
	}

	// Token: 0x0600138C RID: 5004 RVA: 0x0005C780 File Offset: 0x0005A980
	public override Damage GetComplexDamage(float range_km, IDamageableType targetType, float targetCrossSectionalArea_m2, float relativeVelocity_kps = -1f, CombatWeaponCarrierState attacker = null, TIFactionState attackingFaction = null, float warheadMassOverride_kg = -1f)
	{
		if (relativeVelocity_kps < 0f)
		{
			relativeVelocity_kps = this.EstimatedImpactVelocity_kps;
		}
		DamageBreakdown damageBreakdown = this.DamageAtRange_points(0f, targetCrossSectionalArea_m2, attacker, relativeVelocity_kps, (warheadMassOverride_kg > 0f) ? warheadMassOverride_kg : this.warheadMass_kg, true);
		return new Damage(this, 0f, this.GetDamageType(), damageBreakdown.directDamage_Points, damageBreakdown.chippingDamage_Points, 0, attackingFaction);
	}

	// Token: 0x0600138D RID: 5005 RVA: 0x0005C7E4 File Offset: 0x0005A9E4
	public override string SpecificDescriptionData()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(this.GetLocalizedPowerAndDamageAtRange(20f));
		stringBuilder.AppendLine(base.GetLocalizedCooldown());
		if (this.warheadMass_kg > 1f)
		{
			stringBuilder.AppendLine(this.GetLocalizedWarheadMass());
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600138E RID: 5006 RVA: 0x0005C836 File Offset: 0x0005AA36
	public string GetLocalizedWarheadMass()
	{
		return Loc.T("TIProjectileWeaponTemplate.WarheadMass", new object[] { this.warheadMass_kg.ToString("N0") });
	}

	// Token: 0x0600138F RID: 5007 RVA: 0x0005C85C File Offset: 0x0005AA5C
	public override string GetLocalizedPowerAndDamageAtRange(float range)
	{
		if (this.isMissileWeapon && (this.ref_missileWeapon.warheadClass == WarheadClass.Nuclear || this.ref_missileWeapon.warheadClass == WarheadClass.ShapedNuclear))
		{
			return Loc.T("TIProjectileWeaponTemplate.Damage", new object[]
			{
				TIUtilities.FormatBigNumber((double)this.BaseDamageAtRange_MJ(range, false), 1, false),
				TIUtilities.FormatBigNumber((double)this.BaseDamageAtRange_points(range, false), 1, false)
			});
		}
		return Loc.T("TIProjectileWeaponTemplate.Damage", new object[]
		{
			this.BaseDamageAtRange_MJ(range, false).ToString("N1"),
			this.BaseDamageAtRange_points(range, false).ToString("N1")
		});
	}

	// Token: 0x06001390 RID: 5008
	public abstract float GetSurfaceImpactVelocity_kps(TISpaceBodyState spaceBody, float altitude_km);

	// Token: 0x1700028B RID: 651
	// (get) Token: 0x06001391 RID: 5009
	public abstract float EstimatedImpactVelocity_kps { get; }

	// Token: 0x06001392 RID: 5010 RVA: 0x0005C904 File Offset: 0x0005AB04
	public override float EstimateChanceToHit(float range_km, TISpaceShipState targetState = null, TISpaceShipTemplate targetTemplate = null, float overrideTargetAcceleration_mps2 = -1f)
	{
		float num = 1f;
		if (overrideTargetAcceleration_mps2 >= 0f)
		{
			num = overrideTargetAcceleration_mps2;
		}
		else if (targetState != null)
		{
			num = targetState.combatAcceleration_mps2;
		}
		else if (targetTemplate != null)
		{
			num = targetTemplate.baseCombatAcceleration_mps2;
		}
		float num2 = num / 0.4f;
		if (num2 > 1f)
		{
			num2 = Mathf.Pow(num2, 0.5f);
		}
		num2 /= this.EstimatedImpactVelocity_kps / 9f;
		return Mathf.Clamp(1f / num2, 0f, 1f);
	}

	// Token: 0x04001184 RID: 4484
	public int magazine;

	// Token: 0x04001185 RID: 4485
	public ResourceCostBuilder ammoMaterials;

	// Token: 0x04001186 RID: 4486
	public float ammoMass_kg;

	// Token: 0x04001187 RID: 4487
	public float warheadMass_kg;

	// Token: 0x04001188 RID: 4488
	public float flatChipping;

	// Token: 0x04001189 RID: 4489
	protected const float noseSurfaceArea_m2 = 0.001f;

	// Token: 0x0400118A RID: 4490
	protected const float dragCoefficient = 0.04f;

	// Token: 0x0400118B RID: 4491
	public string shotModelResource;

	// Token: 0x0400118C RID: 4492
	public string impactVisualFXResource;

	// Token: 0x0400118D RID: 4493
	public string impactSoundFXResource;

	// Token: 0x0400118E RID: 4494
	private float _baseDamageAtRange_Points_WithChipping = -1f;

	// Token: 0x0400118F RID: 4495
	private float _baseDamageATRange_Points_NoChipping = -1f;
}
