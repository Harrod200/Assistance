using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000245 RID: 581
public class TIMissionModifier_Oligarchs_Defense : TIMissionModifier
{
	// Token: 0x06000798 RID: 1944 RVA: 0x0002406C File Offset: 0x0002226C
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
		if (target.ref_faction != null && attackingCouncilor.faction != target.ref_faction && ((tinationState != null) ? tinationState.GetControlPointTypeOwner(ControlPointType.Oligarchs) : null) == target.ref_faction)
		{
			bool? flag;
			if (tinationState == null)
			{
				flag = null;
			}
			else
			{
				TIControlPoint controlPointOfType = tinationState.GetControlPointOfType(ControlPointType.Oligarchs);
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
