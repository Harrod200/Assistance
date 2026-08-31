using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200057B RID: 1403
	public class CorvetteController : HumanShipController
	{
		// Token: 0x06002513 RID: 9491 RVA: 0x000C7BFA File Offset: 0x000C5DFA
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			if (slot != 7)
			{
			}
			return 0;
		}
	}
}
