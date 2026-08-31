using System;
using System.Linq;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000868 RID: 2152
	internal class HabDesignListItemController : MonoBehaviour
	{
		// Token: 0x06004FBE RID: 20414 RVA: 0x00226FDC File Offset: 0x002251DC
		public void SetListItem(TIHabTemplate template, HabitatsScreenController controller, int listIndex)
		{
			this.controller = controller;
			this.habTemplate = template;
			this.applyTemplateButton.interactable = this.controller.habToDisplay.CanApplySavedTemplate(this.habTemplate);
			this.applyTemplateButtonText.SetText(Loc.T("UI.Habs.CopyHabButton"));
			this.designName.SetText(template.displayName);
			this.backgroundImage.enabled = listIndex % 2 == 0;
			GameControl.assetLoader.LoadAssetForImageAssignment((template.habType == HabType.Base) ? TemplateManager.global.pathGeoscapeBase_gui : TemplateManager.global.pathGeoscapeStation_gui, this.icon);
			this.spaceObjectIcon.sprite = template.naturalSpaceObject.icon;
			this.benefitsList.SetText(template.simpleBenefitsString);
			this.allModulesList.SetText("BodyText", TIUtilities.ConstructTextList((from x in template.AllModuleTemplates(false)
				select x.displayName).ToList<string>(), true, false));
			int num = 0;
			for (int i = 0; i < template.sectors.Length; i++)
			{
				for (int j = 0; j < template.sectors[i].habModules.Length; j++)
				{
					if (template.sectors[i].habModules[j] != null)
					{
						GameControl.assetLoader.LoadAssetForImageAssignment(template.sectors[i].habModules[j].iconResource(template.habType), this.moduleIcons[num]);
						this.moduleIcons[num++].enabled = true;
					}
					else
					{
						this.moduleIcons[num++].enabled = false;
					}
				}
			}
		}

		// Token: 0x06004FBF RID: 20415 RVA: 0x00227193 File Offset: 0x00225393
		public void OnClickRename()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			this.ShowRenameTemplatePanel();
			if (this.habTemplate != null)
			{
				this.renameInputField.text = this.habTemplate.displayName;
			}
		}

		// Token: 0x06004FC0 RID: 20416 RVA: 0x002271C5 File Offset: 0x002253C5
		public void OnClickRevertRename()
		{
			this.renameTemplatePanel.SetActive(false);
			this.renameInputField.text = "";
		}

		// Token: 0x06004FC1 RID: 20417 RVA: 0x002271E4 File Offset: 0x002253E4
		public void OnClickSaveName()
		{
			if (this.habTemplate == null)
			{
				return;
			}
			this.renameTemplatePanel.SetActive(false);
			GameControl.control.activePlayer.playerControl.StartAction(new RenameHabDesignAction(this.habTemplate, this.renameInputField.text));
			this.designName.SetText(this.habTemplate.displayName);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.controller.RefreshHabTemplateManagerList();
		}

		// Token: 0x06004FC2 RID: 20418 RVA: 0x0022725D File Offset: 0x0022545D
		public void ShowRenameTemplatePanel()
		{
			this.renameTemplatePanel.SetActive(true);
			this.renameInputField.Select();
		}

		// Token: 0x06004FC3 RID: 20419 RVA: 0x00227278 File Offset: 0x00225478
		public void OnDeleteClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			GameControl.control.activePlayer.playerControl.StartAction(new DeleteHabDesignAction(GameControl.control.activePlayer, this.habTemplate));
			this.controller.RefreshHabTemplateManagerList();
		}

		// Token: 0x06004FC4 RID: 20420 RVA: 0x002272C5 File Offset: 0x002254C5
		public void OnClickApplyTemplate()
		{
			if (this.habTemplate == null)
			{
				return;
			}
			this.controller.OnCopyHabButtonPressed();
			this.controller.SetSelectedTemplateInDropdown(this.habTemplate);
		}

		// Token: 0x04003321 RID: 13089
		public Button applyTemplateButton;

		// Token: 0x04003322 RID: 13090
		public TMP_Text applyTemplateButtonText;

		// Token: 0x04003323 RID: 13091
		public Image icon;

		// Token: 0x04003324 RID: 13092
		public Image spaceObjectIcon;

		// Token: 0x04003325 RID: 13093
		public TMP_Text designName;

		// Token: 0x04003326 RID: 13094
		private TIHabTemplate habTemplate;

		// Token: 0x04003327 RID: 13095
		public TooltipTrigger allModulesList;

		// Token: 0x04003328 RID: 13096
		private HabitatsScreenController controller;

		// Token: 0x04003329 RID: 13097
		public TMP_Text benefitsList;

		// Token: 0x0400332A RID: 13098
		public Image[] moduleIcons;

		// Token: 0x0400332B RID: 13099
		public GameObject renameTemplatePanel;

		// Token: 0x0400332C RID: 13100
		public TMP_InputField renameInputField;

		// Token: 0x0400332D RID: 13101
		public Image backgroundImage;
	}
}
