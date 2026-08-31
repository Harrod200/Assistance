using System;
using ModelShark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000841 RID: 2113
	public class TraitsListItemController : MonoBehaviour
	{
		// Token: 0x06004C70 RID: 19568 RVA: 0x00204764 File Offset: 0x00202964
		public void UpdateListItem(TITraitTemplate traitTemplate, int k, bool isLast = false)
		{
			this.traitName.SetText(traitTemplate.displayName);
			this.traitSummary.SetDelegate("BodyText", () => traitTemplate.fullTraitSummary);
			if (this.dividingLine != null)
			{
				this.dividingLine.enabled = !isLast;
			}
		}

		// Token: 0x04002E62 RID: 11874
		public TMP_Text traitName;

		// Token: 0x04002E63 RID: 11875
		public TooltipTrigger traitSummary;

		// Token: 0x04002E64 RID: 11876
		public Image dividingLine;
	}
}
