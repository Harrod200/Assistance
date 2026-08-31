using System;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001FB RID: 507
public class TIMissionModifier_CampaignDifficulty_Attacker : TIMissionModifier
{
	// Token: 0x060006ED RID: 1773 RVA: 0x00021D35 File Offset: 0x0001FF35
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		return TemplateManager.global.AI_GlobalMissionDifficultyModifier_Att(attackingCouncilor, target);
	}

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x060006EE RID: 1774 RVA: 0x00021D44 File Offset: 0x0001FF44
	public override string displayName
	{
		get
		{
			return new StringBuilder(Loc.T("UI.Options.DifficultyLabel")).Append(" ").Append(Loc.T(new StringBuilder("UI.Options.Difficulty").Append(GameStateManager.GlobalValues().difficulty).ToString())).ToString();
		}
	}
}
