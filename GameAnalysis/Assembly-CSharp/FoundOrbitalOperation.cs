using System;

// Token: 0x0200034B RID: 843
public class FoundOrbitalOperation : FoundStationOperation
{
	// Token: 0x06000EA6 RID: 3750 RVA: 0x00049288 File Offset: 0x00047488
	public override int SortOrder()
	{
		return 8;
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x0004928B File Offset: 0x0004748B
	public override int GetTier()
	{
		return 2;
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x0004928E File Offset: 0x0004748E
	public override string CoreModuleDataName(bool alien)
	{
		if (!alien)
		{
			return "OrbitalCore";
		}
		return "AlienOrbitalCore";
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x0004929E File Offset: 0x0004749E
	public override Context GetRequiredConstructionTechEffectContext()
	{
		return Context.CanFoundTier2Station;
	}
}
