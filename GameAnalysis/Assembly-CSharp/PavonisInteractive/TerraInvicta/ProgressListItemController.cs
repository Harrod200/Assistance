using System;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000847 RID: 2119
	public class ProgressListItemController : MonoBehaviour
	{
		// Token: 0x06004CFC RID: 19708 RVA: 0x0020B49C File Offset: 0x0020969C
		public void UpdateData(TIFactionState faction)
		{
			this.factionIcon.sprite = faction.factionIcon64;
			if (GameStateManager.PromptQueue().HasPrompt(faction, GameStateManager.MissionPhase(), null, "PromptSelectCouncilorMissions", 0))
			{
				this.statusBG.color = this.completeColor;
				this.statusCheck.enabled = true;
				return;
			}
			this.statusBG.color = this.incompleteColor;
			this.statusCheck.enabled = false;
		}

		// Token: 0x04002F47 RID: 12103
		public Image factionIcon;

		// Token: 0x04002F48 RID: 12104
		public Image statusBG;

		// Token: 0x04002F49 RID: 12105
		public Image statusCheck;

		// Token: 0x04002F4A RID: 12106
		public Color completeColor;

		// Token: 0x04002F4B RID: 12107
		public Color incompleteColor;
	}
}
