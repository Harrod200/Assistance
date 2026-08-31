using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000257 RID: 599
public class TIMissionModifier_NumSentinelModules : TIMissionModifier
{
	// Token: 0x060007C0 RID: 1984 RVA: 0x00024987 File Offset: 0x00022B87
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		return (float)attackingCouncilor.faction.stations.SelectMany<TIHabState, TIHabModuleState>((TIHabState x) => from x in x.ActiveModules()
			where x.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.SentinelModule)
			select x).Count<TIHabModuleState>();
	}
}
