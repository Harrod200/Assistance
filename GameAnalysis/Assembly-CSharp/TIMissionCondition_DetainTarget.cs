using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001A2 RID: 418
public class TIMissionCondition_DetainTarget : TIMissionCondition
{
	// Token: 0x0600061E RID: 1566 RVA: 0x0001C2B8 File Offset: 0x0001A4B8
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (!possibleTarget.isCouncilorState)
		{
			return "TIMissionCondition_GenericFail";
		}
		TICouncilorState ref_councilor = possibleTarget.ref_councilor;
		if (ref_councilor.faction != councilor.faction && ref_councilor.faction != null && (ref_councilor.isHuman || councilor.faction.CanCaptureAlien) && councilor.faction.HasIntelOnCouncilorBasicData(ref_councilor))
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
