using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000572 RID: 1394
	public class AlienFighterController : AlienShipController
	{
		// Token: 0x060024FA RID: 9466 RVA: 0x000C70C9 File Offset: 0x000C52C9
		public override List<GameObject> WhichRadiators(TISpaceShipTemplate ship)
		{
			return new List<GameObject>();
		}

		// Token: 0x060024FB RID: 9467 RVA: 0x000C70D0 File Offset: 0x000C52D0
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			return 0;
		}
	}
}
