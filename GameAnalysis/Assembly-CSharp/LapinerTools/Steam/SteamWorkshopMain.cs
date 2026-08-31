using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using LapinerTools.Steam.Data;
using LapinerTools.Steam.Data.Internal;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Modding;
using Steamworks;
using UnityEngine;

namespace LapinerTools.Steam
{
	// Token: 0x02000532 RID: 1330
	public class SteamWorkshopMain : SteamMainBase<SteamWorkshopMain>
	{
		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06002146 RID: 8518 RVA: 0x000AC4D4 File Offset: 0x000AA6D4
		// (remove) Token: 0x06002147 RID: 8519 RVA: 0x000AC50C File Offset: 0x000AA70C
		public event Action<WorkshopItemListEventArgs> OnItemListLoaded;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06002148 RID: 8520 RVA: 0x000AC544 File Offset: 0x000AA744
		// (remove) Token: 0x06002149 RID: 8521 RVA: 0x000AC57C File Offset: 0x000AA77C
		public event Action<WorkshopItemEventArgs> OnSubscribed;

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x0600214A RID: 8522 RVA: 0x000AC5B4 File Offset: 0x000AA7B4
		// (remove) Token: 0x0600214B RID: 8523 RVA: 0x000AC5EC File Offset: 0x000AA7EC
		public event Action<WorkshopItemEventArgs> OnUnsubscribed;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x0600214C RID: 8524 RVA: 0x000AC624 File Offset: 0x000AA824
		// (remove) Token: 0x0600214D RID: 8525 RVA: 0x000AC65C File Offset: 0x000AA85C
		public event Action<WorkshopItemEventArgs> OnAddedFavorite;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x0600214E RID: 8526 RVA: 0x000AC694 File Offset: 0x000AA894
		// (remove) Token: 0x0600214F RID: 8527 RVA: 0x000AC6CC File Offset: 0x000AA8CC
		public event Action<WorkshopItemEventArgs> OnRemovedFavorite;

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x06002150 RID: 8528 RVA: 0x000AC704 File Offset: 0x000AA904
		// (remove) Token: 0x06002151 RID: 8529 RVA: 0x000AC73C File Offset: 0x000AA93C
		public event Action<WorkshopItemEventArgs> OnVoted;

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x06002152 RID: 8530 RVA: 0x000AC774 File Offset: 0x000AA974
		// (remove) Token: 0x06002153 RID: 8531 RVA: 0x000AC7AC File Offset: 0x000AA9AC
		public event Action<WorkshopItemEventArgs> OnInstalled;

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x06002154 RID: 8532 RVA: 0x000AC7E4 File Offset: 0x000AA9E4
		// (remove) Token: 0x06002155 RID: 8533 RVA: 0x000AC81C File Offset: 0x000AAA1C
		public event Action<WorkshopItemUpdateEventArgs> OnUploaded;

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x06002156 RID: 8534 RVA: 0x000AC851 File Offset: 0x000AAA51
		// (set) Token: 0x06002157 RID: 8535 RVA: 0x000AC859 File Offset: 0x000AAA59
		public WorkshopSortMode Sorting
		{
			get
			{
				return this.m_sorting;
			}
			set
			{
				this.m_sorting = value;
			}
		}

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06002158 RID: 8536 RVA: 0x000AC862 File Offset: 0x000AAA62
		// (set) Token: 0x06002159 RID: 8537 RVA: 0x000AC86A File Offset: 0x000AAA6A
		public string SearchText
		{
			get
			{
				return this.m_searchText;
			}
			set
			{
				this.m_searchText = value;
			}
		}

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x0600215A RID: 8538 RVA: 0x000AC873 File Offset: 0x000AAA73
		// (set) Token: 0x0600215B RID: 8539 RVA: 0x000AC87B File Offset: 0x000AAA7B
		public List<string> SearchTags
		{
			get
			{
				return this.m_searchTags;
			}
			set
			{
				this.m_searchTags = value;
			}
		}

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x0600215C RID: 8540 RVA: 0x000AC884 File Offset: 0x000AAA84
		// (set) Token: 0x0600215D RID: 8541 RVA: 0x000AC88C File Offset: 0x000AAA8C
		public bool SearchMatchAnyTag
		{
			get
			{
				return this.m_searchMatchAnyTag;
			}
			set
			{
				this.m_searchMatchAnyTag = value;
			}
		}

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x0600215E RID: 8542 RVA: 0x000AC895 File Offset: 0x000AAA95
		// (set) Token: 0x0600215F RID: 8543 RVA: 0x000AC89D File Offset: 0x000AAA9D
		public bool IsSteamCacheEnabled
		{
			get
			{
				return this.m_isSteamCacheEnabled;
			}
			set
			{
				this.m_isSteamCacheEnabled = value;
			}
		}

		// Token: 0x06002160 RID: 8544 RVA: 0x000AC8A8 File Offset: 0x000AAAA8
		public bool GetItemList(uint p_page, Action<WorkshopItemListEventArgs> p_onItemListLoaded)
		{
			if (p_page <= 0U)
			{
				LapinerTools.Steam.Data.ErrorEventArgs e = new LapinerTools.Steam.Data.ErrorEventArgs("Page (p_page parameter) must be greater 0, but was '" + p_page.ToString() + "'!");
				this.InvokeEventHandlerSafely<WorkshopItemListEventArgs>(p_onItemListLoaded, new WorkshopItemListEventArgs(e));
				this.HandleError("GetItemList: failed! ", e);
				return false;
			}
			object @lock = this.m_lock;
			bool flag2;
			lock (@lock)
			{
				if (this.m_reqItemList != null)
				{
					flag2 = false;
				}
				else if (SteamManager.Initialized)
				{
					this.m_reqItemList = new WorkshopItemList();
					this.m_reqItemList.Page = p_page;
					this.m_reqItemList.PagesItemsFavorited = 0U;
					this.m_reqItemList.PagesItemsVoted = 0U;
					this.m_pendingRequests.Clear<GetUserItemVoteResult_t>();
					this.SetSingleShotEventHandler<WorkshopItemListEventArgs>("OnItemListLoaded", ref this.OnItemListLoaded, p_onItemListLoaded);
					if (this.m_sorting.SOURCE == EWorkshopSource.OWNED)
					{
						this.QueryPublishedItems(1U);
					}
					else if (this.m_sorting.SOURCE == EWorkshopSource.PUBLIC)
					{
						this.QueryFavoritedItems(1U);
					}
					else if (this.m_sorting.SOURCE == EWorkshopSource.SUBSCRIBED)
					{
						this.QuerySubscribedItems(1U);
					}
					flag2 = true;
				}
				else
				{
					LapinerTools.Steam.Data.ErrorEventArgs e2 = LapinerTools.Steam.Data.ErrorEventArgs.CreateSteamNotInit();
					this.InvokeEventHandlerSafely<WorkshopItemListEventArgs>(p_onItemListLoaded, new WorkshopItemListEventArgs(e2));
					this.HandleError("GetItemList: failed! ", e2);
					flag2 = false;
				}
			}
			return flag2;
		}

		// Token: 0x06002161 RID: 8545 RVA: 0x000AC9F0 File Offset: 0x000AABF0
		public bool Subscribe(WorkshopItem p_item, Action<WorkshopItemEventArgs> p_onSubscribed)
		{
			return this.Subscribe(p_item.SteamNative.m_nPublishedFileId, p_onSubscribed);
		}

		// Token: 0x06002162 RID: 8546 RVA: 0x000ACA04 File Offset: 0x000AAC04
		public bool Subscribe(PublishedFileId_t p_fileId, Action<WorkshopItemEventArgs> p_onSubscribed)
		{
			if (SteamManager.Initialized)
			{
				string text = "OnSubscribed";
				PublishedFileId_t publishedFileId_t = p_fileId;
				this.SetSingleShotEventHandler<WorkshopItemEventArgs>(text + publishedFileId_t.ToString(), ref this.OnSubscribed, p_onSubscribed);
				base.Execute<RemoteStorageSubscribePublishedFileResult_t>(SteamUGC.SubscribeItem(p_fileId), new CallResult<RemoteStorageSubscribePublishedFileResult_t>.APIDispatchDelegate(this.OnSubscribeCallCompleted));
				return true;
			}
			LapinerTools.Steam.Data.ErrorEventArgs e = LapinerTools.Steam.Data.ErrorEventArgs.CreateSteamNotInit();
			this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(p_onSubscribed, new WorkshopItemEventArgs(e));
			this.HandleError("Subscribe: failed! ", e);
			return false;
		}

		// Token: 0x06002163 RID: 8547 RVA: 0x000ACA78 File Offset: 0x000AAC78
		public bool Unsubscribe(WorkshopItem p_item, Action<WorkshopItemEventArgs> p_onUnsubscribed)
		{
			return this.Unsubscribe(p_item.SteamNative.m_nPublishedFileId, p_onUnsubscribed);
		}

		// Token: 0x06002164 RID: 8548 RVA: 0x000ACA8C File Offset: 0x000AAC8C
		public bool Unsubscribe(PublishedFileId_t p_fileId, Action<WorkshopItemEventArgs> p_onUnsubscribed)
		{
			if (SteamManager.Initialized)
			{
				string text = "OnUnsubscribed";
				PublishedFileId_t publishedFileId_t = p_fileId;
				this.SetSingleShotEventHandler<WorkshopItemEventArgs>(text + publishedFileId_t.ToString(), ref this.OnUnsubscribed, p_onUnsubscribed);
				base.Execute<RemoteStorageUnsubscribePublishedFileResult_t>(SteamUGC.UnsubscribeItem(p_fileId), new CallResult<RemoteStorageUnsubscribePublishedFileResult_t>.APIDispatchDelegate(this.OnUnsubscribeCallCompleted));
				return true;
			}
			LapinerTools.Steam.Data.ErrorEventArgs e = LapinerTools.Steam.Data.ErrorEventArgs.CreateSteamNotInit();
			this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(p_onUnsubscribed, new WorkshopItemEventArgs(e));
			this.HandleError("Unsubscribe: failed! ", e);
			return false;
		}

