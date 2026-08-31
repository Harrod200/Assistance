using System;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A63 RID: 2659
	public class SetEndCombatVoteAction : PlayerAction
	{
		// Token: 0x06006513 RID: 25875 RVA: 0x002FB148 File Offset: 0x002F9348
		public SetEndCombatVoteAction(TIFactionState faction, bool preference)
		{
			this.factionID = faction.ID;
			this.preference = preference;
		}

		// Token: 0x06006514 RID: 25876 RVA: 0x002FB164 File Offset: 0x002F9364
		public override void Execute()
		{
			TIFactionState faction = this.factionID.GetState<TIFactionState>(false);
			TISpaceCombatState combatState = GameControl.spaceCombat.combatState;
			combatState.votedEndCombat[faction] = this.preference;
			if (this.preference)
			{
				if (combatState.votedEndCombatFirst == null)
				{
					combatState.votedEndCombatFirst = faction;
				}
			}
			else if (combatState.votedEndCombatFirst == faction)
			{
				TIFactionState tifactionState = combatState.factions.FirstOrDefault<TIFactionState>((TIFactionState x) => x != faction);
				if (tifactionState != null)
				{
					if (combatState.votedEndCombat[tifactionState])
					{
						combatState.votedEndCombatFirst = tifactionState;
					}
					else
					{
						combatState.votedEndCombatFirst = null;
					}
				}
			}
			GameControl.eventManager.TriggerEvent(new EndCombatStanceChanged(faction), null, Array.Empty<object>());
		}

		// Token: 0x04004736 RID: 18230
		private readonly GameStateID factionID;

		// Token: 0x04004737 RID: 18231
		private readonly bool preference;
	}
}
