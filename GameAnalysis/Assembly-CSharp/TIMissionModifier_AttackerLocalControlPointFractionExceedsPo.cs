using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200020D RID: 525
public class TIMissionModifier_AttackerLocalControlPointFractionExceedsPopularity : TIMissionModifier
{
	// Token: 0x06000719 RID: 1817 RVA: 0x00022204 File Offset: 0x00020404
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (target == null)
		{
			return 0f;
		}
		TINationState tinationState = TIMissionModifier.ObjectToNation(attackingCouncilor.faction, target);
		if (tinationState == null)
		{
			return 0f;
		}
		TIFactionState faction = attackingCouncilor.faction;
		float num = (float)tinationState.CountFactionControlPoints(faction, false, true, true);
		float num2 = (float)tinationState.numControlPoints;
		float num3 = num / num2;
		float publicOpinionOfFaction = tinationState.GetPublicOpinionOfFaction(faction);
		float num4 = num3 - publicOpinionOfFaction;
		if (num4 <= 0f)
		{
			return 0f;
		}
		return num4 * 24f;
	}

	// Token: 0x04000627 RID: 1575
	private const float maxValue = 24f;
}
