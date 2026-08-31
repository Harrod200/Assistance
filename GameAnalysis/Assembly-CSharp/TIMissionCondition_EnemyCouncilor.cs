using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001A0 RID: 416
public class TIMissionCondition_EnemyCouncilor : TIMissionCondition
{
	// Token: 0x06000619 RID: 1561 RVA: 0x0001C1B4 File Offset: 0x0001A3B4
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (!possibleTarget.isCouncilorState)
		{
			return "TIMissionCondition_GenericFail";
		}
		TIFactionState faction = possibleTarget.ref_councilor.faction;
		if (faction != null && !faction.permanentAlly(councilor.faction))
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
