using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000573 RID: 1395
	public class AlienFrigateController : AlienShipController
	{
		// Token: 0x060024FD RID: 9469 RVA: 0x000C70DC File Offset: 0x000C52DC
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

		// Token: 0x060024FE RID: 9470 RVA: 0x000C716A File Offset: 0x000C536A
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			if (slot <= 9)
			{
				if (slot == 7)
				{
					return 0;
				}
				if (slot == 9)
				{
					return 0;
				}
			}
			else
			{
				if (slot == 12)
				{
					return 1;
				}
				if (slot == 14)
				{
					return 2;
				}
			}
			return 0;
		}
	}
}
