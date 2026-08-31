using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A4C RID: 2636
	public class ApplyPriorityPresetToControlPoint : PlayerAction
	{
		// Token: 0x060064E5 RID: 25829 RVA: 0x002FA432 File Offset: 0x002F8632
		public ApplyPriorityPresetToControlPoint(TIControlPoint controlPoint, TIFactionState faction, string presetTemplateName)
		{
			this.controlPointID = controlPoint.ID;
			this.factionID = faction.ID;
			this.presetTemplateName = presetTemplateName;
		}

		// Token: 0x060064E6 RID: 25830 RVA: 0x002FA45C File Offset: 0x002F865C
		public override void Execute()
		{
			TIControlPoint state = this.controlPointID.GetState<TIControlPoint>(false);
			TIFactionState state2 = this.factionID.GetState<TIFactionState>(false);
			if (state.faction == state2)
			{
				state.nation.ApplyInvestmentTemplateToControlPoint(state.positionInNation, this.presetTemplateName);
			}
		}

		// Token: 0x040046FF RID: 18175
		private GameStateID controlPointID;

		// Token: 0x04004700 RID: 18176
		private GameStateID factionID;

		// Token: 0x04004701 RID: 18177
		private string presetTemplateName;
	}
}
