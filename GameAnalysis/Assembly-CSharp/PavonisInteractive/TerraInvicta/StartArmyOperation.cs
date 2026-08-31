using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005C3 RID: 1475
	public class StartArmyOperation : GameEvent
	{
		// Token: 0x060027E8 RID: 10216 RVA: 0x000D9A82 File Offset: 0x000D7C82
		public StartArmyOperation(TIGameState actingState, IOperation operationTemplate, TIGameState target)
		{
			this.actingState = actingState;
			this.operationTemplate = operationTemplate;
			this.target = target;
		}

		// Token: 0x04001DDD RID: 7645
		public TIGameState actingState;

		// Token: 0x04001DDE RID: 7646
		public IOperation operationTemplate;

		// Token: 0x04001DDF RID: 7647
		public TIGameState target;
	}
}
