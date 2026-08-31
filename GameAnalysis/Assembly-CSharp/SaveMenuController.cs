using System;
using System.IO;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000435 RID: 1077
public class SaveMenuController : MenuController
{
	// Token: 0x0600164E RID: 5710 RVA: 0x00071C24 File Offset: 0x0006FE24
	private void Start()
	{
		this.saveList.SetSelectionCallback(new CreateSaveFileScrollList.SelectionCallback(this.SaveFileSelected));
		this.saveList.PopulateList();
		this.SaveGameHeader.SetText(Loc.T("UI.Save.Header"));
		this.ReturnText.SetText(Loc.T("UI.Save.Return"));
		this.DeleteText.SetText(Loc.T("UI.Save.Delete"));
		this.SaveText.SetText(Loc.T("UI.Save.Save"));
		this.savingScreen.SetActive(false);
		this.savingText.SetText(Loc.T("UI.Save.Saving"));
		this.deletePanelObject.SetActive(false);
		this.confirmDeleteButtonText.SetText(Loc.T("UI.Load.Delete"));
		this.cancelDeleteButtonText.SetText(Loc.T("UI.Load.Cancel"));
		this.InvalidSaveNameText.SetText(Loc.T("UI.Options.SaveNameInvalid"));
		this.InvalidSaveNameText.enabled = false;
		this.savingFailedOverlay.SetActive(false);
		this.thisCanvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x0600164F RID: 5711 RVA: 0x00071D38 File Offset: 0x0006FF38
	private void Awake()
	{
		this.saveFileString = this.GetDefaultSaveFileString();
		this.saveFileName.text = this.saveFileString;
		this.saveButton.interactable = !SaveMenuController.SavingIsBlocked();
		GameControl.eventManager.AddListener<SaveFilesChangedEvent>(new EventManager.EventDelegate<SaveFilesChangedEvent>(this.UpdateList), null, null, true, false);
	}

	// Token: 0x06001650 RID: 5712 RVA: 0x00071D8F File Offset: 0x0006FF8F
	private void OnEnable()
	{
		this.saveList.PopulateList();
	}

	// Token: 0x06001651 RID: 5713 RVA: 0x00071D9C File Offset: 0x0006FF9C
	private void OnDestroy()
	{
		GameControl.eventManager.RemoveListener<SaveFilesChangedEvent>(new EventManager.EventDelegate<SaveFilesChangedEvent>(this.UpdateList), null);
	}

	// Token: 0x06001652 RID: 5714 RVA: 0x00071DB5 File Offset: 0x0006FFB5
	private void UpdateList(SaveFilesChangedEvent e)
	{
		if (this.thisCanvasGroup != null && this.thisCanvasGroup.alpha == 1f)
		{
			this.saveList.PopulateList();
		}
	}

	// Token: 0x06001653 RID: 5715 RVA: 0x00071DE2 File Offset: 0x0006FFE2
	public override void OnOpen()
	{
		this.saveFileString = this.GetDefaultSaveFileString();
		this.saveFileName.text = this.saveFileString;
		this.saveButton.interactable = !SaveMenuController.SavingIsBlocked();
	}

	// Token: 0x06001654 RID: 5716 RVA: 0x00071E14 File Offset: 0x00070014
	private string GetDefaultSaveFileString()
	{
		return string.Concat(new string[]
		{
			GameControl.control.activePlayer.ideology.ideology.ToString(),
			"save",
			(this.saveList.lastSaveFileIndex + 1).ToString("D5"),
			"_",
			TITimeState.Now().year.ToString(),
			"-",
			TITimeState.Now().month.ToString(),
			"-",
			TITimeState.Now().day.ToString()
		});
	}

	// Token: 0x06001655 RID: 5717 RVA: 0x00071EC4 File Offset: 0x000700C4
	private void SaveFileSelected(SaveFile? selectedSaveInfo)
	{
		if (selectedSaveInfo == null)
		{
			if (string.IsNullOrEmpty(this.saveFileString))
			{
				this.saveButton.interactable = false;
			}
			this.deleteButton.interactable = false;
			this.saveFileName.textComponent.text = string.Empty;
			return;
		}
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
		this.saveButton.interactable = !SaveMenuController.SavingIsBlocked();
		this.deleteButton.interactable = true;
		this.saveFileString = selectedSaveInfo.Value.name;
		this.saveFileName.SetTextWithoutNotify(this.saveFileString);
		this.saveFileName.textComponent.text = this.saveFileString;
	}

	// Token: 0x06001656 RID: 5718 RVA: 0x00071F7C File Offset: 0x0007017C
	public bool ValidSaveFileName(string proposedName)
	{
		return !string.IsNullOrEmpty(proposedName) && proposedName.Length < 58 && proposedName.IndexOfAny(Path.GetInvalidPathChars()) < 0 && proposedName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 && proposedName.IndexOfAny(new char[] { '.' }) < 0 && this.CompressedFilenameAvailable(proposedName);
	}

	// Token: 0x06001657 RID: 5719 RVA: 0x00071FD4 File Offset: 0x000701D4
	private bool CompressedFilenameAvailable(string proposedName)
	{
		string text = CreateSaveFileScrollList.GetSaveFolderPath() + this.saveFileName.text + ".json";
		return !TIPlayerProfileManager.compressSaves || !File.Exists(text);
	}

	// Token: 0x06001658 RID: 5720 RVA: 0x0007200E File Offset: 0x0007020E
	public void TextEntryMode_Enter()
	{
		TIInputManager.BlockKeybindings();
	}

	// Token: 0x06001659 RID: 5721 RVA: 0x00072015 File Offset: 0x00070215
	public void TextEntryMode_End()
	{
		TIInputManager.RestoreKeybindings();
	}

	// Token: 0x0600165A RID: 5722 RVA: 0x0007201C File Offset: 0x0007021C
	public void NewFileNameTyped(string newName)
	{
		newName = this.saveFileName.textComponent.text;
		if (this.ValidSaveFileName(newName))
		{
			this.saveList.SelectSaveFile(null);
			this.saveFileString = newName;
			this.saveButton.interactable = !SaveMenuController.SavingIsBlocked();
			this.InvalidSaveNameText.enabled = false;
			return;
		}
		this.saveButton.interactable = false;
		this.InvalidSaveNameText.enabled = true;
		this.saveFileString = string.Empty;
	}

	// Token: 0x0600165B RID: 5723 RVA: 0x0007209C File Offset: 0x0007029C
	public void WriteSaveFile()
	{
		this.savingScreen.SetActive(true);
		LoadSaveButton loadSaveButton = this.saveList.selectedButton;
		if (TIPlayerProfileManager.compressSaves)
		{
			loadSaveButton = null;
		}
		string text = string.Empty;
		bool flag = true;
		if (loadSaveButton == null)
		{
			if (this.ValidSaveFileName(this.saveFileName.text))
			{
				text = TIUtilities.GetSaveFilePath(this.saveFileName.text);
				Debug.Log("Creating new savefile at path : " + text);
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			text = loadSaveButton.saveInfo.path;
			Debug.Log("Overwriting savefile at path : " + text);
		}
		if (flag)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			if (GameStateManager.SaveAllGameStates(text, false))
			{
				this.returnButton.onClick.Invoke();
			}
		}
		this.savingScreen.SetActive(false);
	}

	// Token: 0x0600165C RID: 5724 RVA: 0x00072168 File Offset: 0x00070368
	public void DisplaySavingFailedDialog(string errorMessage)
	{
		this.savingFailedDialog.Show(errorMessage);
		this.savingFailedOverlay.SetActive(true);
		this.optionsScreen.Show();
		base.GetComponentInParent<MenuManager>().ShowMenu(base.menu);
		this.saveList.PopulateList();
	}

	// Token: 0x0600165D RID: 5725 RVA: 0x000721B4 File Offset: 0x000703B4
	public void AskConfirmDeletion()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
		this.confirmDeleteText.SetText(Loc.T("UI.Load.ConfirmDeletionQuery", new object[] { this.saveList.selectedButton.saveInfo.name }));
		this.deletePanelObject.SetActive(true);
	}

