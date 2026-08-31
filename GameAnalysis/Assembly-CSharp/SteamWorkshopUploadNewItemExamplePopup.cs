using System;
using System.IO;
using LapinerTools.Steam;
using LapinerTools.Steam.Data;
using LapinerTools.Steam.UI;
using LapinerTools.uMyGUI;
using UnityEngine;

// Token: 0x02000412 RID: 1042
public class SteamWorkshopUploadNewItemExamplePopup : MonoBehaviour
{
	// Token: 0x0600154F RID: 5455 RVA: 0x00068BF8 File Offset: 0x00066DF8
	private void Start()
	{
		SteamMainBase<SteamWorkshopMain>.Instance.IsDebugLogEnabled = true;
		string text = Path.Combine(Application.persistentDataPath, "DummyItemContentFolder" + DateTime.Now.Ticks.ToString());
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string text2 = "Save your item/level/mod data here.\nIt does not need to be a text file. Any file type is supported (binary, images, etc...).\nYou can save multiple files, Steam items are folders (not single files).\n";
		File.WriteAllText(Path.Combine(text, "ItemData.txt"), text2);
		WorkshopItemUpdate workshopItemUpdate = new WorkshopItemUpdate();
		workshopItemUpdate.ContentPath = text;
		((SteamWorkshopPopupUpload)uMyGUI_PopupManager.Instance.ShowPopup("steam_ugc_upload")).UploadUI.SetItemData(workshopItemUpdate);
	}
}
