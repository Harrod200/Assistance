using System;

// Token: 0x020003FB RID: 1019
public struct FireModeDataTemplateEntry
{
	// Token: 0x060014D9 RID: 5337 RVA: 0x000661DA File Offset: 0x000643DA
	public FireModeDataTemplateEntry(int slot, FireMode fireMode)
	{
		this.slot = slot;
		this.fireMode = fireMode;
	}

	// Token: 0x0400127B RID: 4731
	public int slot;

	// Token: 0x0400127C RID: 4732
	public FireMode fireMode;
}
