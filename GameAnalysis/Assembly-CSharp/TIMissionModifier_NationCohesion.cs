using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200021F RID: 543
public class TIMissionModifier_NationCohesion : TIMissionModifier
{
	// Token: 0x06000743 RID: 1859 RVA: 0x00022E30 File Offset: 0x00021030
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TINationState tinationState = TIMissionModifier.ObjectToNation(attackingCouncilor.faction, target);
		if (tinationState != null)
		{
			return tinationState.cohesion;
		}
		return 0f;
	}
}
