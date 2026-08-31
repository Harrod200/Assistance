using System;
using ModelShark;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000808 RID: 2056
	public class SkirmishAddShipDropdownItem : MonoBehaviour
	{
		// Token: 0x06004A73 RID: 19059 RVA: 0x001F3A54 File Offset: 0x001F1C54
		public void SetTooltipDelegate(string tooltipString)
		{
			this.shipSummaryTip.SetDelegate("BodyText", () => tooltipString);
		}

		// Token: 0x04002B70 RID: 11120
		public TooltipTrigger shipSummaryTip;
	}
}
