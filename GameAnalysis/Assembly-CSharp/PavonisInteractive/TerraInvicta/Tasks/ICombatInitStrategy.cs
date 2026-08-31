using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000950 RID: 2384
	public interface ICombatInitStrategy
	{
		// Token: 0x06005AE1 RID: 23265
		CombatStance SelectStance(TIFactionState faction, TISpaceCombatState combatState, Dictionary<TINationState, PlannedFighters> fighterPlan);

		// Token: 0x06005AE2 RID: 23266
		float SelectBid_kps(TIFactionState faction, TISpaceCombatState combatState, out CombatStance extendedStance, out List<TISpaceShipState> chasers);
	}
}