		// Token: 0x06002165 RID: 8549 RVA: 0x000ACB00 File Offset: 0x000AAD00
		public bool AddFavorite(WorkshopItem p_item, Action<WorkshopItemEventArgs> p_onAddedFavorite)
		{
			return this.AddFavorite(p_item.SteamNative.m_nPublishedFileId, p_onAddedFavorite);
		}

		// Token: 0x06002166 RID: 8550 RVA: 0x000ACB14 File Offset: 0x000AAD14
		public bool AddFavorite(PublishedFileId_t p_fileId, Action<WorkshopItemEventArgs> p_onAddedFavorite)
		{
			if (SteamManager.Initialized)
			{
				string text = "OnAddedFavorite";
				PublishedFileId_t publishedFileId_t = p_fileId;
				this.SetSingleShotEventHandler<WorkshopItemEventArgs>(text + publishedFileId_t.ToString(), ref this.OnAddedFavorite, p_onAddedFavorite);
				base.Execute<UserFavoriteItemsListChanged_t>(SteamUGC.AddItemToFavorites(SteamUtils.GetAppID(), p_fileId), new CallResult<UserFavoriteItemsListChanged_t>.APIDispatchDelegate(this.OnFavoriteChangeCallCompleted));
				return true;
			}
			LapinerTools.Steam.Data.ErrorEventArgs e = LapinerTools.Steam.Data.ErrorEventArgs.CreateSteamNotInit();
			this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(p_onAddedFavorite, new WorkshopItemEventArgs(e));
			this.HandleError("AddFavorite: failed! ", e);
			return false;
		}

		// Token: 0x06002167 RID: 8551 RVA: 0x000ACB8D File Offset: 0x000AAD8D
		public bool RemoveFavorite(WorkshopItem p_item, Action<WorkshopItemEventArgs> p_onRemovedFavorite)
		{
			return this.RemoveFavorite(p_item.SteamNative.m_nPublishedFileId, p_onRemovedFavorite);
		}

		// Token: 0x06002168 RID: 8552 RVA: 0x000ACBA4 File Offset: 0x000AADA4
		public bool RemoveFavorite(PublishedFileId_t p_fileId, Action<WorkshopItemEventArgs> p_onRemovedFavorite)
		{
			if (SteamManager.Initialized)
			{
				string text = "OnRemovedFavorite";
				PublishedFileId_t publishedFileId_t = p_fileId;
				this.SetSingleShotEventHandler<WorkshopItemEventArgs>(text + publishedFileId_t.ToString(), ref this.OnRemovedFavorite, p_onRemovedFavorite);
				base.Execute<UserFavoriteItemsListChanged_t>(SteamUGC.RemoveItemFromFavorites(SteamUtils.GetAppID(), p_fileId), new CallResult<UserFavoriteItemsListChanged_t>.APIDispatchDelegate(this.OnFavoriteChangeCallCompleted));
				return true;
			}
			LapinerTools.Steam.Data.ErrorEventArgs e = LapinerTools.Steam.Data.ErrorEventArgs.CreateSteamNotInit();
			this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(p_onRemovedFavorite, new WorkshopItemEventArgs(e));
			this.HandleError("RemoveFavorite: failed! ", e);
			return false;
		}

		// Token: 0x06002169 RID: 8553 RVA: 0x000ACC1D File Offset: 0x000AAE1D
		public bool Vote(WorkshopItem p_item, bool p_isUpVote, Action<WorkshopItemEventArgs> p_onVoted)
		{
			return this.Vote(p_item.SteamNative.m_nPublishedFileId, p_isUpVote, p_onVoted);
		}

		// Token: 0x0600216A RID: 8554 RVA: 0x000ACC34 File Offset: 0x000AAE34
		public bool Vote(PublishedFileId_t p_fileId, bool p_isUpVote, Action<WorkshopItemEventArgs> p_onVoted)
		{
			if (SteamManager.Initialized)
			{
				string text = "OnVoted";
				PublishedFileId_t publishedFileId_t = p_fileId;
				this.SetSingleShotEventHandler<WorkshopItemEventArgs>(text + publishedFileId_t.ToString(), ref this.OnVoted, p_onVoted);
				base.Execute<SetUserItemVoteResult_t>(SteamUGC.SetUserItemVote(p_fileId, p_isUpVote), new CallResult<SetUserItemVoteResult_t>.APIDispatchDelegate(this.OnVoteCallCompleted));
				return true;
			}
			LapinerTools.Steam.Data.ErrorEventArgs e = LapinerTools.Steam.Data.ErrorEventArgs.CreateSteamNotInit();
			this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(p_onVoted, new WorkshopItemEventArgs(e));
			this.HandleError("Vote: failed! ", e);
			return false;
		}

		// Token: 0x0600216B RID: 8555 RVA: 0x000ACCA9 File Offset: 0x000AAEA9
		public float GetDownloadProgress(WorkshopItem p_item)
		{
			return this.GetDownloadProgress(p_item.SteamNative.m_nPublishedFileId);
		}

		// Token: 0x0600216C RID: 8556 RVA: 0x000ACCBC File Offset: 0x000AAEBC
		public float GetDownloadProgress(PublishedFileId_t p_fileId)
		{
			if (SteamManager.Initialized)
			{
				EItemState itemState = (EItemState)SteamUGC.GetItemState(p_fileId);
				if (this.IsDownloading(itemState))
				{
					ulong num;
					ulong num2;
					if (SteamUGC.GetItemDownloadInfo(p_fileId, out num, out num2) && num2 != 0UL)
					{
						return num / num2;
					}
					return 0f;
				}
				else if (this.IsInstalled(itemState))
				{
					return 1f;
				}
			}
			return 0f;
		}

		// Token: 0x0600216D RID: 8557 RVA: 0x000ACD14 File Offset: 0x000AAF14
		public float GetUploadProgress(WorkshopItemUpdate p_itemUpdate)
		{
			if (SteamManager.Initialized && p_itemUpdate.SteamNative.m_uploadHandle != UGCUpdateHandle_t.Invalid)
			{
				ulong num;
				ulong num2;
				EItemUpdateStatus itemUpdateProgress = SteamUGC.GetItemUpdateProgress(p_itemUpdate.SteamNative.m_uploadHandle, out num, out num2);
				if (itemUpdateProgress != EItemUpdateStatus.k_EItemUpdateStatusInvalid)
				{
					p_itemUpdate.SteamNative.m_lastValidUpdateStatus = itemUpdateProgress;
				}
				switch (itemUpdateProgress)
				{
				case EItemUpdateStatus.k_EItemUpdateStatusPreparingConfig:
					return 0f;
				case EItemUpdateStatus.k_EItemUpdateStatusPreparingContent:
					return ((num2 > 0UL) ? (num / num2) : 0f) * 0.1f;
				case EItemUpdateStatus.k_EItemUpdateStatusUploadingContent:
					return ((num2 > 0UL) ? (num / num2) : 0f) * 0.65f + 0.1f;
				case EItemUpdateStatus.k_EItemUpdateStatusUploadingPreviewFile:
					return ((num2 > 0UL) ? (num / num2) : 0f) * 0.15f + 0.75f;
				case EItemUpdateStatus.k_EItemUpdateStatusCommittingChanges:
					return ((num2 > 0UL) ? (num / num2) : 0f) * 0.1f + 0.9f;
				default:
					if (p_itemUpdate.SteamNative.m_lastValidUpdateStatus != EItemUpdateStatus.k_EItemUpdateStatusInvalid)
					{
						return 1f;
					}
					break;
				}
			}
			return 0f;
		}

