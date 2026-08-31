using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A61 RID: 2657
	public class SetCombatPrimaryTargetAction : PlayerAction
	{
		// Token: 0x0600650F RID: 25871 RVA: 0x002FB01C File Offset: 0x002F921C
		public SetCombatPrimaryTargetAction(TISpaceShipState ship, CombatTargetableState target)
		{
			this.shipID = ship.ID;
			this.targetID = target.GetTargetableState().ID;
		}

		// Token: 0x06006510 RID: 25872 RVA: 0x002FB044 File Offset: 0x002F9244
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
			state.SetCombatPrimaryTarget(combatTargetableState);
			GameControl.spaceCombat.combatantLookup[state].ref_shipController.SetPrimaryTarget(GameControl.spaceCombat.combatantLookup[combatTargetableState]);
		}

		// Token: 0x04004733 RID: 18227
		private GameStateID shipID;

		// Token: 0x04004734 RID: 18228
		private GameStateID targetID;
	}
}
