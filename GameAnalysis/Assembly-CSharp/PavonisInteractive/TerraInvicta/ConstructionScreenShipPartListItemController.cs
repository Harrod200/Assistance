using System;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200084A RID: 2122
	public class ConstructionScreenShipPartListItemController : MonoBehaviour
	{
		// Token: 0x06004D0A RID: 19722 RVA: 0x0020C194 File Offset: 0x0020A394
		public void SetListItem(TIShipPartTemplate part)
		{
			this.partStr.SetText(part.displayName);
		}

		// Token: 0x04002FA5 RID: 12197
		public TMP_Text partStr;
	}
}
