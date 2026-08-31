using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008B4 RID: 2228
	public class PropellantListItemController : MonoBehaviour
	{
		// Token: 0x060054F3 RID: 21747 RVA: 0x00269A60 File Offset: 0x00267C60
		public void SetListItem(PropellantGroup propellantGroup, OperationCanvasController controller, int idx)
		{
			this.idx = idx;
			this.controller = controller;
			if (propellantGroup.ships.Count == 1)
			{
				this.propellantName.SetText(Loc.T("UI.Operations.PropellantGroupNameSingle", new object[]
				{
					propellantGroup.ToString(),
					propellantGroup.ships.Count.ToString("N0")
				}));
			}
			else
			{
				this.propellantName.SetText(Loc.T("UI.Operations.PropellantGroupName", new object[]
				{
					propellantGroup.ToString(),
					propellantGroup.ships.Count.ToString("N0")
				}));
			}
			this.defaultSprite = this.buttonImage.sprite;
		}

		// Token: 0x060054F4 RID: 21748 RVA: 0x00269B28 File Offset: 0x00267D28
		public void OnPropellantButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.controller.OnPropellantSelected(this.idx, false);
		}

		// Token: 0x060054F5 RID: 21749 RVA: 0x00269B48 File Offset: 0x00267D48
		public void SetButtonHighlight(bool highlight)
		{
			this.buttonImage.sprite = (highlight ? this.propellantButton.spriteState.pressedSprite : this.defaultSprite);
		}

		// Token: 0x04003B21 RID: 15137
		public TMP_Text propellantName;

		// Token: 0x04003B22 RID: 15138
		private OperationCanvasController controller;

		// Token: 0x04003B23 RID: 15139
		public Button propellantButton;

		// Token: 0x04003B24 RID: 15140
		public Image buttonImage;

		// Token: 0x04003B25 RID: 15141
		[HideInInspector]
		public int idx;

		// Token: 0x04003B26 RID: 15142
		private Sprite defaultSprite;
	}
}
