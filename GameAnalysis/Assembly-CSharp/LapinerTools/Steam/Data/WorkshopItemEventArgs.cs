using System;

namespace LapinerTools.Steam.Data
{
	// Token: 0x0200053C RID: 1340
	public class WorkshopItemEventArgs : EventArgsBase
	{
		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x06002235 RID: 8757 RVA: 0x000B22E3 File Offset: 0x000B04E3
		// (set) Token: 0x06002236 RID: 8758 RVA: 0x000B22EB File Offset: 0x000B04EB
		public WorkshopItem Item { get; set; }

		// Token: 0x06002237 RID: 8759 RVA: 0x000B22F4 File Offset: 0x000B04F4
		public WorkshopItemEventArgs()
		{
		}

		// Token: 0x06002238 RID: 8760 RVA: 0x000B22FC File Offset: 0x000B04FC
		public WorkshopItemEventArgs(WorkshopItem p_item)
		{
			this.Item = p_item;
		}

		// Token: 0x06002239 RID: 8761 RVA: 0x000B230B File Offset: 0x000B050B
		public WorkshopItemEventArgs(EventArgsBase p_errorEventArgs)
			: base(p_errorEventArgs)
		{
		}
	}
}
