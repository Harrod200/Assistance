using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000216 RID: 534
public class TIMissionModifier_DefendedAsset : TIMissionModifier
{
	// Token: 0x0600072E RID: 1838 RVA: 0x00022948 File Offset: 0x00020B48
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		if (target.isControlPointState && target.ref_controlPoint.defended)
		{
			num = TemplateManager.global.TIMissionModifier_DefendedAsset + TIEffectsState.SumEffectsModifiers(Context.DefendInterestsValue, attackingCouncilor.faction, TemplateManager.global.TIMissionModifier_DefendedAsset, null);
		}
		else if (target.isHabState && target.ref_hab.coreDefended)
		{
			num = TemplateManager.global.TIMissionModifier_DefendedAsset + TIEffectsState.SumEffectsModifiers(Context.DefendInterestsValue, attackingCouncilor.faction, TemplateManager.global.TIMissionModifier_DefendedAsset, null);
		}
		return num;
	}
}
