using System;
using System.Collections.Generic;

// Token: 0x02000323 RID: 803
public class FoundAutomatedFissionOutpostOperation : FoundAutomatedOutpostFromFleetOperation
{
	// Token: 0x06000D07 RID: 3335 RVA: 0x00041F30 File Offset: 0x00040130
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.FoundAutomatedFissionOutpost };
	}

	// Token: 0x06000D08 RID: 3336 RVA: 0x00041F3F File Offset: 0x0004013F
	public override int SortOrder()
	{
		return 23;
	}

	// Token: 0x06000D09 RID: 3337 RVA: 0x00041F43 File Offset: 0x00040143
	public override List<string> AdditionalModules(bool alien)
	{
		if (!alien)
		{
			return new List<string> { "AutomatedFissionPile", "AutomatedFissionPile", "AutomatedMiningComplex" };
		}
		return new List<string> { "" };
	}

	// Token: 0x06000D0A RID: 3338 RVA: 0x00041F7F File Offset: 0x0004017F
	public override int GetTier()
	{
		return -1;
	}
}
