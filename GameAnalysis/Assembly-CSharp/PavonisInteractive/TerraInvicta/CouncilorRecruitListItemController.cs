using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000835 RID: 2101
	public class CouncilorRecruitListItemController : MonoBehaviour
	{
		// Token: 0x06004C21 RID: 19489 RVA: 0x0020054A File Offset: 0x001FE74A
		public void Init(CouncilGridController controller)
		{
			this.controller = controller;
		}

		// Token: 0x06004C22 RID: 19490 RVA: 0x00200554 File Offset: 0x001FE754
		public void ItemSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.controller.selectedCandidate = this.councilor;
			this.controller.confirmRecruitBox.SetActive(false);
			this.controller.UpdateCandidateDetail(this.councilor);
			this.controller.SelectCandidateListItem(this);
		}

		// Token: 0x06004C23 RID: 19491 RVA: 0x002005AC File Offset: 0x001FE7AC
		public void UpdateListItem(TICouncilorState councilor, TIFactionState council)
		{
			this.councilor = councilor;
			this.council = council;
			this.candidateName.text = councilor.displayName;
			this.profession.text = councilor.typeTemplate.displayName;
			this.cost.text = councilor.GetRecruitCostString(council, true);
			this.persuasion.text = councilor.GetAttribute(CouncilorAttribute.Persuasion, false, true, true, false, false, false).ToString();
			this.investigation.text = councilor.GetAttribute(CouncilorAttribute.Investigation, false, true, true, false, false, false).ToString();
			this.espionage.text = councilor.GetAttribute(CouncilorAttribute.Espionage, false, true, true, false, false, false).ToString();
			this.command.text = councilor.GetAttribute(CouncilorAttribute.Command, false, true, true, false, false, false).ToString();
			this.administration.text = councilor.GetAttribute(CouncilorAttribute.Administration, false, true, true, false, false, false).ToString();
			this.science.text = councilor.GetAttribute(CouncilorAttribute.Science, false, true, true, false, false, false).ToString();
			this.security.text = councilor.GetAttribute(CouncilorAttribute.Security, false, true, true, false, false, false).ToString();
			this.loyalty.text = councilor.GetAttribute(CouncilorAttribute.ApparentLoyalty, false, true, true, false, false, false).ToString();
			this.persuasionTitle.SetText(Loc.T("UI.Global.PersuasionShort"));
			this.investigationTitle.SetText(Loc.T("UI.Global.InvestigationShort"));
			this.espionageTitle.SetText(Loc.T("UI.Global.EspionageShort"));
			this.commandTitle.SetText(Loc.T("UI.Global.CommandShort"));
			this.administrationTitle.SetText(Loc.T("UI.Global.AdministrationShort"));
			this.scienceTitle.SetText(Loc.T("UI.Global.ScienceShort"));
			this.securityTitle.SetText(Loc.T("UI.Global.SecurityShort"));
			this.loyaltyTitle.SetText(Loc.T("UI.Global.LoyaltyShort"));
			GameControl.assetLoader.LoadAssetForImageAssignment(councilor.portraitResource, this.portrait);
			this.nationalityFlag.sprite = councilor.homeNation.flag;
			this.jobTooltip.SetDelegate("BodyText", () => councilor.typeTemplate.description);
			this.newRibbon.enabled = council.newAvailableCouncilors.Contains(councilor);
		}

		// Token: 0x06004C24 RID: 19492 RVA: 0x00200861 File Offset: 0x001FEA61
		public void SetSelected(bool selected)
		{
			if (selected)
			{
				this.backgroundImage.sprite = this.selectedBackground;
				return;
			}
			this.backgroundImage.sprite = this.defaultBackground;
		}

		// Token: 0x04002DBB RID: 11707
		private CouncilGridController controller;

		// Token: 0x04002DBC RID: 11708
		public TMP_Text candidateName;

		// Token: 0x04002DBD RID: 11709
		public TMP_Text profession;

		// Token: 0x04002DBE RID: 11710
		public TMP_Text cost;

		// Token: 0x04002DBF RID: 11711
		public TMP_Text persuasion;

		// Token: 0x04002DC0 RID: 11712
		public TMP_Text investigation;

		// Token: 0x04002DC1 RID: 11713
		public TMP_Text espionage;

		// Token: 0x04002DC2 RID: 11714
		public TMP_Text command;

		// Token: 0x04002DC3 RID: 11715
		public TMP_Text administration;

		// Token: 0x04002DC4 RID: 11716
		public TMP_Text science;

		// Token: 0x04002DC5 RID: 11717
		public TMP_Text security;

		// Token: 0x04002DC6 RID: 11718
		public TMP_Text loyalty;

		// Token: 0x04002DC7 RID: 11719
		public TMP_Text persuasionTitle;

		// Token: 0x04002DC8 RID: 11720
		public TMP_Text investigationTitle;

		// Token: 0x04002DC9 RID: 11721
		public TMP_Text espionageTitle;

		// Token: 0x04002DCA RID: 11722
		public TMP_Text commandTitle;

		// Token: 0x04002DCB RID: 11723
		public TMP_Text administrationTitle;

		// Token: 0x04002DCC RID: 11724
		public TMP_Text scienceTitle;

		// Token: 0x04002DCD RID: 11725
		public TMP_Text securityTitle;

		// Token: 0x04002DCE RID: 11726
		public TMP_Text loyaltyTitle;

		// Token: 0x04002DCF RID: 11727
		public TooltipTrigger jobTooltip;

		// Token: 0x04002DD0 RID: 11728
		public Image portrait;

		// Token: 0x04002DD1 RID: 11729
		public Image nationalityFlag;

		// Token: 0x04002DD2 RID: 11730
		public TICouncilorState councilor;

		// Token: 0x04002DD3 RID: 11731
		public TIFactionState council;

		// Token: 0x04002DD4 RID: 11732
		public Image backgroundImage;

		// Token: 0x04002DD5 RID: 11733
		public Sprite defaultBackground;

		// Token: 0x04002DD6 RID: 11734
		public Sprite selectedBackground;

		// Token: 0x04002DD7 RID: 11735
		public Image newRibbon;
	}
}
