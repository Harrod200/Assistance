using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000240 RID: 576
public class TIMissionModifier_Legislature_ExecCP : TIMissionModifier
{
	// Token: 0x0600078E RID: 1934 RVA: 0x00023CD8 File Offset: 0x00021ED8
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TINationState tinationState;
		if (target.isCouncilorState)
		{
			tinationState = TIMissionPhaseState.CouncilorLastKnownLocation(attackingCouncilor.faction, target.ref_councilor).ref_nation;
		}
		else
		{
			tinationState = target.ref_nation;
		}
		if (((tinationState != null) ? tinationState.GetControlPointTypeOwner(ControlPointType.Legislature) : null) == attackingCouncilor.faction)
		{
			bool? flag;
			if (tinationState == null)
			{
				flag = null;
			}
			else
			{
				TIControlPoint controlPointOfType = tinationState.GetControlPointOfType(ControlPointType.Legislature);
				flag = ((controlPointOfType != null) ? new bool?(!controlPointOfType.benefitsDisabled) : null);
			}
			bool? flag2 = flag;
			if (flag2.GetValueOrDefault())
			{
				TIControlPoint ref_controlPoint = target.ref_controlPoint;
				if (ref_controlPoint != null && ref_controlPoint.controlPointType == ControlPointType.Executive)
				{
					return 3f;
				}
			}
		}
		return 0f;
	}
}
