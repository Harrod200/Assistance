using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003EC RID: 1004
public class TISpaceBodyTemplate : TINaturalSpaceObjectTemplate
{
	// Token: 0x060013E0 RID: 5088 RVA: 0x0005D778 File Offset: 0x0005B978
	public override TIGameState CreateGameState()
	{
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TISpaceBodyState>();
		}
		return tigameState;
	}

	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x060013E1 RID: 5089 RVA: 0x0005D79C File Offset: 0x0005B99C
	public string bodyAltName
	{
		get
		{
			return Loc.T(new StringBuilder().Append(base.GetType().Name).Append(".bodyAltName.").Append(base.dataName)
				.ToString());
		}
	}

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x060013E2 RID: 5090 RVA: 0x0005D7D2 File Offset: 0x0005B9D2
	public string classification
	{
		get
		{
			return Loc.T(new StringBuilder().Append(base.GetType().Name).Append(".classification.").Append(base.dataName)
				.ToString());
		}
	}

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x060013E3 RID: 5091 RVA: 0x0005D808 File Offset: 0x0005BA08
	public string descriptor1
	{
		get
		{
			return Loc.T(new StringBuilder().Append(base.GetType().Name).Append(".descriptor1.").Append(base.dataName)
				.ToString());
		}
	}

	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x060013E4 RID: 5092 RVA: 0x0005D83E File Offset: 0x0005BA3E
	public string descriptor2
	{
		get
		{
			return Loc.T(new StringBuilder().Append(base.GetType().Name).Append(".descriptor2.").Append(base.dataName)
				.ToString());
		}
	}

	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x060013E5 RID: 5093 RVA: 0x0005D874 File Offset: 0x0005BA74
	public string discovery_Year
	{
		get
		{
			return Loc.T(new StringBuilder().Append(base.GetType().Name).Append(".discovery_Year.").Append(base.dataName)
				.ToString());
		}
	}

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x060013E6 RID: 5094 RVA: 0x0005D8AA File Offset: 0x0005BAAA
	public string MapResource
	{
		get
		{
			return this.mapResource;
		}
	}

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x060013E7 RID: 5095 RVA: 0x0005D8B4 File Offset: 0x0005BAB4
	public float MapScale
	{
		get
		{
			float? num = this.mapScale;
			if (num == null)
			{
				return 1f;
			}
			return num.GetValueOrDefault();
		}
	}

	// Token: 0x170002AB RID: 683
	// (get) Token: 0x060013E8 RID: 5096 RVA: 0x0005D8DE File Offset: 0x0005BADE
	public bool irradiated
	{
		get
		{
			return this.irradiatedMultiplier > 1f;
		}
	}

	// Token: 0x040011CE RID: 4558
	public double? equatorialRadius_km;

	// Token: 0x040011CF RID: 4559
	public double? meanRadius_km;

	// Token: 0x040011D0 RID: 4560
	public double? dimensionX_km;

	// Token: 0x040011D1 RID: 4561
	public double? dimensionY_km;

	// Token: 0x040011D2 RID: 4562
	public double? dimensionZ_km;

	// Token: 0x040011D3 RID: 4563
	public float? oblateness;

	// Token: 0x040011D4 RID: 4564
	public float? tilt_Deg;

	// Token: 0x040011D5 RID: 4565
	public float? tiltSkew_Deg;

	// Token: 0x040011D6 RID: 4566
	public double? density_gcm3;

	// Token: 0x040011D7 RID: 4567
	public string rotationPeriod_strHours;

	// Token: 0x040011D8 RID: 4568
	public float? rotationOffset_Deg;

	// Token: 0x040011D9 RID: 4569
	public string fabricatedData;

	// Token: 0x040011DA RID: 4570
	public double? min_periapsis_altitude_km;

	// Token: 0x040011DB RID: 4571
	public float irradiatedMultiplier;

	// Token: 0x040011DC RID: 4572
	public Atmosphere atmosphere;

	// Token: 0x040011DD RID: 4573
	public string mapResource;

	// Token: 0x040011DE RID: 4574
	public float? mapScale;

	// Token: 0x040011DF RID: 4575
	public int numAltModels;

	// Token: 0x040011E0 RID: 4576
	public List<AltSpaceBodyModel> altModels;

	// Token: 0x040011E1 RID: 4577
	public float angularDiameterMultiplier;

	// Token: 0x040011E2 RID: 4578
	public double atmosphereScaleHeight_km;

	// Token: 0x040011E3 RID: 4579
	public double atmosphereSurfaceDensity_kgpm3;

	// Token: 0x040011E4 RID: 4580
	public List<string> habSites = new List<string>();
}
