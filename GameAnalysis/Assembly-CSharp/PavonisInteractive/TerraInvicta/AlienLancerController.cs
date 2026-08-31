using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000575 RID: 1397
	public class AlienLancerController : AlienShipController
	{
		// Token: 0x06002503 RID: 9475 RVA: 0x000C7230 File Offset: 0x000C5430
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

		// Token: 0x06002504 RID: 9476 RVA: 0x000C72C0 File Offset: 0x000C54C0
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
				if (mount - Mount.OneHull > 1)
				{
				}
				return 0;
			case 14:
				switch (mount)
				{
				case Mount.OneHull:
					return 1;
				case Mount.TwoHullHoriz:
					return 0;
				case Mount.ThreeHullHoriz:
					return 1;
				}
				break;
			case 15:
				if (mount - Mount.OneHull <= 1)
				{
					return 2;
				}
				if (mount == Mount.ThreeHullHoriz)
				{
					return 1;
				}
				break;
			case 16:
				switch (mount)
				{
				case Mount.OneHull:
					return 3;
				case Mount.TwoHullHoriz:
					return 2;
				case Mount.ThreeHullHoriz:
					return 1;
				}
				break;
			}
			return 0;
		}
	}
}
