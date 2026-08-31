using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Actions;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000738 RID: 1848
	public class FactionGoal_TruceWithFaction : FactionGoal_FriendlyRelations
	{
		// Token: 0x06002E83 RID: 11907 RVA: 0x000FD500 File Offset: 0x000FB700
		public FactionGoal_TruceWithFaction()
		{
		}

		// Token: 0x06002E84 RID: 11908 RVA: 0x000FD508 File Offset: 0x000FB708
		public FactionGoal_TruceWithFaction(TIFactionState faction, int importance, TIFactionState enemyFaction)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.targetFaction = enemyFaction;
			this.expireDate = new TIDateTime(TITimeState.Now());
			this.expireDate.AddMonths(12);
		}

		// Token: 0x06002E85 RID: 11909 RVA: 0x000FD544 File Offset: 0x000FB744
		public static FactionGoal_TruceWithFaction CreateGoal(FactionGoal_TruceWithFaction p)
		{
			FactionGoal_TruceWithFaction factionGoal_TruceWithFaction = GameStateManager.CreateNewGameState<FactionGoal_TruceWithFaction>();
			factionGoal_TruceWithFaction.faction = p.faction;
			factionGoal_TruceWithFaction.targetFaction = p.targetFaction;
			factionGoal_TruceWithFaction.expireDate = new TIDateTime(p.expireDate);
			factionGoal_TruceWithFaction.yesterdaysHate = p.faction.GetFactionHate(factionGoal_TruceWithFaction.targetFaction);
			return factionGoal_TruceWithFaction;
		}

		// Token: 0x06002E86 RID: 11910 RVA: 0x000FD598 File Offset: 0x000FB798
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_TruceWithFaction>(base.ID, false);
		}

		// Token: 0x06002E87 RID: 11911 RVA: 0x000FD5A7 File Offset: 0x000FB7A7
		public override bool NationMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002E88 RID: 11912 RVA: 0x000FD5AA File Offset: 0x000FB7AA
		public override bool FactionMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002E89 RID: 11913 RVA: 0x000FD5AD File Offset: 0x000FB7AD
		public override bool PoliciesAtTargetNationGoal()
		{
			return true;
		}

		// Token: 0x06002E8A RID: 11914 RVA: 0x000FD5B0 File Offset: 0x000FB7B0
		public override GoalType GetGoalType()
		{
			return GoalType.TruceWithFaction;
		}

		// Token: 0x06002E8B RID: 11915 RVA: 0x000FD5B4 File Offset: 0x000FB7B4
		public override TIGameState actor()
		{
			return this.faction;
		}

		// Token: 0x06002E8C RID: 11916 RVA: 0x000FD5BC File Offset: 0x000FB7BC
		public override TIGameState target()
		{
			return base.targetFaction;
		}

		// Token: 0x06002E8D RID: 11917 RVA: 0x000FD5C4 File Offset: 0x000FB7C4
		public override TIGameState location()
		{
			return null;
		}

		// Token: 0x06002E8E RID: 11918 RVA: 0x000FD5C7 File Offset: 0x000FB7C7
		public override TIGameState goalProduct()
		{
			return base.targetFaction;
		}

		// Token: 0x06002E8F RID: 11919 RVA: 0x000FD5CF File Offset: 0x000FB7CF
		public override bool ValidNewGoal()
		{
			return !this.ShouldDiscardGoal() && !this.GoalFulfilled();
		}

		// Token: 0x06002E90 RID: 11920 RVA: 0x000FD5E4 File Offset: 0x000FB7E4
		public override bool InProgress()
		{
			return true;
		}

		// Token: 0x06002E91 RID: 11921 RVA: 0x000FD5E7 File Offset: 0x000FB7E7
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.targetFaction == null;
		}

		// Token: 0x06002E92 RID: 11922 RVA: 0x000FD600 File Offset: 0x000FB800
		public override bool GoalFulfilled()
		{
			return TITimeState.Now() > this.expireDate;
		}

		// Token: 0x170006E3 RID: 1763
		// (get) Token: 0x06002E93 RID: 11923 RVA: 0x000FD612 File Offset: 0x000FB812
		public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
		{
			get
			{
				return FactionGoal_TruceWithFaction.missionModifiers;
			}
		}

		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x06002E94 RID: 11924 RVA: 0x000FD619 File Offset: 0x000FB819
		public override List<Type> armyOperations
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x06002E95 RID: 11925 RVA: 0x000FD61C File Offset: 0x000FB81C
		public override List<PolicyType> policiesAtTarget
		{
			get
			{
				return FactionGoal_TruceWithFaction.policies_SetPolicy;
			}
		}

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x06002E96 RID: 11926 RVA: 0x000FD623 File Offset: 0x000FB823
		public override List<PolicyType> factionLevelPoliciesAtTarget
		{
			get
			{
				return FactionGoal_TruceWithFaction.policies_faction;
			}
		}

		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x06002E97 RID: 11927 RVA: 0x000FD62A File Offset: 0x000FB82A
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_TruceWithFaction.incompatibleGoalsForFaction;
			}
		}

		// Token: 0x06002E98 RID: 11928 RVA: 0x000FD631 File Offset: 0x000FB831
		public override void OnGoalAssigned()
		{
			this.faction.GainFactionHate(base.targetFaction, -3f, false, "New Truce", true);
		}

		// Token: 0x06002E99 RID: 11929 RVA: 0x000FD650 File Offset: 0x000FB850
		public override void DailyGoalMaintenance()
		{
			float factionHate = this.faction.GetFactionHate(base.targetFaction);
			if (factionHate > base.yesterdaysHate && factionHate > 1f && (factionHate > TIGlobalConfig.globalConfig.factionHateWarThreshold || factionHate - base.yesterdaysHate >= 3f || !AIEvaluators.HumanFactionTooBeatDownToContinue(this.faction, base.targetFaction)))
			{
				this.faction.playerControl.StartAction(new BreakPactAction(this.faction, base.targetFaction, new List<TradeOffer.TreatyType> { TradeOffer.TreatyType.Truce }));
			}
			else if (this.faction.player.isAI && (this.faction.currentlyHuntingHydraToKill || this.faction.currentlyCapturingHydra) && base.targetFaction.IsAlienFaction)
			{
				this.faction.playerControl.StartAction(new BreakPactAction(this.faction, base.targetFaction, new List<TradeOffer.TreatyType> { TradeOffer.TreatyType.Truce }));
			}
			else
			{
				this.faction.GainFactionHate(base.targetFaction, -0.02f, false, "Sustained Truce", true);
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
			base.yesterdaysHate = factionHate;
		}

		// Token: 0x06002E9A RID: 11930 RVA: 0x000FD7EA File Offset: 0x000FB9EA
		public override void OnGoalRemoved()
		{
			TINotificationQueueState.AddCouncilorMessage(this.faction, CouncilorChatType.TruceEnded, base.targetFaction);
		}

		// Token: 0x04002217 RID: 8727
		public const int truceDuration_months = 12;

		// Token: 0x04002218 RID: 8728
		public TIDateTime expireDate;

		// Token: 0x04002219 RID: 8729
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

		// Token: 0x0400221A RID: 8730
		private static readonly List<GoalType> incompatibleGoalsForFaction = new List<GoalType>
		{
			GoalType.WarOnFaction,
			GoalType.NonAggressionPact
		};

		// Token: 0x0400221B RID: 8731
		private static readonly List<PolicyType> policies_SetPolicy = new List<PolicyType> { PolicyType.EndWarOption };

		// Token: 0x0400221C RID: 8732
		private static readonly List<PolicyType> policies_faction = new List<PolicyType> { PolicyType.EndRivalryOption };
	}
}
