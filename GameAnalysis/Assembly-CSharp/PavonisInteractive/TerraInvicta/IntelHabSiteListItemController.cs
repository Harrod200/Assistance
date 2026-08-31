using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200087C RID: 2172
	public class IntelHabSiteListItemController : MonoBehaviour
	{
		// Token: 0x06005133 RID: 20787 RVA: 0x002381A0 File Offset: 0x002363A0
		public void Initialize(IntelScreenHabSiteListItem_Data data)
		{
			this.site = data.habSiteState;
			this.habSiteName.SetText(data.habsiteName);
			this.spaceBodyName.SetText(data.spaceBodyName);
			this.siteDescription.SetText(data.siteDescription);
			this.spaceBodyIcon.sprite = this.site.ref_spaceBody.icon;
			this.controller = data.controller;
			this.habSiteHohmannTip.SetDelegate("BodyText", () => SpaceObjectDetailController.SetTimePenaltyTip(this.site.parentBody));
			if (!data.hasLaunchWindow)
			{
				this.earthLaunchWindow.SetText(string.Empty);
			}
			else
			{
				this.earthLaunchWindow.SetText(Loc.T("UI.Intel.LaunchWindowText", new object[]
				{
					TemplateManager.global.orbitInlineSpritePath,
					data.launchWindowPenalty,
					data.launchWindowCloserToPrior ? TemplateManager.global.upRedArrowInlineSpritePath : TemplateManager.global.downGreenArrowInlineSpritePath
				}));
			}
			this.SetPlanetTag();
			this.Refresh();
		}

		// Token: 0x06005134 RID: 20788 RVA: 0x002382A8 File Offset: 0x002364A8
		public void Refresh()
		{
			if (this.site.hasPlannedOrOperatingBase)
			{
				this.habImage.sprite = this.site.hab.icon;
				this.habImage.enabled = true;
				this.mineImage.enabled = this.site.hab.HasMine;
				this.habName.SetText(this.site.hab.GetDisplayName(GameControl.control.activePlayer));
			}
			else
			{
				this.habImage.enabled = false;
				this.mineImage.enabled = false;
				this.habName.SetText("");
			}
			this.water.SetText(TIUtilities.FormatSmallNumber(this.site.GetMonthlyProduction(FactionResource.Water), 7, 0, true, false));
			this.volatiles.SetText(TIUtilities.FormatSmallNumber(this.site.GetMonthlyProduction(FactionResource.Volatiles), 7, 0, true, false));
			this.metals.SetText(TIUtilities.FormatSmallNumber(this.site.GetMonthlyProduction(FactionResource.Metals), 7, 0, true, false));
			this.nobles.SetText(TIUtilities.FormatSmallNumber(this.site.GetMonthlyProduction(FactionResource.NobleMetals), 7, 0, true, false));
			this.fissiles.SetText(TIUtilities.FormatSmallNumber(this.site.GetMonthlyProduction(FactionResource.Fissiles), 7, 0, true, false));
		}

		// Token: 0x06005135 RID: 20789 RVA: 0x002383F7 File Offset: 0x002365F7
		public void OnClickSort(int sort)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.controller.OnChangeHabSiteSort(sort);
		}

		// Token: 0x06005136 RID: 20790 RVA: 0x00238411 File Offset: 0x00236611
		public void OnClickGoto()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
			TIGameState tigameState = this.site;
			this.controller.Close();
			TIUtilities.GotoGameState(tigameState, true, true, true, true, true, -1f);
		}

		// Token: 0x06005137 RID: 20791 RVA: 0x00238440 File Offset: 0x00236640
		public void OnClickPlanetTag()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			PlayerTag[] array = (PlayerTag[])Enum.GetValues(typeof(PlayerTag));
			int num = (int)((this.site.ref_spaceBody.playerTag + 1) % (PlayerTag)array.Length);
			this.controller.activePlayer.playerControl.StartAction(new SetPlanetTagAction(this.site.ref_spaceBody, array[num]));
			this.controller.UpdateHabSiteListModelData();
			this.controller.UpdateHabSiteListSortTag();
		}

		// Token: 0x06005138 RID: 20792 RVA: 0x002384C4 File Offset: 0x002366C4
		private void SetPlanetTag()
		{
			switch (this.site.ref_spaceBody.playerTag)
			{
			case PlayerTag.Red:
				this.playerTagButtonImage.color = SpaceObjectSymbolController.PlanetTagRed;
				return;
			case PlayerTag.Green:
				this.playerTagButtonImage.color = SpaceObjectSymbolController.PlanetTagGreen;
				return;
			}
			this.playerTagButtonImage.color = Color.white;
		}

		// Token: 0x04003535 RID: 13621
		public TIHabSiteState site;

		// Token: 0x04003536 RID: 13622
		public Image habImage;

		// Token: 0x04003537 RID: 13623
		public Image mineImage;

		// Token: 0x04003538 RID: 13624
		public TMP_Text habSiteName;

		// Token: 0x04003539 RID: 13625
		public TMP_Text habName;

		// Token: 0x0400353A RID: 13626
		public Image spaceBodyIcon;

		// Token: 0x0400353B RID: 13627
		public TMP_Text spaceBodyName;

		// Token: 0x0400353C RID: 13628
		public TMP_Text water;

		// Token: 0x0400353D RID: 13629
		public TMP_Text volatiles;

		// Token: 0x0400353E RID: 13630
		public TMP_Text metals;

		// Token: 0x0400353F RID: 13631
		public TMP_Text nobles;

		// Token: 0x04003540 RID: 13632
		public TMP_Text fissiles;

		// Token: 0x04003541 RID: 13633
		public TMP_Text siteDescription;

		// Token: 0x04003542 RID: 13634
		public TMP_Text earthLaunchWindow;

		// Token: 0x04003543 RID: 13635
		public Image playerTagButtonImage;

		// Token: 0x04003544 RID: 13636
		private IntelScreenController controller;

		// Token: 0x04003545 RID: 13637
		public TooltipTrigger habSiteHohmannTip;

		// Token: 0x04003546 RID: 13638
		public PlayerTag planetTag;

		// Token: 0x04003547 RID: 13639
		public double launchWindowPenalty;

		// Token: 0x04003548 RID: 13640
		public double orbitValue;
	}
}
