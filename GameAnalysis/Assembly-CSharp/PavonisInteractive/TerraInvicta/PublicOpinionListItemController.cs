using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000886 RID: 2182
	public class PublicOpinionListItemController : MonoBehaviour
	{
		// Token: 0x060051A0 RID: 20896 RVA: 0x0023E888 File Offset: 0x0023CA88
		public void InitListItem(TIFactionState faction, FactionIdeology ideology)
		{
			if (faction != null)
			{
				this.factionColorImage.color = faction.template.color;
				this.factionGradientImage.sprite = GameControl.assetLoader.LoadAssetForSpriteAssignment(faction.template.gradientPath);
				this.factionNameText.SetText(faction.displayNameCapitalized);
				this.factionIconImage.sprite = faction.factionIcon256UI;
				this.factionIconImage2.sprite = faction.factionIcon256UI;
				this.factionObjectiveText.SetText(faction.ideology.ideologyStrPublicOpinion);
				this.factionObjectiveText.gameObject.SetActive(true);
			}
			else
			{
				this.factionNameText.SetText(GameStateManager.UndecidedIdeology().ideologyStrPublicOpinion);
				this.factionObjectiveText.SetText("");
				this.factionObjectiveText.gameObject.SetActive(false);
			}
			this.ideology = ideology;
			this.globalPercentageText.SetText("");
		}

		// Token: 0x060051A1 RID: 20897 RVA: 0x0023E980 File Offset: 0x0023CB80
		public void UpdateListItem(string percentage)
		{
			this.globalPercentageText.SetText(percentage);
		}

		// Token: 0x0400363B RID: 13883
		public Image factionColorImage;

		// Token: 0x0400363C RID: 13884
		public Image factionGradientImage;

		// Token: 0x0400363D RID: 13885
		public Image factionIconImage;

		// Token: 0x0400363E RID: 13886
		public Image factionIconImage2;

		// Token: 0x0400363F RID: 13887
		public TMP_Text factionNameText;

		// Token: 0x04003640 RID: 13888
		public TMP_Text factionObjectiveText;

		// Token: 0x04003641 RID: 13889
		public TMP_Text globalPercentageText;

		// Token: 0x04003642 RID: 13890
		public FactionIdeology ideology;
	}
}
