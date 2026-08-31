using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000256 RID: 598
public class TIMissionModifier_NationalRivalries : TIMissionModifier
{
	// Token: 0x060007BE RID: 1982 RVA: 0x000248E8 File Offset: 0x00022AE8
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TINationState targetNation;
		if (target.isCouncilorState)
		{
			targetNation = TIMissionPhaseState.CouncilorLastKnownLocation(attackingCouncilor.faction, target.ref_councilor).ref_nation;
		}
		else
		{
			targetNation = target.ref_nation;
		}
		if (targetNation != null && targetNation.CountFactionControlPoints(attackingCouncilor.faction, false, true, true) == 0)
		{
			return (float)attackingCouncilor.faction.controlPoints.Count<TIControlPoint>((TIControlPoint x) => x.nation.enemies.Contains(targetNation)) * TemplateManager.global.TIMissionModifier_NationalRivalries_Multiplier;
		}
		return 0f;
	}
}
