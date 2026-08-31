using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000242 RID: 578
public class TIMissionModifier_TheParty_NonExecCP : TIMissionModifier
{
	// Token: 0x06000792 RID: 1938 RVA: 0x00023E4C File Offset: 0x0002204C
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
		if (((tinationState != null) ? tinationState.GetControlPointTypeOwner(ControlPointType.TheParty) : null) == attackingCouncilor.faction)
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
