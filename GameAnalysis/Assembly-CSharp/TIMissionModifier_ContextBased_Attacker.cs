using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001FE RID: 510
public class TIMissionModifier_ContextBased_Attacker : TIMissionModifier_ContextBased
{
	// Token: 0x060006F5 RID: 1781 RVA: 0x00021E1B File Offset: 0x0002001B
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		return TIEffectsState.SumEffectsModifiers(this.context, attackingCouncilor.faction, 0f, null);
	}
}
