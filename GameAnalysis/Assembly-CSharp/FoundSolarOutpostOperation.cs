using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200031E RID: 798
public class FoundSolarOutpostOperation : FoundRegularOutpostFromFleetOperation
{
	// Token: 0x06000CF2 RID: 3314 RVA: 0x00041C8A File Offset: 0x0003FE8A
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.FoundSolarOutpost };
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x00041C99 File Offset: 0x0003FE99
	public override int SortOrder()
	{
		return 19;
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x00041C9D File Offset: 0x0003FE9D
	public override List<string> AdditionalModules(bool alien)
	{
		if (!alien)
		{
			return new List<string> { "SolarCollector", "ConstructionModule" };
		}
		return new List<string> { "AlienFusionPile", "AlienAssembler" };
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x00041CDC File Offset: 0x0003FEDC
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		if (base.ActorCanPerformOperation(actorState, target))
		{
			float num = 0f;
			foreach (string text in this.AdditionalModules(actorState.ref_faction.IsAlienFaction))
			{
				TIHabModuleTemplate tihabModuleTemplate = TemplateManager.Find<TIHabModuleTemplate>(text, false);
				num += (float)tihabModuleTemplate.ProspectivePower(actorState.ref_spaceBody, actorState.ref_faction);
			}
			return num >= 0f;
		}
		return false;
	}
}
