using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200057E RID: 1406
	public class DreadnoughtController : HumanShipController
	{
		// Token: 0x06002519 RID: 9497 RVA: 0x000C7CC0 File Offset: 0x000C5EC0
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				return 0;
			case 8:
				if (mount == Mount.ThreeNoseAngle)
				{
					return 0;
				}
				return 2;
			case 9:
				if (mount == Mount.ThreeNoseAngle)
				{
					return 0;
				}
				return 1;
			case 16:
				return 6;
			case 17:
				return 7;
			case 18:
				return 0;
			case 19:
				return 1;
			case 20:
				return 2;
			case 21:
				return 3;
			case 22:
				return 4;
			case 23:
				return 5;
			}
			return 0;
		}
	}
}
