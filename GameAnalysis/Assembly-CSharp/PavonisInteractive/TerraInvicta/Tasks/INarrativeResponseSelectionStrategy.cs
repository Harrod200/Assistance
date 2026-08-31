using System;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x0200094B RID: 2379
	public interface INarrativeResponseSelectionStrategy
	{
		// Token: 0x06005AD8 RID: 23256
		int SelectOption(TIFactionState faction, TIGameState target, TIGameState secondary, TINarrativeEventTemplate eventTemplate);

		// Token: 0x06005AD9 RID: 23257
		int SelectOption(TINationState nation, TIGameState target, TIGameState secondary, TINarrativeEventTemplate eventTemplate);
	}
}
