using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200020F RID: 527
public class TIMissionModifier_AttackerAdjacentControlPoints : TIMissionModifier
{
	// Token: 0x0600071D RID: 1821 RVA: 0x000222FC File Offset: 0x000204FC
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
		float num = 0f;
		float num2 = 0f;
		TIFactionState faction = attackingCouncilor.faction;
		foreach (TINationState tinationState2 in tinationState.AdjacentNations(false))
		{
			foreach (TIControlPoint ticontrolPoint in tinationState2.controlPoints)
			{
				num2 += 1f;
				if (ticontrolPoint.faction == faction && !ticontrolPoint.benefitsDisabled)
				{
					num += 1f;
				}
			}
		}
		if (num2 <= 0f)
		{
			return 0f;
		}
		return num / num2 * TIGlobalConfig.globalConfig.maxValueFromAttackerAdjacentControlPoints;
	}
}
