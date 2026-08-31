using System;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.GamePlayScript.PathFinding;
using PavonisInteractive.TerraInvicta.SpaceCombat;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A1C RID: 2588
	public abstract class CombatShipBehaviourTree
	{
		// Token: 0x17001119 RID: 4377
		// (get) Token: 0x0600643C RID: 25660 RVA: 0x002F48A0 File Offset: 0x002F2AA0
		// (set) Token: 0x0600643B RID: 25659 RVA: 0x002F4897 File Offset: 0x002F2A97
		public CombatShipBehaviourTree.SharedBehaviourData SharedData { get; protected set; }

		// Token: 0x0600643D RID: 25661 RVA: 0x002F48A8 File Offset: 0x002F2AA8
		protected CombatShipBehaviourTree()
		{
		}

		// Token: 0x0600643E RID: 25662 RVA: 0x002F48B0 File Offset: 0x002F2AB0
		public CombatShipBehaviourTree(Pathfinding pathfinder, CombatFleetController fleetController, [Nullable(2)] CombatFleetController opposingFleetController, float secondsBetweenWaypoints, CombatShipBehaviourTree.SharedBehaviourData.FleetPriority priority, TIDateTime time, CombatShipController shipController, CombatHabModuleController[] habControllers)
		{
			this.SharedData = new CombatShipBehaviourTree.SharedBehaviourData
			{
				PathFinder = pathfinder,
				FleetController = fleetController,
				OpposingFleetController = opposingFleetController,
				SecondsBetweenWaypoints = secondsBetweenWaypoints,
				Priority = priority,
				CurrentTime = time,
				FactionState = shipController.faction,
				ShipController = shipController,
				HabModuleControllers = habControllers
			};
			this._localData = new CombatShipBehaviourTree.LocalBehaviourData();
			this.CreateTree();
		}

		// Token: 0x0600643F RID: 25663 RVA: 0x002F4929 File Offset: 0x002F2B29
		public void Update(TIDateTime time)
		{
			this.SharedData.CurrentTime = time;
			this._rootNode.Execute();
		}

		// Token: 0x06006440 RID: 25664
		protected abstract void CreateTree();

		// Token: 0x040046BF RID: 18111
		protected RootNode _rootNode;

		// Token: 0x040046C1 RID: 18113
		protected CombatShipBehaviourTree.LocalBehaviourData _localData;

		// Token: 0x020013C3 RID: 5059
		public enum ConditionResponse
		{
			// Token: 0x040072C2 RID: 29378
			Failed,
			// Token: 0x040072C3 RID: 29379
			Running,
			// Token: 0x040072C4 RID: 29380
			Success
		}

		// Token: 0x020013C4 RID: 5060
		public class SharedBehaviourData
		{
			// Token: 0x040072C5 RID: 29381
			public Pathfinding PathFinder;

			// Token: 0x040072C6 RID: 29382
			public CombatFleetController FleetController;

			// Token: 0x040072C7 RID: 29383
			public CombatFleetController OpposingFleetController;

			// Token: 0x040072C8 RID: 29384
			public float SecondsBetweenWaypoints;

			// Token: 0x040072C9 RID: 29385
			public CombatShipBehaviourTree.SharedBehaviourData.FleetPriority Priority;

			// Token: 0x040072CA RID: 29386
			public TIDateTime CurrentTime;

			// Token: 0x040072CB RID: 29387
			public TIFactionState FactionState;

			// Token: 0x040072CC RID: 29388
			public CombatShipController ShipController;

			// Token: 0x040072CD RID: 29389
			public CombatHabModuleController[] HabModuleControllers;

			// Token: 0x040072CE RID: 29390
			public float MinimumDVThreshold = 2.5f;

			// Token: 0x020013F6 RID: 5110
			public enum FleetPriority
			{
				// Token: 0x0400735C RID: 29532
				Aggression,
				// Token: 0x0400735D RID: 29533
				Defensive,
				// Token: 0x0400735E RID: 29534
				BestForShip
			}
		}

		// Token: 0x020013C5 RID: 5061
		public class LocalBehaviourData
		{
			// Token: 0x040072CF RID: 29391
			public CombatShipController TargetShip;

			// Token: 0x040072D0 RID: 29392
			public CombatHabModuleController TargetModule;

			// Token: 0x040072D1 RID: 29393
			public CombatSquadronController SquadronController;

			// Token: 0x040072D2 RID: 29394
			public float MinimumScaledCombatRange = SpaceCombatManager.km_to_scale(200f);

			// Token: 0x040072D3 RID: 29395
			public CombatTargetPriority TargetType = CombatTargetPriority.Strongest;

			// Token: 0x040072D4 RID: 29396
			public bool CombatReady;

			// Token: 0x040072D5 RID: 29397
			public bool IsRammingTargetShip;

			// Token: 0x040072D6 RID: 29398
			public bool IsDisengaging;

			// Token: 0x040072D7 RID: 29399
			public int SecondsPerTrajectoryUpdate = 61;

			// Token: 0x040072D8 RID: 29400
			public float TargetHeadingTestAngle = 7.5f;
		}
	}
}
