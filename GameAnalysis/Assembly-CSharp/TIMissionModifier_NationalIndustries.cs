using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000246 RID: 582
public class TIMissionModifier_NationalIndustries : TIMissionModifier
{
	// Token: 0x0600079A RID: 1946 RVA: 0x0002412C File Offset: 0x0002232C
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
		if (((tinationState != null) ? tinationState.GetControlPointTypeOwner(ControlPointType.NationalIndustries) : null) == attackingCouncilor.faction)
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
