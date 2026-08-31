using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A4D RID: 2637
	public class ApproachDockAction : PlayerAction
	{
		// Token: 0x060064E7 RID: 25831 RVA: 0x002FA4A8 File Offset: 0x002F86A8
		public ApproachDockAction(TIHabState hab, TISpaceFleetState fleet)
		{
			this.habID = hab.ID;
			this.fleetID = fleet.ID;
		}

		// Token: 0x060064E8 RID: 25832 RVA: 0x002FA4C8 File Offset: 0x002F86C8
		public override void Execute()
		{
			TIHabState state = this.habID.GetState<TIHabState>(false);
			this.fleetID.GetState<TISpaceFleetState>(false).ApproachDock(state);
		}

		// Token: 0x04004702 RID: 18178
		private GameStateID habID;

		// Token: 0x04004703 RID: 18179
		private GameStateID fleetID;
	}
}
