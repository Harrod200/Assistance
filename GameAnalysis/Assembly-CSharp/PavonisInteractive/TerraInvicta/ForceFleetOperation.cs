using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005E0 RID: 1504
	public class ForceFleetOperation : GameEvent
	{
		// Token: 0x06002805 RID: 10245 RVA: 0x000D9C83 File Offset: 0x000D7E83
		public ForceFleetOperation(TISpaceFleetState fleet, TIGameState target, TIOperationTemplate operation)
		{
			this.fleet = fleet;
			this.target = target;
			this.operation = operation;
		}

		// Token: 0x04001E05 RID: 7685
		public TISpaceFleetState fleet;

		// Token: 0x04001E06 RID: 7686
		public TIGameState target;

		// Token: 0x04001E07 RID: 7687
		public TIOperationTemplate operation;
	}
}
