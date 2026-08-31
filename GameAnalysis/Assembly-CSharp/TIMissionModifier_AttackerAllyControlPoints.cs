using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000210 RID: 528
public class TIMissionModifier_AttackerAllyControlPoints : TIMissionModifier
{
	// Token: 0x0600071F RID: 1823 RVA: 0x00022410 File Offset: 0x00020610
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TINationState tinationState = TIMissionModifier.ObjectToNation(attackingCouncilor.faction, target);
		if (tinationState == null)
		{
			return 0f;
		}
		float num = 0f;
		float num2 = 0f;
		TIFactionState faction = attackingCouncilor.faction;
		foreach (TINationState tinationState2 in tinationState.allies)
		{
			foreach (TIControlPoint ticontrolPoint in tinationState2.controlPoints)
			{
				num += 1f;
				if (ticontrolPoint.faction == faction && !ticontrolPoint.benefitsDisabled && (tinationState2.GDP > tinationState.GDP || (tinationState.inFederation && tinationState.federation == tinationState2.federation)))
				{
					num2 += 1f;
				}
			}
		}
		if (num <= 0f)
		{
			return 0f;
		}
		return num2 / num * this.maxValueFromAttackerAllyControlPoints;
	}

	// Token: 0x04000629 RID: 1577
	public float maxValueFromAttackerAllyControlPoints = 6f;
}
