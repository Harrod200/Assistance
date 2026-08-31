using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A8C RID: 2700
	public class SelfDisableControlPoints : PlayerAction
	{
		// Token: 0x0600656C RID: 25964 RVA: 0x002FCDFF File Offset: 0x002FAFFF
		public SelfDisableControlPoints(TIFactionState faction, TINationState nation)
		{
			this.factionID = faction.ID;
			this.nationID = nation.ID;
		}

		// Token: 0x0600656D RID: 25965 RVA: 0x002FCE20 File Offset: 0x002FB020
		public override void Execute()
		{
			TINationState state = this.nationID.GetState<TINationState>(false);
			TIFactionState state2 = this.factionID.GetState<TIFactionState>(false);
			state.SelfDisableControlPoints(state2);
		}

		// Token: 0x040047AE RID: 18350
		private GameStateID factionID;

		// Token: 0x040047AF RID: 18351
		private GameStateID nationID;
	}
}
