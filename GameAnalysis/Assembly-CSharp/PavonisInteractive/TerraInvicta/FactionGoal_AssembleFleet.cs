using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000750 RID: 1872
	public class FactionGoal_AssembleFleet : FactionGoal_Fleet
	{
		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x0600305B RID: 12379 RVA: 0x00107434 File Offset: 0x00105634
		// (set) Token: 0x0600305C RID: 12380 RVA: 0x0010743C File Offset: 0x0010563C
		public TISpaceGameState assemblyLocation { get; private set; }

		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x0600305D RID: 12381 RVA: 0x00107445 File Offset: 0x00105645
		// (set) Token: 0x0600305E RID: 12382 RVA: 0x0010744D File Offset: 0x0010564D
		public TISpaceGameState assemblyPermaLocation { get; private set; }

		// Token: 0x17000734 RID: 1844
		// (get) Token: 0x0600305F RID: 12383 RVA: 0x00107456 File Offset: 0x00105656
		// (set) Token: 0x06003060 RID: 12384 RVA: 0x0010745E File Offset: 0x0010565E
		public float maxStrength { get; private set; }

		// Token: 0x17000735 RID: 1845
		// (get) Token: 0x06003061 RID: 12385 RVA: 0x00107467 File Offset: 0x00105667
		// (set) Token: 0x06003062 RID: 12386 RVA: 0x0010746F File Offset: 0x0010566F
		public bool constructionOnly { get; private set; }

		// Token: 0x06003063 RID: 12387 RVA: 0x00107478 File Offset: 0x00105678
		public FactionGoal_AssembleFleet()
		{
		}

		// Token: 0x06003064 RID: 12388 RVA: 0x00107480 File Offset: 0x00105680
		public FactionGoal_AssembleFleet(TIFactionState faction, int importance, TISpaceGameState assemblyLocation, float maxStrength = float.PositiveInfinity, bool constructionOnly = false)
		{
			this.faction = faction;
			this.assemblyLocation = assemblyLocation;
			this.SetPermaLocation();
			this.maxStrength = maxStrength;
			this.constructionOnly = constructionOnly;
			base.SetImportance(importance);
		}

		// Token: 0x06003065 RID: 12389 RVA: 0x001074B3 File Offset: 0x001056B3
		public static FactionGoal_AssembleFleet CreateGoal(FactionGoal_AssembleFleet p)
		{
			FactionGoal_AssembleFleet factionGoal_AssembleFleet = GameStateManager.CreateNewGameState<FactionGoal_AssembleFleet>();
			factionGoal_AssembleFleet.assemblyLocation = p.assemblyLocation;
			factionGoal_AssembleFleet.SetPermaLocation();
			factionGoal_AssembleFleet.maxStrength = p.maxStrength;
			factionGoal_AssembleFleet.constructionOnly = p.constructionOnly;
			return factionGoal_AssembleFleet;
		}

		// Token: 0x06003066 RID: 12390 RVA: 0x001074E4 File Offset: 0x001056E4
		private void SetPermaLocation()
		{
			this.assemblyPermaLocation = this.assemblyLocation.ref_naturalSpaceObject.ref_orbit ?? this.assemblyLocation.ref_naturalSpaceObject.orbits[0];
		}

		// Token: 0x06003067 RID: 12391 RVA: 0x00107516 File Offset: 0x00105716
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_AssembleFleet>(base.ID, false);
		}

		// Token: 0x06003068 RID: 12392 RVA: 0x00107525 File Offset: 0x00105725
		public override GoalType GetGoalType()
		{
			return GoalType.AssembleFleet;
		}

		// Token: 0x06003069 RID: 12393 RVA: 0x00107529 File Offset: 0x00105729
		public override TIGameState actor()
		{
			return base.assignedFleet;
		}

		// Token: 0x0600306A RID: 12394 RVA: 0x00107531 File Offset: 0x00105731
		public override TIGameState target()
		{
			return this.assemblyLocation;
		}

		// Token: 0x0600306B RID: 12395 RVA: 0x00107539 File Offset: 0x00105739
		public override TIGameState location()
		{
			return this.assemblyLocation;
		}

		// Token: 0x0600306C RID: 12396 RVA: 0x00107541 File Offset: 0x00105741
		public override TIGameState goalProduct()
		{
			return base.assignedFleet;
		}

		// Token: 0x0600306D RID: 12397 RVA: 0x00107549 File Offset: 0x00105749
		public override bool ValidNewGoal()
		{
			return this.assemblyLocation != null;
		}

		// Token: 0x0600306E RID: 12398 RVA: 0x00107557 File Offset: 0x00105757
		public override bool InProgress()
		{
			return TIGameState.Valid(base.assignedFleet);
		}

		// Token: 0x0600306F RID: 12399 RVA: 0x00107564 File Offset: 0x00105764
		public override bool ShouldDiscardGoal()
		{
			return base.importance == 0 || (!TIGameState.Valid(this.assemblyLocation) && !TIGameState.Valid(this.assemblyPermaLocation));
		}

		// Token: 0x06003070 RID: 12400 RVA: 0x0010758D File Offset: 0x0010578D
		public override bool GoalFulfilled()
		{
			return false;
		}

		// Token: 0x17000736 RID: 1846
		// (get) Token: 0x06003071 RID: 12401 RVA: 0x00107590 File Offset: 0x00105790
		public override List<Type> fleetOperations
		{
			get
			{
				return FactionGoal_AssembleFleet.fleetOps;
			}
		}

		// Token: 0x06003072 RID: 12402 RVA: 0x00107597 File Offset: 0x00105797
		public override bool RequiresFleet()
		{
			return true;
		}

		// Token: 0x06003073 RID: 12403 RVA: 0x0010759A File Offset: 0x0010579A
		public override ShipRole GetPrimaryShipRole()
		{
			return ShipRole.NoRole;
		}

		// Token: 0x06003074 RID: 12404 RVA: 0x0010759D File Offset: 0x0010579D
		public override float GetDesiredAssaultCombatValue()
		{
			return 0f;
		}

		// Token: 0x06003075 RID: 12405 RVA: 0x001075A4 File Offset: 0x001057A4
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x06003076 RID: 12406 RVA: 0x001075A7 File Offset: 0x001057A7
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return new List<GoalType>();
			}
		}

		// Token: 0x06003077 RID: 12407 RVA: 0x001075AE File Offset: 0x001057AE
		public override Dictionary<ShipRole, float> GetSecondaryShipRoles()
		{
			return FactionGoal_AssembleFleet.preferredShipRoles;
		}

		// Token: 0x06003078 RID: 12408 RVA: 0x001075B5 File Offset: 0x001057B5
		public override void ChangeTarget(TIGameState newTarget)
		{
			if (newTarget != null)
			{
				this.AssignFleet(newTarget.ref_fleet);
				return;
			}
			this.AssignFleet(null);
		}

		// Token: 0x06003079 RID: 12409 RVA: 0x001075D4 File Offset: 0x001057D4
		public override float ComputeDesiredFleetCombatValue()
		{
			if (this.ShouldPerformMissionMinimallyArmed)
			{
				return 0f;
			}
			float num = Mathf.Max(base.ComputeDesiredFleetCombatValue(), this.faction.fleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue()) * 0.33f);
			if (this.maxStrength > 0f)
			{
				num = Mathf.Min(num, this.maxStrength);
			}
			return num;
		}

		// Token: 0x0600307A RID: 12410 RVA: 0x0010764B File Offset: 0x0010584B
		public override void DailyGoalMaintenance()
		{
			if (!TIGameState.Valid(this.assemblyLocation))
			{
				this.assemblyLocation = this.assemblyPermaLocation;
			}
		}

		// Token: 0x04002266 RID: 8806
		public static readonly List<Type> fleetOps = new List<Type>(FactionGoal_Fleet.coreFleetOpsList);

		// Token: 0x04002267 RID: 8807
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
				1f
			},
			{
				ShipRole.MS_Strike,
				1f
			},
			{
				ShipRole.MM_SpaceSuperiority,
				1f
			},
			{
				ShipRole.ML_Standoff,
				1f
			}
		};
	}
}
