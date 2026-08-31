using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000312 RID: 786
public class FoundSolarPlatformOperation : FoundRegularPlatformFromFleetOperation
{
	// Token: 0x06000CBB RID: 3259 RVA: 0x000414EF File Offset: 0x0003F6EF
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.FoundSolarPlatform };
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x000414FE File Offset: 0x0003F6FE
	public override int SortOrder()
	{
		return 14;
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x00041502 File Offset: 0x0003F702
	public override List<string> AdditionalModules(bool alien)
	{
		if (!alien)
		{
			return new List<string> { "SolarCollector", "ConstructionModule" };
		}
		return new List<string> { "AlienFusionPile", "AlienAssembler" };
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x00041540 File Offset: 0x0003F740
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
