using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000189 RID: 393
public class TIMissionCondition_Human : TIMissionCondition
{
	// Token: 0x060005E8 RID: 1512 RVA: 0x0001B5AE File Offset: 0x000197AE
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (councilor.isHuman)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
