using System;
using System.Collections.Generic;

// Token: 0x0200031B RID: 795
public class FoundAutomatedFissionPlatformOperation : FoundAutomatedPlatformFromFleetOperation
{
	// Token: 0x06000CE5 RID: 3301 RVA: 0x00041978 File Offset: 0x0003FB78
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.FoundAutomatedFissionPlatform };
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x00041987 File Offset: 0x0003FB87
	public override int SortOrder()
	{
		return 18;
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x0004198B File Offset: 0x0003FB8B
	public override List<string> AdditionalModules(bool alien)
	{
		if (!alien)
		{
			return new List<string> { "AutomatedFissionPile", "AutomatedSupplyDepot" };
		}
		return new List<string> { "" };
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x000419BC File Offset: 0x0003FBBC
	public override int GetTier()
	{
		return -1;
	}
}
