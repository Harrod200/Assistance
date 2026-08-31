using System;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A6D RID: 2669
	public class DeleteFleetAlarm : PlayerAction
	{
		// Token: 0x06006528 RID: 25896 RVA: 0x002FB696 File Offset: 0x002F9896
		public DeleteFleetAlarm(TIFactionState faction, TIGameState target)
		{
			this.factionID = faction.ID;
			this.targetID = target.ID;
		}

		// Token: 0x06006529 RID: 25897 RVA: 0x002FB6B8 File Offset: 0x002F98B8
		public override void Execute()
		{
			TIFactionState state = this.factionID.GetState<TIFactionState>(false);
			TIGameState state2 = this.targetID.GetState<TIGameState>(true);
			foreach (Alarm alarm in state.alarms.ToList<Alarm>())
			{
				if (alarm.associatedGameState == state2)
				{
					state.alarms.Remove(alarm);
				}
			}
		}

		// Token: 0x04004752 RID: 18258
		private GameStateID factionID;

		// Token: 0x04004753 RID: 18259
		private GameStateID targetID;
	}
}
