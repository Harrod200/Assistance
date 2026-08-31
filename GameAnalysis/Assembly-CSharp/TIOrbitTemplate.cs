using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000381 RID: 897
public class TIOrbitTemplate : TIDataTemplate
{
	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x06001024 RID: 4132 RVA: 0x00053F4C File Offset: 0x0005214C
	public double Eccentricity
	{
		get
		{
			double? num = this.eccentricity;
			if (num == null)
			{
				return double.Epsilon;
			}
			return num.GetValueOrDefault();
		}
	}

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x06001025 RID: 4133 RVA: 0x00053F7A File Offset: 0x0005217A
	public double Inclination_Rad
	{
		get
		{
			return this.inclination_Deg.GetValueOrDefault() * 0.017453292519943295;
		}
	}

	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x06001026 RID: 4134 RVA: 0x00053F91 File Offset: 0x00052191
	public double LongitudeAscendingNode_Rad
	{
		get
		{
			return this.longAscendingNode_Deg * 0.017453292519943295;
		}
	}

	// Token: 0x170001E6 RID: 486
	// (get) Token: 0x06001027 RID: 4135 RVA: 0x00053FA3 File Offset: 0x000521A3
	public double ArgPeriapsis_Rad
	{
		get
		{
			return this.argPeriapsis_Deg * 0.017453292519943295;
		}
	}

	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x06001028 RID: 4136 RVA: 0x00053FB5 File Offset: 0x000521B5
	public double LongitudePeriapsis
	{
		get
		{
			return this.ArgPeriapsis_Rad + this.LongitudeAscendingNode_Rad;
		}
	}

	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x06001029 RID: 4137 RVA: 0x00053FC4 File Offset: 0x000521C4
	public bool irradiated
	{
		get
		{
			return this.irradiatedMultiplier > 1f;
		}
	}

	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x0600102A RID: 4138 RVA: 0x00053FD3 File Offset: 0x000521D3
	public TINaturalSpaceObjectTemplate barycenterTemplate
	{
		get
		{
			return TemplateManager.Find<TINaturalSpaceObjectTemplate>(this.barycenterName, true);
		}
	}

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x0600102B RID: 4139 RVA: 0x00053FE1 File Offset: 0x000521E1
	public TINaturalSpaceObjectState barycenter
	{
		get
		{
			return GameStateManager.FindByTemplate<TINaturalSpaceObjectState>(this.barycenterName, true);
		}
	}

	// Token: 0x170001EB RID: 491
	// (get) Token: 0x0600102C RID: 4140 RVA: 0x00053FEF File Offset: 0x000521EF
	public float randomSemiMajorAxisRangeValue_km
	{
		get
		{
			return TIUtilities.RandomRange(-this.semiMajorAxisRange_km, this.semiMajorAxisRange_km);
		}
	}

	// Token: 0x170001EC RID: 492
	// (get) Token: 0x0600102D RID: 4141 RVA: 0x00054003 File Offset: 0x00052203
	public float randomInclinationRangeValue_Deg
	{
		get
		{
			return TIUtilities.RandomRange(0f, Mathf.Clamp(this.inclinationRange_Deg, 0f, 90f));
		}
	}

