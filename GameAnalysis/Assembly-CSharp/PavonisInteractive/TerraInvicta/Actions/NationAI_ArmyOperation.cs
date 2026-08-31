using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000AAA RID: 2730
	public class NationAI_ArmyOperation : SimulationAction
	{
		// Token: 0x060065AF RID: 26031 RVA: 0x002FDAFE File Offset: 0x002FBCFE
		public NationAI_ArmyOperation(TIArmyState army, TIGameState target, IOperation operation)
		{
			this.actorID = army.ID;
			this.targetID = target.ID;
			this.operation = operation;
		}

		// Token: 0x060065B0 RID: 26032 RVA: 0x002FDB28 File Offset: 0x002FBD28
		public override void Execute()
		{
			TIArmyState state = this.actorID.GetState<TIArmyState>(false);
			TIGameState state2 = this.targetID.GetState<TIGameState>(true);
			this.operation.OnOperationConfirm(state, state2, null, null);
		}

		// Token: 0x040047FD RID: 18429
		private GameStateID actorID;

		// Token: 0x040047FE RID: 18430
		private GameStateID targetID;

		// Token: 0x040047FF RID: 18431
		private IOperation operation;
	}
}
