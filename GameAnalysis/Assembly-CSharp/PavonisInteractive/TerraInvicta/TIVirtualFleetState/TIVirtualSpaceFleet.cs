using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.TIVirtualFleetState
{
	// Token: 0x02000961 RID: 2401
	public class TIVirtualSpaceFleet : IMobileAsset, ITransferTarget
	{
		// Token: 0x06005B59 RID: 23385 RVA: 0x002BEA20 File Offset: 0x002BCC20
		public TIVirtualSpaceFleet(TISpaceFleetState fleetToCopy)
			: this(fleetToCopy, null)
		{
		}

		// Token: 0x06005B5A RID: 23386 RVA: 0x002BEA2C File Offset: 0x002BCC2C
		public TIVirtualSpaceFleet(IMobileAsset fleetToCopy, TIFactionState faction = null)
		{
			this.ref_orbit = fleetToCopy.ref_orbit;
			this.cruiseAcceleration_mps2 = fleetToCopy.cruiseAcceleration_mps2;
			this.currentDeltaV_mps = fleetToCopy.currentDeltaV_mps;
			this.faction = faction ?? fleetToCopy.faction;
			this.fleetTrajectoryData = new FleetTrajectoryData
			{
				initialDeltaV_mps = (double)fleetToCopy.currentDeltaV_mps
			};
			this.transferAssigned = fleetToCopy.transferAssigned;
			this._orbit = new OrbitalElementsState(fleetToCopy);
			this._barycenter = fleetToCopy.barycenter();
		}

		// Token: 0x06005B5B RID: 23387 RVA: 0x002BEAB0 File Offset: 0x002BCCB0
		public TIVirtualSpaceFleet(TISpaceAssetState assetToStartAt, float acceleration_mps2, float deltaV, TIFactionState faction = null)
		{
			this.ref_orbit = assetToStartAt.orbitState;
			this.cruiseAcceleration_mps2 = acceleration_mps2;
			this.currentDeltaV_mps = deltaV;
			this.faction = faction ?? assetToStartAt.faction;
			this.fleetTrajectoryData = new FleetTrajectoryData
			{
				initialDeltaV_mps = (double)this.currentDeltaV_mps
			};
			this.transferAssigned = false;
			this._orbit = new OrbitalElementsState(assetToStartAt);
			this._barycenter = assetToStartAt.barycenter;
		}

		// Token: 0x06005B5C RID: 23388 RVA: 0x002BEB28 File Offset: 0x002BCD28
		public TIVirtualSpaceFleet(TIOrbitState orbitToStartAt, float acceleration_mps2, float deltaV, TIFactionState faction, TIDateTime epoch = null, double meanAnomalyAtEpoch = 0.0)
		{
			if (epoch == null)
			{
				epoch = TITimeState.Now();
			}
			this.ref_orbit = orbitToStartAt;
			this.cruiseAcceleration_mps2 = acceleration_mps2;
			this.currentDeltaV_mps = deltaV;
			this.faction = faction;
			this.fleetTrajectoryData = new FleetTrajectoryData
			{
				initialDeltaV_mps = (double)this.currentDeltaV_mps
			};
			this.transferAssigned = false;
			this._orbit = new OrbitalElementsState(orbitToStartAt, meanAnomalyAtEpoch, epoch);
			this._barycenter = orbitToStartAt.barycenter;
		}

		// Token: 0x17000F72 RID: 3954
		// (get) Token: 0x06005B5D RID: 23389 RVA: 0x002BEBA4 File Offset: 0x002BCDA4
		// (set) Token: 0x06005B5E RID: 23390 RVA: 0x002BEBAC File Offset: 0x002BCDAC
		public TIOrbitState ref_orbit { get; private set; }

		// Token: 0x17000F73 RID: 3955
		// (get) Token: 0x06005B5F RID: 23391 RVA: 0x002BEBB5 File Offset: 0x002BCDB5
		// (set) Token: 0x06005B60 RID: 23392 RVA: 0x002BEBBD File Offset: 0x002BCDBD
		public float cruiseAcceleration_mps2 { get; private set; }

		// Token: 0x17000F74 RID: 3956
		// (get) Token: 0x06005B61 RID: 23393 RVA: 0x002BEBC6 File Offset: 0x002BCDC6
		// (set) Token: 0x06005B62 RID: 23394 RVA: 0x002BEBCE File Offset: 0x002BCDCE
		public float currentDeltaV_mps { get; private set; }

		// Token: 0x06005B63 RID: 23395 RVA: 0x002BEBD7 File Offset: 0x002BCDD7
		public double meanAnomaly_Rad(TIDateTime time)
		{
			return this._orbit.MeanAnomalyAtTime_Rad(time.ExportTime(), this.barycenter().mass_kg);
		}

		// Token: 0x17000F75 RID: 3957
		// (get) Token: 0x06005B64 RID: 23396 RVA: 0x002BEBF5 File Offset: 0x002BCDF5
		// (set) Token: 0x06005B65 RID: 23397 RVA: 0x002BEBFD File Offset: 0x002BCDFD
		public TIFactionState faction { get; private set; }

		// Token: 0x17000F76 RID: 3958
		// (get) Token: 0x06005B66 RID: 23398 RVA: 0x002BEC06 File Offset: 0x002BCE06
		// (set) Token: 0x06005B67 RID: 23399 RVA: 0x002BEC0E File Offset: 0x002BCE0E
		public FleetTrajectoryData fleetTrajectoryData { get; set; }

		// Token: 0x17000F77 RID: 3959
		// (get) Token: 0x06005B68 RID: 23400 RVA: 0x002BEC17 File Offset: 0x002BCE17
		public List<TISpaceShipState> ships
		{
			get
			{
				return new List<TISpaceShipState>();
			}
		}

		// Token: 0x17000F78 RID: 3960
		// (get) Token: 0x06005B69 RID: 23401 RVA: 0x002BEC1E File Offset: 0x002BCE1E
		// (set) Token: 0x06005B6A RID: 23402 RVA: 0x002BEC26 File Offset: 0x002BCE26
		public bool transferAssigned { get; set; }

		// Token: 0x17000F79 RID: 3961
		// (get) Token: 0x06005B6B RID: 23403 RVA: 0x002BEC2F File Offset: 0x002BCE2F
		public TIDateTime epoch_DateTime
		{
			get
			{
				return new TIDateTime(this._orbit.epoch);
			}
		}

		// Token: 0x06005B6C RID: 23404 RVA: 0x002BEC41 File Offset: 0x002BCE41
		public double M0_rad()
		{
			return this._orbit.meanAnomalyAtEpoch_Rad;
		}

		// Token: 0x06005B6D RID: 23405 RVA: 0x002BEC4E File Offset: 0x002BCE4E
		public double a_m()
		{
			return this._orbit.semiMajorAxis_m;
		}

		// Token: 0x06005B6E RID: 23406 RVA: 0x002BEC5B File Offset: 0x002BCE5B
		public double e()
		{
			return this._orbit.eccentricity;
		}

		// Token: 0x06005B6F RID: 23407 RVA: 0x002BEC68 File Offset: 0x002BCE68
		public double i_rad()
		{
			return this._orbit.inclination_Rad;
		}

		// Token: 0x06005B70 RID: 23408 RVA: 0x002BEC75 File Offset: 0x002BCE75
		public double L0_rad()
		{
			return this._orbit.longAscendingNode_Rad + this._orbit.argPeriapsis_Rad + this._orbit.meanAnomalyAtEpoch_Rad;
		}

		// Token: 0x06005B71 RID: 23409 RVA: 0x002BEC9A File Offset: 0x002BCE9A
		public double t0_jy()
		{
			return new TIDateTime(this._orbit.epoch).ToJulianEpoch();
		}

		// Token: 0x06005B72 RID: 23410 RVA: 0x002BECB1 File Offset: 0x002BCEB1
		public double μ()
		{
			return this._barycenter.mu;
		}

		// Token: 0x06005B73 RID: 23411 RVA: 0x002BECBE File Offset: 0x002BCEBE
		public double Ω_rad()
		{
			return this._orbit.longAscendingNode_Rad;
		}

		// Token: 0x06005B74 RID: 23412 RVA: 0x002BECCB File Offset: 0x002BCECB
		public double ω_rad()
		{
			return this._orbit.argPeriapsis_Rad;
		}

		// Token: 0x06005B75 RID: 23413 RVA: 0x002BECD8 File Offset: 0x002BCED8
		public TINaturalSpaceObjectState barycenter()
		{
			return this._barycenter;
		}

		// Token: 0x06005B76 RID: 23414 RVA: 0x002BECE0 File Offset: 0x002BCEE0
		public TINaturalSpaceObjectState barycenterBarycenter()
		{
			return this._barycenter.barycenter;
		}

		// Token: 0x06005B77 RID: 23415 RVA: 0x002BECED File Offset: 0x002BCEED
		public TINaturalSpaceObjectState barycenterBarycenterBarycenter()
		{
			TINaturalSpaceObjectState barycenter = this._barycenter.barycenter;
			if (barycenter == null)
			{
				return null;
			}
			return barycenter.barycenter;
		}

		// Token: 0x06005B78 RID: 23416 RVA: 0x002BED05 File Offset: 0x002BCF05
		public double common_a_m(TINaturalSpaceObjectState commonBarycenter)
		{
			if (this.barycenter() == commonBarycenter)
			{
				return this.a_m();
			}
			if (this.barycenterBarycenter() == commonBarycenter)
			{
				return this.barycenter().semiMajorAxis_m;
			}
			return this.barycenterBarycenter().semiMajorAxis_m;
		}

		// Token: 0x06005B79 RID: 23417 RVA: 0x002BED41 File Offset: 0x002BCF41
		public double common_e(TINaturalSpaceObjectState commonBarycenter)
		{
			if (this.barycenter() == commonBarycenter)
			{
				return this.e();
			}
			if (this.barycenterBarycenter() == commonBarycenter)
			{
				return this.barycenter().ecc;
			}
			return this.barycenterBarycenter().ecc;
		}

		// Token: 0x06005B7A RID: 23418 RVA: 0x002BED7D File Offset: 0x002BCF7D
		public double common_i_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (this.barycenter() == commonBarycenter)
			{
				return this.i_rad();
			}
			if (this.barycenterBarycenter() == commonBarycenter)
			{
				return this.barycenter().inclination_Rad;
			}
			return this.barycenterBarycenter().inclination_Rad;
		}

		// Token: 0x06005B7B RID: 23419 RVA: 0x002BEDB9 File Offset: 0x002BCFB9
		public double common_L0_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (this.barycenter() == commonBarycenter)
			{
				return this.L0_rad();
			}
			if (this.barycenterBarycenter() == commonBarycenter)
			{
				return this.barycenter().meanLongitude_Rad;
			}
			return this.barycenterBarycenter().meanLongitude_Rad;
		}

		// Token: 0x06005B7C RID: 23420 RVA: 0x002BEDF5 File Offset: 0x002BCFF5
		public double common_M0_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (this.barycenter() == commonBarycenter)
			{
				return this.M0_rad();
			}
			if (this.barycenterBarycenter() == commonBarycenter)
			{
				return this.barycenter().meanAnomalyAtEpoch_Rad;
			}
			return this.barycenterBarycenter().meanAnomalyAtEpoch_Rad;
		}

		// Token: 0x06005B7D RID: 23421 RVA: 0x002BEE34 File Offset: 0x002BD034
		public double common_M_rad(TINaturalSpaceObjectState commonBarycenter, TIDateTime time)
		{
			if (this.barycenter() == commonBarycenter)
			{
				return this._orbit.MeanAnomalyAtTime_Rad(time.ExportTime(), this.barycenter().mass_kg);
			}
			if (this.barycenterBarycenter() == commonBarycenter)
			{
				return this.barycenter().meanAnomaly_Rad(time);
			}
			return this.barycenterBarycenter().meanAnomaly_Rad(time);
		}

		// Token: 0x06005B7E RID: 23422 RVA: 0x002BEE94 File Offset: 0x002BD094
		public double common_period_days(TINaturalSpaceObjectState commonBarycenter)
		{
			if (this.barycenter() == commonBarycenter)
			{
				return this.period_days();
			}
			if (this.barycenterBarycenter() == commonBarycenter)
			{
				return this.barycenter().orbitalPeriod_s / 86400.0;
			}
			return this.barycenterBarycenter().orbitalPeriod_s / 86400.0;
		}

		// Token: 0x06005B7F RID: 23423 RVA: 0x002BEEEF File Offset: 0x002BD0EF
		public Vector3d GetGlobalPositionAtTime(TIDateTime time)
		{
			return this.ToGlobalCartesianStateAtTime(time).position;
		}

		// Token: 0x06005B80 RID: 23424 RVA: 0x002BEEFD File Offset: 0x002BD0FD
		public void getOrbitalElementsState(TIDateTime time, out OrbitalElementsState orbitalElementsState, out TINaturalSpaceObjectState barycenter, out bool meanAnomalyIsGood)
		{
			orbitalElementsState = this._orbit;
			barycenter = this.barycenter();
			meanAnomalyIsGood = true;
		}

		// Token: 0x06005B81 RID: 23425 RVA: 0x002BEF17 File Offset: 0x002BD117
		public Vector3d globalPositionValue(TISpaceFleetState forFleet, TIDateTime time)
		{
			return this.barycenter().GetGlobalPositionAtTime(time);
		}

		// Token: 0x06005B82 RID: 23426 RVA: 0x002BEF25 File Offset: 0x002BD125
		public TINaturalSpaceObjectState localBarycenter(TIDateTime time)
		{
			return this.barycenter();
		}

		// Token: 0x06005B83 RID: 23427 RVA: 0x002BEF2D File Offset: 0x002BD12D
		public double period_days()
		{
			return this._orbit.OrbitalPeriod(this.barycenter().mass_kg) / 86400.0;
		}

		// Token: 0x06005B84 RID: 23428 RVA: 0x002BEF4F File Offset: 0x002BD14F
		public CartesianState relevantGlobalCartesianState(TINaturalSpaceObjectState commonBarycenter, TIDateTime time)
		{
			if (this.barycenter() == commonBarycenter)
			{
				return this.ToGlobalCartesianStateAtTime(time);
			}
			if (this.barycenterBarycenter() == commonBarycenter)
			{
				return this.barycenter().ToGlobalCartesianStateAtTime(time);
			}
			return this.barycenterBarycenter().ToGlobalCartesianStateAtTime(time);
		}

		// Token: 0x06005B85 RID: 23429 RVA: 0x002BEF8E File Offset: 0x002BD18E
		public double relevant_escapeVelocity_mps(TINaturalSpaceObjectState commonBarycenter)
		{
			return commonBarycenter.localEscapeVelocity_mps(this.common_a_m(commonBarycenter));
		}

		// Token: 0x06005B86 RID: 23430 RVA: 0x002BEF9D File Offset: 0x002BD19D
		public double relevant_orbit_m(TINaturalSpaceObjectState commonBarycenter)
		{
			return this.common_a_m(commonBarycenter);
		}

		// Token: 0x06005B87 RID: 23431 RVA: 0x002BEFA6 File Offset: 0x002BD1A6
		public TIGameState selfState()
		{
			Debug.LogError("TIVirtualSpaceFleet.selfState() was called.  Virtual fleets are not TIGameStates");
			return null;
		}

		// Token: 0x06005B88 RID: 23432 RVA: 0x002BEFB3 File Offset: 0x002BD1B3
		public void SetAccelerationPhaseStatus(bool inPhase, bool forceRotation = false, bool forceStop = false)
		{
		}

		// Token: 0x06005B89 RID: 23433 RVA: 0x002BEFB5 File Offset: 0x002BD1B5
		public void SetDecelerationPhaseStatus(bool inPhase, bool forceRotation = false, bool forceStop = false)
		{
		}

		// Token: 0x06005B8A RID: 23434 RVA: 0x002BEFB8 File Offset: 0x002BD1B8
		public CartesianState ToGlobalCartesianStateAtTime(TIDateTime time)
		{
			CartesianState cartesianState = this._orbit.ToCartesianStateAtTime(time.ExportTime(), this.barycenter().mass_kg);
			if (this.barycenter() != null)
			{
				if (this.barycenter().isSpaceBodyState)
				{
					cartesianState = (this.barycenter().SpatialRotation * cartesianState.xzy).xzy;
					cartesianState += this.barycenter().ToGlobalCartesianStateAtTime(time);
				}
				else
				{
					Vector3d vector3d = this.barycenter().ref_lagrangePoint.GetGlobalPositionAtTime(time) + cartesianState.position;
					Vector3d velocity = cartesianState.velocity;
					cartesianState = new CartesianState(vector3d, velocity);
				}
			}
			return cartesianState;
		}

		// Token: 0x06005B8B RID: 23435 RVA: 0x002BF060 File Offset: 0x002BD260
		public CartesianState? tryToGetGlobalCartesianState(TIDateTime time)
		{
			return new CartesianState?(this.ToGlobalCartesianStateAtTime(time));
		}

		// Token: 0x06005B8C RID: 23436 RVA: 0x002BF06E File Offset: 0x002BD26E
		public bool tryToGetLocalCartesianState(TIDateTime time, out CartesianState cartesianState, out TINaturalSpaceObjectState barycenter)
		{
			barycenter = this.barycenter();
			cartesianState = this._orbit.ToCartesianStateAtTime(time.ExportTime(), barycenter.mass_kg);
			return true;
		}

		// Token: 0x06005B8D RID: 23437 RVA: 0x002BF098 File Offset: 0x002BD298
		public Vector3 visualizationPositionValue()
		{
			Debug.LogWarning("TIVirtualSpaceFleet.visualizationPositionValue() was called.  Virtual fleets are never visualized.");
			return default(Vector3);
		}

		// Token: 0x06005B8E RID: 23438 RVA: 0x002BF0B8 File Offset: 0x002BD2B8
		public double common_Ω_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (this.barycenter() == commonBarycenter)
			{
				return this.Ω_rad();
			}
			if (this.barycenterBarycenter() == commonBarycenter)
			{
				return this.barycenter().longAscendingNode_Rad;
			}
			return this.barycenterBarycenter().longAscendingNode_Rad;
		}

		// Token: 0x06005B8F RID: 23439 RVA: 0x002BF0F4 File Offset: 0x002BD2F4
		public double common_ω_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (this.barycenter() == commonBarycenter)
			{
				return this.ω_rad();
			}
			if (this.barycenterBarycenter() == commonBarycenter)
			{
				return this.barycenter().argPeriapsis_Rad;
			}
			return this.barycenterBarycenter().argPeriapsis_Rad;
		}

		// Token: 0x06005B90 RID: 23440 RVA: 0x002BF130 File Offset: 0x002BD330
		public double common_t0_jy(TINaturalSpaceObjectState commonBarycenter)
		{
			if (this.barycenter() == commonBarycenter)
			{
				return this.t0_jy();
			}
			if (this.barycenterBarycenter() == commonBarycenter)
			{
				return this.barycenter().epoch_JYears;
			}
			return this.barycenterBarycenter().epoch_JYears;
		}

		// Token: 0x06005B91 RID: 23441 RVA: 0x002BF16C File Offset: 0x002BD36C
		public double common_μ(TINaturalSpaceObjectState commonBarycenter)
		{
			return commonBarycenter.mu;
		}

		// Token: 0x06005B92 RID: 23442 RVA: 0x002BF174 File Offset: 0x002BD374
		public TINaturalSpaceObjectState FindCommonBarycenter(TIGameState secondSpaceObject)
		{
			new GenericSpaceObject().AssignData(secondSpaceObject);
			TISpaceObjectState tispaceObjectState = secondSpaceObject as TISpaceObjectState;
			if (tispaceObjectState == null)
			{
				return null;
			}
			if (this.barycenter() == tispaceObjectState || this.barycenter() == tispaceObjectState.barycenter || this.barycenter() == tispaceObjectState.barycenter.barycenter)
			{
				return this.barycenter();
			}
			if (this.barycenterBarycenter() == tispaceObjectState || this.barycenterBarycenter() == tispaceObjectState.barycenter || this.barycenterBarycenter() == tispaceObjectState.barycenter.barycenter)
			{
				return this.barycenterBarycenter();
			}
			return this.barycenterBarycenterBarycenter();
		}

		// Token: 0x04004196 RID: 16790
		private OrbitalElementsState _orbit;

		// Token: 0x04004197 RID: 16791
		private TINaturalSpaceObjectState _barycenter;
	}
}
