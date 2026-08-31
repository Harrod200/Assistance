using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008AA RID: 2218
	public class PolicyTargetGridItemController : MonoBehaviour
	{
		// Token: 0x06005412 RID: 21522 RVA: 0x00260665 File Offset: 0x0025E865
		public void Init(NotificationScreenController controller)
		{
			this.controller = controller;
		}

		// Token: 0x06005413 RID: 21523 RVA: 0x00260670 File Offset: 0x0025E870
		public void UpdateListItem(TIGameState target, TIPolicyOption policyOption, TIFactionState policyFaction, TINationState proposingNation)
		{
			this.heldTarget = target;
			TIPolicyOptionWithConfirm tipolicyOptionWithConfirm = policyOption as TIPolicyOptionWithConfirm;
			TINationState tinationState = target as TINationState;
			TIWarState tiwarState = target as TIWarState;
			StringBuilder stringBuilder;
			if (tiwarState != null)
			{
				tinationState = tiwarState.EnemyWarLeader(proposingNation, true);
				this.targetNationFlag.sprite = tinationState.flag;
				stringBuilder = new StringBuilder(Loc.T("WarOption.joinWarListItem", new object[] { tinationState.displayName, tiwarState.displayName }));
				this.secondaryIcon.gameObject.SetActive(false);
			}
			else
			{
				TIFederationState tifederationState = target as TIFederationState;
				if (tifederationState != null)
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(tifederationState.flagResource, this.targetNationFlag);
					stringBuilder = new StringBuilder(tifederationState.displayName);
					this.secondaryIcon.gameObject.SetActive(false);
				}
				else
				{
					this.targetNationFlag.sprite = target.ref_nation.flag;
					stringBuilder = new StringBuilder(TIUtilities.GetStateDisplayName(target, policyFaction, false, false, false, false, true));
					if (target.isArmyState && target.ref_army.deploymentType == DeploymentType.Naval)
					{
						GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathNavalArmyIcon, this.secondaryIcon);
						this.secondaryIcon.gameObject.SetActive(true);
					}
					else if (target.isRegionState && proposingNation.hostileClaims.Contains(target.ref_region))
					{
						GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathUnrestIcon, this.secondaryIcon);
						this.secondaryIcon.gameObject.SetActive(true);
					}
					else if (policyOption.GetPolicyType() == PolicyType.UnificationOption && proposingNation.hostileClaims.Intersect<TIRegionState>(tinationState.regions).Any<TIRegionState>())
					{
						GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathUnrestIcon, this.secondaryIcon);
						this.secondaryIcon.gameObject.SetActive(true);
					}
					else if (policyOption.GetPolicyType() == PolicyType.WarOption && tinationState != null && tinationState.NumNuclearWeaponsDefendingMe() > 0)
					{
						GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathNukesIcon, this.secondaryIcon);
						this.secondaryIcon.gameObject.SetActive(true);
					}
					else
					{
						this.secondaryIcon.gameObject.SetActive(false);
					}
				}
			}
			if (tipolicyOptionWithConfirm != null)
			{
				float num = tipolicyOptionWithConfirm.AIAgreeChance(proposingNation, target);
				string text = num.ToPercent("P0");
				if ((double)num < 0.2)
				{
					text = TIUtilities.RedLine(text);
				}
				else if ((double)num > 0.8)
				{
					text = TIUtilities.GreenLine(text);
				}
				else
				{
					text = TIUtilities.CyanLine(text);
				}
				stringBuilder.Append(Loc.T("UI.Notifications.PolicySuccessChance", new object[] { text }));
			}
			this.targetName.SetText(stringBuilder);
			if (tinationState != null && tinationState.executiveFaction != null)
			{
				this.executiveFactionIcon.sprite = tinationState.executiveFaction.factionIcon64;
				this.executiveFactionIcon.enabled = true;
				return;
			}
			this.executiveFactionIcon.enabled = false;
		}

		// Token: 0x06005414 RID: 21524 RVA: 0x00260968 File Offset: 0x0025EB68
		public void OnClicked()
		{
			this.controller.PolicyTargetSelected(this.heldTarget);
		}

		// Token: 0x04003A52 RID: 14930
		private NotificationScreenController controller;

		// Token: 0x04003A53 RID: 14931
		public TMP_Text targetName;

		// Token: 0x04003A54 RID: 14932
		public Image targetNationFlag;

		// Token: 0x04003A55 RID: 14933
		private TIGameState heldTarget;

		// Token: 0x04003A56 RID: 14934
		public Image executiveFactionIcon;

		// Token: 0x04003A57 RID: 14935
		public Image secondaryIcon;
	}
}
