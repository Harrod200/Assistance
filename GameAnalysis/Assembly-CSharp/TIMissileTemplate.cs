using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

// Token: 0x020003E4 RID: 996
public class TIMissileTemplate : TIProjectileWeaponTemplate
{
	// Token: 0x1700029A RID: 666
	// (get) Token: 0x060013BE RID: 5054 RVA: 0x0005CF6D File Offset: 0x0005B16D
	public override TIMissileTemplate ref_missileWeapon
	{
		get
		{
			return this;
		}
	}

	// Token: 0x1700029B RID: 667
	// (get) Token: 0x060013BF RID: 5055 RVA: 0x0005CF70 File Offset: 0x0005B170
	public override WeaponClass weaponClass
	{
		get
		{
			return WeaponClass.Missile;
		}
	}

	// Token: 0x1700029C RID: 668
	// (get) Token: 0x060013C0 RID: 5056 RVA: 0x0005CF73 File Offset: 0x0005B173
	public override bool isMissileWeapon
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700029D RID: 669
	// (get) Token: 0x060013C1 RID: 5057 RVA: 0x0005CF76 File Offset: 0x0005B176
	public override bool staticLauncher
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700029E RID: 670
	// (get) Token: 0x060013C2 RID: 5058 RVA: 0x0005CF79 File Offset: 0x0005B179
	public override bool canBombardThroughAtmosphere
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700029F RID: 671
	// (get) Token: 0x060013C3 RID: 5059 RVA: 0x0005CF7C File Offset: 0x0005B17C
	public override string ammoIconPath
	{
		get
		{
			return "ui_spacecombat/ICO_remaining_ammo_missile";
		}
	}

	// Token: 0x060013C4 RID: 5060 RVA: 0x0005CF83 File Offset: 0x0005B183
	protected override List<FireMode> GetAllowedFireModes()
	{
		return new List<FireMode>
		{
			FireMode.Offense,
			FireMode.Defense,
			FireMode.Focus
		};
	}

	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x060013C5 RID: 5061 RVA: 0x0005CF9F File Offset: 0x0005B19F
	public float acceleration_mps2
	{
		get
		{
			return this.acceleration_g * 9.80665f;
		}
	}

	// Token: 0x060013C6 RID: 5062 RVA: 0x0005CFB0 File Offset: 0x0005B1B0
	public override DamageType GetDamageType()
	{
		switch (this.warheadClass)
		{
		case WarheadClass.Explosive:
		case WarheadClass.Antimatter:
			return DamageType.Explosive;
		case WarheadClass.Nuclear:
		case WarheadClass.ShapedNuclear:
			return DamageType.Nuclear;
		}
		return DamageType.Kinetic;
	}

	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x060013C7 RID: 5063 RVA: 0x0005CFE9 File Offset: 0x0005B1E9
	public bool AOEWeapon
	{
		get
		{
			return this.warheadClass == WarheadClass.Nuclear || this.warheadClass == WarheadClass.ShapedNuclear || this.warheadClass == WarheadClass.Antimatter;
		}
	}

	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x060013C8 RID: 5064 RVA: 0x0005D008 File Offset: 0x0005B208
	public override float EstimatedImpactVelocity_kps
	{
		get
		{
			return this.deltaV_kps * 0.5f;
		}
	}

	// Token: 0x060013C9 RID: 5065 RVA: 0x0005D018 File Offset: 0x0005B218
	public override float EstimateChanceToHit(float range_km, TISpaceShipState targetState = null, TISpaceShipTemplate targetTemplate = null, float overrideTargetAcceleration_mps2 = -1f)
	{
		bool flag = false;
		float num = -1f;
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
		if ((targetState != null || targetTemplate != null) && range_km > 0f)
		{
			float num2 = this.acceleration_mps2 - num;
			float num3 = this.deltaV_kps * 1000f / this.acceleration_mps2;
			flag = 0.5f * num2 * Mathf.Pow(num3, 2f) < range_km;
			float num4;
			if (targetState != null)
			{
				num4 = targetState.currentDeltaV_kps;
			}
			else
			{
				num4 = targetTemplate.baseCruiseDeltaV_kps(false);
			}
			if (flag && targetState != null && num4 < this.deltaV_kps)
			{
				flag = false;
			}
		}
		return Mathf.Pow(base.EstimateChanceToHit(range_km, targetState, targetTemplate, overrideTargetAcceleration_mps2), 0.3f) * (flag ? 0.5f : 1f);
	}

	// Token: 0x060013CA RID: 5066 RVA: 0x0005D0F8 File Offset: 0x0005B2F8
	public override float EstimateDPS(float expectedRange_km, TISpaceShipTemplate target, bool applyOverkillPenalty)
	{
		return base.EstimateDPS(expectedRange_km, target, applyOverkillPenalty);
	}

	// Token: 0x060013CB RID: 5067 RVA: 0x0005D103 File Offset: 0x0005B303
	public override float EnergyUsage_GJ(float extraInput_MW = 0f)
	{
		return 0f;
	}

