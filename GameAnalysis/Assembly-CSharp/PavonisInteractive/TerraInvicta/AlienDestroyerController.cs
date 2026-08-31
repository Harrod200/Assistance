using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200056F RID: 1391
	public class AlienDestroyerController : AlienShipController
	{
		// Token: 0x060024F1 RID: 9457 RVA: 0x000C6D64 File Offset: 0x000C4F64
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

		// Token: 0x060024F2 RID: 9458 RVA: 0x000C6DF4 File Offset: 0x000C4FF4
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				if (mount != Mount.OneNose)
				{
					return 2;
				}
				return 0;
			case 8:
				if (mount != Mount.OneNose)
				{
					return 2;
				}
				return 1;
			case 9:
				if (mount != Mount.TwoHullHoriz)
				{
					return 0;
				}
				return 2;
			case 12:
				if (mount != Mount.TwoHullHoriz)
				{
					return 1;
				}
				return 2;
			}
			Log.Warn("Couldn't find slot-to-mount data in AlienDestroyerController: " + slot.ToString() + " " + mount.ToString(), Array.Empty<object>());
			return 0;
		}
	}
}
