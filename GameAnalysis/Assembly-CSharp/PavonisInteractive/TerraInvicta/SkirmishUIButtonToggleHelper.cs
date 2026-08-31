using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200080E RID: 2062
	public class SkirmishUIButtonToggleHelper : MonoBehaviour
	{
		// Token: 0x06004A91 RID: 19089 RVA: 0x001F4434 File Offset: 0x001F2634
		public void Toggle()
		{
			if (this.targetToggle != null)
			{
				this.dropdown.SetValueWithoutNotify(base.transform.parent.GetSiblingIndex() - 1);
				this.dropdown.RefreshShownValue();
				this.listItemController.OnShipDropdownChanged();
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyFleetSelect", false, false);
			}
		}

		// Token: 0x04002B89 RID: 11145
		[SerializeField]
		private Toggle targetToggle;

		// Token: 0x04002B8A RID: 11146
		[SerializeField]
		private TMP_Dropdown dropdown;

		// Token: 0x04002B8B RID: 11147
		[SerializeField]
		private SkirmishShipListItemController listItemController;
	}
}
