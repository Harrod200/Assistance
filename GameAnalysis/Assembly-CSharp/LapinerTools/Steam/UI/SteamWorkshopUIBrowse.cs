using System;
using System.Collections.Generic;
using LapinerTools.Steam.Data;
using LapinerTools.uMyGUI;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Modding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LapinerTools.Steam.UI
{
	// Token: 0x02000536 RID: 1334
	public class SteamWorkshopUIBrowse : MonoBehaviour
	{
		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x060021AA RID: 8618 RVA: 0x000B040E File Offset: 0x000AE60E
		public static SteamWorkshopUIBrowse Instance
		{
			get
			{
				if (SteamWorkshopUIBrowse.s_instance == null)
				{
					SteamWorkshopUIBrowse.s_instance = global::UnityEngine.Object.FindObjectOfType<SteamWorkshopUIBrowse>();
				}
				return SteamWorkshopUIBrowse.s_instance;
			}
		}

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x060021AB RID: 8619 RVA: 0x000B042C File Offset: 0x000AE62C
		// (remove) Token: 0x060021AC RID: 8620 RVA: 0x000B0464 File Offset: 0x000AE664
		public event Action<WorkshopSortModeEventArgs> OnSortModeChanged;

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x060021AD RID: 8621 RVA: 0x000B049C File Offset: 0x000AE69C
		// (remove) Token: 0x060021AE RID: 8622 RVA: 0x000B04D4 File Offset: 0x000AE6D4
		public event Action<string> OnSearchButtonClick;

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x060021AF RID: 8623 RVA: 0x000B050C File Offset: 0x000AE70C
		// (remove) Token: 0x060021B0 RID: 8624 RVA: 0x000B0544 File Offset: 0x000AE744
		public event Action<int> OnPageChanged;

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x060021B1 RID: 8625 RVA: 0x000B057C File Offset: 0x000AE77C
		// (remove) Token: 0x060021B2 RID: 8626 RVA: 0x000B05B4 File Offset: 0x000AE7B4
		public event Action<WorkshopItemEventArgs> OnPlayButtonClick;

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x060021B3 RID: 8627 RVA: 0x000B05EC File Offset: 0x000AE7EC
		// (remove) Token: 0x060021B4 RID: 8628 RVA: 0x000B0624 File Offset: 0x000AE824
		public event Action<WorkshopItemEventArgs> OnVoteUpButtonClick;

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x060021B5 RID: 8629 RVA: 0x000B065C File Offset: 0x000AE85C
		// (remove) Token: 0x060021B6 RID: 8630 RVA: 0x000B0694 File Offset: 0x000AE894
		public event Action<WorkshopItemEventArgs> OnVoteDownButtonClick;

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x060021B7 RID: 8631 RVA: 0x000B06CC File Offset: 0x000AE8CC
		// (remove) Token: 0x060021B8 RID: 8632 RVA: 0x000B0704 File Offset: 0x000AE904
		public event Action<WorkshopItemEventArgs> OnSubscribeButtonClick;

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x060021B9 RID: 8633 RVA: 0x000B073C File Offset: 0x000AE93C
		// (remove) Token: 0x060021BA RID: 8634 RVA: 0x000B0774 File Offset: 0x000AE974
		public event Action<WorkshopItemEventArgs> OnUnsubscribeButtonClick;

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x060021BB RID: 8635 RVA: 0x000B07AC File Offset: 0x000AE9AC
		// (remove) Token: 0x060021BC RID: 8636 RVA: 0x000B07E4 File Offset: 0x000AE9E4
		public event Action<WorkshopItemEventArgs> OnAddFavoriteButtonClick;

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x060021BD RID: 8637 RVA: 0x000B081C File Offset: 0x000AEA1C
		// (remove) Token: 0x060021BE RID: 8638 RVA: 0x000B0854 File Offset: 0x000AEA54
		public event Action<WorkshopItemEventArgs> OnRemoveFavoriteButtonClick;

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x060021BF RID: 8639 RVA: 0x000B088C File Offset: 0x000AEA8C
		// (remove) Token: 0x060021C0 RID: 8640 RVA: 0x000B08C4 File Offset: 0x000AEAC4
		public event Action<SteamWorkshopItemNode.ItemDataSetEventArgs> OnItemDataSet;

		// Token: 0x060021C1 RID: 8641 RVA: 0x000B08F9 File Offset: 0x000AEAF9
		public void InvokeOnPlayButtonClick(WorkshopItem p_clickedItem)
		{
			this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(this.OnPlayButtonClick, new WorkshopItemEventArgs(p_clickedItem));
		}

		// Token: 0x060021C2 RID: 8642 RVA: 0x000B090D File Offset: 0x000AEB0D
		public void InvokeOnVoteUpButtonClick(WorkshopItem p_clickedItem)
		{
			this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(this.OnVoteUpButtonClick, new WorkshopItemEventArgs(p_clickedItem));
		}

		// Token: 0x060021C3 RID: 8643 RVA: 0x000B0921 File Offset: 0x000AEB21
		public void InvokeOnVoteDownButtonClick(WorkshopItem p_clickedItem)
		{
			this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(this.OnVoteDownButtonClick, new WorkshopItemEventArgs(p_clickedItem));
		}

		// Token: 0x060021C4 RID: 8644 RVA: 0x000B0935 File Offset: 0x000AEB35
		public void InvokeOnSubscribeButtonClick(WorkshopItem p_clickedItem)
		{
			this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(this.OnSubscribeButtonClick, new WorkshopItemEventArgs(p_clickedItem));
		}

		// Token: 0x060021C5 RID: 8645 RVA: 0x000B0949 File Offset: 0x000AEB49
		public void InvokeOnUnsubscribeButtonClick(WorkshopItem p_clickedItem)
		{
			this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(this.OnUnsubscribeButtonClick, new WorkshopItemEventArgs(p_clickedItem));
		}

		// Token: 0x060021C6 RID: 8646 RVA: 0x000B095D File Offset: 0x000AEB5D
		public void InvokeOnAddFavoriteButtonClick(WorkshopItem p_clickedItem)
		{
			this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(this.OnAddFavoriteButtonClick, new WorkshopItemEventArgs(p_clickedItem));
		}

		// Token: 0x060021C7 RID: 8647 RVA: 0x000B0971 File Offset: 0x000AEB71
		public void InvokeOnRemoveFavoriteButtonClick(WorkshopItem p_clickedItem)
		{
			this.InvokeEventHandlerSafely<WorkshopItemEventArgs>(this.OnRemoveFavoriteButtonClick, new WorkshopItemEventArgs(p_clickedItem));
		}

		// Token: 0x060021C8 RID: 8648 RVA: 0x000B0985 File Offset: 0x000AEB85
		public void InvokeOnItemDataSet(WorkshopItem p_itemData, SteamWorkshopItemNode p_itemUI)
		{
			this.InvokeEventHandlerSafely<SteamWorkshopItemNode.ItemDataSetEventArgs>(this.OnItemDataSet, new SteamWorkshopItemNode.ItemDataSetEventArgs
			{
				ItemData = p_itemData,
				ItemUI = p_itemUI
			});
		}

		// Token: 0x060021C9 RID: 8649 RVA: 0x000B09A6 File Offset: 0x000AEBA6
		public void PulledPublishedItems()
		{
			this.SORTING.DROPDOWN.Select(5);
		}

		// Token: 0x060021CA RID: 8650 RVA: 0x000B09BC File Offset: 0x000AEBBC
		public void SetItems(WorkshopItemList p_itemList)
		{
			if (this.ITEM_BROWSER != null)
			{
				this.m_uiNodeToSteamItem.Clear();
				this.ITEM_BROWSER.Clear();
				this.ITEM_BROWSER.BuildTree(this.ConvertItemsToNodes(p_itemList.Items.ToArray()));
			}
			else
			{
				Debug.LogError("SteamWorkshopUIBrowse: SetItems: ITEM_BROWSER is not set in inspector!");
			}
			if (this.PAGE_SELCTOR != null)
			{
				this.PAGE_SELCTOR.OnPageSelected -= this.SetPage;
				this.PAGE_SELCTOR.SetPageCount((int)p_itemList.PagesItems);
				this.PAGE_SELCTOR.SelectPage((int)p_itemList.Page);
				this.PAGE_SELCTOR.OnPageSelected += this.SetPage;
			}
			else
			{
				Debug.LogError("SteamWorkshopUIBrowse: SetItems: PAGE_SELCTOR is not set in inspector!");
			}
			if (this.m_improveNavigationFocus && this.ITEM_BROWSER != null && this.ITEM_BROWSER.transform.childCount > 0 && this.ITEM_BROWSER.transform.GetChild(0).GetComponent<SteamWorkshopItemNode>() != null)
			{
				this.ITEM_BROWSER.transform.GetChild(0).GetComponent<SteamWorkshopItemNode>().Select();
			}
		}

		// Token: 0x060021CB RID: 8651 RVA: 0x000B0AE4 File Offset: 0x000AECE4
		public void LoadItems(int p_page)
		{
			if (!this.initialized)
			{
				return;
			}
			uMyGUI_PopupManager.Instance.ShowPopup("loading");
			SteamMainBase<SteamWorkshopMain>.Instance.GetItemList((uint)p_page, delegate(WorkshopItemListEventArgs p_itemListArgs)
			{
				uMyGUI_PopupManager.Instance.HidePopup("loading");
				ModManager.checkedForModUpdates = true;
			});
		}

		// Token: 0x060021CC RID: 8652 RVA: 0x000B0B38 File Offset: 0x000AED38
		public void Search(string p_searchText)
		{
			bool flag = p_searchText != SteamMainBase<SteamWorkshopMain>.Instance.SearchText;
			bool flag2 = SteamMainBase<SteamWorkshopMain>.Instance.SearchText != null && !string.IsNullOrEmpty(SteamMainBase<SteamWorkshopMain>.Instance.SearchText);
			bool flag3 = p_searchText != null && !string.IsNullOrEmpty(p_searchText.Trim());
			SteamMainBase<SteamWorkshopMain>.Instance.SearchText = p_searchText;
			if (flag && (flag3 || (flag2 && !flag3)))
			{
				this.InvokeEventHandlerSafely<string>(this.OnSearchButtonClick, p_searchText);
				this.LoadItems(1);
			}
		}

		// Token: 0x060021CD RID: 8653 RVA: 0x000B0BB6 File Offset: 0x000AEDB6
		protected void SetPage(int p_page)
		{
			this.InvokeEventHandlerSafely<int>(this.OnPageChanged, p_page);
			this.LoadItems(p_page);
		}

		// Token: 0x060021CE RID: 8654 RVA: 0x000B0BCC File Offset: 0x000AEDCC
		protected virtual void Start()
		{
			this.InitSorting();
			this.InitSearch();
			this.LoadLocalizedText();
			SteamMainBase<SteamWorkshopMain>.Instance.OnItemListLoaded += this.SetItems;
			SteamMainBase<SteamWorkshopMain>.Instance.OnError += this.ShowErrorMessage;
			Loc.OnLanguageChangedEvent += this.OnLanguageChangedEvent;
			this.initialized = true;
			if (this.m_loadOnStart)
			{
				this.LoadItems(1);
			}
		}

		// Token: 0x060021CF RID: 8655 RVA: 0x000B0C40 File Offset: 0x000AEE40
		protected virtual void LateUpdate()
		{
			if (this.m_improveNavigationFocus)
			{
				EventSystem current = EventSystem.current;
				if (current != null && (current.currentSelectedGameObject == null || !current.currentSelectedGameObject.activeInHierarchy))
				{
					if (current.lastSelectedGameObject != null && current.lastSelectedGameObject.activeInHierarchy)
					{
						current.SetSelectedGameObject(current.lastSelectedGameObject);
						return;
					}
					if ((!(this.ITEM_BROWSER != null) || this.ITEM_BROWSER.transform.childCount <= 0 || !(this.ITEM_BROWSER.transform.GetChild(0).GetComponent<SteamWorkshopItemNode>() != null)) && this.searchInputField != null)
					{
						this.searchInputField.Select();
					}
				}
			}
		}

		// Token: 0x060021D0 RID: 8656 RVA: 0x000B0D08 File Offset: 0x000AEF08
		private void OnLanguageChangedEvent()
		{
			this.InitSorting();
			this.LoadLocalizedText();
			Loc.SwapFonts(base.gameObject);
		}

		// Token: 0x060021D1 RID: 8657 RVA: 0x000B0D21 File Offset: 0x000AEF21
		private void LoadLocalizedText()
		{
			this.installColumnText.SetText(Loc.T("UI.StartScreen.Mods.Install"));
		}

		// Token: 0x060021D2 RID: 8658 RVA: 0x000B0D38 File Offset: 0x000AEF38
		protected virtual void OnDestroy()
		{
			if (SteamMainBase<SteamWorkshopMain>.IsInstanceSet)
			{
				SteamMainBase<SteamWorkshopMain>.Instance.OnItemListLoaded -= this.SetItems;
				SteamMainBase<SteamWorkshopMain>.Instance.OnError -= this.ShowErrorMessage;
			}
			Loc.OnLanguageChangedEvent -= this.OnLanguageChangedEvent;
		}

		// Token: 0x060021D3 RID: 8659 RVA: 0x000B0D8C File Offset: 0x000AEF8C
		protected virtual void ShowErrorMessage(ErrorEventArgs p_errorArgs)
		{
			uMyGUI_PopupManager.Instance.HidePopup("loading");
			((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText("Steam Error", p_errorArgs.ErrorMessage).ShowButton("ok");
		}

		// Token: 0x060021D4 RID: 8660 RVA: 0x000B0DD8 File Offset: 0x000AEFD8
		protected virtual void SetItems(WorkshopItemListEventArgs p_itemListArgs)
		{
			if (!p_itemListArgs.IsError)
			{
				this.SetItems(p_itemListArgs.ItemList);
				return;
			}
			Debug.LogError("SteamWorkshopUIBrowse: SetItems: Steam Error: " + p_itemListArgs.ErrorMessage);
		}

		// Token: 0x060021D5 RID: 8661 RVA: 0x000B0E04 File Offset: 0x000AF004
		protected virtual uMyGUI_TreeBrowser.Node[] ConvertItemsToNodes(WorkshopItem[] p_items)
		{
			uMyGUI_TreeBrowser.Node[] array = new uMyGUI_TreeBrowser.Node[p_items.Length];
			for (int i = 0; i < p_items.Length; i++)
			{
				if (p_items[i] != null)
				{
					uMyGUI_TreeBrowser.Node node = new uMyGUI_TreeBrowser.Node(new SteamWorkshopItemNode.SendMessageInitData
					{
						Item = p_items[i]
					}, null);
					array[i] = node;
					this.m_uiNodeToSteamItem.Add(node, p_items[i]);
				}
				else
				{
					Debug.LogError("SteamWorkshopUIBrowse: ConvertItemsToNodes: item at index '" + i.ToString() + "' is null!");
				}
			}
			return array;
		}

		// Token: 0x060021D6 RID: 8662 RVA: 0x000B0E74 File Offset: 0x000AF074
		protected virtual void InitSorting()
		{
			if (this.SORTING != null && this.SORTING.DROPDOWN != null)
			{
				string[] array = new string[this.SORTING.OPTIONS.Length];
				for (int i = 0; i < array.Length; i++)
				{
					if (this.SORTING.OPTIONS[i] != null)
					{
						array[i] = Loc.T(this.SORTING.OPTIONS[i].DISPLAY_TEXT);
					}
					else
					{
						array[i] = "NULL";
					}
				}
				this.SORTING.DROPDOWN.Entries = array;
				this.SORTING.DROPDOWN.Select(Mathf.Clamp(this.SORTING.DEFAULT_SORT_MODE, 0, array.Length - 1));
				this.SORTING.DROPDOWN.OnSelected += delegate(int p_selectedSortIndex)
				{
					if (p_selectedSortIndex >= 0 && p_selectedSortIndex < this.SORTING.OPTIONS.Length)
					{
						WorkshopSortMode mode = this.SORTING.OPTIONS[p_selectedSortIndex].MODE;
						bool flag = SteamMainBase<SteamWorkshopMain>.Instance.Sorting != mode;
						SteamMainBase<SteamWorkshopMain>.Instance.Sorting = mode;
						if (flag)
						{
							this.InvokeEventHandlerSafely<WorkshopSortModeEventArgs>(this.OnSortModeChanged, new WorkshopSortModeEventArgs(mode));
							this.LoadItems(1);
						}
					}
				};
				if (this.SORTING.DEFAULT_SORT_MODE >= 0 && this.SORTING.DEFAULT_SORT_MODE < this.SORTING.OPTIONS.Length)
				{
					SteamMainBase<SteamWorkshopMain>.Instance.Sorting = this.SORTING.OPTIONS[this.SORTING.DEFAULT_SORT_MODE].MODE;
					return;
				}
			}
			else
			{
				Debug.LogError("SteamWorkshopUIBrowse: SORTING.DROPDOWN is not set in inspector!");
			}
		}

		// Token: 0x060021D7 RID: 8663 RVA: 0x000B0FA4 File Offset: 0x000AF1A4
		protected virtual void InitSearch()
		{
			if (!(this.searchInputField != null))
			{
				Debug.LogError("SteamWorkshopUIBrowse: SEARCH_INPUT is not set in inspector!");
				return;
			}
			this.searchInputField.onEndEdit.AddListener(new UnityAction<string>(this.Search));
			if (this.SEARCH_BUTTON != null)
			{
				this.SEARCH_BUTTON.onClick.AddListener(delegate
				{
					if (this.searchInputField != null)
					{
						this.Search(this.searchInputField.text);
					}
				});
				return;
			}
			Debug.LogError("SteamWorkshopUIBrowse: SEARCH_BUTTON is not set in inspector!");
		}

		// Token: 0x060021D8 RID: 8664 RVA: 0x000B101C File Offset: 0x000AF21C
		protected virtual void InvokeEventHandlerSafely<T>(Action<T> p_handler, T p_data)
		{
			try
			{
				if (p_handler != null)
				{
					p_handler(p_data);
				}
			}
			catch (Exception ex)
			{
				string[] array = new string[6];
				array[0] = "SteamWorkshopUIBrowse: your event handler (";
				int num = 1;
				object target = p_handler.Target;
				array[num] = ((target != null) ? target.ToString() : null);
				array[2] = " - System.Action<";
				int num2 = 3;
				Type typeFromHandle = typeof(T);
				array[num2] = ((typeFromHandle != null) ? typeFromHandle.ToString() : null);
				array[4] = ">) has thrown an excepotion!\n";
				int num3 = 5;
				Exception ex2 = ex;
				array[num3] = ((ex2 != null) ? ex2.ToString() : null);
				Debug.LogError(string.Concat(array));
			}
		}

		// Token: 0x040019CF RID: 6607
		protected static SteamWorkshopUIBrowse s_instance;

		// Token: 0x040019DB RID: 6619
		[SerializeField]
		protected uMyGUI_TreeBrowser ITEM_BROWSER;

		// Token: 0x040019DC RID: 6620
		[SerializeField]
		protected uMyGUI_PageBox PAGE_SELCTOR;

		// Token: 0x040019DD RID: 6621
		[SerializeField]
		protected SteamWorkshopUIBrowse.SortingConfig SORTING;

		// Token: 0x040019DE RID: 6622
		[SerializeField]
		protected TMP_InputField searchInputField;

		// Token: 0x040019DF RID: 6623
		[SerializeField]
		protected Button SEARCH_BUTTON;

		// Token: 0x040019E0 RID: 6624
		[SerializeField]
		protected TMP_Text installColumnText;

		// Token: 0x040019E1 RID: 6625
		[SerializeField]
		[Tooltip("If true, then the first page will be loaded on MonoBehaviour.OnStart")]
		protected bool m_loadOnStart = true;

		// Token: 0x040019E2 RID: 6626
		[SerializeField]
		protected bool m_improveNavigationFocus = true;

		// Token: 0x040019E3 RID: 6627
		protected Dictionary<uMyGUI_TreeBrowser.Node, WorkshopItem> m_uiNodeToSteamItem = new Dictionary<uMyGUI_TreeBrowser.Node, WorkshopItem>();

		// Token: 0x040019E4 RID: 6628
		private bool initialized;

		// Token: 0x02000CAB RID: 3243
		[Serializable]
		public class SortingConfig
		{
			// Token: 0x04004F34 RID: 20276
			[SerializeField]
			public uMyGUI_Dropdown DROPDOWN;

			// Token: 0x04004F35 RID: 20277
			[SerializeField]
			public int DEFAULT_SORT_MODE;

			// Token: 0x04004F36 RID: 20278
			[SerializeField]
			public SteamWorkshopUIBrowse.SortingConfig.Option[] OPTIONS = new SteamWorkshopUIBrowse.SortingConfig.Option[0];

			// Token: 0x020013E7 RID: 5095
			[Serializable]
			public class Option
			{
				// Token: 0x04007320 RID: 29472
				[SerializeField]
				public WorkshopSortMode MODE = new WorkshopSortMode();

				// Token: 0x04007321 RID: 29473
				[SerializeField]
				public string DISPLAY_TEXT = "Votes";
			}
		}
	}
}
