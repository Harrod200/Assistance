using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200025D RID: 605
public class TIMissionModifier_HabStability : TIMissionModifier
{
	// Token: 0x060007CE RID: 1998 RVA: 0x00024AFC File Offset: 0x00022CFC
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		if (target.isHabState)
		{
			foreach (TIHabModuleState tihabModuleState in target.ref_hab.ActiveModules())
			{
				if (tihabModuleState.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.Stability))
				{
					num += tihabModuleState.moduleTemplate.specialRulesValue;
				}
			}
		}
		return num;
	}
}
