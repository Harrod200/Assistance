using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000212 RID: 530
public class TIMissionModifier_JointControlPointStat : TIMissionModifier_StatBased
{
	// Token: 0x06000723 RID: 1827 RVA: 0x000226B0 File Offset: 0x000208B0
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		TINationState tinationState;
		if (target.isCouncilorState)
		{
			tinationState = TIMissionPhaseState.CouncilorLastKnownLocation(attackingCouncilor.faction, target.ref_councilor).ref_nation;
		}
		else
		{
			tinationState = target.ref_nation;
		}
		foreach (TIControlPoint ticontrolPoint in tinationState.controlPoints)
		{
			TIFactionState faction = ticontrolPoint.faction;
			if (faction != null && !ticontrolPoint.benefitsDisabled && attackingCouncilor.faction != faction)
			{
				num += TIMissionModifier.CouncilCollectiveDefense(faction, this.defenderAttribute) / 2f;
			}
		}
		num /= (float)tinationState.numControlPoints;
		float num2 = (float)((tinationState.FactionsWithControlPoint.Count - 1) / tinationState.numControlPoints);
		if (num2 > 0f)
		{
			num2 = 1f - 1f / num2;
			num *= num2;
		}
		return num;
	}

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000724 RID: 1828 RVA: 0x000227A8 File Offset: 0x000209A8
	public override string displayName
	{
		get
		{
			return string.Format(Loc.T(base.displayName), TIUtilities.GetAttributeString(this.defenderAttribute));
		}
	}
}
