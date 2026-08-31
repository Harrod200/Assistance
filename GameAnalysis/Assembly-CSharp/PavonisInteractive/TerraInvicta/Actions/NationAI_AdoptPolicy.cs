using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000AA9 RID: 2729
	public class NationAI_AdoptPolicy : SimulationAction
	{
		// Token: 0x060065AD RID: 26029 RVA: 0x002FDA95 File Offset: 0x002FBC95
		public NationAI_AdoptPolicy(TINationState nation, TIGameState target, TIPolicyOption policy)
		{
			this.actorID = nation.ID;
			this.targetID = target.ID;
			this.policy = policy;
		}

		// Token: 0x060065AE RID: 26030 RVA: 0x002FDABC File Offset: 0x002FBCBC
		public override void Execute()
		{
			TINationState state = this.actorID.GetState<TINationState>(false);
			if (this.policy.Allowed(state))
			{
				TIGameState state2 = this.targetID.GetState<TIGameState>(true);
				this.policy.OnConfirm(state, state2);
			}
		}

		// Token: 0x040047FA RID: 18426
		private GameStateID actorID;

		// Token: 0x040047FB RID: 18427
		private GameStateID targetID;

		// Token: 0x040047FC RID: 18428
		private TIPolicyOption policy;
	}
}
