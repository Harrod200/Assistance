using System;

namespace LapinerTools.Steam.Data
{
	// Token: 0x02000540 RID: 1344
	public class WorkshopItemUpdateEventArgs : EventArgsBase
	{
		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x06002262 RID: 8802 RVA: 0x000B25C2 File Offset: 0x000B07C2
		// (set) Token: 0x06002263 RID: 8803 RVA: 0x000B25CA File Offset: 0x000B07CA
		public WorkshopItemUpdate Item { get; set; }

		// Token: 0x06002264 RID: 8804 RVA: 0x000B25D3 File Offset: 0x000B07D3
		public WorkshopItemUpdateEventArgs()
		{
		}

		// Token: 0x06002265 RID: 8805 RVA: 0x000B25DB File Offset: 0x000B07DB
		public WorkshopItemUpdateEventArgs(EventArgsBase p_errorEventArgs)
			: base(p_errorEventArgs)
		{
		}
	}
}
