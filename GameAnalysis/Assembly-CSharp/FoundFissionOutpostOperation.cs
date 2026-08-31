using System;
using System.Collections.Generic;

// Token: 0x0200031F RID: 799
public class FoundFissionOutpostOperation : FoundRegularOutpostFromFleetOperation
{
	// Token: 0x06000CF7 RID: 3319 RVA: 0x00041D74 File Offset: 0x0003FF74
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.FoundFissionOutpost };
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x00041D83 File Offset: 0x0003FF83
	public override int SortOrder()
	{
		return 20;
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x00041D87 File Offset: 0x0003FF87
	public override List<string> AdditionalModules(bool alien)
	{
		if (!alien)
		{
			return new List<string> { "FissionPile", "ConstructionModule" };
		}
		return new List<string> { "AlienFusionPile", "AlienAssembler" };
	}
}
