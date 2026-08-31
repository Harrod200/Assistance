using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200056B RID: 1387
	public class AlienBattlecruiserController : AlienShipController
	{
		// Token: 0x060024E5 RID: 9445 RVA: 0x000C69D8 File Offset: 0x000C4BD8
		public override List<GameObject> WhichRadiators(TISpaceShipTemplate ship)
		{
			return new List<GameObject> { this.radiator1030, this.radiator130, this.radiator430, this.radiator730 };
		}

		// Token: 0x060024E6 RID: 9446 RVA: 0x000C6A10 File Offset: 0x000C4C10
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				return 0;
			case 8:
				if (mount == Mount.TwoNoseHoriz || mount == Mount.ThreeNoseAngle)
				{
					return 0;
				}
				return 2;
			case 9:
				if (mount == Mount.TwoNoseHoriz || mount == Mount.ThreeNoseAngle)
				{
					return 0;
				}
				return 1;
			case 10:
				return 0;
			case 13:
				return 1;
			case 16:
				return 2;
			}
			Log.Warn("Couldn't find slot-to-mount data in AlienBattlecruiserController", Array.Empty<object>());
			return 0;
		}
	}
}
