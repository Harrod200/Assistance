using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008C9 RID: 2249
	public class TechsButtonListItemController : MonoBehaviour
	{
		// Token: 0x06005650 RID: 22096 RVA: 0x00277F53 File Offset: 0x00276153
		public void Init(ResearchScreenController controller)
		{
			this.controller = controller;
		}

		// Token: 0x06005651 RID: 22097 RVA: 0x00277F5C File Offset: 0x0027615C
		public void UpdateListItem(TITechTemplate template)
		{
			this.SelectTechButtonText.SetText(Loc.T("UI.Science.SelectTechListText", new object[]
			{
				template.displayName,
				template.GetResearchCost(GameControl.control.activePlayer).ToString("N0"),
				TemplateManager.global.researchInlineSpritePath
			}));
			GameControl.assetLoader.LoadAssetForImageAssignment(template.GetCategoryIconPath(), this.SelectTechButtonImage);
			this.heldDataName = template.dataName;
			this.techTemplate = template;
			this.categorySortWeight = (float)(this.techTemplate.techCategory * (TechCategory)1000);
			this.categorySortWeight -= this.techTemplate.GetResearchCost(this.controller.activePlayer) / 1000f;
		}

		// Token: 0x06005652 RID: 22098 RVA: 0x00278024 File Offset: 0x00276224
		public void OnLineClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.controller.SetSelectedTechEntry(this.heldDataName);
		}

		// Token: 0x06005653 RID: 22099 RVA: 0x00278043 File Offset: 0x00276243
		public void SetSelected(bool selected)
		{
			this.backgroundImage.sprite = (selected ? this.selectedBackground : this.defaultBackground);
		}

		// Token: 0x04003D59 RID: 15705
		private ResearchScreenController controller;

		// Token: 0x04003D5A RID: 15706
		public Button SelectTechButton;

		// Token: 0x04003D5B RID: 15707
		public TMP_Text SelectTechButtonText;

		// Token: 0x04003D5C RID: 15708
		public Image SelectTechButtonImage;

		// Token: 0x04003D5D RID: 15709
		public TITechTemplate techTemplate;

		// Token: 0x04003D5E RID: 15710
		public float categorySortWeight;

		// Token: 0x04003D5F RID: 15711
		[HideInInspector]
		public string heldDataName;

		// Token: 0x04003D60 RID: 15712
		[Header("Selection")]
		public Image backgroundImage;

		// Token: 0x04003D61 RID: 15713
		public Sprite defaultBackground;

		// Token: 0x04003D62 RID: 15714
		public Sprite selectedBackground;
	}
}
