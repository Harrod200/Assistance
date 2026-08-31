using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001BF RID: 447
public class TIMissionCondition_FreeCouncilor : TIMissionCondition
{
	// Token: 0x0600065E RID: 1630 RVA: 0x0001CEB4 File Offset: 0x0001B0B4
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isCouncilorState && !possibleTarget.ref_councilor.active && possibleTarget.ref_councilor.detained)
		{
			return "TIMissionCondition_FreeCouncilor";
		}
		return "_Pass";
	}
}
