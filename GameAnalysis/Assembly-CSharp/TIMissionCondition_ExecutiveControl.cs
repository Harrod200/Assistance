using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001B6 RID: 438
public class TIMissionCondition_ExecutiveControl : TIMissionCondition
{
	// Token: 0x06000649 RID: 1609 RVA: 0x0001C9E0 File Offset: 0x0001ABE0
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		TINationState ref_nation = possibleTarget.ref_nation;
		if (councilor.faction == ref_nation.executiveFaction && !ref_nation.executiveControlPoint.benefitsDisabled)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
