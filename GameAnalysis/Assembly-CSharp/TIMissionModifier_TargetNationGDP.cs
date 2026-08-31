using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000208 RID: 520
public class TIMissionModifier_TargetNationGDP : TIMissionModifier
{
	// Token: 0x0600070F RID: 1807 RVA: 0x0002204C File Offset: 0x0002024C
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (target == null)
		{
			return 0f;
		}
		float num = 0f;
		TINationState tinationState = TIMissionModifier.ObjectToNation(attackingCouncilor.faction, target);
		if (tinationState != null)
		{
			num = tinationState.missionDifficultyEconomyScore * TemplateManager.global.TIMissionModifier_TargetNationGDP_Multiplier;
		}
		return num;
	}
}
