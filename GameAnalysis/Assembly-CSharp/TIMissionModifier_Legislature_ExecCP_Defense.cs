using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000241 RID: 577
public class TIMissionModifier_Legislature_ExecCP_Defense : TIMissionModifier
{
	// Token: 0x06000790 RID: 1936 RVA: 0x00023D8C File Offset: 0x00021F8C
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
		TIControlPoint ref_controlPoint = target.ref_controlPoint;
		if (ref_controlPoint != null && ref_controlPoint.executive && ((tinationState != null) ? tinationState.executiveFaction : null) != null && tinationState.GetControlPointTypeOwner(ControlPointType.Legislature) == tinationState.executiveFaction)
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
				return 3f;
			}
		}
		return 0f;
	}
}
