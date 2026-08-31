using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001AE RID: 430
public class TIMissionCondition_MyFactionCouncilor : TIMissionCondition
{
	// Token: 0x06000639 RID: 1593 RVA: 0x0001C7A7 File Offset: 0x0001A9A7
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isCouncilorState && possibleTarget.ref_councilor.faction == councilor.faction && councilor != possibleTarget)
		{
			return "_Pass";
		}
		return "TIMissionCondition_MyFactionCouncilor";
	}
}
