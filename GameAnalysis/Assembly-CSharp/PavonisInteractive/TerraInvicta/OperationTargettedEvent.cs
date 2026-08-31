using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000679 RID: 1657
	public class OperationTargettedEvent : GameEvent
	{
		// Token: 0x060028A1 RID: 10401 RVA: 0x000DA83C File Offset: 0x000D8A3C
		public OperationTargettedEvent(TIGameState target, TIGameState actorState)
		{
			this.target = target;
			this.actorState = actorState;
		}

		// Token: 0x04001EDD RID: 7901
		public TIGameState target;

		// Token: 0x04001EDE RID: 7902
		public TIGameState actorState;
	}
}
