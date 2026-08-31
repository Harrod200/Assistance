using System;
using Steamworks;

namespace LapinerTools.Steam.Data
{
	// Token: 0x0200053B RID: 1339
	public class WorkshopItem
	{
		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x06002208 RID: 8712 RVA: 0x000B215A File Offset: 0x000B035A
		// (set) Token: 0x06002209 RID: 8713 RVA: 0x000B2162 File Offset: 0x000B0362
		public string Name { get; set; }

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x0600220A RID: 8714 RVA: 0x000B216B File Offset: 0x000B036B
		// (set) Token: 0x0600220B RID: 8715 RVA: 0x000B2173 File Offset: 0x000B0373
		public string SanitizedName { get; set; }

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x0600220C RID: 8716 RVA: 0x000B217C File Offset: 0x000B037C
		// (set) Token: 0x0600220D RID: 8717 RVA: 0x000B2184 File Offset: 0x000B0384
		public string Description { get; set; }

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x0600220E RID: 8718 RVA: 0x000B218D File Offset: 0x000B038D
		// (set) Token: 0x0600220F RID: 8719 RVA: 0x000B2195 File Offset: 0x000B0395
		public string OwnerName { get; set; }

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x06002210 RID: 8720 RVA: 0x000B219E File Offset: 0x000B039E
		// (set) Token: 0x06002211 RID: 8721 RVA: 0x000B21A6 File Offset: 0x000B03A6
		public string PreviewImageURL { get; set; }

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x06002212 RID: 8722 RVA: 0x000B21AF File Offset: 0x000B03AF
		// (set) Token: 0x06002213 RID: 8723 RVA: 0x000B21B7 File Offset: 0x000B03B7
		public uint VotesUp { get; set; }

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x06002214 RID: 8724 RVA: 0x000B21C0 File Offset: 0x000B03C0
		// (set) Token: 0x06002215 RID: 8725 RVA: 0x000B21C8 File Offset: 0x000B03C8
		public uint VotesDown { get; set; }

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x06002216 RID: 8726 RVA: 0x000B21D1 File Offset: 0x000B03D1
		// (set) Token: 0x06002217 RID: 8727 RVA: 0x000B21D9 File Offset: 0x000B03D9
		public ulong Subscriptions { get; set; }

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x06002218 RID: 8728 RVA: 0x000B21E2 File Offset: 0x000B03E2
		// (set) Token: 0x06002219 RID: 8729 RVA: 0x000B21EA File Offset: 0x000B03EA
		public ulong Favorites { get; set; }

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x0600221A RID: 8730 RVA: 0x000B21F3 File Offset: 0x000B03F3
		// (set) Token: 0x0600221B RID: 8731 RVA: 0x000B21FB File Offset: 0x000B03FB
		public bool IsSubscribed { get; set; }

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x0600221C RID: 8732 RVA: 0x000B2204 File Offset: 0x000B0404
		// (set) Token: 0x0600221D RID: 8733 RVA: 0x000B220C File Offset: 0x000B040C
		public bool IsFavorited { get; set; }

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x0600221E RID: 8734 RVA: 0x000B2215 File Offset: 0x000B0415
		// (set) Token: 0x0600221F RID: 8735 RVA: 0x000B221D File Offset: 0x000B041D
		public bool IsVotedUp { get; set; }

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06002220 RID: 8736 RVA: 0x000B2226 File Offset: 0x000B0426
		// (set) Token: 0x06002221 RID: 8737 RVA: 0x000B222E File Offset: 0x000B042E
		public bool IsVotedDown { get; set; }

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x06002222 RID: 8738 RVA: 0x000B2237 File Offset: 0x000B0437
		// (set) Token: 0x06002223 RID: 8739 RVA: 0x000B223F File Offset: 0x000B043F
		public bool IsVoteSkipped { get; set; }

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06002224 RID: 8740 RVA: 0x000B2248 File Offset: 0x000B0448
		// (set) Token: 0x06002225 RID: 8741 RVA: 0x000B2250 File Offset: 0x000B0450
		public bool IsOwned { get; set; }

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x06002226 RID: 8742 RVA: 0x000B2259 File Offset: 0x000B0459
		// (set) Token: 0x06002227 RID: 8743 RVA: 0x000B2261 File Offset: 0x000B0461
		public bool IsInstalled { get; set; }

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x06002228 RID: 8744 RVA: 0x000B226A File Offset: 0x000B046A
		// (set) Token: 0x06002229 RID: 8745 RVA: 0x000B2272 File Offset: 0x000B0472
		public bool IsDownloading { get; set; }

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x0600222A RID: 8746 RVA: 0x000B227B File Offset: 0x000B047B
		// (set) Token: 0x0600222B RID: 8747 RVA: 0x000B2283 File Offset: 0x000B0483
		public bool IsUpdateNeeded { get; set; }

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x0600222C RID: 8748 RVA: 0x000B228C File Offset: 0x000B048C
		// (set) Token: 0x0600222D RID: 8749 RVA: 0x000B2294 File Offset: 0x000B0494
		public string InstalledLocalFolder { get; set; }

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x0600222E RID: 8750 RVA: 0x000B229D File Offset: 0x000B049D
		// (set) Token: 0x0600222F RID: 8751 RVA: 0x000B22A5 File Offset: 0x000B04A5
		public ulong InstalledSizeOnDisk { get; set; }

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06002230 RID: 8752 RVA: 0x000B22AE File Offset: 0x000B04AE
		// (set) Token: 0x06002231 RID: 8753 RVA: 0x000B22B6 File Offset: 0x000B04B6
		public DateTime InstalledTimestamp { get; set; }

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06002232 RID: 8754 RVA: 0x000B22BF File Offset: 0x000B04BF
		// (set) Token: 0x06002233 RID: 8755 RVA: 0x000B22C7 File Offset: 0x000B04C7
		public WorkshopItem.SteamNativeData SteamNative { get; set; }

