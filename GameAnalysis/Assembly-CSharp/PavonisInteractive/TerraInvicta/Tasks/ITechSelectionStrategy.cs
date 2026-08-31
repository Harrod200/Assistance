using System;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x0200094E RID: 2382
	public interface ITechSelectionStrategy
	{
		// Token: 0x06005ADE RID: 23262
		TITechTemplate SelectTech(TIFactionState council);
	}
}
