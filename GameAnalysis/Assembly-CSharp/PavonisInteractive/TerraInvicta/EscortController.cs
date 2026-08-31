using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200057F RID: 1407
	public class EscortController : HumanShipController
	{
		// Token: 0x0600251B RID: 9499 RVA: 0x000C7D48 File Offset: 0x000C5F48
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			if (slot == 7)
			{
				return 0;
			}
			if (slot != 8)
			{
				return 0;
			}
			return 1;
		}
	}
}
