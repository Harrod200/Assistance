using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A99 RID: 2713
	public class SetPriorityAction : PlayerAction
	{
		// Token: 0x06006587 RID: 25991 RVA: 0x002FD281 File Offset: 0x002FB481
		public SetPriorityAction(TIControlPoint controlPoint, TIFactionState faction, PriorityType priority, int value, bool onlyIfHigher, bool skipUpdates)
		{
			this.controlPointID = controlPoint.ID;
			this.factionID = faction.ID;
			this.priority = priority;
			this.onlyIfHigher = onlyIfHigher;
			this.value = value;
			this.skipUpdates = skipUpdates;
		}

		// Token: 0x06006588 RID: 25992 RVA: 0x002FD2C0 File Offset: 0x002FB4C0
		public override void Execute()
		{
			TIControlPoint state = this.controlPointID.GetState<TIControlPoint>(false);
			TIFactionState state2 = this.factionID.GetState<TIFactionState>(false);
			if (state.faction == state2)
			{
				if (this.onlyIfHigher)
				{
					int controlPointPriority = state.GetControlPointPriority(this.priority, false);
					this.value = ((this.value > controlPointPriority) ? this.value : controlPointPriority);
				}
				state.SetControlPointPriority(this.priority, this.value, this.skipUpdates, this.skipUpdates, false);
			}
		}

		// Token: 0x040047CF RID: 18383
		private GameStateID controlPointID;

		// Token: 0x040047D0 RID: 18384
		private GameStateID factionID;

		// Token: 0x040047D1 RID: 18385
		private readonly PriorityType priority;

		// Token: 0x040047D2 RID: 18386
		private readonly bool onlyIfHigher;

		// Token: 0x040047D3 RID: 18387
		private int value;

		// Token: 0x040047D4 RID: 18388
		private readonly bool skipUpdates;
	}
}
