using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000570 RID: 1392
	public class AlienDreadnoughtController : AlienShipController
	{
		// Token: 0x060024F4 RID: 9460 RVA: 0x000C6E79 File Offset: 0x000C5079
		public override List<GameObject> WhichRadiators(TISpaceShipTemplate ship)
		{
			return new List<GameObject> { this.radiator1030, this.radiator130, this.radiator430, this.radiator730 };
		}

		// Token: 0x060024F5 RID: 9461 RVA: 0x000C6EB0 File Offset: 0x000C50B0
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				if (mount == Mount.OneNose)
				{
					return 4;
				}
				if (mount - Mount.TwoNoseHoriz <= 3)
				{
					return 0;
				}
				break;
			case 8:
				if (mount == Mount.OneNose)
				{
					return 3;
				}
				if (mount - Mount.TwoNoseHoriz <= 3)
				{
					return 0;
				}
				break;
			case 9:
				if (mount == Mount.OneNose)
				{
					return 1;
				}
				if (mount - Mount.TwoNoseHoriz <= 3)
				{
					return 0;
				}
				break;
			case 10:
				if (mount == Mount.OneNose)
				{
					return 2;
				}
				if (mount - Mount.TwoNoseHoriz <= 3)
				{
					return 0;
				}
				break;
			case 19:
				if (mount - Mount.OneHull <= 3)
				{
					return 10;
				}
				if (mount == Mount.FourHull)
				{
					return 11;
				}
				break;
			case 20:
				if (mount - Mount.OneHull <= 3)
				{
					return 9;
				}
				if (mount == Mount.FourHull)
				{
					return 11;
				}
				break;
			case 21:
				if (mount - Mount.OneHull <= 3)
				{
					return 1;
				}
				if (mount == Mount.FourHull)
				{
					return 2;
				}
				break;
			case 22:
				if (mount - Mount.OneHull <= 3)
				{
					return 0;
				}
				if (mount == Mount.FourHull)
				{
					return 2;
				}
				break;
			case 23:
				if (mount - Mount.OneHull <= 3)
				{
					return 4;
				}
				if (mount == Mount.FourHull)
				{
					return 5;
				}
				break;
			case 24:
				if (mount - Mount.OneHull <= 3)
				{
					return 3;
				}
				if (mount == Mount.FourHull)
				{
					return 5;
				}
				break;
			case 25:
				if (mount - Mount.OneHull <= 3)
				{
					return 7;
				}
				if (mount == Mount.FourHull)
				{
					return 8;
				}
				break;
			case 26:
				if (mount - Mount.OneHull <= 3)
				{
					return 6;
				}
				if (mount == Mount.FourHull)
				{
					return 8;
				}
				break;
			}
			Log.Warn("Couldn't find slot-to-mount data in AlienDreadnoughtController: " + slot.ToString() + mount.ToString(), Array.Empty<object>());
			return 0;
		}
	}
}
