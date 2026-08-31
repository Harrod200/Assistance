using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200057D RID: 1405
	public class DestroyerController : HumanShipController
	{
		// Token: 0x06002517 RID: 9495 RVA: 0x000C7C68 File Offset: 0x000C5E68
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				if (mount == Mount.OneNose || mount - Mount.TwoNoseHoriz > 1)
				{
					return 0;
				}
				return 2;
			case 8:
				if (mount == Mount.OneNose || mount - Mount.TwoNoseHoriz > 1)
				{
					return 1;
				}
				return 2;
			case 9:
				return 1;
			case 11:
				return 0;
			}
			return 0;
		}
	}
}
