using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001BD RID: 445
public class TIMissionCondition_MinimumUnrest : TIMissionCondition
{
	// Token: 0x06000659 RID: 1625 RVA: 0x0001CD98 File Offset: 0x0001AF98
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		TINationState ref_nation = possibleTarget.ref_nation;
		if (ref_nation != null && ref_nation.unrest > 0f)
		{
			return "_Pass";
		}
		return "TIMissionCondition_MinimumUnrest";
	}
}