		// Token: 0x0600216E RID: 8558 RVA: 0x000ACE24 File Offset: 0x000AB024
		public bool Upload(WorkshopItemUpdate p_itemData, Action<WorkshopItemUpdateEventArgs> p_onUploaded)
		{
			if (!SteamManager.Initialized)
			{
				LapinerTools.Steam.Data.ErrorEventArgs e = LapinerTools.Steam.Data.ErrorEventArgs.CreateSteamNotInit();
				this.InvokeEventHandlerSafely<WorkshopItemUpdateEventArgs>(p_onUploaded, new WorkshopItemUpdateEventArgs(e));
				this.HandleError("Upload: failed! ", e);
				return false;
			}
			bool flag = false;
			if (!string.IsNullOrEmpty(p_itemData.ContentPath))
			{
				string[] files = Directory.GetFiles(p_itemData.ContentPath);
				for (int i = 0; i < files.Length; i++)
				{
					if (!object.Equals(files[i], p_itemData.IconPath))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				LapinerTools.Steam.Data.ErrorEventArgs e2 = new LapinerTools.Steam.Data.ErrorEventArgs("No content to upload found! WorkshopItemUpdate.ContentPath is set to '" + p_itemData.ContentPath + "'!");
				this.InvokeEventHandlerSafely<WorkshopItemUpdateEventArgs>(p_onUploaded, new WorkshopItemUpdateEventArgs(e2));
				this.HandleError("Upload: failed! ", e2);
				return false;
			}
			this.m_uploadItemData = p_itemData;
			if (this.m_uploadItemData.SteamNative.m_nPublishedFileId == PublishedFileId_t.Invalid)
			{
				this.SetSingleShotEventHandler<WorkshopItemUpdateEventArgs>("OnUploaded", ref this.OnUploaded, p_onUploaded);
				base.Execute<CreateItemResult_t>(SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeFirst), new CallResult<CreateItemResult_t>.APIDispatchDelegate(this.OnCreateItemCompleted));
			}
			else
			{
				if (!string.IsNullOrEmpty(this.m_uploadItemData.ContentPath))
				{
					using (FileStream fileStream = new FileStream(Path.Combine(this.m_uploadItemData.ContentPath, "WorkshopItemInfo.xml"), FileMode.Create))
					{
						new XmlSerializer(typeof(WorkshopItemInfo)).Serialize(fileStream, new WorkshopItemInfo
						{
							PublishedFileId = this.m_uploadItemData.SteamNative.m_nPublishedFileId.m_PublishedFileId,
							Name = this.m_uploadItemData.Name,
							Description = this.m_uploadItemData.Description,
							IconFileName = ((!string.IsNullOrEmpty(this.m_uploadItemData.IconPath)) ? Path.GetFileName(this.m_uploadItemData.IconPath) : ""),
							Tags = ((this.m_uploadItemData.Tags != null) ? this.m_uploadItemData.Tags.ToArray() : new string[0])
						});
					}
				}
				UGCUpdateHandle_t ugcupdateHandle_t = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), this.m_uploadItemData.SteamNative.m_nPublishedFileId);
				this.m_uploadItemData.SteamNative.m_uploadHandle = ugcupdateHandle_t;
				bool flag2 = SteamUGC.SetItemVisibility(ugcupdateHandle_t, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic);
				bool flag3 = !string.IsNullOrEmpty(this.m_uploadItemData.Name) && SteamUGC.SetItemTitle(ugcupdateHandle_t, this.m_uploadItemData.Name);
				bool flag4 = !string.IsNullOrEmpty(this.m_uploadItemData.Description) && SteamUGC.SetItemDescription(ugcupdateHandle_t, this.m_uploadItemData.Description);
				bool flag5 = !string.IsNullOrEmpty(this.m_uploadItemData.IconPath) && SteamUGC.SetItemPreview(ugcupdateHandle_t, this.m_uploadItemData.IconPath);
				bool flag6 = !string.IsNullOrEmpty(this.m_uploadItemData.ContentPath) && SteamUGC.SetItemContent(ugcupdateHandle_t, this.m_uploadItemData.ContentPath);
				bool flag7 = this.m_uploadItemData.Tags != null && this.m_uploadItemData.Tags.Count > 0 && SteamUGC.SetItemTags(ugcupdateHandle_t, this.m_uploadItemData.Tags, false);
				if (!flag2)
				{
					this.HandleError("Upload: ", new LapinerTools.Steam.Data.ErrorEventArgs("Could not set item visibility to 'public'!"));
				}
				if (!string.IsNullOrEmpty(this.m_uploadItemData.Name) && !flag3)
				{
					this.HandleError("Upload: ", new LapinerTools.Steam.Data.ErrorEventArgs("Could not set item title to '" + this.m_uploadItemData.Name + "'!"));
				}
				if (!string.IsNullOrEmpty(this.m_uploadItemData.Description) && !flag4)
				{
					this.HandleError("Upload: ", new LapinerTools.Steam.Data.ErrorEventArgs("Could not set item description to '" + this.m_uploadItemData.Description + "'!"));
				}
				if (!string.IsNullOrEmpty(this.m_uploadItemData.IconPath) && !flag5)
				{
					this.HandleError("Upload: ", new LapinerTools.Steam.Data.ErrorEventArgs("Could not set item icon path to '" + this.m_uploadItemData.IconPath + "'!"));
				}
				if (!string.IsNullOrEmpty(this.m_uploadItemData.ContentPath) && !flag6)
				{
					this.HandleError("Upload: ", new LapinerTools.Steam.Data.ErrorEventArgs("Could not set item content path to '" + this.m_uploadItemData.ContentPath + "'!"));
				}
				if (this.m_uploadItemData.Tags != null && this.m_uploadItemData.Tags.Count > 0 && !flag7)
				{
					this.HandleError("Upload: ", new LapinerTools.Steam.Data.ErrorEventArgs("Could not set item tags!"));
				}
				if (base.IsDebugLogEnabled)
				{
					Debug.Log("Upload: starting...");
				}
				this.SetSingleShotEventHandler<WorkshopItemUpdateEventArgs>("OnUploaded", ref this.OnUploaded, p_onUploaded);
				base.Execute<SubmitItemUpdateResult_t>(SteamUGC.SubmitItemUpdate(ugcupdateHandle_t, p_itemData.ChangeNote), new CallResult<SubmitItemUpdateResult_t>.APIDispatchDelegate(this.OnItemUpdateCompleted));
			}
			return true;
		}

		// Token: 0x0600216F RID: 8559 RVA: 0x000AD2D4 File Offset: 0x000AB4D4
		public void RenderIcon(Camera p_camera, int p_width, int p_height, string p_saveToFilePath, Action<Texture2D> p_onRenderIconCompleted)
		{
			base.StartCoroutine(this.RenderIconRoutine(p_camera, p_width, p_height, p_saveToFilePath, true, p_onRenderIconCompleted));
		}

		// Token: 0x06002170 RID: 8560 RVA: 0x000AD2EB File Offset: 0x000AB4EB
		public void RenderIcon(Camera p_camera, int p_width, int p_height, string p_saveToFilePath, bool p_keepTextureReference, Action<Texture2D> p_onRenderIconCompleted)
		{
			base.StartCoroutine(this.RenderIconRoutine(p_camera, p_width, p_height, p_saveToFilePath, p_keepTextureReference, p_onRenderIconCompleted));
		}

		// Token: 0x06002171 RID: 8561 RVA: 0x000AD304 File Offset: 0x000AB504
		public WorkshopItemUpdate GetItemUpdateFromFolder(string p_itemContentFolderPath)
		{
			WorkshopItemUpdate workshopItemUpdate = null;
			string text = Path.Combine(p_itemContentFolderPath, "WorkshopItemInfo.xml");
			if (File.Exists(text))
			{
				try
				{
					using (FileStream fileStream = new FileStream(text, FileMode.Open))
					{
						WorkshopItemInfo workshopItemInfo = new XmlSerializer(typeof(WorkshopItemInfo)).Deserialize(fileStream) as WorkshopItemInfo;
						workshopItemUpdate = new WorkshopItemUpdate(new PublishedFileId_t(workshopItemInfo.PublishedFileId))
						{
							Name = workshopItemInfo.Name,
							Description = workshopItemInfo.Description,
							ContentPath = p_itemContentFolderPath
						};
						if (!string.IsNullOrEmpty(workshopItemInfo.IconFileName))
						{
							string text2 = Path.Combine(p_itemContentFolderPath, workshopItemInfo.IconFileName);
							if (File.Exists(text2))
							{
								workshopItemUpdate.IconPath = text2;
							}
						}
					}
					return workshopItemUpdate;
				}
				catch (Exception ex)
				{
					Debug.LogError("SteamWorkshopMain: GetItemUpdateFromFolder: could not parse item info at '" + text + "'!\n" + ex.Message);
					return workshopItemUpdate;
				}
			}
			Debug.LogError("SteamWorkshopMain: GetItemUpdateFromFolder: could not find item info at '" + text + "'!");
			return workshopItemUpdate;
		}

		// Token: 0x06002172 RID: 8562 RVA: 0x000AD40C File Offset: 0x000AB60C
		protected override void LateUpdate()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				for (int i = this.m_downloadingItems.Count - 1; i >= 0; i--)
				{
					PublishedFileId_t publishedFileId_t = this.m_downloadingItems[i];
					WorkshopItem workshopItem;
					if (this.m_items.TryGetValue(publishedFileId_t, out workshopItem))
					{
						EItemState itemState = (EItemState)SteamUGC.GetItemState(publishedFileId_t);
						if (itemState != workshopItem.SteamNative.m_itemState || this.IsInstalled(itemState))
						{
							workshopItem.SteamNative.m_itemState = itemState;
							workshopItem.IsInstalled = this.IsInstalled(itemState);
							workshopItem.IsDownloading = this.IsDownloading(itemState);
							workshopItem.IsUpdateNeeded = this.IsUpdateNeeded(itemState);
							if (workshopItem.IsInstalled)
							{
								DateTime dateTime = DateTime.MinValue;
								ulong num;
								string text;
								uint num2;
								if (SteamUGC.GetItemInstallInfo(publishedFileId_t, out num, out text, 260U, out num2))
								{
									dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
									dateTime = dateTime.AddSeconds(num2).ToLocalTime();
								}
								workshopItem.InstalledLocalFolder = text;
								workshopItem.InstalledSizeOnDisk = num;
								workshopItem.InstalledTimestamp = dateTime;
								this.m_downloadingItems.RemoveAt(i);
								if (base.IsDebugLogEnabled)
								{
									string text2 = "SteamWorkshopMain: item installed ";
									PublishedFileId_t publishedFileId_t2 = publishedFileId_t;
									Debug.Log(text2 + publishedFileId_t2.ToString() + ((this.OnInstalled != null) ? " (will notify)" : " (no listeners)"));
								}
								if (this.OnInstalled != null)
								{
									this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(this.OnInstalled, new WorkshopItemEventArgs(workshopItem));
									string text3 = "OnInstalled";
									PublishedFileId_t publishedFileId_t2 = publishedFileId_t;
									this.ClearSingleShotEventHandlers<WorkshopItemEventArgs>(text3 + publishedFileId_t2.ToString(), ref this.OnInstalled);
								}
							}
						}
					}
					else
					{
						this.m_downloadingItems.RemoveAt(i);
					}
				}
				int num3 = this.m_pendingRequests.Count<GetUserItemVoteResult_t>();
				base.LateUpdate();
				int num4 = this.m_pendingRequests.Count<GetUserItemVoteResult_t>();
				if (num3 > 0 && num4 == 0)
				{
					this.QueryAllItems();
				}
				if (base.IsDebugLogEnabled && Time.frameCount % 300 == 0 && this.m_downloadingItems.Count > 0)
				{
					Debug.Log("Pending downloads left: " + this.m_downloadingItems.Count.ToString());
				}
			}
		}

		// Token: 0x06002173 RID: 8563 RVA: 0x000AD668 File Offset: 0x000AB868
		private void OnDestroy()
		{
			if (this.m_renderedTexture != null)
			{
				global::UnityEngine.Object.Destroy(this.m_renderedTexture);
				this.m_renderedTexture = null;
			}
		}

		// Token: 0x06002174 RID: 8564 RVA: 0x000AD68C File Offset: 0x000AB88C
		private void OnAvailableItemsCallCompleted(SteamUGCQueryCompleted_t p_callback, bool p_bIOFailure)
		{
			if (this.CheckAndLogResult<SteamUGCQueryCompleted_t, WorkshopItemListEventArgs>("OnAvailableItemsCallCompleted", p_callback.m_eResult, p_bIOFailure, "OnItemListLoaded", ref this.OnItemListLoaded))
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					this.m_reqItemList.PagesItems = this.GetPageCount(p_callback);
					for (uint num = 0U; num < p_callback.m_unNumResultsReturned; num += 1U)
					{
						SteamUGCDetails_t steamUGCDetails_t;
						if (SteamUGC.GetQueryUGCResult(p_callback.m_handle, num, out steamUGCDetails_t))
						{
							WorkshopItem item = this.ParseItem(p_callback.m_handle, num, steamUGCDetails_t);
							if ((this.m_sorting.SOURCE != EWorkshopSource.OWNED || item.IsOwned) && this.CanAddRequestedItemToList(item))
							{
								Debug.Log("Retrieved " + item.Name + " from Steam Workshop_OnAvailableItemsCallCompleted");
								this.m_reqItemList.Items.Add(item);
							}
							this.m_items[item.SteamNative.m_nPublishedFileId] = item;
							item.IsFavorited = this.m_reqItemList.ItemsFavorited.Where<WorkshopItem>((WorkshopItem flvl) => flvl.SteamNative.m_nPublishedFileId == item.SteamNative.m_nPublishedFileId).FirstOrDefault<WorkshopItem>() != null;
							WorkshopItem workshopItem = this.m_reqItemList.ItemsVoted.Where<WorkshopItem>((WorkshopItem flvl) => flvl.SteamNative.m_nPublishedFileId == item.SteamNative.m_nPublishedFileId).FirstOrDefault<WorkshopItem>();
							if (workshopItem != null)
							{
								item.IsVotedUp = workshopItem.IsVotedUp;
								item.IsVotedDown = workshopItem.IsVotedDown;
								item.IsVoteSkipped = workshopItem.IsVoteSkipped;
							}
						}
					}
					if (this.OnItemListLoaded != null)
					{
						this.InvokeEventHandlerSafely<WorkshopItemListEventArgs>(this.OnItemListLoaded, new WorkshopItemListEventArgs
						{
							ItemList = this.m_reqItemList
						});
						this.ClearSingleShotEventHandlers<WorkshopItemListEventArgs>("OnItemListLoaded", ref this.OnItemListLoaded);
						if (base.IsDebugLogEnabled)
						{
							Debug.Log(string.Concat(new string[]
							{
								"OnAvailableItemsCallCompleted: loaded ",
								this.m_reqItemList.Items.Count.ToString(),
								" items from page ",
								this.m_reqItemList.Page.ToString(),
								", ",
								this.m_reqItemList.ItemsFavorited.Count.ToString(),
								" favorited by user, ",
								this.m_reqItemList.ItemsVoted.Count.ToString(),
								" voted by user"
							}));
						}
					}
					this.m_reqItemList = null;
					this.m_pendingRequests.Clear<GetUserItemVoteResult_t>();
				}
			}
		}

		// Token: 0x06002175 RID: 8565 RVA: 0x000AD960 File Offset: 0x000ABB60
		private void OnPublishedItemsCallCompleted(SteamUGCQueryCompleted_t p_callback, bool p_bIOFailure)
		{
			if (this.CheckAndLogResult<SteamUGCQueryCompleted_t, WorkshopItemListEventArgs>("OnPublishedItemsCallCompleted", p_callback.m_eResult, p_bIOFailure, "OnItemListLoaded", ref this.OnItemListLoaded))
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					if (this.m_reqItemList.PagesItemsFavorited == 0U)
					{
						this.m_reqItemList.PagesItemsFavorited = this.GetPageCount(p_callback);
					}
					for (uint num = 0U; num < p_callback.m_unNumResultsReturned; num += 1U)
					{
						SteamUGCDetails_t steamUGCDetails_t;
						if (SteamUGC.GetQueryUGCResult(p_callback.m_handle, num, out steamUGCDetails_t))
						{
							WorkshopItem workshopItem = this.ParseItem(p_callback.m_handle, num, steamUGCDetails_t);
							if (workshopItem.IsOwned && this.CanAddRequestedItemToList(workshopItem))
							{
								this.m_reqItemList.Items.Add(workshopItem);
								Debug.Log("Retrieved " + workshopItem.Name + " from Steam Workshop_OnPublishedItemsCallCompleted");
								this.m_items[workshopItem.SteamNative.m_nPublishedFileId] = workshopItem;
							}
						}
					}
					if (this.m_reqPage >= 1U)
					{
						if (this.m_sorting.SOURCE != EWorkshopSource.SUBSCRIBED)
						{
							UGCQueryHandle_t ugcqueryHandle_t = SteamUGC.CreateQueryAllUGCRequest(this.m_sorting.MODE, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, AppId_t.Invalid, SteamUtils.GetAppID(), this.m_reqPage);
							if (this.m_searchText != null && !string.IsNullOrEmpty(this.m_searchText.Trim()))
							{
								SteamUGC.SetSearchText(ugcqueryHandle_t, this.m_searchText);
							}
							if (this.m_searchTags != null && this.m_searchTags.Count > 0)
							{
								SteamUGC.SetMatchAnyTag(ugcqueryHandle_t, this.m_searchMatchAnyTag);
								for (int i = 0; i < this.m_searchTags.Count; i++)
								{
									SteamUGC.AddRequiredTag(ugcqueryHandle_t, this.m_searchTags[i]);
								}
							}
							if (!this.m_isSteamCacheEnabled)
							{
								SteamUGC.SetAllowCachedResponse(ugcqueryHandle_t, 0U);
							}
							SteamUGC.SetReturnLongDescription(ugcqueryHandle_t, true);
							base.Execute<SteamUGCQueryCompleted_t>(SteamUGC.SendQueryUGCRequest(ugcqueryHandle_t), new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(this.OnAvailableItemsCallCompleted));
						}
						if (!SteamUser.BLoggedOn())
						{
							this.HandleError("OnFavoriteItemsCallCompleted: user is offline, user votes will not be loaded! ", LapinerTools.Steam.Data.ErrorEventArgs.Create(EResult.k_EResultNotLoggedOn));
						}
					}
					else
					{
						this.QueryFavoritedItems(this.m_reqPage + 1U);
					}
				}
			}
		}

		// Token: 0x06002176 RID: 8566 RVA: 0x000ADB90 File Offset: 0x000ABD90
		private void OnFavoriteItemsCallCompleted(SteamUGCQueryCompleted_t p_callback, bool p_bIOFailure)
		{
			if (this.CheckAndLogResult<SteamUGCQueryCompleted_t, WorkshopItemListEventArgs>("OnFavoriteItemsCallCompleted", p_callback.m_eResult, p_bIOFailure, "OnItemListLoaded", ref this.OnItemListLoaded))
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					if (this.m_reqItemList.PagesItemsFavorited == 0U)
					{
						this.m_reqItemList.PagesItemsFavorited = this.GetPageCount(p_callback);
					}
					for (uint num = 0U; num < p_callback.m_unNumResultsReturned; num += 1U)
					{
						SteamUGCDetails_t steamUGCDetails_t;
						if (SteamUGC.GetQueryUGCResult(p_callback.m_handle, num, out steamUGCDetails_t))
						{
							WorkshopItem workshopItem = this.ParseItem(p_callback.m_handle, num, steamUGCDetails_t);
							this.m_reqItemList.ItemsFavorited.Add(workshopItem);
							this.m_items[workshopItem.SteamNative.m_nPublishedFileId] = workshopItem;
							workshopItem.IsFavorited = true;
						}
					}
					if (this.m_reqPage >= this.m_reqItemList.PagesItemsFavorited)
					{
						if (SteamUser.BLoggedOn())
						{
							this.QueryVotedItems(1U);
						}
						else
						{
							this.QueryAllItems();
							this.HandleError("OnFavoriteItemsCallCompleted: user is offline, user votes will not be loaded! ", LapinerTools.Steam.Data.ErrorEventArgs.Create(EResult.k_EResultNotLoggedOn));
						}
					}
					else
					{
						this.QueryFavoritedItems(this.m_reqPage + 1U);
					}
				}
			}
		}

		// Token: 0x06002177 RID: 8567 RVA: 0x000ADCBC File Offset: 0x000ABEBC
		private void OnSubscribedItemsCallCompleted(SteamUGCQueryCompleted_t p_callback, bool p_bIOFailure)
		{
			if (this.CheckAndLogResult<SteamUGCQueryCompleted_t, WorkshopItemListEventArgs>("OnSubscribedItemsCallCompleted", p_callback.m_eResult, p_bIOFailure, "OnItemListLoaded", ref this.OnItemListLoaded))
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					if (this.m_reqItemList.PagesItemsSubscribed == 0U)
					{
						this.m_reqItemList.PagesItemsSubscribed = this.GetPageCount(p_callback);
					}
					bool flag2 = false;
					Dictionary<string, string> dictionary = new Dictionary<string, string>(TIPlayerProfileManager.subscribedMods);
					if (!ModManager.checkedForModUpdates)
					{
						TIPlayerProfileManager.ClearSubscribedMods();
					}
					for (uint num = 0U; num < p_callback.m_unNumResultsReturned; num += 1U)
					{
						SteamUGCDetails_t steamUGCDetails_t;
						if (SteamUGC.GetQueryUGCResult(p_callback.m_handle, num, out steamUGCDetails_t))
						{
							WorkshopItem workshopItem = this.ParseItem(p_callback.m_handle, num, steamUGCDetails_t);
							if (!ModManager.checkedForModUpdates && !TIPlayerProfileManager.subscribedMods.ContainsKey(workshopItem.SanitizedName))
							{
								TIPlayerProfileManager.subscribedMods.Add(workshopItem.SanitizedName, workshopItem.InstalledLocalFolder);
								flag2 = true;
							}
							this.m_reqItemList.ItemsSubscribed.Add(workshopItem);
							this.m_items[workshopItem.SteamNative.m_nPublishedFileId] = workshopItem;
							workshopItem.IsSubscribed = true;
						}
					}
					using (Dictionary<string, string>.Enumerator enumerator = dictionary.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, string> registeredMod = enumerator.Current;
							if (!TIPlayerProfileManager.subscribedMods.ContainsKey(registeredMod.Key))
							{
								StartMenuController startMenuController = global::UnityEngine.Object.FindObjectOfType<StartMenuController>();
								if (!Directory.Exists(registeredMod.Value) && ModManager.ModNames.Contains(registeredMod.Key))
								{
									try
									{
										Debug.Log("deleting enabled unsubscribed mod: " + registeredMod.Key);
										if (startMenuController != null)
										{
											startMenuController.ShowModFailureDialog("", Loc.T("UI.StartScreen.Mods.UninstallModsNotice"));
										}
										Directory.Delete(ModManager.ModDirectories.Where<string>((string x) => x == TIUtilities.CombineStrings(new string[] { "Mods/Enabled/", registeredMod.Key })).FirstOrDefault<string>(), true);
										flag2 = true;
										continue;
									}
									catch (Exception ex)
									{
										Debug.Log(ex.Message);
										Debug.Log("could not delete, trying to disable");
										Debug.Log("disabling unsubscribed mod: " + registeredMod.Key);
										if (startMenuController != null)
										{
											startMenuController.ShowModFailureDialog("", Loc.T("UI.StartScreen.Mods.UninstallModsNotice"));
										}
										ModManager.TryRemoveMod(ModManager.ModDirectories.Where<string>((string x) => x == TIUtilities.CombineStrings(new string[] { "Mods/Enabled/", registeredMod.Key })).FirstOrDefault<string>());
										if (!TIPlayerProfileManager.modsToUninstall.ContainsKey(registeredMod.Key))
										{
											TIPlayerProfileManager.modsToUninstall.Add(registeredMod.Key, TIUtilities.CombineStrings(new string[] { "Mods/Disabled/", registeredMod.Key }));
										}
										flag2 = true;
										continue;
									}
								}
								if (!Directory.Exists(registeredMod.Value) && ModManager.DisabledModNames.Contains(registeredMod.Key))
								{
									Debug.Log("deleting unsubscribed mod: " + registeredMod.Key);
									if (startMenuController != null)
									{
										startMenuController.ShowModFailureDialog("", Loc.T("UI.StartScreen.Mods.UninstallModsNotice"));
									}
									Directory.Delete(ModManager.DisabledModDirectories.Where<string>((string x) => x == TIUtilities.CombineStrings(new string[] { "Mods/Disabled/", registeredMod.Key })).FirstOrDefault<string>(), true);
									flag2 = true;
								}
							}
						}
					}
					if (flag2)
					{
						TIPlayerProfileManager.SavePlayerConfig();
					}
					if (this.m_reqPage >= this.m_reqItemList.PagesItemsSubscribed)
					{
						if (SteamUser.BLoggedOn())
						{
							this.QueryVotedItems(1U);
						}
						else
						{
							this.QueryAllItems();
							this.HandleError("OnFavoriteItemsCallCompleted: user is offline, user votes will not be loaded! ", LapinerTools.Steam.Data.ErrorEventArgs.Create(EResult.k_EResultNotLoggedOn));
						}
					}
					else
					{
						this.QueryFavoritedItems(this.m_reqPage + 1U);
					}
				}
			}
		}

		// Token: 0x06002178 RID: 8568 RVA: 0x000AE0CC File Offset: 0x000AC2CC
		private void OnVotedItemsCallCompleted(SteamUGCQueryCompleted_t p_callback, bool p_bIOFailure)
		{
			if (this.CheckAndLogResult<SteamUGCQueryCompleted_t, WorkshopItemListEventArgs>("OnVotedItemsCallCompleted", p_callback.m_eResult, p_bIOFailure, "OnItemListLoaded", ref this.OnItemListLoaded))
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					if (this.m_reqItemList.PagesItemsVoted == 0U)
					{
						this.m_reqItemList.PagesItemsVoted = this.GetPageCount(p_callback);
					}
					if (this.m_reqPage < this.m_reqItemList.PagesItemsVoted)
					{
						this.QueryVotedItems(this.m_reqPage + 1U);
					}
					for (uint num = 0U; num < p_callback.m_unNumResultsReturned; num += 1U)
					{
						SteamUGCDetails_t steamUGCDetails_t;
						if (SteamUGC.GetQueryUGCResult(p_callback.m_handle, num, out steamUGCDetails_t))
						{
							WorkshopItem item = this.ParseItem(p_callback.m_handle, num, steamUGCDetails_t);
							this.m_reqItemList.ItemsVoted.Add(item);
							this.m_items[item.SteamNative.m_nPublishedFileId] = item;
							item.IsFavorited = this.m_reqItemList.ItemsFavorited.Where<WorkshopItem>((WorkshopItem flvl) => flvl.SteamNative.m_nPublishedFileId == item.SteamNative.m_nPublishedFileId).FirstOrDefault<WorkshopItem>() != null;
							base.Execute<GetUserItemVoteResult_t>(SteamUGC.GetUserItemVote(steamUGCDetails_t.m_nPublishedFileId), new CallResult<GetUserItemVoteResult_t>.APIDispatchDelegate(this.OnUserVoteCallCompleted));
						}
					}
					if (this.m_pendingRequests.Count<GetUserItemVoteResult_t>() == 0)
					{
						if (base.IsDebugLogEnabled)
						{
							Debug.Log("OnVotedItemsCallCompleted - no user votes found");
						}
						this.QueryAllItems();
					}
					else if (base.IsDebugLogEnabled)
					{
						Debug.Log("OnVotedItemsCallCompleted - started vote requests: " + this.m_pendingRequests.Count<GetUserItemVoteResult_t>().ToString());
					}
				}
			}
		}

		// Token: 0x06002179 RID: 8569 RVA: 0x000AE294 File Offset: 0x000AC494
		private void OnUserVoteCallCompleted(GetUserItemVoteResult_t p_callback, bool p_bIOFailure)
		{
			object obj;
			if (this.CheckAndLogResultNoEvent<GetUserItemVoteResult_t>("OnUserVoteCallCompleted", p_callback.m_eResult, p_bIOFailure))
			{
				obj = this.m_lock;
				lock (obj)
				{
					WorkshopItem workshopItem = this.m_reqItemList.ItemsVoted.Where<WorkshopItem>((WorkshopItem flvl) => flvl.SteamNative.m_nPublishedFileId == p_callback.m_nPublishedFileId).FirstOrDefault<WorkshopItem>();
					if (workshopItem != null)
					{
						workshopItem.IsVotedUp = p_callback.m_bVotedUp;
						workshopItem.IsVotedDown = p_callback.m_bVotedDown;
						workshopItem.IsVoteSkipped = p_callback.m_bVoteSkipped;
					}
				}
			}
			obj = this.m_lock;
			lock (obj)
			{
				if (this.m_pendingRequests.Count<GetUserItemVoteResult_t>() == 0)
				{
					this.QueryAllItems();
				}
			}
		}

		// Token: 0x0600217A RID: 8570 RVA: 0x000AE388 File Offset: 0x000AC588
		private void OnSubscribeCallCompleted(RemoteStorageSubscribePublishedFileResult_t p_callback, bool p_bIOFailure)
		{
			string text = "OnSubscribeCallCompleted";
			EResult eResult = p_callback.m_eResult;
			string text2 = "OnSubscribed";
			PublishedFileId_t publishedFileId_t = p_callback.m_nPublishedFileId;
			if (this.CheckAndLogResult<RemoteStorageSubscribePublishedFileResult_t, WorkshopItemEventArgs>(text, eResult, p_bIOFailure, text2 + publishedFileId_t.ToString(), ref this.OnSubscribed))
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					WorkshopItem workshopItem;
					if (this.m_items.TryGetValue(p_callback.m_nPublishedFileId, out workshopItem))
					{
						workshopItem.IsSubscribed = true;
						EItemState eitemState = (EItemState)SteamUGC.GetItemState(p_callback.m_nPublishedFileId);
						workshopItem.SteamNative.m_itemState = eitemState;
						workshopItem.IsInstalled = this.IsInstalled(eitemState);
						workshopItem.IsDownloading = this.IsDownloading(eitemState);
						workshopItem.IsUpdateNeeded = this.IsUpdateNeeded(eitemState);
						if ((workshopItem.IsUpdateNeeded || !workshopItem.IsInstalled) && SteamUGC.DownloadItem(p_callback.m_nPublishedFileId, true))
						{
							if (base.IsDebugLogEnabled)
							{
								string text3 = "OnSubscribeCallCompleted: started download for ";
								publishedFileId_t = p_callback.m_nPublishedFileId;
								Debug.Log(text3 + publishedFileId_t.ToString());
							}
							if (!this.m_downloadingItems.Contains(p_callback.m_nPublishedFileId))
							{
								this.m_downloadingItems.Add(p_callback.m_nPublishedFileId);
							}
							eitemState = (EItemState)SteamUGC.GetItemState(p_callback.m_nPublishedFileId);
							workshopItem.SteamNative.m_itemState = eitemState;
							workshopItem.IsInstalled = this.IsInstalled(eitemState);
							workshopItem.IsDownloading = this.IsDownloading(eitemState);
							workshopItem.IsUpdateNeeded = this.IsUpdateNeeded(eitemState);
						}
						else if (base.IsDebugLogEnabled)
						{
							string text4 = "OnSubscribeCallCompleted: subscribed to already installed item ";
							publishedFileId_t = p_callback.m_nPublishedFileId;
							Debug.Log(text4 + publishedFileId_t.ToString());
						}
						if (this.OnSubscribed != null)
						{
							this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(this.OnSubscribed, new WorkshopItemEventArgs(workshopItem));
							string text5 = "OnSubscribed";
							publishedFileId_t = p_callback.m_nPublishedFileId;
							this.ClearSingleShotEventHandlers<WorkshopItemEventArgs>(text5 + publishedFileId_t.ToString(), ref this.OnSubscribed);
						}
					}
					else
					{
						LapinerTools.Steam.Data.ErrorEventArgs e = new LapinerTools.Steam.Data.ErrorEventArgs("Could not find item!");
						this.HandleError("OnSubscribeCallCompleted: failed! ", e);
						if (this.OnSubscribed != null)
						{
							string text6 = "OnSubscribed";
							publishedFileId_t = p_callback.m_nPublishedFileId;
							this.CallSingleShotEventHandlers<WorkshopItemEventArgs>(text6 + publishedFileId_t.ToString(), new WorkshopItemEventArgs(e), ref this.OnSubscribed);
						}
					}
				}
			}
		}

		// Token: 0x0600217B RID: 8571 RVA: 0x000AE5EC File Offset: 0x000AC7EC
		private void OnUnsubscribeCallCompleted(RemoteStorageUnsubscribePublishedFileResult_t p_callback, bool p_bIOFailure)
		{
			string text = "OnUnsubscribeCallCompleted";
			EResult eResult = p_callback.m_eResult;
			string text2 = "OnUnsubscribed";
			PublishedFileId_t publishedFileId_t = p_callback.m_nPublishedFileId;
			if (this.CheckAndLogResult<RemoteStorageUnsubscribePublishedFileResult_t, WorkshopItemEventArgs>(text, eResult, p_bIOFailure, text2 + publishedFileId_t.ToString(), ref this.OnUnsubscribed))
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					WorkshopItem workshopItem;
					if (this.m_items.TryGetValue(p_callback.m_nPublishedFileId, out workshopItem))
					{
						workshopItem.IsSubscribed = false;
						EItemState itemState = (EItemState)SteamUGC.GetItemState(p_callback.m_nPublishedFileId);
						workshopItem.SteamNative.m_itemState = itemState;
						workshopItem.IsInstalled = this.IsInstalled(itemState);
						workshopItem.IsDownloading = this.IsDownloading(itemState);
						workshopItem.IsUpdateNeeded = this.IsUpdateNeeded(itemState);
						if (this.OnUnsubscribed != null)
						{
							this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(this.OnUnsubscribed, new WorkshopItemEventArgs(workshopItem));
							string text3 = "OnUnsubscribed";
							publishedFileId_t = p_callback.m_nPublishedFileId;
							this.ClearSingleShotEventHandlers<WorkshopItemEventArgs>(text3 + publishedFileId_t.ToString(), ref this.OnUnsubscribed);
						}
					}
					else
					{
						LapinerTools.Steam.Data.ErrorEventArgs e = new LapinerTools.Steam.Data.ErrorEventArgs("Could not find subscribed item!");
						this.HandleError("OnUnsubscribeCallCompleted: failed! ", e);
						if (this.OnUnsubscribed != null)
						{
							string text4 = "OnUnsubscribed";
							publishedFileId_t = p_callback.m_nPublishedFileId;
							this.CallSingleShotEventHandlers<WorkshopItemEventArgs>(text4 + publishedFileId_t.ToString(), new WorkshopItemEventArgs(e), ref this.OnUnsubscribed);
						}
					}
				}
			}
		}

		// Token: 0x0600217C RID: 8572 RVA: 0x000AE76C File Offset: 0x000AC96C
		private void OnFavoriteChangeCallCompleted(UserFavoriteItemsListChanged_t p_callback, bool p_bIOFailure)
		{
			WorkshopItem workshopItem;
			this.m_items.TryGetValue(p_callback.m_nPublishedFileId, out workshopItem);
			if (this.CheckAndLogResultNoEvent<UserFavoriteItemsListChanged_t>("OnFavoriteChangeCallCompleted", p_callback.m_eResult, p_bIOFailure))
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					if (workshopItem != null)
					{
						workshopItem.IsFavorited = p_callback.m_bWasAddRequest;
						if (workshopItem.IsFavorited)
						{
							if (this.OnAddedFavorite != null)
							{
								this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(this.OnAddedFavorite, new WorkshopItemEventArgs(workshopItem));
								string text = "OnAddedFavorite";
								PublishedFileId_t publishedFileId_t = p_callback.m_nPublishedFileId;
								this.ClearSingleShotEventHandlers<WorkshopItemEventArgs>(text + publishedFileId_t.ToString(), ref this.OnAddedFavorite);
							}
						}
						else if (this.OnRemovedFavorite != null)
						{
							this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(this.OnRemovedFavorite, new WorkshopItemEventArgs(workshopItem));
							string text2 = "OnRemovedFavorite";
							PublishedFileId_t publishedFileId_t = p_callback.m_nPublishedFileId;
							this.ClearSingleShotEventHandlers<WorkshopItemEventArgs>(text2 + publishedFileId_t.ToString(), ref this.OnRemovedFavorite);
						}
					}
					else
					{
						LapinerTools.Steam.Data.ErrorEventArgs e = new LapinerTools.Steam.Data.ErrorEventArgs("Could not find changed item!");
						this.HandleError("OnFavoriteChangeCallCompleted: failed! ", e);
						if (this.OnAddedFavorite != null)
						{
							string text3 = "OnAddedFavorite";
							PublishedFileId_t publishedFileId_t = p_callback.m_nPublishedFileId;
							this.CallSingleShotEventHandlers<WorkshopItemEventArgs>(text3 + publishedFileId_t.ToString(), new WorkshopItemEventArgs(e), ref this.OnAddedFavorite);
						}
						if (this.OnRemovedFavorite != null)
						{
							string text4 = "OnRemovedFavorite";
							PublishedFileId_t publishedFileId_t = p_callback.m_nPublishedFileId;
							this.CallSingleShotEventHandlers<WorkshopItemEventArgs>(text4 + publishedFileId_t.ToString(), new WorkshopItemEventArgs(e), ref this.OnRemovedFavorite);
						}
					}
					return;
				}
			}
			LapinerTools.Steam.Data.ErrorEventArgs e2 = LapinerTools.Steam.Data.ErrorEventArgs.Create(p_callback.m_eResult);
			if (this.OnAddedFavorite != null)
			{
				string text5 = "OnAddedFavorite";
				PublishedFileId_t publishedFileId_t = p_callback.m_nPublishedFileId;
				this.CallSingleShotEventHandlers<WorkshopItemEventArgs>(text5 + publishedFileId_t.ToString(), new WorkshopItemEventArgs(e2), ref this.OnAddedFavorite);
			}
			if (this.OnRemovedFavorite != null)
			{
				string text6 = "OnRemovedFavorite";
				PublishedFileId_t publishedFileId_t = p_callback.m_nPublishedFileId;
				this.CallSingleShotEventHandlers<WorkshopItemEventArgs>(text6 + publishedFileId_t.ToString(), new WorkshopItemEventArgs(e2), ref this.OnRemovedFavorite);
			}
		}

		// Token: 0x0600217D RID: 8573 RVA: 0x000AE99C File Offset: 0x000ACB9C
		private void OnVoteCallCompleted(SetUserItemVoteResult_t p_callback, bool p_bIOFailure)
		{
			string text = "OnVoteCallCompleted";
			EResult eResult = p_callback.m_eResult;
			string text2 = "OnVoted";
			PublishedFileId_t publishedFileId_t = p_callback.m_nPublishedFileId;
			if (this.CheckAndLogResult<SetUserItemVoteResult_t, WorkshopItemEventArgs>(text, eResult, p_bIOFailure, text2 + publishedFileId_t.ToString(), ref this.OnVoted))
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					WorkshopItem workshopItem;
					if (this.m_items.TryGetValue(p_callback.m_nPublishedFileId, out workshopItem))
					{
						workshopItem.IsVotedUp = p_callback.m_bVoteUp;
						workshopItem.IsVotedDown = !p_callback.m_bVoteUp;
						workshopItem.IsVoteSkipped = false;
						if (this.OnVoted != null)
						{
							this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(this.OnVoted, new WorkshopItemEventArgs(workshopItem));
							string text3 = "OnVoted";
							publishedFileId_t = p_callback.m_nPublishedFileId;
							this.ClearSingleShotEventHandlers<WorkshopItemEventArgs>(text3 + publishedFileId_t.ToString(), ref this.OnVoted);
						}
					}
					else
					{
						LapinerTools.Steam.Data.ErrorEventArgs e = new LapinerTools.Steam.Data.ErrorEventArgs("Could not find voted item!");
						this.HandleError("OnVoteCallCompleted: failed! ", e);
						if (this.OnVoted != null)
						{
							string text4 = "OnVoted";
							publishedFileId_t = p_callback.m_nPublishedFileId;
							this.CallSingleShotEventHandlers<WorkshopItemEventArgs>(text4 + publishedFileId_t.ToString(), new WorkshopItemEventArgs(e), ref this.OnVoted);
						}
					}
				}
			}
		}

		// Token: 0x0600217E RID: 8574 RVA: 0x000AEAE4 File Offset: 0x000ACCE4
		private void OnCreateItemCompleted(CreateItemResult_t p_callback, bool p_bIOFailure)
		{
			if (p_callback.m_bUserNeedsToAcceptWorkshopLegalAgreement)
			{
				Application.OpenURL("https://steamcommunity.com/workshop/workshoplegalagreement/");
				LapinerTools.Steam.Data.ErrorEventArgs e = LapinerTools.Steam.Data.ErrorEventArgs.CreateWorkshopLegalAgreement();
				this.HandleError("OnCreateItemCompleted: failed! ", e);
				if (this.OnUploaded != null)
				{
					this.CallSingleShotEventHandlers<WorkshopItemUpdateEventArgs>("OnUploaded", new WorkshopItemUpdateEventArgs(e), ref this.OnUploaded);
					return;
				}
			}
			else if (this.CheckAndLogResult<SetUserItemVoteResult_t, WorkshopItemUpdateEventArgs>("OnCreateItemCompleted", p_callback.m_eResult, p_bIOFailure, "OnUploaded", ref this.OnUploaded))
			{
				this.m_uploadItemData.SteamNative.m_nPublishedFileId = p_callback.m_nPublishedFileId;
				this.Upload(this.m_uploadItemData, null);
			}
		}

		// Token: 0x0600217F RID: 8575 RVA: 0x000AEB78 File Offset: 0x000ACD78
		private void OnItemUpdateCompleted(SubmitItemUpdateResult_t p_callback, bool p_bIOFailure)
		{
			if (p_callback.m_bUserNeedsToAcceptWorkshopLegalAgreement)
			{
				Application.OpenURL("https://steamcommunity.com/workshop/workshoplegalagreement/");
				LapinerTools.Steam.Data.ErrorEventArgs e = LapinerTools.Steam.Data.ErrorEventArgs.CreateWorkshopLegalAgreement();
				this.HandleError("OnItemUpdateCompleted: failed! ", e);
				if (this.OnUploaded != null)
				{
					this.CallSingleShotEventHandlers<WorkshopItemUpdateEventArgs>("OnUploaded", new WorkshopItemUpdateEventArgs(e), ref this.OnUploaded);
					return;
				}
			}
			else if (this.CheckAndLogResult<SetUserItemVoteResult_t, WorkshopItemUpdateEventArgs>("OnItemUpdateCompleted (" + this.m_uploadItemData.Name + ")", p_callback.m_eResult, p_bIOFailure, "OnUploaded", ref this.OnUploaded) && this.OnUploaded != null)
			{
				this.InvokeEventHandlerSafely<WorkshopItemUpdateEventArgs>(this.OnUploaded, new WorkshopItemUpdateEventArgs
				{
					Item = this.m_uploadItemData
				});
				this.ClearSingleShotEventHandlers<WorkshopItemUpdateEventArgs>("OnUploaded", ref this.OnUploaded);
			}
		}

		// Token: 0x06002180 RID: 8576 RVA: 0x000AEC38 File Offset: 0x000ACE38
		private WorkshopItem ParseItem(UGCQueryHandle_t p_handle, uint p_indexInHandle, SteamUGCDetails_t p_itemDetails)
		{
			string friendPersonaName = SteamFriends.GetFriendPersonaName(new CSteamID(p_itemDetails.m_ulSteamIDOwner));
			ulong num;
			if (!SteamUGC.GetQueryUGCStatistic(p_handle, p_indexInHandle, EItemStatistic.k_EItemStatistic_NumFavorites, out num))
			{
				num = 0UL;
			}
			ulong num2;
			if (!SteamUGC.GetQueryUGCStatistic(p_handle, p_indexInHandle, EItemStatistic.k_EItemStatistic_NumSubscriptions, out num2))
			{
				num2 = 0UL;
			}
			string text;
			if (!SteamUGC.GetQueryUGCPreviewURL(p_handle, p_indexInHandle, out text, 1024U))
			{
				text = "";
			}
			bool flag = (SteamUGC.GetItemState(p_itemDetails.m_nPublishedFileId) & 1U) > 0U;
			EItemState itemState = (EItemState)SteamUGC.GetItemState(p_itemDetails.m_nPublishedFileId);
			DateTime dateTime = DateTime.MinValue;
			ulong num3;
			string text2;
			uint num4;
			if (SteamUGC.GetItemInstallInfo(p_itemDetails.m_nPublishedFileId, out num3, out text2, 260U, out num4))
			{
				dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
				dateTime = dateTime.AddSeconds(num4).ToLocalTime();
			}
			return new WorkshopItem
			{
				SteamNative = new WorkshopItem.SteamNativeData(p_itemDetails.m_nPublishedFileId)
				{
					m_details = p_itemDetails,
					m_itemState = itemState
				},
				Name = p_itemDetails.m_rgchTitle,
				Description = p_itemDetails.m_rgchDescription,
				OwnerName = friendPersonaName,
				IsOwned = (p_itemDetails.m_ulSteamIDOwner == SteamUser.GetSteamID().m_SteamID),
				PreviewImageURL = text,
				VotesUp = p_itemDetails.m_unVotesUp,
				VotesDown = p_itemDetails.m_unVotesDown,
				Subscriptions = num2,
				Favorites = num,
				IsSubscribed = flag,
				IsInstalled = this.IsInstalled(itemState),
				IsDownloading = this.IsDownloading(itemState),
				IsUpdateNeeded = this.IsUpdateNeeded(itemState),
				InstalledLocalFolder = text2,
				InstalledSizeOnDisk = num3,
				InstalledTimestamp = dateTime,
				SanitizedName = TIUtilities.StripInvalidPathCharsFromString(p_itemDetails.m_rgchTitle)
			};
		}

		// Token: 0x06002181 RID: 8577 RVA: 0x000AEDD8 File Offset: 0x000ACFD8
		private bool CanAddRequestedItemToList(WorkshopItem item)
		{
			return this.m_reqItemList != null && this.m_reqItemList.Items != null && this.m_reqItemList.Items.Where<WorkshopItem>((WorkshopItem o) => o.Name == item.Name).Count<WorkshopItem>() == 0;
		}

		// Token: 0x06002182 RID: 8578 RVA: 0x000AEE2D File Offset: 0x000AD02D
		private bool IsInstalled(EItemState p_itemState)
		{
			return (p_itemState & EItemState.k_EItemStateInstalled) == EItemState.k_EItemStateInstalled && (p_itemState & EItemState.k_EItemStateDownloading) != EItemState.k_EItemStateDownloading && (p_itemState & EItemState.k_EItemStateDownloadPending) != EItemState.k_EItemStateDownloadPending;
		}

		// Token: 0x06002183 RID: 8579 RVA: 0x000AEE4A File Offset: 0x000AD04A
		private bool IsDownloading(EItemState p_itemState)
		{
			return (p_itemState & EItemState.k_EItemStateDownloading) == EItemState.k_EItemStateDownloading || (p_itemState & EItemState.k_EItemStateDownloadPending) == EItemState.k_EItemStateDownloadPending;
		}

		// Token: 0x06002184 RID: 8580 RVA: 0x000AEE5E File Offset: 0x000AD05E
		private bool IsUpdateNeeded(EItemState p_itemState)
		{
			return (p_itemState & EItemState.k_EItemStateNeedsUpdate) == EItemState.k_EItemStateNeedsUpdate;
		}

		// Token: 0x06002185 RID: 8581 RVA: 0x000AEE66 File Offset: 0x000AD066
		private uint GetPageCount(SteamUGCQueryCompleted_t p_callback)
		{
			if (p_callback.m_unTotalMatchingResults != 0U)
			{
				return (uint)Mathf.Ceil(p_callback.m_unTotalMatchingResults / 50f);
			}
			return 1U;
		}

		// Token: 0x06002186 RID: 8582 RVA: 0x000AEE88 File Offset: 0x000AD088
		private void QueryPublishedItems(uint p_page)
		{
			if (base.IsDebugLogEnabled)
			{
				Debug.Log("QueryPublishedItems page " + p_page.ToString());
			}
			this.m_reqPage = p_page;
			UGCQueryHandle_t ugcqueryHandle_t = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, AppId_t.Invalid, SteamUtils.GetAppID(), this.m_reqPage);
			if (!this.m_isSteamCacheEnabled)
			{
				SteamUGC.SetAllowCachedResponse(ugcqueryHandle_t, 0U);
			}
			SteamUGC.SetReturnLongDescription(ugcqueryHandle_t, true);
			base.Execute<SteamUGCQueryCompleted_t>(SteamUGC.SendQueryUGCRequest(ugcqueryHandle_t), new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(this.OnPublishedItemsCallCompleted));
		}

		// Token: 0x06002187 RID: 8583 RVA: 0x000AEF10 File Offset: 0x000AD110
		private void QueryFavoritedItems(uint p_page)
		{
			if (base.IsDebugLogEnabled)
			{
				Debug.Log("QueryFavoritedItems page " + p_page.ToString());
			}
			this.m_reqPage = p_page;
			UGCQueryHandle_t ugcqueryHandle_t = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Favorited, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, AppId_t.Invalid, SteamUtils.GetAppID(), this.m_reqPage);
			if (!this.m_isSteamCacheEnabled)
			{
				SteamUGC.SetAllowCachedResponse(ugcqueryHandle_t, 0U);
			}
			SteamUGC.SetReturnLongDescription(ugcqueryHandle_t, true);
			base.Execute<SteamUGCQueryCompleted_t>(SteamUGC.SendQueryUGCRequest(ugcqueryHandle_t), new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(this.OnFavoriteItemsCallCompleted));
		}

		// Token: 0x06002188 RID: 8584 RVA: 0x000AEF98 File Offset: 0x000AD198
		private void QueryVotedItems(uint p_page)
		{
			if (base.IsDebugLogEnabled)
			{
				Debug.Log("QueryVotedItems page " + p_page.ToString());
			}
			this.m_reqPage = p_page;
			UGCQueryHandle_t ugcqueryHandle_t = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_VotedOn, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, AppId_t.Invalid, SteamUtils.GetAppID(), this.m_reqPage);
			if (!this.m_isSteamCacheEnabled)
			{
				SteamUGC.SetAllowCachedResponse(ugcqueryHandle_t, 0U);
			}
			SteamUGC.SetReturnLongDescription(ugcqueryHandle_t, true);
			base.Execute<SteamUGCQueryCompleted_t>(SteamUGC.SendQueryUGCRequest(ugcqueryHandle_t), new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(this.OnVotedItemsCallCompleted));
		}

		// Token: 0x06002189 RID: 8585 RVA: 0x000AF020 File Offset: 0x000AD220
		private void QuerySubscribedItems(uint p_page)
		{
			if (base.IsDebugLogEnabled)
			{
				Debug.Log("QuerySubscribedItems page " + p_page.ToString());
			}
			this.m_reqPage = p_page;
			UGCQueryHandle_t ugcqueryHandle_t = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Subscribed, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, EUserUGCListSortOrder.k_EUserUGCListSortOrder_VoteScoreDesc, AppId_t.Invalid, SteamUtils.GetAppID(), this.m_reqPage);
			if (!this.m_isSteamCacheEnabled)
			{
				SteamUGC.SetAllowCachedResponse(ugcqueryHandle_t, 0U);
			}
			SteamUGC.SetReturnLongDescription(ugcqueryHandle_t, true);
			base.Execute<SteamUGCQueryCompleted_t>(SteamUGC.SendQueryUGCRequest(ugcqueryHandle_t), new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(this.OnSubscribedItemsCallCompleted));
		}

		// Token: 0x0600218A RID: 8586 RVA: 0x000AF0A8 File Offset: 0x000AD2A8
		private void QueryAllItems()
		{
			object obj = this.m_lock;
			lock (obj)
			{
				this.m_reqPage = this.m_reqItemList.Page;
			}
			if (base.IsDebugLogEnabled)
			{
				Debug.Log("QueryAllItems from " + this.m_sorting.SOURCE.ToString() + " page " + this.m_reqPage.ToString());
			}
			UGCQueryHandle_t ugcqueryHandle_t;
			if (this.m_sorting.SOURCE != EWorkshopSource.SUBSCRIBED)
			{
				ugcqueryHandle_t = SteamUGC.CreateQueryAllUGCRequest(this.m_sorting.MODE, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, AppId_t.Invalid, SteamUtils.GetAppID(), this.m_reqPage);
				if (this.m_searchText != null && !string.IsNullOrEmpty(this.m_searchText.Trim()))
				{
					SteamUGC.SetSearchText(ugcqueryHandle_t, this.m_searchText);
				}
				if (this.m_searchTags != null && this.m_searchTags.Count > 0)
				{
					SteamUGC.SetMatchAnyTag(ugcqueryHandle_t, this.m_searchMatchAnyTag);
					for (int i = 0; i < this.m_searchTags.Count; i++)
					{
						SteamUGC.AddRequiredTag(ugcqueryHandle_t, this.m_searchTags[i]);
					}
				}
			}
			else
			{
				uint num = SteamUGC.GetNumSubscribedItems(false);
				if (num <= 0U)
				{
					obj = this.m_lock;
					lock (obj)
					{
						this.m_reqItemList.PagesItems = 0U;
						if (this.OnItemListLoaded != null)
						{
							this.InvokeEventHandlerSafely<WorkshopItemListEventArgs>(this.OnItemListLoaded, new WorkshopItemListEventArgs
							{
								ItemList = this.m_reqItemList
							});
							this.ClearSingleShotEventHandlers<WorkshopItemListEventArgs>("OnItemListLoaded", ref this.OnItemListLoaded);
							if (base.IsDebugLogEnabled)
							{
								Debug.Log("QueryAllItems: no subscribed items");
							}
						}
						this.m_reqItemList = null;
						this.m_pendingRequests.Clear<GetUserItemVoteResult_t>();
					}
					return;
				}
				PublishedFileId_t[] array = new PublishedFileId_t[num];
				num = Math.Min(num, SteamUGC.GetSubscribedItems(array, num, false));
				ugcqueryHandle_t = SteamUGC.CreateQueryUGCDetailsRequest(array, num);
			}
			if (!this.m_isSteamCacheEnabled)
			{
				SteamUGC.SetAllowCachedResponse(ugcqueryHandle_t, 0U);
			}
			SteamUGC.SetReturnLongDescription(ugcqueryHandle_t, true);
			base.Execute<SteamUGCQueryCompleted_t>(SteamUGC.SendQueryUGCRequest(ugcqueryHandle_t), new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(this.OnAvailableItemsCallCompleted));
		}

		// Token: 0x0600218B RID: 8587 RVA: 0x000AF2DC File Offset: 0x000AD4DC
		private IEnumerator RenderIconRoutine(Camera p_camera, int p_width, int p_height, string p_saveToFilePath, bool p_keepTextureReference, Action<Texture2D> p_onRenderIconCompleted)
		{
			yield return new WaitForEndOfFrame();
			Rect pixelRect = p_camera.pixelRect;
			if ((float)p_width > pixelRect.width || (float)p_height > pixelRect.height)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"SteamWorkshopUIUpload: RenderIconRoutine: cannot render icon in given resolution (",
					p_width.ToString(),
					",",
					p_height.ToString(),
					"), because it exceeds the current camera's resolution (",
					pixelRect.width.ToString(),
					",",
					pixelRect.height.ToString(),
					")!"
				}));
				p_width = (int)Mathf.Min((float)p_width, pixelRect.width);
				p_height = (int)Mathf.Min((float)p_height, pixelRect.height);
			}
			Rect rect = new Rect(0f, 0f, (float)p_width, (float)p_height);
			p_camera.pixelRect = rect;
			p_camera.Render();
			p_camera.pixelRect = pixelRect;
			if (this.m_renderedTexture != null)
			{
				global::UnityEngine.Object.Destroy(this.m_renderedTexture);
			}
			this.m_renderedTexture = new Texture2D(p_width, p_height, TextureFormat.RGB24, false, true);
			this.m_renderedTexture.ReadPixels(rect, 0, 0, false);
			this.m_renderedTexture.Apply(false);
			if (!string.IsNullOrEmpty(p_saveToFilePath))
			{
				string directoryName = Path.GetDirectoryName(p_saveToFilePath);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				File.WriteAllBytes(p_saveToFilePath, this.m_renderedTexture.EncodeToPNG());
				if (base.IsDebugLogEnabled)
				{
					Debug.Log("RenderIconRoutine saved icon to '" + p_saveToFilePath + "'");
				}
			}
			if (p_onRenderIconCompleted != null)
			{
				p_onRenderIconCompleted(this.m_renderedTexture);
			}
			if (!p_keepTextureReference)
			{
				this.m_renderedTexture = null;
			}
			yield break;
		}

		// Token: 0x040019A0 RID: 6560
		private uint m_reqPage;

		// Token: 0x040019A1 RID: 6561
		private WorkshopItemList m_reqItemList;

		// Token: 0x040019A2 RID: 6562
		private Dictionary<PublishedFileId_t, WorkshopItem> m_items = new Dictionary<PublishedFileId_t, WorkshopItem>();

		// Token: 0x040019A3 RID: 6563
		private List<PublishedFileId_t> m_downloadingItems = new List<PublishedFileId_t>();

		// Token: 0x040019A4 RID: 6564
		private WorkshopItemUpdate m_uploadItemData;

		// Token: 0x040019A5 RID: 6565
		private Texture2D m_renderedTexture;

		// Token: 0x040019AE RID: 6574
		[SerializeField]
		[Tooltip("Controls the item list sorting. See also OnItemListLoaded and GetItemList.")]
		private WorkshopSortMode m_sorting = new WorkshopSortMode();

		// Token: 0x040019AF RID: 6575
		[SerializeField]
		[Tooltip("This search filter is applied to the item list. See also OnItemListLoaded and GetItemList.")]
		private string m_searchText = "";

		// Token: 0x040019B0 RID: 6576
		[SerializeField]
		[Tooltip("This tag filter is applied to the item list. See also SearchMatchAnyTag, OnItemListLoaded and GetItemList.")]
		private List<string> m_searchTags = new List<string>();

		// Token: 0x040019B1 RID: 6577
		[SerializeField]
		[Tooltip("Should the items filtered by SearchTags just need to have one required tag (true), or all of them (false). See also OnItemListLoaded and GetItemList.")]
		private bool m_searchMatchAnyTag = true;

		// Token: 0x040019B2 RID: 6578
		[SerializeField]
		[Tooltip("Set this property to true if you want your UI to respond faster, but sacrifice up-to-dateness. Disabled by default.")]
		private bool m_isSteamCacheEnabled;
	}
}
