using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LapinerTools.Steam.Data;
using ModelShark;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Modding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LapinerTools.Steam.UI
{
	// Token: 0x02000533 RID: 1331
	public class SteamWorkshopItemNode : MonoBehaviour, IScrollHandler, IEventSystemHandler
	{
		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x0600218D RID: 8589 RVA: 0x000AF369 File Offset: 0x000AD569
		public RawImage Image
		{
			get
			{
				return this.m_image;
			}
		}

		// Token: 0x0600218E RID: 8590 RVA: 0x000AF374 File Offset: 0x000AD574
		public virtual void uMyGUI_TreeBrowser_InitNode(object p_data)
		{
			if (p_data is SteamWorkshopItemNode.SendMessageInitData)
			{
				SteamMainBase<SteamWorkshopMain>.Instance.OnInstalled -= this.OnItemInstalled;
				SteamMainBase<SteamWorkshopMain>.Instance.OnInstalled += this.OnItemInstalled;
				this.m_data = (SteamWorkshopItemNode.SendMessageInitData)p_data;
				if (this.m_image != null && this.m_image.texture == null && this.m_pendingImageDownload == null)
				{
					base.StartCoroutine(this.DownloadPreview(this.m_data.Item.PreviewImageURL));
				}
				if (this.nameText != null)
				{
					this.nameText.text = this.m_data.Item.Name;
				}
				if (this.descriptionText != null)
				{
					this.descriptionText.text = TIUtilities.RemoveWorkshopTags(this.m_data.Item.Description);
				}
				base.StartCoroutine(this.Fixtext());
				StringBuilder stringBuilder = new StringBuilder(this.m_data.Item.Name).AppendLine().Append(this.m_data.Item.Description);
				if (stringBuilder.Length > 2000)
				{
					stringBuilder.Remove(2000, stringBuilder.Length - 2000);
				}
				if (this.modDescriptionTooltipTrigger != null)
				{
					this.modDescriptionTooltipTrigger.SetText("BodyText", TIUtilities.RemoveWorkshopTags(stringBuilder.ToString()));
				}
				if (this.voteCountText != null)
				{
					this.voteCountText.text = this.m_data.Item.VotesUp.ToString() + " / " + this.m_data.Item.VotesDown.ToString();
				}
				if (this.favoritesCountText != null)
				{
					this.favoritesCountText.text = this.m_data.Item.Favorites.ToString();
				}
				if (this.subscriptionCountText != null)
				{
					this.subscriptionCountText.text = this.m_data.Item.Subscriptions.ToString();
				}
				if (this.m_btnFavorites != null && this.m_btnFavoritesActive != null)
				{
					this.m_btnFavorites.gameObject.SetActive(!this.m_data.Item.IsFavorited);
					this.m_btnFavoritesActive.gameObject.SetActive(this.m_data.Item.IsFavorited);
				}
				if (this.m_btnSubscriptions != null && this.m_btnSubscriptionsActive != null)
				{
					this.m_btnSubscriptions.gameObject.SetActive(!this.m_data.Item.IsSubscribed);
					this.m_btnSubscriptionsActive.gameObject.SetActive(this.m_data.Item.IsSubscribed);
				}
				if (this.m_btnVotesUp != null && this.m_btnVotesUpActive != null)
				{
					this.m_btnVotesUp.gameObject.SetActive(!this.m_data.Item.IsVotedUp);
					this.m_btnVotesUpActive.gameObject.SetActive(this.m_data.Item.IsVotedUp);
				}
				if (this.m_btnVotesDown != null && this.m_btnVotesDownActive != null)
				{
					this.m_btnVotesDown.gameObject.SetActive(!this.m_data.Item.IsVotedDown);
					this.m_btnVotesDownActive.gameObject.SetActive(this.m_data.Item.IsVotedDown);
				}
				if (this.m_btnDownload != null)
				{
					this.m_btnDownload.gameObject.SetActive(!this.m_data.Item.IsInstalled && !this.m_data.Item.IsDownloading);
					if (!ModManager.checkedForModUpdates)
					{
						using (List<string>.Enumerator enumerator = ModManager.ModNames.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								string mod = enumerator.Current;
								if (this.m_data.Item.SanitizedName == mod)
								{
									Debug.Log(mod + ": need to check for update");
									Debug.Log(this.m_data.Item.InstalledLocalFolder);
									Debug.Log(ModManager.ModDirectories.Where<string>((string x) => x.Contains(mod)).First<string>());
									string text = TIUtilities.CombineStrings(new string[] { "Mods/Enabled/", mod });
									string installedLocalFolder = this.m_data.Item.InstalledLocalFolder;
									List<string> list = new List<string>();
									if (Directory.Exists(text))
									{
										foreach (string text2 in Directory.GetFiles(text, "*.*", SearchOption.AllDirectories))
										{
											Path.GetFileName(text2);
											string text3 = TIUtilities.CombineStrings(new string[] { text2.Replace(text, installedLocalFolder) });
											if (File.Exists(text3))
											{
												FileSystemInfo fileSystemInfo = new FileInfo(text2);
												FileInfo fileInfo = new FileInfo(text3);
												if (DateTime.Compare(fileSystemInfo.LastWriteTime, fileInfo.LastWriteTime) < 0)
												{
													Debug.Log("Updating file: " + text2);
													File.Copy(text3, text2, true);
												}
											}
											else
											{
												Debug.Log("deleting outdated file: " + text2);
												list.Add(text2);
											}
										}
									}
									foreach (string text4 in Directory.GetFiles(installedLocalFolder, "*.*", SearchOption.AllDirectories))
									{
										string fileName = Path.GetFileName(text4);
										if (!File.Exists(text4.Replace(installedLocalFolder, text)))
										{
											Debug.Log("adding new file: " + text4);
											string text5 = text4.Replace(installedLocalFolder, text);
											Directory.CreateDirectory(text5.Replace(fileName, ""));
											File.Copy(text4, text5);
										}
									}
									foreach (string text6 in list)
									{
										try
										{
											File.Delete(text6);
										}
										catch (Exception ex)
										{
											Debug.LogWarning(ex.Message + ", Could not delete" + text6);
										}
									}
								}
							}
						}
						if (!ModManager.ModNames.Contains(this.m_data.Item.SanitizedName) && !ModManager.DisabledModNames.Contains(this.m_data.Item.SanitizedName))
						{
							string text7 = TIUtilities.CombineStrings(new string[]
							{
								"Mods/Enabled/",
								this.m_data.Item.SanitizedName
							});
							try
							{
								if (!Directory.Exists(text7))
								{
									Directory.CreateDirectory(text7);
								}
								Utilities.CopyDirectory(this.m_data.Item.InstalledLocalFolder, text7, true);
							}
							catch (Exception ex2)
							{
								Debug.LogWarning(string.Concat(new string[]
								{
									ex2.Message,
									",",
									text7,
									",",
									this.m_data.Item.InstalledLocalFolder
								}));
							}
						}
					}
				}
				if (this.m_btnPlay != null)
				{
					this.m_btnPlay.gameObject.SetActive(this.m_data.Item.IsInstalled && !this.m_data.Item.IsDownloading && !ModManager.ModNames.Contains(this.m_data.Item.Name));
				}
				if (this.m_btnDelete != null)
				{
					this.m_btnDelete.gameObject.SetActive(this.m_data.Item.IsSubscribed);
				}
				if (this.m_useExplicitNavigation)
				{
					this.SetNavigationTargetsHorizontal(new Selectable[]
					{
						this.m_btnDelete, this.m_btnVotesUp, this.m_btnVotesUpActive, this.m_btnVotesDown, this.m_btnVotesDownActive, this.m_btnFavorites, this.m_btnFavoritesActive, this.m_btnSubscriptions, this.m_btnSubscriptionsActive, this.m_btnPlay,
						this.m_btnDownload
					});
					base.StartCoroutine(this.SetNavigationTargetsVertical());
				}
				if (this.downloadProgressText != null)
				{
					this.downloadProgressText.gameObject.SetActive(this.m_data.Item.IsDownloading);
				}
				if (this.m_data.Item.IsDownloading)
				{
					base.StartCoroutine(this.ShowDownloadProgress());
				}
				SteamWorkshopUIBrowse.Instance.InvokeOnItemDataSet(this.m_data.Item, this);
				return;
			}
			Debug.LogError("SteamWorkshopItemNode: uMyGUI_TreeBrowser_InitNode: expected p_data to be a SteamWorkshopItemNode.SendMessageInitData! p_data: " + ((p_data != null) ? p_data.ToString() : null));
		}

		// Token: 0x0600218F RID: 8591 RVA: 0x000AFCB8 File Offset: 0x000ADEB8
		private IEnumerator Fixtext()
		{
			yield return null;
			yield return null;
			if (this.descriptionText != null && this.descriptionText.preferredHeight > 100f && this.descriptionText.isTextOverflowing && this.descriptionText.firstOverflowCharacterIndex > 4)
			{
				this.descriptionText.text = TIUtilities.CombineStrings(new string[]
				{
					this.descriptionText.text.Remove(this.descriptionText.firstOverflowCharacterIndex - 4),
					"..."
				});
			}
			yield break;
		}

		// Token: 0x06002190 RID: 8592 RVA: 0x000AFCC7 File Offset: 0x000ADEC7
		public virtual void OnScroll(PointerEventData data)
		{
			if (this.m_parentScroller == null)
			{
				this.m_parentScroller = base.GetComponentInParent<ScrollRect>();
			}
			if (this.m_parentScroller == null)
			{
				return;
			}
			this.m_parentScroller.OnScroll(data);
		}

		// Token: 0x06002191 RID: 8593 RVA: 0x000AFD00 File Offset: 0x000ADF00
		public virtual void Select()
		{
			if (this.m_btnDownload != null && this.m_btnDownload.gameObject.activeSelf)
			{
				this.m_btnDownload.Select();
				return;
			}
			if (this.m_btnPlay != null && this.m_btnPlay.gameObject.activeSelf)
			{
				this.m_btnPlay.Select();
			}
		}

		// Token: 0x06002192 RID: 8594 RVA: 0x000AFD64 File Offset: 0x000ADF64
		protected virtual void Start()
		{
			if (this.m_btnFavorites != null && this.m_btnFavoritesActive != null)
			{
				this.m_btnFavorites.onClick.AddListener(new UnityAction(this.AddFavorite));
				this.m_btnFavoritesActive.onClick.AddListener(new UnityAction(this.RemovedFavorite));
			}
			if (this.m_btnSubscriptions != null && this.m_btnSubscriptionsActive != null)
			{
				this.m_btnSubscriptions.onClick.AddListener(new UnityAction(this.Subscribe));
				this.m_btnSubscriptionsActive.onClick.AddListener(new UnityAction(this.Unsubscribe));
			}
			if (this.m_btnVotesUp != null && this.m_btnVotesUpActive != null)
			{
				this.m_btnVotesUp.onClick.AddListener(new UnityAction(this.VoteUp));
			}
			if (this.m_btnVotesDown != null && this.m_btnVotesDownActive != null)
			{
				this.m_btnVotesDown.onClick.AddListener(new UnityAction(this.VoteDown));
			}
			if (this.m_btnDownload != null)
			{
				this.m_btnDownload.onClick.AddListener(new UnityAction(this.Subscribe));
			}
			if (this.m_btnPlay != null)
			{
				this.m_btnPlay.onClick.AddListener(new UnityAction(this.OnPlayBtn));
			}
			if (this.m_btnDelete != null)
			{
				this.m_btnDelete.onClick.AddListener(new UnityAction(this.Unsubscribe));
			}
		}

		// Token: 0x06002193 RID: 8595 RVA: 0x000AFF10 File Offset: 0x000AE110
		protected virtual void OnDestroy()
		{
			this.isDestroyed = true;
			if (this.m_image != null)
			{
				global::UnityEngine.Object.Destroy(this.m_image.texture);
			}
			if (this.m_pendingImageDownload != null)
			{
				this.m_pendingImageDownload.Dispose();
				this.m_pendingImageDownload = null;
			}
			if (SteamMainBase<SteamWorkshopMain>.IsInstanceSet)
			{
				SteamMainBase<SteamWorkshopMain>.Instance.OnInstalled -= this.OnItemInstalled;
			}
		}

		// Token: 0x06002194 RID: 8596 RVA: 0x000AFF7A File Offset: 0x000AE17A
		protected virtual void OnPlayBtn()
		{
			if (this.m_data != null)
			{
				SteamWorkshopUIBrowse.Instance.InvokeOnPlayButtonClick(this.m_data.Item);
			}
		}

		// Token: 0x06002195 RID: 8597 RVA: 0x000AFF9C File Offset: 0x000AE19C
		protected virtual void Subscribe()
		{
			if (this.m_data != null)
			{
				SteamMainBase<SteamWorkshopMain>.Instance.Subscribe(this.m_data.Item, this.OnItemUpdated(this.m_btnSubscriptionsActive));
				SteamWorkshopUIBrowse.Instance.InvokeOnSubscribeButtonClick(this.m_data.Item);
			}
		}

		// Token: 0x06002196 RID: 8598 RVA: 0x000AFFE8 File Offset: 0x000AE1E8
		protected virtual void Unsubscribe()
		{
			if (this.m_data != null)
			{
				SteamMainBase<SteamWorkshopMain>.Instance.Unsubscribe(this.m_data.Item, this.OnItemUpdated(this.m_btnSubscriptions));
				SteamWorkshopUIBrowse.Instance.InvokeOnUnsubscribeButtonClick(this.m_data.Item);
			}
		}

		// Token: 0x06002197 RID: 8599 RVA: 0x000B0034 File Offset: 0x000AE234
		protected virtual void AddFavorite()
		{
			if (this.m_data != null)
			{
				SteamMainBase<SteamWorkshopMain>.Instance.AddFavorite(this.m_data.Item, this.OnItemUpdated(this.m_btnFavoritesActive));
				SteamWorkshopUIBrowse.Instance.InvokeOnAddFavoriteButtonClick(this.m_data.Item);
			}
		}

		// Token: 0x06002198 RID: 8600 RVA: 0x000B0080 File Offset: 0x000AE280
		protected virtual void RemovedFavorite()
		{
			if (this.m_data != null)
			{
				SteamMainBase<SteamWorkshopMain>.Instance.RemoveFavorite(this.m_data.Item, this.OnItemUpdated(this.m_btnFavorites));
				SteamWorkshopUIBrowse.Instance.InvokeOnRemoveFavoriteButtonClick(this.m_data.Item);
			}
		}

		// Token: 0x06002199 RID: 8601 RVA: 0x000B00CC File Offset: 0x000AE2CC
		protected virtual void VoteUp()
		{
			if (this.m_data != null)
			{
				SteamMainBase<SteamWorkshopMain>.Instance.Vote(this.m_data.Item, true, this.OnItemUpdated(this.m_btnVotesUpActive));
				SteamWorkshopUIBrowse.Instance.InvokeOnVoteUpButtonClick(this.m_data.Item);
			}
		}

		// Token: 0x0600219A RID: 8602 RVA: 0x000B011C File Offset: 0x000AE31C
		protected virtual void VoteDown()
		{
			if (this.m_data != null)
			{
				SteamMainBase<SteamWorkshopMain>.Instance.Vote(this.m_data.Item, false, this.OnItemUpdated(this.m_btnVotesDownActive));
				SteamWorkshopUIBrowse.Instance.InvokeOnVoteDownButtonClick(this.m_data.Item);
			}
		}

		// Token: 0x0600219B RID: 8603 RVA: 0x000B0169 File Offset: 0x000AE369
		protected virtual void OnItemInstalled(WorkshopItemEventArgs p_itemArgs)
		{
			this.OnItemUpdated(this.m_btnPlay)(p_itemArgs);
		}

		// Token: 0x0600219C RID: 8604 RVA: 0x000B017D File Offset: 0x000AE37D
		protected virtual Action<WorkshopItemEventArgs> OnItemUpdated(Selectable p_focusWhenDone)
		{
			return delegate(WorkshopItemEventArgs p_itemArgs)
			{
				if (!this.isDestroyed && this.m_data != null && !p_itemArgs.IsError && this.m_data.Item.SteamNative.m_nPublishedFileId == p_itemArgs.Item.SteamNative.m_nPublishedFileId)
				{
					this.uMyGUI_TreeBrowser_InitNode(new SteamWorkshopItemNode.SendMessageInitData
					{
						Item = p_itemArgs.Item
					});
					if (this.m_improveNavigationFocus && p_focusWhenDone != null)
					{
						p_focusWhenDone.Select();
					}
				}
			};
		}

		// Token: 0x0600219D RID: 8605 RVA: 0x000B01A0 File Offset: 0x000AE3A0
		protected virtual void SetNavigationTargetsHorizontal(Selectable[] p_horizontalNavOrder)
		{
			for (int i = 0; i < p_horizontalNavOrder.Length; i++)
			{
				Selectable selectable = p_horizontalNavOrder[i];
				if (selectable != null)
				{
					Navigation navigation = selectable.navigation;
					navigation.mode = Navigation.Mode.Explicit;
					for (int j = i - 1; j >= 0; j--)
					{
						Selectable selectable2 = p_horizontalNavOrder[j];
						if (selectable2 != null && selectable2.gameObject.activeSelf)
						{
							navigation.selectOnLeft = selectable2;
							break;
						}
					}
					for (int k = i + 1; k < p_horizontalNavOrder.Length; k++)
					{
						Selectable selectable3 = p_horizontalNavOrder[k];
						if (selectable3 != null && selectable3.gameObject.activeSelf)
						{
							navigation.selectOnRight = selectable3;
							break;
						}
					}
					selectable.navigation = navigation;
				}
			}
		}

		// Token: 0x0600219E RID: 8606 RVA: 0x000B025C File Offset: 0x000AE45C
		protected virtual void SetNavigationTargetsVertical(Selectable p_current, Selectable[] p_verticalNavOrder)
		{
			if (p_current == null || !p_current.gameObject.activeSelf)
			{
				return;
			}
			for (int i = 0; i < p_verticalNavOrder.Length; i++)
			{
				Selectable selectable = p_verticalNavOrder[i];
				if (selectable != null && i >= 0)
				{
					Navigation navigation = selectable.navigation;
					navigation.mode = Navigation.Mode.Explicit;
					for (int j = i - 1; j >= 0; j--)
					{
						Selectable selectable2 = p_verticalNavOrder[j];
						if (selectable2 != null && selectable2.gameObject.activeSelf)
						{
							navigation.selectOnUp = selectable2;
							break;
						}
					}
					for (int k = i + 1; k < p_verticalNavOrder.Length; k++)
					{
						Selectable selectable3 = p_verticalNavOrder[k];
						if (selectable3 != null && selectable3.gameObject.activeSelf)
						{
							navigation.selectOnDown = selectable3;
							break;
						}
					}
					selectable.navigation = navigation;
				}
			}
		}

		// Token: 0x0600219F RID: 8607 RVA: 0x000B0334 File Offset: 0x000AE534
		protected virtual IEnumerator SetNavigationTargetsVertical()
		{
			yield return new WaitForEndOfFrame();
			if (base.transform.parent != null)
			{
				SteamWorkshopItemNode[] componentsInChildren = base.transform.parent.GetComponentsInChildren<SteamWorkshopItemNode>();
				int num = Array.IndexOf<SteamWorkshopItemNode>(componentsInChildren, this);
				if (num >= 0)
				{
					SteamWorkshopItemNode steamWorkshopItemNode = componentsInChildren[num];
					SteamWorkshopItemNode steamWorkshopItemNode2 = ((num > 0) ? componentsInChildren[num - 1] : null);
					SteamWorkshopItemNode steamWorkshopItemNode3 = ((num < componentsInChildren.Length - 1) ? componentsInChildren[num + 1] : null);
					this.SetNavigationTargetsVertical(steamWorkshopItemNode.m_btnDelete, new Selectable[]
					{
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnDelete : null,
						steamWorkshopItemNode.m_btnDelete,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnDelete : null
					});
					this.SetNavigationTargetsVertical(steamWorkshopItemNode.m_btnVotesUp, new Selectable[]
					{
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnVotesUp : null,
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnVotesUpActive : null,
						steamWorkshopItemNode.m_btnVotesUp,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnVotesUp : null,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnVotesUpActive : null
					});
					this.SetNavigationTargetsVertical(steamWorkshopItemNode.m_btnVotesUpActive, new Selectable[]
					{
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnVotesUp : null,
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnVotesUpActive : null,
						steamWorkshopItemNode.m_btnVotesUpActive,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnVotesUp : null,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnVotesUpActive : null
					});
					this.SetNavigationTargetsVertical(steamWorkshopItemNode.m_btnVotesDown, new Selectable[]
					{
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnVotesDown : null,
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnVotesDownActive : null,
						steamWorkshopItemNode.m_btnVotesDown,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnVotesDown : null,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnVotesDownActive : null
					});
					this.SetNavigationTargetsVertical(steamWorkshopItemNode.m_btnVotesDownActive, new Selectable[]
					{
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnVotesDown : null,
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnVotesDownActive : null,
						steamWorkshopItemNode.m_btnVotesDownActive,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnVotesDown : null,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnVotesDownActive : null
					});
					this.SetNavigationTargetsVertical(steamWorkshopItemNode.m_btnFavorites, new Selectable[]
					{
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnFavorites : null,
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnFavoritesActive : null,
						steamWorkshopItemNode.m_btnFavorites,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnFavorites : null,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnFavoritesActive : null
					});
					this.SetNavigationTargetsVertical(steamWorkshopItemNode.m_btnFavoritesActive, new Selectable[]
					{
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnFavorites : null,
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnFavoritesActive : null,
						steamWorkshopItemNode.m_btnFavoritesActive,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnFavorites : null,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnFavoritesActive : null
					});
					this.SetNavigationTargetsVertical(steamWorkshopItemNode.m_btnSubscriptions, new Selectable[]
					{
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnSubscriptions : null,
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnSubscriptionsActive : null,
						steamWorkshopItemNode.m_btnSubscriptions,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnSubscriptions : null,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnSubscriptionsActive : null
					});
					this.SetNavigationTargetsVertical(steamWorkshopItemNode.m_btnSubscriptionsActive, new Selectable[]
					{
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnSubscriptions : null,
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnSubscriptionsActive : null,
						steamWorkshopItemNode.m_btnSubscriptionsActive,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnSubscriptions : null,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnSubscriptionsActive : null
					});
					this.SetNavigationTargetsVertical(steamWorkshopItemNode.m_btnPlay, new Selectable[]
					{
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnPlay : null,
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnDownload : null,
						steamWorkshopItemNode.m_btnPlay,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnPlay : null,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnDownload : null
					});
					this.SetNavigationTargetsVertical(steamWorkshopItemNode.m_btnDownload, new Selectable[]
					{
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnPlay : null,
						steamWorkshopItemNode2 ? steamWorkshopItemNode2.m_btnDownload : null,
						steamWorkshopItemNode.m_btnDownload,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnPlay : null,
						steamWorkshopItemNode3 ? steamWorkshopItemNode3.m_btnDownload : null
					});
					if (num == 0 || num == componentsInChildren.Length - 1)
					{
						yield return new WaitForEndOfFrame();
						this.SetAutomaticNavigation(this.m_btnDelete);
						this.SetAutomaticNavigation(this.m_btnVotesUp);
						this.SetAutomaticNavigation(this.m_btnVotesUpActive);
						this.SetAutomaticNavigation(this.m_btnVotesDown);
						this.SetAutomaticNavigation(this.m_btnVotesDownActive);
						this.SetAutomaticNavigation(this.m_btnFavorites);
						this.SetAutomaticNavigation(this.m_btnFavoritesActive);
						this.SetAutomaticNavigation(this.m_btnSubscriptions);
						this.SetAutomaticNavigation(this.m_btnSubscriptionsActive);
						this.SetAutomaticNavigation(this.m_btnPlay);
						this.SetAutomaticNavigation(this.m_btnDownload);
					}
				}
			}
			yield break;
		}

		// Token: 0x060021A0 RID: 8608 RVA: 0x000B0344 File Offset: 0x000AE544
		protected virtual void SetAutomaticNavigation(Selectable p_selectable)
		{
			if (p_selectable != null)
			{
				Navigation navigation = p_selectable.navigation;
				navigation.mode = Navigation.Mode.Automatic;
				p_selectable.navigation = navigation;
			}
		}

		// Token: 0x060021A1 RID: 8609 RVA: 0x000B0370 File Offset: 0x000AE570
		protected virtual IEnumerator ShowDownloadProgress()
		{
			while (this.m_data != null && this.m_data.Item.IsDownloading)
			{
				if (this.downloadProgressText != null)
				{
					this.downloadProgressText.gameObject.SetActive(true);
					this.downloadProgressText.text = ((int)(SteamMainBase<SteamWorkshopMain>.Instance.GetDownloadProgress(this.m_data.Item) * 100f)).ToString() + "%";
				}
				yield return new WaitForSeconds(0.4f);
			}
			yield break;
		}

		// Token: 0x060021A2 RID: 8610 RVA: 0x000B037F File Offset: 0x000AE57F
		protected virtual IEnumerator DownloadPreview(string p_URL)
		{
			if (!string.IsNullOrEmpty(p_URL))
			{
				this.m_pendingImageDownload = new WWW(p_URL);
				yield return this.m_pendingImageDownload;
				if (this.m_pendingImageDownload != null)
				{
					if (this.m_pendingImageDownload.isDone && string.IsNullOrEmpty(this.m_pendingImageDownload.error))
					{
						if (this.m_image != null)
						{
							this.m_image.texture = this.m_pendingImageDownload.texture;
						}
					}
					else
					{
						Debug.LogError("SteamWorkshopItemNode: DownloadPreview: could not load preview image at '" + p_URL + "'\n" + this.m_pendingImageDownload.error);
					}
					this.m_pendingImageDownload = null;
				}
			}
			yield break;
		}

		// Token: 0x040019B3 RID: 6579
		[SerializeField]
		public TMP_Text nameText;

		// Token: 0x040019B4 RID: 6580
		[SerializeField]
		public TMP_Text descriptionText;

		// Token: 0x040019B5 RID: 6581
		public TooltipTrigger modDescriptionTooltipTrigger;

		// Token: 0x040019B6 RID: 6582
		[SerializeField]
		public TMP_Text voteCountText;

		// Token: 0x040019B7 RID: 6583
		[SerializeField]
		protected Button m_btnVotesUp;

		// Token: 0x040019B8 RID: 6584
		[SerializeField]
		protected Button m_btnVotesUpActive;

		// Token: 0x040019B9 RID: 6585
		[SerializeField]
		protected Button m_btnVotesDown;

		// Token: 0x040019BA RID: 6586
		[SerializeField]
		protected Button m_btnVotesDownActive;

		// Token: 0x040019BB RID: 6587
		[SerializeField]
		public TMP_Text favoritesCountText;

		// Token: 0x040019BC RID: 6588
		[SerializeField]
		protected Button m_btnFavorites;

		// Token: 0x040019BD RID: 6589
		[SerializeField]
		protected Button m_btnFavoritesActive;

		// Token: 0x040019BE RID: 6590
		[SerializeField]
		public TMP_Text subscriptionCountText;

		// Token: 0x040019BF RID: 6591
		[SerializeField]
		public TMP_Text downloadProgressText;

		// Token: 0x040019C0 RID: 6592
		[SerializeField]
		protected Button m_btnSubscriptions;

		// Token: 0x040019C1 RID: 6593
		[SerializeField]
		protected Button m_btnSubscriptionsActive;

		// Token: 0x040019C2 RID: 6594
		[SerializeField]
		protected RawImage m_image;

		// Token: 0x040019C3 RID: 6595
		[SerializeField]
		protected Image m_selectionImage;

		// Token: 0x040019C4 RID: 6596
		[SerializeField]
		protected Button m_btnDownload;

		// Token: 0x040019C5 RID: 6597
		[SerializeField]
		protected Button m_btnPlay;

		// Token: 0x040019C6 RID: 6598
		[SerializeField]
		protected Button m_btnDelete;

		// Token: 0x040019C7 RID: 6599
		[SerializeField]
		protected bool m_useExplicitNavigation = true;

		// Token: 0x040019C8 RID: 6600
		[SerializeField]
		protected bool m_improveNavigationFocus = true;

		// Token: 0x040019C9 RID: 6601
		protected SteamWorkshopItemNode.SendMessageInitData m_data;

		// Token: 0x040019CA RID: 6602
		protected ScrollRect m_parentScroller;

		// Token: 0x040019CB RID: 6603
		protected WWW m_pendingImageDownload;

		// Token: 0x040019CC RID: 6604
		protected bool isDestroyed;

		// Token: 0x02000CA3 RID: 3235
		public class ItemDataSetEventArgs : EventArgsBase
		{
			// Token: 0x1700118B RID: 4491
			// (get) Token: 0x06006D5B RID: 27995 RVA: 0x0030ACFB File Offset: 0x00308EFB
			// (set) Token: 0x06006D5C RID: 27996 RVA: 0x0030AD03 File Offset: 0x00308F03
			public WorkshopItem ItemData { get; set; }

			// Token: 0x1700118C RID: 4492
			// (get) Token: 0x06006D5D RID: 27997 RVA: 0x0030AD0C File Offset: 0x00308F0C
			// (set) Token: 0x06006D5E RID: 27998 RVA: 0x0030AD14 File Offset: 0x00308F14
			public SteamWorkshopItemNode ItemUI { get; set; }
		}

		// Token: 0x02000CA4 RID: 3236
		public class SendMessageInitData
		{
			// Token: 0x1700118D RID: 4493
			// (get) Token: 0x06006D60 RID: 28000 RVA: 0x0030AD25 File Offset: 0x00308F25
			// (set) Token: 0x06006D61 RID: 28001 RVA: 0x0030AD2D File Offset: 0x00308F2D
			public WorkshopItem Item { get; set; }
		}
	}
}
