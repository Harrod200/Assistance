using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200080B RID: 2059
	public class SkirmishMenuController : MenuController
	{
		// Token: 0x17000E91 RID: 3729
		// (get) Token: 0x06004A7A RID: 19066 RVA: 0x001F3AD5 File Offset: 0x001F1CD5
		public IEnumerable<SkirmishShipListItemController> skirmishShipElements
		{
			get
			{
				return base.GetComponentsInChildren<SkirmishShipListItemController>();
			}
		}
	}
}
