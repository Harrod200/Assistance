using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200066A RID: 1642
	public class MultiSelectArmiesSelected : GameEvent
	{
		// Token: 0x06002891 RID: 10385 RVA: 0x000DA684 File Offset: 0x000D8884
		public MultiSelectArmiesSelected(List<TIArmyState> armies)
		{
			this.armies = armies;
		}

		// Token: 0x04001EC8 RID: 7880
		public List<TIArmyState> armies;
	}
}
