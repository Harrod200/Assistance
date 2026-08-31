using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200057A RID: 1402
	public class BattleshipController : HumanShipController
	{
		// Token: 0x06002511 RID: 9489 RVA: 0x000C7B90 File Offset: 0x000C5D90
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				if (mount == Mount.TwoNoseVert)
				{
					return 0;
				}
				return 2;
			case 8:
				if (mount == Mount.TwoNoseVert)
				{
					return 0;
				}
				return 1;
			case 9:
				return 0;
			case 10:
				return 1;
			case 12:
				return 2;
			case 13:
				return 3;
			case 16:
				return 4;
			case 17:
				return 5;
			}
			return 0;
		}
	}
}
