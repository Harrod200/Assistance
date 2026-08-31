using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007A1 RID: 1953
	internal class TIAdHocOrbitState : TIOrbitState
	{
		// Token: 0x17000B2A RID: 2858
		// (get) Token: 0x06003EC0 RID: 16064 RVA: 0x00195E91 File Offset: 0x00194091
		public override TIOrbitTemplate template
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000B2B RID: 2859
		// (get) Token: 0x06003EC1 RID: 16065 RVA: 0x00195E94 File Offset: 0x00194094
		public override double eccentricity
		{
			get
			{
				return this._eccentricity;
			}
		}

		// Token: 0x17000B2C RID: 2860
		// (get) Token: 0x06003EC2 RID: 16066 RVA: 0x00195E9C File Offset: 0x0019409C
		public override double inclination_Rad
		{
			get
			{
				return this._inclination_rad;
			}
		}

		// Token: 0x17000B2D RID: 2861
		// (get) Token: 0x06003EC3 RID: 16067 RVA: 0x00195EA4 File Offset: 0x001940A4
		public override double longitudeAscendingNode_Rad
		{
			get
			{
				return this._longitudeAscendingNode_rad;
			}
		}

		// Token: 0x17000B2E RID: 2862
		// (get) Token: 0x06003EC4 RID: 16068 RVA: 0x00195EAC File Offset: 0x001940AC
		public override double argPeriapsis_Rad
		{
			get
			{
				return this._argumentPeriapsis_rad;
			}
		}

		// Token: 0x17000B2F RID: 2863
		// (get) Token: 0x06003EC5 RID: 16069 RVA: 0x00195EB4 File Offset: 0x001940B4
		public override int stationCapacity
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000B30 RID: 2864
		// (get) Token: 0x06003EC6 RID: 16070 RVA: 0x00195EB7 File Offset: 0x001940B7
		public override bool isAdHocOrbit
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000B31 RID: 2865
		// (get) Token: 0x06003EC7 RID: 16071 RVA: 0x00195EBA File Offset: 0x001940BA
		public override bool irradiated
		{
			get
			{
				return this._irradiated;
			}
		}

		// Token: 0x17000B32 RID: 2866
		// (get) Token: 0x06003EC8 RID: 16072 RVA: 0x00195EC2 File Offset: 0x001940C2
		public override float amat_ugpy
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x06003EC9 RID: 16073 RVA: 0x00195ECC File Offset: 0x001940CC
		public override void PostGlobalGameStateCreateInit_2()
		{
			if (this._semimajorAxis_m < 0.0 || this._eccentricity < 0.0 || this._eccentricity >= 1.0)
			{
				Log.Error("Hyperbolic TIAdHocOrbitState.  Circularizing to prevent crashes.", Array.Empty<object>());
				this._eccentricity = 0.0;
				this._semimajorAxis_m = this.barycenter.meanRadius_m * 10.0;
			}
			if (double.IsNaN(this._argumentPeriapsis_rad))
			{
				Log.Error("TIAdHocOrbitState has a NaN argument of periapsis.  Setting to zero to avoid crashes.", Array.Empty<object>());
				this._argumentPeriapsis_rad = 0.0;
			}
			this.semiMajorAxis_km = this._semimajorAxis_m / 1000.0;
			this.semiMajorAxis_m = this._semimajorAxis_m;
			this.semiMajorAxis_AU = this._semimajorAxis_m / 149597870700.0;
			if (this.barycenter == null)
			{
				Log.Error(this.displayName + " has no barycenter", Array.Empty<object>());
			}
			this.assetsInOrbit.RemoveAll((TISpaceAssetState x) => x.isSpaceFleetState);
			this.assetsInOrbit.AddRange(from x in GameStateManager.IterateByClass<TISpaceFleetState>(false)
				where x.faction != null && x.ref_orbit == this
				select x);
		}

		// Token: 0x06003ECA RID: 16074 RVA: 0x00196020 File Offset: 0x00194220
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
			bool flag2;
			if (this.barycenter.isEarth)
			{
				flag2 = this._semimajorAxis_m <= this.barycenter.orbits.Where<TIOrbitState>((TIOrbitState x) => !x.isAdHocOrbit && x.isEarthLEO).Max<TIOrbitState>((TIOrbitState x) => x.semiMajorAxis_m);
			}
			else
			{
				flag2 = false;
			}
			this.isEarthLEO = flag2;
			double num = 0.0;
			foreach (TIOrbitState tiorbitState in this.barycenter.orbits.Where<TIOrbitState>((TIOrbitState x) => x.irradiated))
			{
				num = Mathd.Max(num, tiorbitState.semiMajorAxis_m);
			}
			this._irradiated = num >= this._semimajorAxis_m;
		}

		// Token: 0x06003ECB RID: 16075 RVA: 0x00196164 File Offset: 0x00194364
		private void SetRunTimeData()
		{
			this.PostGlobalGameStateCreateInit_2();
			this.PostAllStartUpInit_5();
		}

		// Token: 0x06003ECC RID: 16076 RVA: 0x00196174 File Offset: 0x00194374
		public static TIAdHocOrbitState CreateAdHocOrbitState(TINaturalSpaceObjectState barycenter, double semimajorAxis_m, double eccentricity, double inclination_Rad, double longitudeAscendingNode_Rad, double argumentPeriapsis_Rad, TISpaceFleetState foundingFleet)
		{
			TIAdHocOrbitState tiadHocOrbitState = GameStateManager.CreateNewGameState<TIAdHocOrbitState>();
			tiadHocOrbitState.barycenter = barycenter;
			tiadHocOrbitState.AssignToBarycenter();
			tiadHocOrbitState._semimajorAxis_m = semimajorAxis_m;
			tiadHocOrbitState._eccentricity = eccentricity;
			if (tiadHocOrbitState._eccentricity >= 1.0 || tiadHocOrbitState._semimajorAxis_m <= 0.0)
			{
				Debug.LogError("Creating hyperbolic ad-hoc 'orbit'.  This should have been a trajectory.");
				tiadHocOrbitState._eccentricity = 0.0;
				tiadHocOrbitState._semimajorAxis_m = barycenter.meanRadius_m * 10.0;
			}
			tiadHocOrbitState._inclination_rad = inclination_Rad;
			tiadHocOrbitState._longitudeAscendingNode_rad = longitudeAscendingNode_Rad;
			tiadHocOrbitState._argumentPeriapsis_rad = argumentPeriapsis_Rad;
			tiadHocOrbitState.interfaceOrbit = false;
			tiadHocOrbitState.assetsInOrbit = new List<TISpaceAssetState>();
			tiadHocOrbitState.SetDisplayName(Loc.T("TIOrbitTemplate.tempOrbit", new object[] { foundingFleet.GetDisplayName(GameControl.control.activePlayer) }));
			tiadHocOrbitState.SetRunTimeData();
			return tiadHocOrbitState;
		}

		// Token: 0x06003ECD RID: 16077 RVA: 0x00196250 File Offset: 0x00194450
		public static TIAdHocOrbitState CreateAdHocOrbitState(TINaturalSpaceObjectState barycenter, OrbitalElementsState orbitalElements, TISpaceFleetState foundingFleet)
		{
			double num = orbitalElements.semiMajorAxis_m;
			double num2 = orbitalElements.eccentricity;
			if (num2 >= 1.0 || num <= 0.0)
			{
				Debug.LogError("Creating hyperbolic ad-hoc 'orbit'.  This should have been a trajectory.");
				num2 = 0.0;
				num = barycenter.meanRadius_m * 10.0;
			}
			if (num2 < 0.0)
			{
				Debug.LogError("Creating impossible ad-hoc orbit with negative eccentricity.  Something has gone very wrong.");
				if (num > barycenter.hillRadius_m || num > 50000000000000.0 || num <= 0.0)
				{
					num = barycenter.meanRadius_m * 10.0;
				}
			}
			return TIAdHocOrbitState.CreateAdHocOrbitState(barycenter, orbitalElements.semiMajorAxis_m, orbitalElements.eccentricity, orbitalElements.inclination_Rad, orbitalElements.longAscendingNode_Rad, orbitalElements.argPeriapsis_Rad, foundingFleet);
		}

		// Token: 0x04002714 RID: 10004
		[SerializeField]
		private double _semimajorAxis_m;

		// Token: 0x04002715 RID: 10005
		[SerializeField]
		private double _eccentricity;

		// Token: 0x04002716 RID: 10006
		[SerializeField]
		private double _inclination_rad;

		// Token: 0x04002717 RID: 10007
		[SerializeField]
		private double _longitudeAscendingNode_rad;

		// Token: 0x04002718 RID: 10008
		[SerializeField]
		private double _argumentPeriapsis_rad;

		// Token: 0x04002719 RID: 10009
		[SerializeField]
		private bool _irradiated;
	}
}
