using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.SolarSystem
{
	// Token: 0x020009A1 RID: 2465
	[UpdateInGroup(typeof(PipelineStages.RenderStage))]
	public class SpaceObjectPositioning : StrategyLayerComponentSystem
	{
		// Token: 0x06005CF6 RID: 23798 RVA: 0x002C4DA1 File Offset: 0x002C2FA1
		public void ResetCounts()
		{
			this.oldSpaceObjectGroupLength = 0;
			this.oldTransfersLength = 0;
			this.oldFleetGroupLength = 0;
		}

		// Token: 0x06005CF7 RID: 23799 RVA: 0x002C4DB8 File Offset: 0x002C2FB8
		public void TriggerForceUpdate()
		{
			this.forceUpdate = true;
			base.UpdateInjectedComponentGroups();
		}

		// Token: 0x06005CF8 RID: 23800 RVA: 0x002C4DC8 File Offset: 0x002C2FC8
		protected override void OnUpdate()
		{
			if (this.spaceObjectGroup.Length != this.oldSpaceObjectGroupLength || this.fleetTransfers.Length != this.oldTransfersLength || this.fleet.Length != this.oldFleetGroupLength || this.cameraManager.ForceVisualizationUpdate)
			{
				this.forceUpdate = true;
			}
			this.now = TITimeState.Now();
			bool flag = false;
			if (this.forceUpdate || this.now != this.lastUpdateDate)
			{
				this.oldTransfersLength = this.fleetTransfers.Length;
				List<ValueTuple<TransferPlanComponent, SpaceObjectComponent>> list = (from x in Enumerable.Range(0, this.fleetTransfers.Length)
					select new ValueTuple<TransferPlanComponent, SpaceObjectComponent>(this.fleetTransfers.TransferPlan[x], this.fleetTransfers.SpaceObject[x])).ToList<ValueTuple<TransferPlanComponent, SpaceObjectComponent>>();
				this.arrivals.Clear();
				foreach (ValueTuple<TransferPlanComponent, SpaceObjectComponent> valueTuple in list)
				{
					if (!(valueTuple.Item1 == null))
					{
						FleetTransferPlan value = valueTuple.Item1.Value;
						if (!value.planningOnly && value.fleet != null && value.fleet.trajectory != null)
						{
							bool flag2;
							valueTuple.Item2.Value.Position = value.fleet.trajectory.PositionAtTime(this.now, true, out flag2);
							if (!flag2 && value.fleet.trajectory.arrivalTime < this.now)
							{
								Debug.LogWarning("A fleet's arrival time has passed, but its trajectory.PositionAtTime(bool arrived) returned false.  This is inconsistent and would result in the fleet's trajectory leaking.  To avoid this, the fleet will arrive now.");
								flag2 = true;
							}
							if (flag2 && value.fleet.trajectory.launched)
							{
								this.arrivals.Add(value.fleet, value.fleet.trajectory.arrivalTime);
								flag = true;
							}
						}
					}
				}
				foreach (KeyValuePair<TISpaceFleetState, TIDateTime> keyValuePair in this.arrivals.OrderBy<KeyValuePair<TISpaceFleetState, TIDateTime>, TIDateTime>((KeyValuePair<TISpaceFleetState, TIDateTime> x) => x.Value))
				{
					keyValuePair.Key.ArriveFleet(false);
				}
				if (this.oldFleetGroupLength != this.fleet.FleetObject.Length || this.oldSpaceObjectGroupLength != this.spaceObjectGroup.Length || flag || this.forceUpdate)
				{
					base.UpdateInjectedComponentGroups();
				}
				this.oldFleetGroupLength = this.fleet.FleetObject.Length;
				for (int i = 0; i < this.fleet.FleetObject.Length; i++)
				{
					TISpaceFleetState tispaceFleetState = this.fleet.FleetObject[i].Fleet;
					if (!(tispaceFleetState == null))
					{
						foreach (TISpaceShipState tispaceShipState in tispaceFleetState.ships)
						{
							tispaceShipState.UpdateCurrentManeuver();
						}
					}
				}
				for (int j = 0; j < this.lagrangePointGroup.Length; j++)
				{
					this.lagrangePointGroup.SpaceObject[j].Value.Position = this.lagrangePointGroup.Navigable[j].State.GetGlobalPositionAtTime(this.now);
				}
				for (int k = 0; k < this.spaceObjectGroup.Length; k++)
				{
					if (!(this.spaceObjectGroup.SpaceObject[k] == null) && !(this.spaceObjectGroup.SpaceObject[k].State == null))
					{
						SpaceObject value2 = this.spaceObjectGroup.SpaceObject[k].Value;
						if (value2.ObjectType != SpaceObjectType.Fleet || (!this.spaceObjectGroup.SpaceObject[k].State.ref_fleet.inTransfer && this.spaceObjectGroup.SpaceObject[k].State.ref_fleet.ships.Count != 0))
						{
							value2.Position = this.spaceObjectGroup.SpaceObject[k].State.GetGlobalPositionAtTime(this.now);
						}
						this.spaceObjectGroup.SpaceObject[k].Value = value2;
					}
				}
				this.oldSpaceObjectGroupLength = this.spaceObjectGroup.Length;
				this.forceUpdate = false;
				this.lastUpdateDate = this.now;
			}
		}

		// Token: 0x06005CF9 RID: 23801 RVA: 0x002C527C File Offset: 0x002C347C
		private static double EaseInOutSine(double value, double start = 0.0, double end = 0.0)
		{
			end -= start;
			return -end * 0.5 * (Mathd.Cos(3.141592653589793 * value) - 1.0) + start;
		}

		// Token: 0x04004277 RID: 17015
		[Inject]
		private SpaceObjectPositioning.SpaceObjectGroup spaceObjectGroup;

		// Token: 0x04004278 RID: 17016
		[Inject]
		private SpaceObjectPositioning.NavigableGroup lagrangePointGroup;

		// Token: 0x04004279 RID: 17017
		[Inject]
		private SpaceObjectPositioning.FleetTransferGroup fleetTransfers;

		// Token: 0x0400427A RID: 17018
		[Inject]
		private SpaceObjectPositioning.FleetGroup fleet;

		// Token: 0x0400427B RID: 17019
		[Inject]
		private GameTimeManager gameTime;

		// Token: 0x0400427C RID: 17020
		[Inject]
		private CameraManager cameraManager;

		// Token: 0x0400427D RID: 17021
		private bool forceUpdate = true;

		// Token: 0x0400427E RID: 17022
		private int oldSpaceObjectGroupLength;

		// Token: 0x0400427F RID: 17023
		private int oldTransfersLength;

		// Token: 0x04004280 RID: 17024
		private int oldFleetGroupLength;

		// Token: 0x04004281 RID: 17025
		private TIDateTime now;

		// Token: 0x04004282 RID: 17026
		private TIDateTime lastUpdateDate;

		// Token: 0x04004283 RID: 17027
		private Dictionary<TISpaceFleetState, TIDateTime> arrivals = new Dictionary<TISpaceFleetState, TIDateTime>();

		// Token: 0x02001348 RID: 4936
		private struct SpaceObjectGroup
		{
			// Token: 0x04006F94 RID: 28564
			public readonly int Length;

			// Token: 0x04006F95 RID: 28565
			[ReadOnly]
			public ComponentArray<OrbitComponent> Orbit;

			// Token: 0x04006F96 RID: 28566
			public ComponentArray<SpaceObjectComponent> SpaceObject;

			// Token: 0x04006F97 RID: 28567
			private SubtractiveComponent<NavigableComponent> _;

			// Token: 0x04006F98 RID: 28568
			private SubtractiveComponent<TransferPlanComponent> _transferPlan;
		}

		// Token: 0x02001349 RID: 4937
		private struct NavigableGroup
		{
			// Token: 0x04006F99 RID: 28569
			public readonly int Length;

			// Token: 0x04006F9A RID: 28570
			[ReadOnly]
			public ComponentArray<OrbitComponent> Orbit;

			// Token: 0x04006F9B RID: 28571
			[ReadOnly]
			public ComponentArray<NavigableComponent> Navigable;

			// Token: 0x04006F9C RID: 28572
			public ComponentArray<SpaceObjectComponent> SpaceObject;
		}

		// Token: 0x0200134A RID: 4938
		private struct FleetTransferGroup
		{
			// Token: 0x04006F9D RID: 28573
			public readonly int Length;

			// Token: 0x04006F9E RID: 28574
			public GameObjectArray GameObject;

			// Token: 0x04006F9F RID: 28575
			[ReadOnly]
			public ComponentArray<TransferPlanComponent> TransferPlan;

			// Token: 0x04006FA0 RID: 28576
			public ComponentArray<SpaceObjectComponent> SpaceObject;
		}

		// Token: 0x0200134B RID: 4939
		private struct FleetGroup
		{
			// Token: 0x04006FA1 RID: 28577
			public readonly int Length;

			// Token: 0x04006FA2 RID: 28578
			[ReadOnly]
			public ComponentArray<FleetComponent> FleetObject;

			// Token: 0x04006FA3 RID: 28579
			public ComponentArray<SpaceObjectComponent> SpaceObject;
		}
	}
}
