using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000259 RID: 601
public class TIMissionModifier_InsufficientCPMaintenance_Defender : TIMissionModifier
{
	// Token: 0x060007C4 RID: 1988 RVA: 0x000249DB File Offset: 0x00022BDB
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIFactionState ref_faction = target.ref_faction;
		if (ref_faction == null)
		{
			return 0f;
		}
		return ref_faction.GetAveragedControlPointCapPenaltyToMissions();
	}
}
