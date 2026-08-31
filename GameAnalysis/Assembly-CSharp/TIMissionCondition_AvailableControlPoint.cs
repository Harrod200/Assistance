using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200018C RID: 396
public class TIMissionCondition_AvailableControlPoint : TIMissionCondition
{
	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x060005EE RID: 1518 RVA: 0x0001B62B File Offset: 0x0001982B
	public override List<string> feedback
	{
		get
		{
			return new List<string> { "TIMissionCondition_AvailableControlPoint", "TIMissionCondition_AvailableControlPoint2" };
		}
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x0001B648 File Offset: 0x00019848
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		TINationState ref_nation = possibleTarget.ref_nation;
		if (ref_nation.NumNativeControlPoints <= 0)
		{
			return "TIMissionCondition_AvailableControlPoint";
		}
		if (ref_nation.NumNativeControlPoints != 1)
		{
			return "_Pass";
		}
		if (ref_nation.executiveControlPoint.CanBeAttacked(councilor.faction))
		{
			return "_Pass";
		}
		return "TIMissionCondition_AvailableControlPoint2";
	}
}
