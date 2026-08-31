using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006A6 RID: 1702
	public class TimeEventComplete : GameEvent
	{
		// Token: 0x060028D2 RID: 10450 RVA: 0x000DAB29 File Offset: 0x000D8D29
		public TimeEventComplete(TIGameState eventObject = null, TIDataTemplate eventDataTemplate = null)
		{
			this.eventObject = eventObject;
			this.eventDataTemplate = eventDataTemplate;
		}

		// Token: 0x04001F0D RID: 7949
		public TIGameState eventObject;

		// Token: 0x04001F0E RID: 7950
		public TIDataTemplate eventDataTemplate;
	}
}
