using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005C6 RID: 1478
	public class OperationExecuted : GameEvent
	{
		// Token: 0x060027EB RID: 10219 RVA: 0x000D9AD9 File Offset: 0x000D7CD9
		public OperationExecuted(TIGameState actingState, IOperation operationTemplate, TIGameState target)
		{
			this.actingState = actingState;
			this.operationTemplate = operationTemplate;
			this.target = target;
		}

		// Token: 0x04001DE6 RID: 7654
		public TIGameState actingState;

		// Token: 0x04001DE7 RID: 7655
		public IOperation operationTemplate;

		// Token: 0x04001DE8 RID: 7656
		public TIGameState target;
	}
}
