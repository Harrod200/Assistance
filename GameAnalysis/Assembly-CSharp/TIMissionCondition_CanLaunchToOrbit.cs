using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000198 RID: 408
public class TIMissionCondition_CanLaunchToOrbit : TIMissionCondition
{
	// Token: 0x06000609 RID: 1545 RVA: 0x0001BB53 File Offset: 0x00019D53
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if ((councilor.isHuman && councilor.OnEarth) || councilor.AtABase)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
