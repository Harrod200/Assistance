using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000664 RID: 1636
	public class NationStateSelected : GameEvent
	{
		// Token: 0x0600288B RID: 10379 RVA: 0x000DA62A File Offset: 0x000D882A
		public NationStateSelected(TINationState nation)
		{
			this.nation = nation;
		}

		// Token: 0x04001EC2 RID: 7874
		public TINationState nation;
	}
}
