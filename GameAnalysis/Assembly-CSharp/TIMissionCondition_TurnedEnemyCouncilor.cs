using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001B0 RID: 432
public class TIMissionCondition_TurnedEnemyCouncilor : TIMissionCondition
{
	// Token: 0x0600063D RID: 1597 RVA: 0x0001C8AB File Offset: 0x0001AAAB
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isCouncilorState && possibleTarget.ref_councilor.agentForFaction == councilor.faction)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
