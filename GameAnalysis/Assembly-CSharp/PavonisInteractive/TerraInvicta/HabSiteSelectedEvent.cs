using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000682 RID: 1666
	public class HabSiteSelectedEvent : GameEvent
	{
		// Token: 0x060028AA RID: 10410 RVA: 0x000DA8D1 File Offset: 0x000D8AD1
		public HabSiteSelectedEvent(TIHabSiteState habSite)
		{
			this.habSite = habSite;
		}

		// Token: 0x04001EE8 RID: 7912
		public TIHabSiteState habSite;
	}
}