	// Token: 0x060013CC RID: 5068 RVA: 0x0005D10A File Offset: 0x0005B30A
	public override float HeatGeneration_GJ(float extraInput_MJ = 0f)
	{
		return 0f;
	}

	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x060013CD RID: 5069 RVA: 0x0005D111 File Offset: 0x0005B311
	public override bool selfPowered
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060013CE RID: 5070 RVA: 0x0005D114 File Offset: 0x0005B314
	protected override float KineticEnergyDamage_MJ(float finalVelocity_kps, float warheadMass_kg)
	{
		switch (this.warheadClass)
		{
		case WarheadClass.Explosive:
			return 0.1f * (0.5f * warheadMass_kg * Mathf.Pow(finalVelocity_kps * 1000f, 2f) * 1E-06f);
		default:
			return 0.5f * warheadMass_kg * Mathf.Pow(finalVelocity_kps * 1000f, 2f) * 1E-06f;
		case WarheadClass.Nuclear:
		case WarheadClass.ShapedNuclear:
		case WarheadClass.Antimatter:
			return 0f;
		}
	}

	// Token: 0x060013CF RID: 5071 RVA: 0x0005D194 File Offset: 0x0005B394
	public override float DamageAtRange_MJ(float range_km, float targetCrossSection_m, CombatWeaponCarrierState attacker, float finalVelocity_kps = 0f, float warheadMass_kg = 0f, bool applyChipping = true)
	{
		float num = (this.flatDamage_MJ + this.KineticEnergyDamage_MJ(finalVelocity_kps, warheadMass_kg)) * (applyChipping ? (1f - this.flatChipping) : 1f);
		float num2 = num;
		WarheadClass warheadClass = this.warheadClass;
		if (warheadClass <= WarheadClass.Fragmentation)
		{
			num2 += TIEffectsState.SumEffectsModifiers(Context.ShipConvMissileDamage, (attacker != null) ? attacker.GetFaction() : null, num, null);
			float num3 = num2;
			float? num4;
			if (attacker == null)
			{
				num4 = null;
			}
			else
			{
				TISpaceShipState tispaceShipState = attacker.ref_shipCarrier();
				num4 = ((tispaceShipState != null) ? new float?(tispaceShipState.SumOfficerEffectsModifiers(OfficerEffectType.MissileDamage, num)) : null);
			}
			float? num5 = num4;
			num2 = num3 + num5.GetValueOrDefault();
			float num6 = num2;
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
			num2 = num6 + num5.GetValueOrDefault();
		}
		return num2;
	}

	// Token: 0x060013D0 RID: 5072 RVA: 0x0005D26C File Offset: 0x0005B46C
	public override float BaseDamageAtRange_MJ(float range_km, bool applyChipping = true)
	{
		return (this.flatDamage_MJ + this.KineticEnergyDamage_MJ(this.deltaV_kps, this.warheadMass_kg)) * (applyChipping ? (1f - this.flatChipping) : 1f);
	}

	// Token: 0x060013D1 RID: 5073 RVA: 0x0005D2A0 File Offset: 0x0005B4A0
	public float RangeAtOneDamage_km(WarheadClass warheadClass)
	{
		switch (warheadClass)
		{
		case WarheadClass.Nuclear:
		case WarheadClass.Antimatter:
			return Mathf.Sqrt(this.flatDamage_MJ / 12.566371f / 1000f) / 20f;
		case WarheadClass.ShapedNuclear:
		{
			float num = Mathf.Tan(0.017453292f * this.shapedChargeAngle);
			return Mathf.Sqrt(this.flatDamage_MJ * 0.1f) / 3.1415927f / num / 1000f / 20f;
		}
		default:
			Debug.LogError("Invalid Warhead Type for calculating range for 1 unit of damage.");
			return 0f;
		}
	}

	// Token: 0x060013D2 RID: 5074 RVA: 0x0005D328 File Offset: 0x0005B528
	public override Damage GetComplexDamage(float range_km, IDamageableType targetType, float targetCrossSectionalArea_m2, float relativeVelocity_kps = -1f, CombatWeaponCarrierState attacker = null, TIFactionState attackingFaction = null, float warheadMassOverride_kg = -1f)
	{
		DamageType damageType = this.GetDamageType();
		if (this.AOEWeapon && targetType == IDamageableType.StationModule)
		{
			return new Damage(this, 0f, damageType, this.flatDamage_MJ / 20f, 0f, 0, attackingFaction);
		}
		if (relativeVelocity_kps < 0f)
		{
			relativeVelocity_kps = this.EstimatedImpactVelocity_kps;
		}
		DamageBreakdown damageBreakdown = this.DamageAtRange_points(0f, targetCrossSectionalArea_m2, attacker, relativeVelocity_kps, this.warheadMass_kg, true);
		float num = (this.AOEWeapon ? 0f : damageBreakdown.chippingDamage_Points);
		int num2 = (this.AOEWeapon ? ((int)damageBreakdown.chippingDamage_Points) : 0);
		return new Damage(this, 0f, damageType, damageBreakdown.directDamage_Points, num, num2, attackingFaction);
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x0005D3D4 File Offset: 0x0005B5D4
	public override string SpecificDescriptionData()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(base.SpecificDescriptionData());
		stringBuilder.AppendLine(this.GetLocalizedWarheadTypeAndChipping());
		if (this.warheadClass == WarheadClass.ShapedNuclear)
		{
			stringBuilder.AppendLine(this.GetLocalizedShapeChargeAngle());
		}
		if (this.AOEWeapon)
		{
			stringBuilder.AppendLine(this.GetLocalizedEffectiveAOE());
		}
		stringBuilder.AppendLine(this.GetLocalizedAcceleration());
		stringBuilder.AppendLine(this.GetLocalizedDV());
		return stringBuilder.ToString();
	}

