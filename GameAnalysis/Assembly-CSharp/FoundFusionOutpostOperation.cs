using System;
using System.Collections.Generic;

// Token: 0x02000320 RID: 800
public class FoundFusionOutpostOperation : FoundRegularOutpostFromFleetOperation
{
	// Token: 0x06000CFB RID: 3323 RVA: 0x00041DCB File Offset: 0x0003FFCB
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.FoundFusionOutpost };
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x00041DDA File Offset: 0x0003FFDA
	public override int SortOrder()
	{
		return 21;
	}

	// Token: 0x06000CFD RID: 3325 RVA: 0x00041DDE File Offset: 0x0003FFDE
	public override List<string> AdditionalModules(bool alien)
	{
		if (!alien)
		{
			return new List<string> { "FusionPile", "ConstructionModule" };
		}
		return new List<string> { "AlienFusionPile", "AlienAssembler" };
	}
}
