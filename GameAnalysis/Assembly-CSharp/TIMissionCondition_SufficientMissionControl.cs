using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001C3 RID: 451
public class TIMissionCondition_SufficientMissionControl : TIMissionCondition
{
	// Token: 0x06000667 RID: 1639 RVA: 0x0001D4BC File Offset: 0x0001B6BC
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		int missionControlBalance = councilor.faction.MissionControlBalance;
		if (possibleTarget.isHabState)
		{
			int num = possibleTarget.ref_hab.MissionControlCost(true, possibleTarget.ref_hab.coreFaction);
			if (num <= 0 || missionControlBalance >= num)
			{
				return "_Pass";
			}
		}
		else if (possibleTarget.isSpaceShipState)
		{
			int missionControlConsumption = possibleTarget.ref_ship.missionControlConsumption;
			if (missionControlConsumption <= 0 || missionControlBalance >= missionControlConsumption)
			{
				return "_Pass";
			}
		}
		return "TIMissionCondition_SufficientMissionControl";
	}
}
