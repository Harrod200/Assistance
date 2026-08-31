using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005C4 RID: 1476
	public class StartFleetOperation : GameEvent
	{
		// Token: 0x060027E9 RID: 10217 RVA: 0x000D9A9F File Offset: 0x000D7C9F
		public StartFleetOperation(TIGameState actingState, IOperation operationTemplate, TIGameState target)
		{
			this.actingState = actingState;
			this.operationTemplate = operationTemplate;
			this.target = target;
		}

		// Token: 0x04001DE0 RID: 7648
		public TIGameState actingState;

		// Token: 0x04001DE1 RID: 7649
		public IOperation operationTemplate;

		// Token: 0x04001DE2 RID: 7650
		public TIGameState target;
	}
}
