using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007A8 RID: 1960
	public class TILagrangePointState : TINaturalSpaceObjectState
	{
		// Token: 0x17000BD6 RID: 3030
		// (get) Token: 0x060040CC RID: 16588 RVA: 0x001A2D69 File Offset: 0x001A0F69
		public new TINavigableTemplate template
		{
			get
			{
				return this.GetMyTemplate<TINavigableTemplate>();
			}
		}

		// Token: 0x17000BD7 RID: 3031
		// (get) Token: 0x060040CD RID: 16589 RVA: 0x001A2D71 File Offset: 0x001A0F71
		public override SpaceObjectType objectType
		{
			get
			{
				return SpaceObjectType.LagrangePoint;
			}
		}

		// Token: 0x17000BD8 RID: 3032
		// (get) Token: 0x060040CE RID: 16590 RVA: 0x001A2D75 File Offset: 0x001A0F75
		public override double mass_kg
		{
			get
			{
				return this.secondaryObject.mass_kg / 10000000.0;
			}
		}

		// Token: 0x17000BD9 RID: 3033
		// (get) Token: 0x060040CF RID: 16591 RVA: 0x001A2D8C File Offset: 0x001A0F8C
		// (set) Token: 0x060040D0 RID: 16592 RVA: 0x001A2D94 File Offset: 0x001A0F94
		public TISpaceBodyState secondaryObject { get; private set; }

		// Token: 0x17000BDA RID: 3034
		// (get) Token: 0x060040D1 RID: 16593 RVA: 0x001A2D9D File Offset: 0x001A0F9D
		public override bool isLagrangePointState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000BDB RID: 3035
		// (get) Token: 0x060040D2 RID: 16594 RVA: 0x001A2DA0 File Offset: 0x001A0FA0
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				return this.secondaryObject;
			}
		}

		// Token: 0x17000BDC RID: 3036
		// (get) Token: 0x060040D3 RID: 16595 RVA: 0x001A2DA8 File Offset: 0x001A0FA8
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000BDD RID: 3037
		// (get) Token: 0x060040D4 RID: 16596 RVA: 0x001A2DAB File Offset: 0x001A0FAB
		public override TILagrangePointState ref_lagrangePoint
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000BDE RID: 3038
		// (get) Token: 0x060040D5 RID: 16597 RVA: 0x001A2DAE File Offset: 0x001A0FAE
		public LagrangeValue lagrangeValue
		{
			get
			{
				return this.template.lagrangeValue;
			}
		}

		// Token: 0x17000BDF RID: 3039
		// (get) Token: 0x060040D6 RID: 16598 RVA: 0x001A2DBC File Offset: 0x001A0FBC
		public override ulong population
		{
			get
			{
				List<TIOrbitState> orbits = this.orbits;
				int? num;
				if (orbits == null)
				{
					num = null;
				}
				else
				{
					num = new int?((from x in orbits.SelectMany<TIOrbitState, TIHabState>((TIOrbitState x) => x.stationsInOrbit)
						where !x.IsAlien()
						select x).Sum<TIHabState>((TIHabState y) => y.crew));
				}
				int? num2 = num;
				return ((num2 != null) ? new ulong?((ulong)((long)num2.GetValueOrDefault())) : null).GetValueOrDefault();
			}
		}

		// Token: 0x060040D7 RID: 16599 RVA: 0x001A2E79 File Offset: 0x001A1079
		public override bool Colonized()
		{
			return this.GetSunOrbitingRelatedObject.isEarth || base.Colonized();
		}

		// Token: 0x060040D8 RID: 16600 RVA: 0x001A2E90 File Offset: 0x001A1090
		public override bool Populous()
		{
			return this.GetSunOrbitingRelatedObject.isEarth || base.Populous();
		}

		// Token: 0x060040D9 RID: 16601 RVA: 0x001A2EA8 File Offset: 0x001A10A8
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			if (this.template != null)
			{
				TISpaceBodyState tispaceBodyState = GameStateManager.FindByTemplate<TISpaceBodyState>(this.template.relatedObject, true);
				if (!Error.IsNull<TISpaceBodyState>(tispaceBodyState))
				{
					base.epoch_DateTime.SetTime(tispaceBodyState.epoch_JYears);
					this.secondaryObject = tispaceBodyState;
					this.barycenter = tispaceBodyState.barycenter;
				}
			}
			base.CreateOrbitStates();
			if (this.orbits.Count == 0)
			{
				Log.Warn(this.displayName + " has no orbits around it.", Array.Empty<object>());
			}
		}

		// Token: 0x060040DA RID: 16602 RVA: 0x001A2F2C File Offset: 0x001A112C
		public override void PostGlobalGameStateCreateInit_2()
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this._sunOrbitingRelatedObject = (TINaturalSpaceObjectState)TISpaceObjectState.GetSunOrbitingRelatedObject_static(this);
			this._orbitalPeriod_s = ((this.barycenter != null) ? (6.283185307179586 * Mathd.Sqrt(this.semiMajorAxis_m * this.semiMajorAxis_m * this.semiMajorAxis_m / (6.67384E-11 * this.barycenter.mass_kg))) : 1.0);
			if (this.orbits.Count > 0)
			{
				base.sphereOfInfluence_m = this.orbits.Where<TIOrbitState>((TIOrbitState x) => !x.isAdHocOrbit).Max<TIOrbitState>((TIOrbitState x) => x.template.SemiMajorAxis_m) * 2.0;
			}
			else
			{
				base.sphereOfInfluence_m = 500000.0;
			}
			base.localBarycenterGravity_kps2 = (float)(6.67384E-11 * this.barycenter.mass_kg / (this.semiMajorAxis_m * this.semiMajorAxis_m) / 1000.0);
			base.SetHillRadius_m();
			base.hillRadius_m = Mathd.Max(base.sphereOfInfluence_m, base.hillRadius_m / 3.0);
		}

		// Token: 0x060040DB RID: 16603 RVA: 0x001A308C File Offset: 0x001A128C
		public override Vector3d GetGlobalPositionAtTime(TIDateTime time)
		{
			return this.template.positionCalculator.GetPosition(this.secondaryObject, time, true).xzy;
		}

		// Token: 0x060040DC RID: 16604 RVA: 0x001A30B9 File Offset: 0x001A12B9
		public override CartesianState ToGlobalCartesianStateAtTime(TIDateTime time)
		{
			return (this.template.positionCalculator as LagrangePosition).GetCartesianState(this.secondaryObject, time);
		}
	}
}
