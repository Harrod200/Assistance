using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000781 RID: 1921
	public struct CurrentNarrativeEventData
	{
		// Token: 0x06003C4C RID: 15436 RVA: 0x0016E462 File Offset: 0x0016C662
		public CurrentNarrativeEventData(TINarrativeEventTemplate eventTemplate, TIGameState actorState, TIGameState selectedTarget, TIGameState secondaryTarget, Dictionary<TIGameState, TIGameState> allTargetsandSeconds)
		{
			this.eventTemplateName = eventTemplate.dataName;
			this.actorState = actorState;
			this.selectedTarget = selectedTarget;
			this.secondaryTarget = secondaryTarget;
			this.allTargetsandSeconds = allTargetsandSeconds;
		}

		// Token: 0x17000AC9 RID: 2761
		// (get) Token: 0x06003C4D RID: 15437 RVA: 0x0016E48E File Offset: 0x0016C68E
		public TINarrativeEventTemplate eventTemplate
		{
			get
			{
				return TemplateManager.Find<TINarrativeEventTemplate>(this.eventTemplateName, false);
			}
		}

		// Token: 0x0400264E RID: 9806
		public string eventTemplateName;

		// Token: 0x0400264F RID: 9807
		public TIGameState actorState;

		// Token: 0x04002650 RID: 9808
		public TIGameState selectedTarget;

		// Token: 0x04002651 RID: 9809
		public TIGameState secondaryTarget;

		// Token: 0x04002652 RID: 9810
		public Dictionary<TIGameState, TIGameState> allTargetsandSeconds;
	}
}
