using System;

// Token: 0x02000350 RID: 848
public class FoundSettlementOperation : FoundBaseOperation
{
	// Token: 0x06000EC0 RID: 3776 RVA: 0x00049431 File Offset: 0x00047631
	public override int SortOrder()
	{
		return 12;
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x00049435 File Offset: 0x00047635
	public override int GetTier()
	{
		return 2;
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x00049438 File Offset: 0x00047638
	public override string CoreModuleDataName(bool alien)
	{
		if (!alien)
		{
			return "SettlementCore";
		}
		return "AlienSettlementCore";
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x00049448 File Offset: 0x00047648
	public override Context GetRequiredConstructionTechEffectContext()
	{
		return Context.CanFoundTier2Base;
	}
}
