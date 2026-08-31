using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001AA RID: 426
public class TIMissionCondition_HasIntelOnCouncilorSecrets : TIMissionCondition
{
	// Token: 0x06000631 RID: 1585 RVA: 0x0001C6B4 File Offset: 0x0001A8B4
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (!possibleTarget.isCouncilorState)
		{
			return "TIMissionCondition_GenericFail";
		}
		TICouncilorState ref_councilor = possibleTarget.ref_councilor;
		if (((ref_councilor != null) ? ref_councilor.faction : null) != null && councilor.faction.HasIntelOnCouncilorSecrets(ref_councilor))
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
