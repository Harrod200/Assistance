using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000854 RID: 2132
	public class ShipClassListItemController : MonoBehaviour
	{
		// Token: 0x06004E4C RID: 20044 RVA: 0x0021B0A7 File Offset: 0x002192A7
		public void Init(FleetsScreenController controller, TISpaceShipTemplate shipDesign)
		{
			this.controller = controller;
			this.shipDesign = shipDesign;
			this.buildButtonText.SetText(Loc.T("UI.Fleets.Build"));
			this.upgradeButtonText.SetText(Loc.T("UI.Fleets.Upgrade"));
		}

		// Token: 0x06004E4D RID: 20045 RVA: 0x0021B0E4 File Offset: 0x002192E4
		public void UpdateListItem()
		{
			this.className.SetText(this.shipDesign.className);
			this.hullName.SetText(this.shipDesign.hullTemplate.displayName);
			this.role.SetText(this.shipDesign.roleStr);
			this.mass.SetText(Loc.T("UI.Fleets.Tons", new object[] { this.shipDesign.wetMass_tons.ToString("N0") }));
			this.acceleration.SetText(FleetsScreenController.dualAccelerationStr(this.shipDesign));
			float num = this.shipDesign.baseCruiseDeltaV_kps(false);
			this.DV.SetText(Loc.T("UI.Fleets.SingleDV", new object[] { num.ToString(TIUtilities.DecimalPlaces((double)num, 7, 0)) }));
			int num2 = GameControl.control.activePlayer.nShipyardQueues.Values.SelectMany<List<ShipConstructionQueueItem>, ShipConstructionQueueItem>((List<ShipConstructionQueueItem> x) => x).Count<ShipConstructionQueueItem>((ShipConstructionQueueItem x) => x.shipDesign.dataName == this.shipDesign.dataName);
			if (num2 > 0)
			{
				this.numberInService.SetText(Loc.T("UI.Fleets.NIS", new object[]
				{
					GameControl.control.activePlayer.ships.Count<TISpaceShipState>((TISpaceShipState x) => x.templateName == this.shipDesign.dataName).ToString("N0"),
					TIUtilities.YellowLine(num2.ToString("N0"))
				}));
			}
			else
			{
				this.numberInService.SetText(GameControl.control.activePlayer.ships.Count<TISpaceShipState>((TISpaceShipState x) => x.templateName == this.shipDesign.dataName).ToString("N0"));
			}
			this.buildCost.SetText(this.shipDesign.spaceResourceConstructionCost(false, null, true, false, false).GetString("Relevant", false, false, false, 7, false, false, GameControl.control.activePlayer, false, FactionResource.None));
			this.combatValue.SetText(this.shipDesign.TemplateSpaceCombatValue(false, -1f, 1f, false).ToString("N0"));
			this.assaultValue.SetText(this.shipDesign.AssaultCombatValue(false).ToString("N0"));
			this.obsoleteToggle.SetIsOnWithoutNotify(this.shipDesign.designingFaction.obsoleteShipDesigns.Contains(this.shipDesign.dataName));
			if (this.obsoleteToggle.isOn)
			{
				this.obsoleteIcon.sprite = this.obsolete_on;
			}
			else
			{
				this.obsoleteIcon.sprite = this.obsolete_off;
			}
			this.DeleteClassButton.interactable = this.shipDesign.CanDeleteDesign;
			CombatantListItemController.SetNoseImage(this.shipDesign, this.nose);
			CombatantListItemController.SetMidImage(this.shipDesign, this.hull);
			CombatantListItemController.SetTailImage(this.shipDesign, this.tail);
			CombatantListItemController.SetRadiatorImage(this.shipDesign, this.radiator);
			CombatantListItemController.SetDriveImage(this.shipDesign, this.drive);
		}

		// Token: 0x06004E4E RID: 20046 RVA: 0x0021B3F7 File Offset: 0x002195F7
		public void OnDesignClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.controller.OnDesignShipButtonClicked();
			this.controller.ShowDesignerTutorial();
			this.controller.LoadExistingShipTemplate(this.shipDesign);
		}

		// Token: 0x06004E4F RID: 20047 RVA: 0x0021B42C File Offset: 0x0021962C
		public void OnObsoleteDesignClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			SpaceObjectSelection.BlockSelectionFrame();
			GameControl.control.activePlayer.playerControl.StartAction(new ToggleObsoleteShipDesignAction(GameControl.control.activePlayer, this.shipDesign.dataName));
			if (this.obsoleteToggle.isOn)
			{
				this.obsoleteIcon.sprite = this.obsolete_on;
			}
			else
			{
				this.obsoleteIcon.sprite = this.obsolete_off;
			}
			if (this.controller.classListHideObsoleteToggle.isOn)
			{
				this.controller.UpdateShipClassListScreen();
			}
		}

		// Token: 0x06004E50 RID: 20048 RVA: 0x0021B4C8 File Offset: 0x002196C8
		public void OnDeleteClassClicked()
		{
			if (this.shipDesign.CanDeleteDesign)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
				SpaceObjectSelection.BlockSelectionFrame();
				GameControl.control.activePlayer.playerControl.StartAction(new DeleteShipDesignAction(GameControl.control.activePlayer, this.shipDesign));
				this.controller.UpdateShipClassListScreen();
			}
		}

		// Token: 0x06004E51 RID: 20049 RVA: 0x0021B527 File Offset: 0x00219727
		public void OnBuildClassClicked()
		{
			this.controller.shipClassListCanvas.enabled = false;
			this.controller.OpenConstructionManager();
			this.controller.UpdateConstructionManager(this.shipDesign);
		}

		// Token: 0x040031D1 RID: 12753
		private FleetsScreenController controller;

		// Token: 0x040031D2 RID: 12754
		public TMP_Text className;

		// Token: 0x040031D3 RID: 12755
		public TMP_Text hullName;

		// Token: 0x040031D4 RID: 12756
		private TISpaceShipTemplate shipDesign;

		// Token: 0x040031D5 RID: 12757
		public TMP_Text role;

		// Token: 0x040031D6 RID: 12758
		public TMP_Text mass;

		// Token: 0x040031D7 RID: 12759
		public TMP_Text acceleration;

		// Token: 0x040031D8 RID: 12760
		public TMP_Text DV;

		// Token: 0x040031D9 RID: 12761
		public TMP_Text buildCost;

		// Token: 0x040031DA RID: 12762
		public TMP_Text combatValue;

		// Token: 0x040031DB RID: 12763
		public TMP_Text assaultValue;

		// Token: 0x040031DC RID: 12764
		public TMP_Text numberInService;

		// Token: 0x040031DD RID: 12765
		public Button DeleteClassButton;

		// Token: 0x040031DE RID: 12766
		public TMP_Text buildButtonText;

		// Token: 0x040031DF RID: 12767
		public TMP_Text upgradeButtonText;

		// Token: 0x040031E0 RID: 12768
		public Toggle obsoleteToggle;

		// Token: 0x040031E1 RID: 12769
		public Image obsoleteIcon;

		// Token: 0x040031E2 RID: 12770
		public Sprite obsolete_on;

		// Token: 0x040031E3 RID: 12771
		public Sprite obsolete_off;

		// Token: 0x040031E4 RID: 12772
		public Image nose;

		// Token: 0x040031E5 RID: 12773
		public Image hull;

		// Token: 0x040031E6 RID: 12774
		public Image tail;

		// Token: 0x040031E7 RID: 12775
		public Image radiator;

		// Token: 0x040031E8 RID: 12776
		public Image drive;

		// Token: 0x040031E9 RID: 12777
		public GameObject altBackgroundObject;
	}
}
