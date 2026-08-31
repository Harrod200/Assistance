using System;

// Token: 0x02000351 RID: 849
public class FoundColonyOperation : FoundBaseOperation
{
	// Token: 0x06000EC5 RID: 3781 RVA: 0x00049457 File Offset: 0x00047657
	public override int SortOrder()
	{
		return 13;
	}

	// Token: 0x06000EC6 RID: 3782 RVA: 0x0004945B File Offset: 0x0004765B
	public override int GetTier()
	{
		return 3;
	}

	// Token: 0x06000EC7 RID: 3783 RVA: 0x0004945E File Offset: 0x0004765E
	public override string CoreModuleDataName(bool alien)
	{
		if (!alien)
		{
			return "ColonyCore";
		}
		return "AlienColonyCore";
	}

	// Token: 0x06000EC8 RID: 3784 RVA: 0x0004946E File Offset: 0x0004766E
	public override Context GetRequiredConstructionTechEffectContext()
	{
		return Context.CanFoundTier3Base;
	}
}
