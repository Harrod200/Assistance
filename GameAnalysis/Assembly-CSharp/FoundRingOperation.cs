using System;

// Token: 0x0200034C RID: 844
public class FoundRingOperation : FoundStationOperation
{
	// Token: 0x06000EAB RID: 3755 RVA: 0x000492AD File Offset: 0x000474AD
	public override int SortOrder()
	{
		return 9;
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x000492B1 File Offset: 0x000474B1
	public override int GetTier()
	{
		return 3;
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x000492B4 File Offset: 0x000474B4
	public override string CoreModuleDataName(bool alien)
	{
		if (!alien)
		{
			return "RingCore";
		}
		return "AlienRingCore";
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x000492C4 File Offset: 0x000474C4
	public override Context GetRequiredConstructionTechEffectContext()
	{
		return Context.CanFoundTier3Station;
	}
}
