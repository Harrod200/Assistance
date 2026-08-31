using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006A5 RID: 1701
	public class TimeEventStart : GameEvent
	{
		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x060028CD RID: 10445 RVA: 0x000DAAE6 File Offset: 0x000D8CE6
		public TIGameState eventObject
		{
			get
			{
				return this.timeEvent.eventObject;
			}
		}

		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x060028CE RID: 10446 RVA: 0x000DAAF3 File Offset: 0x000D8CF3
		public TIGameState eventObject2
		{
			get
			{
				return this.timeEvent.eventObject2;
			}
		}

		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x060028CF RID: 10447 RVA: 0x000DAB00 File Offset: 0x000D8D00
		public TIDataTemplate eventDataTemplate
		{
			get
			{
				return this.timeEvent.eventDataTemplate;
			}
		}

		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x060028D0 RID: 10448 RVA: 0x000DAB0D File Offset: 0x000D8D0D
		public TIDateTime startTime
		{
			get
			{
				return this.timeEvent.time;
			}
		}

		// Token: 0x060028D1 RID: 10449 RVA: 0x000DAB1A File Offset: 0x000D8D1A
		public TimeEventStart(TITimeEvent timeEvent)
		{
			this.timeEvent = timeEvent;
		}

		// Token: 0x04001F0C RID: 7948
		public TITimeEvent timeEvent;
	}
}
