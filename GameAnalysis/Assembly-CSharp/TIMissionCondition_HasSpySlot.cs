using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001AD RID: 429
public class TIMissionCondition_HasSpySlot : TIMissionCondition
{
	// Token: 0x06000637 RID: 1591 RVA: 0x0001C779 File Offset: 0x0001A979
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (councilor.faction.turnedCouncilors.Count < 2)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
