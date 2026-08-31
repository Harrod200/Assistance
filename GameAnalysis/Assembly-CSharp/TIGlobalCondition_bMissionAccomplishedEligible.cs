using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000D5 RID: 213
public class TIGlobalCondition_bMissionAccomplishedEligible : TIGlobalCondition
{
	// Token: 0x060003BB RID: 955 RVA: 0x00013684 File Offset: 0x00011884
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060003BC RID: 956 RVA: 0x0001368C File Offset: 0x0001188C
	public override bool PassesCondition(TIGameState state)
	{
		TINationState tinationState = GameStateManager.NationLookup()["2003_USA"];
		TINationState tinationState2 = GameStateManager.NationLookup()["2003_IRQ"];
		bool flag = tinationState != null && tinationState2 != null && tinationState.IsAlliedWith(tinationState2, false) && !tinationState2.atWar;
		return TICondition.PassesComparison(this.sign, flag, TIUtilities.GetBoolValue(this.strValue));
	}
}
