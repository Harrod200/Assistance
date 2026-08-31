using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000322 RID: 802
public class FoundAutomatedSolarOutpostOperation : FoundAutomatedOutpostFromFleetOperation
{
	// Token: 0x06000D01 RID: 3329 RVA: 0x00041E46 File Offset: 0x00040046
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.FoundAutomatedSolarOutpost };
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x00041E55 File Offset: 0x00040055
	public override int SortOrder()
	{
		return 22;
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x00041E59 File Offset: 0x00040059
	public override List<string> AdditionalModules(bool alien)
	{
		if (!alien)
		{
			return new List<string> { "AutomatedSolarCollector", "AutomatedSolarCollector", "AutomatedMiningComplex" };
		}
		return new List<string> { "" };
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x00041E95 File Offset: 0x00040095
	public override int GetTier()
	{
		return -1;
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x00041E98 File Offset: 0x00040098
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
