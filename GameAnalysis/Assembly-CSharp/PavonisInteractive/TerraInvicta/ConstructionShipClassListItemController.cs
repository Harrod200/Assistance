using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200084B RID: 2123
	public class ConstructionShipClassListItemController : MonoBehaviour
	{
		// Token: 0x06004D0C RID: 19724 RVA: 0x0020C1AF File Offset: 0x0020A3AF
		public void Init(FleetsScreenController controller, TISpaceShipTemplate design)
		{
			this.controller = controller;
			this.design = design;
			if (this.defaultButtonSprite == null)
			{
				this.defaultButtonSprite = this.button.image.sprite;
			}
		}

		// Token: 0x06004D0D RID: 19725 RVA: 0x0020C1E4 File Offset: 0x0020A3E4
		public void UpdateListItem()
		{
			this.shipClassName.SetText(this.design.fullClassName);
			this.role.SetText(this.design.roleStr);
			this.constructionCost.SetText(this.design.spaceResourceConstructionCost(false, null, true, false, false).ToString("Relevant", false, false, null, false, FactionResource.None));
		}

		// Token: 0x06004D0E RID: 19726 RVA: 0x0020C247 File Offset: 0x0020A447
		public void OnListItemClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.controller.SetSelectedShipClassFromClassList(this.design, false, null, false);
		}

		// Token: 0x06004D0F RID: 19727 RVA: 0x0020C26C File Offset: 0x0020A46C
		public void HighlightButtonAfterSelection(TISpaceShipTemplate selectedDesign)
		{
			this.idleButtonImage.sprite = ((this.design == selectedDesign) ? this.button.spriteState.pressedSprite : this.defaultButtonSprite);
		}

		// Token: 0x06004D10 RID: 19728 RVA: 0x0020C2A8 File Offset: 0x0020A4A8
		public void DeSelectButton()
		{
			this.idleButtonImage.sprite = this.defaultButtonSprite;
		}

		// Token: 0x04002FA6 RID: 12198
		private FleetsScreenController controller;

		// Token: 0x04002FA7 RID: 12199
		public TMP_Text shipClassName;

		// Token: 0x04002FA8 RID: 12200
		public TMP_Text role;

		// Token: 0x04002FA9 RID: 12201
		public TMP_Text constructionCost;

		// Token: 0x04002FAA RID: 12202
		private TISpaceShipTemplate design;

		// Token: 0x04002FAB RID: 12203
		private Sprite defaultButtonSprite;

		// Token: 0x04002FAC RID: 12204
		public Image idleButtonImage;

		// Token: 0x04002FAD RID: 12205
		public Button button;
	}
}
