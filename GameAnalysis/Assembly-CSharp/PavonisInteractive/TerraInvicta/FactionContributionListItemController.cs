using System;
using ModelShark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008C3 RID: 2243
	public class FactionContributionListItemController : MonoBehaviour
	{
		// Token: 0x060055A5 RID: 21925 RVA: 0x0026EC68 File Offset: 0x0026CE68
		public void UpdateListItem(TIFactionState factionState, TechProgress currentTechProgress)
		{
			this.factionImage.sprite = factionState.factionIcon64UI;
			this.factionColor.color = factionState.template.color;
			this.factionContribution.text = currentTechProgress.factionContributions[factionState].ToString("N0");
			this.backgroundImage.sprite = ((factionState == currentTechProgress.GetExpectedWinner(false)) ? this.winnerHighlightBackground : this.defaultBackground);
			this.bonusTip.SetDelegate("BodyText", () => ResearchPanelController.TechCategoryTooltip(factionState, currentTechProgress.techTemplate));
		}

		// Token: 0x04003C13 RID: 15379
		public Image factionImage;

		// Token: 0x04003C14 RID: 15380
		public Image factionColor;

		// Token: 0x04003C15 RID: 15381
		public Image backgroundImage;

		// Token: 0x04003C16 RID: 15382
		public Sprite defaultBackground;

		// Token: 0x04003C17 RID: 15383
		public Sprite winnerHighlightBackground;

		// Token: 0x04003C18 RID: 15384
		public TMP_Text factionContribution;

		// Token: 0x04003C19 RID: 15385
		public TooltipTrigger bonusTip;
	}
}
