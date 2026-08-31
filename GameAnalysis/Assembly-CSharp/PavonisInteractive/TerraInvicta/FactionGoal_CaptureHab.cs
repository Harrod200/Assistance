using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200074F RID: 1871
	public class FactionGoal_CaptureHab : FactionGoal_FleetCouncilorGoal
	{
		// Token: 0x1700072C RID: 1836
		// (get) Token: 0x06003039 RID: 12345 RVA: 0x00106DA1 File Offset: 0x00104FA1
		// (set) Token: 0x0600303A RID: 12346 RVA: 0x00106DA9 File Offset: 0x00104FA9
		public TIHabState captureTarget { get; protected set; }

		// Token: 0x0600303B RID: 12347 RVA: 0x00106DB2 File Offset: 0x00104FB2
		public FactionGoal_CaptureHab()
		{
			if (this.assignedCouncilors == null)
			{
				this.assignedCouncilors = new List<TICouncilorState>();
			}
		}

		// Token: 0x0600303C RID: 12348 RVA: 0x00106DCD File Offset: 0x00104FCD
		public FactionGoal_CaptureHab(TIFactionState faction, int importance, TIHabState captureTarget, GoalType habBuildGoal)
		{
			this.faction = faction;
			base.SetImportance(importance);
			this.captureTarget = captureTarget;
			base.councilorDestination = captureTarget;
			this.subsequentGoals = new List<GoalType> { habBuildGoal };
		}

		// Token: 0x0600303D RID: 12349 RVA: 0x00106E04 File Offset: 0x00105004
		public static FactionGoal_CaptureHab CreateGoal(FactionGoal_CaptureHab p)
		{
			FactionGoal_CaptureHab factionGoal_CaptureHab = GameStateManager.CreateNewGameState<FactionGoal_CaptureHab>();
			factionGoal_CaptureHab.captureTarget = p.captureTarget;
			factionGoal_CaptureHab.councilorDestination = p.councilorDestination;
			return factionGoal_CaptureHab;
		}

		// Token: 0x0600303E RID: 12350 RVA: 0x00106E23 File Offset: 0x00105023
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_CaptureHab>(base.ID, false);
		}

		// Token: 0x1700072D RID: 1837
		// (get) Token: 0x0600303F RID: 12351 RVA: 0x00106E34 File Offset: 0x00105034
		private bool accomplishWithoutFleet
		{
			get
			{
				if (!this.captureTarget.ref_naturalSpaceObject.isEarth && !(this.captureTarget.ref_naturalSpaceObject == GameStateManager.Luna()))
				{
					TILagrangePointState ref_lagrangePoint = this.captureTarget.ref_lagrangePoint;
					if (!(((ref_lagrangePoint != null) ? ref_lagrangePoint.ref_spaceBody : null) == GameStateManager.Luna()))
					{
						return this.assignedCouncilor != null && this.assignedCouncilor.location == this.captureTarget;
					}
				}
				return true;
			}
		}

		// Token: 0x1700072E RID: 1838
		// (get) Token: 0x06003040 RID: 12352 RVA: 0x00106EB5 File Offset: 0x001050B5
		public TICouncilorState assignedCouncilor
		{
			get
			{
				List<TICouncilorState> assignedCouncilors = this.assignedCouncilors;
				if (assignedCouncilors == null || assignedCouncilors.Count <= 0)
				{
					return null;
				}
				return this.assignedCouncilors[0];
			}
		}

		// Token: 0x06003041 RID: 12353 RVA: 0x00106EDC File Offset: 0x001050DC
		public override TIGameState actor()
		{
			TICouncilorState assignedCouncilor = this.assignedCouncilor;
			TIGameState tigameState;
			if ((tigameState = ((assignedCouncilor != null) ? assignedCouncilor.ref_gameState : null)) == null)
			{
				TISpaceFleetState assignedFleet = base.assignedFleet;
				if (assignedFleet == null)
				{
					return null;
				}
				tigameState = assignedFleet.ref_gameState;
			}
			return tigameState;
		}

		// Token: 0x06003042 RID: 12354 RVA: 0x00106F05 File Offset: 0x00105105
		public override TIGameState target()
		{
			return this.captureTarget;
		}

		// Token: 0x06003043 RID: 12355 RVA: 0x00106F0D File Offset: 0x0010510D
		public override TIGameState location()
		{
			return this.captureTarget;
		}

		// Token: 0x06003044 RID: 12356 RVA: 0x00106F15 File Offset: 0x00105115
		public override TIGameState goalProduct()
		{
			return this.captureTarget;
		}

		// Token: 0x06003045 RID: 12357 RVA: 0x00106F20 File Offset: 0x00105120
		public override bool ValidNewGoal()
		{
			if (this.captureTarget != null && this.faction.CanExplore(this.captureTarget.ref_spaceObject))
			{
				TIFactionState ref_faction = this.captureTarget.ref_faction;
				return ref_faction == null || !ref_faction.permanentAlly(this.faction);
			}
			return false;
		}

		// Token: 0x06003046 RID: 12358 RVA: 0x00106F74 File Offset: 0x00105174
		public override bool InProgress()
		{
			return (TIGameState.Valid(this.assignedCouncilor) || TIGameState.Valid(base.assignedFleet)) && this.faction.AvailableMissionControl >= this.captureTarget.MissionControlCost(false, null);
		}

		// Token: 0x06003047 RID: 12359 RVA: 0x00106FAF File Offset: 0x001051AF
		public override bool GoalFulfilled()
		{
			TIHabState captureTarget = this.captureTarget;
			return captureTarget != null && captureTarget.ref_faction.permanentAlly(this.faction);
		}

		// Token: 0x06003048 RID: 12360 RVA: 0x00106FCD File Offset: 0x001051CD
		public override GoalType GetGoalType()
		{
			return GoalType.CaptureHab;
		}

		// Token: 0x1700072F RID: 1839
		// (get) Token: 0x06003049 RID: 12361 RVA: 0x00106FD1 File Offset: 0x001051D1
		public override List<Type> fleetOperations
		{
			get
			{
				return FactionGoal_CaptureHab.fleetOps;
			}
		}

		// Token: 0x0600304A RID: 12362 RVA: 0x00106FD8 File Offset: 0x001051D8
		public override bool RequiresFleet()
		{
			return false;
		}

		// Token: 0x17000730 RID: 1840
		// (get) Token: 0x0600304B RID: 12363 RVA: 0x00106FDB File Offset: 0x001051DB
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_CaptureHab.incompatibleFleetGoals;
			}
		}

		// Token: 0x0600304C RID: 12364 RVA: 0x00106FE2 File Offset: 0x001051E2
		public override bool SpaceCombatGoal()
		{
			return true;
		}

		// Token: 0x0600304D RID: 12365 RVA: 0x00106FE5 File Offset: 0x001051E5
		public override bool FactionMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x17000731 RID: 1841
		// (get) Token: 0x0600304E RID: 12366 RVA: 0x00106FE8 File Offset: 0x001051E8
		public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
		{
			get
			{
				return FactionGoal_CaptureHab.missionModifiers;
			}
		}

		// Token: 0x0600304F RID: 12367 RVA: 0x00106FEF File Offset: 0x001051EF
		public override bool ReadyForTransferToTarget(TISpaceFleetState fleet)
		{
			return fleet != null && fleet.SpaceCombatValue() >= base.desiredFleetCombatValue && fleet.CanFulfillGoal(this, false) && fleet.AssaultCombatValue(false) >= this.GetDesiredAssaultCombatValue();
		}

		// Token: 0x06003050 RID: 12368 RVA: 0x00107028 File Offset: 0x00105228
		public override bool NeedsShipsOrdered()
		{
			if (this.accomplishWithoutFleet)
			{
				return false;
			}
			if (!(base.assignedFleet == null) && !base.NeedsShipsOrdered())
			{
				return base.assignedFleet.AssaultCombatValue(false) + this.pendingFleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.AssaultCombatValue(false)) + base.PendingShipTemplates().Sum<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.AssaultCombatValue(false)) < this.GetDesiredAssaultCombatValue();
			}
			return true;
		}

		// Token: 0x06003051 RID: 12369 RVA: 0x001070C4 File Offset: 0x001052C4
		public override ShipRole GetPrimaryShipRole()
		{
			return ShipRole.TroopCarrier;
		}

		// Token: 0x06003052 RID: 12370 RVA: 0x001070C7 File Offset: 0x001052C7
		public override Dictionary<ShipRole, float> GetSecondaryShipRoles()
		{
			return FactionGoal_CaptureHab.preferredRoles;
		}

		// Token: 0x06003053 RID: 12371 RVA: 0x001070CE File Offset: 0x001052CE
		public override void ChangeTarget(TIGameState newTarget)
		{
			this.captureTarget = ((newTarget != null) ? newTarget.ref_hab : null);
		}

		// Token: 0x06003054 RID: 12372 RVA: 0x001070E4 File Offset: 0x001052E4
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			List<TIFactionGoalState> list = new List<TIFactionGoalState>();
			using (List<GoalType>.Enumerator enumerator = this.subsequentGoals.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case GoalType.BuildFullStation:
						list.Add(new FactionGoal_BuildFullStation(this.faction, base.importance, this.captureTarget));
						break;
					case GoalType.BuildFullBase:
						list.Add(new FactionGoal_BuildFullBase(this.faction, base.importance, this.captureTarget));
						break;
					case GoalType.BuildMiningBase:
						list.Add(new FactionGoal_BuildMiningBase(this.faction, base.importance, this.captureTarget));
						break;
					case GoalType.BuildRefuellingStation:
						list.Add(new FactionGoal_BuildRefuellingStation(this.faction, base.importance, this.captureTarget));
						break;
					}
				}
			}
			return list;
		}

		// Token: 0x06003055 RID: 12373 RVA: 0x001071D4 File Offset: 0x001053D4
		public override float ComputeDesiredFleetCombatValue()
		{
			return FactionGoal_AttackWithFleet.ComputeDesiredFleetCombatValueForAttack(this.faction, this.target(), false, false);
		}

		// Token: 0x06003056 RID: 12374 RVA: 0x001071E9 File Offset: 0x001053E9
		public override float GetDesiredAssaultCombatValue()
		{
			return this.captureTarget.ref_hab.AssaultCombatValue(true) * 1.4f * this.faction.aiValues.riskAversion;
		}

		// Token: 0x06003057 RID: 12375 RVA: 0x00107214 File Offset: 0x00105414
		public override IEnumerable<TIMissionTemplate> GetUltimateMissionOptions()
		{
			if (this.assignedCouncilors.Count > 0)
			{
				return base.GetUltimateMissionOptions();
			}
			IEnumerable<TIMissionTemplate> enumerable = Enumerable.Empty<TIMissionTemplate>().Append(TIFactionState.controlHabMission);
			if (base.assignedFleet != null)
			{
				enumerable = enumerable.Append(TIFactionState.seizeHabMission);
			}
			return enumerable;
		}

		// Token: 0x06003058 RID: 12376 RVA: 0x00107264 File Offset: 0x00105464
		public override void DailyGoalMaintenance()
		{
			foreach (TICouncilorState ticouncilorState in this.assignedCouncilors.ToList<TICouncilorState>())
			{
				if (!TIGameState.Valid(ticouncilorState))
				{
					this.assignedCouncilors.Remove(ticouncilorState);
				}
			}
			base.DailyGoalMaintenance();
		}

		// Token: 0x06003059 RID: 12377 RVA: 0x001072D0 File Offset: 0x001054D0
		public override bool ShouldDiscardGoal()
		{
			if (base.importance <= 0)
			{
				return true;
			}
			if (this.captureTarget == null || this.captureTarget.archived)
			{
				return true;
			}
			TIFactionState ref_faction = this.captureTarget.ref_faction;
			return (ref_faction != null && ref_faction.permanentAlly(this.faction)) || (base.importance < 20 && !base.objectiveGoal && base.Age_years >= 5f && !this.InProgress());
		}

		// Token: 0x0400225E RID: 8798
		private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
		{
			{ "ControlSpaceAsset", 5f },
			{ "SeizeSpaceAsset", 5f },
			{ "SabotageHabModule", 5f }
		};

		// Token: 0x0400225F RID: 8799
		private static readonly List<GoalType> incompatibleFleetGoals = new List<GoalType>
		{
			GoalType.AttackWithFleet,
			GoalType.DefendWithFleet
		};

		// Token: 0x04002260 RID: 8800
		private static readonly Dictionary<ShipRole, float> preferredRoles = new Dictionary<ShipRole, float>
		{
			{
				ShipRole.ML_Standoff,
				0.5f
			},
			{
				ShipRole.MM_SpaceSuperiority,
				0.5f
			},
			{
				ShipRole.MS_Strike,
				0.5f
			},
			{
				ShipRole.LL_Intruder,
				1f
			},
			{
				ShipRole.LM_Interdictor,
				1f
			},
			{
				ShipRole.LS_Penetrator,
				1f
			},
			{
				ShipRole.LM_Protector,
				0.5f
			}
		};

		// Token: 0x04002261 RID: 8801
		public static readonly List<Type> fleetOps = new List<Type>(FactionGoal_Fleet.coreFleetOpsList) { typeof(AssaultHabOperation) };
	}
}
