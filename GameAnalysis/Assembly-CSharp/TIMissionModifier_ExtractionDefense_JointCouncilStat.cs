using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200021E RID: 542
public class TIMissionModifier_ExtractionDefense_JointCouncilStat : TIMissionModifier_StatBased
{
	// Token: 0x17000101 RID: 257
	// (get) Token: 0x06000740 RID: 1856 RVA: 0x00022DEC File Offset: 0x00020FEC
	public override string displayName
	{
		get
		{
			return string.Format(Loc.T(base.displayName), TIUtilities.GetAttributeString(this.defenderAttribute));
		}
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x00022E09 File Offset: 0x00021009
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		return TIMissionModifier.CouncilCollectiveDefense(target.ref_councilor.detainingFaction, this.defenderAttribute) / 2f;
	}
}
