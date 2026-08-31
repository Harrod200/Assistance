using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001A8 RID: 424
public class TIMissionCondition_BasicCouncilorIntel : TIMissionCondition
{
	// Token: 0x0600062C RID: 1580 RVA: 0x0001C604 File Offset: 0x0001A804
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (!possibleTarget.isCouncilorState)
		{
			return "TIMissionCondition_GenericFail";
		}
		TICouncilorState ref_councilor = possibleTarget.ref_councilor;
		if (((ref_councilor != null) ? ref_councilor.faction : null) != null && councilor.faction.HasIntelOnCouncilorBasicData(ref_councilor))
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
