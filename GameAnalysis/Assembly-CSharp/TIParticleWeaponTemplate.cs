using System;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

// Token: 0x020003DE RID: 990
public class TIParticleWeaponTemplate : TIBeamWeaponTemplate
{
	// Token: 0x17000280 RID: 640
	// (get) Token: 0x0600136B RID: 4971 RVA: 0x0005C18D File Offset: 0x0005A38D
	public override WeaponClass weaponClass
	{
		get
		{
			return WeaponClass.Particle;
		}
	}

	// Token: 0x17000281 RID: 641
	// (get) Token: 0x0600136C RID: 4972 RVA: 0x0005C190 File Offset: 0x0005A390
	public override bool canBombardThroughAtmosphere
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000282 RID: 642
	// (get) Token: 0x0600136D RID: 4973 RVA: 0x0005C193 File Offset: 0x0005A393
	public override TIParticleWeaponTemplate ref_particleWeapon
	{
		get
		{
			return this;
		}
	}

	// Token: 0x17000283 RID: 643
	// (get) Token: 0x0600136E RID: 4974 RVA: 0x0005C196 File Offset: 0x0005A396
	public override bool isParticleWeapon
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600136F RID: 4975 RVA: 0x0005C19C File Offset: 0x0005A39C
	public override float DamageAtRange_MJ(float range_km, float targetCrossSectionArea_m2, CombatWeaponCarrierState attacker, float finalVelocity_kps = 0f, float warheadMass_kg = 0f, bool applyChipping = true)
	{
		float num = this.BaseDamageAtRange_MJ(range_km, applyChipping);
		float? num2;
		if (attacker == null)
		{
			num2 = null;
		}
		else
		{
			TISpaceShipState tispaceShipState = attacker.ref_shipCarrier();
			num2 = ((tispaceShipState != null) ? new float?(tispaceShipState.GetBonusPowerForWeapon_MJ(this)) : null);
		}
		float? num3 = num2;
		float num4 = num + num3.GetValueOrDefault();
		float num5 = num4;
		float num6 = TIEffectsState.SumEffectsModifiers(Context.ParticleLaserDamage, (attacker != null) ? attacker.GetFaction() : null, num4, null);
		float? num7;
		if (attacker == null)
		{
			num7 = null;
		}
		else
		{
			TISpaceShipState tispaceShipState2 = attacker.ref_shipCarrier();
			num7 = ((tispaceShipState2 != null) ? new float?(tispaceShipState2.SumOfficerEffectsModifiers(OfficerEffectType.BeamDamage, num4)) : null);
		}
		num3 = num7;
		float num8 = num6 + num3.GetValueOrDefault();
		float? num9;
		if (attacker == null)
		{
			num9 = null;
		}
		else
		{
			TISpaceShipState tispaceShipState3 = attacker.ref_shipCarrier();
			num9 = ((tispaceShipState3 != null) ? new float?(tispaceShipState3.SumOfficerEffectsModifiers(OfficerEffectType.GlobalDamage, num4)) : null);
		}
		num3 = num9;
		num4 = num5 + (num8 + num3.GetValueOrDefault());
		float num10 = this.SpotSurfaceArea_m2(range_km);
		if (num10 > targetCrossSectionArea_m2)
		{
			num4 *= targetCrossSectionArea_m2 / num10;
		}
		return num4;
	}

	// Token: 0x06001370 RID: 4976 RVA: 0x0005C28C File Offset: 0x0005A48C
	public float SpotSurfaceArea_m2(float range_km)
	{
		ParticleBeamDispersionModel particleBeamDispersionModel = this.dispersionModel;
		float num;
		if (particleBeamDispersionModel == ParticleBeamDispersionModel.Charged || particleBeamDispersionModel != ParticleBeamDispersionModel.Neutral)
		{
			num = this.lensRadius_cm * Mathf.Pow(2f, range_km / this.doublingRange_km) * 0.01f;
		}
		else
		{
			num = this.emittance_mrad * (range_km * 1000f) * 1E-06f / (this.lensRadius_cm * 0.01f);
		}
		return 3.1415927f * num * num;
	}

