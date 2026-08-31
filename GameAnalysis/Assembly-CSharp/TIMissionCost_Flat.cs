using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001CA RID: 458
public class TIMissionCost_Flat : TIMissionCost
{
	// Token: 0x06000676 RID: 1654 RVA: 0x0001D897 File Offset: 0x0001BA97
	public override float GetCost(float bonus, TICouncilorState councilor = null, TIGameState scalingState = null)
	{
		if (!this.MeetsCondition(councilor))
		{
			return 0f;
		}
		return this.value;
	}
}
