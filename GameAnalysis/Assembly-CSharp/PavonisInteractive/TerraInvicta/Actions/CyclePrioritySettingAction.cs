using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A69 RID: 2665
	public class CyclePrioritySettingAction : PlayerAction
	{
		// Token: 0x06006520 RID: 25888 RVA: 0x002FB548 File Offset: 0x002F9748
		public CyclePrioritySettingAction(TIControlPoint controlPoint, TIFactionState faction, PriorityType priority, bool decrement)
		{
			this.controlPointID = controlPoint.ID;
			this.factionID = faction.ID;
			this.priority = priority;
			this.decrement = decrement;
		}

		// Token: 0x06006521 RID: 25889 RVA: 0x002FB578 File Offset: 0x002F9778
		public override void Execute()
		{
			TIControlPoint state = this.controlPointID.GetState<TIControlPoint>(false);
			TIFactionState state2 = this.factionID.GetState<TIFactionState>(false);
			if (state.faction == state2)
			{
				if (this.decrement)
				{
					state.DecrementControlPointPriority(this.priority);
					return;
				}
				state.IncrementControlPointPriority(this.priority);
			}
		}

		// Token: 0x04004749 RID: 18249
		private GameStateID controlPointID;

		// Token: 0x0400474A RID: 18250
		private GameStateID factionID;

		// Token: 0x0400474B RID: 18251
		private PriorityType priority;

		// Token: 0x0400474C RID: 18252
		private bool decrement;
	}
}
