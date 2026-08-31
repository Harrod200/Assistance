using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200023B RID: 571
public class TIMissionModifier_AttackerMissionControlShortage : TIMissionModifier
{
	// Token: 0x06000784 RID: 1924 RVA: 0x00023A0B File Offset: 0x00021C0B
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (target.ref_hab != null || target.isSpaceShipState || target.isSpaceFleetState)
		{
			return -attackingCouncilor.ref_faction.GetAveragedMissionControlShortage();
		}
		return 0f;
	}
}
