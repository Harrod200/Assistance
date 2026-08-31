using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001C7 RID: 455
public class TIMissionCondition_VictoryCondition : TIMissionCondition
{
	// Token: 0x0600066F RID: 1647 RVA: 0x0001D7E4 File Offset: 0x0001B9E4
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (councilor.ref_faction.unlockedVictoryObjective && councilor.ref_faction.victoryTemplate.AllVictoryConditionsMet(councilor.ref_faction))
		{
			return "_Pass";
		}
		return "TIMissionCondition_VictoryCondition";
	}
}
