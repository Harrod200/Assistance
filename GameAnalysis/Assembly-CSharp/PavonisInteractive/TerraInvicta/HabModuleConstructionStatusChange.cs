using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200064D RID: 1613
	public class HabModuleConstructionStatusChange : GameEvent
	{
		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x06002872 RID: 10354 RVA: 0x000DA49B File Offset: 0x000D869B
		public TISectorState sector
		{
			get
			{
				return this.habModule.sector;
			}
		}

		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x06002873 RID: 10355 RVA: 0x000DA4A8 File Offset: 0x000D86A8
		public TIHabState hab
		{
			get
			{
				return this.habModule.hab;
			}
		}

		// Token: 0x06002874 RID: 10356 RVA: 0x000DA4B5 File Offset: 0x000D86B5
		public HabModuleConstructionStatusChange(TIHabModuleState habModule)
		{
			this.habModule = habModule;
		}

		// Token: 0x04001EA7 RID: 7847
		public TIHabModuleState habModule;
	}
}
