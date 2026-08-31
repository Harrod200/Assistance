using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005C5 RID: 1477
	public class StartFactionOperation : GameEvent
	{
		// Token: 0x060027EA RID: 10218 RVA: 0x000D9ABC File Offset: 0x000D7CBC
		public StartFactionOperation(TIGameState actingState, IOperation operationTemplate, TIGameState target)
		{
			this.actingState = actingState;
			this.operationTemplate = operationTemplate;
			this.target = target;
		}

		// Token: 0x04001DE3 RID: 7651
		public TIGameState actingState;

		// Token: 0x04001DE4 RID: 7652
		public IOperation operationTemplate;

		// Token: 0x04001DE5 RID: 7653
		public TIGameState target;
	}
}
