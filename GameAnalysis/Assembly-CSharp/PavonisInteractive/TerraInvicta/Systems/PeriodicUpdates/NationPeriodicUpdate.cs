using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.GamePlayScript.Systems;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Tasks;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.PeriodicUpdates
{
	// Token: 0x020009A7 RID: 2471
	[UpdateInGroup(typeof(PipelineStages.SimulationStage))]
	public class NationPeriodicUpdate : StrategyLayerComponentSystem
	{
		// Token: 0x06005D25 RID: 23845 RVA: 0x002C6838 File Offset: 0x002C4A38
		public override void Initialize()
		{
			this.dailyCondition1 = GameTimeCondition.Daily1030(this.gameTime.Now);
			this.dailyCondition2 = GameTimeCondition.Daily1200(this.gameTime.Now);
			this.monthlyCondition = GameTimeCondition.Monthly(this.gameTime.Now);
			this.midMonthlyCondition = GameTimeCondition.MidMonthly(this.gameTime.Now);
		}

		// Token: 0x06005D26 RID: 23846 RVA: 0x002C68A0 File Offset: 0x002C4AA0
		protected override void OnUpdate()
		{
			if (this.dailyCondition1.Satisfied(this.gameTime.Now))
			{
				this.OnDailyUpdate();
			}
			if (this.dailyCondition2.Satisfied(this.gameTime.Now))
			{
				this.OnDailyUpdate2();
			}
			if (this.monthlyCondition.Satisfied(this.gameTime.Now))
			{
				this.OnMonthlyUpdate();
				return;
			}
			if (this.midMonthlyCondition.Satisfied(this.gameTime.Now))
			{
				this.OnMidMonthlyUpdate();
			}
		}

		// Token: 0x06005D27 RID: 23847 RVA: 0x002C6928 File Offset: 0x002C4B28
		private void OnDailyUpdate()
		{
			foreach (TINationState tinationState in GameStateManager.AllNations().ToList<TINationState>().Shuffle<TINationState>())
			{
				tinationState.DailyNationUpdate();
			}
		}

		// Token: 0x06005D28 RID: 23848 RVA: 0x002C6984 File Offset: 0x002C4B84
		private void OnDailyUpdate2()
		{
			for (int i = 0; i < this.nations.Length; i++)
			{
				this.DailyNationUpdateTask(this.nations.Nation[i].State);
			}
			int num = this.gameTime.Now.Day % 14;
			for (int j = 0; j < this.nations.Length; j++)
			{
				if (this.nations.Nation[j].State.extant && num == (int)this.nations.Nation[j].ID % 14)
				{
					this.PeriodicNationUpdateTask(this.nations.Nation[j].State);
				}
			}
		}

		// Token: 0x06005D29 RID: 23849 RVA: 0x002C6A5C File Offset: 0x002C4C5C
		private void OnMonthlyUpdate()
		{
			float num = GameStateManager.AlienFaction().AlienHabSurveillanceStrength();
			for (int i = 0; i < this.nations.Length; i++)
			{
				this.nations.Nation[i].State.MonthlyNationUpdate(num);
			}
			if ((this.gameTime.currentTime.month - 1) % 3 == 0)
			{
				GameStateManager.Time().AddQuarterToCampaign();
				this.OnQuarterlyUpdate();
			}
			this.GetEarthNightLightShaderDriverComponent().UpdateRegionLightValues();
		}

		// Token: 0x06005D2A RID: 23850 RVA: 0x002C6ADA File Offset: 0x002C4CDA
		private void OnMidMonthlyUpdate()
		{
			this.GetEarthNightLightShaderDriverComponent().UpdateRegionLightValues();
		}

		// Token: 0x06005D2B RID: 23851 RVA: 0x002C6AE8 File Offset: 0x002C4CE8
		protected void OnQuarterlyUpdate()
		{
			for (int i = 0; i < this.nations.Length; i++)
			{
				this.nations.Nation[i].State.QuarterlyNationUpdate();
			}
		}

		// Token: 0x06005D2C RID: 23852 RVA: 0x002C6B2C File Offset: 0x002C4D2C
		private void PeriodicNationUpdateTask(TINationState nation)
		{
			foreach (TIControlPoint ticontrolPoint in nation.NativeControlPoints)
			{
				if (nation.atWar)
				{
					GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Military_BuildArmy, 3, false));
					GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Military_BuildNavy, 3, false));
					GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Military_BuildNuclearWeapons, (nation.NumNuclearWeaponsDefendingMe() == 0 && nation.numControlPoints >= 5) ? 2 : 0, false));
					GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Military_InitiateNuclearProgram, (nation.NumNuclearWeaponsDefendingMe() == 0 && nation.numControlPoints >= 5) ? 2 : 0, false));
					GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Military_BuildSpaceDefenses, (nation.NumNuclearWeaponsThreateningMeInWars() > 0) ? 3 : 0, false));
					GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Oppression, nation.civilWar ? 3 : 0, true));
					if (nation.wars.Distinct<TINationState>().Sum<TINationState>((TINationState x) => x.numStandardArmies) < nation.NumArmiesDefendingMe())
					{
						if (!nation.regions.Any<TIRegionState>((TIRegionState x) => x.OccupiedOrOccupationUnderway()))
						{
							continue;
						}
					}
					GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Knowledge, 0, false));
					GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Government, 0, false));
					GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Environment, 0, false));
					GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Funding, 0, false));
					GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Civilian_InitiateSpaceflightProgram, 0, false));
					GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.MissionControl, 0, false));
					GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.LaunchFacilities, 0, false));
				}
				else
				{
					nation.ApplyInvestmentTemplateToControlPoint(ticontrolPoint.positionInNation, nation.template.initialPriorityPreset[ticontrolPoint.positionInNation]);
					if (!nation.civilWar && nation.democracy > 3.5f && nation.democracy < 7f && nation.cohesionWarning)
					{
						GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Government, 3, false));
					}
					if (nation.severeInequalityWarning)
					{
						GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Welfare, 1 + (int)nation.democracy / 4, true));
					}
					else if (nation.inequalityWarning)
					{
						GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Welfare, (int)nation.democracy / 4, true));
					}
					if (nation.cohesionWarning || nation.unrestWarning)
					{
						GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Unity, (10 - (int)nation.democracy) / 4, true));
					}
					if (nation.civilWar)
					{
						GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Oppression, 3, false));
					}
					else if ((nation.unrestWarning && nation.democracy < 5f) || nation.unrestMajorWarning)
					{
						GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Oppression, (int)Mathf.Clamp((nation.unrest - nation.democracy) / 2f, 1f, 3f), true));
					}
				}
			}
		}

		// Token: 0x06005D2D RID: 23853 RVA: 0x002C6E1C File Offset: 0x002C501C
		private void LaunchDeployArmyOperation(TIArmyState army, TIGameState destination)
		{
			this.LaunchArmyOperation(army, new DeployArmyOperation_OpenTarget(false), destination);
		}

		// Token: 0x06005D2E RID: 23854 RVA: 0x002C6E2C File Offset: 0x002C502C
		private void LaunchArmyOperation(TIArmyState army, IOperation operation, TIGameState target)
		{
			GameControl.StartSimulationAction(new NationAI_ArmyOperation(army, target, operation));
		}

		// Token: 0x06005D2F RID: 23855 RVA: 0x002C6E3C File Offset: 0x002C503C
		private void DailyNationUpdateTask(TINationState nation)
		{
			if (nation.extant)
			{
				nation.cohesionRestState_dailyCache = nation.cohesionRestState;
				nation.unrestRestState_dailyCache = nation.unrestRestState;
				if (nation.executiveFaction == null && this.gameTime.currentTime.day % 3 == 0 && nation.wars.Count > 0)
				{
					AIDailyFactionPlanner.ConsiderNuclearAttack(nation, null, false);
				}
				IEnumerable<TIArmyState> enumerable = nation.armies.Where<TIArmyState>((TIArmyState x) => x.faction == null);
				if (enumerable.Any<TIArmyState>())
				{
					bool flag = nation.wars.Count > 0 || nation.MegaFaunaArmiesWeShouldAttack().Any<TIArmyState>();
					bool flag2 = flag;
					if (flag)
					{
						foreach (TINationState tinationState in nation.wars)
						{
							if (!nation.WinningWarAgainst(tinationState))
							{
								flag2 = false;
								break;
							}
						}
					}
					foreach (TIArmyState tiarmyState in enumerable)
					{
						bool flag3 = tiarmyState.CurrentOperations().Count > 0;
						if (flag3 && tiarmyState.IsMoving && !tiarmyState.LegalRegion(tiarmyState.CurrentOperations()[0].target as TIRegionState))
						{
							this.LaunchArmyOperation(tiarmyState, new CancelArmyOperation(), tiarmyState);
						}
						else
						{
							bool flag4 = tiarmyState.InBattleWithArmies();
							bool flag5 = tiarmyState.strength < 0.65f;
							bool flag6 = tiarmyState.CanHeal();
							if (flag)
							{
								bool isMoving = tiarmyState.IsMoving;
								bool flag7 = tiarmyState.currentRegion == tiarmyState.homeNation.capital;
								bool flag8 = tiarmyState.InEnemyCapital();
								bool flag9 = nation.capital.OccupiedOrOccupationUnderway();
								if (flag9 && !flag8 && !flag7 && !isMoving)
								{
									TIRegionState armyDestination = TIArmyState.GetArmyDestination(tiarmyState, AIArmyDestination.MyCapital, 4);
									if (armyDestination != null)
									{
										if (armyDestination != tiarmyState.currentRegion)
										{
											if (flag3)
											{
												this.LaunchArmyOperation(tiarmyState, new CancelArmyOperation(), tiarmyState);
											}
											this.LaunchDeployArmyOperation(tiarmyState, armyDestination);
											continue;
										}
										continue;
									}
								}
								bool flag10 = tiarmyState.OccupyingRegion(true);
								flag4 = flag4 || flag10;
								TINationState tinationState2;
								float highestWarAllianceOccupationValueByNation = tiarmyState.currentRegion.GetHighestWarAllianceOccupationValueByNation(tiarmyState.homeNation, out tinationState2);
								bool flag11 = tiarmyState.strength < 0.5f;
								TIRegionState tiregionState = ((tiarmyState.IsMoving && tiarmyState.CurrentOperations().Count > 0) ? (tiarmyState.CurrentOperations()[0].target as TIRegionState) : null);
								TIRegionState finalDestination = tiarmyState.finalDestination;
								if ((!tiarmyState.IsMoving || (!TIArmyState.RegionMeetsDestinationCriteria(tiarmyState, tiregionState, AIArmyDestination.NearestSafeRegion) && !TIArmyState.RegionMeetsDestinationCriteria(tiarmyState, finalDestination, AIArmyDestination.NearestSafeRegion))) && !flag6 && !flag7 && ((flag5 && !flag4) || (flag11 && (!flag10 || tiarmyState.strength < 1f - highestWarAllianceOccupationValueByNation))))
								{
									TIRegionState armyDestination2 = TIArmyState.GetArmyDestination(tiarmyState, AIArmyDestination.NearestSafeRegion, 4);
									if (armyDestination2 != null)
									{
										if (armyDestination2 != tiarmyState.currentRegion && armyDestination2 != tiregionState && armyDestination2 != finalDestination)
										{
											if (flag3)
											{
												this.LaunchArmyOperation(tiarmyState, new CancelArmyOperation(), tiarmyState);
											}
											this.LaunchDeployArmyOperation(tiarmyState, armyDestination2);
											continue;
										}
										continue;
									}
								}
								if (tiarmyState.homeNation.unrest >= 7f)
								{
									TIRegionState armyDestination3 = TIArmyState.GetArmyDestination(tiarmyState, AIArmyDestination.NearestHomeNationRegion, 4);
									if (armyDestination3 != null && armyDestination3 != tiarmyState.currentRegion)
									{
										this.LaunchDeployArmyOperation(tiarmyState, armyDestination3);
										continue;
									}
								}
								bool flag12 = flag2 && !flag9;
								if (!flag4 && !flag3)
								{
									int num = nation.ArmiesThreateningCapital(false, false);
									if (num > 0)
									{
										if (num >= (from x in tiarmyState.homeNation.capital.FilteredArmiesPresent(true, false, false, false, true)
											where x.CurrentOperations().Count == 0
											select x).Count<TIArmyState>())
										{
											TIRegionState armyDestination4 = TIArmyState.GetArmyDestination(tiarmyState, AIArmyDestination.MyCapital, 4);
											if (armyDestination4 != null)
											{
												if (armyDestination4 != tiarmyState.currentRegion)
												{
													this.LaunchDeployArmyOperation(tiarmyState, armyDestination4);
													continue;
												}
												continue;
											}
										}
									}
									if (!flag6)
									{
										bool flag13;
										if (tiarmyState.homeNation.numStandardArmies > 1)
										{
											flag13 = tiarmyState.controlPointIdx == tiarmyState.homeNation.armies.Min<TIArmyState>((TIArmyState x) => x.controlPointIdx);
										}
										else
										{
											flag13 = false;
										}
										bool flag14 = flag13;
										if (flag12 && !flag14)
										{
											TIRegionState armyDestination5 = TIArmyState.GetArmyDestination(tiarmyState, AIArmyDestination.NearestEnemyRegion, 4);
											if (armyDestination5 != null && armyDestination5 != tiarmyState.currentRegion)
											{
												this.LaunchDeployArmyOperation(tiarmyState, armyDestination5);
											}
										}
										else
										{
											TIRegionState armyDestination6 = TIArmyState.GetArmyDestination(tiarmyState, AIArmyDestination.NearestDefensiveBattle, 4);
											if (armyDestination6 != null && armyDestination6 != tiarmyState.currentRegion)
											{
												this.LaunchDeployArmyOperation(tiarmyState, armyDestination6);
											}
										}
									}
								}
							}
							else if ((flag4 || (flag5 && !flag6) || (int)tiarmyState.ID % 4 == TITimeState.CampaignDuration_days() % 4) && !flag3)
							{
								TIRegionState armyDestination7 = TIArmyState.GetArmyDestination(tiarmyState, AIArmyDestination.NearestDefensiveBattle, 0);
								if (armyDestination7 != null)
								{
									if (armyDestination7 != tiarmyState.currentRegion)
									{
										this.LaunchDeployArmyOperation(tiarmyState, armyDestination7);
									}
								}
								else
								{
									TIRegionState armyDestination8 = TIArmyState.GetArmyDestination(tiarmyState, AIArmyDestination.NearestAlienXenoformingThreat, 0);
									if (armyDestination8 != null)
									{
										if (armyDestination8 != tiarmyState.currentRegion)
										{
											this.LaunchDeployArmyOperation(tiarmyState, armyDestination8);
										}
										else
										{
											this.LaunchArmyOperation(tiarmyState, new AssaultAlienAssetOperation(), armyDestination8.xenoforming);
										}
									}
									else
									{
										if (nation.BaseInvestmentPoints_month() >= 2f + TemplateManager.global.nationalInvestmentArmyFactorAway && nation.rivals.Count == 0 && nation.unrest >= TINationState.minUnrestForSecession - 2f)
										{
											TIRegionState armyDestination9 = TIArmyState.GetArmyDestination(tiarmyState, AIArmyDestination.NearestPotentialBreakaway, 0);
											if (armyDestination9 != null)
											{
												if (armyDestination9 != tiarmyState.currentRegion)
												{
													this.LaunchDeployArmyOperation(tiarmyState, armyDestination9);
													continue;
												}
												continue;
											}
										}
										if (tiarmyState.template != null && tiarmyState.template.startRegion != null && tiarmyState.currentRegion != tiarmyState.template.startRegion)
										{
											TIRegionState armyDestination10 = TIArmyState.GetArmyDestination(tiarmyState, AIArmyDestination.MyHome, 4);
											if (armyDestination10 != null && armyDestination10 != tiarmyState.currentRegion)
											{
												this.LaunchDeployArmyOperation(tiarmyState, armyDestination10);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			if (nation.alienNation)
			{
				bool flag15 = nation.extant;
				foreach (TIArmyState tiarmyState2 in GameStateManager.AlienFaction().armies)
				{
					if (!nation.armies.Contains(tiarmyState2))
					{
						nation.AddArmy(tiarmyState2);
					}
					if (tiarmyState2.AlienRegularArmy && tiarmyState2.currentNation != nation && !nation.wars.Contains(tiarmyState2.currentNation) && tiarmyState2.CurrentOperations().Count == 0 && (!nation.extant || (!nation.allies.Contains(tiarmyState2.currentNation) && nation.WarCapableAllies.Intersect<TINationState>(tiarmyState2.currentNation.WarCapableAllies).Count<TINationState>() == 0)))
					{
						nation.EndAlliance(GameStateManager.AlienFaction(), tiarmyState2.currentNation);
						nation.DeclareFullWar(GameStateManager.AlienFaction(), tiarmyState2.currentNation);
						TINotificationQueueState.LogPolicyAdopted(PolicyManager.policies[PolicyType.WarOption] as TIPolicyOption, nation, tiarmyState2.currentNation, null, 1, "", "");
						if (!flag15)
						{
							nation.SetCapital(tiarmyState2.currentRegion);
							flag15 = true;
						}
					}
				}
				if (nation.extant)
				{
					return;
				}
				if (GameStateManager.AlienFaction().armies.Count<TIArmyState>((TIArmyState x) => x.AlienRegularArmy) != 0)
				{
					return;
				}
				if (!GameStateManager.IterateByClass<TIRegionUFOLandingState>(false).None<TIRegionUFOLandingState>((TIRegionUFOLandingState x) => x.Extant()))
				{
					return;
				}
				using (List<TIWarState>.Enumerator enumerator4 = nation.currentWarStates.ToList<TIWarState>().GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						TIWarState tiwarState = enumerator4.Current;
						nation.WhitePeace(GameStateManager.AlienFaction(), tiwarState, false);
					}
					return;
				}
			}
			if (nation.NumOwnedControlPoints == 0 && nation.breakaway && nation.wars.Count == 0 && nation.armies.Count > nation.breakawayParent.NumArmiesDefendingMe() && (nation.breakawayParent.numNuclearWeapons == 0 || nation.numNuclearWeapons > nation.breakawayParent.numNuclearWeapons) && (double)TIUtilities.RandomFloatValue() < 1E-05)
			{
				IPolicyOption policyOption = PolicyManager.policies[PolicyType.DeclareIndependenceOption];
				GameControl.StartSimulationAction(new NationAI_AdoptPolicy(nation, nation.breakawayParent, new DeclareIndependenceOption()));
			}
		}

		// Token: 0x06005D30 RID: 23856 RVA: 0x002C77D4 File Offset: 0x002C59D4
		private EarthNightLightShaderDriver GetEarthNightLightShaderDriverComponent()
		{
			if (this.earthNightLightShaderDriver == null)
			{
				this.earthNightLightShaderDriver = GameStateManager.Earth().gameObjectLink.GetComponentInChildren<EarthNightLightShaderDriver>(true);
			}
			return this.earthNightLightShaderDriver;
		}

		// Token: 0x040042B2 RID: 17074
		[Inject]
		private NationPeriodicUpdate.NationGroup nations;

		// Token: 0x040042B3 RID: 17075
		[Inject]
		private GameTimeManager gameTime;

		// Token: 0x040042B4 RID: 17076
		private GameTimeCondition dailyCondition1;

		// Token: 0x040042B5 RID: 17077
		private GameTimeCondition dailyCondition2;

		// Token: 0x040042B6 RID: 17078
		private GameTimeCondition midMonthlyCondition;

		// Token: 0x040042B7 RID: 17079
		private GameTimeCondition monthlyCondition;

		// Token: 0x040042B8 RID: 17080
		private EarthNightLightShaderDriver earthNightLightShaderDriver;

		// Token: 0x02001352 RID: 4946
		private struct NationGroup
		{
			// Token: 0x04006FBF RID: 28607
			public readonly int Length;

			// Token: 0x04006FC0 RID: 28608
			public ComponentDataArray<Nation> Nation;
		}
	}
}
