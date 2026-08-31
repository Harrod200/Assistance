using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000581 RID: 1409
	public class FrigateController : HumanShipController
	{
		// Token: 0x06002527 RID: 9511 RVA: 0x000C8074 File Offset: 0x000C6274
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				return 0;
			case 8:
				return 0;
			case 11:
				return 1;
			}
			return 0;
		}
	}
}