	// Token: 0x060013D4 RID: 5076 RVA: 0x0005D44C File Offset: 0x0005B64C
	public string GetLocalizedWarheadTypeAndChipping()
	{
		string text = Loc.T("TIMissileTemplate.Warhead." + this.warheadClass.ToString(), new object[] { this.flatChipping.ToPercent("P0") });
		return Loc.T("TIMissileTemplate.Warhead", new object[] { text });
	}

	// Token: 0x060013D5 RID: 5077 RVA: 0x0005D4A7 File Offset: 0x0005B6A7
	public string GetLocalizedWarheadType()
	{
		return Loc.T("TIMissileTemplate.WarheadType." + this.warheadClass.ToString());
	}

	// Token: 0x060013D6 RID: 5078 RVA: 0x0005D4CC File Offset: 0x0005B6CC
	public string GetLocalizedAcceleration()
	{
		return Loc.T("TIMissileTemplate.Acc", new object[] { TIUtilities.FormatSmallNumber(this.acceleration_g, 7, 0, true, false) });
	}

	// Token: 0x060013D7 RID: 5079 RVA: 0x0005D4FC File Offset: 0x0005B6FC
	public string GetLocalizedDV()
	{
		return Loc.T("TIMissileTemplate.DV", new object[] { TIUtilities.FormatSmallNumber(this.deltaV_kps, 7, 0, true, false) });
	}

	// Token: 0x060013D8 RID: 5080 RVA: 0x0005D52C File Offset: 0x0005B72C
	public string GetLocalizedShapeChargeAngle()
	{
		return Loc.T("TIMissileTemplate.ShapedNuclearAngle", new object[] { TIUtilities.FormatSmallNumber(this.shapedChargeAngle, 7, 0, true, false) });
	}

	// Token: 0x060013D9 RID: 5081 RVA: 0x0005D55C File Offset: 0x0005B75C
	public string GetLocalizedEffectiveAOE()
	{
		float num = this.RangeAtOneDamage_km(this.warheadClass);
		switch (this.warheadClass)
		{
		case WarheadClass.Nuclear:
		case WarheadClass.Antimatter:
			return Loc.T("TIMissileTemplate.EffectiveAOERange_Sphere", new object[] { num.ToString("N0") });
		case WarheadClass.ShapedNuclear:
			return Loc.T("TIMissileTemplate.EffectiveAOERange_Cone", new object[] { num.ToString("N0") });
		default:
			return string.Empty;
		}
	}

	// Token: 0x060013DA RID: 5082 RVA: 0x0005D5D8 File Offset: 0x0005B7D8
	public override float GetSurfaceImpactVelocity_kps(TISpaceBodyState spaceBody, float altitude_km)
	{
		float num = (float)spaceBody.meanRadius_m + altitude_km * 1000f;
		float num2 = (float)Mathd.Sqrt(spaceBody.mu / (double)num);
		float num3 = this.deltaV_kps * 1000f;
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
			return num13;
		}
		return num9;
	}

	// Token: 0x060013DB RID: 5083 RVA: 0x0005D733 File Offset: 0x0005B933
	public override bool CanOnlyDefensivelyTargetMissiles()
	{
		return this.defenseMode;
	}

	// Token: 0x060013DC RID: 5084 RVA: 0x0005D73B File Offset: 0x0005B93B
	public override float EffectiveRangeAgainstProjectiles_km()
	{
		return this.targetingRange_km;
	}

	// Token: 0x04001193 RID: 4499
	public float acceleration_g;

	// Token: 0x04001194 RID: 4500
	public float deltaV_kps;

	// Token: 0x04001195 RID: 4501
	public float rotation_degps;

	// Token: 0x04001196 RID: 4502
	public float thrustRamp_s;

	// Token: 0x04001197 RID: 4503
	public float turnRamp_s;

	// Token: 0x04001198 RID: 4504
	public float maneuver_angle;

	// Token: 0x04001199 RID: 4505
	public WarheadClass warheadClass;

	// Token: 0x0400119A RID: 4506
	public float shapedChargeAngle;

	// Token: 0x0400119B RID: 4507
	public const float shapedChargeEfficiency = 0.1f;
}
