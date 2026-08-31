using System;

// Token: 0x0200034E RID: 846
public class FoundOutpostOperation : FoundBaseOperation
{
	// Token: 0x06000EB6 RID: 3766 RVA: 0x000493E5 File Offset: 0x000475E5
	public override int SortOrder()
	{
		return 10;
	}

	// Token: 0x06000EB7 RID: 3767 RVA: 0x000493E9 File Offset: 0x000475E9
	public override int GetTier()
	{
		return 1;
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x000493EC File Offset: 0x000475EC
	public override string CoreModuleDataName(bool alien)
	{
		if (!alien)
		{
			return "OutpostCore";
		}
		return "AlienOutpostCore";
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x000493FC File Offset: 0x000475FC
	public override Context GetRequiredConstructionTechEffectContext()
	{
		return Context.CanFoundTier1Base;
	}
}
