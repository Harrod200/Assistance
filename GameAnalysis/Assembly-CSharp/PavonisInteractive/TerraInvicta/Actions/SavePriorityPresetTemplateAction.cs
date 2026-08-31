using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A83 RID: 2691
	public class SavePriorityPresetTemplateAction : PlayerAction
	{
		// Token: 0x0600655A RID: 25946 RVA: 0x002FC640 File Offset: 0x002FA840
		public SavePriorityPresetTemplateAction(TIFactionState faction, TIPriorityPresetTemplate presetTemplate)
		{
			this.factionID = faction.ID;
			this.presetTemplate = presetTemplate;
		}

		// Token: 0x0600655B RID: 25947 RVA: 0x002FC65B File Offset: 0x002FA85B
		public override void Execute()
		{
			this.factionID.GetState<TIFactionState>(false).SaveCustomPresetDesign(this.presetTemplate);
		}

		// Token: 0x0400478C RID: 18316
		public GameStateID factionID;

		// Token: 0x0400478D RID: 18317
		public TIPriorityPresetTemplate presetTemplate;
	}
}