	// Token: 0x06001371 RID: 4977 RVA: 0x0005C2F4 File Offset: 0x0005A4F4
	public override float BaseDamageAtRange_MJ(float range_km, bool applyChipping = true)
	{
		return (float)Mathf.Max(0, this.shotPower_MJ);
	}

	// Token: 0x06001372 RID: 4978 RVA: 0x0005C304 File Offset: 0x0005A504
	public override float RangeToDoDamage_km(float desiredDamage_Points, TISpaceShipState ship)
	{
		if (this._damageMinRange.ContainsKey(desiredDamage_Points))
		{
			return this._damageMinRange[desiredDamage_Points];
		}
		ParticleBeamDispersionModel particleBeamDispersionModel = this.dispersionModel;
		float num;
		if (particleBeamDispersionModel == ParticleBeamDispersionModel.Charged || particleBeamDispersionModel != ParticleBeamDispersionModel.Neutral)
		{
			num = this.targetingRange_km;
		}
		else
		{
			num = this.targetingRange_km;
		}
		float num2 = Mathf.Max(0f, num);
		this._damageMinRange[desiredDamage_Points] = num2;
		return num2;
	}

	// Token: 0x06001373 RID: 4979 RVA: 0x0005C364 File Offset: 0x0005A564
	public override float chipping(float range_km = 0f)
	{
		return 0f;
	}

	// Token: 0x06001374 RID: 4980 RVA: 0x0005C36C File Offset: 0x0005A56C
	public override string SpecificDescriptionData()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (this.attackMode)
		{
			stringBuilder.AppendLine(this.GetLocalizedPowerAndDamageAtRange(500f));
			stringBuilder.AppendLine(this.GetLocalizedDamageBreakdown());
		}
		stringBuilder.AppendLine(base.GetLocalizedCooldown());
		return stringBuilder.ToString();
	}

	// Token: 0x06001375 RID: 4981 RVA: 0x0005C3BC File Offset: 0x0005A5BC
	public string GetLocalizedDamageBreakdown()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(Loc.T("TIParticleWeaponTemplate.HeatDamage", new object[] { this.heatFraction.ToPercent("P0") }));
		stringBuilder.AppendLine(Loc.T("TIParticleWeaponTemplate.XRayDamage", new object[] { this.xRayFraction.ToPercent("P0") }));
		stringBuilder.AppendLine(Loc.T("TIParticleWeaponTemplate.BaryonicDamage", new object[]
		{
			5f,
			this.baryonFraction.ToPercent("P0")
		}));
		return stringBuilder.ToString();
	}

	// Token: 0x06001376 RID: 4982 RVA: 0x0005C460 File Offset: 0x0005A660
	public override string GetLocalizedPowerAndDamageAtRange(float range)
	{
		return Loc.T("TIParticleWeaponTemplate.DamageAtRange", new object[]
		{
			this.BaseDamageAtRange_MJ(range, false).ToString("N1"),
			range,
			this.BaseDamageAtRange_points(range, false).ToString("N1")
		});
	}

	// Token: 0x06001377 RID: 4983 RVA: 0x0005C4B6 File Offset: 0x0005A6B6
	public override DamageType GetDamageType()
	{
		return DamageType.ParticleBeam;
	}

	// Token: 0x06001378 RID: 4984 RVA: 0x0005C4B9 File Offset: 0x0005A6B9
	public override bool CanOnlyDefensivelyTargetMissiles()
	{
		return this.dispersionModel == ParticleBeamDispersionModel.Charged;
	}

	// Token: 0x0400117C RID: 4476
	public ParticleBeamDispersionModel dispersionModel;

	// Token: 0x0400117D RID: 4477
	public float doublingRange_km = 1f;

	// Token: 0x0400117E RID: 4478
	public float emittance_mrad = 1f;

	// Token: 0x0400117F RID: 4479
	public float lensRadius_cm = 5f;

	// Token: 0x04001180 RID: 4480
	public float heatFraction;

	// Token: 0x04001181 RID: 4481
	public float xRayFraction;

	// Token: 0x04001182 RID: 4482
	public float baryonFraction;

	// Token: 0x04001183 RID: 4483
	public const float BARYONIC_DAMAGE_MULTIPLIER = 5f;
}
