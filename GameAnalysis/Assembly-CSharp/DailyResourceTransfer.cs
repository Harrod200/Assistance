using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000150 RID: 336
public class DailyResourceTransfer
{
	// Token: 0x06000524 RID: 1316 RVA: 0x00016780 File Offset: 0x00014980
	public DailyResourceTransfer(TIFactionState targetFaction, TIDateTime expiry, FactionResource resource, float value)
	{
		this.targetFaction = targetFaction;
		this.expiry = expiry;
		this.transfer = new ResourceValue(resource, value);
	}

	// Token: 0x0400023A RID: 570
	public TIFactionState targetFaction;

	// Token: 0x0400023B RID: 571
	public TIDateTime expiry;

	// Token: 0x0400023C RID: 572
	public ResourceValue transfer;
}
