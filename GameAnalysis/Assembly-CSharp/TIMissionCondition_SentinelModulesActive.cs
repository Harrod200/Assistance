using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001C5 RID: 453
public class TIMissionCondition_SentinelModulesActive : TIMissionCondition
{
	// Token: 0x0600066B RID: 1643 RVA: 0x0001D5A0 File Offset: 0x0001B7A0
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		foreach (TIHabState tihabState in councilor.faction.LEOStations)
		{
			if (tihabState.ActiveModules().Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.SentinelModule)))
			{
				return "_Pass";
			}
		}
		return "TIMissionCondition_SentinelModulesActive";
	}
}
