using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000250 RID: 592
public class TIMissionModifier_PherocyteResistance : TIMissionModifier_HideInCodex
{
	// Token: 0x060007AE RID: 1966 RVA: 0x00024705 File Offset: 0x00022905
	public override bool ShowCondition(TIFactionState faction)
	{
		return faction.completedProjects.Contains(TemplateManager.Find<TIProjectTemplate>("Project_PherocyteResistance", false));
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x0002471D File Offset: 0x0002291D
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (target.ref_faction != null && attackingCouncilor.isAlien)
		{
			return TIEffectsState.SumEffectsModifiers(Context.PherocyteResistance, target.ref_faction, 0f, null);
		}
		return 0f;
	}
}
