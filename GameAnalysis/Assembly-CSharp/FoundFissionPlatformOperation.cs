using System;
using System.Collections.Generic;

// Token: 0x02000313 RID: 787
public class FoundFissionPlatformOperation : FoundRegularPlatformFromFleetOperation
{
	// Token: 0x06000CC0 RID: 3264 RVA: 0x000415D4 File Offset: 0x0003F7D4
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.FoundFissionPlatform };
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x000415E3 File Offset: 0x0003F7E3
	public override int SortOrder()
	{
		return 15;
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x000415E7 File Offset: 0x0003F7E7
	public override List<string> AdditionalModules(bool alien)
	{
		if (!alien)
		{
			return new List<string> { "FissionPile", "ConstructionModule" };
		}
		return new List<string> { "AlienFusionPile", "AlienAssembler" };
	}
}
