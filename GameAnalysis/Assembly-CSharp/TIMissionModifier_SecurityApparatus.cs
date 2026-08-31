using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000249 RID: 585
public class TIMissionModifier_SecurityApparatus : TIMissionModifier
{
	// Token: 0x060007A0 RID: 1952 RVA: 0x00024320 File Offset: 0x00022520
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
		if (((tinationState != null) ? tinationState.GetControlPointTypeOwner(ControlPointType.SecurityApparatus) : null) == attackingCouncilor.faction)
		{
			bool? flag;
			if (tinationState == null)
			{
				flag = null;
			}
			else
			{
				TIControlPoint controlPointOfType = tinationState.GetControlPointOfType(ControlPointType.SecurityApparatus);
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
