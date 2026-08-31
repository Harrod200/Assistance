using System;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200088C RID: 2188
	public class ArmyListItemController : MonoBehaviour
	{
		// Token: 0x060051D6 RID: 20950 RVA: 0x0023F5A5 File Offset: 0x0023D7A5
		public void ItemSelected()
		{
			if (this.army != null && this.army.exists)
			{
				SoundEffectController.PlaySelectSound(this.army);
				TIUtilities.GotoGameState(this.army, true, true, true, true, false, -1f);
			}
		}

		// Token: 0x060051D7 RID: 20951 RVA: 0x0023F5E4 File Offset: 0x0023D7E4
		public string GetArmyTooltip(TIArmyState army, string strengthStr)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (army.faction != null)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.ArmiesTab.Faction", new object[] { army.faction.displayNameWithColor }));
			}
			else
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.ArmiesTab.NoFactionTooltip"));
			}
			if (army.deploymentType == DeploymentType.Naval)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.ArmiesTab.NavyPresent"));
			}
			else
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.ArmiesTab.NoNavy"));
			}
			stringBuilder.AppendLine(Loc.T("UI.Nation.ArmiesTab.Strength", new object[] { strengthStr }));
			if (army.huntingXenofauna)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Army.HuntingXenos"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060051D8 RID: 20952 RVA: 0x0023F6A9 File Offset: 0x0023D8A9
		public void Initialize(TIArmyState army, NationInfoController controller)
		{
			this.RemoveListener();
			this.controller = controller;
			this.army = army;
			this.UpdateListItem();
		}

		// Token: 0x060051D9 RID: 20953 RVA: 0x0023F6C8 File Offset: 0x0023D8C8
		private void OnEnable()
		{
			this.RemoveListener();
			if (this.army != null)
			{
				this.eventName = this.army.armyStatusUpdateEventName;
				GameControl.eventManager.AddListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.OnArmyStatusUpdate), this.eventName, this.army, true, false);
			}
		}

		// Token: 0x060051DA RID: 20954 RVA: 0x0023F720 File Offset: 0x0023D920
		public void UpdateListItem()
		{
			if (!TIGameState.Valid(this.army) || this.army.currentRegion == null || this.army.homeRegion == null || this.army.destroyed || this.army.archived || this == null)
			{
				return;
			}
			this.armyName.SetText(this.army.displayName);
			string strengthStr = this.army.strength.ToPercent("P0");
			if (this.army.CanHeal())
			{
				this.armyStrength.SetText(TIUtilities.GreenLine(strengthStr));
			}
			else if (this.army.InBattleWithArmiesOrRegionDefenses())
			{
				this.armyStrength.SetText(TIUtilities.RedLine(strengthStr));
			}
			else
			{
				this.armyStrength.SetText(strengthStr);
			}
			TMP_Text tmp_Text = this.armyHomeRegion;
			TIRegionState homeRegion = this.army.homeRegion;
			tmp_Text.SetText(((homeRegion != null) ? homeRegion.displayName : null) ?? "ERROR");
			TIRegionState currentRegion = this.army.currentRegion;
			string text = ((currentRegion != null) ? currentRegion.displayName : null) ?? "ERROR";
			if (this.army.homeRegion != null)
			{
				if (this.army.currentRegion == this.army.homeRegion)
				{
					text = TIUtilities.GreenLine(text);
				}
				else if (this.army.currentNation == this.army.homeNation)
				{
					text = TIUtilities.BlueLine(text);
				}
				else if (this.army.currentNation.allies.Contains(this.army.homeNation))
				{
					text = TIUtilities.YellowLine(text);
				}
				else if (this.army.currentNation.wars.Contains(this.army.homeNation))
				{
					text = TIUtilities.RedLine(text);
				}
			}
			this.armyCurrentRegion.SetText(text);
			this.armyControllingFaction.sprite = this.army.GetForegroundIcon();
			this.armyControllingFactionBackground.sprite = this.army.GetIconBackgroundSprite;
			this.armyControllingFactionBackground.color = this.army.GetIconBackgroundResourceColor;
			this.armyTooltip.SetDelegate("BodyText", () => this.GetArmyTooltip(this.army, strengthStr));
			this.armyTechLevel.SetText(this.army.techLevel.ToString("N1"));
			this.armyDeploymentTypeImage.enabled = this.army.deploymentType == DeploymentType.Naval;
			if (this.army.faction != null)
			{
				this.armyControllingFactionIcon.sprite = this.army.faction.factionIcon64UI;
				this.armyControllingFactionIcon.enabled = true;
			}
			else
			{
				this.armyControllingFactionIcon.enabled = false;
			}
			this.armyName.enabled = true;
			this.armyHomeRegion.enabled = true;
			this.armyCurrentRegion.enabled = true;
			this.armyTooltip.enabled = true;
			this.armyTechLevel.enabled = true;
			this.armyStrength.enabled = true;
			this.armyControllingFaction.enabled = true;
			this.armyControllingFactionBackground.enabled = true;
			this.armyStandingOrdersIcon.gameObject.SetActive(this.army.huntingXenofauna);
		}

		// Token: 0x060051DB RID: 20955 RVA: 0x0023FA88 File Offset: 0x0023DC88
		private void OnArmyStatusUpdate(ArmyStatusUpdate e)
		{
			if (this.controller.Visible())
			{
				this.UpdateListItem();
			}
		}

		// Token: 0x060051DC RID: 20956 RVA: 0x0023FA9D File Offset: 0x0023DC9D
		public void RemoveListener()
		{
			if (this.eventName != string.Empty)
			{
				GameControl.eventManager.RemoveListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.OnArmyStatusUpdate), this.eventName);
			}
		}

		// Token: 0x060051DD RID: 20957 RVA: 0x0023FACD File Offset: 0x0023DCCD
		private void OnDisable()
		{
			this.RemoveListener();
		}

		// Token: 0x060051DE RID: 20958 RVA: 0x0023FAD5 File Offset: 0x0023DCD5
		private void OnDestroy()
		{
			this.RemoveListener();
		}

		// Token: 0x04003679 RID: 13945
		private NationInfoController controller;

		// Token: 0x0400367A RID: 13946
		private TIArmyState army;

		// Token: 0x0400367B RID: 13947
		public Image armyControllingFaction;

		// Token: 0x0400367C RID: 13948
		public Image armyControllingFactionBackground;

		// Token: 0x0400367D RID: 13949
		public Image armyControllingFactionIcon;

		// Token: 0x0400367E RID: 13950
		public TooltipTrigger armyTooltip;

		// Token: 0x0400367F RID: 13951
		public TMP_Text armyName;

		// Token: 0x04003680 RID: 13952
		public Image armyDeploymentTypeImage;

		// Token: 0x04003681 RID: 13953
		public TMP_Text armyStrength;

		// Token: 0x04003682 RID: 13954
		public TMP_Text armyHomeRegion;

		// Token: 0x04003683 RID: 13955
		public TMP_Text armyCurrentRegion;

		// Token: 0x04003684 RID: 13956
		public TMP_Text armyTechLevel;

		// Token: 0x04003685 RID: 13957
		public Image armyStandingOrdersIcon;

		// Token: 0x04003686 RID: 13958
		private string eventName = string.Empty;
	}
}
