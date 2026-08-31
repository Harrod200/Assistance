using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A5D RID: 2653
	public class RetractRadiatorsAction : PlayerAction
	{
		// Token: 0x06006507 RID: 25863 RVA: 0x002FAE70 File Offset: 0x002F9070
		public RetractRadiatorsAction(TISpaceShipState ship)
		{
			this.shipID = ship.ID;
		}

		// Token: 0x06006508 RID: 25864 RVA: 0x002FAE84 File Offset: 0x002F9084
		public override void Execute()
		{
			TISpaceShipState state = this.shipID.GetState<TISpaceShipState>(false);
			if (state.radiatorsExtended && !state.radiatorsExtending && !state.radiatorsRetracting)
			{
				state.InitiateRetractRadiators();
			}
		}

		// Token: 0x0400472D RID: 18221
		private GameStateID shipID;
	}
}