		// Token: 0x06002234 RID: 8756 RVA: 0x000B22D0 File Offset: 0x000B04D0
		public WorkshopItem()
		{
			this.SteamNative = new WorkshopItem.SteamNativeData();
		}

		// Token: 0x02000CB1 RID: 3249
		public class SteamNativeData
		{
			// Token: 0x1700119C RID: 4508
			// (get) Token: 0x06006D97 RID: 28055 RVA: 0x0030BAFF File Offset: 0x00309CFF
			// (set) Token: 0x06006D98 RID: 28056 RVA: 0x0030BB07 File Offset: 0x00309D07
			public PublishedFileId_t m_nPublishedFileId { get; set; }

			// Token: 0x1700119D RID: 4509
			// (get) Token: 0x06006D99 RID: 28057 RVA: 0x0030BB10 File Offset: 0x00309D10
			// (set) Token: 0x06006D9A RID: 28058 RVA: 0x0030BB18 File Offset: 0x00309D18
			public SteamUGCDetails_t m_details { get; set; }

			// Token: 0x1700119E RID: 4510
			// (get) Token: 0x06006D9B RID: 28059 RVA: 0x0030BB21 File Offset: 0x00309D21
			// (set) Token: 0x06006D9C RID: 28060 RVA: 0x0030BB29 File Offset: 0x00309D29
			public EItemState m_itemState { get; set; }

			// Token: 0x06006D9D RID: 28061 RVA: 0x0030BB32 File Offset: 0x00309D32
			public SteamNativeData()
			{
			}

			// Token: 0x06006D9E RID: 28062 RVA: 0x0030BB3A File Offset: 0x00309D3A
			public SteamNativeData(PublishedFileId_t p_nPublishedFileId)
			{
				this.m_nPublishedFileId = p_nPublishedFileId;
			}
		}
	}
}
