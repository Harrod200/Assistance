using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001B5 RID: 437
public class TIMissionCondition_NotAlienControlPoint : TIMissionCondition
{
	// Token: 0x06000647 RID: 1607 RVA: 0x0001C9B2 File Offset: 0x0001ABB2
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (!possibleTarget.ref_controlPoint.faction.IsAlienFaction)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
