using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

// Token: 0x020003E2 RID: 994
public class TIPlasmaWeaponTemplate : TIGunTypeWeaponTemplate
{
	// Token: 0x17000293 RID: 659
	// (get) Token: 0x060013A7 RID: 5031 RVA: 0x0005CD20 File Offset: 0x0005AF20
	public override bool canBombardThroughAtmosphere
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000294 RID: 660
	// (get) Token: 0x060013A8 RID: 5032 RVA: 0x0005CD23 File Offset: 0x0005AF23
	public override WeaponClass weaponClass
	{
		get
		{
			return WeaponClass.Plasma;
		}
	}

	// Token: 0x060013A9 RID: 5033 RVA: 0x0005CD26 File Offset: 0x0005AF26
	public override TIResourcesCost magazineCost(float magazines)
	{
		return new TIResourcesCost();
	}

	// Token: 0x060013AA RID: 5034 RVA: 0x0005CD2D File Offset: 0x0005AF2D
	public override bool magazineRequiresResources()
	{
		return false;
	}

	// Token: 0x17000295 RID: 661
	// (get) Token: 0x060013AB RID: 5035 RVA: 0x0005CD30 File Offset: 0x0005AF30
	public override bool isPlasmaWeapon
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060013AC RID: 5036 RVA: 0x0005CD33 File Offset: 0x0005AF33
	public override float EnergyUsage_GJ(float extraInput_MW = 0f)
	{
		return (this.chargingEnergy_GJ + 0.5f * this.warheadMass_kg * Mathf.Pow(this.muzzleVelocity_kps * 1000f, 2f) * 1E-09f) / this.efficiency;
	}

	// Token: 0x060013AD RID: 5037 RVA: 0x0005CD6C File Offset: 0x0005AF6C
	public override float HeatGeneration_GJ(float extraInput_MJ = 0f)
	{
		return this.EnergyUsage_GJ(extraInput_MJ) * (1f - this.efficiency);
	}

	// Token: 0x060013AE RID: 5038 RVA: 0x0005CD84 File Offset: 0x0005AF84
	public override TIResourcesCost buildCost(float value = 0f, float value2 = 0f)
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		tiresourcesCost.SumCosts_NoDuration(this.weightedBuildMaterials.ToResourcesCost(this.buildMass_tons(value, value2, 0f, 0f, false) * TemplateManager.global.spaceResourceToTons));
		return tiresourcesCost;
	}

	// Token: 0x060013AF RID: 5039 RVA: 0x0005CDC5 File Offset: 0x0005AFC5
	public string GetLocalizedChargingEnergy()
	{
		return Loc.T("UI.Fleets.GJ", new object[] { this.chargingEnergy_GJ.ToString("N1") });
	}

	// Token: 0x060013B0 RID: 5040 RVA: 0x0005CDEA File Offset: 0x0005AFEA
	public override DamageType GetDamageType()
	{
		return DamageType.Thermal;
	}

	// Token: 0x060013B1 RID: 5041 RVA: 0x0005CDED File Offset: 0x0005AFED
	public override float GetSurfaceImpactVelocity_kps(TISpaceBodyState spaceBody, float altitude_km)
	{
		return this.muzzleVelocity_kps;
	}

	// Token: 0x060013B2 RID: 5042 RVA: 0x0005CDF5 File Offset: 0x0005AFF5
	public override float EffectiveRangeAgainstProjectiles_km()
	{
		return Mathf.Max(200f, this.attackMode ? (this.targetingRange_km / 3f) : this.targetingRange_km);
	}

	// Token: 0x04001192 RID: 4498
	public float chargingEnergy_GJ;
}
