using System;
using System.IO;
using LapinerTools.Steam;
using LapinerTools.Steam.Data;
using LapinerTools.Steam.UI;
using LapinerTools.uMyGUI;
using UnityEngine;

// Token: 0x0200040F RID: 1039
public class SteamWorkshopBrowseExamplePopup : MonoBehaviour
{
	// Token: 0x06001546 RID: 5446 RVA: 0x00068758 File Offset: 0x00066958
	private void Start()
	{
		SteamMainBase<SteamWorkshopMain>.Instance.IsDebugLogEnabled = true;
		((SteamWorkshopPopupBrowse)uMyGUI_PopupManager.Instance.ShowPopup("steam_ugc_browse")).BrowseUI.OnPlayButtonClick += delegate(WorkshopItemEventArgs p_itemArgs)
		{
			string text = "\n";
			try
			{
				string[] files = Directory.GetFiles(p_itemArgs.Item.InstalledLocalFolder);
				for (int i = 0; i < files.Length; i++)
				{
					text = text + files[i] + "\n";
				}
			}
			catch
			{
				text += "not found!";
			}
			string text2 = string.Concat(new string[]
			{
				"Name: ",
				p_itemArgs.Item.Name,
				"\nPublished File Id: ",
				p_itemArgs.Item.SteamNative.m_nPublishedFileId.ToString(),
				"\nLocal Folder: ",
				p_itemArgs.Item.InstalledLocalFolder,
				"\n",
				text
			});
			((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText("Item Played", "Load your Steam Workshop item here (e.g. could be a new level for your game)\n" + text2).ShowButton("ok");
		};
	}
}
