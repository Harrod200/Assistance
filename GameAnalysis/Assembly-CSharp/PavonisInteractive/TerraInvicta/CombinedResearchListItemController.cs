using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008C0 RID: 2240
	public class CombinedResearchListItemController : MonoBehaviour
	{
		// Token: 0x06005598 RID: 21912 RVA: 0x0026EA08 File Offset: 0x0026CC08
		public void Init(ResearchScreenController controller)
		{
			this.controller = controller;
		}

		// Token: 0x06005599 RID: 21913 RVA: 0x0026EA11 File Offset: 0x0026CC11
		public void UpdateTechListItem(TITechTemplate template)
		{
			this.selectTechButtonText.text = template.displayName;
			GameControl.assetLoader.LoadAssetForImageAssignment(template.GetCategoryIconPath(), this.techIcon);
			this.heldDataName = template.dataName;
		}

		// Token: 0x0600559A RID: 21914 RVA: 0x0026EA48 File Offset: 0x0026CC48
		public void UpdateProjectListItem(TIProjectTemplate template)
		{
			this.selectTechButtonText.text = TemplateManager.global.projectsInlineSpritePath + template.displayName;
			GameControl.assetLoader.LoadAssetForImageAssignment(template.GetCategoryIconPath(), this.techIcon);
			this.heldDataName = template.dataName;
		}

		// Token: 0x0600559B RID: 21915 RVA: 0x0026EA97 File Offset: 0x0026CC97
		public void OnLineClicked()
		{
			this.controller.SetSelectedArchiveEntry(this.heldDataName);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
		}

		// Token: 0x0600559C RID: 21916 RVA: 0x0026EAB6 File Offset: 0x0026CCB6
		public void SetSelected(bool selected)
		{
			this.backgroundImage.sprite = (selected ? this.selectedBackground : this.defaultBackground);
		}

		// Token: 0x04003C02 RID: 15362
		private ResearchScreenController controller;

		// Token: 0x04003C03 RID: 15363
		public Button selectTechButton;

		// Token: 0x04003C04 RID: 15364
		public TMP_Text selectTechButtonText;

		// Token: 0x04003C05 RID: 15365
		public Image techIcon;

		// Token: 0x04003C06 RID: 15366
		[HideInInspector]
		public string heldDataName;

		// Token: 0x04003C07 RID: 15367
		public Image backgroundImage;

		// Token: 0x04003C08 RID: 15368
		public Sprite defaultBackground;

		// Token: 0x04003C09 RID: 15369
		public Sprite selectedBackground;
	}
}
