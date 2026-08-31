using System;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200083B RID: 2107
	public class OrgItemView : MonoBehaviour
	{
		// Token: 0x17000EA4 RID: 3748
		// (get) Token: 0x06004C49 RID: 19529 RVA: 0x002035DB File Offset: 0x002017DB
		private Button Button
		{
			get
			{
				if (this.button == null)
				{
					this.button = base.GetComponent<Button>();
				}
				return this.button;
			}
		}

		// Token: 0x06004C4A RID: 19530 RVA: 0x00203600 File Offset: 0x00201800
		public void UpdateOrgItem(TIOrgState org, TICouncilorState councilor)
		{
			this.org = org;
			StringBuilder stringBuilder = new StringBuilder(org.displayName).Append("\n");
			this.SetStatus();
			stringBuilder.Append(org.QuickDescription(true)).Append("\n");
			switch (this.status)
			{
			case OrgItemView.OrgStatus.ASSIGNED:
				if (this.councilorController.orgCouncilTabActive)
				{
					this.dragDestination = this.councilorController.councilDragDestination;
				}
				else
				{
					this.dragDestination = this.councilorController.availableDragDestination;
				}
				break;
			case OrgItemView.OrgStatus.UNASSIGNED:
				this.dragDestination = this.councilorController.councilorDragDestination;
				stringBuilder.Append(org.GetTransferCost().ToString("N0", false, false, null, false, FactionResource.None));
				break;
			case OrgItemView.OrgStatus.AVAILABLE:
				stringBuilder.Append(org.GetPurchaseCost(councilor.faction).ToString("N0", false, false, null, false, FactionResource.None));
				this.dragDestination = this.councilorController.councilorDragDestination;
				this.newRibbon.enabled = councilor.faction.newAvailableOrgs.Contains(org);
				break;
			}
			this.orgName.SetText(stringBuilder.ToString());
			if (org.requiresNationInterest)
			{
				this.flagIcon.sprite = org.requiredNationInterest.flag;
				this.flagIcon.enabled = true;
			}
			else
			{
				this.flagIcon.enabled = false;
			}
			this.SetAssignable(councilor);
			if (this.tooltip != null)
			{
				this.tooltip.SetDelegate("BodyText", () => org.description(true, GameControl.control.activePlayer, false, false));
			}
			this.orgIcon.sprite = this.org.icon;
			this.tierData.text = org.tierStarsInline;
		}

		// Token: 0x06004C4B RID: 19531 RVA: 0x002037F2 File Offset: 0x002019F2
		private bool IsAssignable(TICouncilorState councilor)
		{
			return this.org.CouncilorCanAcquire(councilor) && councilor.SufficientCapacityForOrg(this.org) && councilor.faction.CanPurchaseOrg(this.org);
		}

		// Token: 0x06004C4C RID: 19532 RVA: 0x00203824 File Offset: 0x00201A24
		private void SetAssignable(TICouncilorState councilor)
		{
			if (this.canvasGroup == null)
			{
				this.canvasGroup = base.GetComponent<CanvasGroup>();
			}
			if (this.canvasGroup != null)
			{
				OrgItemView.OrgStatus orgStatus = this.status;
				if (orgStatus != OrgItemView.OrgStatus.ASSIGNED)
				{
					if (orgStatus - OrgItemView.OrgStatus.UNASSIGNED <= 1)
					{
						this.canvasGroup.alpha = (this.IsAssignable(councilor) ? 1f : 0.3f);
						return;
					}
				}
				else
				{
					this.canvasGroup.alpha = (this.org.applyingBonuses ? 1f : 0.3f);
				}
			}
		}

		// Token: 0x06004C4D RID: 19533 RVA: 0x002038AE File Offset: 0x00201AAE
		private void SetStatus()
		{
			if (this.org.hasCouncilor)
			{
				this.status = OrgItemView.OrgStatus.ASSIGNED;
				return;
			}
			if (this.org.hasFactionbutNoCouncilor)
			{
				this.status = OrgItemView.OrgStatus.UNASSIGNED;
				return;
			}
			this.status = OrgItemView.OrgStatus.AVAILABLE;
		}

		// Token: 0x06004C4E RID: 19534 RVA: 0x002038E1 File Offset: 0x00201AE1
		public TIOrgState GetOrg()
		{
			return this.org;
		}

		// Token: 0x06004C4F RID: 19535 RVA: 0x002038EC File Offset: 0x00201AEC
		public void OnLeftClickItem()
		{
			if (this.dragDestination == this.councilorController.availableDragDestination || this.dragDestination == this.councilorController.councilDragDestination)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.org.tier; i++)
				{
					stringBuilder.Append(TIGlobalConfig.globalConfig.starInlineSpritePath);
				}
				this.councilorController.ShowInfoMyOrg(this.orgName.text, this.org.description(false, GameControl.control.activePlayer, false, false), this.orgIcon.sprite, stringBuilder.ToString());
				this.councilorController.selectedOrgTop = this;
				this.councilorController.orgActionButtonTextTop.SetText(Loc.T("UI.Council.UnequipOrg"));
				this.councilorController.orgActionButtonTextTop2.SetText(Loc.T("UI.Council.SellOrg"));
				this.councilorController.SetOrgSelected(this, true);
			}
			if (this.dragDestination == this.councilorController.councilorDragDestination)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				for (int j = 0; j < this.org.tier; j++)
				{
					stringBuilder2.Append(TIGlobalConfig.globalConfig.starInlineSpritePath);
				}
				this.councilorController.ShowInfoEquipOrg(this.org.displayName, this.org.hasFactionbutNoCouncilor ? this.org.GetSalePrice(false).ToString("Relevant", false, false, null, false, FactionResource.Money) : this.org.GetPurchaseCost(GameControl.control.activePlayer).ToString("Relevant", false, false, null, false, FactionResource.None), this.org.description(false, GameControl.control.activePlayer, false, false), this.orgIcon.sprite, stringBuilder2.ToString(), !this.org.hasFactionbutNoCouncilor);
				this.councilorController.selectedOrgBottom = this;
				if (this.org.hasFactionbutNoCouncilor)
				{
					this.councilorController.orgActionButtonTextBottom.SetText(Loc.T("UI.Council.EquipOrg"));
					this.councilorController.orgActionButtonTextBottom2.SetText(Loc.T("UI.Council.SellOrg"));
					this.councilorController.orgActionButtonBottom2.gameObject.SetActive(true);
				}
				else
				{
					this.councilorController.orgActionButtonTextBottom.SetText(Loc.T("UI.Council.PurchaseOrg"));
					this.councilorController.orgActionButtonBottom2.gameObject.SetActive(false);
				}
				this.councilorController.SetOrgSelected(this, false);
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_DropModuleInShipDesignSlot", false, false);
		}

		// Token: 0x06004C50 RID: 19536 RVA: 0x00203B78 File Offset: 0x00201D78
		public void OnRightClickItem()
		{
			if (this.dragDestination != null && !this.councilorController.lookingAtTurnedCouncilor)
			{
				if (this.dragDestination == this.councilorController.councilorDragDestination)
				{
					this.councilorController.StartOrgPurchase(base.GetComponent<OrgItemView>().GetOrg());
				}
				if (this.dragDestination == this.councilorController.availableDragDestination)
				{
					this.councilorController.StartSellOrg(base.GetComponent<OrgItemView>().GetOrg(), false);
				}
				if (this.dragDestination == this.councilorController.councilDragDestination)
				{
					this.councilorController.StartMoveToCouncilOrgs(base.GetComponent<OrgItemView>().GetOrg());
				}
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_DropModuleInShipDesignSlot", false, false);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06004C51 RID: 19537 RVA: 0x00203C4C File Offset: 0x00201E4C
		public void SetButtonHighlight(bool highlight)
		{
			if (this.background == null || this.Button == null)
			{
				return;
			}
			this.background.sprite = (highlight ? this.selectedSprite : this.Button.spriteState.pressedSprite);
		}

		// Token: 0x04002E22 RID: 11810
		public TMP_Text orgName;

		// Token: 0x04002E23 RID: 11811
		public Image orgIcon;

		// Token: 0x04002E24 RID: 11812
		public OrgItemView.OrgStatus status;

		// Token: 0x04002E25 RID: 11813
		public TooltipTrigger tooltip;

		// Token: 0x04002E26 RID: 11814
		public Image flagIcon;

		// Token: 0x04002E27 RID: 11815
		public TMP_Text tierData;

		// Token: 0x04002E28 RID: 11816
		public Image background;

		// Token: 0x04002E29 RID: 11817
		public Sprite selectedSprite;

		// Token: 0x04002E2A RID: 11818
		private Button button;

		// Token: 0x04002E2B RID: 11819
		public Image newRibbon;

		// Token: 0x04002E2C RID: 11820
		public CouncilGridController councilorController;

		// Token: 0x04002E2D RID: 11821
		private TIOrgState org;

		// Token: 0x04002E2E RID: 11822
		private CanvasGroup canvasGroup;

		// Token: 0x04002E2F RID: 11823
		private DragDestination dragDestination;

		// Token: 0x02001047 RID: 4167
		public enum OrgStatus
		{
			// Token: 0x04006234 RID: 25140
			ASSIGNED,
			// Token: 0x04006235 RID: 25141
			UNASSIGNED,
			// Token: 0x04006236 RID: 25142
			AVAILABLE
		}
	}
}
