using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200078C RID: 1932
	public class EmergencyBurnPlanner
	{
		// Token: 0x06003D85 RID: 15749 RVA: 0x00181FCC File Offset: 0x001801CC
		public static EmergencyBurnPlanner.EmergencyBurnSolution Solve(TISpaceFleetState fleet, Trajectory_Patched doomedTrajectory)
		{
			EmergencyBurnPlanner.EmergencyBurnSolution emergencyBurnSolution = new EmergencyBurnPlanner.EmergencyBurnSolution
			{
				abandonedShips = new List<TISpaceShipState>(fleet.ships),
				rescueTrajectory = null,
				outcome = 5
			};
			fleet.ships.Max<TISpaceShipState>((TISpaceShipState x) => x.cruiseAcceleration_mps2);
			TIDateTime launchTime = doomedTrajectory.launchTime;
			List<EmergencyBurnPlanner.EmergencyBurnSolution> list = new List<EmergencyBurnPlanner.EmergencyBurnSolution>();
			if (doomedTrajectory.endsInCrash)
			{
				Trajectory_Patched.IPatchSegment patchSegment = doomedTrajectory.Segments.Last<Trajectory_Patched.IPatchSegment>();
				OrbitalElementsState orbitalElementsState = patchSegment.OrbitalElementsAtTime(patchSegment.startTime);
				TISpaceBodyState collisionTarget = doomedTrajectory.collisionTarget;
				if (collisionTarget != patchSegment.barycenter)
				{
					string[] array = new string[7];
					array[0] = "EmergencyBurnPlanner.Solve: ";
					array[1] = fleet.displayName;
					array[2] = "'s involuntary trajectory's terminal barycenter (";
					array[3] = patchSegment.barycenter.displayName;
					array[4] = ") did not match its collision target (";
					int num = 5;
					TISpaceBodyState tispaceBodyState = collisionTarget;
					array[num] = ((tispaceBodyState != null) ? tispaceBodyState.ToString() : null);
					array[6] = ").";
					Log.Error(string.Concat(array), Array.Empty<object>());
					return emergencyBurnSolution;
				}
				TIDateTime startTime = patchSegment.startTime;
				double periapsis_m = orbitalElementsState.periapsis_m;
				double num2 = collisionTarget.meanRadius_m + 10000.0;
				if (periapsis_m > num2)
				{
					Log.Error(string.Concat(new string[]
					{
						"EmergencyBurnPlanner.Solve: ",
						fleet.displayName,
						" claims that it will crash but its involuntary trajectory's periapsis is ",
						(periapsis_m - collisionTarget.meanRadius_m).ToString(),
						"m above the surface of ",
						collisionTarget.displayName,
						"."
					}), Array.Empty<object>());
					return emergencyBurnSolution;
				}
				TIDateTime tidateTime = new TIDateTime(startTime);
				double mass_kg = collisionTarget.mass_kg;
				DateTime dateTime = orbitalElementsState.NextTimeAtMeanAnomaly(0.0, startTime.ExportTime(), mass_kg);
				Vector3d vector3d = orbitalElementsState.ToCartesianStateAtTime(dateTime, mass_kg).position.normalized * num2;
				int num3 = 10;
				double num4 = new TIDateTime(dateTime).DifferenceInSeconds(startTime) / (double)(num3 + 1);
				for (int i = 0; i < num3; i++)
				{
					CartesianState cartesianState = orbitalElementsState.ToCartesianStateAtTime(tidateTime.ExportTime(), mass_kg);
					ValueTuple<double, double> conicSectionGivenPeriapsisAndAnotherPoint = EmergencyBurnPlanner.GetConicSectionGivenPeriapsisAndAnotherPoint(vector3d, cartesianState.position);
					OrbitalElementsState orbitalElementsState2 = new OrbitalElementsState(orbitalElementsState);
					orbitalElementsState2.eccentricity = conicSectionGivenPeriapsisAndAnotherPoint.Item1;
					orbitalElementsState2.semiMajorAxis_m = conicSectionGivenPeriapsisAndAnotherPoint.Item2;
					ValueTuple<double, double>? meanAnomalyWhenAtRadius = orbitalElementsState2.GetMeanAnomalyWhenAtRadius(cartesianState.position.magnitude, collisionTarget);
					double num5 = Mathd.Min((meanAnomalyWhenAtRadius != null) ? meanAnomalyWhenAtRadius.GetValueOrDefault().Item1 : 0.0, (meanAnomalyWhenAtRadius != null) ? meanAnomalyWhenAtRadius.GetValueOrDefault().Item2 : 0.0);
					orbitalElementsState2.meanAnomalyAtEpoch_Rad = num5;
					orbitalElementsState2.epoch = tidateTime.ExportTime();
					double magnitude = (orbitalElementsState2.ToCartesianStateAtTime(tidateTime.ExportTime(), mass_kg).velocity - cartesianState.velocity).magnitude;
					double burnDV_kps = magnitude / 1000.0;
					TIDateTime tidateTime2 = new TIDateTime(orbitalElementsState2.NextTimeAtMeanAnomaly(0.0, tidateTime.ExportTime(), mass_kg));
					double num6 = 2.0 * Mathd.Min(tidateTime.DifferenceInSeconds(TITimeState.Now()), tidateTime2.DifferenceInSeconds(tidateTime));
					double minAccelerationForBurn_mps2 = magnitude / num6;
					IEnumerable<TISpaceShipState> enumerable = fleet.ships.Where<TISpaceShipState>((TISpaceShipState ship) => (double)ship.currentDeltaV_kps < burnDV_kps || (double)ship.cruiseAcceleration_mps2 < minAccelerationForBurn_mps2);
					if (enumerable.Count<TISpaceShipState>() < fleet.ships.Count)
					{
						double num7;
						if (enumerable.Count<TISpaceShipState>() != 0)
						{
							num7 = (double)fleet.ships.Except<TISpaceShipState>(enumerable).Min<TISpaceShipState>((TISpaceShipState x) => x.cruiseAcceleration_mps2);
						}
						else
						{
							num7 = (double)fleet.cruiseAcceleration_mps2;
						}
						double num8 = num7;
						Trajectory_Patched trajectory_Patched = new Trajectory_Patched();
						bool flag = trajectory_Patched.BuildCoastTrajectory(fleet, doomedTrajectory, new TIDateTime(tidateTime), orbitalElementsState2, collisionTarget);
						Trajectory_Patched trajectory_Patched2 = new Trajectory_Patched();
						double num9 = magnitude / num8;
						bool flag2;
						if (flag)
						{
							if (trajectory_Patched.endsInCrash)
							{
								Debug.LogError(string.Concat(new string[]
								{
									"EmergencyBurnPlanner.Solve: our solution still crashes.  Was crashing into ",
									collisionTarget.displayName,
									", and is now crashing into ",
									trajectory_Patched.collisionTarget.displayName,
									"."
								}));
								return emergencyBurnSolution;
							}
							flag2 = trajectory_Patched2.BuildInterimTrajectory(fleet, doomedTrajectory, trajectory_Patched, launchTime, tidateTime, num9, orbitalElementsState, collisionTarget, num8);
						}
						else
						{
							flag2 = trajectory_Patched2.BuildInterimTrajectory(fleet, doomedTrajectory, orbitalElementsState2, launchTime, tidateTime, num9, orbitalElementsState, collisionTarget, num8);
						}
						if (!flag2)
						{
							Debug.LogError("EmergencyBurnPlanner.Solve: could not calculate interim trajectory.");
						}
						else
						{
							EmergencyBurnPlanner.EmergencyBurnSolution emergencyBurnSolution2 = new EmergencyBurnPlanner.EmergencyBurnSolution
							{
								abandonedShips = enumerable.ToList<TISpaceShipState>(),
								rescueTrajectory = trajectory_Patched2,
								outcome = ((enumerable.Count<TISpaceShipState>() == 0) ? 1 : 6)
							};
							bool flag3;
							if (!(trajectory_Patched2.destinationOrbit != null))
							{
								Trajectory nextTrajectory = trajectory_Patched2.nextTrajectory;
								flag3 = nextTrajectory != null && !nextTrajectory.destroyOnArrival;
							}
							else
							{
								flag3 = true;
							}
							bool flag4 = flag3;
							if (enumerable.Count<TISpaceShipState>() == 0 && flag4)
							{
								return emergencyBurnSolution2;
							}
							list.Add(emergencyBurnSolution2);
						}
					}
					IEnumerable<TISpaceShipState> enumerable2 = fleet.ships.Where<TISpaceShipState>((TISpaceShipState ship) => (double)ship.currentDeltaV_kps > burnDV_kps);
					if (enumerable2.Count<TISpaceShipState>() == 0)
					{
						return emergencyBurnSolution;
					}
					double num10 = (double)enumerable2.Min<TISpaceShipState>((TISpaceShipState ship) => ship.cruiseAcceleration_mps2);
					double num11 = Mathd.Min(magnitude / num10 * 0.6, num4);
					tidateTime.AddSeconds(num11);
				}
			}
			else
			{
				list.Add(new EmergencyBurnPlanner.EmergencyBurnSolution
				{
					abandonedShips = new List<TISpaceShipState>(),
					rescueTrajectory = doomedTrajectory,
					outcome = 2
				});
			}
			emergencyBurnSolution.outcome = 2;
			foreach (EmergencyBurnPlanner.EmergencyBurnSolution emergencyBurnSolution3 in list.Where<EmergencyBurnPlanner.EmergencyBurnSolution>(delegate(EmergencyBurnPlanner.EmergencyBurnSolution s)
			{
				if (!s.rescueTrajectory.exitsSolarSystem)
				{
					Trajectory nextTrajectory2 = s.rescueTrajectory.nextTrajectory;
					return nextTrajectory2 != null && nextTrajectory2.exitsSolarSystem;
				}
				return true;
			}))
			{
				IEnumerable<TISpaceShipState> enumerable3 = fleet.ships.Where<TISpaceShipState>((TISpaceShipState ship) => ship.cruiseAcceleration_mps2 > 0f && ship.currentDeltaV_kps > 0f);
				bool flag5 = !emergencyBurnSolution3.rescueTrajectory.involuntary;
				if (!flag5)
				{
					emergencyBurnSolution.outcome = 2;
					OrbitalElementsState orbitalElementsAtTime = emergencyBurnSolution3.rescueTrajectory.GetOrbitalElementsAtTime(launchTime);
					TINaturalSpaceObjectState barycenterAtTime = emergencyBurnSolution3.rescueTrajectory.GetBarycenterAtTime(launchTime);
					TIDateTime tidateTime3 = new TIDateTime(orbitalElementsAtTime.NextTimeAtMeanAnomaly(0.0, launchTime.ExportTime(), barycenterAtTime.mass_kg));
					if (tidateTime3 > launchTime && tidateTime3.DifferenceInSeconds(launchTime) < 432000.0)
					{
						CartesianState cartesianState2 = doomedTrajectory.ToGlobalCartesianStateAtTime(tidateTime3);
						TINaturalSpaceObjectState barycenterAtTime2 = doomedTrajectory.GetBarycenterAtTime(tidateTime3);
						CartesianState cartesianState3 = cartesianState2.ToLocal(barycenterAtTime2, tidateTime3);
						double num12 = EmergencyBurnPlanner.LocalMaximumSafeSpeed(cartesianState3.position, barycenterAtTime2, tidateTime3);
						double num13 = cartesianState3.velocity.magnitude - num12;
						double burn_kps2 = num13 / 1000.0;
						enumerable3 = enumerable3.Where<TISpaceShipState>((TISpaceShipState ship) => (double)ship.currentDeltaV_kps > burn_kps2);
						if (enumerable3.Count<TISpaceShipState>() == 0)
						{
							continue;
						}
						double num14 = (double)enumerable3.Min<TISpaceShipState>((TISpaceShipState ship) => ship.cruiseAcceleration_mps2);
						double num15 = num13 / num14;
						if (tidateTime3.DifferenceInSeconds(launchTime) >= num15 / 2.0)
						{
							cartesianState3.velocity = num12 * cartesianState3.velocity.normalized;
							OrbitalElementsState orbitalElementsState3 = cartesianState3.ToOrbitalElementsState(barycenterAtTime2.mu, new DateTime?(tidateTime3.ExportTime()));
							Trajectory_Patched trajectory_Patched3 = new Trajectory_Patched();
							if (!trajectory_Patched3.BuildCoastTrajectory(fleet, doomedTrajectory, tidateTime3, orbitalElementsState3, barycenterAtTime2))
							{
								Trajectory_Patched trajectory_Patched4 = new Trajectory_Patched();
								OrbitalElementsState orbitalElementsAtTime2 = doomedTrajectory.GetOrbitalElementsAtTime(tidateTime3);
								if (!trajectory_Patched4.BuildInterimTrajectory(fleet, doomedTrajectory, orbitalElementsState3, launchTime, tidateTime3, num15, orbitalElementsAtTime2, barycenterAtTime2, (double)fleet.cruiseAcceleration_mps2))
								{
									Debug.LogError("EmergencyBurnPlanner.Solve: Failed to calculate a trajectory to a known burn at periapsis which leads directly to a stable orbit.");
									continue;
								}
								emergencyBurnSolution3.rescueTrajectory = trajectory_Patched4;
								emergencyBurnSolution3.outcome = ((enumerable3.Count<TISpaceShipState>() == fleet.ships.Count) ? 1 : 3);
								continue;
							}
							else
							{
								if (trajectory_Patched3.destroyOnArrival)
								{
									Debug.LogError("EmergencyBurnPlanner.Solve(): will not collide & attempting burn at periapsis to avoid solar exit: generated 'safe' coast trajectory either crashes or leaves solar system.");
									continue;
								}
								Trajectory_Patched trajectory_Patched5 = new Trajectory_Patched();
								double num16 = num15;
								OrbitalElementsState orbitalElementsAtTime3 = doomedTrajectory.GetOrbitalElementsAtTime(tidateTime3);
								if (!trajectory_Patched5.BuildInterimTrajectory(fleet, doomedTrajectory, trajectory_Patched3, launchTime, tidateTime3, num16, orbitalElementsAtTime3, barycenterAtTime2, (double)fleet.cruiseAcceleration_mps2))
								{
									Debug.LogError("EmergencyBurnPlanner.Solve(): will not collide & attempting burn at periapsis to avoid solar exit: failed to build an interim trajectory despite valid coast trajectory and periapsis burn.");
									continue;
								}
								emergencyBurnSolution3.rescueTrajectory = trajectory_Patched5;
								bool flag6 = enumerable3.Count<TISpaceShipState>() == fleet.ships.Count;
								emergencyBurnSolution3.outcome = (flag6 ? 1 : 3);
								if (flag6)
								{
									emergencyBurnSolution3.outcome = 1;
									continue;
								}
								emergencyBurnSolution3.outcome = 3;
								emergencyBurnSolution3.abandonedShips = fleet.ships.Except<TISpaceShipState>(enumerable3).ToList<TISpaceShipState>();
								continue;
							}
						}
					}
				}
				if (enumerable3.Count<TISpaceShipState>() == 0)
				{
					return emergencyBurnSolution;
				}
				Trajectory_Patched trajectory_Patched6;
				TIDateTime tidateTime4;
				TIDateTime tidateTime5;
				if (emergencyBurnSolution3.rescueTrajectory.nextTrajectory == null)
				{
					trajectory_Patched6 = emergencyBurnSolution3.rescueTrajectory;
					tidateTime4 = launchTime;
					tidateTime5 = launchTime;
				}
				else
				{
					trajectory_Patched6 = (Trajectory_Patched)emergencyBurnSolution3.rescueTrajectory.nextTrajectory;
					tidateTime4 = emergencyBurnSolution3.rescueTrajectory.nextTrajectory.launchTime;
					tidateTime5 = tidateTime4;
				}
				TIDateTime tidateTime6 = new TIDateTime(tidateTime4);
				bool flag7 = false;
				int j = 0;
				while (j < 10)
				{
					TINaturalSpaceObjectState barycenterAtTime3 = trajectory_Patched6.GetBarycenterAtTime(tidateTime6);
					CartesianState cartesianState4 = trajectory_Patched6.ToGlobalCartesianStateAtTime(tidateTime6).ToLocal(barycenterAtTime3, tidateTime6);
					double num17 = EmergencyBurnPlanner.LocalMaximumSafeSpeed(cartesianState4.position, barycenterAtTime3, tidateTime6);
					double num18 = (double)enumerable3.Min<TISpaceShipState>((TISpaceShipState ship) => ship.cruiseAcceleration_mps2);
					double num19 = cartesianState4.velocity.magnitude - num17;
					double num20 = num19 / num18;
					if (tidateTime6.DifferenceInSeconds(tidateTime5) < num20 / 2.0)
					{
						tidateTime6 = new TIDateTime(tidateTime4, num20 * 0.6);
						j++;
					}
					else
					{
						double burn_kps3 = num19 / 1000.0;
						enumerable3 = enumerable3.Where<TISpaceShipState>((TISpaceShipState ship) => (double)ship.currentDeltaV_kps >= burn_kps3);
						if (enumerable3.Count<TISpaceShipState>() == 0)
						{
							break;
						}
						cartesianState4.velocity = num17 * cartesianState4.velocity.normalized;
						OrbitalElementsState orbitalElementsState4 = cartesianState4.ToOrbitalElementsState(barycenterAtTime3.mu, new DateTime?(tidateTime6.ExportTime()));
						Trajectory_Patched trajectory_Patched7 = new Trajectory_Patched();
						Trajectory_Patched trajectory_Patched8 = new Trajectory_Patched();
						OrbitalElementsState orbitalElementsAtTime4 = trajectory_Patched6.GetOrbitalElementsAtTime(tidateTime6);
						new TIDateTime(tidateTime6, -num20 / 2.0);
						bool flag8 = trajectory_Patched7.BuildCoastTrajectory(fleet, trajectory_Patched6, tidateTime6, orbitalElementsState4, barycenterAtTime3);
						if (trajectory_Patched7 != null && trajectory_Patched7.destroyOnArrival)
						{
							break;
						}
						bool flag9;
						if (flag8)
						{
							flag9 = trajectory_Patched8.BuildInterimTrajectory(fleet, emergencyBurnSolution3.rescueTrajectory, trajectory_Patched7, launchTime, tidateTime6, num20, orbitalElementsAtTime4, barycenterAtTime3, (double)fleet.cruiseAcceleration_mps2);
						}
						else
						{
							flag9 = trajectory_Patched8.BuildInterimTrajectory(fleet, emergencyBurnSolution3.rescueTrajectory, orbitalElementsState4, launchTime, tidateTime6, num20, orbitalElementsAtTime4, barycenterAtTime3, (double)fleet.cruiseAcceleration_mps2);
						}
						if (flag9)
						{
							if (enumerable3.Count<TISpaceShipState>() == fleet.ships.Count)
							{
								emergencyBurnSolution3.outcome = 1;
							}
							else
							{
								bool flag10 = !emergencyBurnSolution3.rescueTrajectory.involuntary;
								emergencyBurnSolution3.outcome = (flag10 ? 6 : 3);
								emergencyBurnSolution3.abandonedShips = fleet.ships.Except<TISpaceShipState>(enumerable3).ToList<TISpaceShipState>();
							}
							emergencyBurnSolution3.rescueTrajectory = trajectory_Patched8;
							flag7 = true;
							break;
						}
						break;
					}
				}
				if (!flag7)
				{
					tidateTime6 = new TIDateTime(tidateTime4);
					int k = 0;
					while (k < 10)
					{
						if (enumerable3.Count<TISpaceShipState>() == 0)
						{
							break;
						}
						TINaturalSpaceObjectState barycenterAtTime4 = trajectory_Patched6.GetBarycenterAtTime(tidateTime4);
						if (!barycenterAtTime4.isSun)
						{
							break;
						}
						CartesianState cartesianState5 = trajectory_Patched6.ToGlobalCartesianStateAtTime(tidateTime6);
						OrbitalElementsState orbitalElementsAtTime5 = trajectory_Patched6.GetOrbitalElementsAtTime(tidateTime6);
						OrbitalElementsState maxEccentricitySolarAbortOrbit = EmergencyBurnPlanner.GetMaxEccentricitySolarAbortOrbit(orbitalElementsAtTime5, cartesianState5, tidateTime6, barycenterAtTime4);
						double magnitude2 = (maxEccentricitySolarAbortOrbit.ToCartesianStateAtTime(tidateTime6.ExportTime(), barycenterAtTime4.mass_kg).velocity - cartesianState5.velocity).magnitude;
						double num21 = (double)enumerable3.Min<TISpaceShipState>((TISpaceShipState ship) => ship.cruiseAcceleration_mps2);
						double num22 = magnitude2 / num21;
						if (tidateTime6.DifferenceInSeconds(tidateTime4) * 2.0 < num22)
						{
							tidateTime6.AddSeconds(num22 * 0.6);
							k++;
						}
						else
						{
							double burn_kps = magnitude2 / 1000.0;
							enumerable3 = enumerable3.Where<TISpaceShipState>((TISpaceShipState ship) => (double)ship.currentDeltaV_kps >= burn_kps);
							if (enumerable3.Count<TISpaceShipState>() == 0)
							{
								break;
							}
							new TIDateTime(tidateTime6, -num22 / 2.0);
							Trajectory_Patched trajectory_Patched9 = new Trajectory_Patched();
							if (!trajectory_Patched9.BuildInterimTrajectory(fleet, emergencyBurnSolution3.rescueTrajectory, maxEccentricitySolarAbortOrbit, launchTime, tidateTime6, num22, orbitalElementsAtTime5, barycenterAtTime4, (double)fleet.cruiseAcceleration_mps2))
							{
								Debug.LogWarning("EmergencyBurnPlanner.Solve(): attempting early burn to avoid solar impact or exit: failed to build an interim trajectory despite valid coast trajectory.");
								break;
							}
							emergencyBurnSolution3.rescueTrajectory = trajectory_Patched9;
							if (enumerable3.Count<TISpaceShipState>() == fleet.ships.Count)
							{
								emergencyBurnSolution3.outcome = 1;
							}
							else
							{
								emergencyBurnSolution3.outcome = (flag5 ? 6 : 3);
								emergencyBurnSolution3.abandonedShips = fleet.ships.Except<TISpaceShipState>(enumerable3).ToList<TISpaceShipState>();
							}
							flag7 = true;
							break;
						}
					}
				}
				if (!flag7)
				{
					emergencyBurnSolution3.abandonedShips = fleet.ships;
					bool flag11 = !emergencyBurnSolution3.rescueTrajectory.involuntary;
					emergencyBurnSolution3.outcome = (flag11 ? 5 : 2);
					emergencyBurnSolution3.rescueTrajectory = doomedTrajectory;
				}
			}
			list.ForEach(delegate(EmergencyBurnPlanner.EmergencyBurnSolution x)
			{
				x.rescueTrajectory.involuntary = true;
			});
			if (list.Count == 1)
			{
				return list[0];
			}
			if (list.Count == 0)
			{
				return emergencyBurnSolution;
			}
			ValueTuple<EmergencyBurnPlanner.EmergencyBurnSolution, double> valueTuple = new ValueTuple<EmergencyBurnPlanner.EmergencyBurnSolution, double>(list[0], EmergencyBurnPlanner.ScoreSolution(list[0], fleet));
			for (int l = 1; l < list.Count; l++)
			{
				double num23 = EmergencyBurnPlanner.ScoreSolution(list[l], fleet);
				if (num23 > valueTuple.Item2)
				{
					valueTuple = new ValueTuple<EmergencyBurnPlanner.EmergencyBurnSolution, double>(list[l], num23);
				}
			}
			OrbitalElementsState orbitalElementsState5;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			bool flag12;
			fleet.getOrbitalElementsState(TITimeState.Now(), out orbitalElementsState5, out tinaturalSpaceObjectState, out flag12);
			orbitalElementsState5.MeanAnomalyAtTime_Rad(TITimeState.Now().ExportTime(), tinaturalSpaceObjectState.mass_kg);
			return valueTuple.Item1;
		}

		// Token: 0x06003D86 RID: 15750 RVA: 0x00182E54 File Offset: 0x00181054
		private static double LocalMaximumSafeSpeed(Vector3d startPosition, TINaturalSpaceObjectState barycenter, TIDateTime time)
		{
			if (barycenter.isSun)
			{
				double magnitude = startPosition.magnitude;
				return barycenter.localEscapeVelocity_mps(magnitude) * Mathd.Sqrt(9000000000000.0 / (magnitude + 9000000000000.0));
			}
			ValueTuple<TINaturalSpaceObjectState, TINaturalSpaceObjectState, TINaturalSpaceObjectState> barycenters = EmergencyBurnPlanner.GetBarycenters(barycenter);
			double magnitude2 = barycenters.Item2.GetGlobalPositionAtTime(time).magnitude;
			double num = barycenters.Item3.localEscapeVelocity_mps(magnitude2) * Mathd.Sqrt(9000000000000.0 / (magnitude2 + 9000000000000.0));
			CartesianState cartesianState = barycenters.Item2.ToGlobalCartesianStateAtTime(time);
			double num2 = num - cartesianState.velocity.magnitude;
			TINaturalSpaceObjectState item = barycenters.Item2;
			double num3;
			if (!(barycenters.Item1 == null))
			{
				cartesianState = barycenters.Item1.ToLocalCartesianStateAtTime(time);
				num3 = cartesianState.position.magnitude;
			}
			else
			{
				num3 = startPosition.magnitude;
			}
			double num4 = item.localEscapeVelocity_mps(num3);
			double num5 = Mathd.Sqrt(num4 * num4 + num2 * num2);
			if (barycenters.Item1 == null)
			{
				return num5;
			}
			double num6 = num5;
			cartesianState = barycenters.Item1.ToLocalCartesianStateAtTime(time);
			double num7 = num6 - cartesianState.velocity.magnitude;
			double num8 = barycenters.Item1.localEscapeVelocity_mps(startPosition.magnitude);
			return Mathd.Sqrt(num8 * num8 + num7 * num7);
		}

		// Token: 0x06003D87 RID: 15751 RVA: 0x00182F8C File Offset: 0x0018118C
		[return: TupleElementNames(new string[] { "moon", "planet", "sun" })]
		private static ValueTuple<TINaturalSpaceObjectState, TINaturalSpaceObjectState, TINaturalSpaceObjectState> GetBarycenters(TINaturalSpaceObjectState barycenter)
		{
			if (barycenter.isSun)
			{
				return new ValueTuple<TINaturalSpaceObjectState, TINaturalSpaceObjectState, TINaturalSpaceObjectState>(null, null, barycenter);
			}
			if (barycenter.barycenter.isSun)
			{
				return new ValueTuple<TINaturalSpaceObjectState, TINaturalSpaceObjectState, TINaturalSpaceObjectState>(null, barycenter, barycenter.barycenter);
			}
			return new ValueTuple<TINaturalSpaceObjectState, TINaturalSpaceObjectState, TINaturalSpaceObjectState>(barycenter, barycenter.barycenter, barycenter.barycenter.barycenter);
		}

		// Token: 0x06003D88 RID: 15752 RVA: 0x00182FDC File Offset: 0x001811DC
		private static TIDateTime TryToGetNextTimeAtPeriapsis(Trajectory_Patched trajectory, TIDateTime now)
		{
			foreach (Trajectory_Patched.IPatchSegment patchSegment in trajectory.Segments)
			{
				if (patchSegment is Trajectory_Patched.OrbitLERPSegment || patchSegment is Trajectory_Patched.OrbitSegment || patchSegment is Trajectory_Patched.HyperbolicOrbitSegment)
				{
					OrbitalElementsState orbitalElementsState = patchSegment.OrbitalElementsAtTime(patchSegment.startTime);
					TINaturalSpaceObjectState barycenter = patchSegment.barycenter;
					TIDateTime tidateTime = new TIDateTime(orbitalElementsState.NextTimeAtMeanAnomaly(0.0, now.ExportTime(), barycenter.mass_kg));
					if (tidateTime > now)
					{
						return tidateTime;
					}
				}
			}
			return null;
		}

		// Token: 0x06003D89 RID: 15753 RVA: 0x0018308C File Offset: 0x0018128C
		private static double ScoreSolution(EmergencyBurnPlanner.EmergencyBurnSolution solution, TISpaceFleetState fleet)
		{
			IEnumerable<TISpaceShipState> enumerable = fleet.ships.Except<TISpaceShipState>(solution.abandonedShips);
			double DVconsumed_kps = solution.rescueTrajectory.DV_kps;
			return enumerable.Sum<TISpaceShipState>((TISpaceShipState ship) => (double)((1f + ship.SpaceCombatValue(false, 0f)) * ship.currentMaxDeltaV_kps) - DVconsumed_kps);
		}

		// Token: 0x06003D8A RID: 15754 RVA: 0x001830D4 File Offset: 0x001812D4
		[return: TupleElementNames(new string[] { "eccentricity", "semiMajorAxis_m" })]
		private static ValueTuple<double, double> GetConicSectionGivenPeriapsisAndAnotherPoint(Vector3d periapsis, Vector3d otherPoint)
		{
			double magnitude = periapsis.magnitude;
			double magnitude2 = otherPoint.magnitude;
			double magnitude3 = periapsis.magnitude;
			Vector3d normalized = periapsis.normalized;
			double num = magnitude3 - Vector3d.Dot(in otherPoint, in normalized);
			double num2 = (magnitude2 - magnitude) / num;
			double num3 = magnitude / num2;
			double num4 = num2 * num3 / (1.0 - num2);
			if ((num2 < 1.0) ^ (num4 > 0.0))
			{
				Debug.LogError(string.Concat(new string[]
				{
					"EmergencyBurnPlanner.GetConicSectionGivenPeriapsisAndAnotherPoint: calculated solution is contradictory (neither an ellipse nor a hyperbola).\neccentricity    = ",
					num2.ToString(),
					"\nsemi major axis = ",
					num4.ToString(),
					"m\nperiapsis       = ",
					periapsis.ToString(),
					"\nother point     = ",
					otherPoint.ToString()
				}));
				return new ValueTuple<double, double>(num2, -num4);
			}
			return new ValueTuple<double, double>(num2, num4);
		}

		// Token: 0x06003D8B RID: 15755 RVA: 0x001831B8 File Offset: 0x001813B8
		private static OrbitalElementsState GetMaxEccentricitySolarAbortOrbit(OrbitalElementsState originalSolarOrbit, CartesianState originalCartesian, TIDateTime time, TINaturalSpaceObjectState sun)
		{
			OrbitalElementsState orbitalElementsState = new OrbitalElementsState(originalSolarOrbit);
			orbitalElementsState.semiMajorAxis_m = 4500700000000.0;
			orbitalElementsState.eccentricity = 0.9996889372764237;
			double magnitude = originalCartesian.position.magnitude;
			bool flag = Vector3d.Dot(in originalCartesian.position, in originalCartesian.velocity) < 0.0;
			orbitalElementsState.epoch = time.ExportTime();
			ValueTuple<double, double>? meanAnomalyWhenAtRadius = orbitalElementsState.GetMeanAnomalyWhenAtRadius(magnitude, sun);
			orbitalElementsState.meanAnomalyAtEpoch_Rad = (flag ? ((meanAnomalyWhenAtRadius != null) ? new double?(meanAnomalyWhenAtRadius.GetValueOrDefault().Item2) : null) : ((meanAnomalyWhenAtRadius != null) ? new double?(meanAnomalyWhenAtRadius.GetValueOrDefault().Item1) : null)) ?? ((originalCartesian.position.magnitude < orbitalElementsState.semiMajorAxis_m) ? 0.0 : 3.141592653589793);
			orbitalElementsState.argPeriapsis_Rad = 0.0;
			Vector3d position = orbitalElementsState.ToCartesianStateAtTime(time.ExportTime(), sun.mass_kg).position;
			double num = Vector3d.Angle(in originalCartesian.position, in position) * 0.017453292519943295;
			Vector3d vector3d = Vector3d.Cross(originalCartesian.position, position);
			Vector3d normalVector = orbitalElementsState.normalVector;
			if (Vector3d.Dot(in vector3d, in normalVector) < 0.0)
			{
				orbitalElementsState.argPeriapsis_Rad = num;
			}
			else
			{
				orbitalElementsState.argPeriapsis_Rad = -num;
			}
			return orbitalElementsState;
		}

		// Token: 0x04002694 RID: 9876
		public const double MIN_ALTITUDE_m = 10000.0;

		// Token: 0x02000EBB RID: 3771
		public class EmergencyBurnSolution
		{
			// Token: 0x04005A1D RID: 23069
			public List<TISpaceShipState> abandonedShips;

			// Token: 0x04005A1E RID: 23070
			public Trajectory_Patched rescueTrajectory;

			// Token: 0x04005A1F RID: 23071
			public int outcome;
		}
	}
}
