using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000751 RID: 1873
	public class FactionGoal_JoinFleet : FactionGoal_Fleet
	{
		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x0600307C RID: 12412 RVA: 0x001076E8 File Offset: 0x001058E8
		// (set) Token: 0x0600307D RID: 12413 RVA: 0x001076F0 File Offset: 0x001058F0
		public TISpaceFleetState targetFleet { get; protected set; }

		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x0600307E RID: 12414 RVA: 0x001076F9 File Offset: 0x001058F9
		// (set) Token: 0x0600307F RID: 12415 RVA: 0x00107701 File Offset: 0x00105901
		public FactionGoal_Fleet targetFleetGoal { get; protected set; }

		// Token: 0x06003080 RID: 12416 RVA: 0x0010770A File Offset: 0x0010590A
		public FactionGoal_JoinFleet()
		{
		}

		// Token: 0x06003081 RID: 12417 RVA: 0x00107714 File Offset: 0x00105914
		public FactionGoal_JoinFleet(TIFactionState faction, TISpaceFleetState targetFleet)
		{
			this.faction = faction;
			this.targetFleetGoal = targetFleet.AssignedGoal();
			this.targetFleet = targetFleet;
			if (this.targetFleetGoal != null)
			{
				base.SetImportance(Mathf.Clamp(this.targetFleetGoal.importance - 1, 1, 19));
				return;
			}
			base.SetImportance(5);
		}

		// Token: 0x06003082 RID: 12418 RVA: 0x00107772 File Offset: 0x00105972
		public static FactionGoal_JoinFleet CreateGoal(FactionGoal_JoinFleet p)
		{
			FactionGoal_JoinFleet factionGoal_JoinFleet = GameStateManager.CreateNewGameState<FactionGoal_JoinFleet>();
			factionGoal_JoinFleet.targetFleet = p.targetFleet;
			return factionGoal_JoinFleet;
		}

		// Token: 0x06003083 RID: 12419 RVA: 0x00107785 File Offset: 0x00105985
		public override void AssignFleet(TISpaceFleetState fleet)
		{
			TISpaceFleetState targetFleet = this.targetFleet;
			if (targetFleet != null)
			{
				FactionGoal_Fleet factionGoal_Fleet = targetFleet.AssignedGoal();
				if (factionGoal_Fleet != null)
				{
					factionGoal_Fleet.AddPendingFleet(fleet);
				}
			}
			base.AssignFleet(fleet);
		}

		// Token: 0x06003084 RID: 12420 RVA: 0x001077AC File Offset: 0x001059AC
		public override void UnassignFleet()
		{
			TISpaceFleetState targetFleet = this.targetFleet;
			if (targetFleet != null)
			{
				FactionGoal_Fleet factionGoal_Fleet = targetFleet.AssignedGoal();
				if (factionGoal_Fleet != null)
				{
					factionGoal_Fleet.RemovePendingFleet(base.assignedFleet);
				}
			}
			base.UnassignFleet();
		}

		// Token: 0x06003085 RID: 12421 RVA: 0x001077D7 File Offset: 0x001059D7
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_JoinFleet>(base.ID, false);
		}

		// Token: 0x06003086 RID: 12422 RVA: 0x001077E6 File Offset: 0x001059E6
		public override GoalType GetGoalType()
		{
			return GoalType.JoinFleet;
		}

		// Token: 0x06003087 RID: 12423 RVA: 0x001077EA File Offset: 0x001059EA
		public override TIGameState actor()
		{
			return base.assignedFleet;
		}

		// Token: 0x06003088 RID: 12424 RVA: 0x001077F2 File Offset: 0x001059F2
		public override TIGameState target()
		{
			return this.targetFleet;
		}

		// Token: 0x06003089 RID: 12425 RVA: 0x001077FA File Offset: 0x001059FA
		public override TIGameState location()
		{
			return this.targetFleet.location;
		}

		// Token: 0x0600308A RID: 12426 RVA: 0x00107807 File Offset: 0x00105A07
		public override TIGameState goalProduct()
		{
			return this.targetFleet;
		}

		// Token: 0x0600308B RID: 12427 RVA: 0x0010780F File Offset: 0x00105A0F
		public override bool RequiresFleet()
		{
			return true;
		}

		// Token: 0x0600308C RID: 12428 RVA: 0x00107812 File Offset: 0x00105A12
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x0600308D RID: 12429 RVA: 0x00107815 File Offset: 0x00105A15
		public override float ComputeDesiredFleetCombatValue()
		{
			if (this.ShouldPerformMissionMinimallyArmed)
			{
				return 0f;
			}
			return AIEvaluators.GetThreatLevelAtLocation(this.faction, this.target(), true);
		}

		// Token: 0x0600308E RID: 12430 RVA: 0x00107837 File Offset: 0x00105A37
		public override bool ReadyForTransferToTarget(TISpaceFleetState fleet)
		{
			return (fleet.ref_system != null && fleet.ref_system == this.targetFleet.ref_system) || base.ReadyForTransferToTarget(fleet);
		}

		// Token: 0x0600308F RID: 12431 RVA: 0x00107868 File Offset: 0x00105A68
		public override float GetDesiredAssaultCombatValue()
		{
			return 0f;
		}

		// Token: 0x06003090 RID: 12432 RVA: 0x0010786F File Offset: 0x00105A6F
		public override ShipRole GetPrimaryShipRole()
		{
			return ShipRole.NoRole;
		}

		// Token: 0x06003091 RID: 12433 RVA: 0x00107874 File Offset: 0x00105A74
		public override Dictionary<ShipRole, float> GetSecondaryShipRoles()
		{
			return Enums.ShipRoles.ToDictionary<ShipRole, ShipRole, float>((ShipRole x) => x, (ShipRole x) => 1f);
		}

		// Token: 0x06003092 RID: 12434 RVA: 0x001078CC File Offset: 0x00105ACC
		public override bool IsDuplicate(TIFactionGoalState testGoal, TIGameState testTarget)
		{
			FactionGoal_JoinFleet factionGoal_JoinFleet = testGoal as FactionGoal_JoinFleet;
			return factionGoal_JoinFleet != null && base.assignedFleet != null && base.assignedFleet == factionGoal_JoinFleet.assignedFleet;
		}

		// Token: 0x06003093 RID: 12435 RVA: 0x00107906 File Offset: 0x00105B06
		public override bool ValidNewGoal()
		{
			return this.targetFleet != null && this.targetFleet.faction == this.faction;
		}

		// Token: 0x06003094 RID: 12436 RVA: 0x0010792E File Offset: 0x00105B2E
		public override bool GoalFulfilled()
		{
			return base.assignedFleet == null || base.assignedFleet == this.targetFleet;
		}

		// Token: 0x06003095 RID: 12437 RVA: 0x00107951 File Offset: 0x00105B51
		public override bool ShouldDiscardGoal()
		{
			return base.assignedFleet == null || this.targetFleet == null || base.importance <= 0 || this.targetFleet.faction != this.faction;
		}

		// Token: 0x06003096 RID: 12438 RVA: 0x00107990 File Offset: 0x00105B90
		public override bool InProgress()
		{
			return base.assignedFleet != null && this.targetFleet != null && base.assignedFleet.transferAssigned;
		}

		// Token: 0x06003097 RID: 12439 RVA: 0x001079BC File Offset: 0x00105BBC
		public override void ChangeTarget(TIGameState newTarget)
		{
			FactionGoal_Fleet factionGoal_Fleet = this.targetFleet.AssignedGoal();
			if (factionGoal_Fleet != null)
			{
				factionGoal_Fleet.RemovePendingFleet(base.assignedFleet);
			}
			this.targetFleet = ((newTarget != null) ? newTarget.ref_fleet : null);
			if (this.targetFleet != null)
			{
				this.targetFleetGoal = this.targetFleet.AssignedGoal();
				FactionGoal_Fleet targetFleetGoal = this.targetFleetGoal;
				if (targetFleetGoal == null)
				{
					return;
				}
				targetFleetGoal.AddPendingFleet(base.assignedFleet);
			}
		}

		// Token: 0x1700073A RID: 1850
		// (get) Token: 0x06003098 RID: 12440 RVA: 0x00107A2E File Offset: 0x00105C2E
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_JoinFleet.incompatibleFleetGoals;
			}
		}

		// Token: 0x1700073B RID: 1851
		// (get) Token: 0x06003099 RID: 12441 RVA: 0x00107A35 File Offset: 0x00105C35
		public override List<Type> fleetOperations
		{
			get
			{
				return new List<Type>(FactionGoal_Fleet.coreFleetOpsList) { typeof(MergeFleetOperation) };
			}
		}

		// Token: 0x0600309A RID: 12442 RVA: 0x00107A51 File Offset: 0x00105C51
		public override bool NeedsShipsOrdered()
		{
			return false;
		}

		// Token: 0x0600309B RID: 12443 RVA: 0x00107A54 File Offset: 0x00105C54
		public override void OnGoalDiscarded()
		{
			if (TIGameState.Valid(this.targetFleetGoal) && !this.targetFleetGoal.ShouldDiscardGoal() && this.targetFleetGoal.CanUseFleet() && base.assignedFleet != null)
			{
				if (this.targetFleetGoal.LookingForFleet() && base.assignedFleet.CanFulfillGoal(this, false))
				{
					this.targetFleetGoal.AssignFleet(base.assignedFleet);
					return;
				}
				if (this.targetFleetGoal.assignedFleet != null && this.targetFleetGoal.NeedsShipsOrdered())
				{
					this.faction.AddGoal(new FactionGoal_JoinFleet(this.faction, this.targetFleetGoal.assignedFleet), HandleDuplicateGoalRule.Ignore, base.assignedFleet);
				}
			}
		}

		// Token: 0x0400226A RID: 8810
		private static readonly List<GoalType> incompatibleFleetGoals = new List<GoalType>();
	}
}
