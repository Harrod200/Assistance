using System;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000955 RID: 2389
	public class StratTechSelector : ITechSelectionStrategy
	{
		// Token: 0x06005AFA RID: 23290 RVA: 0x002BCC6B File Offset: 0x002BAE6B
		public TITechTemplate SelectTech(TIFactionState faction)
		{
			return AIEvaluators.SelectTech(faction, TIGlobalResearchState.AvailableTechs(), true);
		}
	}
}
