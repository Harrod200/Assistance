using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000201 RID: 513
public class TIMissionModifier_CouncilorAttackStat : TIMissionModifier_CouncilorStat
{
	// Token: 0x060006FC RID: 1788 RVA: 0x00021E87 File Offset: 0x00020087
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		return (float)attackingCouncilor.GetAttribute(this.attackerAttribute, true, true, true, false, false, false) * this.multiplier;
	}

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x060006FD RID: 1789 RVA: 0x00021EA3 File Offset: 0x000200A3
	public override string displayName
	{
		get
		{
			return TIUtilities.GetAttributeString(this.attackerAttribute);
		}
	}
}
