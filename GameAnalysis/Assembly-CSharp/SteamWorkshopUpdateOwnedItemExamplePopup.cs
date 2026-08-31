using System;
using System.IO;
using LapinerTools.Steam;
using LapinerTools.Steam.Data;
using LapinerTools.Steam.UI;
using LapinerTools.uMyGUI;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000411 RID: 1041
public class SteamWorkshopUpdateOwnedItemExamplePopup : MonoBehaviour
{
	// Token: 0x0600154B RID: 5451 RVA: 0x00068990 File Offset: 0x00066B90
	private void Start()
	{
		SteamMainBase<SteamWorkshopMain>.Instance.IsDebugLogEnabled = true;
		uMyGUI_PopupManager.Instance.ShowPopup("loading");
		SteamMainBase<SteamWorkshopMain>.Instance.Sorting = new WorkshopSortMode(EWorkshopSource.OWNED);
		SteamMainBase<SteamWorkshopMain>.Instance.GetItemList(1U, new Action<WorkshopItemListEventArgs>(this.OnOwnedItemListLoaded));
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x000689E0 File Offset: 0x00066BE0
	private void OnOwnedItemListLoaded(WorkshopItemListEventArgs p_itemListArgs)
	{
		uMyGUI_PopupManager.Instance.HidePopup("loading");
		if (p_itemListArgs.IsError)
		{
			return;
		}
		if (p_itemListArgs.ItemList.Items.Count > 0)
		{
			string[] array = new string[p_itemListArgs.ItemList.Items.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = p_itemListArgs.ItemList.Items[i].Name;
			}
			((uMyGUI_PopupDropdown)uMyGUI_PopupManager.Instance.ShowPopup("dropdown")).SetEntries(array).SetOnSelected(delegate(int p_selectedIndex)
			{
				this.OnOwnedItemSelectedForUpdate(p_itemListArgs.ItemList.Items[p_selectedIndex]);
			}).SetText("Select Item", "Select the item, which you want to update");
			return;
		}
		((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText(Loc.T("UI.StartScreen.Mods.ModWarningNoItemsFoundHeader"), Loc.T("UI.StartScreen.Mods.ModWarningNoItemsFoundDescription")).ShowButton("ok");
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x00068AF8 File Offset: 0x00066CF8
	private void OnOwnedItemSelectedForUpdate(WorkshopItem p_item)
	{
		uMyGUI_PopupManager.Instance.HidePopup("dropdown");
		if (!p_item.IsInstalled)
		{
			((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText("Not Installed", "This item is not installed. Please subscribe this item first!").ShowButton("ok", new Action(this.Start));
			return;
		}
		WorkshopItemUpdate workshopItemUpdate = new WorkshopItemUpdate(p_item);
		string text = Path.Combine(workshopItemUpdate.ContentPath, "ItemData.txt");
		if (File.Exists(text))
		{
			File.AppendAllText(text, "\nUpdate - " + DateTime.Now.ToString());
			((SteamWorkshopPopupUpload)uMyGUI_PopupManager.Instance.ShowPopup("steam_ugc_upload")).UploadUI.SetItemData(workshopItemUpdate);
			return;
		}
		((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText("Not Installed", "This item is subscribed, but not installed. Please sync local files in Steam!").ShowButton("ok", new Action(this.Start));
	}
}
