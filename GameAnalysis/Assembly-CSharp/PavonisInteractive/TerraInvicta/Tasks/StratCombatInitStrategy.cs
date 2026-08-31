using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000951 RID: 2385
	public class StratCombatInitStrategy : ICombatInitStrategy
	{
		// Token: 0x06005AE3 RID: 23267 RVA: 0x002BB06C File Offset: 0x002B926C
		private void ChangeStanceWeight(CombatStance stance, float value)
		{
			if (this.stanceWeights.ContainsKey(stance))
			{
				Dictionary<CombatStance, float> dictionary = this.stanceWeights;
				dictionary[stance] += value;
			}
		}

		// Token: 0x06005AE4 RID: 23268 RVA: 0x002BB0A0 File Offset: 0x002B92A0
		private bool AttemptForceStance(CombatStance stance)
		{
			if (this.stanceWeights.ContainsKey(stance))
			{
				foreach (CombatStance combatStance in this.stanceWeights.Keys.ToList<CombatStance>())
				{
					if (stance == combatStance)
					{
						this.stanceWeights[combatStance] = 1f;
					}
					else
					{
						this.stanceWeights[combatStance] = 0f;
					}
				}
				return true;
			}
			if (this.stanceWeights.ContainsKey(CombatStance.Defend))
			{
				Dictionary<CombatStance, float> dictionary = this.stanceWeights;
				dictionary[CombatStance.Defend] = dictionary[CombatStance.Defend] + 1f;
			}
			return false;
		}

		// Token: 0x06005AE5 RID: 23269 RVA: 0x002BB158 File Offset: 0x002B9358
		public CombatStance SelectStance(TIFactionState faction, TISpaceCombatState combatState, Dictionary<TINationState, PlannedFighters> fighterPlan)
		{
			List<CombatStance> list = combatState.allowedStances[faction];
			if (TemplateManager.global.debug_AINeverFleesPrecombat)
			{
				if (list.Contains(CombatStance.Pursue))
				{
					return CombatStance.Pursue;
				}
				if (list.Contains(CombatStance.Defend))
				{
					return CombatStance.Defend;
				}
			}
			else if (TemplateManager.global.debug_AIAlwaysFleesPrecombat && list.Contains(CombatStance.Evade))
			{
				return CombatStance.Evade;
			}
			if (combatState.attacker.faction == faction && combatState.attacker.dummyFleet)
			{
				if (list.Contains(CombatStance.Pursue))
				{
					return CombatStance.Pursue;
				}
				return CombatStance.Defend;
			}
			else if (faction.IsAlienFaction && combatState.fleets.Any<TISpaceFleetState>(delegate(TISpaceFleetState x)
			{
				TIGameState tigameState3 = ((x != null) ? x.ref_system : null);
				TIHabState primaryHab = faction.primaryHab;
				return tigameState3 == ((primaryHab != null) ? primaryHab.ref_system : null);
			}))
			{
				if (list.Contains(CombatStance.Pursue))
				{
					return CombatStance.Pursue;
				}
				return CombatStance.Defend;
			}
			else
			{
				if (list.Count == 1)
				{
					return list[0];
				}
				this.stanceWeights = combatState.allowedStances[faction].ToDictionary<CombatStance, CombatStance, float>((CombatStance x) => x, (CombatStance x) => 0f);
				bool flag = combatState.fleets[0].faction == faction;
				float num = 0f;
				float num2 = 0f;
				TISpaceFleetState tispaceFleetState = null;
				TISpaceFleetState tispaceFleetState2 = null;
				TIFactionState tifactionState = null;
				for (int i = 0; i < combatState.fleets.Length; i++)
				{
					if (!(combatState.fleets[i] == null))
					{
						if (combatState.fleets[i].faction == faction)
						{
							tispaceFleetState = combatState.fleets[i];
							num = tispaceFleetState.SpaceCombatValue();
						}
						else
						{
							tispaceFleetState2 = combatState.fleets[i];
							num2 = faction.GetPerceivedEnemyFleetStrength(tispaceFleetState2);
							tifactionState = tispaceFleetState2.faction;
						}
					}
				}
				if (combatState.hab != null)
				{
					if (combatState.hab.ref_faction.permanentAlly(faction))
					{
						num += combatState.hab.SpaceCombatValue();
					}
					else
					{
						num2 += combatState.hab.SpaceCombatValue();
						if (tifactionState == null)
						{
							tifactionState = combatState.hab.faction;
						}
					}
				}
				num += fighterPlan.Values.Sum<PlannedFighters>((PlannedFighters x) => (float)x.count * x.fighter.TemplateSpaceCombatValue(false, -1f, 1f, false));
				float num3 = num2;
				num2 = Mathf.Max(1f, num2);
				if (tispaceFleetState == null)
				{
					return CombatStance.Defend;
				}
				FactionGoal_Fleet factionGoal_Fleet = tispaceFleetState.AssignedGoal();
				if (num3 > 0f && tispaceFleetState.IsAlien())
				{
					TIOrbitState ref_orbit = tispaceFleetState.ref_orbit;
					if ((ref_orbit == null || ref_orbit.isEarthLEO) && (tispaceFleetState.councilorPassengers.Count > 0 || (num < num2 && factionGoal_Fleet != null && factionGoal_Fleet.GetGoalType() == GoalType.InvadeEarth)) && this.AttemptForceStance(CombatStance.Evade))
					{
						return CombatStance.Evade;
					}
				}
				float num4 = faction.AI_ModifiedRiskAversion();
				float num5 = 1.5f - num4;
				float num6 = num / num2;
				float num7;
				if (num6 > 1f)
				{
					num7 = 1f + (num6 - 1f) * num5;
				}
				else
				{
					num7 = 1f / (1f + (1f / num6 - 1f) / num5);
				}
				float num8 = ((num6 >= 1f) ? num7 : 0f);
				float num9 = num7;
				float num10 = 1f / num7;
				if (tispaceFleetState.IsAlien() && !tifactionState.milestones.Contains(CampaignMilestone.AccessAlienShip))
				{
					num10 = 0f;
				}
				else if (num3 <= 0f && num > 0f)
				{
					num10 = 0f;
				}
				if (factionGoal_Fleet != null && !factionGoal_Fleet.ShouldDiscardGoal())
				{
					float num11 = (float)factionGoal_Fleet.importance;
					bool flag2 = factionGoal_Fleet is FactionGoal_AttackWithFleet || factionGoal_Fleet is FactionGoal_CaptureHab;
					bool flag3 = factionGoal_Fleet is FactionGoal_DefendWithFleet;
					if (num >= factionGoal_Fleet.GetForcePursueFleetCombatValue(tispaceFleetState2, combatState.hab) && !tispaceFleetState.NonCombatFleet())
					{
						TIGameState tigameState = factionGoal_Fleet.location();
						TIGameState tigameState2;
						if (tigameState == null)
						{
							tigameState2 = null;
						}
						else
						{
							TINaturalSpaceObjectState ref_naturalSpaceObject = tigameState.ref_naturalSpaceObject;
							tigameState2 = ((ref_naturalSpaceObject != null) ? ref_naturalSpaceObject.GetSunOrbitingRelatedObject : null);
						}
						TISpaceGameState location = tispaceFleetState.location;
						if (tigameState2 == ((location != null) ? location.ref_naturalSpaceObject.GetSunOrbitingRelatedObject : null))
						{
							if (list.Contains(CombatStance.Pursue))
							{
								return CombatStance.Pursue;
							}
							num10 = 0f;
						}
					}
					if (flag2 || flag3)
					{
						bool flag4 = false;
						if (!flag2 && tispaceFleetState.dockedOrLanded && tispaceFleetState.dockedLocation == factionGoal_Fleet.target())
						{
							return CombatStance.Defend;
						}
						bool flag5;
						if (AIEvaluators.GetBossDefenseGoals(tispaceFleetState.faction).Contains(factionGoal_Fleet))
						{
							flag5 = true;
							flag4 = true;
						}
						else if (flag)
						{
							flag5 = true;
							bool flag6;
							if (!flag2 || num < factionGoal_Fleet.ComputeDesiredFleetCombatValue())
							{
								if (flag2 && combatState.hab != null)
								{
									if (tispaceFleetState2 == null)
									{
										goto IL_0504;
									}
									if (tispaceFleetState2.ships.Where<TISpaceShipState>((TISpaceShipState x) => !x.hull.simpleHull).Count<TISpaceShipState>() == 0)
									{
										goto IL_0504;
									}
								}
								flag6 = flag3 && num6 > 0.5f;
								goto IL_0505;
							}
							IL_0504:
							flag6 = true;
							IL_0505:
							flag4 = flag6;
						}
						else if (flag2)
						{
							if (tispaceFleetState2 != null)
							{
								flag5 = factionGoal_Fleet.target() == tispaceFleetState2 || (tispaceFleetState2.dockedOrLanded && factionGoal_Fleet.target() == tispaceFleetState2.dockedLocation);
							}
							else
							{
								flag5 = factionGoal_Fleet.target() == combatState.hab;
							}
							flag4 = num >= factionGoal_Fleet.ComputeDesiredFleetCombatValue();
						}
						else
						{
							flag5 = factionGoal_Fleet.target() == tispaceFleetState.ref_spaceBody || (factionGoal_Fleet.target().isNationState && tispaceFleetState.ref_spaceBody == GameStateManager.Earth());
							num8 = 0f;
						}
						if (flag4)
						{
							if (this.AttemptForceStance(CombatStance.Pursue))
							{
								return CombatStance.Pursue;
							}
							return CombatStance.Defend;
						}
						else if (flag5)
						{
							if (num6 >= 1f)
							{
								if (flag2 || flag)
								{
									return CombatStance.Pursue;
								}
								return CombatStance.Defend;
							}
							else
							{
								num10 /= num11 + 1f;
								num9 /= Mathf.Max(1f, num11 / 3f);
								if (flag2)
								{
									num8 = 2f * num7;
									num9 = 1f / num7;
								}
							}
						}
					}
				}
				this.ChangeStanceWeight(CombatStance.Pursue, num8);
				this.ChangeStanceWeight(CombatStance.Defend, num9);
				this.ChangeStanceWeight(CombatStance.Evade, num10);
				if (this.stanceWeights.Values.Any<float>((float x) => x > 0f))
				{
					return this.stanceWeights.SelectRandomWeightedItem<KeyValuePair<CombatStance, float>>((KeyValuePair<CombatStance, float> x) => x.Value, -1f, 1E-37f).Key;
				}
				return CombatStance.Defend;
			}
		}

		// Token: 0x06005AE6 RID: 23270 RVA: 0x002BB80C File Offset: 0x002B9A0C
		public float SelectBid_kps(TIFactionState faction, TISpaceCombatState combatState, out CombatStance extendedStance, out List<TISpaceShipState> chasers)
		{
			TISpaceFleetState tispaceFleetState = combatState.FleetFor(faction);
			TISpaceFleetState tispaceFleetState2 = combatState.FleetAgainst(faction);
			float num = combatState.MaxDVBidForPursuit_mps(tispaceFleetState, tispaceFleetState2);
			CombatStance combatStance = combatState.stances[tispaceFleetState.faction];
			CombatStance combatStance2 = combatState.stances[tispaceFleetState2.faction];
			chasers = new List<TISpaceShipState>();
			extendedStance = CombatStance.NotYetSet;
			double num3;
			if (tispaceFleetState.trajectory != null)
			{
				double num2 = tispaceFleetState.trajectory.RemainingDVatTime_mps(combatState.combatStartDateTime);
				num3 = (double)tispaceFleetState.currentDeltaV_mps - num2;
			}
			else
			{
				TINaturalSpaceObjectState barycenter;
				double num4;
				if (combatState.ref_orbit != null)
				{
					barycenter = combatState.ref_orbit.barycenter;
					num4 = combatState.ref_orbit.semiMajorAxis_m;
				}
				else
				{
					CartesianState cartesianState;
					combatState.fleeingFleet.tryToGetLocalCartesianState(combatState.combatStartDateTime, out cartesianState, out barycenter);
					num4 = cartesianState.position.magnitude;
				}
				ValueTuple<double, double, TINaturalSpaceObjectState> valueTuple = new ValueTuple<double, double, TINaturalSpaceObjectState>(double.PositiveInfinity, 0.0, barycenter);
				foreach (TIHabModuleState tihabModuleState in faction.activeHabModules)
				{
					if (!tihabModuleState.destroyed && tihabModuleState.powered && tihabModuleState.moduleTemplate.allowsResupply)
					{
						ITransferTarget ref_orbit = tihabModuleState.ref_orbit;
						if (ref_orbit != null)
						{
							TINaturalSpaceObjectState tinaturalSpaceObjectState = barycenter.FindCommonBarycenter(ref_orbit.barycenter());
							double num5;
							if (barycenter == tinaturalSpaceObjectState)
							{
								num5 = num4;
							}
							else if (barycenter.barycenter == tinaturalSpaceObjectState)
							{
								num5 = barycenter.semiMajorAxis_m;
							}
							else
							{
								num5 = barycenter.barycenter.semiMajorAxis_m;
							}
							double num6 = ref_orbit.common_a_m(tinaturalSpaceObjectState);
							if (num6 >= 0.0 && Mathd.Abs(num6 - num5) < Mathd.Abs(valueTuple.Item2 - valueTuple.Item1))
							{
								valueTuple = new ValueTuple<double, double, TINaturalSpaceObjectState>(num5, num6, tinaturalSpaceObjectState);
							}
						}
					}
				}
				if (valueTuple.Item2 == double.PositiveInfinity)
				{
					return 0f;
				}
				if (valueTuple.Item2 == valueTuple.Item1)
				{
					num3 = (double)Mathf.Max(0f, tispaceFleetState.currentDeltaV_mps - 10000f);
				}
				else
				{
					double num7 = MasterTransferPlanner.HohmannTotalDV_mps(valueTuple.Item1, valueTuple.Item2, valueTuple.Item3.mu);
					num3 = (double)Mathf.Max(0f, (float)((double)tispaceFleetState.currentDeltaV_mps - num7 * 2.0));
				}
			}
			float num8 = Mathf.Max(0f, Mathf.Min((float)num3, num));
			if (num8 == num && combatStance == CombatStance.Pursue && combatStance2 == CombatStance.Evade && TISpaceCombatState.OnTieBidDoesTheFirstFleetWin(tispaceFleetState2, tispaceFleetState))
			{
				bool flag;
				chasers = TISpaceCombatState.PursuerSubsetThatCanCatchEnemyFleet(tispaceFleetState, tispaceFleetState2, out flag);
				if (chasers.Sum<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f)) > tispaceFleetState2.SpaceCombatValue() * 1.2f)
				{
					extendedStance = (flag ? CombatStance.ExtendedPursuit_Envelop : CombatStance.ExtendedPursuit_Stretch);
				}
			}
			return num8 / 1000f;
		}

		// Token: 0x06005AE7 RID: 23271 RVA: 0x002BBB10 File Offset: 0x002B9D10
		public static Formation SelectSkirmishFormation(TISpaceFleetTemplate fleet, bool defendingHab)
		{
			List<TIFormationTemplate> list = TemplateManager.IterateByClass<TIFormationTemplate>(true).ToList<TIFormationTemplate>();
			if (defendingHab)
			{
				list = list.Where<TIFormationTemplate>((TIFormationTemplate x) => x.pos.All<Vector3>((Vector3 y) => y.z == 0f)).ToList<TIFormationTemplate>();
				if (list.Count == 0)
				{
					list = TemplateManager.IterateByClass<TIFormationTemplate>(true).ToList<TIFormationTemplate>();
				}
			}
			Formation formation = new Formation
			{
				patternDataName = list.SelectRandomItem<TIFormationTemplate>().dataName,
				concentration = FormationConcentration.Center,
				focus = FormationFocus.Battle,
				spacing = FormationSpacing.Tight
			};
			Debug.Log("AI Formation: " + formation.displayName);
			return formation;
		}

		// Token: 0x0400415F RID: 16735
		private Dictionary<CombatStance, float> stanceWeights;
	}
}
