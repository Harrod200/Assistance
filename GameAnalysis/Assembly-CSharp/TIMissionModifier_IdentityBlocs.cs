using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200024C RID: 588
public class TIMissionModifier_IdentityBlocs : TIMissionModifier
{
	// Token: 0x060007A6 RID: 1958 RVA: 0x00024500 File Offset: 0x00022700
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
		if (((tinationState != null) ? tinationState.GetControlPointTypeOwner(ControlPointType.IdentityBlocs) : null) == attackingCouncilor.faction)
		{
			bool? flag;
			if (tinationState == null)
			{
				flag = null;
			}
			else
			{
				TIControlPoint controlPointOfType = tinationState.GetControlPointOfType(ControlPointType.IdentityBlocs);
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