	// Token: 0x170001ED RID: 493
	// (get) Token: 0x0600102E RID: 4142 RVA: 0x00054024 File Offset: 0x00052224
	public double randomAnomaly_Rad
	{
		get
		{
			return (double)TIUtilities.RandomRange(0f, 360f) * 0.017453292519943295;
		}
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x00054040 File Offset: 0x00052240
	public override TIGameState CreateGameState()
	{
		base.CreateGameState();
		return GameStateManager.CreateNewGameState<TIOrbitState>();
	}

	// Token: 0x170001EE RID: 494
	// (get) Token: 0x06001030 RID: 4144 RVA: 0x00054050 File Offset: 0x00052250
	public double SemiMajorAxis_m
	{
		get
		{
			if (this._semimajorAxis_m > 0.0)
			{
				return this._semimajorAxis_m;
			}
			double num = 0.0;
			TISpaceBodyState tispaceBodyState = this.barycenter as TISpaceBodyState;
			double num2 = ((tispaceBodyState != null) ? (tispaceBodyState.maxRadiusDimension_m + 10000.0) : (this.barycenter.meanRadius_m + 1000000.0));
			if (this.semiMajorAxis_km != null)
			{
				num = this.semiMajorAxis_km.Value * 1000.0;
			}
			else if (this.altitude_km != null)
			{
				num = (this.barycenter.meanRadius_km + this.altitude_km.Value) * 1000.0;
			}
			else if (this.semiMajorAxis_AU != null)
			{
				num = this.semiMajorAxis_AU.Value * 149597870700.0;
			}
			else if (this.synch)
			{
				if (tispaceBodyState != null)
				{
					num = tispaceBodyState.stationaryOrbitRadius_m;
				}
				else
				{
					num = num2;
				}
			}
			else if (this.radialOrbit)
			{
				if (tispaceBodyState != null)
				{
					num = tispaceBodyState.maxRadiusDimension_m * 3.25;
				}
				else
				{
					num = this.barycenter.meanRadius_m + 1000000.0;
				}
			}
			if (tispaceBodyState != null)
			{
				num = Mathd.Min(num, tispaceBodyState.hillRadius_m);
			}
			bool flag = false;
			num = Mathd.Max(num, num2);
			string text = string.Empty;
			if (this.synch && tispaceBodyState != null && num != tispaceBodyState.stationaryOrbitRadius_m)
			{
				text = text + "Game calculated synchronous orbit that doesn't exist for " + tispaceBodyState.displayName + ". For realism, don't assign a synch orbit for this body.\n";
			}
			else if (tispaceBodyState != null && num > tispaceBodyState.hillRadius_m)
			{
				flag = true;
			}
			if (this.interfaceOrbit && !this.synch)
			{
				TISpaceBodyState tispaceBodyState2 = this.barycenter as TISpaceBodyState;
				if (tispaceBodyState2 != null)
				{
					double num3 = num;
					double rotationperiod_s = tispaceBodyState2.rotationperiod_s;
					double num4 = 6.283185307179586 * Mathd.Sqrt(num * num * num / this.barycenter.mu);
					if (num4 * 0.6699999570846558 < rotationperiod_s && num4 * 1.3300000429153442 > rotationperiod_s)
					{
						double num5 = ((num3 < tispaceBodyState2.hillRadius_m) ? tispaceBodyState2.hillRadius_m : (flag ? (num2 * 5.0) : num2));
						double num6 = num2;
						bool flag2 = false;
						for (int i = 1; i < 96; i++)
						{
							num *= 0.949999988079071;
							num4 = 6.283185307179586 * Mathd.Sqrt(num * num * num / this.barycenter.mu);
							if (num4 * 0.6699999570846558 >= rotationperiod_s || num4 * 1.3300000429153442 <= rotationperiod_s)
							{
								flag2 = true;
								break;
							}
							if (num < num6)
							{
								num = num6;
								break;
							}
						}
						if (!flag2)
						{
							num = num3;
							for (int j = 1; j < 301; j++)
							{
								num *= 1.0099999904632568;
								num4 = 6.283185307179586 * Mathd.Sqrt(num * num * num / this.barycenter.mu);
								if (num4 * 0.6699999570846558 >= rotationperiod_s || num4 * 1.3300000429153442 <= rotationperiod_s)
								{
									flag2 = true;
									break;
								}
								if (num > num5)
								{
									num = num5;
									break;
								}
							}
						}
						if (!flag2)
						{
							text = string.Concat(new string[]
							{
								text,
								"Could not create an interface orbit around ",
								this.barycenter.templateName,
								" that is not a nearly synchronous orbit. This will cause problems for bombardment: Barycenter rotation: ",
								(rotationperiod_s / 3600.0).ToString(),
								" hours. Orbital period: ",
								(num4 / 3600.0).ToString(),
								" hours.\n"
							});
						}
					}
				}
			}
			if ((this.barycenter.isSpaceBodyState && num > this.barycenter.meanRadius_m + 1000.0) || (this.barycenter.isLagrangePointState && num > 1000.0))
			{
				this._semimajorAxis_m = num;
			}
			else
			{
				this._semimajorAxis_m = (this.barycenter.meanRadius_km + 2000.0) * 1000.0;
			}
			if (text != string.Empty)
			{
				Log.Warn(text, Array.Empty<object>());
			}
			return this._semimajorAxis_m;
		}
	}

	// Token: 0x170001EF RID: 495
	// (get) Token: 0x06001031 RID: 4145 RVA: 0x0005448D File Offset: 0x0005268D
	public double SemiMajorAxis_km
	{
		get
		{
			return this.SemiMajorAxis_m / 1000.0;
		}
	}

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x06001032 RID: 4146 RVA: 0x0005449F File Offset: 0x0005269F
	public double SemiMajorAxis_AU
	{
		get
		{
			return this.SemiMajorAxis_m / 149597870700.0;
		}
	}

	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x06001033 RID: 4147 RVA: 0x000544B1 File Offset: 0x000526B1
	public double Altitude_km
	{
		get
		{
			return this.SemiMajorAxis_km - this.barycenter.meanRadius_km;
		}
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x000544C8 File Offset: 0x000526C8
	public OrbitalElementsState Generate(bool allowRandoms = true, bool assignRandomAnomaly = true)
	{
		OrbitalElementsState orbitalElementsState;
		orbitalElementsState.semiMajorAxis_m = this.SemiMajorAxis_m + (double)(allowRandoms ? (this.randomSemiMajorAxisRangeValue_km * 1000f) : 0f);
		orbitalElementsState.eccentricity = this.Eccentricity;
		orbitalElementsState.inclination_Rad = this.Inclination_Rad + (allowRandoms ? (0.017453292519943295 * (double)this.randomInclinationRangeValue_Deg) : 0.0);
		orbitalElementsState.longAscendingNode_Rad = this.LongitudeAscendingNode_Rad;
		orbitalElementsState.argPeriapsis_Rad = this.ArgPeriapsis_Rad;
		orbitalElementsState.epoch = TITimeState.SystemNow();
		if (assignRandomAnomaly)
		{
			orbitalElementsState.meanAnomalyAtEpoch_Rad = this.randomAnomaly_Rad;
		}
		else
		{
			orbitalElementsState.meanAnomalyAtEpoch_Rad = 0.0;
		}
		return orbitalElementsState;
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x00054580 File Offset: 0x00052780
	public OrbitalElementsState Generate(bool allowRandoms, double meanAnomalyAtEpoch_Rad, TIDateTime epoch)
	{
		OrbitalElementsState orbitalElementsState = this.Generate(allowRandoms, false);
		orbitalElementsState.meanAnomalyAtEpoch_Rad = meanAnomalyAtEpoch_Rad;
		orbitalElementsState.epoch = epoch.ExportTime();
		return orbitalElementsState;
	}

	// Token: 0x0400109E RID: 4254
	public string abbreviation;

	// Token: 0x0400109F RID: 4255
	public string description;

	// Token: 0x040010A0 RID: 4256
	public string barycenterName;

	// Token: 0x040010A1 RID: 4257
	public double? semiMajorAxis_AU;

	// Token: 0x040010A2 RID: 4258
	public double? semiMajorAxis_km;

	// Token: 0x040010A3 RID: 4259
	public double? altitude_km;

	// Token: 0x040010A4 RID: 4260
	public bool earthLEO;

	// Token: 0x040010A5 RID: 4261
	public bool synch;

	// Token: 0x040010A6 RID: 4262
	public float semiMajorAxisRange_km = 50f;

	// Token: 0x040010A7 RID: 4263
	public double? eccentricity;

	// Token: 0x040010A8 RID: 4264
	public double? inclination_Deg;

	// Token: 0x040010A9 RID: 4265
	public float inclinationRange_Deg = 10f;

	// Token: 0x040010AA RID: 4266
	public double longAscendingNode_Deg;

	// Token: 0x040010AB RID: 4267
	public double argPeriapsis_Deg;

	// Token: 0x040010AC RID: 4268
	public float irradiatedMultiplier;

	// Token: 0x040010AD RID: 4269
	public bool interfaceOrbit;

	// Token: 0x040010AE RID: 4270
	public bool radialOrbit;

	// Token: 0x040010AF RID: 4271
	public int stationCapacity = 1;

	// Token: 0x040010B0 RID: 4272
	public float amat_ugpy;

	// Token: 0x040010B1 RID: 4273
	public string effectToExplore;

	// Token: 0x040010B2 RID: 4274
	private double _semimajorAxis_m = -1.0;

	// Token: 0x040010B3 RID: 4275
	private const float tooClose = 0.33f;
}
