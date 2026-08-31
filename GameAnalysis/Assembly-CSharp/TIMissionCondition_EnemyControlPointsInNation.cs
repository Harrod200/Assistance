using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000191 RID: 401
public class TIMissionCondition_EnemyControlPointsInNation : TIMissionCondition
{
	// Token: 0x060005F9 RID: 1529 RVA: 0x0001B73F File Offset: 0x0001993F
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.ref_nation.TotalOwningFaction == councilor.faction)
		{
			return base.GetType().Name;
		}
		return "_Pass";
	}
}
