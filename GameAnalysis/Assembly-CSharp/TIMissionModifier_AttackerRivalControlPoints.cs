using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000211 RID: 529
public class TIMissionModifier_AttackerRivalControlPoints : TIMissionModifier
{
	// Token: 0x06000721 RID: 1825 RVA: 0x00022550 File Offset: 0x00020750
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
		float num3 = Mathf.Clamp(this.maxValueFromAttackerRivalControlPoints + (float)tinationState.rivals.Count<TINationState>() * 2f, 0f, 24f);
		foreach (TINationState tinationState2 in tinationState.enemies)
		{
			foreach (TIControlPoint ticontrolPoint in tinationState2.controlPoints)
			{
				num += 1f;
				if (ticontrolPoint.faction == faction && !ticontrolPoint.benefitsDisabled)
				{
					if (tinationState.IsAtWarWith(tinationState2) || tinationState2.breakawayParent == tinationState)
					{
						num2 += 2f;
					}
					else
					{
						num2 += 1f;
					}
				}
			}
		}
		if (num <= 0f)
		{
			return 0f;
		}
		return num2 / num * num3;
	}

	// Token: 0x0400062A RID: 1578
	public float maxValueFromAttackerRivalControlPoints = 4f;
}
