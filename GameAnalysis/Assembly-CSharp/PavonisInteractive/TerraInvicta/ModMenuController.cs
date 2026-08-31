using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LapinerTools.Steam.UI;
using LapinerTools.uMyGUI;
using Microsoft.CSharp.RuntimeBinder;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Modding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007DD RID: 2013
	public class ModMenuController : MonoBehaviour
	{
		// Token: 0x060048BC RID: 18620 RVA: 0x001DDFCB File Offset: 0x001DC1CB
		private void Start()
		{
			this.Initialize();
		}

		// Token: 0x060048BD RID: 18621 RVA: 0x001DDFD4 File Offset: 0x001DC1D4
		private void Initialize()
		{
			this.LoadLocalizedText();
			this.useModsToggle.isOn = TIPlayerProfileManager.GetValue("UseMods") == "True";
			this.init = true;
			if (!TIPlayerProfileManager.useMods || !SteamManager.Initialized)
			{
				return;
			}
			this.steamWorkshopBrowsePanel.SetActive(true);
			this.RefreshInstalledMods();
		}

		// Token: 0x060048BE RID: 18622 RVA: 0x001DE030 File Offset: 0x001DC230
		public void LoadLocalizedText()
		{
			this.moddingTitleText.SetText(Loc.T("UI.StartScreen.Mods"));
			this.tabWorkshopBrowseText.SetText(Loc.T("UI.StartScreen.Mods.WorkshopBrowse"));
			this.tabWorkshopUploadText.SetText(Loc.T("UI.StartScreen.Mods.UploadToSteamWorkshop"));
			this.modsRefreshText.SetText(Loc.T("UI.StartScreen.Mods.ManualRefresh"));
			this.WorkshopUploadTitleText.SetText(Loc.T("UI.StartScreen.Mods.UploadToSteamWorkshop"));
			this.modNameLabelText.SetText(Loc.T("UI.StartScreen.Mods.WorkshopUploadModName"));
			this.modDescLabelText.SetText(Loc.T("UI.StartScreen.Mods.WorkshopUploadModDesc"));
			this.modTagLabelText.SetText(Loc.T("UI.StartScreen.Mods.SelectTag"));
			this.modExplorerHereText.SetText(Loc.T("UI.StartScreen.Mods.WorkshopUploadModFolder"));
			this.modUploadText.SetText(Loc.T("UI.StartScreen.Mods.WorkshopUploadSubmit"));
			this.modUploadHelpText.SetText(Loc.T("UI.StartScreen.Mods.WorkshopUploadModHelp"));
			this.modUseModsText.SetText(Loc.T("UI.StartScreen.Mods.UseMods"));
			this.modUseModsDescriptionText.SetText(Loc.T("UI.StartScreen.Mods.UseModsDescription"));
			this.modUpdateButtonText.SetText(Loc.T("UI.StartScreen.Mods.UpdateMod"));
		}

		// Token: 0x060048BF RID: 18623 RVA: 0x001DE164 File Offset: 0x001DC364
		public void SetSteamWorkshopTabs()
		{
			this.mainModPanel.SetActive(true);
			this.RefreshInstalledMods();
			this.steamWorkshopUploadPanel.SetActive(false);
			this.steamWorkshopBrowsePanel.SetActive(false);
			this.tabWorkshopBrowseText.transform.parent.gameObject.SetActive(false);
			this.tabWorkshopUploadText.transform.parent.gameObject.SetActive(false);
			this.tabWorkshopBrowseText.transform.parent.gameObject.SetActive(SteamManager.Initialized);
			this.tabWorkshopUploadText.transform.parent.gameObject.SetActive(SteamManager.Initialized);
		}

		// Token: 0x060048C0 RID: 18624 RVA: 0x001DE20F File Offset: 0x001DC40F
		public void OnClickBackToMainPanel()
		{
			this.steamWorkshopUploadPanel.SetActive(false);
			this.steamWorkshopBrowsePanel.SetActive(false);
			this.mainModPanel.SetActive(true);
		}

		// Token: 0x060048C1 RID: 18625 RVA: 0x001DE235 File Offset: 0x001DC435
		public void OnToggleUseMods()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			TIPlayerProfileManager.useMods = this.useModsToggle.isOn;
			if (this.init)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
		}

		// Token: 0x060048C2 RID: 18626 RVA: 0x001DE260 File Offset: 0x001DC460
		public void OnClickTabSteamWorkshopUpload()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SwitchTabInTabbedPane", false, false);
			this.mainModPanel.SetActive(false);
			this.steamWorkshopUploadPanel.SetActive(true);
			this.steamWorkshopBrowsePanel.SetActive(false);
		}

		// Token: 0x060048C3 RID: 18627 RVA: 0x001DE294 File Offset: 0x001DC494
		public void OnClickTabSteamWorkshopBrowse()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SwitchTabInTabbedPane", false, false);
			this.modManager.GetEnabledModFiles();
			this.mainModPanel.SetActive(false);
			this.steamWorkshopUploadPanel.SetActive(false);
			if (this.pulledPublishedItems)
			{
				this.pulledPublishedItems = false;
				this.workshopBrowser.PulledPublishedItems();
			}
			this.workshopBrowser.LoadItems(1);
			this.steamWorkshopBrowsePanel.SetActive(true);
		}

		// Token: 0x060048C4 RID: 18628 RVA: 0x001DE303 File Offset: 0x001DC503
		public void OnClickUpdateOwnedItem()
		{
			this.pulledPublishedItems = true;
			this.ownedItemPopup.ShowOwnedModPopup();
		}

		// Token: 0x060048C5 RID: 18629 RVA: 0x001DE318 File Offset: 0x001DC518
		public void ShowModWarningDialog(string warningHeader, string warningDesc)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Decline", false, false);
			this.modWarningObject.SetActive(true);
			this.modWarningHeaderText.SetText(warningHeader);
			this.modWarningDescriptionText.SetText(warningDesc);
			this.modWarningConfirmText.SetText(Loc.T("UI.Councilor.Orgs.AcknowledgeButton"));
		}

		// Token: 0x060048C6 RID: 18630 RVA: 0x001DE36A File Offset: 0x001DC56A
		public void OnClickCloseModWarningDialog()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.modWarningObject.SetActive(false);
		}

		// Token: 0x060048C7 RID: 18631 RVA: 0x001DE384 File Offset: 0x001DC584
		public void OnClickRefreshInstalledMods()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.RefreshInstalledMods();
		}

		// Token: 0x060048C8 RID: 18632 RVA: 0x001DE398 File Offset: 0x001DC598
		public void RefreshInstalledMods()
		{
			ModManager component = base.GetComponent<ModManager>();
			component.enabled = true;
			List<string> enabledModFiles = component.GetEnabledModFiles();
			List<string> disabledModFiles = component.GetDisabledModFiles();
			int num = 0;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			string[] array = new string[] { "/ModInfo.json" };
			foreach (string text in enabledModFiles)
			{
				if (text.Contains("ModInfo.json"))
				{
					num++;
					list.Add(text);
				}
			}
			foreach (string text2 in disabledModFiles)
			{
				if (text2.Contains("ModInfo.json"))
				{
					num++;
					list2.Add(text2);
				}
			}
			this.modListManager.SetListSize<ModItemListItemController>(num, false, false);
			int num2 = 0;
			using (IEnumerator<object> enumerator2 = this.modListManager.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (ModMenuController.<>o__42.<>p__0 == null)
					{
						ModMenuController.<>o__42.<>p__0 = CallSite<Func<CallSite, object, ModItemListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ModItemListItemController), typeof(ModMenuController)));
					}
					ModItemListItemController modItemListItemController = ModMenuController.<>o__42.<>p__0.Target(ModMenuController.<>o__42.<>p__0, enumerator2.Current);
					if (num2 < list.Count)
					{
						modItemListItemController.Init(this);
						string text3 = list[num2].Split(array, StringSplitOptions.None)[0];
						modItemListItemController.modName.text = text3;
						bool flag = TIPlayerProfileManager.subscribedMods.ContainsKey(text3.Replace("Mods/Enabled/", ""));
						modItemListItemController.UpdateListItem(ModItemListItemController.ModStatus.Enabled, flag);
					}
					else
					{
						modItemListItemController.Init(this);
						string text4 = list2[num2 - list.Count].Split(array, StringSplitOptions.None)[0];
						modItemListItemController.modName.text = text4;
						bool flag2 = TIPlayerProfileManager.subscribedMods.ContainsKey(text4.Replace("Mods/Disabled/", ""));
						modItemListItemController.UpdateListItem(ModItemListItemController.ModStatus.Disabled, flag2);
					}
					num2++;
				}
			}
		}

		// Token: 0x060048C9 RID: 18633 RVA: 0x001DE5FC File Offset: 0x001DC7FC
		public void PlayCloseModMenuAudio()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
		}

		// Token: 0x040029DC RID: 10716
		public ModManager modManager;

		// Token: 0x040029DD RID: 10717
		public ListManagerBase modListManager;

		// Token: 0x040029DE RID: 10718
		public SteamWorkshopUpdateOwnedItemExampleStatic ownedItemPopup;

		// Token: 0x040029DF RID: 10719
		public SteamWorkshopUIBrowse workshopBrowser;

		// Token: 0x040029E0 RID: 10720
		private uMyGUI_Dropdown ownedModDropdown;

		// Token: 0x040029E1 RID: 10721
		public GameObject mainModPanel;

		// Token: 0x040029E2 RID: 10722
		public GameObject steamWorkshopUploadPanel;

		// Token: 0x040029E3 RID: 10723
		public GameObject steamWorkshopBrowsePanel;

		// Token: 0x040029E4 RID: 10724
		public GameObject modWarningObject;

		// Token: 0x040029E5 RID: 10725
		public Toggle useModsToggle;

		// Token: 0x040029E6 RID: 10726
		public TMP_Text moddingTitleText;

		// Token: 0x040029E7 RID: 10727
		public TMP_Text tabWorkshopBrowseText;

		// Token: 0x040029E8 RID: 10728
		public TMP_Text tabWorkshopUploadText;

		// Token: 0x040029E9 RID: 10729
		public TMP_Text modsRefreshText;

		// Token: 0x040029EA RID: 10730
		public TMP_Text WorkshopUploadTitleText;

		// Token: 0x040029EB RID: 10731
		public TMP_Text modNameLabelText;

		// Token: 0x040029EC RID: 10732
		public TMP_Text modDescLabelText;

		// Token: 0x040029ED RID: 10733
		public TMP_Text modTagLabelText;

		// Token: 0x040029EE RID: 10734
		public TMP_Text modExplorerHereText;

		// Token: 0x040029EF RID: 10735
		public TMP_Text modUploadText;

		// Token: 0x040029F0 RID: 10736
		public TMP_Text modUploadHelpText;

		// Token: 0x040029F1 RID: 10737
		public TMP_Text modUseModsText;

		// Token: 0x040029F2 RID: 10738
		public TMP_Text modUseModsDescriptionText;

		// Token: 0x040029F3 RID: 10739
		public TMP_Text modWarningHeaderText;

		// Token: 0x040029F4 RID: 10740
		public TMP_Text modWarningDescriptionText;

		// Token: 0x040029F5 RID: 10741
		public TMP_Text modWarningConfirmText;

		// Token: 0x040029F6 RID: 10742
		public TMP_Text modUpdateButtonText;

		// Token: 0x040029F7 RID: 10743
		public TMP_Text modSearchPlaceholderText;

		// Token: 0x040029F8 RID: 10744
		private bool init;

		// Token: 0x040029F9 RID: 10745
		private bool pulledPublishedItems;
	}
}
