using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000207 RID: 519
public class TIMissionModifier_OrgPoolDefense : TIMissionModifier_JointCouncilStat
{
	// Token: 0x0600070D RID: 1805 RVA: 0x00022019 File Offset: 0x00020219
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (target.ref_faction.unassignedOrgs.Contains(target.ref_org))
		{
			return base.GetModifier(attackingCouncilor, target, resourcesSpent, resource);
		}
		return 0f;
	}
}
