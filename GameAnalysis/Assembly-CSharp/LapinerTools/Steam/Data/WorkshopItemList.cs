using System;
using System.Collections.Generic;

namespace LapinerTools.Steam.Data
{
	// Token: 0x0200053D RID: 1341
	public class WorkshopItemList
	{
		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x0600223A RID: 8762 RVA: 0x000B2314 File Offset: 0x000B0514
		// (set) Token: 0x0600223B RID: 8763 RVA: 0x000B231C File Offset: 0x000B051C
		public uint Page { get; set; }

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x0600223C RID: 8764 RVA: 0x000B2325 File Offset: 0x000B0525
		// (set) Token: 0x0600223D RID: 8765 RVA: 0x000B232D File Offset: 0x000B052D
		public uint PagesItems { get; set; }

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x0600223E RID: 8766 RVA: 0x000B2336 File Offset: 0x000B0536
		// (set) Token: 0x0600223F RID: 8767 RVA: 0x000B233E File Offset: 0x000B053E
		public List<WorkshopItem> Items { get; set; }

		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x06002240 RID: 8768 RVA: 0x000B2347 File Offset: 0x000B0547
		// (set) Token: 0x06002241 RID: 8769 RVA: 0x000B234F File Offset: 0x000B054F
		public uint PagesItemsFavorited { get; set; }

		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x06002242 RID: 8770 RVA: 0x000B2358 File Offset: 0x000B0558
		// (set) Token: 0x06002243 RID: 8771 RVA: 0x000B2360 File Offset: 0x000B0560
		public List<WorkshopItem> ItemsFavorited { get; set; }

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x06002244 RID: 8772 RVA: 0x000B2369 File Offset: 0x000B0569
		// (set) Token: 0x06002245 RID: 8773 RVA: 0x000B2371 File Offset: 0x000B0571
		public uint PagesItemsSubscribed { get; set; }

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x06002246 RID: 8774 RVA: 0x000B237A File Offset: 0x000B057A
		// (set) Token: 0x06002247 RID: 8775 RVA: 0x000B2382 File Offset: 0x000B0582
		public List<WorkshopItem> ItemsSubscribed { get; set; }

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x06002248 RID: 8776 RVA: 0x000B238B File Offset: 0x000B058B
		// (set) Token: 0x06002249 RID: 8777 RVA: 0x000B2393 File Offset: 0x000B0593
		public uint PagesItemsVoted { get; set; }

		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x0600224A RID: 8778 RVA: 0x000B239C File Offset: 0x000B059C
		// (set) Token: 0x0600224B RID: 8779 RVA: 0x000B23A4 File Offset: 0x000B05A4
		public List<WorkshopItem> ItemsVoted { get; set; }

		// Token: 0x0600224C RID: 8780 RVA: 0x000B23B0 File Offset: 0x000B05B0
		public WorkshopItemList()
		{
			this.Page = 1U;
			this.PagesItems = 1U;
			this.Items = new List<WorkshopItem>();
			this.PagesItemsFavorited = 1U;
			this.ItemsFavorited = new List<WorkshopItem>();
			this.PagesItemsVoted = 1U;
			this.ItemsVoted = new List<WorkshopItem>();
			this.ItemsSubscribed = new List<WorkshopItem>();
		}
	}
}
