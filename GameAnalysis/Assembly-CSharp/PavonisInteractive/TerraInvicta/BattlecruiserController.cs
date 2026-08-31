using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000579 RID: 1401
	public class BattlecruiserController : HumanShipController
	{
		// Token: 0x0600250F RID: 9487 RVA: 0x000C7ADC File Offset: 0x000C5CDC
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				switch (mount)
				{
				case Mount.OneNose:
					return 1;
				}
				return 0;
			case 8:
				switch (mount)
				{
				case Mount.TwoHullVert:
				case Mount.TwoNoseHoriz:
				case Mount.ThreeNoseAngle:
					return 0;
				default:
					return 3;
				}
				break;
			case 9:
				switch (mount)
				{
				case Mount.TwoHullVert:
				case Mount.TwoNoseHoriz:
				case Mount.ThreeNoseAngle:
					return 0;
				default:
					return 2;
				}
				break;
			case 11:
				return 0;
			case 14:
				return 1;
			}
			return 0;
		}
	}
}
