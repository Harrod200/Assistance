using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008C4 RID: 2244
	public class ProjectsButtonListItemController : MonoBehaviour
	{
		// Token: 0x060055A7 RID: 21927 RVA: 0x0026ED3E File Offset: 0x0026CF3E
		public void Init(ResearchScreenController controller)
		{
			this.controller = controller;
			this.obsoleteTooltip.SetText("BodyText", Loc.T("UI.Science.ObsoleteTooltip"));
			this.favoriteTooltip.SetText("BodyText", Loc.T("UI.Science.FavoriteTooltip"));
		}

		// Token: 0x060055A8 RID: 21928 RVA: 0x0026ED7C File Offset: 0x0026CF7C
		public void UpdateListItem(TIProjectTemplate template, TIFactionState faction)
		{
			float projectProgressValueByTemplate = faction.GetProjectProgressValueByTemplate(template);
			this.SelectProjectButtonText.SetText(Loc.T("UI.Science.SelectProjectListText", new object[]
			{
				template.displayName,
				projectProgressValueByTemplate.ToString("N0"),
				template.GetResearchCost(faction).ToString("N0"),
				TemplateManager.global.researchInlineSpritePath
			}));
			GameControl.assetLoader.LoadAssetForImageAssignment(template.GetCategoryIconPath(), this.SelectProjectButtonImage);
			this.heldDataName = template.dataName;
			this.projectTemplate = template;
			this.obsoleteToggle.gameObject.SetActive(template.FulfillsObjective(faction, true) == null);
			this.categorySortWeight = (float)(this.projectTemplate.techCategory * (TechCategory)1000);
			this.categorySortWeight -= this.projectTemplate.GetResearchCost(this.controller.activePlayer) / 1000f;
		}

		// Token: 0x060055A9 RID: 21929 RVA: 0x0026EE70 File Offset: 0x0026D070
		public void UpdateToggles(TIProjectTemplate template, TIFactionState faction)
		{
			if (faction.hiddenProjects.Contains(template.dataName))
			{
				this.obsoleteToggle.SetIsOnWithoutNotify(true);
				this.obsoleteIcon.sprite = this.obsolete_on;
			}
			else
			{
				this.obsoleteToggle.SetIsOnWithoutNotify(false);
				this.obsoleteIcon.sprite = this.obsolete_off;
			}
			if (faction.favoredProjects.Contains(template.dataName))
			{
				this.favoriteToggle.SetIsOnWithoutNotify(true);
				this.favoriteIcon.sprite = this.favorite_on;
				return;
			}
			this.favoriteToggle.SetIsOnWithoutNotify(false);
			this.favoriteIcon.sprite = this.favorite_off;
		}

		// Token: 0x060055AA RID: 21930 RVA: 0x0026EF1A File Offset: 0x0026D11A
		public void OnLineClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.controller.SetSelectedProjectEntry(this.heldDataName);
		}

		// Token: 0x060055AB RID: 21931 RVA: 0x0026EF3C File Offset: 0x0026D13C
		public void OnObsoleteToggle()
		{
			if (this.obsoleteToggle.isOn)
			{
				this.obsoleteIcon.sprite = this.obsolete_on;
			}
			else
			{
				this.obsoleteIcon.sprite = this.obsolete_off;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.controller.OnProjectObsoleteToggle(this.obsoleteToggle.isOn, this.heldDataName);
		}

		// Token: 0x060055AC RID: 21932 RVA: 0x0026EFA4 File Offset: 0x0026D1A4
		public void OnFavoriteToggle()
		{
			if (this.favoriteToggle.isOn)
			{
				this.favoriteIcon.sprite = this.favorite_on;
			}
			else
			{
				this.favoriteIcon.sprite = this.favorite_off;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.controller.OnProjectFavoriteToggle(this.favoriteToggle.isOn, this.heldDataName);
		}

		// Token: 0x060055AD RID: 21933 RVA: 0x0026F00A File Offset: 0x0026D20A
		public void SetSelected(bool selected)
		{
			this.backgroundImage.sprite = (selected ? this.selectedBackground : this.defaultBackground);
		}

		// Token: 0x04003C1A RID: 15386
		private ResearchScreenController controller;

		// Token: 0x04003C1B RID: 15387
		public Button SelectProjectButton;

		// Token: 0x04003C1C RID: 15388
		public Image SelectProjectButtonImage;

		// Token: 0x04003C1D RID: 15389
		public TMP_Text SelectProjectButtonText;

		// Token: 0x04003C1E RID: 15390
		public TIProjectTemplate projectTemplate;

		// Token: 0x04003C1F RID: 15391
		public float categorySortWeight;

		// Token: 0x04003C20 RID: 15392
		public Toggle obsoleteToggle;

		// Token: 0x04003C21 RID: 15393
		public Toggle favoriteToggle;

		// Token: 0x04003C22 RID: 15394
		public Image obsoleteIcon;

		// Token: 0x04003C23 RID: 15395
		public Image favoriteIcon;

		// Token: 0x04003C24 RID: 15396
		[Header("Tooltips")]
		public TooltipTrigger obsoleteTooltip;

		// Token: 0x04003C25 RID: 15397
		public TooltipTrigger favoriteTooltip;

		// Token: 0x04003C26 RID: 15398
		[Header("Sprites")]
		public Sprite favorite_on;

		// Token: 0x04003C27 RID: 15399
		public Sprite favorite_off;

		// Token: 0x04003C28 RID: 15400
		public Sprite obsolete_on;

		// Token: 0x04003C29 RID: 15401
		public Sprite obsolete_off;

		// Token: 0x04003C2A RID: 15402
		[Header("Selection")]
		public Image backgroundImage;

		// Token: 0x04003C2B RID: 15403
		public Sprite defaultBackground;

		// Token: 0x04003C2C RID: 15404
		public Sprite selectedBackground;

		// Token: 0x04003C2D RID: 15405
		[HideInInspector]
		public string heldDataName;
	}
}
