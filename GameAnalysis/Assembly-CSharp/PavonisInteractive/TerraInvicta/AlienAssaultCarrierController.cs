using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200056A RID: 1386
	public class AlienAssaultCarrierController : AlienShipController
	{
		// Token: 0x060024E2 RID: 9442 RVA: 0x000C682C File Offset: 0x000C4A2C
		public override List<GameObject> WhichRadiators(TISpaceShipTemplate ship)
		{
			List<GameObject> list = new List<GameObject>();
			float num = 0f;
			if (ship.powerPlantTemplate != null && ship.driveTemplate != null)
			{
				num = ship.radiatorTemplate.radiatorArea_m2(ship.wasteHeat_GW);
			}
			if (num < 800f)
			{
				list.Add(this.radiator1030);
				list.Add(this.radiator130);
			}
			else
			{
				list.Add(this.radiator1030);
				list.Add(this.radiator130);
				list.Add(this.radiator430);
				list.Add(this.radiator730);
			}
			return list;
		}

		// Token: 0x060024E3 RID: 9443 RVA: 0x000C68BC File Offset: 0x000C4ABC
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				switch (mount)
				{
				case Mount.OneHull:
					return 2;
				case Mount.TwoHullHoriz:
					return 2;
				case Mount.ThreeHullHoriz:
					return 2;
				}
				break;
			case 9:
				switch (mount)
				{
				case Mount.OneHull:
					return 5;
				case Mount.TwoHullHoriz:
					return 5;
				case Mount.ThreeHullHoriz:
					return 5;
				}
				break;
			case 10:
				switch (mount)
				{
				case Mount.OneHull:
					return 1;
				case Mount.TwoHullHoriz:
					return 1;
				case Mount.ThreeHullHoriz:
					return 2;
				}
				break;
			case 12:
				switch (mount)
				{
				case Mount.OneHull:
					return 4;
				case Mount.TwoHullHoriz:
					return 4;
				case Mount.ThreeHullHoriz:
					return 5;
				}
				break;
			case 13:
				switch (mount)
				{
				case Mount.OneHull:
					return 0;
				case Mount.TwoHullHoriz:
					return 1;
				case Mount.ThreeHullHoriz:
					return 2;
				}
				break;
			case 15:
				switch (mount)
				{
				case Mount.OneHull:
					return 3;
				case Mount.TwoHullHoriz:
					return 4;
				case Mount.ThreeHullHoriz:
					return 5;
				}
				break;
			}
			Log.Warn("Couldn't find slot-to-mount data in AlienAssaultCarrierController", Array.Empty<object>());
			return 0;
		}
	}
}
