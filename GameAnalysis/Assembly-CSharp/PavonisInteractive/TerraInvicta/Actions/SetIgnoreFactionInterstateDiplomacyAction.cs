using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A94 RID: 2708
	public class SetIgnoreFactionInterstateDiplomacyAction : PlayerAction
	{
		// Token: 0x0600657D RID: 25981 RVA: 0x002FD0FA File Offset: 0x002FB2FA
		public SetIgnoreFactionInterstateDiplomacyAction(TIFactionState actingFaction, TIFactionState targetFaction, bool ignore)
		{
			this.actingFactionID = actingFaction.ID;
			this.targetFactionID = targetFaction.ID;
			this.ignore = ignore;
		}

		// Token: 0x0600657E RID: 25982 RVA: 0x002FD124 File Offset: 0x002FB324
		public override void Execute()
		{
			TIFactionState state = this.actingFactionID.GetState<TIFactionState>(false);
			TIFactionState state2 = this.targetFactionID.GetState<TIFactionState>(false);
			if (this.ignore)
			{
				state.ignoreInterstateDiplomacy.AddUnique(state2);
				return;
			}
			state.ignoreInterstateDiplomacy.Remove(state2);
		}

		// Token: 0x040047C1 RID: 18369
		private GameStateID actingFactionID;

		// Token: 0x040047C2 RID: 18370
		private GameStateID targetFactionID;

		// Token: 0x040047C3 RID: 18371
		private bool ignore;
	}
}
