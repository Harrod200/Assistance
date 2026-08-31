using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001A9 RID: 425
public class TIMissionCondition_BasicCouncilorIntelOrHab : TIMissionCondition
{
	// Token: 0x170000EA RID: 234
	// (get) Token: 0x0600062E RID: 1582 RVA: 0x0001C661 File Offset: 0x0001A861
	public override List<string> feedback
	{
		get
		{
			return new List<string> { "TIMissionCondition_BasicCouncilorIntel" };
		}
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x0001C673 File Offset: 0x0001A873
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isCouncilorState)
		{
			return this.basicCouncilorIntelCondition.CanTarget(councilor, possibleTarget);
		}
		if (possibleTarget.isHabState)
		{
			return "_Pass";
		}
		return "TIMissionCondition_GenericFail";
	}

	// Token: 0x04000619 RID: 1561
	private TIMissionCondition_BasicCouncilorIntel basicCouncilorIntelCondition = new TIMissionCondition_BasicCouncilorIntel();
}
