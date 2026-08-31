using System;

namespace PavonisInteractive.TerraInvicta.Systems.Bootstrap
{
	// Token: 0x020009B8 RID: 2488
	public interface IScenario
	{
		// Token: 0x17001023 RID: 4131
		// (get) Token: 0x06005DCA RID: 24010
		string scenarioTemplateName { get; }

		// Token: 0x17001024 RID: 4132
		// (get) Token: 0x06005DCB RID: 24011
		TIMetaTemplate scenarioTemplate { get; }

		// Token: 0x17001025 RID: 4133
		// (get) Token: 0x06005DCC RID: 24012
		TIFactionTemplate activePlayerFaction { get; }

		// Token: 0x06005DCD RID: 24013
		bool Initialize();
	}
}
