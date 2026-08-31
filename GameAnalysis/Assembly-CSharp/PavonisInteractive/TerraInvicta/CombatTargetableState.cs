using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007C0 RID: 1984
	public interface CombatTargetableState
	{
		// Token: 0x06004516 RID: 17686
		TIGameState GetTargetableState();

		// Token: 0x06004517 RID: 17687
		bool IsAlien();

		// Token: 0x06004518 RID: 17688
		float ECMValue(TIFactionState attacker, TIHabState alliedHab);
	}
}
