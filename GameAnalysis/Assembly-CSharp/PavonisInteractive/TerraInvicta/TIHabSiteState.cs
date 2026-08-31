using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullSerializer;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007A4 RID: 1956
	public class TIHabSiteState : TISpaceGameState
	{
		// Token: 0x17000B7B RID: 2939
		// (get) Token: 0x06003F78 RID: 16248 RVA: 0x00198CA8 File Offset: 0x00196EA8
		// (set) Token: 0x06003F79 RID: 16249 RVA: 0x00198CB0 File Offset: 0x00196EB0
		[fsIgnore]
		public TIMiningProfileTemplate miningProfile { get; private set; }

		// Token: 0x17000B7C RID: 2940
		// (get) Token: 0x06003F7A RID: 16250 RVA: 0x00198CB9 File Offset: 0x00196EB9
		public override bool isHabSiteState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000B7D RID: 2941
		// (get) Token: 0x06003F7B RID: 16251 RVA: 0x00198CBC File Offset: 0x00196EBC
		public override Searchable searchable
		{
			get
			{
				return Searchable.withIntel;
			}
		}

		// Token: 0x17000B7E RID: 2942
		// (get) Token: 0x06003F7C RID: 16252 RVA: 0x00198CBF File Offset: 0x00196EBF
		public override TIFactionState ref_faction
		{
			get
			{
				TIHabState tihabState = this.hab;
				return ((tihabState != null) ? tihabState.ref_faction : null) ?? null;
			}
		}

		// Token: 0x17000B7F RID: 2943
		// (get) Token: 0x06003F7D RID: 16253 RVA: 0x00198CD8 File Offset: 0x00196ED8
		public override List<TIFactionState> ref_factions
		{
			get
			{
				TIHabState tihabState = this.hab;
				return ((tihabState != null) ? tihabState.ref_factions : null) ?? new List<TIFactionState>();
			}
		}

		// Token: 0x17000B80 RID: 2944
		// (get) Token: 0x06003F7E RID: 16254 RVA: 0x00198CF5 File Offset: 0x00196EF5
		public override TIHabState ref_hab
		{
			get
			{
				return this.hab;
			}
		}

		// Token: 0x17000B81 RID: 2945
		// (get) Token: 0x06003F7F RID: 16255 RVA: 0x00198CFD File Offset: 0x00196EFD
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				return this.parentBody;
			}
		}

		// Token: 0x17000B82 RID: 2946
		// (get) Token: 0x06003F80 RID: 16256 RVA: 0x00198D05 File Offset: 0x00196F05
		public override TIHabSiteState ref_habSite
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000B83 RID: 2947
		// (get) Token: 0x06003F81 RID: 16257 RVA: 0x00198D08 File Offset: 0x00196F08
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				return this.ref_naturalSpaceObject;
			}
		}

		// Token: 0x17000B84 RID: 2948
		// (get) Token: 0x06003F82 RID: 16258 RVA: 0x00198D10 File Offset: 0x00196F10
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				return this.ref_spaceBody;
			}
		}

		// Token: 0x17000B85 RID: 2949
		// (get) Token: 0x06003F83 RID: 16259 RVA: 0x00198D18 File Offset: 0x00196F18
		public override bool hasMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000B86 RID: 2950
		// (get) Token: 0x06003F84 RID: 16260 RVA: 0x00198D1B File Offset: 0x00196F1B
		public override bool inSpace
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000B87 RID: 2951
		// (get) Token: 0x06003F85 RID: 16261 RVA: 0x00198D1E File Offset: 0x00196F1E
		public string detailDisplayName
		{
			get
			{
				return Loc.T("UI.Habs.BaseLocationName", new object[]
				{
					this.displayName,
					this.parentBody.displayName
				});
			}
		}

		// Token: 0x17000B88 RID: 2952
		// (get) Token: 0x06003F86 RID: 16262 RVA: 0x00198D47 File Offset: 0x00196F47
		public bool hasOperatingBase
		{
			get
			{
				return this.hab != null && this.hab.anyCoreCompleted;
			}
		}

		// Token: 0x17000B89 RID: 2953
		// (get) Token: 0x06003F87 RID: 16263 RVA: 0x00198D64 File Offset: 0x00196F64
		public bool hasPlannedOrOperatingBase
		{
			get
			{
				return this.hab != null && this.hab.PresentModules().Count > 0;
			}
		}

		// Token: 0x17000B8A RID: 2954
		// (get) Token: 0x06003F88 RID: 16264 RVA: 0x00198D89 File Offset: 0x00196F89
		public double surfaceGravity_g
		{
			get
			{
				return this.parentBody.surfaceGravity_g;
			}
		}

		// Token: 0x17000B8B RID: 2955
		// (get) Token: 0x06003F89 RID: 16265 RVA: 0x00198D96 File Offset: 0x00196F96
		public double surfaceGravity_mps2
		{
			get
			{
				return this.parentBody.surfaceGravity_mps2;
			}
		}

		// Token: 0x17000B8C RID: 2956
		// (get) Token: 0x06003F8A RID: 16266 RVA: 0x00198DA3 File Offset: 0x00196FA3
		public TIHabSiteTemplate template
		{
			get
			{
				return this.GetMyTemplate<TIHabSiteTemplate>();
			}
		}

		// Token: 0x17000B8D RID: 2957
		// (get) Token: 0x06003F8B RID: 16267 RVA: 0x00198DAB File Offset: 0x00196FAB
		public bool irradiated
		{
			get
			{
				return this.parentBody.irradiated;
			}
		}

		// Token: 0x17000B8E RID: 2958
		// (get) Token: 0x06003F8C RID: 16268 RVA: 0x00198DB8 File Offset: 0x00196FB8
		public float irradiatedValue
		{
			get
			{
				return this.parentBody.irradiatedMultiplier;
			}
		}

		// Token: 0x06003F8D RID: 16269 RVA: 0x00198DC8 File Offset: 0x00196FC8
		public double MinDeltaVToLaunch_kps(float acceleration_mps2)
		{
			return this.parentBody.orbits.Min<TIOrbitState>((TIOrbitState x) => x.DeltaVToReachFromSurface_kps(this.latitude, (double)acceleration_mps2));
		}

		// Token: 0x17000B8F RID: 2959
		// (get) Token: 0x06003F8E RID: 16270 RVA: 0x00198E05 File Offset: 0x00197005
		public double rotationalVelocity_kps
		{
			get
			{
				return Mathd.Cos((double)this.latitude * 0.017453292519943295) * this.parentBody.circumfrence_km / this.parentBody.rotationperiod_s;
			}
		}

		// Token: 0x17000B90 RID: 2960
		// (get) Token: 0x06003F8F RID: 16271 RVA: 0x00198E35 File Offset: 0x00197035
		public int maxTier
		{
			get
			{
				return this.ref_naturalSpaceObject.maxHabTier;
			}
		}

		// Token: 0x06003F90 RID: 16272 RVA: 0x00198E42 File Offset: 0x00197042
		public HabSiteController GetController()
		{
			return this.controller;
		}

		// Token: 0x17000B91 RID: 2961
		// (get) Token: 0x06003F91 RID: 16273 RVA: 0x00198E4A File Offset: 0x0019704A
		public float radius_gameUnits
		{
			get
			{
				return this.controller.radius_gameUnits;
			}
		}

		// Token: 0x06003F92 RID: 16274 RVA: 0x00198E58 File Offset: 0x00197058
		public override void InitWithTemplate(TIDataTemplate rawTemplate)
		{
			base.InitWithTemplate(rawTemplate);
			TIHabSiteTemplate tihabSiteTemplate = rawTemplate as TIHabSiteTemplate;
			this.templateName = tihabSiteTemplate.dataName;
			this.displayName = tihabSiteTemplate.displayName;
			this.parentBody = tihabSiteTemplate.parentBody;
			this.miningProfile = tihabSiteTemplate.miningProfile;
			this.landedFleets = new List<TISpaceFleetState>();
			this.parentBody = tihabSiteTemplate.parentBody;
			this.RandomizeSiteMiningData();
			this.gameStateSubjectCreated = true;
			this.latitude = ((tihabSiteTemplate.latitude != null) ? tihabSiteTemplate.latitude.Value : ((float)TIUtilities.RandomRange(-60, 60)));
			this.longitude = ((tihabSiteTemplate.longitude != null) ? tihabSiteTemplate.longitude.Value : TIUtilities.RandomRange(-180f, 179.99f));
		}

		// Token: 0x06003F93 RID: 16275 RVA: 0x00198F22 File Offset: 0x00197122
		public void SetController(HabSiteController controller)
		{
			this.controller = controller;
		}

		// Token: 0x06003F94 RID: 16276 RVA: 0x00198F2C File Offset: 0x0019712C
		public override void PostGlobalGameStateCreateInit_2()
		{
			this.miningProfile = this.template.miningProfile;
			if (this.parentBody == null)
			{
				this.parentBody = this.template.parentBody;
				if (this.parentBody == null)
				{
					Log.Error(this.displayName + " has no parent body assigned", Array.Empty<object>());
				}
			}
			this.localized_coordinates_offset = Quaternion.AngleAxis(this.longitude, -Vector3.up) * Quaternion.AngleAxis(this.latitude, -Vector3.right) * Vector3.forward * (float)this.parentBody.meanRadius_m;
			foreach (TISpaceFleetState tispaceFleetState in new List<TISpaceFleetState>(this.landedFleets))
			{
				if (tispaceFleetState.deleted || tispaceFleetState.location.ref_habSite != this)
				{
					Log.Error("Bad fleet " + tispaceFleetState.ID.ToString() + " was included in landedFleets in this savegame, at hab site " + this.displayName, Array.Empty<object>());
					this.landedFleets.Remove(tispaceFleetState);
				}
			}
			this.solarMultiplier = TIHabModuleState.SetLocationSolarPowerMultiplier(this);
		}

		// Token: 0x06003F95 RID: 16277 RVA: 0x0019908C File Offset: 0x0019728C
		public Vector3d GlobalPosition(TIDateTime time)
		{
			Quaterniond spatialRotation = this.parentBody.SpatialRotation;
			Quaternion quaternion = Quaternion.AngleAxis((float)this.parentBody.GetSurfaceRotation_Rad(time) * 57.29578f, Vector3.up);
			Vector3 vector = (Quaternion)spatialRotation * quaternion * this.localized_coordinates_offset;
			return this.parentBody.GetGlobalPositionAtTime(time) + new Vector3d(vector.x, vector.z, vector.y);
		}

		// Token: 0x06003F96 RID: 16278 RVA: 0x00199104 File Offset: 0x00197304
		public double DeltaVToLandFromInterface_kps(TIOrbitState orbit, double fleetAcceleration_mps2, bool generic, bool aerodynamic)
		{
			double num = 0.0;
			if (orbit == null)
			{
				orbit = this.parentBody.interfaceOrbits.MinBy<TIOrbitState, double>((TIOrbitState x) => x.altitude_km);
			}
			double num2;
			double num4;
			double num5;
			if (generic && orbit.altitude_km > 200.0)
			{
				num2 = 200000.0;
				double num3 = num2 + this.parentBody.meanRadius_m;
				num4 = Mathd.Sqrt(this.parentBody.mu / num3);
				num5 = this.parentBody.mu / (num3 * num3);
			}
			else if (orbit.altitude_km > 200.0)
			{
				double num6 = Mathd.Sqrt(this.parentBody.mu / (orbit.altitude_m + this.parentBody.meanRadius_m));
				num2 = 200000.0;
				double num7 = num2 + this.parentBody.meanRadius_m;
				num4 = Mathd.Sqrt(this.parentBody.mu / num7);
				num = Mathd.Abs(num4 - num6);
				num5 = this.parentBody.mu / (num7 * num7);
			}
			else
			{
				num4 = orbit.averageOrbitalVelocity_kps * 1000.0;
				num5 = orbit.localGravity_mps2;
				num2 = orbit.altitude_m;
			}
			double num8 = this.rotationalVelocity_kps * 1000.0;
			double num9 = num4 - num8;
			if (num9 < 0.0)
			{
				num9 = num4 + num8;
			}
			double num10 = (num5 + this.parentBody.surfaceGravity_mps2) / 2.0;
			double num11 = Mathd.Sqrt(2.0 * num2 / num10) * num10;
			if (generic)
			{
				return (num11 + num9) * (1.0 - this.parentBody.DragDeltaVSavingsToLand_Frac(aerodynamic)) / 1000.0;
			}
			double num12 = Mathd.Max(num9 / fleetAcceleration_mps2 * num10, num11);
			return (num + (num12 + num9) * (1.0 - this.parentBody.DragDeltaVSavingsToLand_Frac(aerodynamic))) / 1000.0;
		}

		// Token: 0x06003F97 RID: 16279 RVA: 0x0019930D File Offset: 0x0019750D
		public void MarkPendingHab()
		{
			this.pendingHab = true;
		}

		// Token: 0x06003F98 RID: 16280 RVA: 0x00199316 File Offset: 0x00197516
		public void FoundHab()
		{
			this.pendingHab = false;
		}

		// Token: 0x17000B92 RID: 2962
		// (get) Token: 0x06003F99 RID: 16281 RVA: 0x0019931F File Offset: 0x0019751F
		public int numIncomes
		{
			get
			{
				return TIResourcesCost.basicSpaceResources.Count<FactionResource>((FactionResource x) => this.GetDailyProduction(x) > 0f);
			}
		}

		// Token: 0x06003F9A RID: 16282 RVA: 0x00199338 File Offset: 0x00197538
		public float GetDailyProduction(FactionResource resource)
		{
			switch (resource)
			{
			case FactionResource.Water:
				return this.water_day;
			case FactionResource.Volatiles:
				return this.volatiles_day;
			case FactionResource.Metals:
				return this.metals_day;
			case FactionResource.NobleMetals:
				return this.nobles_day;
			case FactionResource.Fissiles:
				return this.fissiles_day;
			default:
				return 0f;
			}
		}

		// Token: 0x06003F9B RID: 16283 RVA: 0x0019938C File Offset: 0x0019758C
		public IEnumerable<FactionResource> PrimaryResources()
		{
			float highestOutput = TIResourcesCost.basicSpaceResources.Max<FactionResource>((FactionResource x) => this.GetDailyProduction(x));
			return TIResourcesCost.basicSpaceResources.Where<FactionResource>((FactionResource x) => this.GetDailyProduction(x) > 0.8f * highestOutput);
		}

		// Token: 0x06003F9C RID: 16284 RVA: 0x001993D8 File Offset: 0x001975D8
		public float GetMonthlyProduction(FactionResource resource)
		{
			return this.GetDailyProduction(resource) * 30.436874f;
		}

		// Token: 0x06003F9D RID: 16285 RVA: 0x001993E8 File Offset: 0x001975E8
		public string ProductivityString(bool probed)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(TemplateManager.global.pathInlineSolarIcon).Append(TIUtilities.FormatSmallNumber(this.solarMultiplier, 7, 3, true, false));
			if (probed)
			{
				if (this.water_day > 0f)
				{
					stringBuilder.Append(TemplateManager.global.waterInlineSpritePath);
					float monthlyProduction = this.GetMonthlyProduction(FactionResource.Water);
					stringBuilder.Append(monthlyProduction.ToString(TIUtilities.DecimalPlaces((double)monthlyProduction, 7, 0)));
				}
				if (this.volatiles_day > 0f)
				{
					stringBuilder.Append(TemplateManager.global.volatilesInlineSpritePath);
					float monthlyProduction2 = this.GetMonthlyProduction(FactionResource.Volatiles);
					stringBuilder.Append(monthlyProduction2.ToString(TIUtilities.DecimalPlaces((double)monthlyProduction2, 7, 0)));
				}
				if (this.metals_day > 0f)
				{
					stringBuilder.Append(TemplateManager.global.metalsInlineSpritePath);
					float monthlyProduction3 = this.GetMonthlyProduction(FactionResource.Metals);
					stringBuilder.Append(monthlyProduction3.ToString(TIUtilities.DecimalPlaces((double)monthlyProduction3, 7, 0)));
				}
				if (this.nobles_day > 0f)
				{
					stringBuilder.Append(TemplateManager.global.noblesInlineSpritePath);
					float monthlyProduction4 = this.GetMonthlyProduction(FactionResource.NobleMetals);
					stringBuilder.Append(monthlyProduction4.ToString(TIUtilities.DecimalPlaces((double)monthlyProduction4, 7, 0)));
				}
				if (this.fissiles_day > 0f)
				{
					stringBuilder.Append(TemplateManager.global.fissilesInlineSpritePath);
					float monthlyProduction5 = this.GetMonthlyProduction(FactionResource.Fissiles);
					stringBuilder.Append(monthlyProduction5.ToString(TIUtilities.DecimalPlaces((double)monthlyProduction5, 7, 0)));
				}
			}
			else
			{
				stringBuilder.Append(TemplateManager.global.waterInlineSpritePath);
				stringBuilder.Append(TemplateManager.global.volatilesInlineSpritePath);
				stringBuilder.Append(TemplateManager.global.metalsInlineSpritePath);
				stringBuilder.Append(TemplateManager.global.noblesInlineSpritePath);
				stringBuilder.Append(TemplateManager.global.fissilesInlineSpritePath);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06003F9E RID: 16286 RVA: 0x001995BF File Offset: 0x001977BF
		public float GetHabSiteMinProductivity_day(FactionResource resource)
		{
			return this.GetHabSiteMinProductivity_month(resource) / 30.436874f;
		}

		// Token: 0x06003F9F RID: 16287 RVA: 0x001995D0 File Offset: 0x001977D0
		public float GetHabSiteMinProductivity_month(FactionResource resource)
		{
			switch (resource)
			{
			case FactionResource.Water:
				return Mathf.Max(this.miningProfile.water_min, this.GetHabSiteExpectedProductivity_month(resource) - this.ModifyBaseValueForConditions(this.miningProfile.water_width, false) / 2f);
			case FactionResource.Volatiles:
				return Mathf.Max(this.miningProfile.volatiles_min, this.GetHabSiteExpectedProductivity_month(resource) - this.ModifyBaseValueForConditions(this.miningProfile.volatiles_width, false) / 2f);
			case FactionResource.Metals:
				return Mathf.Max(this.miningProfile.metals_min, this.GetHabSiteExpectedProductivity_month(resource) - this.ModifyBaseValueForConditions(this.miningProfile.metals_width, false) / 2f);
			case FactionResource.NobleMetals:
				return Mathf.Max(this.miningProfile.nobles_min, this.GetHabSiteExpectedProductivity_month(resource) - this.ModifyBaseValueForConditions(this.miningProfile.nobles_width, false) / 2f);
			case FactionResource.Fissiles:
				return Mathf.Max(this.miningProfile.fissiles_min, this.GetHabSiteExpectedProductivity_month(resource) - this.ModifyBaseValueForConditions(this.miningProfile.fissiles_width, false) / 2f);
			default:
				return 0f;
			}
		}

		// Token: 0x06003FA0 RID: 16288 RVA: 0x001996F8 File Offset: 0x001978F8
		public float GetHabSiteMaxProductivity_day(FactionResource resource)
		{
			return this.GetHabSiteMaxProductivity_month(resource) / 30.436874f;
		}

		// Token: 0x06003FA1 RID: 16289 RVA: 0x00199708 File Offset: 0x00197908
		public float GetHabSiteMaxProductivity_month(FactionResource resource)
		{
			switch (resource)
			{
			case FactionResource.Water:
				return this.GetHabSiteExpectedProductivity_month(resource) + this.ModifyBaseValueForConditions(this.miningProfile.water_width, false) / 2f;
			case FactionResource.Volatiles:
				return this.GetHabSiteExpectedProductivity_month(resource) + this.ModifyBaseValueForConditions(this.miningProfile.volatiles_width, false) / 2f;
			case FactionResource.Metals:
				return this.GetHabSiteExpectedProductivity_month(resource) + this.ModifyBaseValueForConditions(this.miningProfile.metals_width, false) / 2f;
			case FactionResource.NobleMetals:
				return this.GetHabSiteExpectedProductivity_month(resource) + this.ModifyBaseValueForConditions(this.miningProfile.nobles_width, false) / 2f;
			case FactionResource.Fissiles:
				return this.GetHabSiteExpectedProductivity_month(resource) + this.ModifyBaseValueForConditions(this.miningProfile.fissiles_width, false) / 2f;
			default:
				return 0f;
			}
		}

		// Token: 0x06003FA2 RID: 16290 RVA: 0x001997E0 File Offset: 0x001979E0
		public float GetHabSiteExpectedProductivity_day(FactionResource resource)
		{
			return this.GetHabSiteExpectedProductivity_month(resource) / 30.436874f;
		}

		// Token: 0x06003FA3 RID: 16291 RVA: 0x001997F0 File Offset: 0x001979F0
		public float GetHabSiteExpectedProductivity_month(FactionResource resource)
		{
			switch (resource)
			{
			case FactionResource.Water:
				return this.ModifyBaseValueForConditions(this.miningProfile.water_mean, false);
			case FactionResource.Volatiles:
				return this.ModifyBaseValueForConditions(this.miningProfile.volatiles_mean, false);
			case FactionResource.Metals:
				return this.ModifyBaseValueForConditions(this.miningProfile.metals_mean, true);
			case FactionResource.NobleMetals:
				return Mathf.Min(this.GetHabSiteExpectedProductivity_month(FactionResource.Metals) / 2f, this.ModifyBaseValueForConditions(this.miningProfile.nobles_mean, true));
			case FactionResource.Fissiles:
				return this.ModifyBaseValueForConditions(this.miningProfile.fissiles_mean, true);
			default:
				return 0f;
			}
		}

		// Token: 0x06003FA4 RID: 16292 RVA: 0x00199892 File Offset: 0x00197A92
		public TIHabSiteState.Statistics.SpaceResourceGrade GetExpectedResourceGrade(FactionResource resource)
		{
			return TIHabSiteState.Statistics.GetResourceGrade(resource, TIHabSiteState.Statistics.ExpectedSpaceResourcesPerMonth[this][resource]);
		}

		// Token: 0x06003FA5 RID: 16293 RVA: 0x001998AB File Offset: 0x00197AAB
		public TIHabSiteState.Statistics.SpaceResourceGrade GetActualResourceGrade(FactionResource resource)
		{
			return TIHabSiteState.Statistics.GetResourceGrade(resource, this.GetMonthlyProduction(resource));
		}

		// Token: 0x06003FA6 RID: 16294 RVA: 0x001998BA File Offset: 0x00197ABA
		private bool Nothing(float mean, float width, float min, float jump)
		{
			return mean <= 0f && min <= 0f && width <= 0f;
		}

		// Token: 0x06003FA7 RID: 16295 RVA: 0x001998D8 File Offset: 0x00197AD8
		private float ModifyBaseValueForConditions(float baseValue, bool densitySensitive)
		{
			float num = baseValue;
			if (this.miningProfile.modifyBySize && num > 0f)
			{
				float num2 = 1f;
				if (this.parentBody.mass_kg <= TemplateManager.global.maxMassforMiningResourceMalus)
				{
					num2 *= (float)(this.parentBody.mass_kg / TemplateManager.global.maxMassforMiningResourceMalus);
				}
				if (densitySensitive)
				{
					if (this.parentBody.density_gcm3 <= (double)TemplateManager.global.metalsMalusDensityCutPoint)
					{
						num2 *= (float)(this.parentBody.density_gcm3 / (double)TemplateManager.global.metalsMalusDensityCutPoint);
					}
					else if (this.parentBody.density_gcm3 >= (double)TemplateManager.global.metalsBonusDensityCutPoint)
					{
						num2 *= (float)(this.parentBody.density_gcm3 / (double)TemplateManager.global.metalsBonusDensityCutPoint);
					}
				}
				num *= Mathf.Clamp(num2, 0.75f, 1.25f);
			}
			return num * TIGlobalValuesState.GetGlobalMineProductivityModifier();
		}

		// Token: 0x06003FA8 RID: 16296 RVA: 0x001999C0 File Offset: 0x00197BC0
		private float ModifyWidthValueFromSettings(float baseValue, float jump)
		{
			if (TIGlobalValuesState.GetGlobalMineProductivityModifier() > 2f)
			{
				return baseValue *= 2f + Mathf.Pow(TIGlobalValuesState.GetGlobalMineProductivityModifier(), 1f - jump);
			}
			return baseValue *= TIGlobalValuesState.GetGlobalMineProductivityModifier();
		}

		// Token: 0x06003FA9 RID: 16297 RVA: 0x001999F8 File Offset: 0x00197BF8
		private float SetDailyOutputValue(float mean, float width, float min, float jump, bool densistySensitive)
		{
			int num = 0;
			bool flag = false;
			mean = this.ModifyBaseValueForConditions(mean, densistySensitive);
			min = this.ModifyBaseValueForConditions(min, densistySensitive);
			width = this.ModifyWidthValueFromSettings(width, jump);
			while (!flag)
			{
				if (TIUtilities.RandomFloatValue() < jump)
				{
					num++;
				}
				else
				{
					flag = true;
				}
			}
			if (num > 0 && (double)TIUtilities.RandomFloatValue() < 0.5)
			{
				num *= -1;
			}
			mean += (float)num * width;
			float num2 = mean - width / 2f + TIUtilities.RandomRange(0f, width);
			if (min <= 0f)
			{
				num2 = Mathf.Max(num2, 0f);
			}
			else
			{
				num2 = Mathf.Max(num2, min * (0.8f + TIUtilities.RandomRange(0f, 0.4f)));
			}
			if (num2 < 0.01f)
			{
				num2 = 0f;
			}
			return num2 / 30.436874f;
		}

		// Token: 0x06003FAA RID: 16298 RVA: 0x00199AC0 File Offset: 0x00197CC0
		public Dictionary<FactionResource, float> SampleProductivityPerDay()
		{
			Dictionary<FactionResource, float> dictionary = TIResourcesCost.basicSpaceResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource x) => 0f);
			bool flag = this.Nothing(this.miningProfile.water_mean, this.miningProfile.water_width, this.miningProfile.water_min, this.miningProfile.water_jump);
			bool flag2 = this.Nothing(this.miningProfile.volatiles_mean, this.miningProfile.volatiles_width, this.miningProfile.volatiles_min, this.miningProfile.volatiles_jump);
			bool flag3 = this.Nothing(this.miningProfile.metals_mean, this.miningProfile.metals_width, this.miningProfile.metals_min, this.miningProfile.metals_jump);
			bool flag4 = flag3 || this.Nothing(this.miningProfile.nobles_mean, this.miningProfile.nobles_width, this.miningProfile.nobles_min, this.miningProfile.nobles_jump);
			bool flag5 = this.Nothing(this.miningProfile.fissiles_mean, this.miningProfile.fissiles_width, this.miningProfile.fissiles_min, this.miningProfile.fissiles_jump);
			if (!flag || !flag2 || !flag3 || !flag4 || !flag5)
			{
				int num = 0;
				while (dictionary.Values.Sum() == 0f && num < 1000)
				{
					if (!flag)
					{
						dictionary[FactionResource.Water] = this.SetDailyOutputValue(this.miningProfile.water_mean, this.miningProfile.water_width, this.miningProfile.water_min, this.miningProfile.water_jump, false);
					}
					if (!flag2)
					{
						dictionary[FactionResource.Volatiles] = this.SetDailyOutputValue(this.miningProfile.volatiles_mean, this.miningProfile.volatiles_width, this.miningProfile.volatiles_min, this.miningProfile.volatiles_jump, false);
					}
					if (!flag3)
					{
						dictionary[FactionResource.Metals] = this.SetDailyOutputValue(this.miningProfile.metals_mean, this.miningProfile.metals_width, this.miningProfile.metals_min, this.miningProfile.metals_jump, true);
					}
					if (!flag4)
					{
						dictionary[FactionResource.NobleMetals] = Mathf.Min(dictionary[FactionResource.Metals] / (float)(2 + TIUtilities.RandomRange(0, 2)), this.SetDailyOutputValue(this.miningProfile.nobles_mean, this.miningProfile.nobles_width, this.miningProfile.nobles_min, this.miningProfile.nobles_jump, true));
					}
					if (!flag5)
					{
						dictionary[FactionResource.Fissiles] = this.SetDailyOutputValue(this.miningProfile.fissiles_mean, this.miningProfile.fissiles_width, this.miningProfile.fissiles_min, this.miningProfile.fissiles_jump, true);
					}
					num++;
				}
			}
			return dictionary;
		}

		// Token: 0x06003FAB RID: 16299 RVA: 0x00199DAC File Offset: 0x00197FAC
		private void RandomizeSiteMiningData()
		{
			Dictionary<FactionResource, float> dictionary = this.SampleProductivityPerDay();
			foreach (FactionResource factionResource in dictionary.Keys)
			{
				switch (factionResource)
				{
				case FactionResource.Water:
					this.water_day = dictionary[factionResource];
					break;
				case FactionResource.Volatiles:
					this.volatiles_day = dictionary[factionResource];
					break;
				case FactionResource.Metals:
					this.metals_day = dictionary[factionResource];
					break;
				case FactionResource.NobleMetals:
					this.nobles_day = dictionary[factionResource];
					break;
				case FactionResource.Fissiles:
					this.fissiles_day = dictionary[factionResource];
					break;
				}
			}
		}

		// Token: 0x06003FAC RID: 16300 RVA: 0x00199E68 File Offset: 0x00198068
		public void ModifySiteMiningData(float modifier)
		{
			float num = 1f + modifier;
			if (this.water_day > 0f)
			{
				this.water_day *= num;
			}
			if (this.volatiles_day > 0f)
			{
				this.volatiles_day *= num;
			}
			if (this.metals_day > 0f)
			{
				this.metals_day *= num;
			}
			if (this.nobles_day > 0f)
			{
				this.nobles_day *= num;
			}
			if (this.fissiles_day > 0f)
			{
				this.fissiles_day *= num;
			}
		}

		// Token: 0x06003FAD RID: 16301 RVA: 0x00199F04 File Offset: 0x00198104
		public void LandFleet(TISpaceFleetState fleet)
		{
			if (!this.landedFleets.Contains(fleet))
			{
				this.landedFleets.Add(fleet);
			}
		}

		// Token: 0x06003FAE RID: 16302 RVA: 0x00199F20 File Offset: 0x00198120
		public void LaunchFleet(TISpaceFleetState fleet)
		{
			this.landedFleets.Remove(fleet);
		}

		// Token: 0x06003FAF RID: 16303 RVA: 0x00199F2F File Offset: 0x0019812F
		public List<TIHabSiteState> AdjacentSites()
		{
			List<TIHabSiteState> list = new List<TIHabSiteState>(this.parentBody.habSites);
			list.Remove(this);
			return list;
		}

		// Token: 0x04002749 RID: 10057
		public TISpaceBodyState parentBody;

		// Token: 0x0400274A RID: 10058
		public TIHabState hab;

		// Token: 0x0400274B RID: 10059
		public List<TISpaceFleetState> landedFleets;

		// Token: 0x0400274C RID: 10060
		public float water_day;

		// Token: 0x0400274D RID: 10061
		public float volatiles_day;

		// Token: 0x0400274E RID: 10062
		public float metals_day;

		// Token: 0x0400274F RID: 10063
		public float nobles_day;

		// Token: 0x04002750 RID: 10064
		public float fissiles_day;

		// Token: 0x04002751 RID: 10065
		public float latitude = -1f;

		// Token: 0x04002752 RID: 10066
		public float longitude = -1f;

		// Token: 0x04002753 RID: 10067
		public bool pendingHab;

		// Token: 0x04002754 RID: 10068
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x04002755 RID: 10069
		[fsIgnore]
		private HabSiteController controller;

		// Token: 0x04002756 RID: 10070
		[fsIgnore]
		public float solarMultiplier;

		// Token: 0x04002758 RID: 10072
		private Vector3 localized_coordinates_offset;

		// Token: 0x04002759 RID: 10073
		public Vector3d positionOffsetDueToIrregularBody = Vector3d.zero;

		// Token: 0x02000EEF RID: 3823
		public static class Statistics
		{
			// Token: 0x06007B18 RID: 31512 RVA: 0x0032118C File Offset: 0x0031F38C
			public static TIHabSiteState.Statistics.SpaceResourceGrade GetResourceGrade(FactionResource resource, float incomePerMonth)
			{
				if (incomePerMonth == 0f)
				{
					return TIHabSiteState.Statistics.SpaceResourceGrade.None;
				}
				float num = TIHabSiteState.Statistics.SpaceResourcesPerMonth_Mean[resource];
				float num2 = TIHabSiteState.Statistics.SpaceResourcesPerMonth_StandardDeviation[resource];
				if (incomePerMonth < num - num2 * 1.3f)
				{
					return TIHabSiteState.Statistics.SpaceResourceGrade.Awful;
				}
				if (incomePerMonth < num - num2 * 0.8f)
				{
					return TIHabSiteState.Statistics.SpaceResourceGrade.Poor;
				}
				if (incomePerMonth < num + num2 * 0f)
				{
					return TIHabSiteState.Statistics.SpaceResourceGrade.BelowAverage;
				}
				if (incomePerMonth < num + num2 * 0.8f)
				{
					return TIHabSiteState.Statistics.SpaceResourceGrade.AboveAverage;
				}
				if (incomePerMonth < num + num2 * 1.8f)
				{
					return TIHabSiteState.Statistics.SpaceResourceGrade.Good;
				}
				return TIHabSiteState.Statistics.SpaceResourceGrade.Great;
			}

			// Token: 0x06007B19 RID: 31513 RVA: 0x00321204 File Offset: 0x0031F404
			public static void Recalculate()
			{
				IEnumerable<TIHabSiteState> enumerable = GameStateManager.IterateByClass<TIHabSiteState>(false);
				TIHabSiteState.Statistics.ExpectedSpaceResourcesPerMonth = enumerable.ToDictionary<TIHabSiteState, TIHabSiteState, Dictionary<FactionResource, float>>((TIHabSiteState x) => x, delegate(TIHabSiteState habSite)
				{
					Dictionary<FactionResource, float> dictionary = habSite.SampleProductivityPerDay();
					int sampleCount = 100;
					for (int i = 0; i < sampleCount - 1; i++)
					{
						Dictionary<FactionResource, float> dictionary2 = habSite.SampleProductivityPerDay();
						foreach (FactionResource factionResource in dictionary2.Keys)
						{
							Dictionary<FactionResource, float> dictionary3 = dictionary;
							FactionResource factionResource2 = factionResource;
							dictionary3[factionResource2] += dictionary2[factionResource];
						}
					}
					return dictionary.ToDictionary<KeyValuePair<FactionResource, float>, FactionResource, float>((KeyValuePair<FactionResource, float> x) => x.Key, (KeyValuePair<FactionResource, float> x) => x.Value * 30.436874f / (float)sampleCount);
				});
				List<TIHabSiteState> sampleHabSites = (from x in enumerable
					group x by x.ref_spaceBody).SelectMany<IGrouping<TISpaceBodyState, TIHabSiteState>, TIHabSiteState>((IGrouping<TISpaceBodyState, TIHabSiteState> x) => x.Take_Random<TIHabSiteState>(1)).ToList<TIHabSiteState>();
				HashSet<FactionResource> basicSpaceResources = TIResourcesCost.basicSpaceResources;
				Dictionary<FactionResource, List<float>> incomes = basicSpaceResources.ToDictionary<FactionResource, FactionResource, List<float>>((FactionResource x) => x, (FactionResource resource) => sampleHabSites.Select<TIHabSiteState, float>((TIHabSiteState habSite) => TIHabSiteState.Statistics.ExpectedSpaceResourcesPerMonth[habSite][resource]).ToList<float>());
				incomes = incomes.Keys.ToDictionary<FactionResource, FactionResource, List<float>>((FactionResource x) => x, (FactionResource x) => incomes[x].Where<float>((float income) => income > ((x == FactionResource.Fissiles) ? 0.2f : 2f)).ToList<float>());
				TIHabSiteState.Statistics.SpaceResourcesPerMonth_Mean = basicSpaceResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource x) => incomes[x].Sum() / (float)incomes[x].Count);
				Func<FactionResource, float> GetStandardDeviation = (FactionResource resource) => Mathf.Sqrt(incomes[resource].Sum<float>((float x) => Mathf.Pow(x - TIHabSiteState.Statistics.SpaceResourcesPerMonth_Mean[resource], 2f)) / (float)(incomes[resource].Count - 1));
				TIHabSiteState.Statistics.SpaceResourcesPerMonth_StandardDeviation = basicSpaceResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource x) => GetStandardDeviation(x));
			}

			// Token: 0x04005B59 RID: 23385
			public static Dictionary<TIHabSiteState, Dictionary<FactionResource, float>> ExpectedSpaceResourcesPerMonth = new Dictionary<TIHabSiteState, Dictionary<FactionResource, float>>();

			// Token: 0x04005B5A RID: 23386
			public static Dictionary<FactionResource, float> SpaceResourcesPerMonth_Mean;

			// Token: 0x04005B5B RID: 23387
			public static Dictionary<FactionResource, float> SpaceResourcesPerMonth_StandardDeviation;

			// Token: 0x020013EF RID: 5103
			public enum SpaceResourceGrade
			{
				// Token: 0x04007343 RID: 29507
				None,
				// Token: 0x04007344 RID: 29508
				Awful,
				// Token: 0x04007345 RID: 29509
				Poor,
				// Token: 0x04007346 RID: 29510
				BelowAverage,
				// Token: 0x04007347 RID: 29511
				AboveAverage,
				// Token: 0x04007348 RID: 29512
				Good,
				// Token: 0x04007349 RID: 29513
				Great
			}
		}
	}
}
