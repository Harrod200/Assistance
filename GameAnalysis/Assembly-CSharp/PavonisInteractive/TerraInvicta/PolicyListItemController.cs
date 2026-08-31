using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008A9 RID: 2217
	public class PolicyListItemController : MonoBehaviour
	{
		// Token: 0x0600540F RID: 21519 RVA: 0x002605B0 File Offset: 0x0025E7B0
		public void SetListItem(NotificationScreenController controller, TIPolicyOption policyOption, TINationState nationState)
		{
			this.controller = controller;
			this.policyOption = policyOption;
			this.policyName.SetText(policyOption.GetDisplayName());
			string text = policyOption.GetDescription();
			if (policyOption.GetPolicyType() == PolicyType.TransferRegionsOption && policyOption.GetPossibleTargets(nationState).Contains(GameStateManager.AlienNation()))
			{
				text = new StringBuilder(text).Append(Loc.T("TransferRegionsOption.specialDescription")).ToString();
			}
			this.policyDescription.SetText(text);
			this.policySelectionButton.interactable = policyOption.Allowed(nationState);
		}

		// Token: 0x06005410 RID: 21520 RVA: 0x00260639 File Offset: 0x0025E839
		public void OnClicked()
		{
			this.controller.confirmPanelObject.SetActive(false);
			this.controller.PolicySelected(this.policyOption);
		}

		// Token: 0x04003A4D RID: 14925
		private NotificationScreenController controller;

		// Token: 0x04003A4E RID: 14926
		public TMP_Text policyName;

		// Token: 0x04003A4F RID: 14927
		public TMP_Text policyDescription;

		// Token: 0x04003A50 RID: 14928
		public Button policySelectionButton;

		// Token: 0x04003A51 RID: 14929
		private TIPolicyOption policyOption;
	}
}
