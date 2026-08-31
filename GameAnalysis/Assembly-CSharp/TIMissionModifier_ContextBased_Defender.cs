using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001FF RID: 511
public class TIMissionModifier_ContextBased_Defender : TIMissionModifier_ContextBased
{
	// Token: 0x060006F7 RID: 1783 RVA: 0x00021E3C File Offset: 0x0002003C
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (target.ref_faction != null)
		{
			return TIEffectsState.SumEffectsModifiers(this.context, target.ref_faction, 0f, null);
		}
		return 0f;
	}

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x060006F8 RID: 1784 RVA: 0x00021E69 File Offset: 0x00020069
	public override string displayName
	{
		get
		{
			return "Effect";
		}
	}
}
