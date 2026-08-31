using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A55 RID: 2645
	public class ChangeCouncilorBio : PlayerAction
	{
		// Token: 0x060064F7 RID: 25847 RVA: 0x002FAC38 File Offset: 0x002F8E38
		public ChangeCouncilorBio(TICouncilorState councilor, string givenName, string familyName, TICouncilorAppearanceTemplate appearanceTemplate, TICouncilorVoiceTemplate voiceTemplate)
		{
			this.councilorID = councilor.ID;
			this.appearanceTemplateDataName = appearanceTemplate.dataName;
			this.voiceTemplateDataName = voiceTemplate.dataName;
			this.givenName = givenName;
			this.familyName = familyName;
		}

		// Token: 0x060064F8 RID: 25848 RVA: 0x002FAC74 File Offset: 0x002F8E74
		public override void Execute()
		{
			this.councilorID.GetState<TICouncilorState>(false).UpdateBiographicalInformation(this.givenName, this.familyName, TemplateManager.Find<TICouncilorAppearanceTemplate>(this.appearanceTemplateDataName, false), TemplateManager.Find<TICouncilorVoiceTemplate>(this.voiceTemplateDataName, false));
		}

		// Token: 0x0400471A RID: 18202
		private GameStateID councilorID;

		// Token: 0x0400471B RID: 18203
		private string givenName;

		// Token: 0x0400471C RID: 18204
		private string familyName;

		// Token: 0x0400471D RID: 18205
		private string appearanceTemplateDataName;

		// Token: 0x0400471E RID: 18206
		private string voiceTemplateDataName;
	}
}
