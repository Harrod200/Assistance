using System;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200088D RID: 2189
	public class ClaimListItemController : MonoBehaviour
	{
		// Token: 0x060051E0 RID: 20960 RVA: 0x0023FAF0 File Offset: 0x0023DCF0
		public void UpdateListItem(TINationState claimantNation, TIRegionState region)
		{
			this.claimantNation = claimantNation;
			bool flag = false;
			if (region != null && claimantNation.template.unionTrigger > 0)
			{
				if (claimantNation.isUnion)
				{
					flag = true;
				}
				else if (claimantNation.inFederation && claimantNation.federation.leadNation == claimantNation && (claimantNation.federation.members.Contains(region.nation) || claimantNation.federation.MemberClaims(true).Contains(region)) && claimantNation.WillbeUnion(1))
				{
					flag = true;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (claimantNation.hostileClaims.Contains(region))
			{
				stringBuilder.Append(TemplateManager.global.unrestInlineSpritePath);
			}
			if (flag)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(claimantNation.template.GetUnionFlagResource(), this.claimFlag);
				this.claimTTTrigger.SetText("BodyText", stringBuilder.Append(claimantNation.template.unionDisplayName).ToString());
			}
			else
			{
				this.claimFlag.sprite = claimantNation.flag;
				this.claimTTTrigger.SetText("BodyText", stringBuilder.Append(claimantNation.displayName).ToString());
			}
			this.claimFlag.enabled = true;
			this.claimTTTrigger.enabled = true;
		}

		// Token: 0x060051E1 RID: 20961 RVA: 0x0023FC30 File Offset: 0x0023DE30
		public void OnFlagButtonPressed()
		{
			if (this.claimantNation.extant)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_RegionSelect", false, false);
				TIUtilities.GotoGameState(this.claimantNation, true, true, true, true, false, -1f);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x04003687 RID: 13959
		private TINationState claimantNation;

		// Token: 0x04003688 RID: 13960
		public Image claimFlag;

		// Token: 0x04003689 RID: 13961
		public TooltipTrigger claimTTTrigger;
	}
}
