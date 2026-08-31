using System;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.GamePlayScript.PathFinding;
using PavonisInteractive.TerraInvicta.SpaceCombat;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A21 RID: 2593
	public class InterceptorBehaviourTree : CombatShipBehaviourTree
	{
		// Token: 0x0600644D RID: 25677 RVA: 0x002F49E5 File Offset: 0x002F2BE5
		private InterceptorBehaviourTree()
		{
		}

		// Token: 0x0600644E RID: 25678 RVA: 0x002F49F0 File Offset: 0x002F2BF0
		public InterceptorBehaviourTree(Pathfinding pathfinder, CombatFleetController fleetController, [Nullable(2)] CombatFleetController opposingFleetController, CombatSquadronController squadronController, float secondsBetweenWaypoints, CombatShipBehaviourTree.SharedBehaviourData.FleetPriority priority, TIDateTime time, CombatShipController shipController, CombatHabModuleController[] habControllers)
		{
			base.SharedData = new CombatShipBehaviourTree.SharedBehaviourData
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
			this._localData.SquadronController = squadronController;
			this._localData.MinimumScaledCombatRange = shipController.GetShipEffectiveScaledCombatRange();
			this._localData.SecondsPerTrajectoryUpdate = 121;
			this._localData.TargetHeadingTestAngle = 45f;
			this.CreateTree();
		}

		// Token: 0x0600644F RID: 25679 RVA: 0x002F4AA8 File Offset: 0x002F2CA8
		protected override void CreateTree()
		{
			this._rootNode = new StructureTestRootNode(base.SharedData, this._localData, new ITreeNode[]
			{
				new SelectRunningOrSuccessBranchNode(base.SharedData, this._localData, new ITreeNode[]
				{
					new AssessShipIsMissileCarrierBranchNode(base.SharedData, this._localData, new ITreeNode[]
					{
						new SelectBranchNode(base.SharedData, this._localData, new ITreeNode[]
						{
							new SequenceBranchNode(base.SharedData, this._localData, new ITreeNode[]
							{
								new FindMissileTargetShipLeafNode(base.SharedData, this._localData),
								new AssessShipIsClosingWithTargetLeafNode(base.SharedData, this._localData),
								new FireMissilesLeafNode(base.SharedData, this._localData)
							}),
							new SequenceBranchNode(base.SharedData, this._localData, new ITreeNode[]
							{
								new IdleMissilesLeafNode(base.SharedData, this._localData),
								new FindTargetShipLeafNode(base.SharedData, this._localData)
							})
						})
					}),
					new FindTargetShipLeafNode(base.SharedData, this._localData)
				}),
				new SelectBranchNode(base.SharedData, this._localData, new ITreeNode[]
				{
					new HasCombatBeenJoinedBranchNode(base.SharedData, this._localData, new ITreeNode[]
					{
						new SequenceBranchNode(base.SharedData, this._localData, new ITreeNode[]
						{
							new AssessDisengageOptionLeafNode(base.SharedData, this._localData),
							new LeaveSquadronLeafNode(base.SharedData, this._localData),
							new InverseResultDecoratorNode(base.SharedData, this._localData, new ITreeNode[]
							{
								new IsThreatenedBranchNode(base.SharedData, this._localData, new ITreeNode[]
								{
									new UpdateDefensiveTrajectoryLeafNode(base.SharedData, this._localData)
								})
							}),
							new SelectBranchNode(base.SharedData, this._localData, new ITreeNode[]
							{
								new SequenceBranchNode(base.SharedData, this._localData, new ITreeNode[]
								{
									new InverseResultDecoratorNode(base.SharedData, this._localData, new ITreeNode[]
									{
										new AssessShipIsPointedTowardsEnemyFleetCenterOfMassLeafNode(base.SharedData, this._localData)
									}),
									new InverseResultDecoratorNode(base.SharedData, this._localData, new ITreeNode[]
									{
										new AssessShipIsAtDisengagementSpeedLeafNode(base.SharedData, this._localData)
									}),
									new UpdateFullSpeedAheadTrajectoryLeafNode(base.SharedData, this._localData)
								}),
								new SequenceBranchNode(base.SharedData, this._localData, new ITreeNode[]
								{
									new InverseResultDecoratorNode(base.SharedData, this._localData, new ITreeNode[]
									{
										new AssessIfShipIsInTargetsRangeLeafNode(base.SharedData, this._localData)
									}),
									new RotateAwayFromEnemyCenterOfMassLeafNode(base.SharedData, this._localData)
								}),
								new DoNothingLeafNode(base.SharedData, this._localData)
							})
						}),
						new SelectBranchNode(base.SharedData, this._localData, new ITreeNode[]
						{
							new ShipDoesNotHaveSquadron_OR_ShipIsSquadronLeader_BranchNode(base.SharedData, this._localData, new ITreeNode[]
							{
								new ShipManeuverabilityIsSignificantlyDimishedBranchNode(base.SharedData, this._localData, new ITreeNode[]
								{
									new LeaveSquadronLeafNode(base.SharedData, this._localData)
								}),
								new SelectBranchNode(base.SharedData, this._localData, new ITreeNode[]
								{
									new IsThreatenedBranchNode(base.SharedData, this._localData, new ITreeNode[]
									{
										new UpdateDefensiveTrajectoryLeafNode(base.SharedData, this._localData)
									}),
									new SequenceBranchNode(base.SharedData, this._localData, new ITreeNode[]
									{
										new AssessTargetIsInEffectiveRangeLeafNode(base.SharedData, this._localData),
										new RotateToTargetLeafNode(base.SharedData, this._localData)
									}),
									new SequenceBranchNode(base.SharedData, this._localData, new ITreeNode[]
									{
										new InverseResultDecoratorNode(base.SharedData, this._localData, new ITreeNode[]
										{
											new AssessTargetIsInEffectiveRangeLeafNode(base.SharedData, this._localData)
										}),
										new InverseResultDecoratorNode(base.SharedData, this._localData, new ITreeNode[]
										{
											new AssessShipIsClosingWithTargetAtSpeedLeafNode(base.SharedData, this._localData)
										}),
										new UpdateInterceptCourseTrajectoryLeafNode(base.SharedData, this._localData)
									}),
									new UpdateInterceptorTrajectoryLeafNode(base.SharedData, this._localData)
								})
							}),
							new SequenceBranchNode(base.SharedData, this._localData, new ITreeNode[]
							{
								new FollowSquadronLeaderTrajectoryLeafNode(base.SharedData, this._localData),
								new ShipManeuverabilityIsSignificantlyDimishedBranchNode(base.SharedData, this._localData, new ITreeNode[]
								{
									new LeaveSquadronLeafNode(base.SharedData, this._localData)
								})
							})
						})
					})
				}),
				new CheckRadiatorsLeafNode(base.SharedData, this._localData)
			});
		}
	}
}
