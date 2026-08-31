using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200025E RID: 606
public class TIMissionModifier_OrgDefenses : TIMissionModifier
{
	// Token: 0x060007D0 RID: 2000 RVA: 0x00024B88 File Offset: 0x00022D88
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		if (target.isOrgState)
		{
			return (float)target.ref_org.tier * TemplateManager.global.TIMissionModifier_OrgDefenses;
		}
		return num;
	}
}
