using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000217 RID: 535
public class TIMissionModifier_numDefendedControlPoints : TIMissionModifier
{
	// Token: 0x06000730 RID: 1840 RVA: 0x000229D8 File Offset: 0x00020BD8
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
		return (float)tinationState.controlPoints.Where<TIControlPoint>((TIControlPoint x) => x.defended).Count<TIControlPoint>();
	}
}
