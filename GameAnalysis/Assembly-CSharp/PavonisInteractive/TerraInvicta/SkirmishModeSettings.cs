using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200080C RID: 2060
	public class SkirmishModeSettings
	{
		// Token: 0x06004A7C RID: 19068 RVA: 0x001F3AE5 File Offset: 0x001F1CE5
		public SkirmishModeSettings(List<TISpaceFleetTemplate> fleetTemplates, TIHabTemplate habTemplate, List<TISpaceShipTemplate> importedShips)
		{
			this.fleetTemplates = fleetTemplates;
			this.habTemplate = habTemplate;
			this.importedShips = importedShips;
		}

		// Token: 0x04002B76 RID: 11126
		public List<TISpaceFleetTemplate> fleetTemplates;

		// Token: 0x04002B77 RID: 11127
		public TIHabTemplate habTemplate;

		// Token: 0x04002B78 RID: 11128
		public List<TISpaceShipTemplate> importedShips;
	}
}
