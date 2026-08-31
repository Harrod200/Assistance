using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000218 RID: 536
public class TIMissionModifier_LocalPassiveStatDefense : TIMissionModifier_StatBased
{
	// Token: 0x06000732 RID: 1842 RVA: 0x00022A48 File Offset: 0x00020C48
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		if (target.ref_faction == null)
		{
			return num;
		}
		foreach (TICouncilorState ticouncilorState in target.ref_faction.councilors)
		{
			if (ticouncilorState != target && TIMissionPhaseState.CouncilorLastKnownLocation(attackingCouncilor.faction, ticouncilorState) == TIUtilities.ObjectToExactLocation(target))
			{
				num += (float)ticouncilorState.GetAttribute(this.defenderAttribute, true, true, true, false, false, false);
			}
		}
		return num;
	}
}
