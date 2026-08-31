using System;

// Token: 0x02000349 RID: 841
public class FoundPlatformOperation : FoundStationOperation
{
	// Token: 0x06000E9C RID: 3740 RVA: 0x0004923E File Offset: 0x0004743E
	public override int SortOrder()
	{
		return 6;
	}

	// Token: 0x06000E9D RID: 3741 RVA: 0x00049241 File Offset: 0x00047441
	public override int GetTier()
	{
		return 1;
	}

	// Token: 0x06000E9E RID: 3742 RVA: 0x00049244 File Offset: 0x00047444
	public override string CoreModuleDataName(bool alien)
	{
		if (!alien)
		{
			return "PlatformCore";
		}
		return "AlienPlatformCore";
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x00049254 File Offset: 0x00047454
	public override Context GetRequiredConstructionTechEffectContext()
	{
		return Context.CanFoundTier1Station;
	}
}
