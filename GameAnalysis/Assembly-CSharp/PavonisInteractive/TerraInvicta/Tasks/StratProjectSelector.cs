using System;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000954 RID: 2388
	public class StratProjectSelector : IProjectSelectionStrategy
	{
		// Token: 0x06005AF7 RID: 23287 RVA: 0x002BCC52 File Offset: 0x002BAE52
		public TIProjectTemplate SelectProject(TIFactionState faction, int slot = -1)
		{
			return AIEvaluators.SelectProject(faction, slot);
		}

		// Token: 0x06005AF8 RID: 23288 RVA: 0x002BCC5B File Offset: 0x002BAE5B
		public int SelectSlotForProject(TIFactionState faction)
		{
			return faction.BestAvailableEmptySlot();
		}
	}
}
