using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200056C RID: 1388
	public class AlienBattleshipController : AlienShipController
	{
		// Token: 0x060024E8 RID: 9448 RVA: 0x000C6A8B File Offset: 0x000C4C8B
		public override List<GameObject> WhichRadiators(TISpaceShipTemplate ship)
		{
			return new List<GameObject> { this.radiator1030, this.radiator130, this.radiator430, this.radiator730 };
		}

		// Token: 0x060024E9 RID: 9449 RVA: 0x000C6AC4 File Offset: 0x000C4CC4
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				if (mount == Mount.OneNose)
				{
					return 2;
				}
				return 0;
			case 8:
				if (mount == Mount.OneNose)
				{
					return 1;
				}
				return 0;
			case 9:
				return 0;
			case 10:
				return 1;
			case 12:
				return 2;
			case 13:
				return 3;
			case 16:
				return 4;
			case 17:
				return 5;
			}
			Log.Warn("Couldn't find slot-to-mount data in AlienBattleshipController:" + slot.ToString() + mount.ToString(), Array.Empty<object>());
			return 0;
		}
	}
}
