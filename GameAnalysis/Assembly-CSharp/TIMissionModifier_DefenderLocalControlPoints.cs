using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200020E RID: 526
public class TIMissionModifier_DefenderLocalControlPoints : TIMissionModifier
{
	// Token: 0x0600071B RID: 1819 RVA: 0x00022288 File Offset: 0x00020488
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
		TIFactionState ref_faction = target.ref_faction;
		if (ref_faction == null)
		{
			return 0f;
		}
		float num = (float)tinationState.CountFactionControlPoints(ref_faction, false, true, true);
		int numControlPoints = tinationState.numControlPoints;
		return num / (float)numControlPoints * 6f;
	}

	// Token: 0x04000628 RID: 1576
	private const float maxValueFromDefenderLocalControlPoints = 6f;
}
