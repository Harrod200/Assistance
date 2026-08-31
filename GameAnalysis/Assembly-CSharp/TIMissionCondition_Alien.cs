using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200018A RID: 394
public class TIMissionCondition_Alien : TIMissionCondition
{
	// Token: 0x060005EA RID: 1514 RVA: 0x0001B5D1 File Offset: 0x000197D1
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (councilor.isAlien)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
