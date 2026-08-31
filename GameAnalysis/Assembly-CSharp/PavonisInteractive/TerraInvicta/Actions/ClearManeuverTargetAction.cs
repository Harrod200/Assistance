using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A60 RID: 2656
	public class ClearManeuverTargetAction : PlayerAction
	{
		// Token: 0x0600650D RID: 25869 RVA: 0x002FAFCB File Offset: 0x002F91CB
		public ClearManeuverTargetAction(TISpaceShipState ship)
		{
			this.shipID = ship.ID;
		}

		// Token: 0x0600650E RID: 25870 RVA: 0x002FAFE0 File Offset: 0x002F91E0
		public override void Execute()
		{
			TISpaceShipState state = this.shipID.GetState<TISpaceShipState>(false);
			state.SetCombatManeuverTarget(null);
			GameControl.spaceCombat.combatantLookup[state].ref_shipController.SetManeuverTarget(null);
		}

		// Token: 0x04004732 RID: 18226
		private GameStateID shipID;
	}
}
