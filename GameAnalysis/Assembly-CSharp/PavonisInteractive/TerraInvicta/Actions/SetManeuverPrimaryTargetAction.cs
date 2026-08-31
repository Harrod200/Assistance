using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A5F RID: 2655
	public class SetManeuverPrimaryTargetAction : PlayerAction
	{
		// Token: 0x0600650B RID: 25867 RVA: 0x002FAEF0 File Offset: 0x002F90F0
		public SetManeuverPrimaryTargetAction(TISpaceShipState ship, CombatTargetableState target)
		{
			this.shipID = ship.ID;
			this.targetID = target.GetTargetableState().ID;
		}

		// Token: 0x0600650C RID: 25868 RVA: 0x002FAF18 File Offset: 0x002F9118
		public override void Execute()
		{
			TISpaceShipState state = this.shipID.GetState<TISpaceShipState>(false);
			CombatTargetableState combatTargetableState = this.targetID.GetState<TIGameState>(true) as CombatTargetableState;
			if (!GameControl.spaceCombat.combatantLookup.ContainsKey(state))
			{
				Debug.LogError("Combat Lookup Is Missing the ship attempting to target: " + ((state != null) ? state.displayName : null));
				return;
			}
			if (!GameControl.spaceCombat.combatantLookup.ContainsKey(combatTargetableState))
			{
				Debug.LogError("Combat Lookup Is Missing the target ship" + state.displayName);
				return;
			}
			state.SetCombatManeuverTarget(combatTargetableState);
			GameControl.spaceCombat.combatantLookup[state].ref_shipController.SetManeuverTarget(GameControl.spaceCombat.combatantLookup[combatTargetableState]);
		}

		// Token: 0x04004730 RID: 18224
		private GameStateID shipID;

		// Token: 0x04004731 RID: 18225
		private GameStateID targetID;
	}
}
