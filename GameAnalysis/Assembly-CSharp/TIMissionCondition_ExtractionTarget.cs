using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001A3 RID: 419
public class TIMissionCondition_ExtractionTarget : TIMissionCondition
{
	// Token: 0x06000620 RID: 1568 RVA: 0x0001C338 File Offset: 0x0001A538
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		TICouncilorState ref_councilor = possibleTarget.ref_councilor;
		if (ref_councilor.detained && ref_councilor.faction == councilor.faction && ref_councilor.detainingFaction != councilor.faction)
		{
			return "_Pass";
		}
		return "TIMissionCondition_ExtractionTarget";
	}
}
