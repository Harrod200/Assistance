using System;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x0200094D RID: 2381
	public interface IProjectSelectionStrategy
	{
		// Token: 0x06005ADD RID: 23261
		TIProjectTemplate SelectProject(TIFactionState faction, int slot = -1);
	}
}
