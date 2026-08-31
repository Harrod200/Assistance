using System;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020003E0 RID: 992
public abstract class TIGunTypeWeaponTemplate : TIProjectileWeaponTemplate
{
	// Token: 0x1700028C RID: 652
	// (get) Token: 0x06001394 RID: 5012 RVA: 0x0005C9A0 File Offset: 0x0005ABA0
	public override bool canBombardThroughAtmosphere
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700028D RID: 653
	// (get) Token: 0x06001395 RID: 5013 RVA: 0x0005C9A3 File Offset: 0x0005ABA3
	public override TIGunTypeWeaponTemplate ref_gunWeapon
	{
		get
		{
			return this;
		}
	}

	// Token: 0x1700028E RID: 654
	// (get) Token: 0x06001396 RID: 5014 RVA: 0x0005C9A6 File Offset: 0x0005ABA6
	public override bool isGunTypeWeapon
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001397 RID: 5015 RVA: 0x0005C9A9 File Offset: 0x0005ABA9
	public override float BaseDamageAtRange_MJ(float range_km, bool applyChipping = true)
	{
		return this.KineticEnergyDamage_MJ(this.muzzleVelocity_kps, this.warheadMass_kg) * (applyChipping ? (1f - this.flatChipping) : 1f);
	}

	// Token: 0x06001398 RID: 5016 RVA: 0x0005C9D4 File Offset: 0x0005ABD4
	public float MinVelocityFor1Damage_kps()
	{
		if (this._shipVelocityFor1Damage_kps == -1f)
		{
			if (this.BaseDamageAtRange_MJ(0f, false) >= 20f)
			{
				this._shipVelocityFor1Damage_kps = 0f;
			}
			else
			{
				this._shipVelocityFor1Damage_kps = Mathf.Sqrt(20f / (0.5f * this.warheadMass_kg)) - this.muzzleVelocity_kps;
			}
		}
		return this._shipVelocityFor1Damage_kps;
	}

	// Token: 0x06001399 RID: 5017 RVA: 0x0005CA38 File Offset: 0x0005AC38
	public override float DamageAtRange_MJ(float range_km, float targetCrossSection_m, CombatWeaponCarrierState attacker = null, float finalVelocity_kps = 0f, float warheadMass_kg = 0f, bool applyChipping = true)
	{
		float num = this.KineticEnergyDamage_MJ(finalVelocity_kps, warheadMass_kg) * (applyChipping ? (1f - this.flatChipping) : 1f);
		float num2 = num;
		float? num3;
		if (attacker == null)
		{
			num3 = null;
		}
		else
		{
			TISpaceShipState tispaceShipState = attacker.ref_shipCarrier();
			num3 = ((tispaceShipState != null) ? new float?(tispaceShipState.SumOfficerEffectsModifiers(OfficerEffectType.GunDamage, num)) : null);
		}
		float? num4 = num3;
		float valueOrDefault = num4.GetValueOrDefault();
		float? num5;
		if (attacker == null)
		{
			num5 = null;
		}
		else
		{
			TISpaceShipState tispaceShipState2 = attacker.ref_shipCarrier();
			num5 = ((tispaceShipState2 != null) ? new float?(tispaceShipState2.SumOfficerEffectsModifiers(OfficerEffectType.GlobalDamage, num)) : null);
		}
		num4 = num5;
		return num2 + (valueOrDefault + num4.GetValueOrDefault());
	}

	// Token: 0x0600139A RID: 5018 RVA: 0x0005CAE0 File Offset: 0x0005ACE0
	public override string SpecificDescriptionData()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(this.GetLocalizedMuzzleVelocity());
		stringBuilder.AppendLine(this.GetLocalizedPowerAndDamageAtRange(20f));
		stringBuilder.AppendLine(base.GetLocalizedCooldown());
		if (this.warheadMass_kg > 1f)
		{
			stringBuilder.AppendLine(base.GetLocalizedWarheadMass());
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600139B RID: 5019 RVA: 0x0005CB3F File Offset: 0x0005AD3F
	public string GetLocalizedMuzzleVelocity()
	{
		return Loc.T("TIProjectileWeaponTemplate.MuzzleVelocity", new object[] { this.muzzleVelocity_kps.ToString("N2") });
	}

	// Token: 0x0600139C RID: 5020 RVA: 0x0005CB64 File Offset: 0x0005AD64
	public override float GetSurfaceImpactVelocity_kps(TISpaceBodyState spaceBody, float altitude_km)
	{
		float num = (float)spaceBody.meanRadius_m + altitude_km * 1000f;
		float num2 = (float)Mathd.Sqrt(spaceBody.mu / (double)num);
		float num3 = this.muzzleVelocity_kps * 1000f;
		float num4 = Mathf.Sqrt(num3 * num3 + num2 * num2);
		float num5 = 0.5f * this.warheadMass_kg * num4 * num4;
		float num6 = (float)(spaceBody.surfaceGravity_mps2 + spaceBody.localAccelerationDueToGravity_ms2((double)num)) / 2f;
		float num7 = this.warheadMass_kg * altitude_km * 1000f * num6;
		float num8 = num5 + num7;
		float num9 = Mathf.Sqrt(2f * num8 / this.warheadMass_kg);
		if (spaceBody.atmosphere > Atmosphere.Trace)
		{
			float num10 = (float)spaceBody.template.atmosphereScaleHeight_km * 1000f * (num4 / num3);
			float num11 = (float)spaceBody.template.atmosphereSurfaceDensity_kgpm3 * num10 * 0.001f * num4 * num4;
			float num12 = Mathf.Max(0f, num5 + num7 - num11);
			float num13 = Mathf.Sqrt(2f * num12 / this.warheadMass_kg);
			float num14 = Mathf.Sqrt(2f * this.warheadMass_kg * (float)spaceBody.surfaceGravity_mps2 / 4.0000003E-05f / (float)spaceBody.template.atmosphereSurfaceDensity_kgpm3);
			if (num9 > num14)
			{
				num13 = Mathf.Max(num14, num13);
			}
			else
			{
				num13 = Mathf.Min(num14, num13);
			}
			return num13 / 1000f;
		}
		return num9 / 1000f;
	}

	// Token: 0x1700028F RID: 655
	// (get) Token: 0x0600139D RID: 5021 RVA: 0x0005CCCB File Offset: 0x0005AECB
	public override float EstimatedImpactVelocity_kps
	{
		get
		{
			return this.muzzleVelocity_kps;
		}
	}

	// Token: 0x0600139E RID: 5022 RVA: 0x0005CCD3 File Offset: 0x0005AED3
	public override float EffectiveRangeAgainstProjectiles_km()
	{
		return Mathf.Max(this.targetingRange_km / 2f, 200f);
	}

	// Token: 0x04001190 RID: 4496
	public float muzzleVelocity_kps;

	// Token: 0x04001191 RID: 4497
	private float _shipVelocityFor1Damage_kps = -1f;
}
