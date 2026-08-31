using System;

namespace LapinerTools.Steam.Data
{
	// Token: 0x02000542 RID: 1346
	public class WorkshopSortModeEventArgs : EventArgsBase
	{
		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x0600226C RID: 8812 RVA: 0x000B2665 File Offset: 0x000B0865
		// (set) Token: 0x0600226D RID: 8813 RVA: 0x000B266D File Offset: 0x000B086D
		public WorkshopSortMode SortMode { get; set; }

		// Token: 0x0600226E RID: 8814 RVA: 0x000B2676 File Offset: 0x000B0876
		public WorkshopSortModeEventArgs()
		{
		}

		// Token: 0x0600226F RID: 8815 RVA: 0x000B267E File Offset: 0x000B087E
		public WorkshopSortModeEventArgs(WorkshopSortMode p_sortMode)
		{
			this.SortMode = p_sortMode;
		}

		// Token: 0x06002270 RID: 8816 RVA: 0x000B268D File Offset: 0x000B088D
		public WorkshopSortModeEventArgs(EventArgsBase p_errorEventArgs)
			: base(p_errorEventArgs)
		{
		}
	}
}
