using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000221 RID: 545
public class TIMissionModifier_NationUnrest : TIMissionModifier
{
	// Token: 0x06000747 RID: 1863 RVA: 0x00022EA8 File Offset: 0x000210A8
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TINationState tinationState = TIMissionModifier.ObjectToNation(attackingCouncilor.faction, target);
		if (tinationState != null)
		{
			return tinationState.unrest;
		}
		return 0f;
	}
}
