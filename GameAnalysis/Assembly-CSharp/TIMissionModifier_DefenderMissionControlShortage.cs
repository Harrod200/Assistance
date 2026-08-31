using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200023C RID: 572
public class TIMissionModifier_DefenderMissionControlShortage : TIMissionModifier
{
	// Token: 0x06000786 RID: 1926 RVA: 0x00023A45 File Offset: 0x00021C45
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (target.ref_hab != null || target.isSpaceShipState || target.isSpaceFleetState)
		{
			return -target.ref_faction.GetAveragedMissionControlShortage();
		}
		return 0f;
	}
}
