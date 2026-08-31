using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000853 RID: 2131
	public class RefitClassListItemController : MonoBehaviour
	{
		// Token: 0x06004E46 RID: 20038 RVA: 0x0021AF24 File Offset: 0x00219124
		public void Init(FleetsScreenController controller, TISpaceShipTemplate design, TISpaceShipTemplate newDesign, TISpaceShipState shipToRefit)
		{
			this.controller = controller;
			this.design = design;
			this.oldDesign = newDesign;
			this.shipToRefit = shipToRefit;
			if (this.defaultButtonSprite == null)
			{
				this.defaultButtonSprite = this.button.image.sprite;
			}
		}

		// Token: 0x06004E47 RID: 20039 RVA: 0x0021AF74 File Offset: 0x00219174
		public void UpdateListItem()
		{
			base.gameObject.SetActive(true);
			this.shipClassName.SetText(this.design.fullClassName);
			this.role.SetText(this.design.roleStr);
			this.constructionCost.SetText(this.design.RefitResourceCost(null, this.oldDesign, true, true, this.shipToRefit).ToString("Relevant", false, false, null, false, FactionResource.None));
		}

		// Token: 0x06004E48 RID: 20040 RVA: 0x0021AFF0 File Offset: 0x002191F0
		public void OnListItemClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			if (this.controller != null)
			{
				this.controller.designToRefitTo = this.design;
				this.controller.originalShipTemplate = this.oldDesign;
				this.controller.SetSelectedShipClassFromClassList(this.design, false, null, false);
			}
		}

		// Token: 0x06004E49 RID: 20041 RVA: 0x0021B050 File Offset: 0x00219250
		public void HighlightButtonAfterSelection(TISpaceShipTemplate selectedDesign)
		{
			this.idleButtonImage.sprite = ((this.design == selectedDesign) ? this.button.spriteState.pressedSprite : this.defaultButtonSprite);
		}

		// Token: 0x06004E4A RID: 20042 RVA: 0x0021B08C File Offset: 0x0021928C
		public void DeSelectButton()
		{
			this.idleButtonImage.sprite = this.defaultButtonSprite;
		}

		// Token: 0x040031C6 RID: 12742
		private FleetsScreenController controller;

		// Token: 0x040031C7 RID: 12743
		public TMP_Text shipName;

		// Token: 0x040031C8 RID: 12744
		public TMP_Text shipClassName;

		// Token: 0x040031C9 RID: 12745
		public TMP_Text role;

		// Token: 0x040031CA RID: 12746
		public TMP_Text constructionCost;

		// Token: 0x040031CB RID: 12747
		private TISpaceShipTemplate design;

		// Token: 0x040031CC RID: 12748
		private TISpaceShipTemplate oldDesign;

		// Token: 0x040031CD RID: 12749
		private Sprite defaultButtonSprite;

		// Token: 0x040031CE RID: 12750
		public Image idleButtonImage;

		// Token: 0x040031CF RID: 12751
		public Button button;

		// Token: 0x040031D0 RID: 12752
		public TISpaceShipState shipToRefit;
	}
}
