using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A5C RID: 2652
	public class ExtendRadiatorsAction : PlayerAction
	{
		// Token: 0x06006505 RID: 25861 RVA: 0x002FAE23 File Offset: 0x002F9023
		public ExtendRadiatorsAction(TISpaceShipState ship)
		{
			this.shipID = ship.ID;
		}

		// Token: 0x06006506 RID: 25862 RVA: 0x002FAE38 File Offset: 0x002F9038
		public override void Execute()
		{
			TISpaceShipState state = this.shipID.GetState<TISpaceShipState>(false);
			if (!state.radiatorsExtended && !state.radiatorsExtending && !state.radiatorsRetracting)
			{
				state.InitiateExtendRadiators();
			}
		}

		// Token: 0x0400472C RID: 18220
		private GameStateID shipID;
	}
}
