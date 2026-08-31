using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A48 RID: 2632
	public class AssignOrbitalTransfer : PlayerAction
	{
		// Token: 0x060064DA RID: 25818 RVA: 0x002FA011 File Offset: 0x002F8211
		public AssignOrbitalTransfer(TISpaceFleetState fleet, IOrbitalTransfer transfer, TISpaceObjectState target)
		{
			this.fleetID = fleet.ID;
			this.transfer = transfer;
			this.targetID = target.ID;
		}

		// Token: 0x060064DB RID: 25819 RVA: 0x002FA038 File Offset: 0x002F8238
		public override void Execute()
		{
			TISpaceFleetState state = this.fleetID.GetState<TISpaceFleetState>(false);
			this.targetID.GetState<TISpaceObjectState>(true);
			state.barycenter = this.transfer.GetBarycenter();
			GameControl.eventManager.TriggerEvent(new OrbitChangedEvent(state), null, new object[] { state });
		}

		// Token: 0x060064DC RID: 25820 RVA: 0x002FA08C File Offset: 0x002F828C
		private TIDateTime BinarySearchFindDateTime(IOrbitalTransfer transfer, TISpaceObjectState target, double targetDistance)
		{
			TIDateTime tidateTime = new TIDateTime(transfer.GetTransferEndTime());
			CartesianState cartesianState = transfer.ToCartesianStateAtTime(tidateTime);
			TIDateTime tidateTime2 = new TIDateTime(transfer.GetTransferEndTime());
			double num = -2.0 * targetDistance / cartesianState.velocity.magnitude;
			tidateTime2.AddSeconds(num);
			double num2;
			for (num2 = this.DeltaDistance(transfer, target, tidateTime2); num2 < targetDistance; num2 = this.DeltaDistance(transfer, target, tidateTime2))
			{
				num -= targetDistance / cartesianState.velocity.magnitude;
				tidateTime2.AddSeconds(-targetDistance / cartesianState.velocity.magnitude);
			}
			double num3 = num / 2.0;
			tidateTime.AddSeconds(num3);
			double num4 = 0.0;
			int num5 = 0;
			num2 = this.DeltaDistance(transfer, target, tidateTime);
			while (Mathd.Abs(num2 - targetDistance) > 1.0 && num4 - num > 0.0005 && num5++ < 100)
			{
				if (num2 > targetDistance)
				{
					num = num3;
				}
				else
				{
					num4 = num3;
				}
				num3 = (num + num4) / 2.0;
				tidateTime.CopyDateTime(transfer.GetTransferEndTime());
				tidateTime.AddSeconds(num3);
				num2 = this.DeltaDistance(transfer, target, tidateTime);
			}
			if (num5 >= 100)
			{
				Debug.LogError("Failure to converge in binary search for interception date time");
			}
			return tidateTime;
		}

		// Token: 0x060064DD RID: 25821 RVA: 0x002FA1D4 File Offset: 0x002F83D4
		private double DeltaDistance(IOrbitalTransfer transfer, TISpaceObjectState target, TIDateTime time)
		{
			ref CartesianState ptr = transfer.ToCartesianStateAtTime(time);
			CartesianState cartesianState = target.ToGlobalCartesianStateAtTime(time);
			return (ptr.position - cartesianState.position).magnitude;
		}

		// Token: 0x040046EC RID: 18156
		private GameStateID fleetID;

		// Token: 0x040046ED RID: 18157
		private GameStateID targetID;

		// Token: 0x040046EE RID: 18158
		private readonly IOrbitalTransfer transfer;
	}
}
