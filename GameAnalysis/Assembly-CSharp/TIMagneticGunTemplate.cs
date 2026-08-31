using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

// Token: 0x020003E3 RID: 995
public class TIMagneticGunTemplate : TIGunTypeWeaponTemplate
{
	// Token: 0x17000296 RID: 662
	// (get) Token: 0x060013B4 RID: 5044 RVA: 0x0005CE25 File Offset: 0x0005B025
	public override WeaponClass weaponClass
	{
		get
		{
			return WeaponClass.Magnetic;
		}
	}

	// Token: 0x17000297 RID: 663
	// (get) Token: 0x060013B5 RID: 5045 RVA: 0x0005CE28 File Offset: 0x0005B028
	public override bool canBombardThroughAtmosphere
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x060013B6 RID: 5046 RVA: 0x0005CE2B File Offset: 0x0005B02B
	public override bool isMagneticGunWeapon
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000299 RID: 665
	// (get) Token: 0x060013B7 RID: 5047 RVA: 0x0005CE2E File Offset: 0x0005B02E
	public override float minDamageForPDToFire
	{
		get
		{
			return TemplateManager.global.DP_FireAtMagRound;
		}
	}

	// Token: 0x060013B8 RID: 5048 RVA: 0x0005CE3A File Offset: 0x0005B03A
	public override float EnergyUsage_GJ(float extraInput_MW = 0f)
	{
		return 0.5f * this.ammoMass_kg * (Mathf.Pow(this.muzzleVelocity_kps * 1000f, 2f) / this.efficiency) * 1E-09f;
	}

	// Token: 0x060013B9 RID: 5049 RVA: 0x0005CE6C File Offset: 0x0005B06C
	protected override float KineticEnergyDamage_MJ(float finalVelocity_kps, float warheadMass_kg)
	{
		return 0.5f * warheadMass_kg * Mathf.Pow(finalVelocity_kps * 1000f, 2f) * 1E-06f;
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x0005CE8D File Offset: 0x0005B08D
	public override float HeatGeneration_GJ(float extraInput_MJ = 0f)
	{
		return this.EnergyUsage_GJ(extraInput_MJ) * (1f - this.efficiency);
	}

	// Token: 0x060013BB RID: 5051 RVA: 0x0005CEA4 File Offset: 0x0005B0A4
	public override float DamageAtRange_MJ(float range_km, float targetCrossSection_m, CombatWeaponCarrierState attacker = null, float finalVelocity_kps = 0f, float warheadMass_kg = 0f, bool applyChipping = true)
	{
		float num = this.KineticEnergyDamage_MJ(finalVelocity_kps, warheadMass_kg) * (applyChipping ? (1f - this.flatChipping) : 1f);
		float num2 = num;
		float num3 = TIEffectsState.SumEffectsModifiers(Context.ShipMagDamage, (attacker != null) ? attacker.GetFaction() : null, num, null);
		float? num4;
		if (attacker == null)
		{
			num4 = null;
		}
		else
		{
			TISpaceShipState tispaceShipState = attacker.ref_shipCarrier();
			num4 = ((tispaceShipState != null) ? new float?(tispaceShipState.SumOfficerEffectsModifiers(OfficerEffectType.GunDamage, num)) : null);
		}
		float? num5 = num4;
		float num6 = num3 + num5.GetValueOrDefault();
		float? num7;
		if (attacker == null)
		{
			num7 = null;
		}
		else
		{
			TISpaceShipState tispaceShipState2 = attacker.ref_shipCarrier();
			num7 = ((tispaceShipState2 != null) ? new float?(tispaceShipState2.SumOfficerEffectsModifiers(OfficerEffectType.GlobalDamage, num)) : null);
		}
		num5 = num7;
		return num2 + (num6 + num5.GetValueOrDefault());
	}

	// Token: 0x060013BC RID: 5052 RVA: 0x0005CF62 File Offset: 0x0005B162
	public override DamageType GetDamageType()
	{
		return DamageType.Kinetic;
	}
}
