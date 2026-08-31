using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000194 RID: 404
public class TIMissionCondition_ScannableObjectWithMyControlPoints : TIMissionCondition
{
	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x06000600 RID: 1536 RVA: 0x0001B887 File Offset: 0x00019A87
	public override List<string> feedback
	{
		get
		{
			return new List<string> { "TIMissionCondition_ScannableObjectWithMyControlPoints1", "TIMissionCondition_ScannableObjectWithMyControlPoints2", "TIMissionCondition_ScannableObjectWithMyControlPoints3" };
		}
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x0001B8B0 File Offset: 0x00019AB0
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isNationState)
		{
			if (possibleTarget.ref_nation.FactionsWithControlPoint.Contains(councilor.faction))
			{
				return "_Pass";
			}
			return "TIMissionCondition_ScannableObjectWithMyControlPoints1";
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
