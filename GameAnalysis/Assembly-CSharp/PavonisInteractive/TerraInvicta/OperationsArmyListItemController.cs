using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008B3 RID: 2227
	public class OperationsArmyListItemController : MonoBehaviour
	{
		// Token: 0x060054EE RID: 21742 RVA: 0x0026961F File Offset: 0x0026781F
		public void ItemSelected()
		{
			if (this.army != null && this.army.exists)
			{
				SoundEffectController.PlaySelectSound(this.army);
				TIUtilities.GotoGameState(this.army, true, true, true, true, false, -1f);
			}
		}

		// Token: 0x060054EF RID: 21743 RVA: 0x0026965C File Offset: 0x0026785C
		public void Initialize(TIArmyState army)
		{
			this.cachedArmyToggle = true;
			this.selectArmyToggle.interactable = true;
			this.army = army;
			this.UpdateListItem(null);
		}

		// Token: 0x060054F0 RID: 21744 RVA: 0x00269680 File Offset: 0x00267880
		public void UpdateListItem(TIRegionState destination = null)
		{
			if (!TIGameState.Valid(this.army) || this.army.currentRegion == null || this.army.homeRegion == null || this.army.destroyed || this.army.archived || this == null)
			{
				return;
			}
			this.armyName.SetText(this.army.displayName);
			string text = this.army.strength.ToPercent("P0");
			if (this.army.CanHeal())
			{
				this.armyStrength.SetText(TIUtilities.GreenLine(text));
			}
			else if (this.army.InBattleWithArmiesOrRegionDefenses())
			{
				this.armyStrength.SetText(TIUtilities.RedLine(text));
			}
			else
			{
				this.armyStrength.SetText(text);
			}
			TMP_Text tmp_Text = this.armyHomeRegion;
			TIRegionState homeRegion = this.army.homeRegion;
			tmp_Text.SetText(((homeRegion != null) ? homeRegion.displayName : null) ?? "ERROR");
			TIRegionState currentRegion = this.army.currentRegion;
			string text2 = ((currentRegion != null) ? currentRegion.displayName : null) ?? "ERROR";
			if (this.army.homeRegion != null)
			{
				if (this.army.currentRegion == this.army.homeRegion)
				{
					text2 = TIUtilities.GreenLine(text2);
				}
				else if (this.army.currentNation == this.army.homeNation)
				{
					text2 = TIUtilities.BlueLine(text2);
				}
				else if (this.army.currentNation.allies.Contains(this.army.homeNation))
				{
					text2 = TIUtilities.YellowLine(text2);
				}
				else if (this.army.currentNation.wars.Contains(this.army.homeNation))
				{
					text2 = TIUtilities.RedLine(text2);
				}
			}
			this.armyCurrentRegion.SetText(text2);
			this.armyControllingFaction.sprite = this.army.GetForegroundIcon();
			this.armyControllingFactionBackground.sprite = this.army.GetIconBackgroundSprite;
			this.armyControllingFactionBackground.color = this.army.GetIconBackgroundResourceColor;
			this.armyTechLevel.SetText(this.army.techLevel.ToString("N1"));
			this.armyDeploymentTypeImage.enabled = this.army.deploymentType == DeploymentType.Naval;
			if (destination != null)
			{
				float num;
				this.army.GetJourney(this.army.currentRegion, destination, out num);
				if (num == float.PositiveInfinity)
				{
					this.cachedArmyToggle = this.selectArmyToggle.isOn;
					this.selectArmyToggle.SetIsOnWithoutNotify(false);
					this.selectArmyToggle.interactable = false;
				}
				else
				{
					this.selectArmyToggle.interactable = true;
					this.selectArmyToggle.isOn = this.cachedArmyToggle;
					this.armyTravelTime.SetText(num.ToString(TIUtilities.DecimalPlaces((double)num, 1, 0)));
				}
			}
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
			this.armyTechLevel.enabled = true;
			this.armyStrength.enabled = true;
			this.armyControllingFaction.enabled = true;
			this.armyControllingFactionBackground.enabled = true;
			this.armyStandingOrdersIcon.gameObject.SetActive(this.army.huntingXenofauna);
		}

		// Token: 0x060054F1 RID: 21745 RVA: 0x00269A30 File Offset: 0x00267C30
		public void OnUpdateArmyToggle()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.cachedArmyToggle = this.selectArmyToggle.isOn;
		}

		// Token: 0x04003B12 RID: 15122
		public TIArmyState army;

		// Token: 0x04003B13 RID: 15123
		public Image armyControllingFaction;

		// Token: 0x04003B14 RID: 15124
		public Image armyControllingFactionBackground;

		// Token: 0x04003B15 RID: 15125
		public Image armyControllingFactionIcon;

		// Token: 0x04003B16 RID: 15126
		public TMP_Text armyName;

		// Token: 0x04003B17 RID: 15127
		public Image armyDeploymentTypeImage;

		// Token: 0x04003B18 RID: 15128
		public TMP_Text armyStrength;

		// Token: 0x04003B19 RID: 15129
		public TMP_Text armyHomeRegion;

		// Token: 0x04003B1A RID: 15130
		public TMP_Text armyCurrentRegion;

		// Token: 0x04003B1B RID: 15131
		public TMP_Text armyTechLevel;

		// Token: 0x04003B1C RID: 15132
		public TMP_Text armyTravelTime;

		// Token: 0x04003B1D RID: 15133
		public Image armyStandingOrdersIcon;

		// Token: 0x04003B1E RID: 15134
		public Toggle selectArmyToggle;

		// Token: 0x04003B1F RID: 15135
		public Image validDestinationImageColor;

		// Token: 0x04003B20 RID: 15136
		private bool cachedArmyToggle = true;
	}
}
