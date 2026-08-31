using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000574 RID: 1396
	public class AlienGunshipController : AlienShipController
	{
		// Token: 0x06002500 RID: 9472 RVA: 0x000C719C File Offset: 0x000C539C
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

		// Token: 0x06002501 RID: 9473 RVA: 0x000C721E File Offset: 0x000C541E
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			return 0;
		}
	}
}
