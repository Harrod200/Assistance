using System;
using System.Collections.Generic;

// Token: 0x02000314 RID: 788
public class FoundFusionPlatformOperation : FoundRegularPlatformFromFleetOperation
{
	// Token: 0x06000CC4 RID: 3268 RVA: 0x0004162B File Offset: 0x0003F82B
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.FoundFusionPlatform };
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x0004163A File Offset: 0x0003F83A
	public override int SortOrder()
	{
		return 16;
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x0004163E File Offset: 0x0003F83E
	public override List<string> AdditionalModules(bool alien)
	{
		if (!alien)
		{
			return new List<string> { "FusionPile", "ConstructionModule" };
		}
		return new List<string> { "AlienFusionPile", "AlienAssembler" };
	}
}
