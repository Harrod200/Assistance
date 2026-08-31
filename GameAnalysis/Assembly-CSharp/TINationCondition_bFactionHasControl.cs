using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000061 RID: 97
public class TINationCondition_bFactionHasControl : TINationCondition
{
	// Token: 0x06000279 RID: 633 RVA: 0x00010E05 File Offset: 0x0000F005
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x0600027A RID: 634 RVA: 0x00010E0D File Offset: 0x0000F00D
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { GameStateManager.FindByTemplate<TIFactionState>(this.strIdx, false).displayNameWithColor };
		}
	}

	// Token: 0x0600027B RID: 635 RVA: 0x00010E2C File Offset: 0x0000F02C
	public override bool PassesCondition(TIGameState state)
	{
		TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>(this.strIdx, false);
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.executiveFaction == tifactionState, TIUtilities.GetBoolValue(this.strValue));
	}
}
