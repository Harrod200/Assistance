using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200075A RID: 1882
	public class FactionGoal_InvadeEarth : FactionGoal_Fleet
	{
		// Token: 0x0600312E RID: 12590 RVA: 0x00108E59 File Offset: 0x00107059
		public FactionGoal_InvadeEarth()
		{
		}

		// Token: 0x0600312F RID: 12591 RVA: 0x00108E61 File Offset: 0x00107061
		public FactionGoal_InvadeEarth(TIFactionState faction, int importance)
		{
			this.faction = faction;
			base.SetImportance(importance);
			this.subsequentGoals = new List<GoalType>
			{
				GoalType.DefendWithFleet,
				GoalType.InvadeEarth
			};
		}

		// Token: 0x06003130 RID: 12592 RVA: 0x00108E92 File Offset: 0x00107092
		public static FactionGoal_InvadeEarth CreateGoal(FactionGoal_InvadeEarth p)
		{
			return GameStateManager.CreateNewGameState<FactionGoal_InvadeEarth>();
		}

		// Token: 0x06003131 RID: 12593 RVA: 0x00108E99 File Offset: 0x00107099
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_InvadeEarth>(base.ID, false);
		}

		// Token: 0x06003132 RID: 12594 RVA: 0x00108EA8 File Offset: 0x001070A8
		public override GoalType GetGoalType()
		{
			return GoalType.InvadeEarth;
		}

		// Token: 0x06003133 RID: 12595 RVA: 0x00108EAC File Offset: 0x001070AC
		public override TIGameState actor()
		{
			return base.assignedFleet;
		}

		// Token: 0x06003134 RID: 12596 RVA: 0x00108EB4 File Offset: 0x001070B4
		public override TIGameState target()
		{
			return GameStateManager.LEOStates()[0];
		}

		// Token: 0x06003135 RID: 12597 RVA: 0x00108EC1 File Offset: 0x001070C1
		public override TIGameState location()
		{
			return this.target();
		}

		// Token: 0x06003136 RID: 12598 RVA: 0x00108EC9 File Offset: 0x001070C9
		public override TIGameState goalProduct()
		{
			return base.assignedFleet;
		}

		// Token: 0x06003137 RID: 12599 RVA: 0x00108ED1 File Offset: 0x001070D1
		public override bool RequiresFleet()
		{
			return true;
		}

		// Token: 0x06003138 RID: 12600 RVA: 0x00108ED4 File Offset: 0x001070D4
		public override bool ValidNewGoal()
		{
			return true;
		}

		// Token: 0x06003139 RID: 12601 RVA: 0x00108ED7 File Offset: 0x001070D7
		public override bool InProgress()
		{
			return base.assignedFleet != null;
		}

		// Token: 0x0600313A RID: 12602 RVA: 0x00108EE5 File Offset: 0x001070E5
		public override bool IsDuplicate(TIFactionGoalState testGoal, TIGameState testTarget)
		{
			return base.assignedFleet != null && testGoal.ref_fleetGoal.assignedFleet == base.assignedFleet;
		}

		// Token: 0x0600313B RID: 12603 RVA: 0x00108F0D File Offset: 0x0010710D
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0;
		}

		// Token: 0x0600313C RID: 12604 RVA: 0x00108F1B File Offset: 0x0010711B
		public override bool GoalFulfilled()
		{
			TISpaceFleetState assignedFleet = base.assignedFleet;
			return ((assignedFleet != null) ? assignedFleet.location : null) == this.target() && !base.assignedFleet.HasSpecialModuleCapability(SpecialModuleRule.LandArmy);
		}

		// Token: 0x1700074B RID: 1867
		// (get) Token: 0x0600313D RID: 12605 RVA: 0x00108F4E File Offset: 0x0010714E
		public override List<Type> fleetOperations
		{
			get
			{
				return FactionGoal_InvadeEarth.fleetOps;
			}
		}

		// Token: 0x1700074C RID: 1868
		// (get) Token: 0x0600313E RID: 12606 RVA: 0x00108F55 File Offset: 0x00107155
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600313F RID: 12607 RVA: 0x00108F58 File Offset: 0x00107158
		public override void ChangeTarget(TIGameState newTarget)
		{
		}

		// Token: 0x1700074D RID: 1869
		// (get) Token: 0x06003140 RID: 12608 RVA: 0x00108F5A File Offset: 0x0010715A
		public override bool buildFleetsSequentially
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003141 RID: 12609 RVA: 0x00108F5D File Offset: 0x0010715D
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return new List<TIFactionGoalState>();
		}

		// Token: 0x06003142 RID: 12610 RVA: 0x00108F64 File Offset: 0x00107164
		public override void OnGoalComplete()
		{
			TISpaceFleetState assignedFleet = base.assignedFleet;
			List<TISpaceFleetState> list = new List<TISpaceFleetState>();
			List<TISpaceFleetState> list2 = new List<TISpaceFleetState>();
			TISpaceFleetState tispaceFleetState = null;
			foreach (TISpaceFleetState tispaceFleetState2 in this.faction.fleets)
			{
				if (tispaceFleetState2 != base.assignedFleet)
				{
					FactionGoal_Fleet factionGoal_Fleet = tispaceFleetState2.AssignedGoal();
					if (factionGoal_Fleet != null && factionGoal_Fleet.GetGoalType() == GoalType.JoinFleet && factionGoal_Fleet.target() == assignedFleet)
					{
						if (tispaceFleetState2.InvasionFleet())
						{
							list.Add(tispaceFleetState2);
						}
						else
						{
							list2.Add(tispaceFleetState2);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				TISpaceFleetState tispaceFleetState3 = list.MinBy<TISpaceFleetState, double>((TISpaceFleetState x) => TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(x, GameStateManager.Earth()));
				this.AssignFleet(tispaceFleetState3);
				tispaceFleetState = tispaceFleetState3;
				using (List<TISpaceFleetState>.Enumerator enumerator = list2.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TISpaceFleetState tispaceFleetState4 = enumerator.Current;
						this.faction.AddGoal(new FactionGoal_JoinFleet(this.faction, tispaceFleetState), HandleDuplicateGoalRule.Ignore, tispaceFleetState4);
					}
					goto IL_012A;
				}
			}
			base.OnGoalComplete();
			IL_012A:
			if (TIGameState.Valid(assignedFleet))
			{
				if (tispaceFleetState != null)
				{
					this.faction.AddGoal(new FactionGoal_JoinFleet(this.faction, tispaceFleetState), HandleDuplicateGoalRule.Ignore, assignedFleet);
					return;
				}
				TIFactionGoalState tifactionGoalState = this.faction.GoalsOfType(GoalType.SecureEarthSpace, false, true).FirstOrDefault<TIFactionGoalState>();
				if (tifactionGoalState != null)
				{
					FactionGoal_SecureEarthSpace factionGoal_SecureEarthSpace = tifactionGoalState as FactionGoal_SecureEarthSpace;
					if (TIGameState.Valid(factionGoal_SecureEarthSpace.assignedFleet) && factionGoal_SecureEarthSpace.assignedFleet.location.ref_naturalSpaceObject.isEarth)
					{
						this.faction.AddGoal(new FactionGoal_JoinFleet(this.faction, factionGoal_SecureEarthSpace.assignedFleet), HandleDuplicateGoalRule.Ignore, assignedFleet);
						return;
					}
					TISpaceFleetState tispaceFleetState5 = null;
					if (TIGameState.Valid(factionGoal_SecureEarthSpace.assignedFleet))
					{
						tispaceFleetState5 = factionGoal_SecureEarthSpace.assignedFleet;
					}
					factionGoal_SecureEarthSpace.AssignFleet(assignedFleet);
					if (tispaceFleetState5 != null)
					{
						this.faction.AddGoal(new FactionGoal_JoinFleet(this.faction, factionGoal_SecureEarthSpace.assignedFleet), HandleDuplicateGoalRule.Ignore, tispaceFleetState5);
						return;
					}
				}
				else
				{
					this.faction.AddGoal(new FactionGoal_SecureEarthSpace(this.faction, 10), HandleDuplicateGoalRule.ResetImportanceIfHigher, assignedFleet);
				}
			}
		}

		// Token: 0x06003143 RID: 12611 RVA: 0x001091C4 File Offset: 0x001073C4
		public override ShipRole GetPrimaryShipRole()
		{
			return ShipRole.ArmyCarrier;
		}

		// Token: 0x06003144 RID: 12612 RVA: 0x001091C7 File Offset: 0x001073C7
		public override Dictionary<ShipRole, float> GetSecondaryShipRoles()
		{
			return FactionGoal_InvadeEarth.preferredShipRoles;
		}

		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x06003145 RID: 12613 RVA: 0x001091D0 File Offset: 0x001073D0
		public float ProspectiveInvasionCombatValue
		{
			get
			{
				TISpaceFleetState assignedFleet = base.assignedFleet;
				return ((assignedFleet != null) ? assignedFleet.InvasionCombatValue() : 0f) + this.pendingFleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.InvasionCombatValue()) + base.PendingShipTemplates().Sum<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.InvasionCombatValue());
			}
		}

		// Token: 0x06003146 RID: 12614 RVA: 0x00109249 File Offset: 0x00107449
		public override bool NeedsPrimaryRoleOrdered(List<TISpaceShipTemplate> pendingShipTemplates)
		{
			return this.ProspectiveInvasionCombatValue < this.GetDesiredAssaultCombatValue();
		}

		// Token: 0x06003147 RID: 12615 RVA: 0x0010925C File Offset: 0x0010745C
		public override float ComputeDesiredFleetCombatValue()
		{
			if (this.ShouldPerformMissionMinimallyArmed)
			{
				return 0f;
			}
			TISpaceFleetState assignedFleet = base.assignedFleet;
			float num;
			if (assignedFleet == null)
			{
				num = 1f;
			}
			else
			{
				num = assignedFleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.role == this.GetPrimaryShipRole()).Sum<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f));
			}
			return num + base.ComputeDesiredFleetCombatValue();
		}

		// Token: 0x06003148 RID: 12616 RVA: 0x001092CC File Offset: 0x001074CC
		public override float GetDesiredAssaultCombatValue()
		{
			return TemplateManager.global.AssaultValue_AlienArmy * (float)TemplateManager.global.alienArmiesFromLanding + Mathf.Clamp(TemplateManager.global.AssaultValue_AlienArmy * (float)(this.faction.armiesLost[ArmyType.AlienInvader] - TemplateManager.global.AI_invaderArmiesLostBeforeBuildup), 0f, TemplateManager.global.AssaultValue_AlienArmy * 12f);
		}

		// Token: 0x06003149 RID: 12617 RVA: 0x00109334 File Offset: 0x00107534
		private bool ShouldWaitToInvade()
		{
			if (!this.faction.IsAlienFaction)
			{
				return false;
			}
			if (GameStateManager.AlienNation().extant)
			{
				return false;
			}
			return !GameStateManager.AllHumanFactions().Any<TIFactionState>((TIFactionState x) => x.MilestoneCompleted(CampaignMilestone.OverthrewAlienNation)) && AIEvaluators.GetAlienQuietness() >= 0.2f;
		}

		// Token: 0x0600314A RID: 12618 RVA: 0x0010939A File Offset: 0x0010759A
		public override bool ReadyForTransferToTarget(TISpaceFleetState fleet)
		{
			return !this.ShouldWaitToInvade() && (fleet != null && fleet.SpaceCombatValue() >= base.desiredFleetCombatValue && fleet.CanFulfillGoal(this, false)) && fleet.InvasionCombatValue() >= this.GetDesiredAssaultCombatValue();
		}

		// Token: 0x0600314B RID: 12619 RVA: 0x001093DC File Offset: 0x001075DC
		public override void DailyGoalMaintenance()
		{
			base.DailyGoalMaintenance();
			if (TITimeState.Now().day % 8 != 0)
			{
				return;
			}
			if (new AlienLandArmyOperation().GetPossibleTargets(this.faction, null).Count == 0)
			{
				string dataName = OperationsManager.operationsLookup[typeof(AlienLandArmyOperation)].GetTemplate().dataName;
				TIRegionState tiregionState = AIEvaluators.SelectAlienArmyLandingRegion(true);
				if (tiregionState != null && tiregionState.antiSpaceDefenses)
				{
					if (!(from x in this.faction.GoalsOfType(GoalType.AttackWithFleet, false, true)
						where x.target() is TISpaceDefensesFacilityState
						select x).ToList<TIFactionGoalState>().Any<TIFactionGoalState>())
					{
						int num = Mathf.Min(base.importance + 1, 19);
						this.faction.AddGoal(new FactionGoal_AttackWithFleet(this.faction, num, tiregionState.spaceDefenseFacility, false, null, false), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
						return;
					}
				}
				else
				{
					Log.Debug("There are no valid landing locations for alien armies and the AI does not know how to deal with it.", Array.Empty<object>());
				}
			}
		}

		// Token: 0x04002275 RID: 8821
		public static readonly List<Type> fleetOps = new List<Type>(FactionGoal_Fleet.coreFleetOpsList) { typeof(AlienLandArmyOperation) };

		// Token: 0x04002276 RID: 8822
		private static readonly Dictionary<ShipRole, float> preferredShipRoles = new Dictionary<ShipRole, float>
		{
			{
				ShipRole.LS_Penetrator,
				1f
			},
			{
				ShipRole.LM_Interdictor,
				1f
			},
			{
				ShipRole.LL_Intruder,
				1f
			},
			{
				ShipRole.LM_Protector,
				0.75f
			}
		};
	}
}
