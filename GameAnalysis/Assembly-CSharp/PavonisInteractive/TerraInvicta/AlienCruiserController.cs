using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200056E RID: 1390
	public class AlienCruiserController : AlienShipController
	{
		// Token: 0x060024EE RID: 9454 RVA: 0x000C6BE4 File Offset: 0x000C4DE4
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

		// Token: 0x060024EF RID: 9455 RVA: 0x000C6C74 File Offset: 0x000C4E74
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				if (mount != Mount.OneNose)
				{
				}
				return 0;
			case 8:
				if (mount == Mount.OneNose)
				{
					return 1;
				}
				if (mount != Mount.TwoNoseVert)
				{
					return 0;
				}
				return 0;
			case 9:
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
				break;
			case 10:
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
				}
				break;
			case 11:
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
				}
				break;
			case 12:
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
				}
				break;
			}
			return 0;
		}
	}
}
