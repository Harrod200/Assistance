using System;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000836 RID: 2102
	internal class FactionIconGridItemController : MonoBehaviour
	{
		// Token: 0x06004C26 RID: 19494 RVA: 0x00200891 File Offset: 0x001FEA91
		public void SetListItem(TIFactionState faction)
		{
			this.factionIcon.sprite = faction.factionIcon64UI;
		}

		// Token: 0x04002DD8 RID: 11736
		public Image factionIcon;
	}
}
