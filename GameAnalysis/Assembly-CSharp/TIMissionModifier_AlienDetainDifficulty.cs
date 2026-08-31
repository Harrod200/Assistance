using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000255 RID: 597
public class TIMissionModifier_AlienDetainDifficulty : TIMissionModifier_HideInCodex
{
	// Token: 0x060007BB RID: 1979 RVA: 0x000248AB File Offset: 0x00022AAB
	public override bool ShowCondition(TIFactionState faction)
	{
		return faction.finishedProjectNames.Contains("Project_AlienContainment");
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x000248BD File Offset: 0x00022ABD
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (target.isCouncilorState && target.ref_councilor.isAlien)
		{
			return 6f;
		}
		return 0f;
	}
}
