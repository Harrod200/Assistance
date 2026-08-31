using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000782 RID: 1922
	public struct PriorNarrativeEventData
	{
		// Token: 0x06003C4E RID: 15438 RVA: 0x0016E49C File Offset: 0x0016C69C
		public PriorNarrativeEventData(TINarrativeEventTemplate priorEvent, TIGameState actorState, TIGameState selectedTarget, TIGameState secondaryTarget, Dictionary<TIGameState, TIGameState> allTargetsandSeconds)
		{
			this.priorEventTemplateName = priorEvent.dataName;
			this.actorState = actorState;
			this.selectedTarget = selectedTarget;
			this.secondaryTarget = secondaryTarget;
			this.allTargetsandSeconds = allTargetsandSeconds;
		}

		// Token: 0x17000ACA RID: 2762
		// (get) Token: 0x06003C4F RID: 15439 RVA: 0x0016E4C8 File Offset: 0x0016C6C8
		public TINarrativeEventTemplate priorEventTemplate
		{
			get
			{
				return TemplateManager.Find<TINarrativeEventTemplate>(this.priorEventTemplateName, false);
			}
		}

		// Token: 0x04002653 RID: 9811
		public string priorEventTemplateName;

		// Token: 0x04002654 RID: 9812
		public TIGameState actorState;

		// Token: 0x04002655 RID: 9813
		public TIGameState selectedTarget;

		// Token: 0x04002656 RID: 9814
		public TIGameState secondaryTarget;

		// Token: 0x04002657 RID: 9815
		public Dictionary<TIGameState, TIGameState> allTargetsandSeconds;
	}
}
