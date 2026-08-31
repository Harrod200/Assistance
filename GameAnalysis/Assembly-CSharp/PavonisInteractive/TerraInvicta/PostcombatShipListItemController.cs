using System;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008BD RID: 2237
	public class PostcombatShipListItemController : MonoBehaviour
	{
		// Token: 0x06005574 RID: 21876 RVA: 0x0026D8AC File Offset: 0x0026BAAC
		public void SetListItem(CombatRecord.SingleAssetCombatRecord item, bool combatAutoDestroyHab)
		{
			TIGameState asset = item.asset;
			if (asset != null && asset.isSpaceShipState)
			{
				TISpaceShipState ref_ship = item.asset.ref_ship;
				if (ref_ship.ShipDestroyed())
				{
					this.itemName.SetText(ref_ship.displayName);
				}
				else
				{
					this.itemName.SetText(ref_ship.NameWithDamageIcons());
				}
				CombatantListItemController.SetNoseImage(ref_ship, this.nose);
				CombatantListItemController.SetMidImage(ref_ship, this.hull);
				CombatantListItemController.SetTailImage(ref_ship, this.tail);
				this.SetShipImagesDestroyed(ref_ship.ShipDestroyed());
				if (ref_ship.isAlien)
				{
					this.radiators.enabled = false;
					this.drive.enabled = false;
					this.nose.enabled = true;
					this.tail.enabled = true;
					this.hull.enabled = true;
				}
				else
				{
					CombatantListItemController.SetRadiatorImage(ref_ship, this.radiators);
					CombatantListItemController.SetDriveImage(ref_ship, this.drive);
					this.radiators.enabled = true;
					this.drive.enabled = true;
					this.nose.enabled = true;
					this.tail.enabled = true;
					this.hull.enabled = true;
				}
				this.tooltip.enabled = true;
				this.tooltip.SetDelegate("BodyText", () => item.assetSummary);
			}
			else
			{
				this.itemName.SetText(item.assetName);
				this.radiators.enabled = true;
				this.drive.enabled = false;
				this.nose.enabled = false;
				this.tail.enabled = false;
				this.hull.enabled = false;
				CombatantListItemController.SetHabImage(item.asset.ref_hab, this.radiators);
				this.tooltip.enabled = true;
				this.tooltip.SetDelegate("BodyText", () => item.assetSummary);
			}
			if (item.asset.isHabState && combatAutoDestroyHab)
			{
				this.itemStatus.SetText(Loc.T(new StringBuilder("UI.SpaceCombat.Status.").Append(SingleAssetCombatOutcome.Destroyed.ToString()).ToString()));
				return;
			}
			this.itemStatus.SetText(Loc.T(new StringBuilder("UI.SpaceCombat.Status.").Append(item.outcome.ToString()).ToString()));
		}

		// Token: 0x06005575 RID: 21877 RVA: 0x0026DB30 File Offset: 0x0026BD30
		private void SetShipImagesDestroyed(bool value)
		{
			if (!value)
			{
				this.radiators.color = Color.white;
				this.drive.color = Color.white;
				this.nose.color = Color.white;
				this.tail.color = Color.white;
				this.hull.color = Color.white;
				return;
			}
			Color color = new Color(0.7f, 0.7f, 0.7f, 1f);
			this.radiators.color = color;
			this.drive.color = color;
			this.nose.color = color;
			this.tail.color = color;
			this.hull.color = color;
		}

		// Token: 0x04003BC7 RID: 15303
		public Image nose;

		// Token: 0x04003BC8 RID: 15304
		public Image hull;

		// Token: 0x04003BC9 RID: 15305
		public Image tail;

		// Token: 0x04003BCA RID: 15306
		public Image radiators;

		// Token: 0x04003BCB RID: 15307
		public Image drive;

		// Token: 0x04003BCC RID: 15308
		public TMP_Text itemName;

		// Token: 0x04003BCD RID: 15309
		public TMP_Text itemStatus;

		// Token: 0x04003BCE RID: 15310
		public TooltipTrigger tooltip;
	}
}
