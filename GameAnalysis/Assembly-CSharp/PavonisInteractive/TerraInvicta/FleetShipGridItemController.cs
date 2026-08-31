using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008D3 RID: 2259
	public class FleetShipGridItemController : MonoBehaviour
	{
		// Token: 0x06005694 RID: 22164 RVA: 0x00279BA4 File Offset: 0x00277DA4
		public string BuildShipTooltip(TISpaceShipState ship)
		{
			return ship.GetDisplayName(GameControl.control.activePlayer);
		}

		// Token: 0x06005695 RID: 22165 RVA: 0x00279BB8 File Offset: 0x00277DB8
		public void SetGridItem(TISpaceShipState ship, TISpaceFleetState fleet, bool enableTip = true)
		{
			this.shipState = ship;
			if (ship == null)
			{
				this.theresMoreText.SetText(Loc.T("UI.Space.Fleet.MoreShips", new object[] { fleet.ships.Count - 14 }));
				this.theresMoreText.enabled = true;
				this.tooltip.enabled = false;
				return;
			}
			CombatantListItemController.SetNoseImage(ship, this.nose);
			CombatantListItemController.SetMidImage(ship, this.hull);
			CombatantListItemController.SetTailImage(ship, this.tail);
			if (ship.isAlien)
			{
				this.radiators.enabled = false;
				this.drive.enabled = false;
			}
			else
			{
				CombatantListItemController.SetRadiatorImage(ship, this.radiators);
				CombatantListItemController.SetDriveImage(ship, this.drive);
				this.radiators.enabled = true;
				this.drive.enabled = true;
			}
			bool hideDistance = ship.isAlien && TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(GameStateManager.Sol(), ship.fleet) > 149597870700.0 * (double)(1.02f + TIEffectsState.SumEffectsModifiers(Context.OuterExplorationRange_AU, GameControl.control.activePlayer, 1.02f, null));
			this.tooltip.SetDelegate("BodyText", () => ship.template.quickSummary(ship.isAlien && !GameControl.control.activePlayer.finishedProjectNames.Contains("Project_TheirWarships"), ship, hideDistance, false, ship.faction == GameControl.control.activePlayer));
			this.tooltip.enabled = enableTip;
			this.theresMoreText.enabled = false;
		}

		// Token: 0x06005696 RID: 22166 RVA: 0x00279D58 File Offset: 0x00277F58
		public void SetGridItem_Alt(TISpaceShipState ship, ParameterizedTextField.BuildStringOnTooltipHover del, bool enableTip = true)
		{
			CombatantListItemController.SetNoseImage(ship, this.nose);
			CombatantListItemController.SetMidImage(ship, this.hull);
			CombatantListItemController.SetTailImage(ship, this.tail);
			if (ship.isAlien || ship.hull.simpleHull)
			{
				this.radiators.enabled = false;
				this.drive.enabled = false;
			}
			else
			{
				CombatantListItemController.SetRadiatorImage(ship, this.radiators);
				CombatantListItemController.SetDriveImage(ship, this.drive);
				this.radiators.enabled = true;
				this.drive.enabled = true;
			}
			this.tooltip.SetDelegate("BodyText", del);
			this.tooltip.enabled = enableTip;
		}

		// Token: 0x06005697 RID: 22167 RVA: 0x00279E05 File Offset: 0x00278005
		public void OnClickItem()
		{
			if (TIGameState.Valid(this.shipState))
			{
				SoundEffectController.PlaySelectSound(this.shipState.fleet);
				GameControl.eventManager.TriggerEvent(new ShipDetailRequested(this.shipState), null, Array.Empty<object>());
			}
		}

		// Token: 0x04003DAB RID: 15787
		public Image nose;

		// Token: 0x04003DAC RID: 15788
		public Image hull;

		// Token: 0x04003DAD RID: 15789
		public Image tail;

		// Token: 0x04003DAE RID: 15790
		public Image radiators;

		// Token: 0x04003DAF RID: 15791
		public Image drive;

		// Token: 0x04003DB0 RID: 15792
		public TMP_Text theresMoreText;

		// Token: 0x04003DB1 RID: 15793
		public TooltipTrigger tooltip;

		// Token: 0x04003DB2 RID: 15794
		private TISpaceShipState shipState;
	}
}
