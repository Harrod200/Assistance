using System;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000898 RID: 2200
	public class RivalsGridItemController : MonoBehaviour
	{
		// Token: 0x06005308 RID: 21256 RVA: 0x0024D8FB File Offset: 0x0024BAFB
		public void ItemSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_RegionSelect", false, false);
			TIUtilities.GotoGameState(this.nation, true, true, true, true, false, -1f);
		}

		// Token: 0x06005309 RID: 21257 RVA: 0x0024D920 File Offset: 0x0024BB20
		public void UpdateGridItem(TINationState viewedNation, TINationState rivalNationState)
		{
			this.nation = rivalNationState;
			this.rivalFlag.sprite = rivalNationState.flag;
			if (rivalNationState.federation != null)
			{
				this.federationFlag.enabled = true;
				this.federationFlagBorder.enabled = true;
				GameControl.assetLoader.LoadAssetForImageAssignment(rivalNationState.federation.flagResource, this.federationFlag);
			}
			else if (rivalNationState.breakawayParent == viewedNation || viewedNation.breakawayParent == rivalNationState)
			{
				this.federationFlag.enabled = true;
				this.federationFlagBorder.enabled = true;
				GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathUnrestIcon, this.federationFlag);
			}
			else
			{
				this.federationFlag.enabled = false;
				this.federationFlagBorder.enabled = false;
			}
			this.nukesImage.enabled = rivalNationState.numNuclearWeapons > 0;
			this.armyImage.enabled = rivalNationState.numStandardArmies > 0;
			this.rivalTTTrigger.SetDelegate("BodyText", () => this.RivalTooltip(viewedNation, rivalNationState));
		}

		// Token: 0x0600530A RID: 21258 RVA: 0x0024DA81 File Offset: 0x0024BC81
		public string RivalTooltip(TINationState nation, TINationState rival)
		{
			StringBuilder stringBuilder = new StringBuilder(rival.displayName).AppendLine();
			stringBuilder.AppendLine(nation.CanAttackFeedback(rival)).AppendLine(rival.CanAttackFeedback(nation)).AppendLine(nation.CanEndRivalryFeedback(rival));
			return stringBuilder.ToString();
		}

		// Token: 0x0400380D RID: 14349
		public Image rivalFlag;

		// Token: 0x0400380E RID: 14350
		public Image federationFlag;

		// Token: 0x0400380F RID: 14351
		public Image federationFlagBorder;

		// Token: 0x04003810 RID: 14352
		public TooltipTrigger rivalTTTrigger;

		// Token: 0x04003811 RID: 14353
		public TINationState nation;

		// Token: 0x04003812 RID: 14354
		public Image armyImage;

		// Token: 0x04003813 RID: 14355
		public Image nukesImage;
	}
}
