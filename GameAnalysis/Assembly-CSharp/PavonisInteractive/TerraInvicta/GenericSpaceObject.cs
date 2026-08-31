using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007BE RID: 1982
	public class GenericSpaceObject
	{
		// Token: 0x17000D5A RID: 3418
		// (get) Token: 0x06004509 RID: 17673 RVA: 0x001C2BE9 File Offset: 0x001C0DE9
		// (set) Token: 0x0600450A RID: 17674 RVA: 0x001C2BF1 File Offset: 0x001C0DF1
		public TIGameState trueState { get; private set; }

		// Token: 0x0600450B RID: 17675 RVA: 0x001C2BFC File Offset: 0x001C0DFC
		public void AssignData(TIGameState state)
		{
			this.trueState = state;
			if (state.isSpaceBodyState)
			{
				TISpaceBodyState ref_spaceBody = state.ref_spaceBody;
				this.barycenter = ref_spaceBody.barycenter;
				this.semiMajorAxis_m = ref_spaceBody.semiMajorAxis_m;
				return;
			}
			if (state.isLagrangePointState)
			{
				TILagrangePointState ref_lagrangePoint = state.ref_lagrangePoint;
				this.barycenter = ref_lagrangePoint.barycenter;
				this.semiMajorAxis_m = ref_lagrangePoint.semiMajorAxis_m;
				return;
			}
			if (state.isOrbitState)
			{
				TIOrbitState ref_orbit = state.ref_orbit;
				this.barycenter = ref_orbit.barycenter;
				this.semiMajorAxis_m = ref_orbit.semiMajorAxis_m;
				return;
			}
			if (state.isHabSiteState)
			{
				TIHabSiteState ref_habSite = state.ref_habSite;
				this.barycenter = ref_habSite.parentBody;
				this.semiMajorAxis_m = ref_habSite.parentBody.meanRadius_m;
				return;
			}
			if (!state.isSpaceFleetState)
			{
				if (state.isHabModuleState)
				{
					state = state.ref_hab;
				}
				if (state.isHabState)
				{
					TIHabState ref_hab = state.ref_hab;
					if (ref_hab.IsStation)
					{
						this.barycenter = ref_hab.barycenter;
						this.semiMajorAxis_m = ref_hab.orbitState.semiMajorAxis_m;
						return;
					}
					if (ref_hab.IsBase)
					{
						this.barycenter = ref_hab.habSite.parentBody;
						this.semiMajorAxis_m = ref_hab.habSite.parentBody.meanRadius_m;
						return;
					}
				}
				else if (state.isRegionState)
				{
					this.barycenter = state.ref_spaceBody;
					this.semiMajorAxis_m = this.barycenter.semiMajorAxis_m;
				}
				return;
			}
			TISpaceFleetState ref_fleet = state.ref_fleet;
			if (!ref_fleet.inTransfer)
			{
				this.barycenter = ref_fleet.barycenter;
				this.semiMajorAxis_m = ref_fleet.semiMajorAxis_m;
				return;
			}
			this.barycenter = ref_fleet.trajectory.commonBarycenter;
			Trajectory_WithOrbitalElements trajectory_WithOrbitalElements = ref_fleet.trajectory as Trajectory_WithOrbitalElements;
			if (trajectory_WithOrbitalElements != null)
			{
				this.semiMajorAxis_m = trajectory_WithOrbitalElements.transferOrbit.semiMajorAxis_m;
				return;
			}
			this.semiMajorAxis_m = TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(ref_fleet, this.barycenter);
		}

		// Token: 0x0600450C RID: 17676 RVA: 0x001C2DD8 File Offset: 0x001C0FD8
		public TINaturalSpaceObjectState FindCommonBarycenter(GenericSpaceObject genericSpaceObject)
		{
			List<TINaturalSpaceObjectState> list = new List<TINaturalSpaceObjectState>();
			TINaturalSpaceObjectState tinaturalSpaceObjectState = this.trueState as TINaturalSpaceObjectState;
			if (tinaturalSpaceObjectState != null)
			{
				list.Add(tinaturalSpaceObjectState);
			}
			if (this.barycenter != null)
			{
				list.Add(this.barycenter);
				if (this.barycenter.barycenter != null)
				{
					list.Add(this.barycenter.barycenter);
					if (this.barycenter.barycenter.barycenter != null)
					{
						list.Add(this.barycenter.barycenter.barycenter);
						if (this.barycenter.barycenter.barycenter.barycenter != null)
						{
							list.Add(this.barycenter.barycenter.barycenter.barycenter);
						}
					}
				}
			}
			List<TISpaceObjectState> list2 = new List<TISpaceObjectState>();
			if (genericSpaceObject.trueState.isSpaceObjectState)
			{
				list2.Add(genericSpaceObject.trueState.ref_spaceObject);
			}
			if (genericSpaceObject.barycenter != null)
			{
				list2.Add(genericSpaceObject.barycenter);
				if (genericSpaceObject.barycenter.barycenter != null)
				{
					list2.Add(genericSpaceObject.barycenter.barycenter);
					if (genericSpaceObject.barycenter.barycenter.barycenter != null)
					{
						list2.Add(genericSpaceObject.barycenter.barycenter.barycenter);
						if (genericSpaceObject.barycenter.barycenter.barycenter.barycenter != null)
						{
							list2.Add(genericSpaceObject.barycenter.barycenter.barycenter.barycenter);
						}
					}
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					if (list[i] == list2[j])
					{
						return list[i];
					}
				}
			}
			if (this.trueState == null)
			{
				Log.Error("Missing trueState in TISpaceObjectState.FindCommonbaryCenter", Array.Empty<object>());
			}
			else if (genericSpaceObject.trueState == null)
			{
				Log.Error("Missing trueState for GenericSpaceObject in TISpaceObjectState.FindCommonBaryCenter", Array.Empty<object>());
			}
			else
			{
				Log.Error("Could not find common barycenter for " + this.trueState.templateName + " and " + genericSpaceObject.trueState.templateName, Array.Empty<object>());
			}
			return null;
		}

		// Token: 0x0600450D RID: 17677 RVA: 0x001C302C File Offset: 0x001C122C
		public double GetRelevantSemimajorAxis_m(TISpaceObjectState testBarycenter)
		{
			if (testBarycenter == null)
			{
				Error.Log("Body will null barycenter passed to GetRelevantSemimajorAxis", Array.Empty<object>());
				return -1.0;
			}
			if (testBarycenter == this.trueState)
			{
				if (testBarycenter.isSpaceBodyState)
				{
					return testBarycenter.ref_spaceBody.meanRadius_m;
				}
				return 0.0;
			}
			else
			{
				if (testBarycenter == this.barycenter)
				{
					return this.semiMajorAxis_m;
				}
				if (testBarycenter == this.barycenter.barycenter)
				{
					return this.barycenter.semiMajorAxis_m;
				}
				if (testBarycenter == this.barycenter.barycenter.barycenter)
				{
					return this.barycenter.barycenter.semiMajorAxis_m;
				}
				if (testBarycenter == this.barycenter.barycenter.barycenter.barycenter)
				{
					return this.barycenter.barycenter.barycenter.semiMajorAxis_m;
				}
				return -1.0;
			}
		}

		// Token: 0x0600450E RID: 17678 RVA: 0x001C3120 File Offset: 0x001C1320
		public double GetRelevantEccentricity(TISpaceObjectState testBarycenter)
		{
			if (testBarycenter == null)
			{
				Error.Log("Body will null barycenter passed to GetRelevantSemimajorAxis", Array.Empty<object>());
				return -1.0;
			}
			if (testBarycenter == this.trueState)
			{
				if (testBarycenter.isSpaceBodyState)
				{
					return testBarycenter.ref_spaceBody.ecc;
				}
				return 0.0;
			}
			else
			{
				if (testBarycenter == this.barycenter)
				{
					return this.trueState.ref_spaceObject.ecc;
				}
				if (testBarycenter == this.barycenter.barycenter)
				{
					return this.barycenter.ecc;
				}
				if (testBarycenter == this.barycenter.barycenter.barycenter)
				{
					return this.barycenter.barycenter.ecc;
				}
				if (testBarycenter == this.barycenter.barycenter.barycenter.barycenter)
				{
					return this.barycenter.barycenter.barycenter.ecc;
				}
				return -1.0;
			}
		}

		// Token: 0x0600450F RID: 17679 RVA: 0x001C3220 File Offset: 0x001C1420
		public double GetRelevantInclination_Rad(TISpaceObjectState testBarycenter)
		{
			if (testBarycenter == null)
			{
				Error.Log("Body will null barycenter passed to GetRelevantSemimajorAxis", Array.Empty<object>());
				return -1.0;
			}
			if (testBarycenter == this.trueState)
			{
				if (testBarycenter.isSpaceBodyState)
				{
					return testBarycenter.ref_spaceBody.inclination_Rad;
				}
				return 0.0;
			}
			else
			{
				if (testBarycenter == this.barycenter)
				{
					return this.trueState.ref_spaceObject.inclination_Rad;
				}
				if (testBarycenter == this.barycenter.barycenter)
				{
					return this.barycenter.inclination_Rad;
				}
				if (testBarycenter == this.barycenter.barycenter.barycenter)
				{
					return this.barycenter.barycenter.inclination_Rad;
				}
				if (testBarycenter == this.barycenter.barycenter.barycenter.barycenter)
				{
					return this.barycenter.barycenter.barycenter.inclination_Rad;
				}
				return -1.0;
			}
		}

		// Token: 0x06004510 RID: 17680 RVA: 0x001C3320 File Offset: 0x001C1520
		public double GetRelevantArgPeriapsis_Rad(TISpaceObjectState testBarycenter)
		{
			if (testBarycenter == null)
			{
				Error.Log("Body will null barycenter passed to GetRelevantSemimajorAxis", Array.Empty<object>());
				return -1.0;
			}
			if (testBarycenter == this.trueState)
			{
				if (testBarycenter.isSpaceBodyState)
				{
					return testBarycenter.ref_spaceBody.argPeriapsis_Rad;
				}
				return 0.0;
			}
			else
			{
				if (testBarycenter == this.barycenter)
				{
					return this.trueState.ref_spaceObject.argPeriapsis_Rad;
				}
				if (testBarycenter == this.barycenter.barycenter)
				{
					return this.barycenter.argPeriapsis_Rad;
				}
				if (testBarycenter == this.barycenter.barycenter.barycenter)
				{
					return this.barycenter.barycenter.argPeriapsis_Rad;
				}
				if (testBarycenter == this.barycenter.barycenter.barycenter.barycenter)
				{
					return this.barycenter.barycenter.barycenter.argPeriapsis_Rad;
				}
				return -1.0;
			}
		}

		// Token: 0x06004511 RID: 17681 RVA: 0x001C3420 File Offset: 0x001C1620
		public double GetRelevantMeanAnomaly_Rad(TISpaceObjectState testBarycenter)
		{
			if (testBarycenter == null)
			{
				Error.Log("Body will null barycenter passed to GetRelevantSemimajorAxis", Array.Empty<object>());
				return -1.0;
			}
			if (testBarycenter == this.trueState)
			{
				if (testBarycenter.isSpaceBodyState)
				{
					return testBarycenter.ref_spaceBody.meanAnomalyAtEpoch_Rad;
				}
				return 0.0;
			}
			else
			{
				if (testBarycenter == this.barycenter)
				{
					return this.trueState.ref_spaceObject.meanAnomalyAtEpoch_Rad;
				}
				if (testBarycenter == this.barycenter.barycenter)
				{
					return this.barycenter.meanAnomalyAtEpoch_Rad;
				}
				if (testBarycenter == this.barycenter.barycenter.barycenter)
				{
					return this.barycenter.barycenter.meanAnomalyAtEpoch_Rad;
				}
				if (testBarycenter == this.barycenter.barycenter.barycenter.barycenter)
				{
					return this.barycenter.barycenter.barycenter.meanAnomalyAtEpoch_Rad;
				}
				return -1.0;
			}
		}

		// Token: 0x06004512 RID: 17682 RVA: 0x001C3520 File Offset: 0x001C1720
		public double GetRelevantOrbitalVelocity_mps(TISpaceObjectState testBarycenter)
		{
			if (testBarycenter == null)
			{
				Error.Log("Body will null barycenter passed to GetRelevantSemimajorAxis", Array.Empty<object>());
				return -1.0;
			}
			if (testBarycenter == this.trueState)
			{
				if (testBarycenter.isSpaceBodyState)
				{
					return testBarycenter.ref_spaceBody.meanVelocity_mps;
				}
				return 0.0;
			}
			else
			{
				if (testBarycenter == this.barycenter)
				{
					return this.trueState.ref_spaceObject.meanVelocity_mps;
				}
				if (testBarycenter == this.barycenter.barycenter)
				{
					return this.barycenter.meanVelocity_mps;
				}
				if (testBarycenter == this.barycenter.barycenter.barycenter)
				{
					return this.barycenter.barycenter.meanVelocity_mps;
				}
				if (testBarycenter == this.barycenter.barycenter.barycenter.barycenter)
				{
					return this.barycenter.barycenter.barycenter.meanVelocity_mps;
				}
				return -1.0;
			}
		}

		// Token: 0x0400288B RID: 10379
		private TINaturalSpaceObjectState barycenter;

		// Token: 0x0400288C RID: 10380
		private double semiMajorAxis_m;
	}
}
