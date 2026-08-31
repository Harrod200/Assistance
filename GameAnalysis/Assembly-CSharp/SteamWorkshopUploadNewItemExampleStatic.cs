using System;
using System.IO;
using LapinerTools.Steam;
using LapinerTools.Steam.Data;
using LapinerTools.Steam.UI;
using LapinerTools.uMyGUI;
using UnityEngine;

// Token: 0x02000416 RID: 1046
public class SteamWorkshopUploadNewItemExampleStatic : MonoBehaviour
{
	// Token: 0x0600155E RID: 5470 RVA: 0x000693D4 File Offset: 0x000675D4
	private void Start()
	{
		SteamMainBase<SteamWorkshopMain>.Instance.IsDebugLogEnabled = true;
		if (SteamWorkshopUIUpload.Instance == null)
		{
			string text = "SteamWorkshopUploadNewItemExampleStatic: you have no SteamWorkshopUIUpload in this scene! Please drag an drop the 'SteamWorkshopItemUpload' prefab from 'LapinerTools/Steam/Workshop' into your Canvas object!";
			Debug.LogError(text);
			((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText("Error", text);
			return;
		}
		string text2 = Path.Combine(Application.persistentDataPath, "DummyItemContentFolder" + DateTime.Now.Ticks.ToString());
		if (!Directory.Exists(text2))
		{
			Directory.CreateDirectory(text2);
		}
		string text3 = "Save your item/level/mod data here.\nIt does not need to be a text file. Any file type is supported (binary, images, etc...).\nYou can save multiple files, Steam items are folders (not single files).\n";
		File.WriteAllText(Path.Combine(text2, "ItemData.txt"), text3);
		string text4 = "{\n\t\"title\": \"Example Mod Title\",\n\t\"author\": \"Author Name\",\n\t\"description\": \"Mod Description\",\n\t\"mod url\": \"https://steampowered.com/workshop/modurl\",\n\t\"LoadOrder\": 0,\n\t\"TemplatesToConcatArrays\": [\n\t\n\t],\n\t\"TemplatesToReplaceArrays\": [\n\t\n\t],\n\t\"TemplatesToReplace\": [\n\t\n\t]\n}";
		File.WriteAllText(Path.Combine(text2, "ModInfo.json"), text4);
		WorkshopItemUpdate workshopItemUpdate = new WorkshopItemUpdate();
		workshopItemUpdate.ContentPath = text2;
		SteamWorkshopUIUpload.Instance.SetItemData(workshopItemUpdate);
	}
}
