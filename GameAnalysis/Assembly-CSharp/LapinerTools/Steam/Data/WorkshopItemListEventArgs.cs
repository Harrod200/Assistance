using System;

namespace LapinerTools.Steam.Data
{
	// Token: 0x0200053E RID: 1342
	public class WorkshopItemListEventArgs : EventArgsBase
	{
		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x0600224D RID: 8781 RVA: 0x000B240B File Offset: 0x000B060B
		// (set) Token: 0x0600224E RID: 8782 RVA: 0x000B2413 File Offset: 0x000B0613
		public WorkshopItemList ItemList { get; set; }

		// Token: 0x0600224F RID: 8783 RVA: 0x000B241C File Offset: 0x000B061C
		public WorkshopItemListEventArgs()
		{
		}

		// Token: 0x06002250 RID: 8784 RVA: 0x000B2424 File Offset: 0x000B0624
		public WorkshopItemListEventArgs(EventArgsBase p_errorEventArgs)
			: base(p_errorEventArgs)
		{
		}
	}
}
