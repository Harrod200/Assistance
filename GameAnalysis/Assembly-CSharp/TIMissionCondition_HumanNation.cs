using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000190 RID: 400
public class TIMissionCondition_HumanNation : TIMissionCondition
{
	// Token: 0x060005F7 RID: 1527 RVA: 0x0001B71D File Offset: 0x0001991D
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (!possibleTarget.ref_nation.alienNation)
		{
			return "_Pass";
		}
		return "TIMissionCondition_HumanNation";
	}
}
