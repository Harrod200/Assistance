using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PavonisInteractive.TerraInvicta;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000447 RID: 1095
public class CreateSaveFileScrollList : MonoBehaviour
{
	// Token: 0x060016DA RID: 5850 RVA: 0x00075555 File Offset: 0x00073755
	private void Start()
	{
		this.normalSprite = this.loadSaveGameButton.GetComponent<Image>().sprite;
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x0007556D File Offset: 0x0007376D
	public void OnApplicationQuit()
	{
	}

	// Token: 0x060016DC RID: 5852 RVA: 0x0007556F File Offset: 0x0007376F
	public void SetSelectionCallback(CreateSaveFileScrollList.SelectionCallback selFn)
	{
		this.currentCallbackFn = selFn;
	}

	// Token: 0x060016DD RID: 5853 RVA: 0x00075578 File Offset: 0x00073778
	public void PopulateList()
	{
		if (this.contentPanel != null)
		{
			foreach (object obj in this.contentPanel)
			{
				global::UnityEngine.Object.Destroy(((Transform)obj).gameObject);
			}
		}
		List<SaveFile> list = new List<SaveFile>();
		string[] array;
		if (TIPlayerProfileManager.compressSaves)
		{
			array = Directory.GetFiles(CreateSaveFileScrollList.GetSaveFolderPath(), "*.gz");
		}
		else
		{
			array = Directory.GetFiles(CreateSaveFileScrollList.GetSaveFolderPath(), "*.json");
		}
		foreach (string text in array)
		{
			string text2 = Path.GetFileNameWithoutExtension(text);
			if (text2 != null && text2 != "")
			{
				SaveFile saveFile;
				saveFile.name = text2;
				saveFile.dateTime = File.GetLastWriteTime(text);
				saveFile.path = text;
				saveFile.invalid = false;
				list.Add(saveFile);
			}
			if (text2.ToLower().Contains("save"))
			{
				int num = text2.IndexOf("_");
				if (num > 0)
				{
					text2 = text2.Substring(0, num);
				}
				string text3 = new Regex("[^\\d]").Replace(text2, "");
				int num2 = 1;
				int.TryParse(text3, out num2);
				if (num2 > this.lastSaveFileIndex)
				{
					this.lastSaveFileIndex = num2;
				}
			}
		}
		if (list != null)
		{
			list = list.OrderByDescending<SaveFile, DateTime>((SaveFile x) => x.dateTime).ToList<SaveFile>();
			foreach (SaveFile saveFile2 in list)
			{
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.loadSaveGameButton, this.contentPanel);
				gameObject.name = saveFile2.name + "Button";
				LoadSaveButton button = gameObject.GetComponent<LoadSaveButton>();
				button.SetSaveFileInfo(saveFile2);
				button.button.onClick.AddListener(delegate
				{
					this.SelectSaveFile(button);
				});
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}

	// Token: 0x060016DE RID: 5854 RVA: 0x000757EC File Offset: 0x000739EC
	public static string GetSaveFolderPath()
	{
		if (CreateSaveFileScrollList.savedGamesPath == null)
		{
			CreateSaveFileScrollList.savedGamesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			CreateSaveFileScrollList.savedGamesPath = CreateSaveFileScrollList.savedGamesPath.Replace("\\", "/");
			CreateSaveFileScrollList.savedGamesPath += "/My Games/TerraInvicta/Saves/";
			if (TIPlayerProfileManager.useAlternateSavePath)
			{
				CreateSaveFileScrollList.savedGamesPath = TIPlayerProfileManager.alternateSavePath;
			}
			Log.Info("Setting savedGamesPath: " + CreateSaveFileScrollList.savedGamesPath, Array.Empty<object>());
		}
		return CreateSaveFileScrollList.savedGamesPath;
	}

	// Token: 0x060016DF RID: 5855 RVA: 0x00075868 File Offset: 0x00073A68
	public void SelectSaveFile(LoadSaveButton saveButton)
	{
		if (this.selectedButton != null)
		{
			this.selectedButton.button.GetComponent<Image>().sprite = this.normalSprite;
		}
		this.selectedButton = saveButton;
		if (this.selectedButton != null)
		{
			bool flag;
			TIMetadataState timetadataState = TIMetadataState.LoadMetaData(this.selectedButton.saveInfo.path, out flag, true);
			this.metadataScreenController.RefreshUIWithMetaData(timetadataState, this.selectedButton.saveInfo.name);
			if (!flag)
			{
				this.selectedButton.saveInfo.invalid = true;
			}
		}
		if (this.selectedButton != null)
		{
			this.selectedButton.button.GetComponent<Image>().sprite = this.highlightedSprite;
			this.currentCallbackFn(new SaveFile?(this.selectedButton.saveInfo));
			return;
		}
		if (this.currentCallbackFn != null)
		{
			this.currentCallbackFn(null);
		}
	}

	// Token: 0x060016E0 RID: 5856 RVA: 0x00075960 File Offset: 0x00073B60
	private void OnEnable()
	{
		string saveFolderPath = CreateSaveFileScrollList.GetSaveFolderPath();
		if (!Directory.Exists(saveFolderPath))
		{
			try
			{
				Directory.CreateDirectory(saveFolderPath);
			}
			catch
			{
				Debug.Log("Unable to create directory at " + saveFolderPath);
			}
		}
	}

	// Token: 0x060016E1 RID: 5857 RVA: 0x000759A8 File Offset: 0x00073BA8
	private void OnDisable()
	{
	}

	// Token: 0x060016E2 RID: 5858 RVA: 0x000759AA File Offset: 0x00073BAA
	private void OnChanged(object source, FileSystemEventArgs e)
	{
		this.PopulateList();
	}

	// Token: 0x060016E3 RID: 5859 RVA: 0x000759B2 File Offset: 0x00073BB2
	private void OnRenamed(object source, RenamedEventArgs e)
	{
	}

	// Token: 0x04001550 RID: 5456
	private Sprite normalSprite;

	// Token: 0x04001551 RID: 5457
	public Sprite highlightedSprite;

	// Token: 0x04001552 RID: 5458
	public GameObject loadSaveGameButton;

	// Token: 0x04001553 RID: 5459
	public Transform contentPanel;

	// Token: 0x04001554 RID: 5460
	public LoadSaveButton selectedButton;

	// Token: 0x04001555 RID: 5461
	public MetadataScreenController metadataScreenController;

	// Token: 0x04001556 RID: 5462
	public CreateSaveFileScrollList.SelectionCallback currentCallbackFn;

	// Token: 0x04001557 RID: 5463
	public int lastSaveFileIndex;

	// Token: 0x04001558 RID: 5464
	private static string savedGamesPath;

	// Token: 0x02000C3C RID: 3132
	// (Invoke) Token: 0x06006C1B RID: 27675
	public delegate void SelectionCallback(SaveFile? saveInfo);
}
