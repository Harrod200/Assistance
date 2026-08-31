using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001B1 RID: 433
public class TIMissionCondition_FreeCouncilSlot : TIMissionCondition
{
	// Token: 0x0600063F RID: 1599 RVA: 0x0001C8E8 File Offset: 0x0001AAE8
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		TIFactionState faction = councilor.faction;
		if (faction.councilors.Count < faction.maxCouncilSize)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
