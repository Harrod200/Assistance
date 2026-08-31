using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200031A RID: 794
public class FoundAutomatedSolarPlatformOperation : FoundAutomatedPlatformFromFleetOperation
{
	// Token: 0x06000CDF RID: 3295 RVA: 0x0004189A File Offset: 0x0003FA9A
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.FoundAutomatedSolarPlatform };
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x000418A9 File Offset: 0x0003FAA9
	public override int SortOrder()
	{
		return 17;
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x000418AD File Offset: 0x0003FAAD
	public override List<string> AdditionalModules(bool alien)
	{
		if (!alien)
		{
			return new List<string> { "AutomatedSolarCollector", "AutomatedSupplyDepot" };
		}
		return new List<string> { "" };
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x000418DE File Offset: 0x0003FADE
	public override int GetTier()
	{
		return -1;
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x000418E4 File Offset: 0x0003FAE4
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		if (base.ActorCanPerformOperation(actorState, target))
		{
			float num = 0f;
			foreach (string text in this.AdditionalModules(actorState.ref_faction.IsAlienFaction))
			{
				TIHabModuleTemplate tihabModuleTemplate = TemplateManager.Find<TIHabModuleTemplate>(text, false);
				num += (float)tihabModuleTemplate.ProspectivePower(actorState.ref_orbit);
			}
			return num >= 0f;
		}
		return false;
	}
}
