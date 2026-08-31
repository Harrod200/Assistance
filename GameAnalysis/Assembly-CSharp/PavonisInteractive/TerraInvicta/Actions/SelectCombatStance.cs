using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A87 RID: 2695
	public class SelectCombatStance : PlayerAction
	{
		// Token: 0x06006562 RID: 25954 RVA: 0x002FC90C File Offset: 0x002FAB0C
		public SelectCombatStance(TISpaceCombatState combat, TIFactionState faction, CombatStance stance)
		{
			this.combatID = combat.ID;
			this.factionID = faction.ID;
			this.stance = stance;
		}

		// Token: 0x06006563 RID: 25955 RVA: 0x002FC934 File Offset: 0x002FAB34
		public override void Execute()
		{
			TIFactionState state = this.factionID.GetState<TIFactionState>(false);
			TISpaceCombatState state2 = this.combatID.GetState<TISpaceCombatState>(false);
			TIPromptQueueState.RemovePromptStatic(state, null, state2, "PromptSelectSpaceCombatStance", 0);
			state2.stances[state] = this.stance;
			Debug.Log("Setting Combat Stance for " + state.displayName + ": " + this.stance.ToString());
			state2.SetRequiresBidding();
			state2.SetRequiresCombat();
			GameControl.eventManager.TriggerEvent(new CombatStanceSelected(state), null, Array.Empty<object>());
		}

		// Token: 0x04004797 RID: 18327
		private GameStateID combatID;

		// Token: 0x04004798 RID: 18328
		private GameStateID factionID;

		// Token: 0x04004799 RID: 18329
		private readonly CombatStance stance;
	}
}
