using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000583 RID: 1411
	public class LancerController : HumanShipController
	{
		// Token: 0x0600252B RID: 9515 RVA: 0x000C80B4 File Offset: 0x000C62B4
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			switch (slot)
			{
			case 7:
				if (mount - Mount.ThreeNoseAngle <= 1)
				{
					return 0;
				}
				return 4;
			case 8:
				switch (mount)
				{
				case Mount.TwoNoseHoriz:
				case Mount.ThreeNoseAngle:
				case Mount.FourNose:
					return 0;
				}
				return 2;
			case 9:
				switch (mount)
				{
				case Mount.TwoNoseHoriz:
				case Mount.ThreeNoseAngle:
				case Mount.FourNose:
					return 0;
				}
				return 3;
			case 10:
				switch (mount)
				{
				case Mount.TwoNoseHoriz:
				case Mount.ThreeNoseAngle:
				case Mount.FourNose:
					return 0;
				}
				return 1;
			case 11:
				return 0;
			case 14:
				return 1;
			case 18:
				return 2;
			}
			return 0;
		}
	}
}
