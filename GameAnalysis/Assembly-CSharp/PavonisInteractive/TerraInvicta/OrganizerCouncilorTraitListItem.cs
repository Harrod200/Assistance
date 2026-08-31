using System;
using ModelShark;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200083E RID: 2110
	public class OrganizerCouncilorTraitListItem : MonoBehaviour
	{
		// Token: 0x06004C67 RID: 19559 RVA: 0x002045D4 File Offset: 0x002027D4
		public void SetListItem(TITraitTemplate traitTemplate)
		{
			this.traitName.SetText(traitTemplate.displayName);
			this.traitTooltip.SetDelegate("BodyText", () => traitTemplate.fullTraitSummary);
		}

		// Token: 0x04002E56 RID: 11862
		public TMP_Text traitName;

		// Token: 0x04002E57 RID: 11863
		public TooltipTrigger traitTooltip;
	}
}
