using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A9B RID: 2715
	public class SetSTOFightersForCombatAction : PlayerAction
	{
		// Token: 0x0600658B RID: 25995 RVA: 0x002FD38C File Offset: 0x002FB58C
		public SetSTOFightersForCombatAction(TISpaceCombatState combat, TIFactionState faction, Dictionary<TINationState, PlannedFighters> fighterPlan)
		{
			this.combatID = combat.ID;
			this.factionID = faction.ID;
			this.fighterPlan = fighterPlan;
		}

		// Token: 0x0600658C RID: 25996 RVA: 0x002FD3B4 File Offset: 0x002FB5B4
		public override void Execute()
		{
			TIFactionState state = this.factionID.GetState<TIFactionState>(false);
			this.combatID.GetState<TISpaceCombatState>(false).AddFightersToCombat(state, this.fighterPlan);
		}

		// Token: 0x040047D8 RID: 18392
		private GameStateID factionID;

		// Token: 0x040047D9 RID: 18393
		private GameStateID combatID;

		// Token: 0x040047DA RID: 18394
		private readonly Dictionary<TINationState, PlannedFighters> fighterPlan;
	}
}
