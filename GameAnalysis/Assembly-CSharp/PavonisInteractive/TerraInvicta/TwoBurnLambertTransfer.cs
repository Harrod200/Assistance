using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007D3 RID: 2003
	public class TwoBurnLambertTransfer : ImpulseTransfer
	{
		// Token: 0x17000E2A RID: 3626
		// (get) Token: 0x060047F9 RID: 18425 RVA: 0x001DC599 File Offset: 0x001DA799
		// (set) Token: 0x060047FA RID: 18426 RVA: 0x001DC5A1 File Offset: 0x001DA7A1
		public Vector3d deltaV0 { get; private set; }

		// Token: 0x17000E2B RID: 3627
		// (get) Token: 0x060047FB RID: 18427 RVA: 0x001DC5AA File Offset: 0x001DA7AA
		// (set) Token: 0x060047FC RID: 18428 RVA: 0x001DC5B2 File Offset: 0x001DA7B2
		public Vector3d deltaV1 { get; private set; }

		// Token: 0x060047FD RID: 18429 RVA: 0x001DC5BC File Offset: 0x001DA7BC
		public TransferResult Solve(TIDateTime launchTime, TIDateTime arrivalTime, double transitDuration_s, ITransferTarget iOrigin, ITransferTarget iDestination, TINaturalSpaceObjectState transferBarycenter, double fleetAcceleration_mps2)
		{
			CartesianState cartesianState = iDestination.relevantGlobalCartesianState(transferBarycenter, arrivalTime).ToLocal(transferBarycenter, arrivalTime);
			CartesianState cartesianState2 = iOrigin.relevantGlobalCartesianState(transferBarycenter, launchTime).ToLocal(transferBarycenter, launchTime);
			return this.SolveCartesian(launchTime, arrivalTime, transitDuration_s, cartesianState2, cartesianState, transferBarycenter, fleetAcceleration_mps2);
		}

		// Token: 0x060047FE RID: 18430 RVA: 0x001DC604 File Offset: 0x001DA804
		public TransferResult Solve(TIDateTime launchTime, TIDateTime arrivalTime, double transitDuration_s, ITransferTarget iOrigin, TIOrbitState iDestination, double destinationMeanAnomaly_Rad, TINaturalSpaceObjectState transferBarycenter, double fleetAcceleration_mps2)
		{
			CartesianState cartesianState = iDestination.relevantCartesianState(transferBarycenter, arrivalTime, destinationMeanAnomaly_Rad).ToLocal(transferBarycenter, arrivalTime);
			CartesianState cartesianState2 = iOrigin.relevantGlobalCartesianState(transferBarycenter, launchTime).ToLocal(transferBarycenter, launchTime);
			return this.SolveCartesian(launchTime, arrivalTime, transitDuration_s, cartesianState2, cartesianState, transferBarycenter, fleetAcceleration_mps2);
		}

		// Token: 0x060047FF RID: 18431 RVA: 0x001DC650 File Offset: 0x001DA850
		public TransferResult SolveCartesian(TIDateTime launchTime, TIDateTime arrivalTime, double transitDuration_s, CartesianState sourceLocalToDestination, CartesianState destinationLocalToDestination, TINaturalSpaceObjectState transferBarycenter, double fleetAcceleration_mps2)
		{
			if (transitDuration_s <= 0.0)
			{
				return new TransferResult(TransferResult.Outcome.Fail_ArrivalBeforeLaunch, transitDuration_s, 0.0);
			}
			this.launchTime = new TIDateTime(launchTime);
			this.transitDuration_s = transitDuration_s;
			this.arrivalTime = new TIDateTime(arrivalTime);
			Vector3d position = sourceLocalToDestination.position;
			Vector3d velocity = sourceLocalToDestination.velocity;
			sourceLocalToDestination = new CartesianState(position, velocity);
			CartesianState cartesianState = sourceLocalToDestination;
			this.s0 = new CartesianState(cartesianState.position, cartesianState.velocity);
			Vector3d position2 = destinationLocalToDestination.position;
			Vector3d velocity2 = destinationLocalToDestination.velocity;
			destinationLocalToDestination = new CartesianState(position2, velocity2);
			CartesianState cartesianState2 = destinationLocalToDestination;
			this.s1 = new CartesianState(cartesianState2.position, cartesianState2.velocity);
			LambertEquations lambertEquations = default(LambertEquations);
			double num = lambertEquations.SolveLambert(transitDuration_s, cartesianState, cartesianState2, transferBarycenter.mu, false, false);
			LambertEquations lambertEquations2 = default(LambertEquations);
			double num2 = lambertEquations2.SolveLambert(transitDuration_s, cartesianState, cartesianState2, transferBarycenter.mu, true, false);
			if (num < num2)
			{
				this.deltaV0 = lambertEquations.burn0;
				this.deltaV1 = lambertEquations.burn1;
				this.s0.velocity = lambertEquations.initialVelocity;
				this.s1.velocity = lambertEquations.finalVelocity;
			}
			else
			{
				this.deltaV0 = lambertEquations2.burn0;
				this.deltaV1 = lambertEquations2.burn1;
				this.s0.velocity = lambertEquations2.initialVelocity;
				this.s1.velocity = lambertEquations2.finalVelocity;
			}
			this._transferOrbit = this.s0.ToOrbitalElementsState(transferBarycenter.mu, new DateTime?(launchTime.ExportTime()));
			if (double.IsNaN(this._transferOrbit.meanAnomalyAtEpoch_Rad) || double.IsInfinity(this._transferOrbit.meanAnomalyAtEpoch_Rad))
			{
				Log.Warn(string.Concat(new string[]
				{
					"CartesianState.ToOrbitalElementsState failed to produce a valid orbit.  Likely it's a radial trajectory (probably crashing).\neccentricity = ",
					this._transferOrbit.eccentricity.ToString(),
					"\nsemi major axis = ",
					this._transferOrbit.semiMajorAxis_m.ToString(),
					"m"
				}), Array.Empty<object>());
				return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
			}
			DateTime dateTime = launchTime.ExportTime();
			DateTime dateTime2 = arrivalTime.ExportTime();
			if (Mathd.Approximately(this._transferOrbit.eccentricity, 1.0))
			{
				return new TransferResult(TransferResult.Outcome.Fail_Parabolic, this._transferOrbit.eccentricity, 0.0);
			}
			ValueTuple<double, double>? valueTuple = this._transferOrbit.GetMeanAnomalyWhenAtRadius(transferBarycenter.meanRadius_m, transferBarycenter);
			if (valueTuple != null)
			{
				DateTime dateTime3 = this._transferOrbit.NextTimeAtMeanAnomaly((valueTuple != null) ? valueTuple.GetValueOrDefault().Item1 : 0.0, dateTime, transferBarycenter.mass_kg);
				DateTime dateTime4 = this._transferOrbit.NextTimeAtMeanAnomaly((valueTuple != null) ? valueTuple.GetValueOrDefault().Item2 : 0.0, dateTime, transferBarycenter.mass_kg);
				if ((dateTime < dateTime3 && dateTime3 < dateTime2) || (dateTime < dateTime4 && dateTime4 < dateTime2))
				{
					double periapsis_m = this._transferOrbit.periapsis_m;
					if (num < num2)
					{
						this.deltaV0 = lambertEquations2.burn0;
						this.deltaV1 = lambertEquations2.burn1;
						this.s0.velocity = lambertEquations2.initialVelocity;
						this.s1.velocity = lambertEquations2.finalVelocity;
					}
					else
					{
						this.deltaV0 = lambertEquations.burn0;
						this.deltaV1 = lambertEquations.burn1;
						this.s0.velocity = lambertEquations.initialVelocity;
						this.s1.velocity = lambertEquations.finalVelocity;
					}
					this._transferOrbit = this.s0.ToOrbitalElementsState(transferBarycenter.mu, new DateTime?(dateTime));
					if (Mathd.Approximately(this._transferOrbit.eccentricity, 1.0))
					{
						return new TransferResult(TransferResult.Outcome.Fail_Parabolic, this._transferOrbit.eccentricity, 0.0);
					}
					valueTuple = this._transferOrbit.GetMeanAnomalyWhenAtRadius(transferBarycenter.meanRadius_m, transferBarycenter);
					if (valueTuple != null)
					{
						dateTime3 = this._transferOrbit.NextTimeAtMeanAnomaly((valueTuple != null) ? valueTuple.GetValueOrDefault().Item1 : 0.0, dateTime, transferBarycenter.mass_kg);
						dateTime4 = this._transferOrbit.NextTimeAtMeanAnomaly((valueTuple != null) ? valueTuple.GetValueOrDefault().Item2 : 0.0, dateTime, transferBarycenter.mass_kg);
						if ((dateTime < dateTime3 && dateTime3 < dateTime2) || (dateTime < dateTime4 && dateTime4 < dateTime2))
						{
							return new TransferResult(TransferResult.Outcome.Fail_WouldCollideWithBody, Mathd.Max(periapsis_m, this._transferOrbit.periapsis_m), transferBarycenter.meanRadius_m);
						}
					}
				}
			}
			double num3 = this._transferOrbit.OrbitalPeriod(transferBarycenter.mass_kg);
			if (this._transferOrbit.eccentricity < 1.0 && num3 / 7500.0 > 31556924.0)
			{
				return new TransferResult(TransferResult.Outcome.Fail_OrbitPeriod, num3, this._transferOrbit.eccentricity);
			}
			base.boost_DV_mps = this.deltaV0.magnitude;
			base.decel_DV_mps = this.deltaV1.magnitude;
			base.DV_mps = base.boost_DV_mps + base.decel_DV_mps;
			double num4 = base.boost_DV_mps / fleetAcceleration_mps2;
			double num5 = base.decel_DV_mps / fleetAcceleration_mps2;
			if (arrivalTime.DifferenceInSeconds(launchTime) * 2.0 < num4 + num5)
			{
				return new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanTransfer, (num4 + num5) / 2.0, arrivalTime.DifferenceInSeconds(launchTime));
			}
			if (double.IsNaN(num5) || double.IsNaN(num4))
			{
				return new TransferResult(TransferResult.Outcome.Fail_BurnNaN, 0.0, 0.0);
			}
			this.launchTime.AddSeconds(-0.5 * num4);
			this.arrivalTime.AddSeconds(0.5 * num5);
			return new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
		}

		// Token: 0x06004800 RID: 18432 RVA: 0x001DCC90 File Offset: 0x001DAE90
		public TransferResult ModifyDV(double additionalBoostDV_mps, double additionalDecelDV_mps, double fleetAcceleration_mps2)
		{
			base.boost_DV_mps += additionalBoostDV_mps;
			base.decel_DV_mps += additionalDecelDV_mps;
			base.DV_mps = base.boost_DV_mps + base.decel_DV_mps;
			double num = base.boost_DV_mps / fleetAcceleration_mps2;
			double num2 = base.decel_DV_mps / fleetAcceleration_mps2;
			if (this.arrivalTime.DifferenceInSeconds(this.launchTime) * 2.0 >= num + num2)
			{
				return new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
			}
			return new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanTransfer, (num + num2) / 2.0, this.arrivalTime.DifferenceInSeconds(this.launchTime));
		}

		// Token: 0x040029A9 RID: 10665
		private CartesianState s0;

		// Token: 0x040029AA RID: 10666
		private CartesianState s1;
	}
}
