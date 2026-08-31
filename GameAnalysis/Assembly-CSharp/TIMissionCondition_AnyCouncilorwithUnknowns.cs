using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001A7 RID: 423
public class TIMissionCondition_AnyCouncilorwithUnknowns : TIMissionCondition
{
	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x06000629 RID: 1577 RVA: 0x0001C544 File Offset: 0x0001A744
	public override List<string> feedback
	{
		get
		{
			return new List<string>
			{
				base.GetType().Name,
				new StringBuilder(base.GetType().Name).Append("2").ToString()
			};
		}
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x0001C584 File Offset: 0x0001A784
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (!possibleTarget.isCouncilorState)
		{
			return "TIMissionCondition_GenericFail";
		}
		TICouncilorState ref_councilor = possibleTarget.ref_councilor;
		if (ref_councilor != null && ref_councilor.faction != null && councilor.faction.HasIntelOnCouncilorLocation(ref_councilor) && councilor != ref_councilor)
		{
			return "_Pass";
		}
		return new StringBuilder(base.GetType().Name).Append("2").ToString();
	}
}
