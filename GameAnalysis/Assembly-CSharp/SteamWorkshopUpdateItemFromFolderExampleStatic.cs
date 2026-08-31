using System;
using System.Collections.Generic;
using System.IO;
using LapinerTools.Steam;
using LapinerTools.Steam.Data;
using LapinerTools.Steam.UI;
using LapinerTools.uMyGUI;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000414 RID: 1044
public class SteamWorkshopUpdateItemFromFolderExampleStatic : MonoBehaviour
{
	// Token: 0x06001555 RID: 5461 RVA: 0x00068F28 File Offset: 0x00067128
	private void Start()
	{
		SteamMainBase<SteamWorkshopMain>.Instance.IsDebugLogEnabled = true;
		if (SteamWorkshopUIUpload.Instance == null)
		{
			string text = "SteamWorkshopUpdateItemFromFolderExampleStatic: you have no SteamWorkshopUIUpload in this scene! Please drag an drop the 'SteamWorkshopItemUpload' prefab from 'LapinerTools/Steam/Workshop' into your Canvas object!";
			Debug.LogError(text);
			((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText("Error", text);
			return;
		}
		List<WorkshopItemUpdate> itemsAvailableForUpdate = new List<WorkshopItemUpdate>();
		foreach (string text2 in Directory.GetDirectories(Application.persistentDataPath))
		{
			WorkshopItemUpdate itemUpdateFromFolder = SteamMainBase<SteamWorkshopMain>.Instance.GetItemUpdateFromFolder(text2);
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

	// Token: 0x06001556 RID: 5462 RVA: 0x00069090 File Offset: 0x00067290
	private void OnExistingItemSelectedForUpdate(WorkshopItemUpdate p_updateExistingItem)
	{
		uMyGUI_PopupManager.Instance.HidePopup("dropdown");
		string text = Path.Combine(p_updateExistingItem.ContentPath, "ItemData.txt");
		if (File.Exists(text))
		{
			File.AppendAllText(text, "\nUpdate - " + DateTime.Now.ToString());
			SteamWorkshopUIUpload.Instance.SetItemData(p_updateExistingItem);
			return;
		}
		((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText("Content File Is Missing", "Have you changed this item's data?!").ShowButton("ok");
	}
}
