using System;
using System.Text;
using ModelShark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000845 RID: 2117
	public class OrgListItemController : MonoBehaviour
	{
		// Token: 0x06004CF8 RID: 19704 RVA: 0x0020B3CC File Offset: 0x002095CC
		public void UpdateListItem(TIOrgState org)
		{
			StringBuilder stringBuilder = new StringBuilder(org.displayName).Append(org.tierStarsInline);
			if (org.grantsMarked)
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.warningInlineSpritePath);
			}
			this.orgName.SetText(stringBuilder.ToString());
			this.orgIcon.sprite = org.icon;
			this.orgDescription.SetDelegate("BodyText", () => org.description(true, GameControl.control.activePlayer, false, false));
			this.orgDescription.enabled = true;
		}

		// Token: 0x04002F43 RID: 12099
		public Image orgIcon;

		// Token: 0x04002F44 RID: 12100
		public TMP_Text orgName;

		// Token: 0x04002F45 RID: 12101
		public TooltipTrigger orgDescription;
	}
}
