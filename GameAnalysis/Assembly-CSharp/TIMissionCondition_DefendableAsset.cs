using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000193 RID: 403
public class TIMissionCondition_DefendableAsset : TIMissionCondition
{
	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x060005FD RID: 1533 RVA: 0x0001B7B8 File Offset: 0x000199B8
	public override List<string> feedback
	{
		get
		{
			return new List<string> { "TIMissionCondition_DefendableAsset", "TIMissionCondition_ScannableObjectWithMyControlPoints2", "TIMissionCondition_ScannableObjectWithMyControlPoints3" };
		}
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x0001B7E0 File Offset: 0x000199E0
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isNationState)
		{
			if (possibleTarget.ref_nation.FactionControlPoints(councilor.faction, false, false, true).Count > 0)
			{
				return "_Pass";
			}
			return "TIMissionCondition_DefendableAsset";
		}
		else if (possibleTarget.isHabState)
		{
			if (possibleTarget.ref_hab.faction == councilor.faction)
			{
				return "_Pass";
			}
			return "TIMissionCondition_ScannableObjectWithMyControlPoints2";
		}
		else
		{
			if (!possibleTarget.isSpaceShipState)
			{
				return "TIMissionCondition_GenericFail";
			}
			if (possibleTarget.ref_ship.fleet.faction == councilor.faction)
			{
				return "_Pass";
			}
			return "TIMissionCondition_ScannableObjectWithMyControlPoints3";
		}
	}
}
