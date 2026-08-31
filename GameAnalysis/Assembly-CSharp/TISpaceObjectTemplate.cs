using System;
using System.Threading;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020003F3 RID: 1011
public class TISpaceObjectTemplate : TIDataTemplate
{
	// Token: 0x170002B0 RID: 688
	// (get) Token: 0x060013F3 RID: 5107 RVA: 0x0005DA05 File Offset: 0x0005BC05
	public virtual string ModelResource
	{
		get
		{
			if (!string.IsNullOrEmpty(this.modelResource))
			{
				return this.modelResource;
			}
			return "placeholders/PlaceholderSpaceObject";
		}
	}

	// Token: 0x170002B1 RID: 689
	// (get) Token: 0x060013F4 RID: 5108 RVA: 0x0005DA20 File Offset: 0x0005BC20
	public virtual float ModelScale
	{
		get
		{
			return this.modelScale;
		}
	}

	// Token: 0x170002B2 RID: 690
	// (get) Token: 0x060013F5 RID: 5109 RVA: 0x0005DA28 File Offset: 0x0005BC28
	public virtual double Eccentricity
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

	// Token: 0x170002B3 RID: 691
	// (get) Token: 0x060013F6 RID: 5110 RVA: 0x0005DA56 File Offset: 0x0005BC56
	public virtual double Inclination_Rad
	{
		get
		{
			return this.inclination_Deg.GetValueOrDefault() * 0.017453292519943295;
		}
	}

	// Token: 0x170002B4 RID: 692
	// (get) Token: 0x060013F7 RID: 5111 RVA: 0x0005DA70 File Offset: 0x0005BC70
	public virtual double LongitudeAscendingNode_Rad
	{
		get
		{
			double? num = 0.017453292519943295 * this.longAscendingNode_Deg;
			if (num == null)
			{
				return this.genLongitudeAscendingNode_deg;
			}
			return num.GetValueOrDefault();
		}
	}

	// Token: 0x170002B5 RID: 693
	// (get) Token: 0x060013F8 RID: 5112 RVA: 0x0005DAC8 File Offset: 0x0005BCC8
	public virtual double Epoch_floatJYears
	{
		get
		{
			double? num = this.epoch_floatJYears;
			if (num == null)
			{
				return 2000.0;
			}
			return num.GetValueOrDefault();
		}
	}

	// Token: 0x170002B6 RID: 694
	// (get) Token: 0x060013F9 RID: 5113 RVA: 0x0005DAF8 File Offset: 0x0005BCF8
	public double LongitudePeriapsis_Deg
	{
		get
		{
			double? num = this.longPeriapsis_Deg;
			if (num == null)
			{
				return double.Epsilon;
			}
			return num.GetValueOrDefault();
		}
	}

	// Token: 0x170002B7 RID: 695
	// (get) Token: 0x060013FA RID: 5114 RVA: 0x0005DB28 File Offset: 0x0005BD28
	public double Mass
	{
		get
		{
			double? num = this.mass_kg;
			if (num == null)
			{
				return double.Epsilon;
			}
			return num.GetValueOrDefault();
		}
	}

	// Token: 0x170002B8 RID: 696
	// (get) Token: 0x060013FB RID: 5115 RVA: 0x0005DB56 File Offset: 0x0005BD56
	public TINaturalSpaceObjectTemplate barycenterTemplate
	{
		get
		{
			return TemplateManager.Find<TINaturalSpaceObjectTemplate>(this.barycenterName, true);
		}
	}

	// Token: 0x170002B9 RID: 697
	// (get) Token: 0x060013FC RID: 5116 RVA: 0x0005DB64 File Offset: 0x0005BD64
	public virtual double SemiMajorAxis_m
	{
		get
		{
			if (this.semiMajorAxis_km != null)
			{
				return this.semiMajorAxis_km.Value * 1000.0;
			}
			if (this.semiMajorAxis_AU != null)
			{
				return this.semiMajorAxis_AU.Value * 149597870700.0;
			}
			Debug.Log("Space object " + this.displayName + " has no semiMajorAxis value defined");
			return 0.0;
		}
	}

