using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001FA RID: 506
public class TIMissionModifier_TraitModifier : TIMissionModifier
{
	// Token: 0x060006EA RID: 1770 RVA: 0x00021CBC File Offset: 0x0001FEBC
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (this.attacking)
		{
			return (float)this.trait.ApplyTraitStatValue(this.attribute, attackingCouncilor, attackingCouncilor.faction, WhichStatModifier.ConditionalOnly, true, target);
		}
		if (target != null && target.isCouncilorState)
		{
			return (float)this.trait.ApplyTraitStatValue(this.attribute, target.ref_councilor, attackingCouncilor.faction, WhichStatModifier.ConditionalOnly, true, target);
		}
		return 0f;
	}

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x060006EB RID: 1771 RVA: 0x00021D20 File Offset: 0x0001FF20
	public override string displayName
	{
		get
		{
			return this.trait.displayName;
		}
	}

	// Token: 0x04000621 RID: 1569
	public TITraitTemplate trait;

	// Token: 0x04000622 RID: 1570
	public CouncilorAttribute attribute;

	// Token: 0x04000623 RID: 1571
	public bool attacking;
}
