using System;
using System.IO;
using LapinerTools.Steam;
using LapinerTools.Steam.Data;
using LapinerTools.Steam.UI;
using LapinerTools.uMyGUI;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000413 RID: 1043
public class SteamWorkshopBrowseExampleStatic : MonoBehaviour
{
	// Token: 0x06001551 RID: 5457 RVA: 0x00068C98 File Offset: 0x00066E98
	private void Start()
	{
		SteamMainBase<SteamWorkshopMain>.Instance.IsDebugLogEnabled = true;
		if (SteamWorkshopUIBrowse.Instance == null)
		{
			string text = "SteamWorkshopBrowseExampleStatic: you have no SteamWorkshopUIBrowse in this scene! Please drag an drop the 'SteamWorkshopItemBrowser' prefab from 'LapinerTools/Steam/Workshop' into your Canvas object!";
			Debug.LogError(text);
			((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText("Error", text);
			return;
		}
		SteamWorkshopUIBrowse.Instance.OnPlayButtonClick += delegate(WorkshopItemEventArgs p_itemArgs)
		{
			string text2 = "\n";
			try
			{
				if (!Directory.Exists("Mods/Enabled/" + p_itemArgs.Item.SanitizedName))
				{
					Directory.CreateDirectory("Mods/Enabled/" + p_itemArgs.Item.SanitizedName);
				}
				this.CopyDirectory(p_itemArgs.Item.InstalledLocalFolder, "Mods/Enabled/" + p_itemArgs.Item.SanitizedName, true);
			}
			catch
			{
				text2 += Loc.T("UI.StartScreen.Mods.ModDownloadError");
			}
			string text3 = string.Concat(new string[]
			{
				Loc.T("UI.StartScreen.Mods.ModDownloadedName"),
				p_itemArgs.Item.Name,
				"\n",
				Loc.T("UI.StartScreen.Mods.ModDownloadedID"),
				p_itemArgs.Item.SteamNative.m_nPublishedFileId.ToString(),
				"\n",
				Loc.T("UI.StartScreen.Mods.ModDownloadedLocalFolder"),
				p_itemArgs.Item.InstalledLocalFolder,
				"\nMods/Enabled/",
				p_itemArgs.Item.Name
			});
			((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText(Loc.T("UI.StartScreen.Mods.ModDownloaded"), Loc.T("UI.StartScreen.Mods.ModDownloadedInfo") + text3).ShowButton("ok");
		};
	}

	// Token: 0x06001552 RID: 5458 RVA: 0x00068D00 File Offset: 0x00066F00
	private void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(sourceDir);
		if (!directoryInfo.Exists)
		{
			throw new DirectoryNotFoundException("Source directory not found: " + directoryInfo.FullName);
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		Directory.CreateDirectory(destinationDir);
		foreach (FileInfo fileInfo in directoryInfo.GetFiles())
		{
			string text = Path.Combine(destinationDir, fileInfo.Name);
			fileInfo.CopyTo(text, true);
		}
		if (recursive)
		{
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				string text2 = Path.Combine(destinationDir, directoryInfo2.Name);
				this.CopyDirectory(directoryInfo2.FullName, text2, true);
			}
		}
	}
}
