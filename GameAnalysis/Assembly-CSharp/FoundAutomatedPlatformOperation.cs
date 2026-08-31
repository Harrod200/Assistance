using System;

// Token: 0x0200034A RID: 842
public class FoundAutomatedPlatformOperation : FoundStationOperation
{
	// Token: 0x06000EA1 RID: 3745 RVA: 0x00049263 File Offset: 0x00047463
	public override int SortOrder()
	{
		return 7;
	}

	// Token: 0x06000EA2 RID: 3746 RVA: 0x00049266 File Offset: 0x00047466
	public override int GetTier()
	{
		return -1;
	}

	// Token: 0x06000EA3 RID: 3747 RVA: 0x00049269 File Offset: 0x00047469
	public override string CoreModuleDataName(bool alien)
	{
		if (!alien)
		{
			return "AutomatedPlatformCore";
		}
		return "";
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x00049279 File Offset: 0x00047479
	public override Context GetRequiredConstructionTechEffectContext()
	{
		return Context.CanFoundAutomatedT1Station;
	}
}
