using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200058D RID: 1421
	public class TitanController : HumanShipController
	{
		// Token: 0x060025C5 RID: 9669 RVA: 0x000CCF58 File Offset: 0x000CB158
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				if (mount - Mount.TwoNoseHoriz <= 3)
				{
					return 0;
				}
				return 1;
			case 8:
				if (mount - Mount.TwoNoseHoriz <= 3)
				{
					return 0;
				}
				return 3;
			case 9:
				if (mount - Mount.TwoNoseHoriz <= 3)
				{
					return 0;
				}
				return 2;
			case 10:
				if (mount - Mount.TwoNoseHoriz <= 3)
				{
					return 0;
				}
				return 4;
			case 12:
				return 0;
			case 13:
				return 1;
			case 16:
				return 2;
			case 17:
				return 3;
			case 20:
				return 4;
			case 21:
				return 5;
			}
			return 0;
		}
	}
}
