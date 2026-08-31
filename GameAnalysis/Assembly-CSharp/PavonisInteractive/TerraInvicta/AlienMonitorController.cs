using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000576 RID: 1398
	public class AlienMonitorController : AlienShipController
	{
		// Token: 0x06002506 RID: 9478 RVA: 0x000C7468 File Offset: 0x000C5668
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
				list.Add(this.radiator730);
				list.Add(this.radiator430);
			}
			return list;
		}

		// Token: 0x06002507 RID: 9479 RVA: 0x000C74F8 File Offset: 0x000C56F8
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				return 0;
			case 9:
				switch (mount)
				{
				case Mount.OneHull:
					return 1;
				case Mount.TwoHullHoriz:
				case Mount.ThreeHullHoriz:
					return 1;
				case Mount.TwoHullVert:
					return 0;
				case Mount.FourHull:
					return 1;
				default:
					return 0;
				}
				break;
			case 10:
				switch (mount)
				{
				case Mount.OneHull:
					return 2;
				case Mount.TwoHullHoriz:
				case Mount.ThreeHullHoriz:
					return 1;
				case Mount.TwoHullVert:
					return 2;
				case Mount.FourHull:
					return 1;
				default:
					return 0;
				}
				break;
			case 11:
				switch (mount)
				{
				case Mount.OneHull:
					return 3;
				case Mount.TwoHullHoriz:
				case Mount.ThreeHullHoriz:
					return 1;
				case Mount.TwoHullVert:
					return 2;
				case Mount.FourHull:
					return 1;
				default:
					return 0;
				}
				break;
			}
			switch (mount)
			{
			case Mount.OneHull:
				return 0;
			case Mount.TwoHullHoriz:
			case Mount.TwoHullVert:
			case Mount.ThreeHullHoriz:
				return 0;
			case Mount.FourHull:
				return 1;
			}
			return 0;
		}
	}
}
