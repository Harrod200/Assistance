using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000214 RID: 532
public class TIMissionModifier_OwnedControlPoint : TIMissionModifier_StatBased
{
	// Token: 0x06000729 RID: 1833 RVA: 0x0002280C File Offset: 0x00020A0C
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		if (target.isControlPointState)
		{
			if (!target.ref_controlPoint.benefitsDisabled)
			{
				num = TIMissionModifier.CouncilCollectiveDefense(target.ref_faction, this.defenderAttribute) / 2f;
			}
		}
		else if (target.isHabState)
		{
			num = TIMissionModifier.CouncilCollectiveDefense(target.ref_faction, this.defenderAttribute) / 2f;
		}
		return num;
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x0600072A RID: 1834 RVA: 0x0002286F File Offset: 0x00020A6F
	public override string displayName
	{
		get
		{
			return string.Format(Loc.T(base.displayName), TIUtilities.GetAttributeString(this.defenderAttribute));
		}
	}
}
