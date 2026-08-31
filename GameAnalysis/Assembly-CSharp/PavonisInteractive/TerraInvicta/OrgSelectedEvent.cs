using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000666 RID: 1638
	public class OrgSelectedEvent : GameEvent
	{
		// Token: 0x0600288D RID: 10381 RVA: 0x000DA648 File Offset: 0x000D8848
		public OrgSelectedEvent(TIOrgState org)
		{
			this.org = org;
		}

		// Token: 0x04001EC4 RID: 7876
		public TIOrgState org;
	}
}
