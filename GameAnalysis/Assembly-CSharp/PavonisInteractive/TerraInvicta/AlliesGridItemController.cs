using System;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200088B RID: 2187
	public class AlliesGridItemController : MonoBehaviour
	{
		// Token: 0x060051D2 RID: 20946 RVA: 0x0023F37D File Offset: 0x0023D57D
		public void ItemSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_RegionSelect", false, false);
			TIUtilities.GotoGameState(this.nation, true, true, true, true, false, -1f);
		}

		// Token: 0x060051D3 RID: 20947 RVA: 0x0023F3A0 File Offset: 0x0023D5A0
		public void UpdateGridItem(TINationState viewedNation, TINationState allyNationState)
		{
			this.nation = allyNationState;
			this.allyFlag.sprite = allyNationState.flag;
			if (allyNationState.inFederation)
			{
				this.federationFlag.enabled = true;
				this.federationFlagBorder.enabled = true;
				GameControl.assetLoader.LoadAssetForImageAssignment(allyNationState.federation.flagResource, this.federationFlag);
			}
			else
			{
				this.federationFlag.enabled = false;
				this.federationFlagBorder.enabled = false;
			}
			this.nukesImage.enabled = allyNationState.numNuclearWeapons > 0;
			this.armyImage.enabled = allyNationState.numStandardArmies > 0;
			this.allyTrigger.SetDelegate("BodyText", () => this.AllyTooltip(viewedNation, allyNationState));
		}

		// Token: 0x060051D4 RID: 20948 RVA: 0x0023F498 File Offset: 0x0023D698
		public string AllyTooltip(TINationState nation, TINationState ally)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(ally.displayName);
			if (!nation.allies.Contains(ally))
			{
				stringBuilder.AppendLine(nation.CanAllyFeedback(ally));
			}
			else
			{
				stringBuilder.AppendLine(nation.CanEndAllianceFeedback(ally));
			}
			if (nation.federation == null && ally.federation == null)
			{
				stringBuilder.AppendLine(nation.CanFormFederationFeedback(ally));
			}
			if (nation.federation != null && ally.federation == null)
			{
				stringBuilder.AppendLine(ally.CanJoinFederationFeedback(nation.federation, ally));
			}
			if (nation.federation == null && ally.federation != null)
			{
				stringBuilder.AppendLine(nation.CanJoinFederationFeedback(ally.federation, nation));
			}
			if (ally.federation != null)
			{
				stringBuilder.AppendLine(ally.CanLeaveFederationFeedback());
			}
			stringBuilder.AppendLine(nation.CanUnifyFeedback(ally));
			return stringBuilder.ToString();
		}

		// Token: 0x04003672 RID: 13938
		public Image allyFlag;

		// Token: 0x04003673 RID: 13939
		public Image federationFlag;

		// Token: 0x04003674 RID: 13940
		public Image federationFlagBorder;

		// Token: 0x04003675 RID: 13941
		public TooltipTrigger allyTrigger;

		// Token: 0x04003676 RID: 13942
		public TINationState nation;

		// Token: 0x04003677 RID: 13943
		public Image armyImage;

		// Token: 0x04003678 RID: 13944
		public Image nukesImage;
	}
}
