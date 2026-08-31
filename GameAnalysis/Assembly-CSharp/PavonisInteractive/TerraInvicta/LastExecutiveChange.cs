using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200076D RID: 1901
	public struct LastExecutiveChange
	{
		// Token: 0x060039B4 RID: 14772 RVA: 0x001554DD File Offset: 0x001536DD
		public LastExecutiveChange(TIFactionState faction, TIDateTime date, ControlPointChangeCause cause)
		{
			this.newExecutive = faction;
			this.date = date;
			this.cause = cause;
		}

		// Token: 0x0400256B RID: 9579
		public TIFactionState newExecutive;

		// Token: 0x0400256C RID: 9580
		public TIDateTime date;

		// Token: 0x0400256D RID: 9581
		public ControlPointChangeCause cause;
	}
}
