using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200018B RID: 395
public class TIMissionCondition_AlienOrProxy : TIMissionCondition
{
	// Token: 0x060005EC RID: 1516 RVA: 0x0001B5F4 File Offset: 0x000197F4
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (!councilor.isAlien)
		{
			TIFactionState ref_faction = councilor.ref_faction;
			if (ref_faction == null || !ref_faction.IsAlienProxy)
			{
				return base.GetType().Name;
			}
		}
		return "_Pass";
	}
}
