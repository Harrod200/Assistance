using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A62 RID: 2658
	public class ClearPrimaryTargetAction : PlayerAction
	{
		// Token: 0x06006511 RID: 25873 RVA: 0x002FB0F7 File Offset: 0x002F92F7
		public ClearPrimaryTargetAction(TISpaceShipState ship)
		{
			this.shipID = ship.ID;
		}

		// Token: 0x06006512 RID: 25874 RVA: 0x002FB10C File Offset: 0x002F930C
		public override void Execute()
		{
			TISpaceShipState state = this.shipID.GetState<TISpaceShipState>(false);
			state.SetCombatPrimaryTarget(null);
			GameControl.spaceCombat.combatantLookup[state].ref_shipController.SetPrimaryTarget(null);
		}

		// Token: 0x04004735 RID: 18229
		private GameStateID shipID;
	}
}
