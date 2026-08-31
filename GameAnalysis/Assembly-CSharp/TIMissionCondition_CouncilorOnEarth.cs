using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000197 RID: 407
public class TIMissionCondition_CouncilorOnEarth : TIMissionCondition
{
	// Token: 0x06000607 RID: 1543 RVA: 0x0001BB30 File Offset: 0x00019D30
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (councilor.OnEarth)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
