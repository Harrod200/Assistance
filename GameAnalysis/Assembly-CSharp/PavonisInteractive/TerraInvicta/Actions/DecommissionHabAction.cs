using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A6B RID: 2667
	public class DecommissionHabAction : PlayerAction
	{
		// Token: 0x06006524 RID: 25892 RVA: 0x002FB634 File Offset: 0x002F9834
		public DecommissionHabAction(TIHabState hab)
		{
			this.habID = hab.ID;
		}

		// Token: 0x06006525 RID: 25893 RVA: 0x002FB648 File Offset: 0x002F9848
		public override void Execute()
		{
			this.habID.GetState<TIHabState>(false).BeginDecommissionHab();
		}

		// Token: 0x04004750 RID: 18256
		private GameStateID habID;
	}
}
