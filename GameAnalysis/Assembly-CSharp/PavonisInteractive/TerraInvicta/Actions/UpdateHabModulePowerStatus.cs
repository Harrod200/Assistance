using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000AA4 RID: 2724
	public class UpdateHabModulePowerStatus : PlayerAction
	{
		// Token: 0x060065A1 RID: 26017 RVA: 0x002FD7D1 File Offset: 0x002FB9D1
		public UpdateHabModulePowerStatus(TIHabModuleState module, bool status, Action callback)
		{
			this.moduleID = module.ID;
			this.status = status;
			this.callback = callback;
		}

		// Token: 0x060065A2 RID: 26018 RVA: 0x002FD7F3 File Offset: 0x002FB9F3
		public override void Execute()
		{
			this.moduleID.GetState<TIHabModuleState>(false).SetPowerStatus(this.status, false);
			if (this.callback != null)
			{
				this.callback();
			}
		}

		// Token: 0x040047F1 RID: 18417
		private GameStateID moduleID;

		// Token: 0x040047F2 RID: 18418
		private bool status;

		// Token: 0x040047F3 RID: 18419
		private Action callback;
	}
}
