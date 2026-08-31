using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001BB RID: 443
public class TIMissionCondition_AlienFacilityInNation : TIMissionCondition
{
	// Token: 0x06000654 RID: 1620 RVA: 0x0001CBFB File Offset: 0x0001ADFB
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		TINationState ref_nation = possibleTarget.ref_nation;
		if (ref_nation != null && ref_nation.hasAlienFacility)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
