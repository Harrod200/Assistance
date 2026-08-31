using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000223 RID: 547
public class TIMissionModifier_NationDemocracy : TIMissionModifier
{
	// Token: 0x0600074B RID: 1867 RVA: 0x00022F20 File Offset: 0x00021120
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TINationState tinationState = TIMissionModifier.ObjectToNation(attackingCouncilor.faction, target);
		if (tinationState != null)
		{
			return tinationState.democracy;
		}
		return 0f;
	}
}
