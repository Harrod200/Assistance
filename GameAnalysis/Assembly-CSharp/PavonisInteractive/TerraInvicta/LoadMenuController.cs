using System;
using System.IO;
using System.Threading;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008FE RID: 2302
	public class LoadMenuController : MenuController
	{
		// Token: 0x17000F2A RID: 3882
		// (get) Token: 0x0600581B RID: 22555 RVA: 0x00286BEA File Offset: 0x00284DEA
		public StartMenuController startMenuController
		{
			get
			{
				return base.GetComponentInParent<StartMenuController>();
			}
		}

		// Token: 0x0600581C RID: 22556 RVA: 0x00286BF4 File Offset: 0x00284DF4
		private void Start()
		{
			if (SceneManager.self.onSolarSystem)
			{
				this.sceneManager = SolarSystemInstaller.container.Resolve<SceneManager>();
			}
			else if (SceneManager.self.onStartScreen)
			{
				this.sceneManager = StartScreenInstaller.container.Resolve<SceneManager>();
			}
			this.saveList.SetSelectionCallback(new CreateSaveFileScrollList.SelectionCallback(this.SaveFileSelected));
			this.saveList.PopulateList();
			this.loadingScreen.SetActive(false);
			this.deletePanelObject.SetActive(false);
			this.selectedFilename.SetText(string.Empty);
			this.LoadLocalizedText();
			this.openSaveFolderButton.gameObject.SetActive(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor);
			Loc.OnLanguageChangedEvent += this.OnLanguageChangedEvent;
			GameControl.eventManager.AddListener<SaveFilesChangedEvent>(new EventManager.EventDelegate<SaveFilesChangedEvent>(this.UpdateList), null, null, true, false);
			if (!this.importMode)
			{
				this.EnterLoadMode();
			}
			this.thisCanvasGroup = base.GetComponent<CanvasGroup>();
		}

		// Token: 0x0600581D RID: 22557 RVA: 0x00286CF4 File Offset: 0x00284EF4
		private void LoadLocalizedText()
		{
			this.loadHeader.SetText(Loc.T("UI.Load.Header"));
			this.loadButtonText.SetText(Loc.T("UI.Load.Load"));
			this.deleteButtonText.SetText(Loc.T("UI.Load.Delete"));
			this.confirmDeleteText.SetText(Loc.T("UI.Load.ConfirmDeletionQuery"));
			this.confirmDeleteButtonText.SetText(Loc.T("UI.Load.Delete"));
			this.cancelDeleteButtonText.SetText(Loc.T("UI.Load.Cancel"));
			this.openSaveFolderText.SetText(Loc.T("UI.StartScreen.OpenSaveFolder"));
			if (this.isImportingAvailable)
			{
				this.importingPopupText.text = Loc.T("UI.Import.Importing");
			}
		}

		// Token: 0x0600581E RID: 22558 RVA: 0x00286DB1 File Offset: 0x00284FB1
		private void OnLanguageChangedEvent()
		{
			this.LoadLocalizedText();
			Loc.SwapFonts(base.gameObject);
		}

		// Token: 0x0600581F RID: 22559 RVA: 0x00286DC4 File Offset: 0x00284FC4
		private void Update()
		{
			if (this.isImporting)
			{
				this.secondsSinceLastWorkCycle += Time.deltaTime;
				if (this.secondsSinceLastWorkCycle > 1f)
				{
					this.importingPopupText.text = Loc.T("UI.Import.Importing") + new string('.', this.workCycleCount = (this.workCycleCount + 1) % 4);
					this.secondsSinceLastWorkCycle = 0f;
				}
				if (this.importedSaveStructure != null)
				{
					this.isImporting = false;
					Action<SaveStructure> action = this.importCallback;
					this.startMenuController.menuManager.HideMenu();
					action(this.importedSaveStructure);
					this.importedSaveStructure = null;
				}
			}
		}

		// Token: 0x06005820 RID: 22560 RVA: 0x00286E71 File Offset: 0x00285071
		private void OnEnable()
		{
			this.saveList.PopulateList();
		}

		// Token: 0x06005821 RID: 22561 RVA: 0x00286E7E File Offset: 0x0028507E
		private void OnDestroy()
		{
			Loc.OnLanguageChangedEvent -= this.OnLanguageChangedEvent;
			GameControl.eventManager.RemoveListener<SaveFilesChangedEvent>(new EventManager.EventDelegate<SaveFilesChangedEvent>(this.UpdateList), null);
		}

		// Token: 0x06005822 RID: 22562 RVA: 0x00286EA8 File Offset: 0x002850A8
		private void OnApplicationFocus(bool focus)
		{
			CanvasGroup canvasGroup = this.thisCanvasGroup;
			if (canvasGroup != null && canvasGroup.alpha == (float)1 && focus)
			{
				this.saveList.PopulateList();
			}
		}

		// Token: 0x06005823 RID: 22563 RVA: 0x00286ED0 File Offset: 0x002850D0
		public override void OnClose()
		{
			base.OnClose();
			if (this.importMode)
			{
				this.isImporting = false;
				if (this.cancelImportCallback != null)
				{
					this.cancelImportCallback();
				}
				this.EnterLoadMode();
				this.startMenuController.buttonsCanvasGroup.interactable = true;
			}
		}

		// Token: 0x06005824 RID: 22564 RVA: 0x00286F1C File Offset: 0x0028511C
		private void UpdateList(SaveFilesChangedEvent e)
		{
			CanvasGroup canvasGroup = this.thisCanvasGroup;
			if (canvasGroup != null && canvasGroup.alpha == (float)1)
			{
				this.saveList.PopulateList();
			}
		}

		// Token: 0x06005825 RID: 22565 RVA: 0x00286F44 File Offset: 0x00285144
		private void SaveFileSelected(SaveFile? saveFileButton)
		{
			if (saveFileButton == null || this.saveList.selectedButton.saveInfo.invalid)
			{
				this.loadButton.interactable = false;
				this.deleteButton.interactable = false;
				this.selectedFilename.SetText(string.Empty);
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.loadButton.interactable = true;
			this.deleteButton.interactable = true;
			this.selectedFilename.SetText(this.saveList.selectedButton.saveInfo.name);
		}

		// Token: 0x17000F2B RID: 3883
		// (get) Token: 0x06005826 RID: 22566 RVA: 0x00286FEA File Offset: 0x002851EA
		private bool isImportingAvailable
		{
			get
			{
				return this.importingPopup != null;
			}
		}

		// Token: 0x17000F2C RID: 3884
		// (get) Token: 0x06005827 RID: 22567 RVA: 0x00286FF8 File Offset: 0x002851F8
		// (set) Token: 0x06005828 RID: 22568 RVA: 0x00287014 File Offset: 0x00285214
		private bool isImporting
		{
			get
			{
				return this.isImportingAvailable && this.importingPopup.gameObject.activeSelf;
			}
			set
			{
				this.importingPopup.gameObject.SetActive(value);
				this.startMenuController.canvasGroup.interactable = !value;
			}
		}

		// Token: 0x06005829 RID: 22569 RVA: 0x0028703C File Offset: 0x0028523C
		public void LoadSaveFile()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			LoadSaveButton selectedButton = this.saveList.selectedButton;
			if (selectedButton == null)
			{
				return;
			}
			this.LoadSaveFilePath(selectedButton.saveInfo.path);
		}

		// Token: 0x0600582A RID: 22570 RVA: 0x0028707C File Offset: 0x0028527C
		public void LoadSaveFilePath(string saveFilePath)
		{
			if (this.importMode)
			{
				this.isImporting = true;
				new Thread(delegate
				{
					this.importedSaveStructure = SaveStructure.Load(saveFilePath);
				}).Start();
				return;
			}
			this.loadingScreen.SetActive(true);
			GameControl.control.skirmishMode = false;
			TIArmyState.FinishBakingJourneyHeuristic();
			GameControl.control.viewMgr.ClearGameData(true);
			this.sceneManager.LoadScene("SolarSystemScene", delegate(DiContainer container)
			{
				container.BindInstance<string>(saveFilePath).WhenInjectedInto<SolarSystemBootstrap>();
			});
		}

		// Token: 0x0600582B RID: 22571 RVA: 0x0028710C File Offset: 0x0028530C
		public void AskConfirmDeletion()
		{
			if (this.importMode)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
				this.startMenuController.menuManager.HideMenu();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
			if (this.saveList.selectedButton == null)
			{
				return;
			}
			this.confirmDeleteText.SetText(Loc.T("UI.Load.ConfirmDeletionQuery", new object[] { this.saveList.selectedButton.saveInfo.name }));
			this.deletePanelObject.SetActive(true);
		}

		// Token: 0x0600582C RID: 22572 RVA: 0x002871A0 File Offset: 0x002853A0
		public void OnConfirmDelete()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Decline", false, false);
			this.DeleteSaveFile();
			this.SaveFileSelected(null);
			this.metadataScreenController.ClearUI();
		}

		// Token: 0x0600582D RID: 22573 RVA: 0x002871D9 File Offset: 0x002853D9
		public void OnCancelDelete()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			this.deletePanelObject.SetActive(false);
		}

		// Token: 0x0600582E RID: 22574 RVA: 0x002871F4 File Offset: 0x002853F4
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
			if (this.continueButton != null)
			{
				this.continueButton.interactable = File.Exists(StartMenuController.continueSaveFilepath);
			}
		}

		// Token: 0x17000F2D RID: 3885
		// (get) Token: 0x0600582F RID: 22575 RVA: 0x00287285 File Offset: 0x00285485
		public bool importMode
		{
			get
			{
				return this.importCallback != null;
			}
		}

		// Token: 0x06005830 RID: 22576 RVA: 0x00287290 File Offset: 0x00285490
		public void EnterImportMode(Action<SaveStructure> importCallback_, Action cancelImportCallback_ = null)
		{
			if (!this.isImportingAvailable)
			{
				return;
			}
			this.importCallback = importCallback_;
			this.cancelImportCallback = cancelImportCallback_;
			this.loadHeader.SetText(Loc.T("UI.Import.Header"));
			this.loadButtonText.SetText(Loc.T("UI.Import.Import"));
			this.deleteButtonText.SetText(Loc.T("UI.Load.Cancel"));
			this.startMenuController.menuManager.ShowMenu(base.menu);
			this.startMenuController.buttonsCanvasGroup.interactable = false;
		}

		// Token: 0x06005831 RID: 22577 RVA: 0x0028731C File Offset: 0x0028551C
		public void EnterLoadMode()
		{
			this.importCallback = null;
			this.cancelImportCallback = null;
			this.loadHeader.SetText(Loc.T("UI.Load.Header"));
			this.loadButtonText.SetText(Loc.T("UI.Load.Load"));
			this.deleteButtonText.SetText(Loc.T("UI.Load.Delete"));
		}

		// Token: 0x06005832 RID: 22578 RVA: 0x00287376 File Offset: 0x00285576
		public void OnOpenSaveFolderClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericSelect", false, false);
			TIUtilities.OpenFileSystemURL(CreateSaveFileScrollList.GetSaveFolderPath());
		}

		// Token: 0x04003FAE RID: 16302
		public CreateSaveFileScrollList saveList;

		// Token: 0x04003FAF RID: 16303
		public Button loadButton;

		// Token: 0x04003FB0 RID: 16304
		public Button deleteButton;

		// Token: 0x04003FB1 RID: 16305
		public Button continueButton;

		// Token: 0x04003FB2 RID: 16306
		public Button openSaveFolderButton;

		// Token: 0x04003FB3 RID: 16307
		public MetadataScreenController metadataScreenController;

		// Token: 0x04003FB4 RID: 16308
		private SceneManager sceneManager;

		// Token: 0x04003FB5 RID: 16309
		public TMP_Text loadHeader;

		// Token: 0x04003FB6 RID: 16310
		public TMP_Text loadButtonText;

		// Token: 0x04003FB7 RID: 16311
		public TMP_Text deleteButtonText;

		// Token: 0x04003FB8 RID: 16312
		public TMP_Text openSaveFolderText;

		// Token: 0x04003FB9 RID: 16313
		public GameObject loadingScreen;

		// Token: 0x04003FBA RID: 16314
		public TMP_Text loadingText;

		// Token: 0x04003FBB RID: 16315
		public GameObject deletePanelObject;

		// Token: 0x04003FBC RID: 16316
		public TMP_Text confirmDeleteText;

		// Token: 0x04003FBD RID: 16317
		public TMP_Text confirmDeleteButtonText;

		// Token: 0x04003FBE RID: 16318
		public TMP_Text cancelDeleteButtonText;

		// Token: 0x04003FBF RID: 16319
		public TMP_Text selectedFilename;

		// Token: 0x04003FC0 RID: 16320
		public GameObject importingPopup;

		// Token: 0x04003FC1 RID: 16321
		public TMP_Text importingPopupText;

		// Token: 0x04003FC2 RID: 16322
		private CanvasGroup thisCanvasGroup;

		// Token: 0x04003FC3 RID: 16323
		private int workCycleCount;

		// Token: 0x04003FC4 RID: 16324
		private float secondsSinceLastWorkCycle;

		// Token: 0x04003FC5 RID: 16325
		private SaveStructure importedSaveStructure;

		// Token: 0x04003FC6 RID: 16326
		private Action<SaveStructure> importCallback;

		// Token: 0x04003FC7 RID: 16327
		private Action cancelImportCallback;
	}
}
