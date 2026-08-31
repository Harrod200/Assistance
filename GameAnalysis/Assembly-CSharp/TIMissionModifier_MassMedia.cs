using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000248 RID: 584
public class TIMissionModifier_MassMedia : TIMissionModifier
{
	// Token: 0x0600079E RID: 1950 RVA: 0x00024280 File Offset: 0x00022480
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
		if (((tinationState != null) ? tinationState.GetControlPointTypeOwner(ControlPointType.MassMedia) : null) == attackingCouncilor.faction)
		{
			bool? flag;
			if (tinationState == null)
			{
				flag = null;
			}
			else
			{
				TIControlPoint controlPointOfType = tinationState.GetControlPointOfType(ControlPointType.MassMedia);
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
