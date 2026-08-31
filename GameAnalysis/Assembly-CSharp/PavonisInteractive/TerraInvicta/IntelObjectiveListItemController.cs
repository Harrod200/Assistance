using System;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200087F RID: 2175
	public class IntelObjectiveListItemController : MonoBehaviour
	{
		// Token: 0x06005140 RID: 20800 RVA: 0x00238724 File Offset: 0x00236924
		public void SetListItem(TIObjectiveTemplate objective, TIFactionState faction, bool victory)
		{
			if (victory)
			{
				this.objectiveTitle.SetText(objective.VictorySummary(faction).Trim());
				return;
			}
			this.objectiveTitle.SetText(objective.displayName(faction).Trim());
		}

		// Token: 0x04003557 RID: 13655
		public TMP_Text objectiveTitle;
	}
}
