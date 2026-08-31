using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200057C RID: 1404
	public class CruiserController : HumanShipController
	{
		// Token: 0x06002515 RID: 9493 RVA: 0x000C7C10 File Offset: 0x000C5E10
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				if (mount == Mount.TwoNoseVert)
				{
					return 0;
				}
				return 1;
			case 8:
				return 2;
			case 10:
				return 0;
			case 13:
				return 1;
			case 15:
				return 2;
			}
			return 0;
		}
	}
}
