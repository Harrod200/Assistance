using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000577 RID: 1399
	public class AlienMothershipController : AlienShipController
	{
		// Token: 0x06002509 RID: 9481 RVA: 0x000C75C4 File Offset: 0x000C57C4
		public override List<GameObject> WhichRadiators(TISpaceShipTemplate ship)
		{
			return new List<GameObject> { this.radiator1030, this.radiator130, this.radiator430, this.radiator730, this.radiator12, this.radiator3, this.radiator4, this.radiator6, this.radiator8, this.radiator9 };
		}

		// Token: 0x0600250A RID: 9482 RVA: 0x000C7650 File Offset: 0x000C5850
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
					return 2;
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
					return 3;
				}
				if (mount - Mount.TwoNoseHoriz <= 3)
				{
					return 0;
				}
				break;
			case 11:
				if (mount - Mount.OneHull <= 4)
				{
					return 0;
				}
				break;
			case 12:
				if (mount - Mount.OneHull <= 4)
				{
					return 1;
				}
				break;
			case 13:
				if (mount - Mount.OneHull <= 4)
				{
					return 2;
				}
				break;
			case 14:
				if (mount - Mount.OneHull <= 4)
				{
					return 3;
				}
				break;
			case 15:
				if (mount - Mount.OneHull <= 4)
				{
					return 16;
				}
				break;
			case 16:
				if (mount - Mount.OneHull <= 4)
				{
					return 17;
				}
				break;
			case 17:
				if (mount - Mount.OneHull <= 4)
				{
					return 18;
				}
				break;
			case 18:
				if (mount - Mount.OneHull <= 4)
				{
					return 19;
				}
				break;
			case 19:
				if (mount - Mount.OneHull <= 3)
				{
					return 13;
				}
				if (mount == Mount.FourHull)
				{
					return 14;
				}
				break;
			case 20:
				if (mount - Mount.OneHull <= 3)
				{
					return 15;
				}
				if (mount == Mount.FourHull)
				{
					return 14;
				}
				break;
			case 21:
				if (mount - Mount.OneHull <= 3)
				{
					return 4;
				}
				if (mount == Mount.FourHull)
				{
					return 5;
				}
				break;
			case 22:
				if (mount - Mount.OneHull <= 3)
				{
					return 6;
				}
				if (mount == Mount.FourHull)
				{
					return 5;
				}
				break;
			case 23:
				if (mount - Mount.OneHull <= 3)
				{
					return 7;
				}
				if (mount == Mount.FourHull)
				{
					return 8;
				}
				break;
			case 24:
				if (mount - Mount.OneHull <= 3)
				{
					return 9;
				}
				if (mount == Mount.FourHull)
				{
					return 8;
				}
				break;
			case 25:
				if (mount - Mount.OneHull <= 3)
				{
					return 10;
				}
				if (mount == Mount.FourHull)
				{
					return 11;
				}
				break;
			case 26:
				if (mount - Mount.OneHull <= 3)
				{
					return 12;
				}
				if (mount == Mount.FourHull)
				{
					return 11;
				}
				break;
			}
			Log.Warn("Couldn't find slot-to-mount data in AlienMothershipController " + slot.ToString() + " " + mount.ToString(), Array.Empty<object>());
			return 0;
		}
	}
}
