using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200025B RID: 603
public class TIMissionModifier_NationSizeEnthrallDifficulty : TIMissionModifier
{
	// Token: 0x060007C9 RID: 1993 RVA: 0x00024A62 File Offset: 0x00022C62
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		return (float)target.ref_nation.numControlPoints_unclamped * TemplateManager.global.enthrallElitesBySizeMultiplier;
	}
}
