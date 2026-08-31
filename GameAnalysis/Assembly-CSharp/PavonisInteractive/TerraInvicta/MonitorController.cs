using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000584 RID: 1412
	public class MonitorController : HumanShipController
	{
		// Token: 0x0600252D RID: 9517 RVA: 0x000C817B File Offset: 0x000C637B
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				return 3;
			case 9:
				return 2;
			case 10:
				if (mount == Mount.TwoHullHoriz)
				{
					return 3;
				}
				return 1;
			case 12:
				if (mount == Mount.TwoHullHoriz)
				{
					return 2;
				}
				return 0;
			}
			return 0;
		}
	}
}
