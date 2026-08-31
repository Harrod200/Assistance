using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A96 RID: 2710
	public class SetNationAutoAbandon : PlayerAction
	{
		// Token: 0x06006581 RID: 25985 RVA: 0x002FD1A2 File Offset: 0x002FB3A2
		public SetNationAutoAbandon(TIFactionState faction, TINationState nation, bool newSetting)
		{
			this.factionID = faction.ID;
			this.nationID = nation.ID;
			this.newSetting = newSetting;
		}

		// Token: 0x06006582 RID: 25986 RVA: 0x002FD1CC File Offset: 0x002FB3CC
		public override void Execute()
		{
			TINationState state = this.nationID.GetState<TINationState>(false);
			this.factionID.GetState<TIFactionState>(false).SetPermaAbandonNationStatus(state, this.newSetting);
		}

		// Token: 0x040047C6 RID: 18374
		private GameStateID factionID;

		// Token: 0x040047C7 RID: 18375
		private GameStateID nationID;

		// Token: 0x040047C8 RID: 18376
		private readonly bool newSetting;
	}
}
