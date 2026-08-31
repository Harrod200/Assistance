using System;
using ModelShark;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008CA RID: 2250
	public class CombatShipCouncilorGridItemController : MonoBehaviour
	{
		// Token: 0x06005655 RID: 22101 RVA: 0x0027806C File Offset: 0x0027626C
		public void SetGridItem(CouncilorView councilor)
		{
			GameControl.assetLoader.LoadAssetForImageAssignment(councilor.mapIconResourcePathCurrent, this.icon);
			if (this.tooltip == null)
			{
				return;
			}
			this.tooltip.SetDelegate("BodyText", () => councilor.councilor.GetDisplayName(GameControl.control.activePlayer));
		}

		// Token: 0x04003D63 RID: 15715
		public Image icon;

		// Token: 0x04003D64 RID: 15716
		public TooltipTrigger tooltip;
	}
}
