using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Systems;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007AB RID: 1963
	public class TIOrbitState : TISpaceGameState, ITransferTarget
	{
		// Token: 0x17000C0D RID: 3085
		// (get) Token: 0x06004157 RID: 16727 RVA: 0x001A5DF3 File Offset: 0x001A3FF3
		public override bool isOrbitState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000C0E RID: 3086
		// (get) Token: 0x06004158 RID: 16728 RVA: 0x001A5DF6 File Offset: 0x001A3FF6
		public override TIOrbitState ref_orbit
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000C0F RID: 3087
		// (get) Token: 0x06004159 RID: 16729 RVA: 0x001A5DF9 File Offset: 0x001A3FF9
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				if (!this.barycenter.isSpaceBodyState)
				{
					return null;
				}
				return this.barycenter.ref_spaceBody;
			}
		}

		// Token: 0x17000C10 RID: 3088
		// (get) Token: 0x0600415A RID: 16730 RVA: 0x001A5E15 File Offset: 0x001A4015
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				return this.barycenter;
			}
		}

		// Token: 0x17000C11 RID: 3089
		// (get) Token: 0x0600415B RID: 16731 RVA: 0x001A5E1D File Offset: 0x001A401D
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				return this.barycenter;
			}
		}

		// Token: 0x17000C12 RID: 3090
		// (get) Token: 0x0600415C RID: 16732 RVA: 0x001A5E25 File Offset: 0x001A4025
		public override TILagrangePointState ref_lagrangePoint
		{
			get
			{
				if (!this.barycenter.isLagrangePointState)
				{
					return null;
				}
				return this.barycenter.ref_lagrangePoint;
			}
		}

		// Token: 0x17000C13 RID: 3091
		// (get) Token: 0x0600415D RID: 16733 RVA: 0x001A5E41 File Offset: 0x001A4041
		public override bool hasMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000C14 RID: 3092
		// (get) Token: 0x0600415E RID: 16734 RVA: 0x001A5E44 File Offset: 0x001A4044
		public override bool inSpace
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000C15 RID: 3093
		// (get) Token: 0x0600415F RID: 16735 RVA: 0x001A5E47 File Offset: 0x001A4047
		// (set) Token: 0x06004160 RID: 16736 RVA: 0x001A5E4F File Offset: 0x001A404F
		public virtual float amat_ugpy { get; private set; }

		// Token: 0x17000C16 RID: 3094
		// (get) Token: 0x06004161 RID: 16737 RVA: 0x001A5E58 File Offset: 0x001A4058
		public virtual TIOrbitTemplate template
		{
			get
			{
				return this.GetMyTemplate<TIOrbitTemplate>();
			}
		}

		// Token: 0x17000C17 RID: 3095
		// (get) Token: 0x06004162 RID: 16738 RVA: 0x001A5E60 File Offset: 0x001A4060
		public virtual int stationCapacity
		{
			get
			{
				return this.template.stationCapacity;
			}
		}

		// Token: 0x17000C18 RID: 3096
		// (get) Token: 0x06004163 RID: 16739 RVA: 0x001A5E6D File Offset: 0x001A406D
		public virtual bool irradiated
		{
			get
			{
				return this.template.irradiated;
			}
		}

		// Token: 0x17000C19 RID: 3097
		// (get) Token: 0x06004164 RID: 16740 RVA: 0x001A5E7A File Offset: 0x001A407A
		public double altitude_m
		{
			get
			{
				return this.semiMajorAxis_m - this.barycenter.meanRadius_m;
			}
		}

		// Token: 0x17000C1A RID: 3098
		// (get) Token: 0x06004165 RID: 16741 RVA: 0x001A5E8E File Offset: 0x001A408E
		public double altitude_km
		{
			get
			{
				return this.altitude_m / 1000.0;
			}
		}

		// Token: 0x17000C1B RID: 3099
		// (get) Token: 0x06004166 RID: 16742 RVA: 0x001A5EA0 File Offset: 0x001A40A0
		public virtual double eccentricity
		{
			get
			{
				return this.template.Eccentricity;
			}
		}

		// Token: 0x17000C1C RID: 3100
		// (get) Token: 0x06004167 RID: 16743 RVA: 0x001A5EAD File Offset: 0x001A40AD
		public virtual double inclination_Rad
		{
			get
			{
				return this.template.Inclination_Rad;
			}
		}

		// Token: 0x17000C1D RID: 3101
		// (get) Token: 0x06004168 RID: 16744 RVA: 0x001A5EBA File Offset: 0x001A40BA
		public virtual double longitudeAscendingNode_Rad
		{
			get
			{
				return this.template.LongitudeAscendingNode_Rad;
			}
		}

		// Token: 0x17000C1E RID: 3102
		// (get) Token: 0x06004169 RID: 16745 RVA: 0x001A5EC7 File Offset: 0x001A40C7
		public virtual double longitudePeriapsis
		{
			get
			{
				return this.template.LongitudePeriapsis;
			}
		}

		// Token: 0x17000C1F RID: 3103
		// (get) Token: 0x0600416A RID: 16746 RVA: 0x001A5ED4 File Offset: 0x001A40D4
		public virtual double argPeriapsis_Rad
		{
			get
			{
				return this.template.ArgPeriapsis_Rad;
			}
		}

		// Token: 0x17000C20 RID: 3104
		// (get) Token: 0x0600416B RID: 16747 RVA: 0x001A5EE1 File Offset: 0x001A40E1
		public virtual bool isAdHocOrbit
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000C21 RID: 3105
		// (get) Token: 0x0600416C RID: 16748 RVA: 0x001A5EE4 File Offset: 0x001A40E4
		public float irradiatedValue
		{
			get
			{
				return this.template.irradiatedMultiplier;
			}
		}

		// Token: 0x17000C22 RID: 3106
		// (get) Token: 0x0600416D RID: 16749 RVA: 0x001A5EF1 File Offset: 0x001A40F1
		public float circumference_km
		{
			get
			{
				return 6.2831855f * (float)this.semiMajorAxis_km;
			}
		}

		// Token: 0x0600416E RID: 16750 RVA: 0x001A5F00 File Offset: 0x001A4100
		public IEnumerable<TIEffectTemplate> GetExplorationEffectOptions()
		{
			TIOrbitTemplate template = this.template;
			if (!string.IsNullOrEmpty((template != null) ? template.effectToExplore : null))
			{
				TIEffectTemplate tieffectTemplate = TemplateManager.Find<TIEffectTemplate>(this.template.effectToExplore, false);
				return from x in Enumerable.Empty<TIEffectTemplate>().Append(tieffectTemplate)
					where x != null
					select x;
			}
			return this.barycenter.GetExplorationEffectOptions();
		}

		// Token: 0x0600416F RID: 16751 RVA: 0x001A5F73 File Offset: 0x001A4173
		TIGameState ITransferTarget.selfState()
		{
			return this;
		}

		// Token: 0x06004170 RID: 16752 RVA: 0x001A5F76 File Offset: 0x001A4176
		TINaturalSpaceObjectState ITransferTarget.barycenter()
		{
			return this.barycenter;
		}

		// Token: 0x06004171 RID: 16753 RVA: 0x001A5F7E File Offset: 0x001A417E
		TINaturalSpaceObjectState ITransferTarget.barycenterBarycenter()
		{
			return this.barycenter.barycenter;
		}

		// Token: 0x06004172 RID: 16754 RVA: 0x001A5F8B File Offset: 0x001A418B
		TINaturalSpaceObjectState ITransferTarget.barycenterBarycenterBarycenter()
		{
			TINaturalSpaceObjectState barycenter = this.barycenter.barycenter;
			if (barycenter == null)
			{
				return null;
			}
			return barycenter.barycenter;
		}

		// Token: 0x06004173 RID: 16755 RVA: 0x001A5FA3 File Offset: 0x001A41A3
		double ITransferTarget.a_m()
		{
			return this.semiMajorAxis_m;
		}

		// Token: 0x06004174 RID: 16756 RVA: 0x001A5FAB File Offset: 0x001A41AB
		double ITransferTarget.e()
		{
			return this.eccentricity;
		}

		// Token: 0x06004175 RID: 16757 RVA: 0x001A5FB3 File Offset: 0x001A41B3
		double ITransferTarget.i_rad()
		{
			return this.inclination_Rad;
		}

		// Token: 0x06004176 RID: 16758 RVA: 0x001A5FBB File Offset: 0x001A41BB
		double ITransferTarget.Ω_rad()
		{
			return this.longitudeAscendingNode_Rad;
		}

		// Token: 0x06004177 RID: 16759 RVA: 0x001A5FC3 File Offset: 0x001A41C3
		double ITransferTarget.ω_rad()
		{
			return this.argPeriapsis_Rad;
		}

		// Token: 0x06004178 RID: 16760 RVA: 0x001A5FCB File Offset: 0x001A41CB
		double ITransferTarget.M0_rad()
		{
			return 0.0;
		}

		// Token: 0x06004179 RID: 16761 RVA: 0x001A5FD6 File Offset: 0x001A41D6
		double ITransferTarget.L0_rad()
		{
			return 0.0 + this.longitudeAscendingNode_Rad + this.argPeriapsis_Rad;
		}

		// Token: 0x0600417A RID: 16762 RVA: 0x001A5FEF File Offset: 0x001A41EF
		double ITransferTarget.t0_jy()
		{
			return 2000.0;
		}

		// Token: 0x0600417B RID: 16763 RVA: 0x001A5FFA File Offset: 0x001A41FA
		double ITransferTarget.μ()
		{
			return this.barycenter.mu;
		}

		// Token: 0x0600417C RID: 16764 RVA: 0x001A6007 File Offset: 0x001A4207
		double ITransferTarget.period_days()
		{
			return this.period_s / 86400.0;
		}

		// Token: 0x0600417D RID: 16765 RVA: 0x001A6019 File Offset: 0x001A4219
		Vector3d ITransferTarget.globalPositionValue(TISpaceFleetState fleet, TIDateTime time)
		{
			return this.barycenter.GetGlobalPositionAtTime(time);
		}

		// Token: 0x0600417E RID: 16766 RVA: 0x001A6027 File Offset: 0x001A4227
		Vector3 ITransferTarget.visualizationPositionValue()
		{
			return this.barycenter.controller.transform.position;
		}

		// Token: 0x0600417F RID: 16767 RVA: 0x001A6040 File Offset: 0x001A4240
		double ITransferTarget.common_a_m(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.semiMajorAxis_m;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.semiMajorAxis_m;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.semiMajorAxis_m;
			}
			Log.Error("Can't find semimajor axis for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004180 RID: 16768 RVA: 0x001A60C4 File Offset: 0x001A42C4
		double ITransferTarget.common_e(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.eccentricity;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.ecc;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.ecc;
			}
			Log.Error("Can't find ecc for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004181 RID: 16769 RVA: 0x001A6148 File Offset: 0x001A4348
		double ITransferTarget.common_i_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.inclination_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.inclination_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.inclination_Rad;
			}
			Log.Error("Can't find i for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004182 RID: 16770 RVA: 0x001A61CC File Offset: 0x001A43CC
		double ITransferTarget.common_Ω_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.longitudeAscendingNode_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.longAscendingNode_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.longAscendingNode_Rad;
			}
			Log.Error("Can't find i for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004183 RID: 16771 RVA: 0x001A6250 File Offset: 0x001A4450
		double ITransferTarget.common_ω_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.argPeriapsis_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.argPeriapsis_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.argPeriapsis_Rad;
			}
			Log.Error("Can't find ω for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004184 RID: 16772 RVA: 0x001A62D4 File Offset: 0x001A44D4
		double ITransferTarget.common_M0_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return 0.0;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.meanAnomalyAtEpoch_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.meanAnomalyAtEpoch_Rad;
			}
			Log.Error("Can't find M0 for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004185 RID: 16773 RVA: 0x001A635C File Offset: 0x001A455C
		double ITransferTarget.common_M_rad(TINaturalSpaceObjectState commonBarycenter, TIDateTime time)
		{
			if (commonBarycenter == this.barycenter)
			{
				return 0.0;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.meanAnomaly_Rad(time);
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.meanAnomaly_Rad(time);
			}
			Log.Error("Can't find M0 for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004186 RID: 16774 RVA: 0x001A63E4 File Offset: 0x001A45E4
		double ITransferTarget.common_L0_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return 0.0;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.meanLongitude_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.meanLongitude_Rad;
			}
			Log.Error("Can't find M0 for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004187 RID: 16775 RVA: 0x001A646C File Offset: 0x001A466C
		double ITransferTarget.common_t0_jy(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return 2000.0;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.epoch_JYears;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.epoch_JYears;
			}
			Log.Error("Can't find epoch for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004188 RID: 16776 RVA: 0x001A64F4 File Offset: 0x001A46F4
		double ITransferTarget.common_μ(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.barycenter.mu;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.mu;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.mu;
			}
			Log.Error("Can't find epoch for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004189 RID: 16777 RVA: 0x001A657C File Offset: 0x001A477C
		double ITransferTarget.common_period_days(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.period_s / 86400.0;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.orbitalPeriod_s / 86400.0;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.orbitalPeriod_s / 86400.0;
			}
			Log.Error("Can't find period value for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x0600418A RID: 16778 RVA: 0x001A661C File Offset: 0x001A481C
		public double relevant_orbit_m(TINaturalSpaceObjectState commonBarycenter)
		{
			if (this.barycenter == commonBarycenter)
			{
				return this.semiMajorAxis_m;
			}
			if (this.barycenter.barycenter == commonBarycenter)
			{
				return this.barycenter.semiMajorAxis_m;
			}
			return this.barycenter.barycenter.semiMajorAxis_m;
		}

		// Token: 0x0600418B RID: 16779 RVA: 0x001A6670 File Offset: 0x001A4870
		CartesianState ITransferTarget.relevantGlobalCartesianState(TINaturalSpaceObjectState commonBarycenter, TIDateTime dateTime)
		{
			if (this.barycenter == commonBarycenter)
			{
				return this.barycenter.ToGlobalCartesianStateAtTime(dateTime) + this.ToOrbitalElementsState(dateTime, 0.0).ToCartesianStateAtTime(dateTime.ExportTime(), this.barycenter.mass_kg);
			}
			if (this.barycenter.barycenter == commonBarycenter)
			{
				return this.barycenter.ToGlobalCartesianStateAtTime(dateTime);
			}
			return this.barycenter.barycenter.ToGlobalCartesianStateAtTime(dateTime);
		}

		// Token: 0x0600418C RID: 16780 RVA: 0x001A66F8 File Offset: 0x001A48F8
		public CartesianState relevantCartesianState(TINaturalSpaceObjectState commonBarycenter, TIDateTime dateTime, double meanAnomaly_Rad)
		{
			if (this.barycenter == commonBarycenter)
			{
				CartesianState cartesianState = this.ToOrbitalElementsState(dateTime, meanAnomaly_Rad).ToCartesianStateAtTime(dateTime.ExportTime(), this.barycenter.mass_kg);
				CartesianState xzy = (this.barycenter.SpatialRotation * cartesianState.xzy).xzy;
				return this.barycenter.ToGlobalCartesianStateAtTime(dateTime) + xzy;
			}
			if (this.barycenter.barycenter == commonBarycenter)
			{
				return this.barycenter.ToGlobalCartesianStateAtTime(dateTime);
			}
			return this.barycenter.barycenter.ToGlobalCartesianStateAtTime(dateTime);
		}

		// Token: 0x0600418D RID: 16781 RVA: 0x001A679C File Offset: 0x001A499C
		double ITransferTarget.relevant_escapeVelocity_mps(TINaturalSpaceObjectState commonBarycenter)
		{
			if (this.barycenter == commonBarycenter)
			{
				return 0.0;
			}
			if (this.barycenter.barycenter == commonBarycenter)
			{
				return this.barycenter.localEscapeVelocity_mps(this.relevant_orbit_m(commonBarycenter));
			}
			return this.barycenter.barycenter.localEscapeVelocity_mps(this.relevant_orbit_m(commonBarycenter));
		}

		// Token: 0x0600418E RID: 16782 RVA: 0x001A6800 File Offset: 0x001A4A00
		public List<TISpaceFleetState> knownFleetsInOrbit(TIFactionState faction)
		{
			return this.fleetsInOrbit.Where<TISpaceFleetState>((TISpaceFleetState x) => x.VisibleToFaction(faction)).ToList<TISpaceFleetState>();
		}

		// Token: 0x17000C23 RID: 3107
		// (get) Token: 0x0600418F RID: 16783 RVA: 0x001A6838 File Offset: 0x001A4A38
		public List<TISpaceFleetState> fleetsInOrbit
		{
			get
			{
				return (from x in this.assetsInOrbit
					where x.isSpaceFleetState
					select x.ref_fleet).ToList<TISpaceFleetState>();
			}
		}

		// Token: 0x17000C24 RID: 3108
		// (get) Token: 0x06004190 RID: 16784 RVA: 0x001A6898 File Offset: 0x001A4A98
		public List<TIHabState> stationsInOrbit
		{
			get
			{
				return (from x in this.assetsInOrbit
					where x.isHabState
					select x.ref_hab).ToList<TIHabState>();
			}
		}

		// Token: 0x06004191 RID: 16785 RVA: 0x001A68F8 File Offset: 0x001A4AF8
		public override void InitWithTemplate(TIDataTemplate rawTemplate)
		{
			this.templateName = rawTemplate.dataName;
			this.assetsInOrbit = new List<TISpaceAssetState>();
			this.isEarthLEO = this.template.earthLEO;
			this.displayName = this.template.displayName;
			this.barycenter = this.template.barycenter;
		}

		// Token: 0x06004192 RID: 16786 RVA: 0x001A6950 File Offset: 0x001A4B50
		public override void PostGlobalGameStateCreateInit_2()
		{
			this.barycenter = this.template.barycenter;
			this.semiMajorAxis_m = this.template.SemiMajorAxis_m;
			this.semiMajorAxis_km = this.template.SemiMajorAxis_km;
			this.semiMajorAxis_AU = this.template.SemiMajorAxis_AU;
			this.isEarthLEO = this.template.earthLEO;
			if (!this.gameStateSubjectCreated)
			{
				this.amat_ugpy = this.template.amat_ugpy;
			}
			if (this.barycenter == null)
			{
				Log.Error(this.displayName + " has no barycenter", Array.Empty<object>());
			}
			bool flag;
			if (!this.interfaceOrbit && !this.template.interfaceOrbit)
			{
				if (!this.template.synch)
				{
					flag = this.barycenter.orbits.Where<TIOrbitState>((TIOrbitState x) => x.semiMajorAxis_m > this.semiMajorAxis_m).Any<TIOrbitState>((TIOrbitState y) => y.interfaceOrbit);
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = true;
			}
			this.interfaceOrbit = flag;
			new List<TISpaceFleetState>(this.fleetsInOrbit);
			foreach (TISpaceFleetState tispaceFleetState in this.fleetsInOrbit)
			{
				if (tispaceFleetState.faction == null)
				{
					Log.Error("Bad fleet " + tispaceFleetState.ID.ToString() + " was in this savegame, in " + this.displayName, Array.Empty<object>());
					this.assetsInOrbit.Remove(tispaceFleetState);
				}
			}
			this.solarMultiplier = TIHabModuleState.SetLocationSolarPowerMultiplier(this);
		}

		// Token: 0x06004193 RID: 16787 RVA: 0x001A6B08 File Offset: 0x001A4D08
		public override void PostAllStartUpInit_5()
		{
			bool flag;
			if (!this.barycenter.ref_spaceBody.alienTerritory)
			{
				TINaturalSpaceObjectState barycenter = this.barycenter.barycenter;
				flag = barycenter != null && barycenter.ref_spaceBody.alienTerritory;
			}
			else
			{
				flag = true;
			}
			this.alienTerritory = flag;
			if (!this.gameStateSubjectCreated && this.barycenter.GetSunOrbitingRelatedObject == this.barycenter && this.interfaceOrbit)
			{
				List<TIOrbitTemplate> list = (from x in TemplateManager.IterateByClass<TIOrbitTemplate>(true)
					where x.barycenterName == this.barycenter.templateName
					select x).ToList<TIOrbitTemplate>();
				IEnumerable<string> allMoons = from x in TemplateManager.IterateByClass<TISpaceBodyTemplate>(true)
					where x.barycenterName == this.barycenter.templateName
					select x.dataName;
				IEnumerable<TIOrbitTemplate> enumerable = from x in TemplateManager.IterateByClass<TIOrbitTemplate>(true)
					where allMoons.Contains(x.barycenterName)
					select x;
				list.AddRange(enumerable);
				List<TIOrbitState> list2 = new List<TIOrbitState>(this.barycenter.orbits);
				list2.AddRange(this.barycenter.ref_spaceBody.naturalSatellites.SelectMany<TISpaceBodyState, TIOrbitState>((TISpaceBodyState x) => x.orbits));
				float num = list2.Sum<TIOrbitState>((TIOrbitState x) => x.amat_ugpy);
				float num2 = list.Sum<TIOrbitTemplate>((TIOrbitTemplate x) => x.amat_ugpy);
				if (num2 > 0f && num2 > num)
				{
					list2.MaxBy<TIOrbitState, float>((TIOrbitState x) => x.amat_ugpy).amat_ugpy += num2 - num;
				}
			}
			this.gameStateSubjectCreated = true;
		}

		// Token: 0x06004194 RID: 16788 RVA: 0x001A6CE0 File Offset: 0x001A4EE0
		public void AssignToBarycenter()
		{
			this.barycenter.orbits.Add(this);
			this.barycenter.orbits = this.barycenter.orbits.OrderBy<TIOrbitState, double>((TIOrbitState x) => x.semiMajorAxis_km).ToList<TIOrbitState>();
		}

		// Token: 0x06004195 RID: 16789 RVA: 0x001A6D3D File Offset: 0x001A4F3D
		public TINaturalSpaceObjectState FindCommonBarycenter(TIOrbitState orbit)
		{
			return this.barycenter.FindCommonBarycenter(orbit.barycenter);
		}

		// Token: 0x06004196 RID: 16790 RVA: 0x001A6D50 File Offset: 0x001A4F50
		public TINaturalSpaceObjectState FindCommonBarycenter(TISpaceObjectState spaceObject)
		{
			return this.barycenter.FindCommonBarycenter(spaceObject);
		}

		// Token: 0x06004197 RID: 16791 RVA: 0x001A6D5E File Offset: 0x001A4F5E
		public void MarkPendingHab()
		{
			this.pendingHabs++;
		}

		// Token: 0x06004198 RID: 16792 RVA: 0x001A6D6E File Offset: 0x001A4F6E
		public void FoundHab()
		{
			this.pendingHabs--;
		}

		// Token: 0x17000C25 RID: 3109
		// (get) Token: 0x06004199 RID: 16793 RVA: 0x001A6D7E File Offset: 0x001A4F7E
		public double period_s
		{
			get
			{
				return 6.283185307179586 * Mathd.Sqrt(this.semiMajorAxis_m * this.semiMajorAxis_m * this.semiMajorAxis_m / this.barycenter.mu);
			}
		}

		// Token: 0x17000C26 RID: 3110
		// (get) Token: 0x0600419A RID: 16794 RVA: 0x001A6DAF File Offset: 0x001A4FAF
		public float antimatterPerMonth_dekatonnes
		{
			get
			{
				return this.amat_ugpy * 1E-12f * TemplateManager.global.spaceResourceToTons / 12f;
			}
		}

		// Token: 0x0600419B RID: 16795 RVA: 0x001A6DD0 File Offset: 0x001A4FD0
		public Vector3d GetGlobalPositionAtTimeAndAnomaly(TIDateTime time, double meanAnomalyAtEpoch_deg)
		{
			return this.barycenter.GetGlobalPositionAtTime(time) + this.ToOrbitalElementsState(time, meanAnomalyAtEpoch_deg * 0.017453292519943295).ToCartesianStateAtTime(time.ExportTime(), this.barycenter.mass_kg).position;
		}

		// Token: 0x0600419C RID: 16796 RVA: 0x001A6E1E File Offset: 0x001A501E
		public double OffsetToAnomaly_Rad(double desiredOffset_km)
		{
			return desiredOffset_km / this.semiMajorAxis_km;
		}

		// Token: 0x0600419D RID: 16797 RVA: 0x001A6E28 File Offset: 0x001A5028
		public double TestAndCorrectAnomalyToAvoidOverlap(TISpaceAssetState assetToCheck, double proposedAnomaly_Rad, bool docking, bool extraDistance = false)
		{
			foreach (TISpaceAssetState tispaceAssetState in this.assetsInOrbit)
			{
				if (tispaceAssetState != assetToCheck && proposedAnomaly_Rad == tispaceAssetState.meanAnomalyAtEpoch_Rad)
				{
					double num = ((docking || (assetToCheck.ref_faction == tispaceAssetState.ref_faction && !extraDistance)) ? 1.5 : 3.0);
					if (extraDistance)
					{
						num *= 10.0;
					}
					if (this.semiMajorAxis_km > 0.0)
					{
						num /= this.semiMajorAxis_km;
					}
					else
					{
						num /= this.template.SemiMajorAxis_km;
					}
					proposedAnomaly_Rad += num;
					return this.TestAndCorrectAnomalyToAvoidOverlap(assetToCheck, proposedAnomaly_Rad, docking, false);
				}
			}
			return proposedAnomaly_Rad;
		}

		// Token: 0x0600419E RID: 16798 RVA: 0x001A6F10 File Offset: 0x001A5110
		public Orbit ToOrbit(TIDateTime epoch, double meanAnomalyAtEpoch_Rad)
		{
			return new Orbit
			{
				SemimajorAxis_m = this.semiMajorAxis_km * 1000.0,
				Eccentricity = this.eccentricity,
				Inclination_Rad = this.inclination_Rad,
				LongitudeAscendingNode_Rad = this.longitudeAscendingNode_Rad,
				ArgumentPeriapsis_Rad = this.argPeriapsis_Rad,
				MeanAnomalyAtEpoch_Rad = meanAnomalyAtEpoch_Rad,
				Epoch = epoch.ExportTime()
			};
		}

		// Token: 0x0600419F RID: 16799 RVA: 0x001A6F86 File Offset: 0x001A5186
		public OrbitalElementsState ToOrbitalElementsState(TIDateTime epoch, double meanAnomalyAtEpoch_Rad)
		{
			return new OrbitalElementsState(this.ToOrbit(epoch, meanAnomalyAtEpoch_Rad));
		}

		// Token: 0x060041A0 RID: 16800 RVA: 0x001A6F95 File Offset: 0x001A5195
		public bool NewStationAllowed(int tier = 0, TIFactionState faction = null)
		{
			return this.stationsInOrbit.Count + this.pendingHabs < this.stationCapacity && tier <= this.ref_naturalSpaceObject.maxHabTier;
		}

		// Token: 0x17000C27 RID: 3111
		// (get) Token: 0x060041A1 RID: 16801 RVA: 0x001A6FC4 File Offset: 0x001A51C4
		public double localEscapeVelocity_mps
		{
			get
			{
				return this.barycenter.localEscapeVelocity_mps(this.semiMajorAxis_m);
			}
		}

		// Token: 0x17000C28 RID: 3112
		// (get) Token: 0x060041A2 RID: 16802 RVA: 0x001A6FD7 File Offset: 0x001A51D7
		public double localGravity_mps2
		{
			get
			{
				return this.barycenter.mu / (this.semiMajorAxis_m * this.semiMajorAxis_m);
			}
		}

		// Token: 0x17000C29 RID: 3113
		// (get) Token: 0x060041A3 RID: 16803 RVA: 0x001A6FF2 File Offset: 0x001A51F2
		public double localGravity_kps2
		{
			get
			{
				return this.localGravity_mps2 / 1000.0;
			}
		}

		// Token: 0x17000C2A RID: 3114
		// (get) Token: 0x060041A4 RID: 16804 RVA: 0x001A7004 File Offset: 0x001A5204
		public double localGravity_gs
		{
			get
			{
				return this.localGravity_mps2 / 9.806650161743164;
			}
		}

		// Token: 0x17000C2B RID: 3115
		// (get) Token: 0x060041A5 RID: 16805 RVA: 0x001A7016 File Offset: 0x001A5216
		public double averageOrbitalVelocity_mps
		{
			get
			{
				return Mathd.Sqrt(this.barycenter.mu / this.semiMajorAxis_m);
			}
		}

		// Token: 0x17000C2C RID: 3116
		// (get) Token: 0x060041A6 RID: 16806 RVA: 0x001A702F File Offset: 0x001A522F
		public double averageOrbitalVelocity_kps
		{
			get
			{
				return this.averageOrbitalVelocity_mps / 1000.0;
			}
		}

		// Token: 0x060041A7 RID: 16807 RVA: 0x001A7044 File Offset: 0x001A5244
		public double DeltaVToReachFromSurface_kps(float latitude_deg)
		{
			if (this.barycenter.isSpaceBodyState)
			{
				TISpaceBodyState ref_spaceBody = this.barycenter.ref_spaceBody;
				return Mathd.Abs(this.averageOrbitalVelocity_kps - Mathd.Cos((double)latitude_deg * 0.017453292519943295) * ref_spaceBody.circumfrence_km / ref_spaceBody.rotationperiod_s + ref_spaceBody.DragVelocityPenaltyToReachOrbit_kps());
			}
			return 0.0;
		}

		// Token: 0x060041A8 RID: 16808 RVA: 0x001A70A8 File Offset: 0x001A52A8
		public double DeltaVToReachFromSurface_kps(float latitude_deg, double fleetAcceleration_mps2)
		{
			if (!this.barycenter.isSpaceBodyState)
			{
				return 0.0;
			}
			TISpaceBodyState ref_spaceBody = this.barycenter.ref_spaceBody;
			double num;
			double num2;
			if (this.altitude_km < 200.0)
			{
				num = (this.localGravity_mps2 + ref_spaceBody.surfaceGravity_mps2) / 2.0;
				num2 = Mathd.Sqrt(2.0 * this.altitude_m / (fleetAcceleration_mps2 - num * 0.5)) * num;
				return Mathd.Abs(this.averageOrbitalVelocity_kps - Mathd.Cos((double)latitude_deg * 0.017453292519943295) * ref_spaceBody.circumfrence_km / ref_spaceBody.rotationperiod_s + num2 / 1000.0 + ref_spaceBody.DragVelocityPenaltyToReachOrbit_kps());
			}
			int num3 = 200000;
			double num4 = (double)num3 + ref_spaceBody.meanRadius_m;
			num = (this.barycenter.mu / (num4 * num4) + ref_spaceBody.surfaceGravity_mps2) / 2.0;
			num2 = Mathd.Sqrt((double)(2 * num3) / (fleetAcceleration_mps2 - num * 0.5)) * num;
			double num5 = Mathd.Sqrt(this.barycenter.mu / num4);
			double num6 = Mathd.Abs(num5 - this.averageOrbitalVelocity_mps);
			return Mathd.Abs(num5 / 1000.0 - Mathd.Cos((double)latitude_deg * 0.017453292519943295) * ref_spaceBody.circumfrence_km / ref_spaceBody.rotationperiod_s + num2 / 1000.0 + num6 / 1000.0 + ref_spaceBody.DragVelocityPenaltyToReachOrbit_kps());
		}

		// Token: 0x060041A9 RID: 16809 RVA: 0x001A722C File Offset: 0x001A542C
		public CartesianState? tryToGetGlobalCartesianState(TIDateTime time)
		{
			return null;
		}

		// Token: 0x060041AA RID: 16810 RVA: 0x001A7242 File Offset: 0x001A5442
		public bool tryToGetLocalCartesianState(TIDateTime time, out CartesianState cartesianState, out TINaturalSpaceObjectState barycenter)
		{
			barycenter = this.barycenter;
			cartesianState = default(CartesianState);
			return false;
		}

		// Token: 0x060041AB RID: 16811 RVA: 0x001A7254 File Offset: 0x001A5454
		public TINaturalSpaceObjectState localBarycenter(TIDateTime time)
		{
			return this.barycenter;
		}

		// Token: 0x060041AC RID: 16812 RVA: 0x001A725C File Offset: 0x001A545C
		public void getOrbitalElementsState(TIDateTime time, out OrbitalElementsState orbitalElementsState, out TINaturalSpaceObjectState barycenter, out bool meanAnomalyIsGood)
		{
			barycenter = this.barycenter;
			meanAnomalyIsGood = false;
			double num = 2000.0;
			orbitalElementsState = new OrbitalElementsState(this, 0.0, new TIDateTime().SetTime(num));
		}

		// Token: 0x060041AD RID: 16813 RVA: 0x001A729F File Offset: 0x001A549F
		public void DestroyedAssetsChange(int change)
		{
			this.destroyedAssets += change;
			this.destroyedAssets = Mathf.Max(0, this.destroyedAssets);
		}

		// Token: 0x060041AE RID: 16814 RVA: 0x001A72C4 File Offset: 0x001A54C4
		public bool OrbitOfInterest(TIFactionState faction, int filter)
		{
			switch (filter)
			{
			case 1:
				return this.assetsInOrbit.Any<TISpaceAssetState>((TISpaceAssetState x) => x.faction == faction) || (this.interfaceOrbit && this.ref_spaceBody != null && (this.ref_spaceBody.surfaceBases.Any<TIHabState>((TIHabState x) => x.faction == faction) || this.ref_spaceBody.landedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => x.faction == faction)));
			case 2:
				return this.ref_naturalSpaceObject.fleetsInOrbit.Any<TISpaceFleetState>((TISpaceFleetState x) => x.faction == faction) || this.ref_naturalSpaceObject.stationsInOrbit.Any<TIHabState>((TIHabState x) => x.faction == faction) || (this.ref_spaceBody != null && (this.ref_spaceBody.surfaceBases.Any<TIHabState>((TIHabState x) => x.faction == faction) || this.ref_spaceBody.landedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => x.faction == faction)));
			case 3:
			{
				TINaturalSpaceObjectState ref_naturalSpaceObject = this.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.ref_naturalSpaceObject;
				if (ref_naturalSpaceObject != null)
				{
					if (ref_naturalSpaceObject.fleetsInSystem.Any<TISpaceFleetState>((TISpaceFleetState x) => x.faction == faction))
					{
						return true;
					}
					if (ref_naturalSpaceObject.habsInSystem.Any<TIHabState>((TIHabState x) => x.faction == faction))
					{
						return true;
					}
				}
				return false;
			}
			default:
				return false;
			}
		}

		// Token: 0x060041AF RID: 16815 RVA: 0x001A744C File Offset: 0x001A564C
		public int OrbitInterestLevel(TIFactionState faction)
		{
			if (this.assetsInOrbit.Any<TISpaceAssetState>((TISpaceAssetState x) => x.faction == faction))
			{
				return 3;
			}
			if (this.ref_spaceBody != null && (this.ref_spaceBody.surfaceBases.Any<TIHabState>((TIHabState x) => x.faction == faction) || this.ref_spaceBody.landedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => x.faction == faction)))
			{
				if (this.interfaceOrbit)
				{
					return 3;
				}
				return 2;
			}
			else
			{
				if (this.ref_naturalSpaceObject.fleetsInOrbit.Any<TISpaceFleetState>((TISpaceFleetState x) => x.faction == faction))
				{
					return 2;
				}
				if (this.ref_naturalSpaceObject.stationsInOrbit.Any<TIHabState>((TIHabState x) => x.faction == faction))
				{
					return 2;
				}
				TINaturalSpaceObjectState ref_naturalSpaceObject = this.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.ref_naturalSpaceObject;
				if (ref_naturalSpaceObject != null)
				{
					if (ref_naturalSpaceObject.fleetsInSystem.Any<TISpaceFleetState>((TISpaceFleetState x) => x.faction == faction))
					{
						return 1;
					}
					if (ref_naturalSpaceObject.habsInSystem.Any<TIHabState>((TIHabState x) => x.faction == faction))
					{
						return 1;
					}
				}
				return 0;
			}
		}

		// Token: 0x060041B0 RID: 16816 RVA: 0x001A7570 File Offset: 0x001A5770
		public static string OrbitTooltip(TIOrbitState orbit)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(orbit.displayName);
			if (orbit.interfaceOrbit)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Space.OrbitInterface"));
			}
			stringBuilder.AppendLine().AppendLine(Loc.T("UI.Space.SolarMultiplier", new object[]
			{
				TemplateManager.global.pathInlineSolarIcon,
				TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(orbit.solarMultiplier, 7, 1, true, false))
			}));
			if (orbit.irradiated)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Space.OrbitHazard", new object[]
				{
					TemplateManager.global.irradiatedInlineSpritePath,
					TIUtilities.RedLine(orbit.irradiatedValue.ToString())
				}));
			}
			if (orbit.amat_ugpy > 0f)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Space.AMAT", new object[] { TemplateManager.global.antimatterInlineSpritePath }));
			}
			if (orbit.destroyedAssets > 0)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Space.Debris", new object[]
				{
					TemplateManager.global.spaceDebrisInlineSpritePath,
					orbit.destroyedAssets.ToString("N0")
				}));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x040027A5 RID: 10149
		public List<TISpaceAssetState> assetsInOrbit;

		// Token: 0x040027A6 RID: 10150
		public int pendingHabs;

		// Token: 0x040027A7 RID: 10151
		public int destroyedAssets;

		// Token: 0x040027A9 RID: 10153
		[fsIgnore]
		public float solarMultiplier;

		// Token: 0x040027AA RID: 10154
		public bool interfaceOrbit;

		// Token: 0x040027AB RID: 10155
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x040027AC RID: 10156
		[fsIgnore]
		public double semiMajorAxis_km;

		// Token: 0x040027AD RID: 10157
		[fsIgnore]
		public double semiMajorAxis_m;

		// Token: 0x040027AE RID: 10158
		[fsIgnore]
		public double semiMajorAxis_AU;

		// Token: 0x040027AF RID: 10159
		[fsIgnore]
		public bool isEarthLEO;

		// Token: 0x040027B0 RID: 10160
		[fsIgnore]
		public bool alienTerritory;
	}
}
