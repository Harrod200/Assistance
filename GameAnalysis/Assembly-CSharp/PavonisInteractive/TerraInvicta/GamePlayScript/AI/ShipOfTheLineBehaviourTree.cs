using System;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.GamePlayScript.PathFinding;
using PavonisInteractive.TerraInvicta.SpaceCombat;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A46 RID: 2630
	public class ShipOfTheLineBehaviourTree : CombatShipBehaviourTree
	{
		// Token: 0x060064CD RID: 25805 RVA: 0x002F8C66 File Offset: 0x002F6E66
		private ShipOfTheLineBehaviourTree()
		{
		}

		// Token: 0x060064CE RID: 25806 RVA: 0x002F8C70 File Offset: 0x002F6E70
		public ShipOfTheLineBehaviourTree(Pathfinding pathfinder, CombatFleetController fleetController, [Nullable(2)] CombatFleetController opposingFleetController, CombatSquadronController squadronController, float secondsBetweenWaypoints, CombatShipBehaviourTree.SharedBehaviourData.FleetPriority priority, TIDateTime time, CombatShipController shipController, CombatHabModuleController[] habControllers)
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
			this._localData.SecondsPerTrajectoryUpdate = 181;
			this._localData.TargetHeadingTestAngle = 45f;
			this.CreateTree();
		}

		// Token: 0x060064CF RID: 25807 RVA: 0x002F8D28 File Offset: 0x002F6F28
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
								new AssessTargetClusterIsWithinRange(base.SharedData, this._localData),
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
					new SequenceBranchNode(base.SharedData, this._localData, new ITreeNode[]
					{
						new AssessDisengageOptionLeafNode(base.SharedData, this._localData),
						new LeaveSquadronLeafNode(base.SharedData, this._localData),
						new InverseResultDecoratorNode(base.SharedData, this._localData, new ITreeNode[]
						{
							new IsThreatenedBranchNode(base.SharedData, this._localData, new ITreeNode[]
							{
								new KeepNoseToThreatLeafNode(base.SharedData, this._localData)
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
					new ShipDoesNotHaveSquadron_OR_ShipIsSquadronLeader_BranchNode(base.SharedData, this._localData, new ITreeNode[]
					{
						new SelectBranchNode(base.SharedData, this._localData, new ITreeNode[]
						{
							new IsThreatenedBranchNode(base.SharedData, this._localData, new ITreeNode[]
							{
								new KeepNoseToThreatLeafNode(base.SharedData, this._localData)
							}),
							new SelectBranchNode(base.SharedData, this._localData, new ITreeNode[]
							{
								new SequenceBranchNode(base.SharedData, this._localData, new ITreeNode[]
								{
									new SelectBranchNode(base.SharedData, this._localData, new ITreeNode[]
									{
										new AssessShipIsClosingWithTargetAtSpeedLeafNode(base.SharedData, this._localData),
										new AssessTargetIsInEffectiveRangeLeafNode(base.SharedData, this._localData)
									}),
									new RotateToTargetLeafNode(base.SharedData, this._localData)
								}),
								new FleetIsAttemptingToEscapeBranchNode(base.SharedData, this._localData, new ITreeNode[]
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
									})
								}),
								new SelectBranchNode(base.SharedData, this._localData, new ITreeNode[]
								{
									new SequenceBranchNode(base.SharedData, this._localData, new ITreeNode[]
									{
										new AssessShipIsHeadedInDirectionOfTargetLeafNode(base.SharedData, this._localData),
										new ShipDoesNotHaveSquadron_OR_SquadronIsReadyToMove_BranchNode(base.SharedData, this._localData, new ITreeNode[]
										{
											new UpdateFullSpeedAheadTrajectoryLeafNode(base.SharedData, this._localData)
										})
									}),
									new ShipIsMovingAwayFromTargetAtSpeedBranchNode(base.SharedData, this._localData, new ITreeNode[]
									{
										new ShipDoesNotHaveSquadron_OR_SquadronIsReadyToMove_BranchNode(base.SharedData, this._localData, new ITreeNode[]
										{
											new SetBrakingTrajectoryLeafNode(base.SharedData, this._localData)
										})
									}),
									new ShipDoesNotHaveSquadron_OR_SquadronIsReadyToMove_BranchNode(base.SharedData, this._localData, new ITreeNode[]
									{
										new UpdateInterceptCourseTrajectoryLeafNode(base.SharedData, this._localData)
									})
								})
							})
						})
					}),
					new SelectBranchNode(base.SharedData, this._localData, new ITreeNode[]
					{
						new SequenceBranchNode(base.SharedData, this._localData, new ITreeNode[]
						{
							new AssessTargetIsInEffectiveRangeLeafNode(base.SharedData, this._localData),
							new RotateToTargetLeafNode(base.SharedData, this._localData),
							new RemoveShipFromSquadronTrajectoryMatchedShipsLeafNode(base.SharedData, this._localData)
						}),
						new ShipHasMatchedSquadronLeaderBranchNode(base.SharedData, this._localData, new ITreeNode[]
						{
							new FollowSquadronLeaderTrajectoryLeafNode(base.SharedData, this._localData)
						}),
						new MatchSquadronLeaderTrajectoryLeafNode(base.SharedData, this._localData)
					})
				}),
				new CheckRadiatorsLeafNode(base.SharedData, this._localData)
			});
		}
	}
}
