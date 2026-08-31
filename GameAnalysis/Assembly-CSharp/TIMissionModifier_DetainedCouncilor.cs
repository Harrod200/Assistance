using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200021A RID: 538
public class TIMissionModifier_DetainedCouncilor : TIMissionModifier
{
	// Token: 0x06000736 RID: 1846 RVA: 0x00022B58 File Offset: 0x00020D58
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TICouncilorState ref_councilor = target.ref_councilor;
		if (ref_councilor != null)
		{
			if (ref_councilor.detainingFaction == attackingCouncilor.faction)
			{
				return 8f;
			}
			if (ref_councilor.detained)
			{
				return 4f;
			}
		}
		return 0f;
	}
}
