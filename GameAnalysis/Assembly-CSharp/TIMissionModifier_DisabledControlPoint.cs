using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000215 RID: 533
public class TIMissionModifier_DisabledControlPoint : TIMissionModifier
{
	// Token: 0x0600072C RID: 1836 RVA: 0x00022894 File Offset: 0x00020A94
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		if (target.isControlPointState && target.ref_controlPoint.benefitsDisabled)
		{
			num = TemplateManager.global.TIMissionModifier_DisabledControlPoint;
			foreach (TIControlPoint ticontrolPoint in target.ref_nation.controlPoints)
			{
				if (ticontrolPoint != target && ticontrolPoint.ref_faction == target.ref_faction && ticontrolPoint.benefitsDisabled)
				{
					num += TemplateManager.global.TIMissionModifier_AdditionalDisabledControlPoints;
				}
			}
		}
		return num;
	}
}
