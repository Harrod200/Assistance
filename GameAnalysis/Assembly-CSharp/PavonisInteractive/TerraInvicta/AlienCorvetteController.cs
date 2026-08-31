using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200056D RID: 1389
	public class AlienCorvetteController : AlienShipController
	{
		// Token: 0x060024EB RID: 9451 RVA: 0x000C6B54 File Offset: 0x000C4D54
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
				list.Add(this.radiator6);
			}
			return list;
		}

		// Token: 0x060024EC RID: 9452 RVA: 0x000C6BD6 File Offset: 0x000C4DD6
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			return 0;
		}
	}
}
