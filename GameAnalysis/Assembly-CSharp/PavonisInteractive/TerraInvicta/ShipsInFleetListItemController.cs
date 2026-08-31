using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008D8 RID: 2264
	public class ShipsInFleetListItemController : MonoBehaviour
	{
		// Token: 0x060056A9 RID: 22185 RVA: 0x0027A748 File Offset: 0x00278948
		public void SetListItem(TISpaceShipState shipState)
		{
			this.ship = shipState;
			this.shipName.SetText(shipState.NameWithDamageIcons());
			this.shipClass.SetText(shipState.template.fullClassName);
			this.acceleration.SetText(FleetsScreenController.accelerationStr((double)shipState.cruiseAcceleration_gs, false, false, true));
			this.UpdateDVData(shipState);
			List<CouncilorView> list = shipState.CouncilorViewsPresentAndKnownToFaction(GameControl.control.activePlayer);
			this.councilorGrid.SetListSize<CombatShipCouncilorGridItemController>(list.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.councilorGrid.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ShipsInFleetListItemController.<>o__8.<>p__0 == null)
					{
						ShipsInFleetListItemController.<>o__8.<>p__0 = CallSite<Func<CallSite, object, CombatShipCouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CombatShipCouncilorGridItemController), typeof(ShipsInFleetListItemController)));
					}
					ShipsInFleetListItemController.<>o__8.<>p__0.Target(ShipsInFleetListItemController.<>o__8.<>p__0, enumerator.Current).SetGridItem(list[num++]);
				}
			}
			if (list.Count > 2)
			{
				this.councilorGrid.gameObject.GetComponent<GridLayoutGroup>().spacing = new Vector2(0f, (float)(list.Count * -4));
			}
			bool hideDistance = this.ship.isAlien && TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(GameStateManager.Sol(), this.ship.fleet) > 149597870700.0 * (double)(1.02f + TIEffectsState.SumEffectsModifiers(Context.OuterExplorationRange_AU, GameControl.control.activePlayer, 1.02f, null));
			this.shipTip.SetDelegate("BodyText", () => this.ship.template.quickSummary(!GameControl.control.activePlayer.finishedProjectNames.Contains("Project_TheirWarships"), this.ship, hideDistance, false, this.ship.faction == GameControl.control.activePlayer));
		}

		// Token: 0x060056AA RID: 22186 RVA: 0x0027A908 File Offset: 0x00278B08
		public void UpdateDVData(TISpaceShipState shipState)
		{
			this.DV.SetText(Loc.T("UI.Fleets.DVValue", new object[]
			{
				TIUtilities.FormatBigOrSmallNumber(shipState.currentDeltaV_kps, 1, 3, 0, false, false),
				TIUtilities.FormatBigOrSmallNumber(shipState.currentMaxDeltaV_kps, 1, 3, 0, false, false)
			}));
			string text = shipState.drive.PropellantIcons(true, shipState.faction);
			this.propellantResources.SetText(text);
			if (!string.IsNullOrEmpty(text))
			{
				this.propellantResources.gameObject.SetActive(true);
				this.DV.verticalAlignment = VerticalAlignmentOptions.Bottom;
				return;
			}
			this.propellantResources.gameObject.SetActive(false);
			this.DV.verticalAlignment = VerticalAlignmentOptions.Middle;
		}

		// Token: 0x060056AB RID: 22187 RVA: 0x0027A9C0 File Offset: 0x00278BC0
		public void OnShipsInFleetListItemClicked()
		{
			SoundEffectController.PlaySelectSound(this.ship.fleet);
			GameControl.eventManager.TriggerEvent(new ShipDetailRequested(this.ship), null, Array.Empty<object>());
		}

		// Token: 0x04003DD1 RID: 15825
		private TISpaceShipState ship;

		// Token: 0x04003DD2 RID: 15826
		public TMP_Text shipName;

		// Token: 0x04003DD3 RID: 15827
		public TMP_Text shipClass;

		// Token: 0x04003DD4 RID: 15828
		public TMP_Text DV;

		// Token: 0x04003DD5 RID: 15829
		public TMP_Text propellantResources;

		// Token: 0x04003DD6 RID: 15830
		public TMP_Text acceleration;

		// Token: 0x04003DD7 RID: 15831
		public ListManagerBase councilorGrid;

		// Token: 0x04003DD8 RID: 15832
		public TooltipTrigger shipTip;
	}
}
