using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Actions;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000739 RID: 1849
	public class FactionGoal_NonAggressionPact : FactionGoal_FriendlyRelations
	{
		// Token: 0x06002E9E RID: 11934 RVA: 0x000FD9A1 File Offset: 0x000FBBA1
		public FactionGoal_NonAggressionPact()
		{
		}

		// Token: 0x06002E9F RID: 11935 RVA: 0x000FD9A9 File Offset: 0x000FBBA9
		public FactionGoal_NonAggressionPact(TIFactionState faction, int importance, TIFactionState enemyFaction)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.targetFaction = enemyFaction;
		}

		// Token: 0x06002EA0 RID: 11936 RVA: 0x000FD9C8 File Offset: 0x000FBBC8
		public static FactionGoal_NonAggressionPact CreateGoal(FactionGoal_NonAggressionPact p)
		{
			FactionGoal_NonAggressionPact factionGoal_NonAggressionPact = GameStateManager.CreateNewGameState<FactionGoal_NonAggressionPact>();
			factionGoal_NonAggressionPact.faction = p.faction;
			factionGoal_NonAggressionPact.targetFaction = p.targetFaction;
			factionGoal_NonAggressionPact.yesterdaysHate = factionGoal_NonAggressionPact.faction.GetFactionHate(factionGoal_NonAggressionPact.targetFaction);
			return factionGoal_NonAggressionPact;
		}

		// Token: 0x06002EA1 RID: 11937 RVA: 0x000FDA0B File Offset: 0x000FBC0B
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_NonAggressionPact>(base.ID, false);
		}

		// Token: 0x06002EA2 RID: 11938 RVA: 0x000FDA1A File Offset: 0x000FBC1A
		public override bool NationMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002EA3 RID: 11939 RVA: 0x000FDA1D File Offset: 0x000FBC1D
		public override bool FactionMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002EA4 RID: 11940 RVA: 0x000FDA20 File Offset: 0x000FBC20
		public override bool PoliciesAtTargetNationGoal()
		{
			return true;
		}

		// Token: 0x06002EA5 RID: 11941 RVA: 0x000FDA23 File Offset: 0x000FBC23
		public override GoalType GetGoalType()
		{
			return GoalType.NonAggressionPact;
		}

		// Token: 0x06002EA6 RID: 11942 RVA: 0x000FDA27 File Offset: 0x000FBC27
		public override TIGameState actor()
		{
			return this.faction;
		}

		// Token: 0x06002EA7 RID: 11943 RVA: 0x000FDA2F File Offset: 0x000FBC2F
		public override TIGameState target()
		{
			return base.targetFaction;
		}

		// Token: 0x06002EA8 RID: 11944 RVA: 0x000FDA37 File Offset: 0x000FBC37
		public override TIGameState location()
		{
			return null;
		}

		// Token: 0x06002EA9 RID: 11945 RVA: 0x000FDA3A File Offset: 0x000FBC3A
		public override TIGameState goalProduct()
		{
			return base.targetFaction;
		}

		// Token: 0x06002EAA RID: 11946 RVA: 0x000FDA42 File Offset: 0x000FBC42
		public override bool ValidNewGoal()
		{
			return base.importance > 0 && !(base.targetFaction == null) && !this.GoalFulfilled();
		}

		// Token: 0x06002EAB RID: 11947 RVA: 0x000FDA66 File Offset: 0x000FBC66
		public override bool InProgress()
		{
			return true;
		}

		// Token: 0x06002EAC RID: 11948 RVA: 0x000FDA69 File Offset: 0x000FBC69
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.targetFaction == null || AIEvaluators.FactionsGoToWar(this.faction, base.targetFaction);
		}

		// Token: 0x06002EAD RID: 11949 RVA: 0x000FDA95 File Offset: 0x000FBC95
		public override bool GoalFulfilled()
		{
			return !this.faction.permanentAlly(base.targetFaction) && base.targetFaction.GetObjectivesByTypeAndStatus(ObjectiveType.Victory, ObjectiveStatus.Unlocked).Count > 0;
		}

		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x06002EAE RID: 11950 RVA: 0x000FDAC1 File Offset: 0x000FBCC1
		public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
		{
			get
			{
				return FactionGoal_NonAggressionPact.missionModifiers;
			}
		}

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x06002EAF RID: 11951 RVA: 0x000FDAC8 File Offset: 0x000FBCC8
		public override List<Type> armyOperations
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x06002EB0 RID: 11952 RVA: 0x000FDACB File Offset: 0x000FBCCB
		public override List<PolicyType> policiesAtTarget
		{
			get
			{
				return FactionGoal_NonAggressionPact.policies_SetPolicy;
			}
		}

		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x06002EB1 RID: 11953 RVA: 0x000FDAD2 File Offset: 0x000FBCD2
		public override List<PolicyType> factionLevelPoliciesAtTarget
		{
			get
			{
				return FactionGoal_NonAggressionPact.policies_faction;
			}
		}

		// Token: 0x170006EC RID: 1772
		// (get) Token: 0x06002EB2 RID: 11954 RVA: 0x000FDAD9 File Offset: 0x000FBCD9
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_NonAggressionPact.incompatibleGoalsForFaction;
			}
		}

		// Token: 0x06002EB3 RID: 11955 RVA: 0x000FDAE0 File Offset: 0x000FBCE0
		public override void DailyGoalMaintenance()
		{
			float factionHate = this.faction.GetFactionHate(base.targetFaction);
			if (factionHate > 1f && factionHate > base.yesterdaysHate && (factionHate >= TIGlobalConfig.globalConfig.factionHateWarThreshold || factionHate - base.yesterdaysHate >= 3f || !AIEvaluators.HumanFactionTooBeatDownToContinue(this.faction, base.targetFaction)))
			{
				this.faction.playerControl.StartAction(new BreakPactAction(this.faction, base.targetFaction, new List<TradeOffer.TreatyType> { TradeOffer.TreatyType.NAP }));
			}
			else if (this.faction.player.isAI && (this.faction.currentlyHuntingHydraToKill || this.faction.currentlyCapturingHydra) && base.targetFaction.IsAlienFaction)
			{
				this.faction.playerControl.StartAction(new BreakPactAction(this.faction, base.targetFaction, new List<TradeOffer.TreatyType> { TradeOffer.TreatyType.NAP }));
			}
			else
			{
				List<TIFactionGoalState> list = this.faction.factionGoals[GoalType.AttackWithFleet].Where<TIFactionGoalState>(delegate(TIFactionGoalState x)
				{
					TIGameState tigameState = x.target();
					return ((tigameState != null) ? tigameState.ref_faction : null) == this.target();
				}).ToList<TIFactionGoalState>();
				list.AddRange(this.faction.factionGoals[GoalType.CaptureHab].Where<TIFactionGoalState>(delegate(TIFactionGoalState x)
				{
					TIGameState tigameState2 = x.target();
					return ((tigameState2 != null) ? tigameState2.ref_faction : null) == this.target();
				}).ToList<TIFactionGoalState>());
				list.ForEach(delegate(TIFactionGoalState x)
				{
					x.SetImportance(0);
				});
			}
			if (this.faction.intelSharingFactions.Contains(base.targetFaction) && AIEvaluators.GetWillingnessToShareIntel(this.faction, base.targetFaction, false, true) < 1)
			{
				this.faction.playerControl.StartAction(new BreakPactAction(this.faction, base.targetFaction, new List<TradeOffer.TreatyType> { TradeOffer.TreatyType.Intel }));
			}
			base.yesterdaysHate = factionHate;
		}

		// Token: 0x06002EB4 RID: 11956 RVA: 0x000FDCB8 File Offset: 0x000FBEB8
		public override void OnGoalComplete()
		{
			base.OnGoalComplete();
			if (this.faction.intelSharingFactions.Contains(base.targetFaction) && AIEvaluators.GetWillingnessToShareIntel(this.faction, base.targetFaction, false, true) < 1)
			{
				this.faction.playerControl.StartAction(new BreakPactAction(this.faction, base.targetFaction, new List<TradeOffer.TreatyType> { TradeOffer.TreatyType.Intel }));
			}
		}

		// Token: 0x06002EB5 RID: 11957 RVA: 0x000FDD26 File Offset: 0x000FBF26
		public override void OnGoalRemoved()
		{
			TINotificationQueueState.AddCouncilorMessage(this.faction, CouncilorChatType.NAPEnded, base.targetFaction);
		}

		// Token: 0x0400221D RID: 8733
		private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
		{
			{ "Coup", 0f },
			{ "Crackdown", 0f },
			{ "Purge", 0f },
			{ "Unrest", 0f },
			{ "Propaganda", 0.1f },
			{ "SabotageFacilities", 0f },
			{ "Assassinate", 0f },
			{ "Detain", 0f },
			{ "HostileTakeover", 0f },
			{ "SabotageProject", 0f },
			{ "StealProject", 0f },
			{ "Turn", 0f },
			{ "SeizeSpaceAsset", 0f },
			{ "ControlSpaceAsset", 0f },
			{ "SabotageHabModule", 0f },
			{ "EnthrallElites", 0f },
			{ "EnthrallOrg", 0f }
		};

		// Token: 0x0400221E RID: 8734
		private static readonly List<GoalType> incompatibleGoalsForFaction = new List<GoalType>
		{
			GoalType.WarOnFaction,
			GoalType.TruceWithFaction
		};

		// Token: 0x0400221F RID: 8735
		private static readonly List<PolicyType> policies_SetPolicy = new List<PolicyType> { PolicyType.EndWarOption };

		// Token: 0x04002220 RID: 8736
		private static readonly List<PolicyType> policies_faction = new List<PolicyType>
		{
			PolicyType.EndRivalryOption,
			PolicyType.ProposeAllianceOption
		};
	}
}
