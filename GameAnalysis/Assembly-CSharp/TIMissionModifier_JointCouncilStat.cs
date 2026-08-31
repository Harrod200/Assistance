using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000213 RID: 531
public class TIMissionModifier_JointCouncilStat : TIMissionModifier_StatBased
{
	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000726 RID: 1830 RVA: 0x000227CD File Offset: 0x000209CD
	public override string displayName
	{
		get
		{
			return string.Format(Loc.T(base.displayName), TIUtilities.GetAttributeString(this.defenderAttribute));
		}
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x000227EA File Offset: 0x000209EA
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		return TIMissionModifier.CouncilCollectiveDefense(target.ref_faction, this.defenderAttribute) / 2f;
	}
}
