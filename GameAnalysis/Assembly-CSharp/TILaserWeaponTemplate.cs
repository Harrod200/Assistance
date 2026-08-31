using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

// Token: 0x020003DD RID: 989
public class TILaserWeaponTemplate : TIBeamWeaponTemplate
{
	// Token: 0x17000279 RID: 633
	// (get) Token: 0x06001351 RID: 4945 RVA: 0x0005B907 File Offset: 0x00059B07
	public override WeaponClass weaponClass
	{
		get
		{
			return WeaponClass.Laser;
		}
	}

	// Token: 0x1700027A RID: 634
	// (get) Token: 0x06001352 RID: 4946 RVA: 0x0005B90A File Offset: 0x00059B0A
	public override bool isLaserWeapon
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700027B RID: 635
	// (get) Token: 0x06001353 RID: 4947 RVA: 0x0005B90D File Offset: 0x00059B0D
	public override bool canBombardThroughAtmosphere
	{
		get
		{
			return this.wavelength_nm >= 380 && this.wavelength_nm <= 740;
		}
	}

	// Token: 0x1700027C RID: 636
	// (get) Token: 0x06001354 RID: 4948 RVA: 0x0005B92E File Offset: 0x00059B2E
	public override TILaserWeaponTemplate ref_laserWeapon
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06001355 RID: 4949 RVA: 0x0005B931 File Offset: 0x00059B31
	public float ModifyArmorValueForLaserShot(float range_km, float baseArmorValue, float armorEffectiveness = -1f)
	{
		if (armorEffectiveness < 0f)
		{
			armorEffectiveness = this.ArmorEffectivenessAtRange(range_km);
		}
		return Mathf.Pow(baseArmorValue, 1.5f) * armorEffectiveness;
	}

	// Token: 0x06001356 RID: 4950 RVA: 0x0005B951 File Offset: 0x00059B51
	public float ArmorEffectivenessAtRange(float range_km)
	{
		return this.SpotAreaPrecise_m2(range_km) / 0.005f;
	}

	// Token: 0x06001357 RID: 4951 RVA: 0x0005B960 File Offset: 0x00059B60
	public override float RangeToDoDamage_km(float desiredDamage_Points, TISpaceShipState ship)
	{
		float num;
		if (ship == null)
		{
			num = (float)this.shotPower_MJ / 20f;
		}
		else
		{
			if (this._damageMinRange.ContainsKey(desiredDamage_Points))
			{
				return this._damageMinRange[desiredDamage_Points];
			}
			num = this.DamageAtRange_MJ(1f, 20f, ship, 0f, 0f, true) / 20f;
		}
		float num2 = 0.005f * num / (0.7853982f * desiredDamage_Points);
		num2 = Mathf.Sqrt(num2);
		num2 /= this._SpotDiameterPreciseFactor_m;
		if (ship == null)
		{
			this._damageMinRange[desiredDamage_Points] = num2;
		}
		return num2;
	}

