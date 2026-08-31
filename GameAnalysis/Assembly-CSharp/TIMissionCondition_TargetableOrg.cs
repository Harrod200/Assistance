using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200019E RID: 414
public class TIMissionCondition_TargetableOrg : TIMissionCondition
{
	// Token: 0x06000615 RID: 1557 RVA: 0x0001C07C File Offset: 0x0001A27C
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isOrgState)
		{
			if (possibleTarget.ref_org.hasFactionbutNoCouncilor && councilor.faction != possibleTarget.ref_org.factionOrbit && councilor.faction.GetViewofFaction(possibleTarget.ref_org.factionOrbit).knownUnassignedOrgsPool.Contains(possibleTarget.ref_org))
			{
				return "_Pass";
			}
			if (councilor.faction.GetViewofCouncilor(possibleTarget.ref_councilor).orgs.Contains(possibleTarget.ref_org))
			{
				return "_Pass";
			}
		}
		return base.GetType().Name;
	}
}
