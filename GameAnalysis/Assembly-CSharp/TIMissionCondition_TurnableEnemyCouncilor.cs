using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001A1 RID: 417
public class TIMissionCondition_TurnableEnemyCouncilor : TIMissionCondition
{
	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x0600061B RID: 1563 RVA: 0x0001C20A File Offset: 0x0001A40A
	public override List<string> feedback
	{
		get
		{
			return new List<string> { "TIMissionCondition_TurnableEnemyCouncilor2", "TIMissionCondition_TurnableEnemyCouncilor" };
		}
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x0001C228 File Offset: 0x0001A428
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (!possibleTarget.isCouncilorState)
		{
			return "TIMissionCondition_GenericFail";
		}
		TICouncilorState ref_councilor = possibleTarget.ref_councilor;
		if (!(ref_councilor.faction != councilor.faction) || !ref_councilor.isHuman || !(ref_councilor.faction != null))
		{
			return "TIMissionCondition_TurnableEnemyCouncilor";
		}
		if (ref_councilor.agentForFaction != councilor.faction && !councilor.faction.factionsCompromised.Contains(ref_councilor.faction))
		{
			return "_Pass";
		}
		return "TIMissionCondition_TurnableEnemyCouncilor2";
	}
}
