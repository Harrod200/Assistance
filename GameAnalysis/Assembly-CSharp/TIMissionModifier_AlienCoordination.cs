using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200025A RID: 602
public class TIMissionModifier_AlienCoordination : TIMissionModifier_HideInCodex
{
	// Token: 0x060007C6 RID: 1990 RVA: 0x000249FA File Offset: 0x00022BFA
	public override bool ShowCondition(TIFactionState faction)
	{
		return faction.MilestoneCompleted(CampaignMilestone.AlienDiplomacy);
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x00024A04 File Offset: 0x00022C04
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIFactionState tifactionState = GameStateManager.AlienProxy();
		float num = TIEffectsState.SumEffectsModifiers(Context.DetectAlienActivity, tifactionState, 0f, null);
		if (tifactionState.MilestoneCompleted(CampaignMilestone.AlienDiplomacy))
		{
			num += 1f;
			if (TIEffectsState.CheckForAnyEffectInContext(Context.AlienRelationsEstablished, tifactionState))
			{
				num += 2f;
				if (TIEffectsState.CheckForAnyEffectInContext(Context.CanTransferTerritoryToAliens, tifactionState))
				{
					num += 3f;
				}
			}
		}
		return num;
	}
}