	// Token: 0x06001358 RID: 4952 RVA: 0x0005B9FC File Offset: 0x00059BFC
	public float EstimatedDamageAtRange_MJ(float range_km, float targetCrossSection_m, CombatWeaponCarrierState attackingShip)
	{
		if (TILaserWeaponTemplate.ExpectedArmorHistogram == null)
		{
			TILaserWeaponTemplate.ExpectedArmorHistogram = new Dictionary<int, float>();
			TILaserWeaponTemplate.ExpectedArmorHistogram[3] = 2f;
			TILaserWeaponTemplate.ExpectedArmorHistogram[5] = 3f;
			TILaserWeaponTemplate.ExpectedArmorHistogram[7] = 4f;
			TILaserWeaponTemplate.ExpectedArmorHistogram[10] = 12f;
			TILaserWeaponTemplate.ExpectedArmorHistogram[15] = 8f;
			TILaserWeaponTemplate.ExpectedArmorHistogram[25] = 8f;
			TILaserWeaponTemplate.ExpectedArmorHistogram[35] = 12f;
			TILaserWeaponTemplate.ExpectedArmorHistogram[45] = 8f;
			TILaserWeaponTemplate.ExpectedArmorHistogram[65] = 4f;
			TILaserWeaponTemplate.ExpectedArmorHistogram[100] = 2f;
			float num = TILaserWeaponTemplate.ExpectedArmorHistogram.Values.Sum();
			foreach (KeyValuePair<int, float> keyValuePair in TILaserWeaponTemplate.ExpectedArmorHistogram.ToList<KeyValuePair<int, float>>())
			{
				TILaserWeaponTemplate.ExpectedArmorHistogram[keyValuePair.Key] = keyValuePair.Value / num;
			}
		}
		float baseDamage_MJ = this.DamageAtRange_MJ(range_km, targetCrossSection_m, attackingShip, 0f, 0f, true);
		float armorEffectiveness = this.ArmorEffectivenessAtRange(range_km);
		float num2 = TILaserWeaponTemplate.ExpectedArmorHistogram.Sum<KeyValuePair<int, float>>(delegate(KeyValuePair<int, float> entry)
		{
			int key = entry.Key;
			float num3 = this.ModifyArmorValueForLaserShot(range_km, (float)key, armorEffectiveness);
			return Mathf.Max(baseDamage_MJ + (float)key - num3, 0f) * entry.Value;
		});
		if (num2 < baseDamage_MJ)
		{
			num2 = 0.3f * baseDamage_MJ + 0.7f * num2;
		}
		return num2;
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x0005BBB0 File Offset: 0x00059DB0
	public override float DamageAtRange_MJ(float range_km, float targetCrossSectionalArea_m2, CombatWeaponCarrierState attackingShip, float finalVelocity_kps = 0f, float warheadMass_kg = 0f, bool applyChipping = true)
	{
		float num = (float)this.shotPower_MJ;
		float? num2;
		if (attackingShip == null)
		{
			num2 = null;
		}
		else
		{
			TISpaceShipState tispaceShipState = attackingShip.ref_shipCarrier();
			num2 = ((tispaceShipState != null) ? new float?(tispaceShipState.GetBonusPowerForWeapon_MJ(this)) : null);
		}
		float? num3 = num2;
		float num4 = num + num3.GetValueOrDefault();
		float num5 = num4;
		float num6 = TIEffectsState.SumEffectsModifiers(Context.ShipLaserDamage, (attackingShip != null) ? attackingShip.GetFaction() : null, num4, null);
		float? num7;
		if (attackingShip == null)
		{
			num7 = null;
		}
		else
		{
			TISpaceShipState tispaceShipState2 = attackingShip.ref_shipCarrier();
			num7 = ((tispaceShipState2 != null) ? new float?(tispaceShipState2.SumOfficerEffectsModifiers(OfficerEffectType.BeamDamage, num4)) : null);
		}
		num3 = num7;
		float num8 = num6 + num3.GetValueOrDefault();
		float? num9;
		if (attackingShip == null)
		{
			num9 = null;
		}
		else
		{
			TISpaceShipState tispaceShipState3 = attackingShip.ref_shipCarrier();
			num9 = ((tispaceShipState3 != null) ? new float?(tispaceShipState3.SumOfficerEffectsModifiers(OfficerEffectType.GlobalDamage, num4)) : null);
		}
		num3 = num9;
		num4 = num5 + (num8 + num3.GetValueOrDefault());
		float num10 = this.SpotAreaPrecise_m2(range_km);
		if (num10 > targetCrossSectionalArea_m2)
		{
			num4 *= targetCrossSectionalArea_m2 / num10;
		}
		return num4;
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x0005BC9B File Offset: 0x00059E9B
	public override float BaseDamageAtRange_MJ(float range_km, bool applyChipping = true)
	{
		return (float)this.shotPower_MJ;
	}

	// Token: 0x0600135B RID: 4955 RVA: 0x0005BCA4 File Offset: 0x00059EA4
	public override float chipping(float range_km)
	{
		return Mathf.Clamp(this.ArmorEffectivenessAtRange(range_km) / 1000f, 0f, 0.25f);
	}

	// Token: 0x0600135C RID: 4956 RVA: 0x0005BCC2 File Offset: 0x00059EC2
	public override float EffectiveRangeAgainstProjectiles_km()
	{
		return Mathf.Max(200f, this.attackMode ? (this.targetingRange_km / 3f) : this.targetingRange_km);
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x0005BCEC File Offset: 0x00059EEC
	private float SpotDiameterPrecise_m(float range_km)
	{
		if (this._SpotDiameterPreciseFactor_m == -1f)
		{
			this._SpotDiameterPreciseFactor_m = 1000f * Mathf.Sqrt(Mathf.Pow(1.22f * this.wavelength_m * this.beam_quality, 2f) + Mathf.Pow(2f * this.jitter_Rad * this.mirrorDiameter_m, 2f)) / this.mirrorDiameter_m;
		}
		return range_km * this._SpotDiameterPreciseFactor_m;
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x0005BD61 File Offset: 0x00059F61
	private float SpotAreaPrecise_m2(float range_km)
	{
		return 0.7853982f * this.SpotDiameterPrecise_m(range_km) * this.SpotDiameterPrecise_m(range_km);
	}

	// Token: 0x1700027D RID: 637
	// (get) Token: 0x0600135F RID: 4959 RVA: 0x0005BD78 File Offset: 0x00059F78
	private float mirrorDiameter_m
	{
		get
		{
			return this.mirrorRadius_m * 2f;
		}
	}

	// Token: 0x1700027E RID: 638
	// (get) Token: 0x06001360 RID: 4960 RVA: 0x0005BD86 File Offset: 0x00059F86
	private float mirrorRadius_m
	{
		get
		{
			return (float)this.mirrorRadius_cm / 100f;
		}
	}

	// Token: 0x1700027F RID: 639
	// (get) Token: 0x06001361 RID: 4961 RVA: 0x0005BD95 File Offset: 0x00059F95
	private float wavelength_m
	{
		get
		{
			return (float)this.wavelength_nm / 1E+09f;
		}
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x0005BDA4 File Offset: 0x00059FA4
	public override string SpecificDescriptionData()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(this.GetLocalizedWaveLength());
		stringBuilder.AppendLine(this.GetLocalizedPowerAndDamageAtRange(base.shortRange));
		if (this.attackMode)
		{
			stringBuilder.AppendLine(this.GetLocalizedArmorEffectivenessAtRange(base.shortRange));
			stringBuilder.AppendLine(this.GetLocalizedArmorEffectivenessAtRange(base.mediumRange));
			stringBuilder.AppendLine(this.GetLocalizedArmorEffectivenessAtRange(base.longRange));
		}
		stringBuilder.AppendLine(base.GetLocalizedCooldown());
		return stringBuilder.ToString();
	}

	// Token: 0x06001363 RID: 4963 RVA: 0x0005BE2B File Offset: 0x0005A02B
	public string GetLocalizedWaveLength()
	{
		return Loc.T("TILaserWeaponTemplate.Wavelength", new object[] { this.wavelength_nm.ToString("N0") });
	}

	// Token: 0x06001364 RID: 4964 RVA: 0x0005BE50 File Offset: 0x0005A050
	public override string GetLocalizedPowerAndDamageAtRange(float range)
	{
		return Loc.T("TIBeamWeaponTemplate.Power", new object[]
		{
			this.shotPower_MJ.ToString("N0"),
			this.BaseDamageAtRange_points(range, false).ToString("N1")
		});
	}

	// Token: 0x06001365 RID: 4965 RVA: 0x0005BE98 File Offset: 0x0005A098
	public string GetLocalizedArmorEffectivenessAtRange(float range)
	{
		return Loc.T("TILaserWeaponTemplate.ArmorPiercingAtRange", new object[]
		{
			range.ToString("N0"),
			this.ArmorEffectivenessAtRange(range).ToPercent("P0")
		});
	}

	// Token: 0x06001366 RID: 4966 RVA: 0x0005BED0 File Offset: 0x0005A0D0
	public static TILaserWeaponTemplate GetBestHeavyDefenseLaser(TIFactionState faction, TISpaceBodyState spaceBody, int tier)
	{
		float laserValue;
		if (faction == null)
		{
			laserValue = 1.1f;
		}
		else if (faction.IsAlienFaction)
		{
			laserValue = 9.9f;
		}
		else if (tier == 0)
		{
			laserValue = TIEffectsState.SumEffectsModifiers(Context.LaserDefenseType, faction, 0f, null) + Mathf.Min(0.2f, TIEffectsState.SumEffectsModifiers(Context.LaserDefenseFreq, faction, 0f, null));
		}
		else
		{
			laserValue = TIEffectsState.SumEffectsModifiers(Context.LaserDefenseType, faction, 0f, null) + ((spaceBody.atmosphere > Atmosphere.Thin) ? Mathf.Min(0.2f, TIEffectsState.SumEffectsModifiers(Context.LaserDefenseFreq, faction, 0f, null)) : TIEffectsState.SumEffectsModifiers(Context.LaserDefenseFreq, faction, 0f, null));
		}
		Mount mount;
		switch (tier)
		{
		case 0:
			mount = Mount.RegionDefense;
			goto IL_00EA;
		case 2:
			mount = Mount.T2BaseDefense;
			goto IL_00EA;
		case 3:
			mount = Mount.T3BaseDefense;
			goto IL_00EA;
		}
		mount = Mount.T1BaseDefense;
		IL_00EA:
		TILaserWeaponTemplate tilaserWeaponTemplate = TemplateManager.IterateByClass<TILaserWeaponTemplate>(true).FirstOrDefault<TILaserWeaponTemplate>((TILaserWeaponTemplate x) => x.mount == mount && x.bombardmentValue == laserValue);
		if (tilaserWeaponTemplate == null)
		{
			Log.Warn("No defense laser weapon template configured at " + laserValue.ToString(), Array.Empty<object>());
			switch (tier)
			{
			case 0:
				return TemplateManager.Find<TILaserWeaponTemplate>("RegionDefenseIRLaser", false);
			case 2:
				return TemplateManager.Find<TILaserWeaponTemplate>("T2BaseIRLaser", false);
			case 3:
				return TemplateManager.Find<TILaserWeaponTemplate>("T3BaseIRLaser", false);
			}
			tilaserWeaponTemplate = TemplateManager.Find<TILaserWeaponTemplate>("T1BaseIRLaser", false);
		}
		return tilaserWeaponTemplate;
	}

	// Token: 0x06001367 RID: 4967 RVA: 0x0005C050 File Offset: 0x0005A250
	public override DamageType GetDamageType()
	{
		return DamageType.Thermal;
	}

	// Token: 0x06001368 RID: 4968 RVA: 0x0005C054 File Offset: 0x0005A254
	public override float EstimateDPS(float expectedRange, TISpaceShipTemplate target, bool applyOverkillPenalty)
	{
		float num = (((this.targetingRange_km >= expectedRange) ? (this.EstimatedDamageAtRange_MJ(expectedRange, (target != null) ? target.hullTemplate.width_m : 50f, null) * 0.3f) : 0f) + ((this.targetingRange_km >= 200f) ? (this.EstimatedDamageAtRange_MJ(200f, (target != null) ? target.hullTemplate.width_m : 50f, null) * 0.15f) : 0f) + ((this.targetingRange_km >= 500f) ? (this.EstimatedDamageAtRange_MJ(500f, (target != null) ? target.hullTemplate.width_m : 50f, null) * 0.25f) : 0f) + ((this.targetingRange_km >= 800f) ? (this.EstimatedDamageAtRange_MJ(800f, (target != null) ? target.hullTemplate.width_m : 50f, null) * 0.3f) : 0f)) / 20f;
		if (applyOverkillPenalty)
		{
			num = base.ApplyOverkillPenalty(num);
		}
		return num / base.averageCooldown_s;
	}

	// Token: 0x04001176 RID: 4470
	public int mirrorRadius_cm;

	// Token: 0x04001177 RID: 4471
	public int wavelength_nm;

	// Token: 0x04001178 RID: 4472
	public float beam_quality = 1.3f;

	// Token: 0x04001179 RID: 4473
	public float jitter_Rad = 2.5E-07f;

	// Token: 0x0400117A RID: 4474
	private static Dictionary<int, float> ExpectedArmorHistogram;

	// Token: 0x0400117B RID: 4475
	private float _SpotDiameterPreciseFactor_m = -1f;
}
