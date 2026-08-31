using System;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008C2 RID: 2242
	public class FactionContributionBarListItemController : MonoBehaviour
	{
		// Token: 0x060055A2 RID: 21922 RVA: 0x0026EB48 File Offset: 0x0026CD48
		public void UpdateListItem(TIFactionState factionState, TechProgress currentTechProgress, int spacers)
		{
			this.factionColor.color = factionState.template.color;
			float num = currentTechProgress.factionContributions[factionState] / (currentTechProgress.techTemplate.GetResearchCost(GameControl.control.activePlayer) - ((currentTechProgress.remainingResearch < 0f) ? currentTechProgress.remainingResearch : 0f));
			float num2 = (float)(540 - spacers * 2);
			this.thisRT.sizeDelta = new Vector2((float)((int)Mathf.Clamp(num * num2, 1f, 540f)), this.thisRT.sizeDelta.y);
		}

		// Token: 0x060055A3 RID: 21923 RVA: 0x0026EBE8 File Offset: 0x0026CDE8
		public void UpdateListItem(TIFactionState factionState, ProjectProgress currentProjectProgress, int spacers)
		{
			this.factionColor.color = factionState.template.color;
			float num = currentProjectProgress.accumulatedResearch / currentProjectProgress.projectTemplate.GetResearchCost(factionState);
			float num2 = (float)(540 - spacers * 2);
			this.thisRT.sizeDelta = new Vector2((float)((int)Mathf.Clamp(num * num2, 1f, 540f)), this.thisRT.sizeDelta.y);
		}

		// Token: 0x04003C11 RID: 15377
		public Image factionColor;

		// Token: 0x04003C12 RID: 15378
		public RectTransform thisRT;
	}
}
