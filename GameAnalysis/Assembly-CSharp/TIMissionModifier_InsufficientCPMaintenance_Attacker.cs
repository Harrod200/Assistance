using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000258 RID: 600
public class TIMissionModifier_InsufficientCPMaintenance_Attacker : TIMissionModifier
{
	// Token: 0x060007C2 RID: 1986 RVA: 0x000249C6 File Offset: 0x00022BC6
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		return attackingCouncilor.faction.GetAveragedControlPointCapPenaltyToMissions();
	}
}
