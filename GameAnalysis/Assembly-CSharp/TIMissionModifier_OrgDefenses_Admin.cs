using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200025F RID: 607
public class TIMissionModifier_OrgDefenses_Admin : TIMissionModifier
{
	// Token: 0x060007D2 RID: 2002 RVA: 0x00024BC4 File Offset: 0x00022DC4
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (target.isOrgState)
		{
			return (float)target.ref_org.administration * 2f;
		}
		return 0f;
	}
}
