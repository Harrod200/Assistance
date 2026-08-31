using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000247 RID: 583
public class TIMissionModifier_NationalIndustries_Def : TIMissionModifier
{
	// Token: 0x0600079C RID: 1948 RVA: 0x000241D0 File Offset: 0x000223D0
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
		if (target.ref_faction != null && ((tinationState != null) ? tinationState.GetControlPointTypeOwner(ControlPointType.NationalIndustries) : null) == target.ref_faction)
		{
			bool? flag;
			if (tinationState == null)
			{
				flag = null;
			}
			else
			{
				TIControlPoint controlPointOfType = tinationState.GetControlPointOfType(ControlPointType.NationalIndustries);
				flag = ((controlPointOfType != null) ? new bool?(!controlPointOfType.benefitsDisabled) : null);
			}
			bool? flag2 = flag;
			if (flag2.GetValueOrDefault())
			{
				return TIGlobalConfig.globalConfig.TIMissionModifier_NationalIndustries;
			}
		}
		return 0f;
	}
}
