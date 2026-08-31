using System;

namespace LapinerTools.Steam.Data.Internal
{
	// Token: 0x02000544 RID: 1348
	public class WorkshopItemInfo
	{
		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x0600227C RID: 8828 RVA: 0x000B29BD File Offset: 0x000B0BBD
		// (set) Token: 0x0600227D RID: 8829 RVA: 0x000B29C5 File Offset: 0x000B0BC5
		public ulong PublishedFileId { get; set; }

		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x0600227E RID: 8830 RVA: 0x000B29CE File Offset: 0x000B0BCE
		// (set) Token: 0x0600227F RID: 8831 RVA: 0x000B29D6 File Offset: 0x000B0BD6
		public string Name { get; set; }

		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06002280 RID: 8832 RVA: 0x000B29DF File Offset: 0x000B0BDF
		// (set) Token: 0x06002281 RID: 8833 RVA: 0x000B29E7 File Offset: 0x000B0BE7
		public string Description { get; set; }

		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x06002282 RID: 8834 RVA: 0x000B29F0 File Offset: 0x000B0BF0
		// (set) Token: 0x06002283 RID: 8835 RVA: 0x000B29F8 File Offset: 0x000B0BF8
		public string IconFileName { get; set; }

		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x06002284 RID: 8836 RVA: 0x000B2A01 File Offset: 0x000B0C01
		// (set) Token: 0x06002285 RID: 8837 RVA: 0x000B2A09 File Offset: 0x000B0C09
		public string[] Tags { get; set; }
	}
}
