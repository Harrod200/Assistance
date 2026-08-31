using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A9C RID: 2716
	public class SetUserAlarmAction : PlayerAction
	{
		// Token: 0x0600658D RID: 25997 RVA: 0x002FD3E6 File Offset: 0x002FB5E6
		public SetUserAlarmAction(TIFactionState faction, TIGameState targetState, AlarmType alarm, TIDateTime dateTime, string userString)
		{
			this.factionID = faction.ID;
			this.targetID = targetState.ID;
			this.alarm = alarm;
			this.dateTime = dateTime;
			this.userString = userString;
		}

		// Token: 0x0600658E RID: 25998 RVA: 0x002FD420 File Offset: 0x002FB620
		public override void Execute()
		{
			TIFactionState state = this.factionID.GetState<TIFactionState>(false);
			TIGameState state2 = this.targetID.GetState<TIGameState>(true);
			state.SetAlarm(state2, null, this.dateTime, this.alarm, this.userString);
		}

		// Token: 0x040047DB RID: 18395
		private GameStateID factionID;

		// Token: 0x040047DC RID: 18396
		private GameStateID targetID;

		// Token: 0x040047DD RID: 18397
		private TIDateTime dateTime;

		// Token: 0x040047DE RID: 18398
		private AlarmType alarm;

		// Token: 0x040047DF RID: 18399
		private string userString;
	}
}