	// Token: 0x170002BA RID: 698
	// (get) Token: 0x060013FD RID: 5117 RVA: 0x0005DBDC File Offset: 0x0005BDDC
	public virtual double ArgumentPeriapsis_Rad
	{
		get
		{
			if (this.argPeriapsis_Deg != null)
			{
				return 0.017453292519943295 * this.argPeriapsis_Deg.Value;
			}
			if (this.longPeriapsis_Deg != null && this.longAscendingNode_Deg != null)
			{
				return 0.017453292519943295 * (this.longPeriapsis_Deg.Value - this.longAscendingNode_Deg.Value);
			}
			return this.genArgumentPeriapsis_deg * 0.017453292519943295;
		}
	}

	// Token: 0x170002BB RID: 699
	// (get) Token: 0x060013FE RID: 5118 RVA: 0x0005DC58 File Offset: 0x0005BE58
	public virtual double MeanAnomalyAtEpoch_Rad
	{
		get
		{
			if (this.meanAnomalyAtEpoch_Deg != null)
			{
				return 0.017453292519943295 * this.meanAnomalyAtEpoch_Deg.Value;
			}
			if (this.meanLongitude_Deg != null && this.longPeriapsis_Deg != null)
			{
				return 0.017453292519943295 * (this.meanLongitude_Deg.Value - this.longPeriapsis_Deg.Value);
			}
			return this.genMeanAnomalyEpoch_deg * 0.017453292519943295;
		}
	}

	// Token: 0x170002BC RID: 700
	// (get) Token: 0x060013FF RID: 5119 RVA: 0x0005DCD4 File Offset: 0x0005BED4
	public double MeanLongitude_Rad
	{
		get
		{
			if (this.meanLongitude_Deg == null)
			{
				return this.LongitudeAscendingNode_Rad + this.ArgumentPeriapsis_Rad + this.MeanAnomalyAtEpoch_Rad;
			}
			return 0.017453292519943295 * this.meanLongitude_Deg.Value;
		}
	}

	// Token: 0x04001201 RID: 4609
	public SpaceObjectType objectType;

	// Token: 0x04001202 RID: 4610
	public string barycenterName;

	// Token: 0x04001203 RID: 4611
	public double? semiMajorAxis_AU;

	// Token: 0x04001204 RID: 4612
	public double? semiMajorAxis_km;

	// Token: 0x04001205 RID: 4613
	public double? eccentricity;

	// Token: 0x04001206 RID: 4614
	public double? inclination_Deg;

	// Token: 0x04001207 RID: 4615
	public double? longAscendingNode_Deg;

	// Token: 0x04001208 RID: 4616
	public double? argPeriapsis_Deg;

	// Token: 0x04001209 RID: 4617
	public double? meanAnomalyAtEpoch_Deg;

	// Token: 0x0400120A RID: 4618
	public double? longPeriapsis_Deg;

	// Token: 0x0400120B RID: 4619
	public double? meanLongitude_Deg;

	// Token: 0x0400120C RID: 4620
	public double? apsidalPrecession_Years;

	// Token: 0x0400120D RID: 4621
	public double? nodalPrecession_Years;

	// Token: 0x0400120E RID: 4622
	public double? epoch_floatJYears = new double?((double)2000);

	// Token: 0x0400120F RID: 4623
	public double? mass_kg;

	// Token: 0x04001210 RID: 4624
	public string modelResource;

	// Token: 0x04001211 RID: 4625
	public float modelScale = 0.5f;

	// Token: 0x04001212 RID: 4626
	public string symbolTexture;

	// Token: 0x04001213 RID: 4627
	private readonly double genLongitudeAscendingNode_deg = (TIUtilities.IsMainThread(Thread.CurrentThread) ? ((double)TIUtilities.RandomRange(0f, 360f)) : TIUtilities.RandomDouble(0.0, 360.0));

	// Token: 0x04001214 RID: 4628
	private readonly double genArgumentPeriapsis_deg = (TIUtilities.IsMainThread(Thread.CurrentThread) ? ((double)TIUtilities.RandomRange(0f, 360f)) : TIUtilities.RandomDouble(0.0, 360.0));

	// Token: 0x04001215 RID: 4629
	private readonly double genMeanAnomalyEpoch_deg = (TIUtilities.IsMainThread(Thread.CurrentThread) ? ((double)TIUtilities.RandomRange(0f, 360f)) : TIUtilities.RandomDouble(0.0, 360.0));
}
