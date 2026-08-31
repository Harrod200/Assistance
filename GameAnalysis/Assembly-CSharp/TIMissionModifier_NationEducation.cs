using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000224 RID: 548
public class TIMissionModifier_NationEducation : TIMissionModifier
{
	// Token: 0x0600074D RID: 1869 RVA: 0x00022F58 File Offset: 0x00021158
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TINationState tinationState = TIMissionModifier.ObjectToNation(attackingCouncilor.faction, target);
		if (tinationState != null)
		{
			return tinationState.education;
		}
		return 0f;
	}
}