	// Token: 0x0600165E RID: 5726 RVA: 0x0007220C File Offset: 0x0007040C
	public void OnConfirmDelete()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
		this.DeleteSaveFile();
		this.metadataScreenController.ClearUI();
	}

	// Token: 0x0600165F RID: 5727 RVA: 0x0007222B File Offset: 0x0007042B
	public void OnCancelDelete()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
		this.deletePanelObject.SetActive(false);
	}

	// Token: 0x06001660 RID: 5728 RVA: 0x00072248 File Offset: 0x00070448
	public void DeleteSaveFile()
	{
		LoadSaveButton selectedButton = this.saveList.selectedButton;
		if (selectedButton == null)
		{
			return;
		}
		Debug.Log("Trying to delete " + selectedButton.saveInfo.path);
		File.Delete(selectedButton.saveInfo.path);
		this.deletePanelObject.SetActive(false);
		GameControl.eventManager.TriggerEvent(new SaveFilesChangedEvent(), null, Array.Empty<object>());
	}

	// Token: 0x06001661 RID: 5729 RVA: 0x000722B6 File Offset: 0x000704B6
	private void OnApplicationFocus(bool focus)
	{
		CanvasGroup canvasGroup = this.thisCanvasGroup;
		if (canvasGroup != null && canvasGroup.alpha == (float)1 && focus)
		{
			this.saveList.PopulateList();
		}
	}

	// Token: 0x06001662 RID: 5730 RVA: 0x000722DD File Offset: 0x000704DD
	public static bool SavingIsBlocked()
	{
		if (TIPromptQueueState.ActivePlayerHasSaveBlockingPrompt())
		{
			return true;
		}
		TISpaceCombatState currentActiveCombat = TISpaceCombatState.CurrentActiveCombat;
		return currentActiveCombat != null && currentActiveCombat.autoresolving;
	}

	// Token: 0x040014B0 RID: 5296
	public static SaveMenuController Singleton;

	// Token: 0x040014B1 RID: 5297
	public OptionsScreenController optionsScreen;

	// Token: 0x040014B2 RID: 5298
	public CreateSaveFileScrollList saveList;

	// Token: 0x040014B3 RID: 5299
	public Button returnButton;

	// Token: 0x040014B4 RID: 5300
	public Button deleteButton;

	// Token: 0x040014B5 RID: 5301
	public Button saveButton;

	// Token: 0x040014B6 RID: 5302
	public MetadataScreenController metadataScreenController;

	// Token: 0x040014B7 RID: 5303
	public TMP_InputField saveFileName;

	// Token: 0x040014B8 RID: 5304
	private string saveFileString;

	// Token: 0x040014B9 RID: 5305
	public TMP_Text SaveGameHeader;

	// Token: 0x040014BA RID: 5306
	public TMP_Text ReturnText;

	// Token: 0x040014BB RID: 5307
	public TMP_Text DeleteText;

	// Token: 0x040014BC RID: 5308
	public TMP_Text SaveText;

	// Token: 0x040014BD RID: 5309
	public TMP_Text InvalidSaveNameText;

	// Token: 0x040014BE RID: 5310
	public GameObject savingScreen;

	// Token: 0x040014BF RID: 5311
	public TMP_Text savingText;

	// Token: 0x040014C0 RID: 5312
	public GameObject deletePanelObject;

	// Token: 0x040014C1 RID: 5313
	public TMP_Text confirmDeleteText;

	// Token: 0x040014C2 RID: 5314
	public TMP_Text confirmDeleteButtonText;

	// Token: 0x040014C3 RID: 5315
	public TMP_Text cancelDeleteButtonText;

	// Token: 0x040014C4 RID: 5316
	private CanvasGroup thisCanvasGroup;

	// Token: 0x040014C5 RID: 5317
	public GameObject savingFailedOverlay;

	// Token: 0x040014C6 RID: 5318
	public SavingFailedDialog savingFailedDialog;
}
