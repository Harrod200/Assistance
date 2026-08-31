using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LapinerTools.Steam;
using LapinerTools.Steam.Data;
using LapinerTools.Steam.UI;
using LapinerTools.uMyGUI;
using UnityEngine;

// Token: 0x0200040E RID: 1038
public class SteamWorkshopTagsExample : MonoBehaviour
{
	// Token: 0x06001543 RID: 5443 RVA: 0x000684E1 File Offset: 0x000666E1
	private void Start()
	{
		SteamMainBase<SteamWorkshopMain>.Instance.IsDebugLogEnabled = true;
		SteamMainBase<SteamWorkshopMain>.Instance.SearchMatchAnyTag = true;
	}

	// Token: 0x06001544 RID: 5444 RVA: 0x000684FC File Offset: 0x000666FC
	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0f, (float)(Screen.height - 28), (float)Screen.width, 28f));
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("Browse With Tags", new GUILayoutOption[] { GUILayout.Height(28f) }))
		{
			if (SteamWorkshopUIBrowse.Instance != null)
			{
				SteamWorkshopUIBrowse.Instance.LoadItems(1);
			}
			else
			{
				((SteamWorkshopPopupBrowse)uMyGUI_PopupManager.Instance.ShowPopup("steam_ugc_browse")).BrowseUI.OnPlayButtonClick += delegate(WorkshopItemEventArgs p_itemArgs)
				{
					((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText("Item Played", "Item Name: " + p_itemArgs.Item.Name + "\nFor further item details check SteamWorkshopBrowseExamplePopup or SteamWorkshopBrowseExampleStatic classes.").ShowButton("ok");
				};
			}
		}
		if (GUILayout.Button("Upload With Tags", new GUILayoutOption[] { GUILayout.Height(40f) }))
		{
			string text = Path.Combine(Application.persistentDataPath, "DummyItemContentFolder" + DateTime.Now.Ticks.ToString());
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string text2 = "Save your item/level/mod data here.\nIt does not need to be a text file. Any file type is supported (binary, images, etc...).\nYou can save multiple files, Steam items are folders (not single files).\n";
			File.WriteAllText(Path.Combine(text, "ItemData.txt"), text2);
			WorkshopItemUpdate workshopItemUpdate = new WorkshopItemUpdate();
			workshopItemUpdate.ContentPath = text;
			workshopItemUpdate.Tags = this.m_tagsToUse;
			SteamWorkshopPopupUpload steamWorkshopPopupUpload = (SteamWorkshopPopupUpload)uMyGUI_PopupManager.Instance.ShowPopup("steam_ugc_upload");
			steamWorkshopPopupUpload.UploadUI.SetItemData(workshopItemUpdate);
			steamWorkshopPopupUpload.UploadUI.OnFinishedUpload += delegate(WorkshopItemUpdateEventArgs p_args)
			{
				if (!p_args.IsError && p_args.Item != null)
				{
					uMyGUI_PopupText uMyGUI_PopupText = (uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text");
					string text3 = "Item Uploaded";
					string[] array = new string[5];
					array[0] = "Item '";
					array[1] = p_args.Item.Name;
					array[2] = "' was successfully uploaded!\nTags: ";
					array[3] = p_args.Item.Tags.Aggregate<string>((string tag1, string tag2) => tag1 + ", " + tag2);
					array[4] = "\nIt can take a long time for this new level to arrive in the Steam Workshop listing, sometimes longer than an hour! Be patient...";
					uMyGUI_PopupText.SetText(text3, string.Concat(array)).ShowButton("ok");
				}
			};
		}
		for (int i = 0; i < this.TAGS.Length; i++)
		{
			bool flag = this.m_tagsToUse.Contains(this.TAGS[i]);
			bool flag2 = GUILayout.Toggle(flag, this.TAGS[i], Array.Empty<GUILayoutOption>());
			if (flag2 && !flag)
			{
				this.m_tagsToUse.Add(this.TAGS[i]);
			}
			if (!flag2 && flag)
			{
				this.m_tagsToUse.Remove(this.TAGS[i]);
			}
			SteamMainBase<SteamWorkshopMain>.Instance.SearchTags = this.m_tagsToUse;
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	// Token: 0x040012B1 RID: 4785
	[SerializeField]
	public string[] TAGS = new string[] { "TAG1", "TAG2", "TAG3", "TAG4" };

	// Token: 0x040012B2 RID: 4786
	private List<string> m_tagsToUse = new List<string>();
}
