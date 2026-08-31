using System;
using System.Collections.Generic;
using System.IO;
using LapinerTools.Steam;
using LapinerTools.Steam.Data;
using LapinerTools.Steam.UI;
using LapinerTools.uMyGUI;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000410 RID: 1040
public class SteamWorkshopUpdateItemFromFolderExamplePopup : MonoBehaviour
{
	// Token: 0x06001548 RID: 5448 RVA: 0x000687B8 File Offset: 0x000669B8
	private void Start()
	{
		SteamMainBase<SteamWorkshopMain>.Instance.IsDebugLogEnabled = true;
		List<WorkshopItemUpdate> itemsAvailableForUpdate = new List<WorkshopItemUpdate>();
		foreach (string text in Directory.GetDirectories(Application.persistentDataPath))
		{
			WorkshopItemUpdate itemUpdateFromFolder = SteamMainBase<SteamWorkshopMain>.Instance.GetItemUpdateFromFolder(text);
			if (itemUpdateFromFolder != null)
			{
				itemsAvailableForUpdate.Add(itemUpdateFromFolder);
			}
		}
		if (itemsAvailableForUpdate.Count > 0)
		{
			string[] array = new string[itemsAvailableForUpdate.Count];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = itemsAvailableForUpdate[j].Name;
			}
			((uMyGUI_PopupDropdown)uMyGUI_PopupManager.Instance.ShowPopup("dropdown")).SetEntries(array).SetOnSelected(delegate(int p_selectedIndex)
			{
				this.OnExistingItemSelectedForUpdate(itemsAvailableForUpdate[p_selectedIndex]);
			}).SetText("Select Item", "Select the item, which you want to update");
			return;
		}
		((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText(Loc.T("UI.StartScreen.Mods.ModWarningNoItemsFoundHeader"), Loc.T("UI.StartScreen.Mods.ModWarningNoItemsFoundDescription")).ShowButton("ok");
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x000688E4 File Offset: 0x00066AE4
	private void OnExistingItemSelectedForUpdate(WorkshopItemUpdate p_updateExistingItem)
	{
		uMyGUI_PopupManager.Instance.HidePopup("dropdown");
		string text = Path.Combine(p_updateExistingItem.ContentPath, "ItemData.txt");
		if (File.Exists(text))
		{
			File.AppendAllText(text, "\nUpdate - " + DateTime.Now.ToString());
			((SteamWorkshopPopupUpload)uMyGUI_PopupManager.Instance.ShowPopup("steam_ugc_upload")).UploadUI.SetItemData(p_updateExistingItem);
			return;
		}
		((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText("Content File Is Missing", "Have you changed this item's data?!").ShowButton("ok");
	}
}
