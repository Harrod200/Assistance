using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200021B RID: 539
public class TIMissionModifier_ProtectionBonus : TIMissionModifier_StatBased
{
	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06000738 RID: 1848 RVA: 0x00022BA9 File Offset: 0x00020DA9
	public override string displayName
	{
		get
		{
			return string.Format(Loc.T(base.displayName), TIUtilities.GetAttributeString(this.defenderAttribute));
		}
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x00022BC8 File Offset: 0x00020DC8
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (attackingCouncilor.faction != target.ref_faction)
		{
			if (target.isCouncilorState || (target.isOrgState && target.ref_councilor != null))
			{
				return target.ref_councilor.GetProtectionBonus(this.defenderAttribute);
			}
			if (target.isHabModuleState || target.isHabState)
			{
				return target.ref_hab.GetProtectionBonus(this.defenderAttribute);
			}
			if (target.isRegionState || target.isRegionSpaceFacility || target.isRegionAlienAsset)
			{
				return target.ref_region.GetProtectionBonus(this.defenderAttribute);
			}
		}
		return 0f;
	}
}
