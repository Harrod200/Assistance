using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000571 RID: 1393
	public class AlienEscortController : AlienShipController
	{
		// Token: 0x060024F7 RID: 9463 RVA: 0x000C7020 File Offset: 0x000C5220
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

		// Token: 0x060024F8 RID: 9464 RVA: 0x000C70AE File Offset: 0x000C52AE
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			if (slot == 7 || slot != 8)
			{
				return 0;
			}
			if (mount == Mount.TwoHullHoriz)
			{
				return 0;
			}
			return 1;
		}
	}
}
