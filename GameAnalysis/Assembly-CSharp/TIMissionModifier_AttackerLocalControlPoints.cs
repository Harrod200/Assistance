using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200020C RID: 524
public class TIMissionModifier_AttackerLocalControlPoints : TIMissionModifier
{
	// Token: 0x06000717 RID: 1815 RVA: 0x000221A0 File Offset: 0x000203A0
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
		return num / num2 * 6f;
	}

	// Token: 0x04000626 RID: 1574
	private const float maxValueFromAttackerLocalControlPoints = 6f;
}
