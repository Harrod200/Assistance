using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001CB RID: 459
public class TIMissionCost_FlatOnEarth : TIMissionCost_Flat
{
	// Token: 0x06000678 RID: 1656 RVA: 0x0001D8B6 File Offset: 0x0001BAB6
	public override bool MeetsCondition(TICouncilorState councilor)
	{
		return councilor != null && councilor.OnEarth;
	}
}
