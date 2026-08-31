using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;

namespace LapinerTools.Steam.Data
{
	// Token: 0x0200053F RID: 1343
	public class WorkshopItemUpdate
	{
		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x06002251 RID: 8785 RVA: 0x000B242D File Offset: 0x000B062D
		// (set) Token: 0x06002252 RID: 8786 RVA: 0x000B2435 File Offset: 0x000B0635
		public string Name { get; set; }

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x06002253 RID: 8787 RVA: 0x000B243E File Offset: 0x000B063E
		// (set) Token: 0x06002254 RID: 8788 RVA: 0x000B2446 File Offset: 0x000B0646
		public string Description { get; set; }

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x06002255 RID: 8789 RVA: 0x000B244F File Offset: 0x000B064F
		// (set) Token: 0x06002256 RID: 8790 RVA: 0x000B2457 File Offset: 0x000B0657
		public string IconPath { get; set; }

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x06002257 RID: 8791 RVA: 0x000B2460 File Offset: 0x000B0660
		// (set) Token: 0x06002258 RID: 8792 RVA: 0x000B2468 File Offset: 0x000B0668
		public string ContentPath { get; set; }

		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x06002259 RID: 8793 RVA: 0x000B2471 File Offset: 0x000B0671
		// (set) Token: 0x0600225A RID: 8794 RVA: 0x000B2479 File Offset: 0x000B0679
		public string ChangeNote { get; set; }

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x0600225B RID: 8795 RVA: 0x000B2482 File Offset: 0x000B0682
		// (set) Token: 0x0600225C RID: 8796 RVA: 0x000B248A File Offset: 0x000B068A
		public List<string> Tags { get; set; }

		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x0600225D RID: 8797 RVA: 0x000B2493 File Offset: 0x000B0693
		// (set) Token: 0x0600225E RID: 8798 RVA: 0x000B249B File Offset: 0x000B069B
		public WorkshopItemUpdate.SteamNativeData SteamNative { get; set; }

		// Token: 0x0600225F RID: 8799 RVA: 0x000B24A4 File Offset: 0x000B06A4
		public WorkshopItemUpdate()
		{
			this.SteamNative = new WorkshopItemUpdate.SteamNativeData();
			this.ChangeNote = "Initial version";
			this.Tags = new List<string>();
		}

		// Token: 0x06002260 RID: 8800 RVA: 0x000B24D0 File Offset: 0x000B06D0
		public WorkshopItemUpdate(WorkshopItem p_existingItem)
		{
			if (p_existingItem.SteamNative != null)
			{
				this.Name = p_existingItem.Name;
				this.Description = p_existingItem.Description;
				this.ContentPath = p_existingItem.InstalledLocalFolder;
				this.SteamNative = new WorkshopItemUpdate.SteamNativeData(p_existingItem.SteamNative.m_nPublishedFileId);
				this.ChangeNote = "";
				this.Tags = new List<string>();
				if (!string.IsNullOrEmpty(this.ContentPath))
				{
					string text = Path.Combine(this.ContentPath, this.Name + ".png");
					if (File.Exists(text))
					{
						this.IconPath = text;
						return;
					}
				}
			}
			else
			{
				this.SteamNative = new WorkshopItemUpdate.SteamNativeData();
				this.ChangeNote = "Initial version";
				this.Tags = new List<string>();
			}
		}

		// Token: 0x06002261 RID: 8801 RVA: 0x000B2598 File Offset: 0x000B0798
		public WorkshopItemUpdate(PublishedFileId_t p_existingPublishedFileId)
		{
			this.SteamNative = new WorkshopItemUpdate.SteamNativeData(p_existingPublishedFileId);
			this.ChangeNote = "";
			this.Tags = new List<string>();
		}

		// Token: 0x02000CB2 RID: 3250
		public class SteamNativeData
		{
			// Token: 0x1700119F RID: 4511
			// (get) Token: 0x06006D9F RID: 28063 RVA: 0x0030BB49 File Offset: 0x00309D49
			// (set) Token: 0x06006DA0 RID: 28064 RVA: 0x0030BB51 File Offset: 0x00309D51
			public PublishedFileId_t m_nPublishedFileId { get; set; }

			// Token: 0x170011A0 RID: 4512
			// (get) Token: 0x06006DA1 RID: 28065 RVA: 0x0030BB5A File Offset: 0x00309D5A
			// (set) Token: 0x06006DA2 RID: 28066 RVA: 0x0030BB62 File Offset: 0x00309D62
			public UGCUpdateHandle_t m_uploadHandle { get; set; }

			// Token: 0x170011A1 RID: 4513
			// (get) Token: 0x06006DA3 RID: 28067 RVA: 0x0030BB6B File Offset: 0x00309D6B
			// (set) Token: 0x06006DA4 RID: 28068 RVA: 0x0030BB73 File Offset: 0x00309D73
			public EItemUpdateStatus m_lastValidUpdateStatus { get; set; }

			// Token: 0x06006DA5 RID: 28069 RVA: 0x0030BB7C File Offset: 0x00309D7C
			public SteamNativeData()
			{
				this.m_nPublishedFileId = PublishedFileId_t.Invalid;
				this.m_uploadHandle = UGCUpdateHandle_t.Invalid;
				this.m_lastValidUpdateStatus = EItemUpdateStatus.k_EItemUpdateStatusInvalid;
			}

			// Token: 0x06006DA6 RID: 28070 RVA: 0x0030BBA1 File Offset: 0x00309DA1
			public SteamNativeData(PublishedFileId_t p_nPublishedFileId)
			{
				this.m_nPublishedFileId = p_nPublishedFileId;
				this.m_uploadHandle = UGCUpdateHandle_t.Invalid;
				this.m_lastValidUpdateStatus = EItemUpdateStatus.k_EItemUpdateStatusInvalid;
			}
		}
	}
}
