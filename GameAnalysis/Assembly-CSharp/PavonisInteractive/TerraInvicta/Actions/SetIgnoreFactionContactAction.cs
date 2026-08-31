using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A93 RID: 2707
	public class SetIgnoreFactionContactAction : PlayerAction
	{
		// Token: 0x0600657B RID: 25979 RVA: 0x002FD088 File Offset: 0x002FB288
		public SetIgnoreFactionContactAction(TIFactionState actingFaction, TIFactionState targetFaction, bool ignore)
		{
			this.actingFactionID = actingFaction.ID;
			this.targetFactionID = targetFaction.ID;
			this.ignore = ignore;
		}

		// Token: 0x0600657C RID: 25980 RVA: 0x002FD0B0 File Offset: 0x002FB2B0
		public override void Execute()
		{
			TIFactionState state = this.actingFactionID.GetState<TIFactionState>(false);
			TIFactionState state2 = this.targetFactionID.GetState<TIFactionState>(false);
			if (this.ignore)
			{
				state.ignoreContacts.AddUnique(state2);
				return;
			}
			state.ignoreContacts.Remove(state2);
		}

		// Token: 0x040047BE RID: 18366
		private GameStateID actingFactionID;

		// Token: 0x040047BF RID: 18367
		private GameStateID targetFactionID;

		// Token: 0x040047C0 RID: 18368
		private bool ignore;
	}
}
