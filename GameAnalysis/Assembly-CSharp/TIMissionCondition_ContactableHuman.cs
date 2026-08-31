using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001BA RID: 442
public class TIMissionCondition_ContactableHuman : TIMissionCondition
{
	// Token: 0x06000652 RID: 1618 RVA: 0x0001CB8C File Offset: 0x0001AD8C
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isCouncilorState && possibleTarget.ref_councilor.isHuman && possibleTarget.ref_councilor.faction != null && councilor.faction.HasIntelOnCouncilorBasicData(possibleTarget.ref_councilor) && TIEffectsState.CheckForAnyEffectInContext(Context.AlienRelationsEstablished, possibleTarget.ref_faction))
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
