using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200080A RID: 2058
	public class SkirmishFactionToggleItem : MonoBehaviour
	{
		// Token: 0x06004A77 RID: 19063 RVA: 0x001F3AAF File Offset: 0x001F1CAF
		private void OnEnable()
		{
			base.StartCoroutine(this.ValidateToggle());
		}

		// Token: 0x06004A78 RID: 19064 RVA: 0x001F3ABE File Offset: 0x001F1CBE
		private IEnumerator ValidateToggle()
		{
			yield return null;
			if ((this.factionIndex == 0 && this.textLabel.text == this.controller.skirmishFactionDropdown[1].options[this.controller.skirmishFactionDropdown[1].value].text) || (this.factionIndex == 1 && this.textLabel.text == this.controller.skirmishFactionDropdown[0].options[this.controller.skirmishFactionDropdown[0].value].text))
			{
				this.toggleItem.interactable = false;
				this.textLabel.color = TIUtilities.UIDisabled;
			}
			else
			{
				this.toggleItem.interactable = true;
				this.textLabel.color = TIUtilities.UITextColor;
			}
			yield break;
		}

		// Token: 0x04002B72 RID: 11122
		public Toggle toggleItem;

		// Token: 0x04002B73 RID: 11123
		public TMP_Text textLabel;

		// Token: 0x04002B74 RID: 11124
		public StartMenuController controller;

		// Token: 0x04002B75 RID: 11125
		public int factionIndex;
	}
}
