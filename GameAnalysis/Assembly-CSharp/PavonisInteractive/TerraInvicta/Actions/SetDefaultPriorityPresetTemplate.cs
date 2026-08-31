using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A92 RID: 2706
	public class SetDefaultPriorityPresetTemplate : PlayerAction
	{
		// Token: 0x06006579 RID: 25977 RVA: 0x002FD054 File Offset: 0x002FB254
		public SetDefaultPriorityPresetTemplate(TIFactionState faction, string presetTemplateName)
		{
			this.factionID = faction.ID;
			this.presetTemplateName = presetTemplateName;
		}

		// Token: 0x0600657A RID: 25978 RVA: 0x002FD06F File Offset: 0x002FB26F
		public override void Execute()
		{
			this.factionID.GetState<TIFactionState>(false).SetDefaultPreset(this.presetTemplateName);
		}

		// Token: 0x040047BC RID: 18364
		private GameStateID factionID;

		// Token: 0x040047BD RID: 18365
		private string presetTemplateName;
	}
}
