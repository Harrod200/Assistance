using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000785 RID: 1925
	public struct PendingNarrativeEvent
	{
		// Token: 0x06003C52 RID: 15442 RVA: 0x0016E4FD File Offset: 0x0016C6FD
		public PendingNarrativeEvent(Prompt prompt, string dataName, Dictionary<TIGameState, TIGameState> allTargetsandSeconds)
		{
			this.prompt = prompt;
			this.dataName = dataName;
			this.allTargetsandSeconds = new Dictionary<TIGameState, TIGameState>(allTargetsandSeconds);
		}

		// Token: 0x17000ACB RID: 2763
		// (get) Token: 0x06003C53 RID: 15443 RVA: 0x0016E519 File Offset: 0x0016C719
		public TINarrativeEventTemplate narrativeEvent
		{
			get
			{
				return TemplateManager.Find<TINarrativeEventTemplate>(this.dataName, false);
			}
		}

		// Token: 0x0400265D RID: 9821
		public Prompt prompt;

		// Token: 0x0400265E RID: 9822
		public string dataName;

		// Token: 0x0400265F RID: 9823
		public Dictionary<TIGameState, TIGameState> allTargetsandSeconds;
	}
}
