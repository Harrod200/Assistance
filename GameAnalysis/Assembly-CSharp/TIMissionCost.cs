using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001C8 RID: 456
public abstract class TIMissionCost
{
	// Token: 0x06000671 RID: 1649
	public abstract float GetCost(float bonus, TICouncilorState councilor = null, TIGameState scalingState = null);

	// Token: 0x06000672 RID: 1650 RVA: 0x0001D81E File Offset: 0x0001BA1E
	public virtual bool MeetsCondition(TICouncilorState councilor)
	{
		return true;
	}

	// Token: 0x0400061A RID: 1562
	public FactionResource resourceType;

	// Token: 0x0400061B RID: 1563
	public float value;
}
