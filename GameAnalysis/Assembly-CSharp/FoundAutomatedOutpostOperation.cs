using System;

// Token: 0x0200034F RID: 847
public class FoundAutomatedOutpostOperation : FoundBaseOperation
{
	// Token: 0x06000EBB RID: 3771 RVA: 0x0004940B File Offset: 0x0004760B
	public override int SortOrder()
	{
		return 11;
	}

	// Token: 0x06000EBC RID: 3772 RVA: 0x0004940F File Offset: 0x0004760F
	public override int GetTier()
	{
		return -1;
	}

	// Token: 0x06000EBD RID: 3773 RVA: 0x00049412 File Offset: 0x00047612
	public override string CoreModuleDataName(bool alien)
	{
		if (!alien)
		{
			return "AutomatedOutpostCore";
		}
		return "";
	}

	// Token: 0x06000EBE RID: 3774 RVA: 0x00049422 File Offset: 0x00047622
	public override Context GetRequiredConstructionTechEffectContext()
	{
		return Context.CanFoundAutomatedT1Base;
	}
}
