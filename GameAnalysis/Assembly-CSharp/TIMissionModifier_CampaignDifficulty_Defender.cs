using System;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001FC RID: 508
public class TIMissionModifier_CampaignDifficulty_Defender : TIMissionModifier
{
	// Token: 0x060006F0 RID: 1776 RVA: 0x00021D9F File Offset: 0x0001FF9F
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		return TemplateManager.global.AI_GlobalMissionDifficultyModifier_Def(attackingCouncilor, target);
	}

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x060006F1 RID: 1777 RVA: 0x00021DB0 File Offset: 0x0001FFB0
	public override string displayName
	{
		get
		{
			return new StringBuilder(Loc.T("UI.Options.DifficultyLabel")).Append(" ").Append(Loc.T(new StringBuilder("UI.Options.Difficulty").Append(GameStateManager.GlobalValues().difficulty).ToString())).ToString();
		}
	}
}
