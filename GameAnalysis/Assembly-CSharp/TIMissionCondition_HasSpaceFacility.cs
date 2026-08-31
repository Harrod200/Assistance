using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001B2 RID: 434
public class TIMissionCondition_HasSpaceFacility : TIMissionCondition
{
	// Token: 0x06000641 RID: 1601 RVA: 0x0001C928 File Offset: 0x0001AB28
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isRegionState && possibleTarget.ref_region.hasAnySpaceFacility)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
