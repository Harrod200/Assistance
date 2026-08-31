using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A6C RID: 2668
	public class DecommissionHabModuleAction : PlayerAction
	{
		// Token: 0x06006526 RID: 25894 RVA: 0x002FB65B File Offset: 0x002F985B
		public DecommissionHabModuleAction(TIHabModuleState module)
		{
			this.moduleID = module.ID;
		}

		// Token: 0x06006527 RID: 25895 RVA: 0x002FB670 File Offset: 0x002F9870
		public override void Execute()
		{
			TIHabModuleState state = this.moduleID.GetState<TIHabModuleState>(false);
			state.ref_hab.BeginDecommissionModule(state);
		}

		// Token: 0x04004751 RID: 18257
		private GameStateID moduleID;
	}
}
