using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000578 RID: 1400
	public class AlienTitanController : AlienShipController
	{
		// Token: 0x0600250C RID: 9484 RVA: 0x000C7828 File Offset: 0x000C5A28
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

		// Token: 0x0600250D RID: 9485 RVA: 0x000C78B8 File Offset: 0x000C5AB8
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				switch (mount)
				{
				case Mount.OneNose:
					return 4;
				case Mount.TwoNoseHoriz:
					return 4;
				case Mount.TwoNoseVert:
					return 4;
				case Mount.ThreeNoseAngle:
					return 7;
				case Mount.FourNose:
					return 0;
				}
				break;
			case 8:
				switch (mount)
				{
				case Mount.OneNose:
					return 3;
				case Mount.TwoNoseHoriz:
					return 3;
				case Mount.TwoNoseVert:
					return 4;
				case Mount.ThreeNoseAngle:
					return 8;
				case Mount.FourNose:
					return 0;
				}
				break;
			case 9:
				switch (mount)
				{
				case Mount.OneNose:
					return 2;
				case Mount.TwoNoseHoriz:
					return 2;
				case Mount.TwoNoseVert:
					return 2;
				case Mount.ThreeNoseAngle:
					return 7;
				case Mount.FourNose:
					return 0;
				}
				break;
			case 10:
				switch (mount)
				{
				case Mount.OneNose:
					return 1;
				case Mount.TwoNoseHoriz:
					return 1;
				case Mount.TwoNoseVert:
					return 2;
				case Mount.ThreeNoseAngle:
					return 8;
				case Mount.FourNose:
					return 0;
				}
				break;
			case 11:
				switch (mount)
				{
				case Mount.OneNose:
					return 6;
				case Mount.TwoNoseHoriz:
					return 2;
				case Mount.TwoNoseVert:
					return 6;
				case Mount.ThreeNoseAngle:
					return 7;
				case Mount.FourNose:
					return 0;
				}
				break;
			case 12:
				switch (mount)
				{
				case Mount.OneNose:
					return 5;
				case Mount.TwoNoseHoriz:
					return 1;
				case Mount.TwoNoseVert:
					return 6;
				case Mount.ThreeNoseAngle:
					return 8;
				case Mount.FourNose:
					return 0;
				}
				break;
			case 13:
				if (mount == Mount.OneHull)
				{
					return 0;
				}
				if (mount - Mount.TwoHullHoriz <= 3)
				{
					return 1;
				}
				break;
			case 14:
				if (mount - Mount.OneHull <= 4)
				{
					return 1;
				}
				break;
			case 15:
				switch (mount)
				{
				case Mount.OneHull:
					return 2;
				case Mount.TwoHullHoriz:
				case Mount.TwoHullVert:
				case Mount.ThreeHullHoriz:
					return 3;
				case Mount.FourHull:
					return 1;
				}
				break;
			case 16:
				if (mount - Mount.OneHull <= 3)
				{
					return 3;
				}
				if (mount == Mount.FourHull)
				{
					return 1;
				}
				break;
			case 17:
				if (mount == Mount.OneHull)
				{
					return 4;
				}
				if (mount - Mount.TwoHullHoriz <= 3)
				{
					return 5;
				}
				break;
			case 18:
				if (mount - Mount.OneHull <= 4)
				{
					return 5;
				}
				break;
			case 19:
				switch (mount)
				{
				case Mount.OneHull:
					return 6;
				case Mount.TwoHullHoriz:
				case Mount.TwoHullVert:
				case Mount.ThreeHullHoriz:
					return 7;
				case Mount.FourHull:
					return 5;
				}
				break;
			case 20:
				if (mount - Mount.OneHull <= 3)
				{
					return 7;
				}
				if (mount == Mount.FourHull)
				{
					return 5;
				}
				break;
			}
			Log.Warn("Couldn't find slot-to-mount data in AlienTitanController: " + slot.ToString() + mount.ToString(), Array.Empty<object>());
			return 0;
		}
	}
}
