using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200025C RID: 604
public class TIMissionModifier_ConditionalEnthrallDefendedPoint : TIMissionModifier_HideInCodex
{
	// Token: 0x060007CB RID: 1995 RVA: 0x00024A83 File Offset: 0x00022C83
	public override bool ShowCondition(TIFactionState faction)
	{
		return faction.completedProjects.Contains(TemplateManager.Find<TIProjectTemplate>("Project_TheirMethods", false));
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x00024A9C File Offset: 0x00022C9C
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		if (target.isControlPointState && target.ref_controlPoint.defended)
		{
			num = TemplateManager.global.TIMissionModifier_DefendedAssetConditionalAliens;
		}
		else if (target.isHabState && target.ref_hab.coreDefended)
		{
			num = TemplateManager.global.TIMissionModifier_DefendedAssetConditionalAliens;
		}
		return num;
	}
}
