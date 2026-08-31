using System;
using System.IO;
using LapinerTools.Steam;
using LapinerTools.Steam.Data;
using LapinerTools.Steam.UI;
using LapinerTools.uMyGUI;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000415 RID: 1045
public class SteamWorkshopUpdateOwnedItemExampleStatic : MonoBehaviour
{
	// Token: 0x06001558 RID: 5464 RVA: 0x00069125 File Offset: 0x00067325
	private void Start()
	{
	}

	// Token: 0x06001559 RID: 5465 RVA: 0x00069127 File Offset: 0x00067327
	private void OnEnable()
	{
	}

	// Token: 0x0600155A RID: 5466 RVA: 0x0006912C File Offset: 0x0006732C
	public void ShowOwnedModPopup()
	{
		SteamMainBase<SteamWorkshopMain>.Instance.IsDebugLogEnabled = true;
		if (SteamWorkshopUIUpload.Instance == null)
		{
			string text = "SteamWorkshopUpdateOwnedItemExampleStatic: you have no SteamWorkshopUIUpload in this scene! Please drag an drop the 'SteamWorkshopItemUpload' prefab from 'LapinerTools/Steam/Workshop' into your Canvas object!";
			Debug.LogError(text);
			((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText("Error", text);
			return;
		}
		uMyGUI_PopupManager.Instance.ShowPopup("loading");
		SteamMainBase<SteamWorkshopMain>.Instance.Sorting = new WorkshopSortMode(EWorkshopSource.OWNED);
		SteamMainBase<SteamWorkshopMain>.Instance.GetItemList(1U, new Action<WorkshopItemListEventArgs>(this.OnOwnedItemListLoaded));
	}

	// Token: 0x0600155B RID: 5467 RVA: 0x000691B8 File Offset: 0x000673B8
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

	// Token: 0x0600155C RID: 5468 RVA: 0x000692D0 File Offset: 0x000674D0
	private void OnOwnedItemSelectedForUpdate(WorkshopItem p_item)
	{
		uMyGUI_PopupManager.Instance.HidePopup("dropdown");
		if (!p_item.IsInstalled)
		{
			((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText("Not Installed", "This item is not installed. Please subscribe this item first!").ShowButton("ok");
			this.Start();
			return;
		}
		WorkshopItemUpdate workshopItemUpdate = new WorkshopItemUpdate(p_item);
		string text = Path.Combine(workshopItemUpdate.ContentPath, "ItemData.txt");
		if (!File.Exists(text))
		{
			string text2 = "Save your item/level/mod data here.\nIt does not need to be a text file. Any file type is supported (binary, images, etc...).\nYou can save multiple files, Steam items are folders (not single files).\n";
			File.WriteAllText(Path.Combine(workshopItemUpdate.ContentPath, "ItemData.txt"), text2);
		}
		if (File.Exists(text))
		{
			File.AppendAllText(text, "\nUpdate - " + DateTime.Now.ToString());
			SteamWorkshopUIUpload.Instance.SetItemData(workshopItemUpdate);
			return;
		}
		((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText("Not Installed", "This item is subscribed, but not installed. Please sync local files in Steam!").ShowButton("ok");
		this.Start();
	}
}
