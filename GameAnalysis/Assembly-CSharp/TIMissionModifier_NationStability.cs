using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000222 RID: 546
public class TIMissionModifier_NationStability : TIMissionModifier
{
	// Token: 0x06000749 RID: 1865 RVA: 0x00022EE0 File Offset: 0x000210E0
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TINationState tinationState = TIMissionModifier.ObjectToNation(attackingCouncilor.faction, target);
		if (tinationState != null)
		{
			return 10f - tinationState.unrest;
		}
		return 0f;
	}
}
