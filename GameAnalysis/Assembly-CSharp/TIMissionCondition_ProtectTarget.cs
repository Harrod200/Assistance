using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001AF RID: 431
public class TIMissionCondition_ProtectTarget : TIMissionCondition
{
	// Token: 0x0600063B RID: 1595 RVA: 0x0001C7E8 File Offset: 0x0001A9E8
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isCouncilorState)
		{
			if (possibleTarget.ref_councilor.faction == councilor.faction && councilor != possibleTarget)
			{
				return "_Pass";
			}
			if (councilor.faction.IsAlienProxy && possibleTarget.ref_councilor.isAlien && councilor.faction.HasIntelOnCouncilorDetails(possibleTarget.ref_councilor))
			{
				return "_Pass";
			}
			return base.GetType().Name;
		}
		else if (possibleTarget.isHabState)
		{
			if (possibleTarget.ref_factions.Contains(councilor.faction))
			{
				return "_Pass";
			}
			return base.GetType().Name;
		}
		else
		{
			if (possibleTarget.isRegionState)
			{
				return "_Pass";
			}
			return "TIMissionCondition_GenericFail";
		}
	}
}
