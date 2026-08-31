using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000243 RID: 579
public class TIMissionModifier_TheParty_NonExecCP_Defense : TIMissionModifier
{
	// Token: 0x06000794 RID: 1940 RVA: 0x00023F04 File Offset: 0x00022104
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
		if (target.ref_faction != null && ((tinationState != null) ? tinationState.GetControlPointTypeOwner(ControlPointType.TheParty) : null) == target.ref_faction)
		{
			bool? flag;
			if (tinationState == null)
			{
				flag = null;
			}
			else
			{
				TIControlPoint controlPointOfType = tinationState.GetControlPointOfType(ControlPointType.TheParty);
				flag = ((controlPointOfType != null) ? new bool?(!controlPointOfType.benefitsDisabled) : null);
			}
			bool? flag2 = flag;
			if (flag2.GetValueOrDefault())
			{
				TIControlPoint ref_controlPoint = target.ref_controlPoint;
				if (ref_controlPoint == null || ref_controlPoint.controlPointType != ControlPointType.Executive)
				{
					return 3f;
				}
			}
		}
		return 0f;
	}
}
