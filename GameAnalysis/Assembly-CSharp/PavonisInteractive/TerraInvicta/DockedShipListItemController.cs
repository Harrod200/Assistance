using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200084C RID: 2124
	public class DockedShipListItemController : MonoBehaviour
	{
		// Token: 0x06004D12 RID: 19730 RVA: 0x0020C2C3 File Offset: 0x0020A4C3
		public void Init(FleetsScreenController controller, TISpaceShipTemplate design, TISpaceShipState shipState, string shipDisplayName)
		{
			this.controller = controller;
			this.design = design;
			this.shipState = shipState;
			this.shipDisplayName = shipDisplayName;
		}

		// Token: 0x06004D13 RID: 19731 RVA: 0x0020C2E4 File Offset: 0x0020A4E4
		public void UpdateListItem()
		{
			this.shipName.SetText(this.shipDisplayName);
			this.shipClassName.SetText(this.design.fullClassName);
			this.role.SetText(this.design.roleStr);
			if (this.shipState != null)
			{
				this.location.gameObject.SetActive(true);
				this.location.SetText(TIUtilities.GetLocationString(this.shipState.fleet.location, false, false));
				return;
			}
			this.location.gameObject.SetActive(false);
		}

		// Token: 0x06004D14 RID: 19732 RVA: 0x0020C381 File Offset: 0x0020A581
		public void OnListItemClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.controller.designToRefitTo = null;
			this.controller.SetSelectedShipClassFromClassList(this.design, true, this.shipState, TIInputManager.IsShiftKeyDown);
		}

		// Token: 0x06004D15 RID: 19733 RVA: 0x0020C3B8 File Offset: 0x0020A5B8
		public void HighlightButtonAfterSelection(TISpaceShipState shipToRefit)
		{
			this.idleButtonImage.sprite = ((this.shipState == shipToRefit) ? this.button.spriteState.pressedSprite : this.defaultButtonSprite);
		}

		// Token: 0x06004D16 RID: 19734 RVA: 0x0020C3FC File Offset: 0x0020A5FC
		public void HighlightButtonAfterSelection(List<TISpaceShipState> multiSelectedShips)
		{
			this.idleButtonImage.sprite = (multiSelectedShips.Contains(this.shipState) ? this.button.spriteState.pressedSprite : this.defaultButtonSprite);
		}

		// Token: 0x06004D17 RID: 19735 RVA: 0x0020C43D File Offset: 0x0020A63D
		public void DeSelectButton()
		{
			this.idleButtonImage.sprite = this.defaultButtonSprite;
		}

		// Token: 0x04002FAE RID: 12206
		private FleetsScreenController controller;

		// Token: 0x04002FAF RID: 12207
		public TMP_Text shipName;

		// Token: 0x04002FB0 RID: 12208
		public TMP_Text shipClassName;

		// Token: 0x04002FB1 RID: 12209
		public TMP_Text role;

		// Token: 0x04002FB2 RID: 12210
		public TMP_Text location;

		// Token: 0x04002FB3 RID: 12211
		private TISpaceShipTemplate design;

		// Token: 0x04002FB4 RID: 12212
		public TISpaceShipState shipState;

		// Token: 0x04002FB5 RID: 12213
		public Sprite defaultButtonSprite;

		// Token: 0x04002FB6 RID: 12214
		private string shipDisplayName;

		// Token: 0x04002FB7 RID: 12215
		public Image idleButtonImage;

		// Token: 0x04002FB8 RID: 12216
		public Button button;
	}
}
