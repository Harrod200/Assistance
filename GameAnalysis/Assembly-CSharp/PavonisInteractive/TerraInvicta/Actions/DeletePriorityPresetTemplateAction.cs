using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A6F RID: 2671
	public class DeletePriorityPresetTemplateAction : PlayerAction
	{
		// Token: 0x0600652C RID: 25900 RVA: 0x002FB78E File Offset: 0x002F998E
		public DeletePriorityPresetTemplateAction(TIFactionState faction, TIPriorityPresetTemplate presetTemplate)
		{
			this.factionID = faction.ID;
			this.presetTemplate = presetTemplate;
		}

		// Token: 0x0600652D RID: 25901 RVA: 0x002FB7A9 File Offset: 0x002F99A9
		public override void Execute()
		{
			this.factionID.GetState<TIFactionState>(false).DeleteCustomPresetDesign(this.presetTemplate);
		}

		// Token: 0x04004756 RID: 18262
		public GameStateID factionID;

		// Token: 0x04004757 RID: 18263
		public TIPriorityPresetTemplate presetTemplate;
	}
}
