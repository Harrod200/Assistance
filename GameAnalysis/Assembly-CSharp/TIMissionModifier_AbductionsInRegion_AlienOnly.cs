using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000236 RID: 566
public class TIMissionModifier_AbductionsInRegion_AlienOnly : TIMissionModifier_HideInCodex
{
	// Token: 0x06000778 RID: 1912 RVA: 0x00023700 File Offset: 0x00021900
	public override bool ShowCondition(TIFactionState faction)
	{
		return faction.CanDetectAbductions;
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00023708 File Offset: 0x00021908
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIFactionState faction = attackingCouncilor.faction;
		TIRegionState ref_region = target.ref_region;
		if (faction.ideology.alien && ref_region != null)
		{
			return ref_region.GetAbductionsMissionBonusFromRegion();
		}
		return 0f;
	}
}
