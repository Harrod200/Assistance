using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200019F RID: 415
public class TIMissionCondition_EnemyOrTurnedCouncilor : TIMissionCondition
{
	// Token: 0x06000617 RID: 1559 RVA: 0x0001C12C File Offset: 0x0001A32C
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (!possibleTarget.isCouncilorState || !(possibleTarget != councilor))
		{
			return "TIMissionCondition_GenericFail";
		}
		TICouncilorState ref_councilor = possibleTarget.ref_councilor;
		TIFactionState faction = ref_councilor.faction;
		if (((faction != null && !faction.permanentAlly(councilor.faction)) || councilor.faction.GetViewofCouncilor(ref_councilor).turned) && councilor.faction.HasIntelOnCouncilorLocation(ref_councilor))
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
