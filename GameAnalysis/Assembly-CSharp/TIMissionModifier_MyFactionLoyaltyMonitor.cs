using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000254 RID: 596
public class TIMissionModifier_MyFactionLoyaltyMonitor : TIMissionModifier_HideInCodex
{
	// Token: 0x060007B8 RID: 1976 RVA: 0x00024852 File Offset: 0x00022A52
	public override bool ShowCondition(TIFactionState faction)
	{
		return faction.finishedProjectNames.Contains("Project_CyberneticImplants");
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x00024864 File Offset: 0x00022A64
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (target.isCouncilorState && attackingCouncilor.faction == target.ref_faction && target.ref_councilor.traitTemplateNames.Contains("LoyaltyMonitor"))
		{
			return -50f;
		}
		return 0f;
	}
}
